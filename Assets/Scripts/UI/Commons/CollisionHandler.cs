using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
  private const float PROXIMITY_TRIGGER_DISTANCE = 0.3f;

  public int ATOMS_NECESSARY_TO_FORM_MOLECULE;

  private Transform MainCameraTransform;

  public Dictionary<string, GameObject> AtomsByName { get; private set; }

  private readonly Dictionary<string, AtomCommand> CommandByAtomName = new();

  private readonly Queue DestroyBondedAtomsQueue = new();

  private readonly Dictionary<string, float> ElapsedTimeByAtomName = new();

  private GameObject WaterAnimation;

  public Material HidrogenOnBondMaterial;

  public Material SodiumOnBondMaterial;

  private Molecule Molecule;

  private bool IsShowingElement;

  void Start()
  {
    AtomsByName = new Dictionary<string, GameObject>();

    MainCameraTransform = GameObject.FindWithTag("MainCamera").transform;

    WaterAnimation = GameObject.FindWithTag("WaterAnimation");
    WaterAnimation.GetComponent<Renderer>().enabled = false;
  }

  // TODO - style - on camera proximity
  // TODO - style - keep eletrons rotating in a fixed axis
  // TODO - style - change border when forming molecule
  // TODO - style - make atoms go to opposite poles when forming molecules
  void Update()
  {
    lock (AtomsByName)
    {
      // refactor - make a queue of Maps (string hidrogenName, Commmand command) for commands and iterate through it
      foreach (GameObject atom in AtomsByName.Values)
      {
        // Debug.Log($"Executing {CommandByHidrogenName[hidrogen.name]} for hidrogen {hidrogen.name}");
        HandleAtomCommand(CommandByAtomName[atom.name], atom);
      }

      if (ShouldShowElement())
      {
        HandleElementRendering();
        IsShowingElement = true;
      }
      else
      {
        HandleMoleculeRendering();
        IsShowingElement = false;
      }


      while (DestroyBondedAtomsQueue.Count > 0)
      {
        string atomName = (string)DestroyBondedAtomsQueue.Dequeue();
        DestroyAtom(atomName);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (IsMoleculeFormed()) return;

    if (RenderHandler.GetRendererEnabledValue(other.transform) == false) return;

    if (other.gameObject.CompareTag("Untagged") || other.gameObject.CompareTag("CardPlane")) return;

    if (other != null && !AtomsByName.ContainsKey(other.gameObject.tag))
    {
      AtomsByName.Add(other.gameObject.tag, IntantiateNewSphere(other.transform, other.gameObject.tag, transform.parent.transform));
      RenderHandler.ChangeSelfIncludingChildren(other.transform, false);
      CommandByAtomName[other.gameObject.tag] = AtomCommand.MoveToBond;

      RenderHandler.ChangeChildrenIncludingChildren(transform, false);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    CommandByAtomName[other.gameObject.tag] = AtomCommand.MoveToTarget;
  }

  private GameObject IntantiateNewSphere(Transform original, string name, Transform parent)
  {
    GameObject sphereModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    sphereModel.transform.localScale = new Vector3(original.localScale.x, original.localScale.y, original.localScale.z);

    if (name.StartsWith("Hidrogen")) sphereModel.GetComponent<Renderer>().material = HidrogenOnBondMaterial;
    else if (name.StartsWith("Sodium")) sphereModel.GetComponent<Renderer>().material = SodiumOnBondMaterial;

    GameObject sphere = Instantiate(sphereModel, original.position, transform.rotation, parent);
    sphere.name = name;

    Destroy(sphereModel, 0f);

    return sphere;
  }

  private void DestroyAtom(string atomName)
  {
    RenderHandler.ChangeSelfIncludingChildren(GameObject.FindWithTag(atomName).transform, true);
    GameObject atom = AtomsByName[atomName];
    AtomsByName.Remove(atomName);
    Destroy(atom, 0f);
  }

  private bool ShouldApproximateCustomAtom(GameObject customAtom)
  {
    return Vector3.Distance(customAtom.transform.position, transform.position) >= Transition.GetAtomsDistanceToFormMolecule(customAtom.transform, transform);
  }

  private void ApproximateTo(GameObject @object, Vector3 toPosition, float maxDistanceToGetClose = 0f)
  {
    if (ElapsedTimeByAtomName.ContainsKey(@object.name) == false)
    {
      ElapsedTimeByAtomName.Add(@object.name, 0f);
    }
    else if (ElapsedTimeByAtomName[@object.name] >= Transition.DurationInSeconds)
    {
      ElapsedTimeByAtomName[@object.name] = 0f;
    }

    float easedValue = Transition.GetEasedValue(ElapsedTimeByAtomName[@object.name], Time.deltaTime);

    Vector3 currentPosition = @object.transform.position;
    float distanceToTarget = Vector3.Distance(currentPosition, toPosition);

    // Ensure the distance moved is no closer than max units from the target
    float maxDistanceToMove = Mathf.Min(easedValue, distanceToTarget - maxDistanceToGetClose);
    Vector3 newPosition = Vector3.MoveTowards(currentPosition, toPosition, maxDistanceToMove);

    @object.transform.position = newPosition;

    ElapsedTimeByAtomName[@object.name] += Time.deltaTime;
  }

  private bool HasCustomAtomReachedImageTarget(GameObject @object)
  {
    return Vector3.Distance(@object.transform.position, GameObject.FindWithTag(@object.name).transform.position) <= 0.001;
  }

  private void HandleAtomCommand(AtomCommand command, GameObject atom)
  {
    switch (command)
    {
      case AtomCommand.MoveToBond:
        ApproximateTo(atom, transform.position, Transition.GetAtomsDistanceToFormMolecule(atom.transform, transform));
        if (!ShouldApproximateCustomAtom(atom))
        {
          ElapsedTimeByAtomName[atom.name] = 0f;
          CommandByAtomName[atom.name] = AtomCommand.KeepBonded;

          if (IsMoleculeFormed())
          {
            SetMoleculeType();
          }
        }
        break;
      case AtomCommand.MoveToTarget:
        Vector3 imageTargetObjectPosition = GameObject.FindWithTag(atom.name).transform.position;
        ApproximateTo(atom, imageTargetObjectPosition);

        if (HasCustomAtomReachedImageTarget(atom))
        {
          ElapsedTimeByAtomName[atom.name] = 0f;
          CommandByAtomName[atom.name] = AtomCommand.QueueToDestroy;

          if (!HasAnyAtomBonded()) RenderHandler.ChangeChildrenIncludingChildren(transform, true);
        }
        break;
      case AtomCommand.QueueToDestroy:
        DestroyBondedAtomsQueue.Enqueue(atom.name);
        break;
    }
  }

  private bool IsMoleculeFormed()
  {
    int atomsBonded = 0;

    foreach (GameObject atomBonded in AtomsByName.Values)
    {
      if (CommandByAtomName[atomBonded.name] == AtomCommand.KeepBonded || CommandByAtomName[atomBonded.name] == AtomCommand.MoveToBond)
      {
        atomsBonded++;
      }
    }

    return atomsBonded >= ATOMS_NECESSARY_TO_FORM_MOLECULE;
  }

  public bool HasAnyAtomBonded()
  {
    return CommandByAtomName.ContainsValue(AtomCommand.KeepBonded);
  }

  private bool ShouldShowElement()
  {
    if (MainCameraTransform == null)
    {
      return false;
    }

    bool hasReachedTriggerDistance = Vector3.Distance(transform.position, MainCameraTransform.position) <= PROXIMITY_TRIGGER_DISTANCE;

    return hasReachedTriggerDistance && IsMoleculeFormed();
  }

  private void HandleElementRendering()
  {
    RenderHandler.ChangeSiblingsIncludingChildren(transform, false, "CardPlane");

    if (Molecule == Molecule.H2O)
    {
      GetComponent<Renderer>().enabled = false;
      WaterAnimation.GetComponent<Renderer>().enabled = true;
    }
    else if (Molecule == Molecule.NaCl)
    {
      // GetComponent<Renderer>().material.color = Color.white;
    }
    else if (Molecule == Molecule.NaOH)
    {
      // GetComponent<Renderer>().material.color = Color.blue;
    }
    else if (Molecule == Molecule.HCl)
    {
      // GetComponent<Renderer>().material.color = Color.black;
    }
  }

  private void HandleMoleculeRendering()
  {
    WaterAnimation.GetComponent<Renderer>().enabled = false;

    if (IsShowingElement)
    {
      GetComponent<Renderer>().enabled = true;
      RenderHandler.ChangeSiblingsIncludingChildren(transform, true);
    }
  }

  private void SetMoleculeType()
  {
    if (transform.name.StartsWith("Oxigen"))
    {
      if (AtomsByName.ContainsKey("Hidrogen1") && AtomsByName.ContainsKey("Hidrogen2"))
      {
        Molecule = Molecule.H2O;
      }
      else if (AtomsByName.ContainsKey("Sodium") && (AtomsByName.ContainsKey("Hidrogen1") || AtomsByName.ContainsKey("Hidrogen2")))
      {
        Molecule = Molecule.NaOH;
      }
    }
    else if (transform.name.StartsWith("Chlorine"))
    {
      if (AtomsByName.ContainsKey("Sodium"))
      {
        Molecule = Molecule.NaCl;
      }
      else if (AtomsByName.ContainsKey("Hidrogen1") || AtomsByName.ContainsKey("Hidrogen2"))
      {
        Molecule = Molecule.HCl;
      }
    }
  }
}

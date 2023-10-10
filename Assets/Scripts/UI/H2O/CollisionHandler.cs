using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
  private const float PROXIMITY_TRIGGER_DISTANCE = 0.3f;

  private const int ATOMS_NECESSARY_TO_FORM_MOLECULE = 2;

  private Transform MainCameraTransform;

  private readonly Dictionary<string, GameObject> HidrogensByName = new();

  private readonly Dictionary<string, AtomCommand> CommandByHidrogenName = new();

  private readonly Queue DestroyHidrogenQueue = new();

  private readonly Dictionary<string, float> ElapsedTimeByHidrogenName = new();

  void Start()
  {
    MainCameraTransform = GameObject.FindWithTag("MainCamera").transform;
  }

  // TODO - style - on camera proximity
  // TODO - fix - create unique marcador for each atom and render its image target on the card
  // TODO - style - change border when forming molecule
  // TODO - style - make atoms go to opposite poles when forming molecules
  void Update()
  {
    lock (HidrogensByName)
    {
      // refactor - make a queue of Maps (string hidrogenName, Commmand command) for commands and iterate through it
      foreach (GameObject hidrogen in HidrogensByName.Values)
      {
        // Debug.Log($"Executing {CommandByHidrogenName[hidrogen.name]} for hidrogen {hidrogen.name}");
        HandleAtomCommand(CommandByHidrogenName[hidrogen.name], hidrogen);
      }

      Action renderAction = ShouldShowElement() ? new Action(HandleElementRendering) : new Action(HandleMoleculeRendering);
      renderAction.Invoke();

      while (DestroyHidrogenQueue.Count > 0)
      {
        string hidrogenName = (string)DestroyHidrogenQueue.Dequeue();
        GameObject.FindWithTag(hidrogenName).GetComponent<Renderer>().enabled = true;
        GameObject hidrogen = HidrogensByName[hidrogenName];
        HidrogensByName.Remove(hidrogenName);
        Destroy(hidrogen, 0f);
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other != null && !other.gameObject.CompareTag("Untagged") && !HidrogensByName.ContainsKey(other.gameObject.tag))
    {
      HidrogensByName.Add(other.gameObject.tag, IntantiateNewSphere(other.transform.position, other.gameObject.tag, GameObject.FindWithTag("OxigenioTarget").transform));
      other.GetComponent<Renderer>().enabled = false;
      CommandByHidrogenName[other.gameObject.tag] = AtomCommand.MoveToBond;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    CommandByHidrogenName[other.gameObject.tag] = AtomCommand.MoveToTarget;
  }

  private GameObject IntantiateNewSphere(Vector3 position, string name, Transform parent)
  {
    GameObject sphereModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    sphereModel.GetComponent<Renderer>().material.color = Color.green;
    sphereModel.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
    GameObject sphere = Instantiate(sphereModel, position, transform.rotation, parent);
    sphere.name = name;

    Destroy(sphereModel, 0f);

    return sphere;
  }

  private bool ShouldApproximateCustomHidrogen(GameObject customHidrogen)
  {
    return Vector3.Distance(customHidrogen.transform.position, transform.position) >= 0.0225;
  }

  private void ApproximateTo(GameObject @object, Vector3 toPosition)
  {
    if (ElapsedTimeByHidrogenName.ContainsKey(@object.name) == false)
    {
      ElapsedTimeByHidrogenName.Add(@object.name, 0f);
    }
    else if (ElapsedTimeByHidrogenName[@object.name] >= Transition.DurationInSeconds)
    {
      ElapsedTimeByHidrogenName[@object.name] = 0f;
    }

    float easedValue = Transition.GetEasedValue(ElapsedTimeByHidrogenName[@object.name], Time.deltaTime);

    @object.transform.position = Vector3.MoveTowards(@object.transform.position, toPosition, easedValue);

    ElapsedTimeByHidrogenName[@object.name] += Time.deltaTime;
  }

  private bool HasCustomHidrogenReachedImageTarget(GameObject @object)
  {
    Debug.Log($"Distance: {Vector3.Distance(@object.transform.position, GameObject.FindWithTag(@object.name).transform.position)}");
    return Vector3.Distance(@object.transform.position, GameObject.FindWithTag(@object.name).transform.position) <= 0.001;
  }

  private void HandleAtomCommand(AtomCommand command, GameObject hidrogen)
  {
    switch (command)
    {
      case AtomCommand.MoveToBond:
        ApproximateTo(hidrogen, transform.position);
        if (!ShouldApproximateCustomHidrogen(hidrogen))
        {
          ElapsedTimeByHidrogenName[hidrogen.name] = 0f;
          CommandByHidrogenName[hidrogen.name] = AtomCommand.KeepBonded;
        }
        break;
      case AtomCommand.MoveToTarget:
        Vector3 imageTargetObjectPosition = GameObject.FindWithTag(hidrogen.name).transform.position;
        ApproximateTo(hidrogen, imageTargetObjectPosition);
        if (HasCustomHidrogenReachedImageTarget(hidrogen))
        {
          ElapsedTimeByHidrogenName[hidrogen.name] = 0f;
          CommandByHidrogenName[hidrogen.name] = AtomCommand.QueueToDestroy;
        }
        break;
      case AtomCommand.QueueToDestroy:
        DestroyHidrogenQueue.Enqueue(hidrogen.name);
        break;
    }
  }

  private bool IsMoleculeFormed()
  {
    int atomsBonded = 0;

    foreach (GameObject hidrogenBonded in HidrogensByName.Values)
    {
      if (CommandByHidrogenName[hidrogenBonded.name] == AtomCommand.KeepBonded)
      {
        atomsBonded++;
      }
    }

    return atomsBonded >= ATOMS_NECESSARY_TO_FORM_MOLECULE;
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
    GetComponent<Renderer>().material.color = Color.blue;
  }

  private void HandleMoleculeRendering()
  {
    GetComponent<Renderer>().material.color = Color.white;
  }
}

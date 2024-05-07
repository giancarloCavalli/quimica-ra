using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TakerAtom : Atom
{
    private Dictionary<string, AtomCommand> _commandByAtomName;

    private Dictionary<string, GameObject> _clonedAtomNameAndOriginalAtomRef = new();

    private GameObject _bondedAtomsContainer;

    private Molecule _moleculeType = Molecule.None;

    private readonly Dictionary<string, float> _elapsedTimeByAtomName = new();

    private readonly Queue<string> _destroyBondedAtomsQueue = new();

    void Start()
    {
        _commandByAtomName = new Dictionary<string, AtomCommand>();
        _bondedAtomsContainer = transform.Find("BondedAtomsContainer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _bondedAtomsContainer.transform.childCount; i++)
        {
            HandleAtomCommand(_commandByAtomName[_bondedAtomsContainer.transform.GetChild(i).name], _bondedAtomsContainer.transform.GetChild(i).gameObject);
        }

        while (_destroyBondedAtomsQueue.Count > 0)
        {
            string atomName = _destroyBondedAtomsQueue.Dequeue();
            DestroyAtom(atomName);
        }
    }

    public bool CanBondWith(Atom other)
    {
        return AtomHelpers.CanBond(this, other);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!AtomHelpers.IsAtom(other.transform))
        {
            return;
        }

        if (!CanBondWith(other.GetComponent<Atom>()))
        {
            return;
        }

        StartBondingWith(other.transform);
    }

    public void OnTriggerExit(Collider other)
    {
        AtomCard atomCard = other.GetComponent<AtomCard>();
        if (atomCard != null)
        {
            string atomName = atomCard.AtomCardVariant.ToString();
            StartUnbondingWith(atomName);
        }
    }

    private void StartBondingWith(Transform other)
    {
        // Esconde os eletrons e o canvas do atomo Taker
        ElectronsContainer.SetActive(false);
        Canvas.SetActive(false);

        // Adiciona comando de mover para ligar
        GameObject newAtom = CloneAtom(other, GetAtomName(other.name), _bondedAtomsContainer.transform, other.GetComponent<Atom>().Type);
        _commandByAtomName[newAtom.name] = AtomCommand.MoveToBond;

        // Salva clone e referencia ao game object original
        _clonedAtomNameAndOriginalAtomRef.Add(newAtom.name, other.gameObject);

        // Esconde o atomo ligante original para exibir o clonado
        other.gameObject.SetActive(false);
    }

    private void StartUnbondingWith(string atomName)
    {
        _commandByAtomName[atomName] = AtomCommand.MoveToTarget;
    }

    private string GetAtomName(string atomName)
    {
        int qtOfAtomsOfSameType = 1;

        // Checa se já existe, por exemplo, um Hydrogen1 para então no caso de mais um criar o Hydrogen2
        foreach (var child in _bondedAtomsContainer.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains(atomName))
            {
                qtOfAtomsOfSameType++;
            }
        }

        return atomName + qtOfAtomsOfSameType;
    }

    private GameObject CloneAtom(Transform original, string cloneName, Transform parent, AtomType atomType)
    {
        GameObject sphereModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        Vector3 ajustedScale = new Vector3(original.localScale.x / transform.localScale.x, original.localScale.y / transform.localScale.y, original.localScale.z / transform.localScale.z);
        sphereModel.transform.localScale = ajustedScale;

        sphereModel.GetComponent<Renderer>().material = GameObject.FindWithTag("AtomMaterials").GetComponent<BondMaterials>().GetMaterial(atomType);

        GameObject sphere = Instantiate(sphereModel, original.position, transform.rotation, parent);
        sphere.name = cloneName;

        Destroy(sphereModel, 0f);

        return sphere;
    }

    private void HandleAtomCommand(AtomCommand command, GameObject atom)
    {
        // Debug.Log("Handling atom command: " + command + " for atom: " + atom.name);
        switch (command)
        {
            case AtomCommand.MoveToBond:
                Transition.ApproximateTo(atom, transform.position, _elapsedTimeByAtomName);

                if (!AtomHelpers.ShouldApproximateClonedAtom(transform, atom))
                {
                    _elapsedTimeByAtomName[atom.name] = 0f;
                    _commandByAtomName[atom.name] = AtomCommand.KeepBonded;

                    if (IsMoleculeFormed())
                    {
                        _moleculeType = MoleculeClassifier.GetMoleculeBasedOn(transform, _commandByAtomName.Keys.ToList());
                    }
                }
                break;
            case AtomCommand.MoveToTarget:
                Vector3 originalAtomPosition = _clonedAtomNameAndOriginalAtomRef[atom.name].transform.position;
                Transition.ApproximateTo(atom, originalAtomPosition, _elapsedTimeByAtomName);

                float distanceBetweenCloneAndOriginal = Vector3.Distance(originalAtomPosition, atom.transform.position);
                if (distanceBetweenCloneAndOriginal <= 0.001)
                {
                    _elapsedTimeByAtomName[atom.name] = 0f;
                    _commandByAtomName[atom.name] = AtomCommand.QueueToDestroy;

                    if (!HasAnyAtomBonded())
                    {
                        ElectronsContainer.SetActive(true);
                        Canvas.SetActive(true);
                    }
                }
                break;
            case AtomCommand.QueueToDestroy:
                _destroyBondedAtomsQueue.Enqueue(atom.name);
                break;
            default:
                break;
        }
    }

    private bool IsMoleculeFormed()
    {
        int atomsBonded = 0;

        foreach (AtomCommand command in _commandByAtomName.Values)
        {
            if (command == AtomCommand.KeepBonded)
            {
                atomsBonded++;
            }
        }

        // TODO refatorar
        return Type switch
        {
            AtomType.Oxygen => atomsBonded >= 2,
            AtomType.Chlorine => atomsBonded >= 1,
            _ => false,
        };
    }

    private bool HasAnyAtomBonded()
    {
        return _commandByAtomName.ContainsValue(AtomCommand.KeepBonded);
    }

    private void DestroyAtom(string atomName)
    {
        // Remove o atomo da lista de clones e referencias ao original
        _clonedAtomNameAndOriginalAtomRef[atomName].SetActive(true);
        _clonedAtomNameAndOriginalAtomRef.Remove(atomName);

        // Remove o atomo da lista de comandos
        _commandByAtomName.Remove(atomName);

        // Tira a classificação de molécula caso houver
        _moleculeType = Molecule.None;

        // Destrói o objeto clonado do grafo de cena
        Destroy(_bondedAtomsContainer.transform.Find(atomName).gameObject, 0f);
    }
}

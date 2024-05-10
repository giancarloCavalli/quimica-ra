using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TakerAtom : Atom
{
    private const float PROXIMITY_TRIGGER_DISTANCE = 0.3f;

    public int NR_OF_ATOMS_NECESSARY_TO_FORM_MOLECULE;

    private Dictionary<string, AtomCommand> _commandByAtomName;

    private readonly Dictionary<string, GameObject> _clonedAtomNameAndOriginalAtomRef = new();

    private GameObject _bondedAtomsContainer;

    private Molecule _moleculeType = Molecule.None;

    private readonly Dictionary<string, float> _elapsedTimeByAtomName = new();

    private readonly Queue<string> _destroyBondedAtomsQueue = new();

    private readonly Queue<KeyValuePair<Molecule, bool>> _elementActiveCommandQueue = new();

    private List<GiverAtom> BondedGiverAtoms => _clonedAtomNameAndOriginalAtomRef.Values.Select(atomGameObj => atomGameObj.GetComponent<GiverAtom>()).ToList();

    private ElementObjects _elementObjects;

    private bool _isElementStatusActive = false;

    void Start()
    {
        _commandByAtomName = new Dictionary<string, AtomCommand>();
        _bondedAtomsContainer = transform.Find("BondedAtomsContainer").gameObject;

        _elementObjects = GameObject.FindWithTag("ElementObjects").GetComponent<ElementObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _bondedAtomsContainer.transform.childCount; i++)
        {
            Transform bondedAtomTransform = _bondedAtomsContainer.transform.GetChild(i);
            HandleAtomCommand(_commandByAtomName[bondedAtomTransform.name], bondedAtomTransform.gameObject);
        }

        // Se deveria estar mostrando o elemento E não está mostrando, adiciona comando para mostrar
        if (ShouldShowElement() && !_isElementStatusActive)
        {
            HandleElementActiveCommand(_moleculeType, true);
            _isElementStatusActive = true;
        }
        // Se não deveria estar mostrando o elemento E está mostrando, adiciona comando para esconder
        else if (!ShouldShowElement() && _isElementStatusActive)
        {
            HandleElementActiveCommand(_moleculeType, false);
            _isElementStatusActive = false;
        }

        while (_destroyBondedAtomsQueue.Count > 0)
        {
            string atomName = _destroyBondedAtomsQueue.Dequeue();
            DestroyAtom(atomName);
        }
    }

    public bool CanBondWith(GiverAtom giverAtom)
    {
        if (!this.IsTracked || !giverAtom.IsTracked) return false;
        if (HasReachedBondedAtomsCountLimit()) return false;

        return AtomHelpers.BondingPossibilities.ContainsKey(this.Type) && AtomHelpers.BondingPossibilities[this.Type].Contains(giverAtom.Type);
    }

    public void OnTriggerEnter(Collider other)
    {
        GiverAtom giverAtom = other.GetComponent<GiverAtom>();

        if (giverAtom == null || !CanBondWith(giverAtom))
        {
            return;
        }

        StartBondingWith(giverAtom);
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

    private void StartBondingWith(GiverAtom giverAtom)
    {
        // Desativa os eletrons e o canvas do atomo Taker
        ElectronsContainer.SetActive(false);
        Canvas.SetActive(false);

        // Adiciona comando de mover para ligar
        GameObject newAtom = CloneAtom(giverAtom.AtomCard, _bondedAtomsContainer.transform);
        _commandByAtomName[newAtom.name] = AtomCommand.MoveToBond;

        // Salva clone e referencia ao game object original
        _clonedAtomNameAndOriginalAtomRef.Add(newAtom.name, giverAtom.gameObject);

        // Desativa o atomo ligante original para exibir o clonado
        giverAtom.gameObject.SetActive(false);
    }

    private void StartUnbondingWith(string atomName)
    {
        Debug.Log("Unbonding with: " + atomName);
        _commandByAtomName[atomName] = AtomCommand.MoveToTarget;
    }

    private GameObject CloneAtom(AtomCard giverAtomCard, Transform parent)
    {
        GameObject sphereModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        Transform giverAtomTransform = giverAtomCard.AtomGameObject.transform;

        Vector3 giverAtomAdjustedScale = AtomHelpers.GetAjustedVectorForAtom(giverAtomTransform);
        Vector3 takerAtomAdjustedScale = AtomHelpers.GetAjustedVectorForAtom(transform);

        Vector3 ajustedScale = new(giverAtomAdjustedScale.x / takerAtomAdjustedScale.x, giverAtomAdjustedScale.y / takerAtomAdjustedScale.y, giverAtomAdjustedScale.z / takerAtomAdjustedScale.z);
        sphereModel.transform.localScale = ajustedScale;

        AtomType atomType = giverAtomCard.Atom.Type;
        sphereModel.GetComponent<Renderer>().material = GameObject.FindWithTag("AtomMaterials").GetComponent<BondMaterials>().GetMaterial(atomType);

        GameObject sphere = Instantiate(sphereModel, giverAtomTransform.position, transform.rotation, parent);
        sphere.name = giverAtomCard.AtomCardVariant.ToString();

        Destroy(sphereModel, 0f);

        return sphere;
    }

    private void HandleAtomCommand(AtomCommand command, GameObject clonedAtom)
    {
        switch (command)
        {
            case AtomCommand.MoveToBond:
                float maxDistance = AtomHelpers.GetSumOfRadiusOfTakerAndGiverAtom(this, clonedAtom.transform);
                Transition.ApproximateTo(clonedAtom, transform.position, _elapsedTimeByAtomName, maxDistance);

                if (!AtomHelpers.ShouldApproximateClonedAtom(this, clonedAtom))
                {
                    _elapsedTimeByAtomName[clonedAtom.name] = 0f;
                    _commandByAtomName[clonedAtom.name] = AtomCommand.KeepBonded;

                    if (HasReachedBondedAtomsCountLimit())
                    {
                        _moleculeType = MoleculeClassifier.GetMoleculeBasedOn(this, BondedGiverAtoms);
                    }
                }
                break;
            case AtomCommand.MoveToTarget:
                Vector3 originalAtomPosition = _clonedAtomNameAndOriginalAtomRef[clonedAtom.name].transform.position;
                Transition.ApproximateTo(clonedAtom, originalAtomPosition, _elapsedTimeByAtomName);

                float distanceBetweenCloneAndOriginal = Vector3.Distance(originalAtomPosition, clonedAtom.transform.position);
                if (distanceBetweenCloneAndOriginal <= 0.001)
                {
                    _elapsedTimeByAtomName[clonedAtom.name] = 0f;
                    _commandByAtomName[clonedAtom.name] = AtomCommand.QueueToDestroy;

                    if (!HasAnyAtomBonded())
                    {
                        ElectronsContainer.SetActive(true);
                        Canvas.SetActive(true);
                    }
                }
                break;
            case AtomCommand.QueueToDestroy:
                _destroyBondedAtomsQueue.Enqueue(clonedAtom.name);
                break;
            default:
                break;
        }
    }

    private bool HasReachedBondedAtomsCountLimit()
    {
        int atomsBonded = _commandByAtomName.Values.Count(command => command == AtomCommand.KeepBonded);
        return atomsBonded >= NR_OF_ATOMS_NECESSARY_TO_FORM_MOLECULE;
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

    private void HandleElementActiveCommand(Molecule molecule, bool active)
    {
        switch (molecule)
        {
            case Molecule.H2O:
                // Seta active para animação da água
                _elementObjects.GetObjectFor(molecule).SetActive(active);
                break;
            default:
                // Obtem o painel do elemento, seta o material e ativa o painel
                GameObject elementPanel = _elementObjects.GetPanelFor(this.AtomCard.AtomCardVariant);
                elementPanel.GetComponent<Renderer>().material = _elementObjects.GetMaterialFor(molecule);
                elementPanel.SetActive(active);
                break;
        }

        // Se ativar o elemento, desativa o objeto do atomo
        if (active == true)
        {
            //TODO mudar para SetActive
            GetComponent<Renderer>().enabled = false;
        }
        // Se desativar o elemento, ativa o objeto do atomo (POR GARANTIA, se false, desativa todos)
        else
        {
            //TODO mudar para SetActive
            GetComponent<Renderer>().enabled = true;
        }
    }

    private bool ShouldShowElement()
    {
        if (Camera.main.transform == null)
        {
            return false;
        }

        bool hasReachedTriggerDistance = Vector3.Distance(transform.position, Camera.main.transform.position) <= PROXIMITY_TRIGGER_DISTANCE;

        return hasReachedTriggerDistance && _moleculeType != Molecule.None;
    }
}
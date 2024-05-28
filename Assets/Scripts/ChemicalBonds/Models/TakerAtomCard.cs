using UnityEngine;

public class TakerAtomCard : AtomCard
{
    private const float PROXIMITY_TRIGGER_DISTANCE = 0.32f;
    private bool _isElementStatusActive = false;
    private ElementObjects _elementObjects;
    public TakerAtom TakerAtom
    {
        get => (TakerAtom)Atom;
    }

    void Start()
    {
        _elementObjects = GameObject.FindWithTag("ElementObjects").GetComponent<ElementObjects>();
    }

    void Update()
    {
        // Se deveria estar mostrando o elemento E não está mostrando, aciona rotina
        // para mostrar e atualiza status
        if (ShouldShowElement() && !_isElementStatusActive)
        {
            HandleElementActiveCommand(TakerAtom.MoleculeType, true);
            _isElementStatusActive = true;
        }
        // Se não deveria estar mostrando o elemento E está mostrando, aciona rotina
        // para esconder e atualiza status
        else if (!ShouldShowElement() && _isElementStatusActive)
        {
            HandleElementActiveCommand(TakerAtom.MoleculeType, false);
            _isElementStatusActive = false;
        }
    }

    private bool ShouldShowElement()
    {
        if (Camera.main.transform == null)
        {
            return false;
        }

        bool hasReachedTriggerDistance = Vector3.Distance(transform.position, Camera.main.transform.position) <= PROXIMITY_TRIGGER_DISTANCE;

        return hasReachedTriggerDistance && TakerAtom.MoleculeType != Molecule.None;
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
                GameObject elementPanel = _elementObjects.GetPanelFor(this.AtomCardVariant);
                elementPanel.GetComponent<Renderer>().material = _elementObjects.GetMaterialFor(molecule);
                elementPanel.SetActive(active);
                break;
        }

        // Se ativar o elemento, desativa o objeto do atomo
        if (active == true)
        {
            this.AtomGameObject.SetActive(false);
        }
        // Se desativar o elemento, ativa o objeto do atomo (POR GARANTIA, se false, desativa todos)
        else
        {
            this.AtomGameObject.SetActive(true);
        }
    }
}

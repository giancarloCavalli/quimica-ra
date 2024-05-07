using System.Collections.Generic;
using UnityEngine;

public class TakerAtom : Atom
{
    private Dictionary<string, AtomCommand> _commandByAtomName;

    private GameObject _bondedAtomsContainer;

    // Start is called before the first frame update
    void Start()
    {
        _commandByAtomName = new Dictionary<string, AtomCommand>();
        _bondedAtomsContainer = transform.Find("BondedAtomsContainer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

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

        BondWith(other.transform);

        Debug.Log("Trigger enter with: " + other.gameObject.name);
    }

    public void OnTriggerExit(Collider other)
    {

    }

    private void BondWith(Transform other)
    {
        // Adiciona comando de mover para ligar
        _commandByAtomName[GetAtomName(other.name)] = AtomCommand.MoveToBond;
        CloneAtom(other, GetAtomName(other.name), _bondedAtomsContainer.transform, other.GetComponent<Atom>().Type);
        other.gameObject.SetActive(false);
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
}

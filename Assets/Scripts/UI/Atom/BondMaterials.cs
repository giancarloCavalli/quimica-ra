using UnityEngine;

public class BondMaterials : MonoBehaviour
{
    public Material HydrogenMaterial;
    public Material SodiumMaterial;

    public Material GetMaterial(AtomType type)
    {
        Debug.Log("Get material for " + type);

        return type switch
        {
            AtomType.Hydrogen => HydrogenMaterial,
            AtomType.Sodium => SodiumMaterial,
            _ => throw new System.NotSupportedException("Unsupported atom type")
        };
    }
}

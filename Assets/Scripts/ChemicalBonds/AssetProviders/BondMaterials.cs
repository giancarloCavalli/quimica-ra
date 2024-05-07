using UnityEngine;

public class BondMaterials : MonoBehaviour
{
    public Material HydrogenMaterial;
    public Material SodiumMaterial;

    public Material GetMaterial(AtomType type)
    {
        return type switch
        {
            AtomType.Hydrogen => HydrogenMaterial,
            AtomType.Sodium => SodiumMaterial,
            _ => throw new System.NotSupportedException("Unsupported atom type")
        };
    }
}

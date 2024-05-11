using UnityEngine;

public class ElementObjects : MonoBehaviour
{
    public GameObject Water;
    public Material Salt;
    public Material MuriaticAcid;
    public Material SodiumHydroxide;
    public GameObject OxygenPanel;
    public GameObject ChlorinePanel;

    public void Start()
    {
        Water.SetActive(false);
        OxygenPanel.SetActive(false);
        ChlorinePanel.SetActive(false);
    }

    public GameObject GetObjectFor(Molecule molecule)
    {
        return molecule switch
        {
            Molecule.H2O => Water,
            _ => throw new System.NotSupportedException("Unsupported molecule type")
        };
    }

    public Material GetMaterialFor(Molecule molecule)
    {
        return molecule switch
        {
            Molecule.NaCl => Salt,
            Molecule.HCl => MuriaticAcid,
            Molecule.NaOH => SodiumHydroxide,
            _ => throw new System.NotSupportedException("Unsupported molecule type")
        };
    }

    public GameObject GetPanelFor(CardVariant cardVariant)
    {
        return cardVariant switch
        {
            CardVariant.Oxygen => OxygenPanel,
            CardVariant.Chlorine => ChlorinePanel,
            _ => throw new System.NotSupportedException("Unsupported atom type")
        };
    }
}

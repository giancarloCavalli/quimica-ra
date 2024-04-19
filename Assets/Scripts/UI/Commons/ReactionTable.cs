using System.Collections.Generic;
using UnityEngine;

public class ReactionTable : MonoBehaviour
{
    private readonly Dictionary<int, Molecule> _tableSlots = new Dictionary<int, Molecule>();

    public GameObject NaOHPanel;
    public GameObject HClPanel;
    public GameObject NaClPanel;
    public GameObject H2OAnimation;

    public GameObject Slot1Anchor;
    public GameObject Slot2Anchor;
    public GameObject Slot3Anchor;
    public GameObject Slot4Anchor;

    void Start()
    {
        NaOHPanel.SetActive(false);
        HClPanel.SetActive(false);
        NaClPanel.SetActive(false);
        H2OAnimation.SetActive(false);
    }

    void Update()
    {

    }

    public void HandleCollision(Molecule molecule, TableSide tableSide)
    {
        molecule = Molecule.NaCl;
        PlaceMolecule(molecule, tableSide);
    }

    public void PlaceMolecule(Molecule molecule, TableSide tableSide)
    {
        int slot = GetFreeSlotPosition(tableSide);
        if (slot == -1)
        {
            Debug.Log("Não há slots disponíveis");
            return;
        }

        GameObject moleculeObject = GetMoleculeObject(molecule);

        moleculeObject.transform.position = GetSlotPosition(slot);
        moleculeObject.SetActive(true);
    }

    private GameObject GetMoleculeObject(Molecule molecule)
    {
        return molecule switch
        {
            Molecule.H2O => H2OAnimation,
            Molecule.NaOH => NaOHPanel,
            Molecule.HCl => HClPanel,
            Molecule.NaCl => NaClPanel,
            _ => null,
        };

    }

    private int GetFreeSlotPosition(TableSide tableSide)
    {
        int slot;
        if (tableSide == TableSide.Left)
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_tableSlots.ContainsKey(i))
                {
                    slot = i;
                    return slot;
                }
            }
        }
        else
        {
            for (int i = 2; i < 4; i++)
            {
                if (!_tableSlots.ContainsKey(i))
                {
                    slot = i;
                    return slot;
                }
            }
        }

        // Não há slots disponíveis
        return -1;
    }

    private Vector3 GetSlotPosition(int slot)
    {
        return slot switch
        {
            0 => Slot1Anchor.transform.position,
            1 => Slot2Anchor.transform.position,
            2 => Slot3Anchor.transform.position,
            3 => Slot4Anchor.transform.position,
            _ => Vector3.zero,
        };

    }
}

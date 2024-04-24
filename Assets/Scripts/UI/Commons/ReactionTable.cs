using System.Collections.Generic;
using UnityEngine;

public class ReactionTable : MonoBehaviour
{
    private readonly Dictionary<int, Molecule> _tableSlots = new();

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

    // void Update()
    // {

    // }

    public void HandleCollision(Molecule molecule, TableSide tableSide)
    {
        // molecule = Molecule.H2O;
        HandleMoleculePosicioning(molecule, tableSide);
    }

    public void HandleMoleculePosicioning(Molecule molecule, TableSide tableSide)
    {
        TableSide moleculeTableSide = GetMoleculeTableSide(molecule);

        // Molécula já está no lado solicitado da mesa
        if (moleculeTableSide == tableSide)
        {
            return;
        }
        // Molécula está do outro lado da mesa
        else if (moleculeTableSide != TableSide.None && moleculeTableSide != tableSide)
        {
            RemoveMoleculeFromTable(molecule);
        }

        int slot = GetFreeSlotPosition(tableSide);
        // Não há slots disponíveis neste lado da mesa
        if (slot == -1)
        {
            return;
        }

        AddMoleculeToTable(molecule, tableSide);
    }

    private void RemoveMoleculeFromTable(Molecule molecule)
    {
        int slotOccupiedByMolecule = GetSlotPositionByMolecule(molecule);
        if (slotOccupiedByMolecule == -1)
        {
            return;
        }

        GetMoleculeObject(_tableSlots[slotOccupiedByMolecule]).SetActive(false);
        _tableSlots.Remove(slotOccupiedByMolecule);
    }

    private void AddMoleculeToTable(Molecule molecule, TableSide tableSide)
    {
        GameObject moleculeObject = GetMoleculeObject(molecule);
        int slot = GetFreeSlotPosition(tableSide);

        _tableSlots.Add(slot, molecule);
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
            _ => throw new System.Exception("Slot inválido"),
        };

    }

    private TableSide GetMoleculeTableSide(Molecule molecule)
    {
        foreach (KeyValuePair<int, Molecule> slot in _tableSlots)
        {
            if (slot.Value == molecule)
            {
                return GetTableSideBySlot(slot.Key);
            }
        }
        return TableSide.None;
    }

    private int GetSlotPositionByMolecule(Molecule molecule)
    {
        foreach (KeyValuePair<int, Molecule> slot in _tableSlots)
        {
            if (slot.Value == molecule)
            {
                return slot.Key;
            }
        }

        return -1;
    }

    private TableSide GetTableSideBySlot(int slot)
    {
        return slot switch
        {
            0 => TableSide.Left,
            1 => TableSide.Left,
            2 => TableSide.Right,
            3 => TableSide.Right,
            _ => throw new System.Exception("Slot inválido"),
        };
    }
}

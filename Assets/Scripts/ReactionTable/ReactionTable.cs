using System.Collections.Generic;
using UnityEngine;

public class ReactionTable : MonoBehaviour
{
    private readonly Dictionary<Molecule, int> _slotByMolecule = new();

    public GameObject NaOHPanel;
    public GameObject HClPanel;
    public GameObject NaClPanel;
    public GameObject H2OAnimation;

    public GameObject Slot1Anchor;
    public GameObject Slot2Anchor;
    public GameObject Slot3Anchor;
    public GameObject Slot4Anchor;

    public GameObject Slot1AnchorLow;
    public GameObject Slot2AnchorLow;
    public GameObject Slot3AnchorLow;
    public GameObject Slot4AnchorLow;

    public GameObject CorrectAnswerText;
    public GameObject WrongAnswerText;
    public GameObject ResetTableText;

    void Start()
    {
        NaOHPanel.SetActive(false);
        HClPanel.SetActive(false);
        NaClPanel.SetActive(false);
        H2OAnimation.SetActive(false);

        CorrectAnswerText.SetActive(false);
        WrongAnswerText.SetActive(false);
        ResetTableText.SetActive(false);
    }

    public void HandleCollision(Molecule molecule, TableSide tableSide, bool resetCommand = false)
    {
        if (resetCommand && IsReactionDone())
        {
            ResetTable();
            return;
        }
        else if (molecule == Molecule.None)
        {
            return;
        }

        HandleMoleculePositioning(molecule, tableSide);
    }

    public void HandleMoleculePositioning(Molecule molecule, TableSide tableSide)
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

        int slot = GetFreeSlotPositionOn(tableSide);
        // Não há slots disponíveis neste lado da mesa
        if (slot == -1)
        {
            return;
        }

        AddMoleculeToTable(molecule, tableSide);

        if (_slotByMolecule.Count == 4)
        {
            PresentReactionResult();
        }
    }

    private void RemoveMoleculeFromTable(Molecule molecule)
    {
        int slotOccupiedByMolecule = GetSlotPositionByMolecule(molecule);
        if (slotOccupiedByMolecule == -1)
        {
            return;
        }

        GetMoleculeObjectFor(molecule).SetActive(false);
        _slotByMolecule.Remove(molecule);
    }

    private void AddMoleculeToTable(Molecule molecule, TableSide tableSide)
    {
        GameObject moleculeObject = GetMoleculeObjectFor(molecule);
        int slot = GetFreeSlotPositionOn(tableSide);

        _slotByMolecule.Add(molecule, slot);
        moleculeObject.transform.position = GetSlotPosition(slot, molecule);
        moleculeObject.SetActive(true);
    }

    private GameObject GetMoleculeObjectFor(Molecule molecule)
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

    private int GetFreeSlotPositionOn(TableSide tableSide)
    {
        int slot;
        if (tableSide == TableSide.Left)
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_slotByMolecule.ContainsValue(i))
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
                if (!_slotByMolecule.ContainsValue(i))
                {
                    slot = i;
                    return slot;
                }
            }
        }

        // Não há slots disponíveis
        return -1;
    }

    private Vector3 GetSlotPosition(int slot, Molecule molecule)
    {
        if (molecule == Molecule.H2O)
        {
            return slot switch
            {
                0 => Slot1AnchorLow.transform.position,
                1 => Slot2AnchorLow.transform.position,
                2 => Slot3AnchorLow.transform.position,
                3 => Slot4AnchorLow.transform.position,
                _ => throw new System.Exception("Slot inválido"),
            };
        }

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
        if (_slotByMolecule.ContainsKey(molecule))
        {
            return GetTableSideBySlot(_slotByMolecule[molecule]);
        }
        return TableSide.None;
    }

    private int GetSlotPositionByMolecule(Molecule molecule)
    {
        if (_slotByMolecule.ContainsKey(molecule))
        {
            return _slotByMolecule[molecule];
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

    private void PresentReactionResult()
    {
        TableSide h2oSide = GetMoleculeTableSide(Molecule.H2O);
        TableSide naohSide = GetMoleculeTableSide(Molecule.NaOH);
        TableSide hclSide = GetMoleculeTableSide(Molecule.HCl);
        TableSide naclSide = GetMoleculeTableSide(Molecule.NaCl);

        bool isCorrect = (naohSide == TableSide.Left && hclSide == TableSide.Left && h2oSide == TableSide.Right && naclSide == TableSide.Right);

        if (isCorrect)
        {
            CorrectAnswerText.SetActive(true);
        }
        else
        {
            WrongAnswerText.SetActive(true);
        }

        ResetTableText.SetActive(true);
    }

    private bool IsReactionDone()
    {
        return _slotByMolecule.Count >= 4;
    }

    private void ResetTable()
    {
        Molecule[] molecules = new Molecule[_slotByMolecule.Count];
        _slotByMolecule.Keys.CopyTo(molecules, 0);

        foreach (Molecule molecule in molecules)
        {
            RemoveMoleculeFromTable(molecule);
        }

        CorrectAnswerText.SetActive(false);
        WrongAnswerText.SetActive(false);
        ResetTableText.SetActive(false);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleculeClassifier
{
    public static Molecule GetMoleculeBasedOn(TakerAtom takerAtom, List<GiverAtom> giverAtoms)
    {
        List<AtomType> atomTypes = giverAtoms.Select(giverAtom => giverAtom.Type).ToList();

        return takerAtom.Type switch
        {
            AtomType.Oxygen => GetMoleculeBasedOnOxigen(atomTypes),
            AtomType.Chlorine => GetMoleculeBasedOnChlorine(atomTypes),
            _ => Molecule.None,
        };
    }

    private static Molecule GetMoleculeBasedOnOxigen(List<AtomType> atomTypes)
    {
        // Checa se há 2 hidrogenios
        if (atomTypes.Count(atomType => atomType == AtomType.Hydrogen) == 2)
        {
            return Molecule.H2O;
        }
        else if (atomTypes.Contains(AtomType.Sodium) && atomTypes.Contains(AtomType.Hydrogen))
        {
            return Molecule.NaOH;
        }

        return Molecule.None;
    }

    private static Molecule GetMoleculeBasedOnChlorine(List<AtomType> atomTypes)
    {
        if (atomTypes.Contains(AtomType.Sodium))
        {
            return Molecule.NaCl;
        }
        else if (atomTypes.Contains(AtomType.Hydrogen) || atomTypes.Contains(AtomType.Hydrogen))
        {
            return Molecule.HCl;
        }

        return Molecule.None;
    }

    public static Molecule GetMoleculeBasedOn(Transform transform, List<string> atomsBonded)
    {
        return transform.name switch
        {
            "Oxigen" => GetMoleculeBasedOnOxigen(atomsBonded),
            "Chlorine" => GetMoleculeBasedOnChlorine(atomsBonded),
            _ => Molecule.None,
        };
    }

    private static Molecule GetMoleculeBasedOnOxigen(List<string> atomsBonded)
    {
        if (atomsBonded.Contains("Hidrogen1") && atomsBonded.Contains("Hidrogen2"))
        {
            return Molecule.H2O;
        }
        else if (atomsBonded.Contains("Sodium") && (atomsBonded.Contains("Hidrogen1") || atomsBonded.Contains("Hidrogen2")))
        {
            return Molecule.NaOH;
        }
        else
        {
            return Molecule.None;
        }
    }

    private static Molecule GetMoleculeBasedOnChlorine(List<string> atomsBonded)
    {
        if (atomsBonded.Contains("Sodium"))
        {
            return Molecule.NaCl;
        }
        else if (atomsBonded.Contains("Hidrogen1") || atomsBonded.Contains("Hidrogen2"))
        {
            return Molecule.HCl;
        }
        else
        {
            return Molecule.None;
        }
    }
}

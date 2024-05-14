using System.Collections.Generic;
using System.Linq;

public class MoleculeClassifier
{
    public static Molecule GetMoleculeBasedOn(TakerAtom takerAtom, List<GiverAtom> giverAtoms)
    {
        List<AtomType> atomTypes = giverAtoms.Select(giverAtom => giverAtom.Type).ToList();

        return takerAtom.Type switch
        {
            AtomType.Oxygen => GetMoleculeBasedOnOxygen(atomTypes),
            AtomType.Chlorine => GetMoleculeBasedOnChlorine(atomTypes),
            _ => Molecule.None,
        };
    }

    private static Molecule GetMoleculeBasedOnOxygen(List<AtomType> atomTypes)
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
}

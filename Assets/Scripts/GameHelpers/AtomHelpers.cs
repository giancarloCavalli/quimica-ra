using System.Collections.Generic;
using UnityEngine;

public static class AtomHelpers
{
    private static Dictionary<AtomType, List<AtomType>> _bondingPossibilities = new()
    {
        { AtomType.Oxygen, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } },
        { AtomType.Chlorine, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } }
    };

    public static bool CanBond(Atom taker, Atom giver)
    {
        if (!taker.IsTracked || !giver.IsTracked)
        {
            return false;
        }

        return _bondingPossibilities.ContainsKey(taker.Type) && _bondingPossibilities[taker.Type].Contains(giver.Type);
    }

    public static bool IsAtom(Transform transform)
    {
        return transform.GetComponent<Atom>() != null;
    }
}

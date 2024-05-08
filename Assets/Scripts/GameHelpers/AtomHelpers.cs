using System.Collections.Generic;
using UnityEngine;

public static class AtomHelpers
{
    private static Dictionary<AtomType, List<AtomType>> _bondingPossibilities = new()
    {
        { AtomType.Oxygen, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } },
        { AtomType.Chlorine, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } }
    };

    public static bool CanBond(TakerAtom taker, GiverAtom giver)
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

    public static bool ShouldApproximateClonedAtom(Transform self, GameObject clonedAtom)
    {
        return Vector3.Distance(clonedAtom.transform.position, self.position) >= GetSumOfRadiusOfTakerAndGiverAtom(self, clonedAtom.transform);
    }

    public static float GetSumOfRadiusOfTakerAndGiverAtom(Transform taker, Transform giver)
    {
        float ajustedScaleOfGiver = giver.localScale.x * taker.localScale.x;
        return (taker.localScale.x / 2) + (ajustedScaleOfGiver / 2);
    }

    public static Vector3 GetAjustedVectorForGiverAtom(Transform giverAtomTransform)
    {
        return new Vector3(giverAtomTransform.localScale.x * giverAtomTransform.transform.parent.localScale.x, giverAtomTransform.localScale.y * giverAtomTransform.transform.parent.localScale.y, giverAtomTransform.localScale.z * giverAtomTransform.transform.parent.localScale.z);
    }
}

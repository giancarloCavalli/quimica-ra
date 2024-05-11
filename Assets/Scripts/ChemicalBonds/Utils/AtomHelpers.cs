using System.Collections.Generic;
using UnityEngine;

public static class AtomHelpers
{
    public static Dictionary<AtomType, List<AtomType>> BondingPossibilities = new()
    {
        { AtomType.Oxygen, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } },
        { AtomType.Chlorine, new List<AtomType> { AtomType.Hydrogen, AtomType.Sodium } }
    };

    public static bool ShouldApproximateClonedAtom(TakerAtom takerAtom, GameObject clonedAtom)
    {
        return Vector3.Distance(clonedAtom.transform.position, takerAtom.transform.position) >= GetSumOfRadiusOfTakerAndGiverAtom(takerAtom, clonedAtom.transform);
    }

    public static float GetSumOfRadiusOfTakerAndGiverAtom(TakerAtom takerAtom, Transform giver)
    {
        // Necessario pegar a escala ajustada para esse calculo devido ao fato do Atom possuir um elemento pai
        Vector3 takerPosition = takerAtom.GetVector3WithAdjustedScale();
        float ajustedScaleOfGiver = giver.localScale.x * takerPosition.x;
        return (takerPosition.x / 2) + (ajustedScaleOfGiver / 2);
    }

    public static Vector3 GetAjustedVectorForAtom(Transform atomTransform)
    {
        return new Vector3(atomTransform.localScale.x * atomTransform.transform.parent.localScale.x, atomTransform.localScale.y * atomTransform.transform.parent.localScale.y, atomTransform.localScale.z * atomTransform.transform.parent.localScale.z);
    }
}

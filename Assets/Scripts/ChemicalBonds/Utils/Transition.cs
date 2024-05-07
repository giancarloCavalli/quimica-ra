using System.Collections.Generic;
using UnityEngine;

public static class Transition
{
    public static float MinSpeed { get; private set; } = 0.06f;

    public static float MaxSpeed { get; private set; } = 0.8f;

    public static float DurationInSeconds { get; private set; } = 1.6f;

    public static float GetEasedValue(float elapsedTime, float deltaTime)
    {
        float t = Mathf.SmoothStep(0f, 1f, elapsedTime / DurationInSeconds);
        return Mathf.Lerp(MinSpeed, MaxSpeed, t) * deltaTime;
    }

    public static float GetAtomsDistanceToFormMolecule(Transform atomA, Transform atomB)
    {
        return (atomA.localScale.x / 2) + (atomB.localScale.x / 2);
    }

    public static void ApproximateTo(GameObject @object, Vector3 toPosition, Dictionary<string, float> elapsedTimeByAtomName, float maxDistanceToGetClose = 0f)
    {
        if (elapsedTimeByAtomName.ContainsKey(@object.name) == false)
        {
            elapsedTimeByAtomName.Add(@object.name, 0f);
        }
        else if (elapsedTimeByAtomName[@object.name] >= DurationInSeconds)
        {
            elapsedTimeByAtomName[@object.name] = 0f;
        }

        float easedValue = GetEasedValue(elapsedTimeByAtomName[@object.name], Time.deltaTime);

        Vector3 currentPosition = @object.transform.position;
        float distanceToTarget = Vector3.Distance(currentPosition, toPosition);

        float maxDistanceToMove = Mathf.Min(easedValue, distanceToTarget - maxDistanceToGetClose);

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, toPosition, maxDistanceToMove);

        @object.transform.position = newPosition;

        elapsedTimeByAtomName[@object.name] += Time.deltaTime;
    }
}
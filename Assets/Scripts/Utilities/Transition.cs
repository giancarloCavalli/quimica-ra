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
}
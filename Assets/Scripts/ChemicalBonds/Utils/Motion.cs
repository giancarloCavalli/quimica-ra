using UnityEngine;

public static class Motion
{
  public static void Rotate(GameObject objectToRotate, GameObject referenceObject, float degreesPerSecond)
  {
    objectToRotate.transform.RotateAround(referenceObject.transform.position, Vector3.up, degreesPerSecond * Time.deltaTime);
  }
}
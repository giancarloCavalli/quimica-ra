using UnityEngine;

public static class Motion
{
  public static void Rotate(GameObject objectToRotate, GameObject referenceObject, float degreesPerSecond)
  {
    // Obtém o vetor up no espaço local do objeto
    Vector3 localUp = objectToRotate.transform.InverseTransformDirection(Vector3.up);

    // Rotaciona em relação ao vetor up local
    objectToRotate.transform.RotateAround(referenceObject.transform.position, localUp, degreesPerSecond * Time.deltaTime);
  }
}
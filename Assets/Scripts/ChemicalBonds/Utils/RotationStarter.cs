using UnityEngine;

public class RotationStarter : MonoBehaviour
{
  void Update()
  {
    Motion.Rotate(transform.gameObject, transform.parent.gameObject, 90f);
  }
}

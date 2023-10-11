using UnityEngine;

public class RotationStarter : MonoBehaviour
{
  void Start()
  {
  }

  void Update()
  {
    Motion.Rotate(transform.gameObject, transform.parent.gameObject, 90f);
  }
}

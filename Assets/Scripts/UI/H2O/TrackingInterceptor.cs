using UnityEngine;
using Vuforia;

public class TrackingInterceptor : MonoBehaviour
{
  public ImageTargetBehaviour HidrogenioTarget;

  void Start()
  {
    if (HidrogenioTarget != null)
    {
      HidrogenioTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }
  }

  private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    GameObject imageTargetChild = HidrogenioTarget.transform.GetChild(0).gameObject;
    if (status.Status == Status.TRACKED)
    {
      // Debug.Log($"MY TAG IS {imageTargetChild.tag}");
      foreach (Transform child in GameObject.FindWithTag("OxigenioTarget").transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          // Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          RenderHandler.ChangeIncludingChildren(imageTargetChild.transform, false);
        }
      }
    }
  }

  void Update()
  { }
}

using UnityEngine;
using Vuforia;

public class TrackingInterceptor : MonoBehaviour
{
  public ImageTargetBehaviour ImageTarget;

  public GameObject OxigenTarget;

  public GameObject ChlorineTarget;

  void Start()
  {
    if (ImageTarget != null)
    {
      ImageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }
  }

  private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    GameObject imageTargetChild = ImageTarget.transform.GetChild(0).gameObject;
    if (status.Status == Status.TRACKED)
    {
      // Debug.Log($"MY TAG IS {imageTargetChild.tag}");

      foreach (Transform child in OxigenTarget.transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          // Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          RenderHandler.ChangeSelfIncludingChildren(imageTargetChild.transform, false);
        }
      }

      foreach (Transform child in ChlorineTarget.transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          // Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          RenderHandler.ChangeSelfIncludingChildren(imageTargetChild.transform, false);
        }
      }
    }
  }
}

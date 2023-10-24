using UnityEngine;
using Vuforia;

public class TrackingInterceptor : MonoBehaviour
{
  public ImageTargetBehaviour ImageTarget;

  public GameObject ParentTargetWhenBonded;

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
      foreach (Transform child in ParentTargetWhenBonded.transform)
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

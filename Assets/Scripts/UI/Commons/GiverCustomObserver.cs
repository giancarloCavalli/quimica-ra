using UnityEngine;
using Vuforia;

public class GiverCustomObserver : MonoBehaviour
{
  private readonly ObserverBehaviour mObserverBehaviour;

  private ImageTargetBehaviour mImageTargetBehaviour;

  public GameObject OxigenTarget;

  public GameObject ChlorineTarget;

  void Awake()
  {
    RenderHandler.ChangeChildrenIncludingChildren(transform, false);

    if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
      mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;

    mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();
  }
  void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    if (status.Status == Status.TRACKED)
    {
      // Debug.Log($"MY TAG IS {imageTargetChild.tag}");
      RenderHandler.ChangeSiblingsIncludingChildren(imageTargetChild.transform, true);
      bool shouldRender = true;

      foreach (Transform child in OxigenTarget.transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          // Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          shouldRender = false;
          continue;
        }
      }

      foreach (Transform child in ChlorineTarget.transform)
      {
        if (imageTargetChild.tag == child.name)
        {
          // Debug.Log($"Found clone. Hiding {imageTargetChild.tag}");
          shouldRender = false;
          continue;
        }
      }

      RenderHandler.ChangeSelfIncludingChildren(imageTargetChild.transform, shouldRender);
    }

    else
    {
      RenderHandler.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, false);
    }
  }
  void OnDestroy()
  {
    if (mObserverBehaviour != null)
      mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
  }
}
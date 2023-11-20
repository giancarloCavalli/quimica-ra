using UnityEngine;
using Vuforia;

public class StatusEventHandler : MonoBehaviour
{
  private readonly ObserverBehaviour mObserverBehaviour;

  private ImageTargetBehaviour mImageTargetBehaviour;

  public GameObject OxigenTarget;

  public GameObject ChlorineTarget;

  void Awake()
  {
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
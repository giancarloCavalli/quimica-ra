using UnityEngine;
using Vuforia;

public class TakerTargetObserver : MonoBehaviour
{
  private readonly ObserverBehaviour mObserverBehaviour;

  private ImageTargetBehaviour mImageTargetBehaviour;

  private CollisionHandler mCollisionHandler;

  void Awake()
  {
    RenderHandler.ChangeChildrenIncludingChildren(transform, false);

    if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
      mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;

    mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

    mCollisionHandler = transform.GetChild(0).GetComponent<CollisionHandler>();
  }
  void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

    if (status.Status == Status.TRACKED)
    {
      if (mCollisionHandler.HasAnyAtomBonded())
        RenderWithoutEletrons();
      else
        RenderWithEletrons();
    }
    else
    {
      HideAll();
    }
  }
  void OnDestroy()
  {
    if (mObserverBehaviour != null)
      mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
  }

  private void RenderWithoutEletrons()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    imageTargetChild.GetComponent<Renderer>().enabled = true;
    RenderHandler.ChangeSiblingsIncludingChildren(imageTargetChild.transform, true);
  }

  private void RenderWithEletrons()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    RenderHandler.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, true);
  }

  private void HideAll()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    RenderHandler.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, false);
  }
}

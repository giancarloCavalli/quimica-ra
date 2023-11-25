using System.Linq;
using UnityEngine;
using Vuforia;

public class TakerTargetObserver : MonoBehaviour
{
  private readonly ObserverBehaviour mObserverBehaviour;

  private ImageTargetBehaviour mImageTargetBehaviour;

  private CollisionHandler mCollisionHandler;

  void Awake()
  {
    RenderHelper.ChangeChildrenIncludingChildren(transform, false);

    if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
      mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;

    mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

    mCollisionHandler = transform.GetChild(0).GetComponent<CollisionHandler>();
  }
  void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
  {
    Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

    switch (status.Status)
    {
      case Status.TRACKED:
        if (mCollisionHandler.HasAnyAtomBonded())
          RenderWithoutEletrons();
        else
          RenderWithEletrons();
        break;
      case Status.NO_POSE:
        HideAll();

        foreach (string atomName in mCollisionHandler.CommandByAtomName.Keys.ToList())
        {
          mCollisionHandler.CommandByAtomName[atomName] = AtomCommand.QueueToDestroy;
        }
        break;
      default:
        HideAll();
        break;
    }
  }

  private void RenderWithoutEletrons()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    imageTargetChild.GetComponent<Renderer>().enabled = true;
    RenderHelper.ChangeSiblingsIncludingChildren(imageTargetChild.transform, true);
  }

  private void RenderWithEletrons()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    RenderHelper.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, true);
  }

  private void HideAll()
  {
    GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
    RenderHelper.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, false);
  }

  void OnDestroy()
  {
    if (mObserverBehaviour != null)
      mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
  }
}

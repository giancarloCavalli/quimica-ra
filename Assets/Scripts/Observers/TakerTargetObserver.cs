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
        RenderHelper.ChangeChildrenIgnoringTags(transform, false, ElementTags.GetAll());

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
                mCollisionHandler.IsVisible = true;

                if (mCollisionHandler.HasAnyAtomBonded())
                {
                    string[] tagsToIgnore = new string[] { "Oxigen", "Chlorine" };
                    ChangeAllRenderExcept(tagsToIgnore);
                }
                else
                    RenderAll();
                break;
            case Status.NO_POSE:
                mCollisionHandler.IsVisible = false;

                HideAll();

                foreach (string atomName in mCollisionHandler.CommandByAtomName.Keys.ToList())
                {
                    mCollisionHandler.CommandByAtomName[atomName] = AtomCommand.QueueToDestroy;
                }
                break;
            default:
                mCollisionHandler.IsVisible = false;
                HideAll();
                break;
        }
    }

    private void ChangeAllRenderExcept(string[] tagsToIgnore)
    {
        GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
        imageTargetChild.GetComponent<Renderer>().enabled = true;
        RenderHelper.ChangeChildrenIgnoringTags(transform, true, ElementTags.GetAll().Concat(tagsToIgnore).ToArray());
    }

    private void RenderAll()
    {
        RenderHelper.ChangeChildrenIgnoringTags(transform, true, ElementTags.GetAll());
    }

    private void HideAll()
    {
        RenderHelper.ChangeChildrenIgnoringTags(transform, false, ElementTags.GetAll());
    }

    void OnDestroy()
    {
        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
}

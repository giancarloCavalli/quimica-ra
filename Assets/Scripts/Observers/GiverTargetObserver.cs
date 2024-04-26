using UnityEngine;
using Vuforia;

public class GiverTargetObserver : MonoBehaviour
{
    private readonly ObserverBehaviour mObserverBehaviour;

    private ImageTargetBehaviour mImageTargetBehaviour;

    public GameObject OxigenTarget;

    public GameObject ChlorineTarget;

    public GameObject GiverAtomGameObject;

    private GiverAtom _giverAtom;

    void Awake()
    {
        RenderHelper.ChangeChildrenIncludingChildren(transform, false);

        if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;

        mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

        _giverAtom = GiverAtomGameObject.GetComponent<GiverAtom>();
    }
    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

        GameObject imageTargetChild = mImageTargetBehaviour.transform.GetChild(0).gameObject;
        if (status.Status == Status.TRACKED)
        {
            _giverAtom.IsVisible = true;
            // Debug.Log($"MY TAG IS {imageTargetChild.tag}");
            RenderHelper.ChangeSiblingsIncludingChildren(imageTargetChild.transform, true);
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
            RenderHelper.ChangeSelfIncludingChildren(imageTargetChild.transform, shouldRender);
        }

        else
        {
            _giverAtom.IsVisible = false;
            RenderHelper.ChangeSelfAndSiblingsIncludingChildren(imageTargetChild.transform, false);
        }
    }
    void OnDestroy()
    {
        if (mObserverBehaviour != null)
            mObserverBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
}
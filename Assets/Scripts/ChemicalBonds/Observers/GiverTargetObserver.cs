using UnityEngine;
using Vuforia;

public class GiverTargetObserver : MonoBehaviour
{
    private readonly ObserverBehaviour _observerBehaviour;

    public GameObject OxigenTarget;

    public GameObject ChlorineTarget;

    public GameObject AtomGameObject;

    private Atom _atom;

    void Awake()
    {
        if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
        {
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;
        }

        _atom = AtomGameObject.GetComponent<Atom>();
    }
    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

        if (status.Status == Status.TRACKED)
        {
            AtomGameObject.SetActive(true);
            _atom.IsTracked = true;
        }
        else
        {
            AtomGameObject.SetActive(false);
            _atom.IsTracked = false;
        }
    }
    void OnDestroy()
    {
        if (_observerBehaviour != null)
            _observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
}
using UnityEngine;
using Vuforia;

public class TakerTargetObserver : MonoBehaviour
{
    private readonly ObserverBehaviour _observerBehaviour;

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

        switch (status.Status)
        {
            case Status.TRACKED:
                AtomGameObject.SetActive(true);
                _atom.IsTracked = true;
                break;
            case Status.NO_POSE:
                AtomGameObject.SetActive(false);
                _atom.IsTracked = false;
                break;
            default:
                AtomGameObject.SetActive(false);
                _atom.IsTracked = false;
                break;
        }
    }

    void OnDestroy()
    {
        if (_observerBehaviour != null)
            _observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
}

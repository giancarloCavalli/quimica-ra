using UnityEngine;
using Vuforia;

public class TakerTargetObserver : MonoBehaviour
{
    private readonly ObserverBehaviour _observerBehaviour;

    private AtomCard AtomCard
    {
        get => transform.Find("AtomCard").GetComponent<AtomCard>();
    }

    private Atom _atom;

    void Awake()
    {
        if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
        {
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;
        }

        _atom = AtomCard.Atom;
        AtomCard.gameObject.SetActive(false);
    }
    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

        switch (status.Status)
        {
            case Status.TRACKED:
                AtomCard.gameObject.SetActive(true);
                _atom.IsTracked = true;
                break;
            default:
                AtomCard.gameObject.SetActive(false);
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

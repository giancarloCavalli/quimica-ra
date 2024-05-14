using UnityEngine;
using Vuforia;

public class GiverTargetObserver : MonoBehaviour
{
    private readonly ObserverBehaviour _observerBehaviour;

    public GameObject AtomCardGameObject;

    private AtomCard _atomCard;

    void Awake()
    {
        if (TryGetComponent<ObserverBehaviour>(out var mObserverBehaviour))
        {
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;
        }

        _atomCard = AtomCardGameObject.GetComponent<AtomCard>();
        AtomCardGameObject.SetActive(false);
    }
    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.LogFormat("TargetName: {0}, Status is: {1}, StatusInfo is: {2}", behaviour.TargetName, status.Status, status.StatusInfo);

        if (status.Status == Status.TRACKED)
        {
            AtomCardGameObject.SetActive(true);
            _atomCard.Atom.IsTracked = true;
        }
        else
        {
            AtomCardGameObject.SetActive(false);
            _atomCard.Atom.IsTracked = false;
        }
    }
    void OnDestroy()
    {
        if (_observerBehaviour != null)
            _observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }
}
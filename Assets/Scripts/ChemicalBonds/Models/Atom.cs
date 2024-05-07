using UnityEngine;

public class Atom : MonoBehaviour
{
    public AtomType Type;

    public bool IsTracked = false;

    public GameObject ElectronsContainer
    {
        get => transform.Find("ElectronsContainer").gameObject;
    }

    public GameObject Canvas
    {
        get => transform.Find("Canvas").gameObject;
    }
}

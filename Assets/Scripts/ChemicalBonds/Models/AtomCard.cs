using UnityEngine;

public class AtomCard : MonoBehaviour
{
    public CardVariant AtomCardVariant;

    public GameObject AtomGameObject => transform.Find("Atom").gameObject;

    public Atom Atom => AtomGameObject.GetComponent<Atom>();
}

using UnityEngine;

public class Atom : MonoBehaviour
{
    public AtomType Type;

    public bool IsTracked = false;

    public AtomCard AtomCard => transform.parent.GetComponent<AtomCard>();

    public GameObject ElectronsContainer
    {
        get => transform.Find("ElectronsContainer").gameObject;
    }

    public GameObject Canvas
    {
        get => transform.Find("Canvas").gameObject;
    }

    // Considerando que o atomo possue UM pai (AtomCard) para as operacoes de movimentacao e instanciacao de clone é necessário usar essa escala ajustada
    public Vector3 GetVector3WithAdjustedScale()
    {
        return new Vector3(transform.localScale.x * transform.parent.localScale.x, transform.localScale.y * transform.parent.localScale.y, transform.localScale.z * transform.parent.localScale.z);
    }
}

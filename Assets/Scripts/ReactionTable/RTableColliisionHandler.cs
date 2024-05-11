using UnityEngine;

public class RTableCollisionHandler : MonoBehaviour
{
    public GameObject ReactionTableGameObject;

    private ReactionTable _reactionTable;
    public TableSide TableSide;

    void Start()
    {
        _reactionTable = ReactionTableGameObject.GetComponent<ReactionTable>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<TakerAtomCard>(out var takerAtomCard))
        {
            _reactionTable.HandleCollision(takerAtomCard.TakerAtom.MoleculeType, TableSide);
        }

        // Aproximar um hidrogenio da mesa representa um comando de reset
        AtomCard atomCard = other.gameObject.GetComponent<AtomCard>();
        if (atomCard != null && atomCard.Atom.Type == AtomType.Hydrogen)
        {
            _reactionTable.HandleCollision(Molecule.None, TableSide, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Reaction Table trigger exit");
    }
}

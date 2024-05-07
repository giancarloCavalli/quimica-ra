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
        if (other.tag == "Oxigen" || other.tag == "Chlorine")
        {
            _reactionTable.HandleCollision(other.gameObject.GetComponent<CollisionHandler>().Molecule, TableSide);
        }

        // Aproximar um hidrogenio da mesa representa um comando de reset
        if (other.name.ToLower() == "hidrogen")
        {
            _reactionTable.HandleCollision(Molecule.None, TableSide, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Reaction Table trigger exit");
    }
}

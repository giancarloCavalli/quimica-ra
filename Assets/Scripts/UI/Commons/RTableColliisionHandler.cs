using UnityEngine;

public class RTableCollisionHandler : MonoBehaviour
{
    public GameObject ReactionTableGameObject;

    private ReactionTable ReactionTable;
    public TableSide TableSide;

    void Start()
    {
        ReactionTable = ReactionTableGameObject.GetComponent<ReactionTable>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Oxigen" || other.tag == "Chlorine")
        {
            ReactionTable.HandleCollision(other.gameObject.GetComponent<CollisionHandler>().Molecule, TableSide);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Reaction Table trigger exit");
    }
}

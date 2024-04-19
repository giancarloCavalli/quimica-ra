using UnityEngine;

public class RTableCollisionHandler : MonoBehaviour
{
    private CollisionHandler OxigenCollisionHandler;
    private CollisionHandler SodiumCollisionHandler;

    public GameObject ReactionTableGameObject;

    private ReactionTable ReactionTable;
    public TableSide TableSide;

    void Start()
    {
        OxigenCollisionHandler = GameObject.FindWithTag("Oxigen").GetComponent<CollisionHandler>();
        SodiumCollisionHandler = GameObject.FindWithTag("Sodium").GetComponent<CollisionHandler>();
        ReactionTable = ReactionTableGameObject.GetComponent<ReactionTable>();
    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        ReactionTable.HandleCollision(OxigenCollisionHandler.Molecule, TableSide);
        // if (other.tag == "Oxigen")
        // {
        //     Debug.Log("Oxigen collision");
        //     Debug.Log(oxigenCollisionHandler.Molecule);
        // }
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Reaction Table trigger exit");
    }
}

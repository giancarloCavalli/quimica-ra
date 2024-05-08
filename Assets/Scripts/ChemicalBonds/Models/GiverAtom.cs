public class GiverAtom : Atom
{
    public AtomCard AtomCard => transform.parent.GetComponent<AtomCard>();
}

public abstract class Node
{
    public enum NodeState { SUCCESS, FAILURE, RUNNING }
    protected NodeState state;
    public abstract NodeState Evaluate(BlackboardBase blackboard);
}
public enum StatePriority
{
    Idle = 1,
    Patrol = 2,
    Chase = 3,
    Attack = 4,
    Stun = 5,
    Flee=6,
    Dead = 100
}

// Abstract FSM Base Class
public abstract class FSMBase
{
    protected EnemyBase enemy;
    public StatePriority Priority { get; private set; }

    public FSMBase(EnemyBase enemy, StatePriority priority)
    {
        this.enemy = enemy;
        this.Priority = priority;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
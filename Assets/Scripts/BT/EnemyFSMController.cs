using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSMController
{
    public FSMBase CurrentState { get; private set; }

    public void ChangeState(FSMBase newState)
    {
        if (CurrentState == null || newState.Priority > CurrentState.Priority)
        {
            Debug.Log($"🔁 FSM: Chuyển từ [{CurrentState?.GetType().Name ?? "None"}] → [{newState.GetType().Name}]");

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
        else
        {
            Debug.Log($"⚠️ FSM BLOCK: Không thể chuyển từ [{CurrentState.Priority}] sang [{newState.Priority}] vì thấp ưu tiên.");
        }
    }
    public void ForceChangeState(FSMBase newState)
    {
        Debug.Log($"💥 FSM FORCE: Ép chuyển từ [{CurrentState?.GetType().Name ?? "None"}] → [{newState.GetType().Name}]");

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void Reset()
    {
        CurrentState?.Exit();
        CurrentState = null;
    }
}

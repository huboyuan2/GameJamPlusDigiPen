using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMState
{
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();

    protected StateID stateID;

    public StateID ID => stateID;

    public void AddTransition(Transition trans, StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
        }
        else if (id == StateID.NullStateID)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
        }
        else if (map.ContainsKey(trans))
        {
            Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() + "Impossible to assign to another state");
        }
        else
        {
            map.Add(trans, id);
        }
    }

    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() + " was not on the state's transition list");
    }

    public StateID GetOutputState(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    public virtual void DoBeforeEntering()
    {
    }

    public virtual void DoBeforeLeaving()
    {
    }

    public abstract void Reason(GameObject player, GameObject npc);

    public abstract void Act(GameObject player, GameObject npc);
}

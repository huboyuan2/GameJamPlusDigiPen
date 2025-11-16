using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FsmSystem
{
	private List<FSMState> states;

	private StateID currentStateID;

	private FSMState currentState;

	public StateID CurrentStateID => currentStateID;

	public FSMState CurrentState => currentState;

	public FsmSystem()
	{
		states = new List<FSMState>();
	}

	public void AddState(FSMState s)
	{
		if (s == null)
		{
			Debug.LogError("FSM ERROR: Null reference is not allowed");
		}
		if (states.Count == 0)
		{
			states.Add(s);
			currentState = s;
			currentStateID = s.ID;
			return;
		}
		foreach (FSMState state in states)
		{
			if (state.ID == s.ID)
			{
				Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() + " because state has already been added");
				return;
			}
		}
		states.Add(s);
	}

	public void DeleteState(StateID id)
	{
		if (id == StateID.NullStateID)
		{
			Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
			return;
		}
		foreach (FSMState state in states)
		{
			if (state.ID == id)
			{
				states.Remove(state);
				return;
			}
		}
		Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() + ". It was not on the list of states");
	}

	public void PerformTransition(Transition trans)
	{
		if (trans == Transition.NullTransition)
		{
			Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
			return;
		}
		StateID outputState = currentState.GetOutputState(trans);
		if (outputState == StateID.NullStateID)
		{
			return;
		}
		currentStateID = outputState;
		foreach (FSMState state in states)
		{
			if (state.ID == currentStateID)
			{
				currentState.DoBeforeLeaving();
				currentState = state;
				currentState.DoBeforeEntering();
				break;
			}
		}
	}
}

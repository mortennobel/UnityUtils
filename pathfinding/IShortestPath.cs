using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface for a shortest path problem
/// </summary>
public interface IShortestPath<State,Action> {
	/// <summary>
	/// Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
	/// </summary>
	float Heuristic(State fromLocation, State toLocation);
	
	/// <summary>
	/// Return the legal moves from position
	/// </summary>
	List<Action> Expand(State position);
	
	/// <summary>
	/// Return the actual cost between two adjecent locations
	/// </summary>
	float ActualCost(State fromLocation, Action action);
	
	/// <summary>
	/// Returns the new state after an action has been applied
	/// </summary>
	State ApplyAction(State location, Action action);
}

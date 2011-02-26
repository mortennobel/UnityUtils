using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IShortestPath<State,Action> {
	/**
	 * Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
	 */
	float Heuristic(State fromLocation, State toLocation);
	
	/**
	 * Return the legal moves from position
	 */ 
	List<Action> Expand(State position);
	
	/**
	 * Return the actual cost between two adjecent locations
	 */ 
	float ActualCost(State fromLocation, Action action);
	
	/**
	 * Returns the new state after an action has been applied
	 */ 
	State ApplyAction(State location, Action action);
}

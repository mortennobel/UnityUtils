/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A test case for pathfinding
/// </summary>
public class ShortestPathTest : UUnitTestCase, IShortestPath<Vector2i, Vector2i> {
	private bool moveDiagonal = true;
	public void TestNoObstacles () {
		ShortestPathGraphSearch<Vector2i,Vector2i> search = new ShortestPathGraphSearch<Vector2i,Vector2i>(this);
		List<Vector2i> list = search.GetShortestPath(new Vector2i(1,0), new Vector2i(3,0));
		Assert.Equals(2,list.Count);
		Debug.Log("Test No Obstacles");
		foreach (Vector2i pos in list){
			Debug.Log("Position "+pos);
		}
	}
	
	public void TestObstacles () {
		moveDiagonal = true;
		ShortestPathGraphSearch<Vector2i,Vector2i> search = new ShortestPathGraphSearch<Vector2i,Vector2i>(this);
		List<Vector2i> list = search.GetShortestPath(new Vector2i(-1,0), new Vector2i(1,0));
		Assert.Equals(2,list.Count);
		Debug.Log("Test Obstacles");
		foreach (Vector2i pos in list){
			Debug.Log("Position "+pos);
		}
	}
	
	public void TestObstaclesNoDiagonal () {
		moveDiagonal = false;
		ShortestPathGraphSearch<Vector2i,Vector2i> search = new ShortestPathGraphSearch<Vector2i,Vector2i>(this);
		List<Vector2i> list = search.GetShortestPath(new Vector2i(-1,0), new Vector2i(1,0));
		Assert.Equals(4,list.Count);
		Debug.Log("Test Obstacles");
		foreach (Vector2i pos in list){
			Debug.Log("Position "+pos);
		}
	}
	
	/**
	 * Should return a estimate of shortest distance. The estimate must me admissible (never overestimate)
	 */
	public float Heuristic(Vector2i fromLocation, Vector2i toLocation){
		if (moveDiagonal){
			return (fromLocation-toLocation).magnitude; // return straight line distance
		} else {
			Vector2i res = fromLocation-toLocation;
			return Mathf.Abs(res.x)+Mathf.Abs(res.y); // manhatten-distance
		}
	}
	
	/**
	 * Return the legal moves from position
	 */ 
	public List<Vector2i> Expand(Vector2i state){
		List<Vector2i> res = new List<Vector2i>();
		for (int x=-1;x<=1;x++) for (int y=-1;y<=1;y++){
			if (x==0 && y==0) continue;
			Vector2i action = new Vector2i(x,y);
			Vector2i newState = ApplyAction(state, action);
			if (newState.magnitude==0) continue; // location 0,0 is blocked
			if (!moveDiagonal && x*y!=0) continue; // 
			res.Add(action);
		}
		return res;
	}
	
	/**
	 * Return the actual cost between two adjecent locations
	 */ 
	public float ActualCost(Vector2i fromLocation, Vector2i toLocation){
		return (fromLocation-toLocation).magnitude;
	}
	
	public Vector2i ApplyAction(Vector2i state, Vector2i action){
		return state+action;
	}
}

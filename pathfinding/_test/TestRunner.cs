using UnityEngine;
using System.Collections;

/// <summary>
/// A test runner for pathfinding
/// </summary>
public class TestRunner : MonoBehaviour {
	
	/// <summary>
	/// Initialization
	/// </summary>
	void Start () {
		UUnitTestSuite suite = new UUnitTestSuite();
		suite.Add(typeof(PriorityQueueTest));
		suite.Add(typeof(ShortestPathTest));
		
		UUnitTestResult result = suite.RunAll();
		Debug.Log("Result: "+result.Summary());
	}
}

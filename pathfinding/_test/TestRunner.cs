using UnityEngine;
using System.Collections;

/**
 * A test runner for pathfinding
 * Depends on the UnitTest4U (https://github.com/mortennobel/UnitTest4U)
 */
public class TestRunner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UUnitTestSuite suite = new UUnitTestSuite();
		suite.Add(typeof(PriorityQueueTest));
		suite.Add(typeof(ShortestPathTest));
		
		UUnitTestResult result = suite.RunAll();
		Debug.Log("Result: "+result.Summary());
	}
}

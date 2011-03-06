/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


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

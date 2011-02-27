using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract test-case.
/// Subclass this to create your own unit-tests.
/// Every method starting with Test will be invoked (using reflections)
/// </summary>
public class UUnitTestCase  {
	public virtual void Setup(){}
	
	public virtual void TearDown(){}
}

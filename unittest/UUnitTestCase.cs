/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

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

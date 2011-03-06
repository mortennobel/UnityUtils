/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System.Collections;

/// <summary>
/// A test case for PriorityQueue
/// </summary>
public class PriorityQueueTest  : UUnitTestCase {
	public void TestSimple () {
		PriorityQueue<int,string> testQueue = new PriorityQueue<int,string>();
		Assert.True(testQueue.IsEmpty, "Queue is empty");
		
		testQueue.Enqueue("Test 2",2);
		testQueue.Enqueue("Test 1",1);
		testQueue.Enqueue("Test 3",3);
		string txt = testQueue.Dequeue();
		Assert.Equals("Test 1", txt);
		Assert.False(testQueue.IsEmpty, "");

		txt = testQueue.Dequeue();
		Assert.Equals("Test 2", txt);
		Assert.False(testQueue.IsEmpty, "");
		
		txt = testQueue.Dequeue();
		Assert.Equals("Test 3", txt);
		Assert.True(testQueue.IsEmpty, "");
	}
	
	public void TestReplace(){
		PriorityQueue<int,string> testQueue = new PriorityQueue<int,string>();
		Assert.True(testQueue.IsEmpty, "Queue is empty");
		
		testQueue.Enqueue("Test 2",2);
		testQueue.Enqueue("Test 1",1);
		testQueue.Enqueue("Test 3",3);
		testQueue.Replace("Test 3",3,1);
		testQueue.Replace("Test 1",1,3);
		
		string txt = testQueue.Dequeue();
		Assert.Equals("Test 3", txt);
		Assert.False(testQueue.IsEmpty, "");
		txt = testQueue.Dequeue();
		Assert.Equals("Test 2", txt);
		Assert.False(testQueue.IsEmpty, "");
		txt = testQueue.Dequeue();
		Assert.Equals("Test 1", txt);
		Assert.True(testQueue.IsEmpty, "");
		
	}


}

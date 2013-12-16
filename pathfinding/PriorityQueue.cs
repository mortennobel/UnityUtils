/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Based on http://blogs.msdn.com/b/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx
/// Backported to C# 2.0
/// </summary>
public class PriorityQueue<P, V>
{
    private SortedDictionary<P, LinkedList<V>> list = new SortedDictionary<P, LinkedList<V>>();

    public void Enqueue(V value, P priority)
    {
        LinkedList<V> q;
		if (!list.TryGetValue(priority, out q))
        {
            q = new LinkedList<V>();
            list.Add(priority, q);
        }
        q.AddLast(value);
    }
	
    public V Dequeue()
    {
        // will throw exception if there isn’t any first element!
		SortedDictionary<P, LinkedList<V>>.KeyCollection.Enumerator enume = list.Keys.GetEnumerator();
		enume.MoveNext();
        P key = enume.Current;
		LinkedList<V> v = list[key];
		V res = v.First.Value;
		v.RemoveFirst();
    	if (v.Count == 0){ // nothing left of the top priority.
            list.Remove(key);
		}
		return res;
    }
	
	
	
	public void Replace(V value, P oldPriority, P newPriority){
		LinkedList<V> v = list[oldPriority];
		v.Remove(value);

		if (v.Count == 0){ // nothing left of the top priority.
			list.Remove(oldPriority);
		}

		Enqueue(value, newPriority);
	}
		
    public bool IsEmpty
    {
        get { return list.Count==0; }
    }
	
	public override string ToString() {
		string res = "";
		foreach (P key in list.Keys){
			foreach (V val in list[key]){
				res += val+", ";
			}
		}
		return res;
	}
}
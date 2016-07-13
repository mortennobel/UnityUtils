/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// static kd tree
// idea is that you first insert nodes and then query. It is possible to update tree after construction but manually
// The tree is never rebalance itself
public class KDTree<T> {
	

	public class KDTreeEntry {
		public int splittingAxis;
		public float split;
		public KDTreeEntry left;
		public KDTreeEntry right;
		public List<T> values;

		public void Insert(T face, Bounds b){
			if (values == null){
				if (b.max[splittingAxis] <= split){
					left.Insert(face, b);
				}
				if (b.min[splittingAxis] >= split){
					right.Insert(face, b);
				}
			} else {
				values.Add(face);
			}
		}

		public void Query(Bounds b, List<KDTreeEntry> res){
			if (values == null){
				if (b.max[splittingAxis] <= split){
					left.Query(b, res);
				}
				if (b.min[splittingAxis] >= split){
					right.Query(b, res);
				}
			} else {
				res.Add(this);
			}
		} 
	}
	int depth;
	KDTreeEntry node;

	public bool[] axis = new bool[]{true, false, true};
	public 	KDTree(int depth, Bounds b){
		this.depth = depth;
		CreateTree(b);
	}

	public void Insert(T obj, Bounds b){
		node.Insert(obj, b);
	}

	// Note that objects T may be returned multiple times (in different KDTreeEntry 
	public List<KDTreeEntry> Query(Bounds b){
		List<KDTreeEntry> res = new List<KDTreeEntry>();
		node.Query(b, res);
		return res;
	}

	KDTreeEntry CreateNode(Bounds b, int currentDepth){
		KDTreeEntry entry = new KDTreeEntry();
		if (currentDepth < depth){
			float maxValue = 0;
			for (int i=0;i<3;i++){
				if (b.size[i] > maxValue && axis[i]){
					maxValue = b.size[i];
					entry.splittingAxis = i;
				}
			}

			entry.split = b.center[entry.splittingAxis];
			Vector3 newExtents = b.extents;
			newExtents[entry.splittingAxis] *= 0.5f;
			b.extents = newExtents;
			Vector3 center = b.center;

			center[entry.splittingAxis] = b.center[entry.splittingAxis] - newExtents[entry.splittingAxis];
			Bounds left = b;
			left.center = center;
			center[entry.splittingAxis] = b.center[entry.splittingAxis] + newExtents[entry.splittingAxis];
			Bounds right = b;
			right.center = center;

			entry.left = CreateNode(left,currentDepth+1);
			entry.right = CreateNode(right,currentDepth+1);

		} else {
			entry.values = new List<T>();
		}
		return entry;
	}

	void CreateTree(Bounds bounds){
		node = CreateNode(bounds, 0);
	}
}

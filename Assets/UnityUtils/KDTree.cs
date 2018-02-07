/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using UnityEngine;
using System.Collections.Generic;
using System.IO;

// static kd tree
// idea is that you first insert nodes and then query. It is possible to update tree after construction but manually
// The tree is never rebalances itself
public class KDTree<T> {

    public class KDTreeValue
    {
        public T value;
        public int label;
        public Bounds b;

        public KDTreeValue(T value,Bounds b)
        {
            this.value = value;
            this.b = b;
            label = 0;
        }
    }

    public class KDTreeEntry {
		public int splittingAxis;
		public float split;
		public KDTreeEntry left;
		public KDTreeEntry right;
		public List<KDTreeValue> values;
#if HMDebug
		public Bounds bounds;
#endif

		public void Insert(KDTreeValue val, Bounds b){
			if (values == null){
				if ( split <= b.max[splittingAxis]){
					left.Insert(val, b);
				}
				if (split >= b.min[splittingAxis] ){
					right.Insert(val, b);
				}
			} else {
				values.Add(val);
			}
		}

		public void Query(Bounds b, List<KDTreeEntry> res){
		    if (values == null){
				if (split <= b.max[splittingAxis]){
					left.Query(b, res);
				}
				if (split >= b.min[splittingAxis]){
					right.Query(b, res);
				}
			} else {
				res.Add(this);
			}
		}

        public void Query(Bounds b, List<T> res, int id){
            if (values == null){
                if (split <= b.max[splittingAxis]){
                    left.Query(b, res, id);
                }
                if (split >= b.min[splittingAxis]){
                    right.Query(b, res, id);
                }
            } else
            {
                foreach (KDTreeValue v in values)
                {
                    if (id > v.label)
                    {
                        v.label = id;
                        res.Add(v.value);
                    }
                }
            }
        }

       

        public int Debug(int depth, StringWriter sw, string path)
        {
            int sum = 0;
            for (int i = 0; i < depth; i++)
            {
                sw.Write(" ");
            }
            if (values == null)
            {
                sw.Write(path+" SpitAxis " + this.splittingAxis + " val " + this.split.ToString("R") + "\n");

                sum+=left.Debug(depth + 1, sw, path+"L");
                sum+=right.Debug(depth + 1, sw, path+"R");
            }
            else
            {
                sw.Write(path+"{");
                sum += values.Count;
                foreach (var v in values)
                {
                    sw.Write(v+", ");
                }
                sw.Write("}\n");
            }
            return sum;


        }

        public int ItemCount()
        {
            if (values == null)
            {
                return left.ItemCount() +
                       right.ItemCount();
            }
            else
            {
                return values.Count;
            }
        }

        public void Clear()
        {
            if (left != null)
            {
                left.Clear();
            }
            if (right != null)
            {
                right.Clear();
            }
            if (values != null)
            {
                values.Clear();
            }
        }

        public void All(List<T> res, int id)
        {
            if (values != null){
                foreach (KDTreeValue v in values)
                {
                    if (id > v.label)
                    {
                        v.label = id;
                        res.Add(v.value);
                    }
                }
            }
            if (left != null)
            {
                left.All(res, id);
            }
            if (right != null)
            {
                right.All(res, id);
            }

        }
#if HMDebug
        public void GetFinalBounds(List<Bounds> res)
        {
            if (values != null)
            {
                res.Add(bounds);
            }
            else
            {
                left.GetFinalBounds(res);
                right.GetFinalBounds(res);
            }
        }
#endif
        public bool Intersects(Bounds b)
        {
            if (values == null){
                if (split <= b.max[splittingAxis]){
                    if (left.Intersects(b))
                    {
                        return true;
                    }
                }
                if (split >= b.min[splittingAxis]){
                    if (right.Intersects(b))
                    {
                        return true;
                    }
                }
            } else
            {
                foreach (KDTreeValue v in values)
                {
                    if (v.b.Intersects(b))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
	int depth;
	KDTreeEntry node;
    private int queryId = 0;

	public 	KDTree(int depth, Bounds b){
		this.depth = depth;
	    CreateTree(b);
	}

#if HMDebug
    public List<Bounds> GetFinalBounds()
    {
        List<Bounds> res = new List<Bounds>();
        node.GetFinalBounds(res);
        return res;
    }
#endif

    public string Debug()
    {
        StringWriter sw = new StringWriter();
        sw.Write("KDTree:\n");
        if (node == null)
        {
            sw.Write("null");
        }
        else
        {
            int sum = node.Debug(0,sw,"");
            sw.Write("\nTotal elements "+sum);
        }
        return sw.ToString();
    }

    // May include duplicates
    public int ItemCount()
    {
        return node.ItemCount();
    }

    public void Insert(T obj, Bounds b)
    {
        KDTreeValue v = new KDTreeValue(obj,b);
		node.Insert(v, b);
	}
    
    public bool Intersects(Bounds b)
    {
        if (node == null)
        {
            return false;
        }
        return node.Intersects(b);
    }

	// Note that objects T may be returned multiple times (in different KDTreeEntry 
	public List<KDTreeEntry> Query(Bounds b){
		List<KDTreeEntry> res = new List<KDTreeEntry>();
		node.Query(b, res);
		return res;
	}


    // Note that objects T may be returned multiple times (in different KDTreeEntry
    public List<T> QueryUnique(Bounds b)
    {
        queryId++;
        List<T> res = new List<T>();
        node.Query(b, res, queryId);
        return res;
    }

    // Note that objects T may be returned multiple times (in different KDTreeEntry
    public List<T> AllUnique()
    {
        queryId++;
        List<T> res = new List<T>();
        node.All(res, queryId);
        return res;
    }


    public void Clear()
    {
        if (node != null)
        {
            node.Clear();
        }
    }

    int LargestComponentIndex(Vector3 v)
    {
        float val = v[0];
        int maxIdx = 0;
        for (int i = 1; i < 3; i++)
        {
            if (val < v[i])
            {
                val = v[i];
                maxIdx = i;
            }
        }
        return maxIdx;
    }

    KDTreeEntry CreateNode(Bounds b, int currentDepth){
		KDTreeEntry entry = new KDTreeEntry();
		if (currentDepth < depth)
		{
		    entry.splittingAxis = LargestComponentIndex(b.size);

			entry.split = b.center[entry.splittingAxis];

		    Vector3 newExtents = b.extents;
			newExtents[entry.splittingAxis] *= 0.5f;
			b.extents = newExtents;

			Vector3 leftCenter = b.center;
			Vector3 rightCenter = b.center;

		    leftCenter[entry.splittingAxis] = b.center[entry.splittingAxis] - newExtents[entry.splittingAxis];
		    rightCenter[entry.splittingAxis] = b.center[entry.splittingAxis] + newExtents[entry.splittingAxis];

		    Bounds left = b;
			left.center = leftCenter;
		    Bounds right = b;
			right.center = rightCenter;

		    entry.left = CreateNode(left,currentDepth+1);
		    entry.right = CreateNode(right,currentDepth+1);
		} else {
			entry.values = new List<KDTreeValue>();
#if HMDebug
		    entry.bounds = b;
#endif
		}
		return entry;
	}

	void CreateTree(Bounds bounds){
		node = CreateNode(bounds, 0);
	}
}

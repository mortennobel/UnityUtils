/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LineSegment  {
	public Vector3D first{
		get{ 
			return array [0];
		}
	}
	public Vector3D second{
		get {
			return array[1];
		}
	}
	Vector3D[] array = new Vector3D[2];

	public LineSegment(Vector3D first_, Vector3D second_){
		array[0] = first_;
		array[1] = second_;
	}

	public double Length(){
		return (first - second).magnitude;
	}

	public bool IsSameDirection(LineSegment other){
		Vector3D dirThis = (second - first).normalized;
		Vector3D dirOther = (other.second - other.first).normalized;
		return Vector3D.Dot (dirThis, dirOther) >= 0.99f;
	}

	public bool flag = false;

	public LineSegment2D To2D(int skip = 1){
		if (skip == 1) {
			return new LineSegment2D (new Vector2D (first.x, first.z), new Vector2D (second.x, second.z));
		}
		Debug.LogError ("Not supported");;
		return null;
	}
}

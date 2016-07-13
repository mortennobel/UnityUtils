using UnityEngine;
using System.Collections;

public struct Matrix3x3 {
	// Matrices in unity are column major. Data is accessed as: row + (column*3).
	public float[] data;

	public Matrix3x3(float[] data){
		this.data = new float[9];
		for (int i=0;i<9;i++){
			this.data[i] = data[i];
		}
	}
	//
	// Static Properties
	//
	public static Matrix3x3 identity {
		get{
			return new Matrix3x3(new float[]{
				1,0,0,
				0,1,0,
				0,0,1
			});
		}
	}

	public static Matrix3x3 zero {
		get{
			return new Matrix3x3(new float[]{
				0,0,0,
				0,0,0,
				0,0,0
			});
		}
	}

	//
	// Properties
	//
	public float determinant {
		get{
			var a = this;
			return a[0,0] * (a[2,2] * a[1,1] - a[1,2] * a[2,1])
				+ a[0,1] * (a[1,2] * a[2,0] - a[2,2] * a[1,0])
				+ a[0,2] * (a[2,1] * a[1,0] - a[1,1] * a[2,0]);
		}
	}

	/*public Matrix4x4 inverse {
		get;
	}*/

	public bool isIdentity {
		get{
			return this == identity;
		}
	}

	public Matrix4x4 transpose {
		get {
			Matrix4x4 n = Matrix4x4.zero;
			for (int i=0;i<3;i++){
				for (int j=0;j<3;j++){
					n[j,i] = this[i,j];
				}	
			}
			return n;
		}
	}

	//
	// Indexer
	//
	/*public float this [int index] {
		get;
		set;
	}*/

	public float this [int row, int column] {
		get{
			return data[row + (column*3)];
		}
		set{
			data[row + (column*3)] = value;
		}
	}


	//
	// Methods
	//
	public override bool Equals (object other){
		if (!GetType().IsInstanceOfType(other)){
			return false;
		}
		var mat = (Matrix3x3)other;
		return mat == this;
	}

	public void SetColumn (int column, Vector3 v){
		for (int r=0;r<3;r++){
			this[r,column] = v[r];
		}
	}

	public void SetRow (int r, Vector3 v){
		for (int c=0;c<3;c++){
			this[r,c] = v[c];
		}
	}

	/*public Vector4 GetColumn (int i);

	public override int GetHashCode ();

	public Vector4 GetRow (int i);

	public Vector3 MultiplyPoint (Vector3 v);

	public Vector3 MultiplyPoint3x4 (Vector3 v);

	public Vector3 MultiplyVector (Vector3 v);

	public void SetColumn (int i, Vector4 v);

	public void SetRow (int i, Vector4 v);

	public void SetTRS (Vector3 pos, Quaternion q, Vector3 s);

	public override string ToString ();

	public string ToString (string format);
*/
	//
	// Operators
	//
	public static bool operator == (Matrix3x3 lhs, Matrix3x3 rhs){
		for (int i=0;i<9;i++){
			if (lhs.data[i] != rhs.data[i]){
				return false;
			}
		}
		return true;
	}

	public static bool operator != (Matrix3x3 lhs, Matrix3x3 rhs){
		return !(lhs == rhs);
	}

	//public static Matrix4x4 operator * (Matrix4x4 lhs, Matrix4x4 rhs);

	//public static Vector4 operator * (Matrix4x4 lhs, Vector4 v);
}

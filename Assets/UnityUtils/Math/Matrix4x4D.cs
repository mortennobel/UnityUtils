/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System;

public struct Matrix4x4D {
	// Matrices in unity are column major. Data is accessed as: row + (column*3).
	public double[] data;

    public Matrix4x4D(double[] data){
		this.data = new double[16];
		for (int i=0;i<16;i++){
			this.data[i] = data[i];
		}
	}

    public Matrix4x4D(Vector4D col1, Vector4D col2, Vector4D col3, Vector4D col4)
    {
        this.data = new double[16];
        SetColumn(0,col1);
        SetColumn(1,col2);
        SetColumn(2,col3);
        SetColumn(4,col4);
    }

    //
	// Static Properties
	//
	public static Matrix4x4D identity {
		get{
			return new Matrix4x4D(new double[]{
				1,0,0,0,
				0,1,0,0,
				0,0,1,0,
			    0,0,0,1
			});
		}
	}

	public static Matrix4x4D zero {
		get{
			return new Matrix4x4D(new double[]{
				0,0,0,0,
				0,0,0,0,
				0,0,0,0,
				0,0,0,0
			});
		}
	}

	//
	// Properties
	//
	public double determinant {
		get
		{
		    var m = this;
		    var SubFactor00 = m[2,2] * m[3,3] - m[3,2] * m[2,3];
		    var SubFactor01 = m[2,1] * m[3,3] - m[3,1] * m[2,3];
		    var SubFactor02 = m[2,1] * m[3,2] - m[3,1] * m[2,2];
		    var SubFactor03 = m[2,0] * m[3,3] - m[3,0] * m[2,3];
		    var SubFactor04 = m[2,0] * m[3,2] - m[3,0] * m[2,2];
		    var SubFactor05 = m[2,0] * m[3,1] - m[3,0] * m[2,1];

		    var DetCof = new Vector4D(
		    + (m[1,1] * SubFactor00 - m[1,2] * SubFactor01 + m[1,3] * SubFactor02),
		    - (m[1,0] * SubFactor00 - m[1,2] * SubFactor03 + m[1,3] * SubFactor04),
		    + (m[1,0] * SubFactor01 - m[1,1] * SubFactor03 + m[1,3] * SubFactor05),
		    - (m[1,0] * SubFactor02 - m[1,1] * SubFactor04 + m[1,2] * SubFactor05));

		    return
		        m[0,0] * DetCof[0] + m[0,1] * DetCof[1] +
		        m[0,2] * DetCof[2] + m[0,3] * DetCof[3];
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

	public Matrix4x4D transpose {
		get {
			Matrix4x4D n = Matrix4x4D.zero;
			for (int i=0;i<4;i++){
				for (int j=0;j<4;j++){
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

	public double this [int row, int column] {
		get{
			return data[row + (column*4)];
		}
		set{
			data[row + (column*4)] = value;
		}
	}


	//
	// Methods
	//
	public override bool Equals (object other){
		if (!GetType().IsInstanceOfType(other)){
			return false;
		}
		var mat = (Matrix4x4D)other;
		return mat == this;
	}


	public void SetColumn (int column, Vector4D v){
		for (int r=0;r<4;r++){
			this[r,column] = v[r];
		}
	}

	public void SetRow (int r, Vector4D v){
		for (int c=0;c<4;c++){
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
	public static bool operator == (Matrix4x4D lhs, Matrix4x4D rhs){
		for (int i=0;i<16;i++){
			if (lhs.data[i] != rhs.data[i]){
				return false;
			}
		}
		return true;
	}

	public static bool operator != (Matrix4x4D lhs, Matrix4x4D rhs){
		return !(lhs == rhs);
	}

	//public static Matrix4x4 operator * (Matrix4x4 lhs, Matrix4x4 rhs);

	//public static Vector4 operator * (Matrix4x4 lhs, Vector4 v);

    public override string ToString()
    {
        return String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00}\n{4:0.00} {5:0.00} {6:0.00} {7:0.00}\n{8:0.00} {9:0.00} {10:0.00} {11:0.00}\n{12:0.00} {13:0.00} {14:0.00} {15:0.00}",
            this[0, 0], this[0, 1], this[0, 2],this[0, 3],
            this[1, 0], this[1, 1], this[1, 2],this[1, 3],
            this[2, 0], this[2, 1], this[2, 2],this[2, 3],
            this[3, 0], this[3, 1], this[3, 2],this[3, 3]

            );
    }
}

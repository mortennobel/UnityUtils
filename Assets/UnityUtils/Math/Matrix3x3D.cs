/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System;

public struct Matrix3x3D {
	// Matrices in unity are column major. Data is accessed as: row + (column*3).
	public double[] data;

    public Matrix3x3D(double[] data){
		this.data = new double[9];
		for (int i=0;i<9;i++){
			this.data[i] = data[i];
		}
	}

    public Matrix3x3D(Vector3D col1, Vector3D col2, Vector3D col3)
    {
        this.data = new double[9];
        SetColumn(0,col1);
        SetColumn(1,col2);
        SetColumn(2,col3);
    }

    //
	// Static Properties
	//
	public static Matrix3x3D identity {
		get{
			return new Matrix3x3D(new double[]{
				1,0,0,
				0,1,0,
				0,0,1
			});
		}
	}

	public static Matrix3x3D zero {
		get{
			return new Matrix3x3D(new double[]{
				0,0,0,
				0,0,0,
				0,0,0
			});
		}
	}

	//
	// Properties
	//
	public double determinant {
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

	public Matrix3x3D transpose {
		get {
			Matrix3x3D n = Matrix3x3D.zero;
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

	public double this [int row, int column] {
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
		var mat = (Matrix3x3D)other;
		return mat == this;
	}

	public void SetColumn (int column, Vector3D v){
		for (int r=0;r<3;r++){
			this[r,column] = v[r];
		}
	}

	public void SetRow (int r, Vector3D v){
		for (int c=0;c<3;c++){
			this[r,c] = v[c];
		}
	}

    public Vector3D MultiplyPoint(Vector3D v)
    {
        Vector3D res = new Vector3D();
        for (int r = 0; r < 3; r++)
        {
            double dotSum = 0;
            for (int c = 0; c < 3; c++)
            {
                dotSum += this[r, c] * v[c];
            }
            dotSum += this[r, 3];
            res[r] = dotSum;
        }
        return res;
    }

    public Vector3D MultiplyVector(Vector3D v)
    {
        Vector3D res = new Vector3D();
        for (int r = 0; r < 3; r++)
        {
            double dotSum = 0;
            for (int c = 0; c < 3; c++)
            {
                dotSum += this[r, c] * v[c];
            }
            res[r] = dotSum;
        }
        return res;
    }

    /*public Vector4 GetColumn (int i);

    public override int GetHashCode ();

    public Vector4 GetRow (int i);



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
	public static bool operator == (Matrix3x3D lhs, Matrix3x3D rhs){
		for (int i=0;i<9;i++){
			if (lhs.data[i] != rhs.data[i]){
				return false;
			}
		}
		return true;
	}

	public static bool operator != (Matrix3x3D lhs, Matrix3x3D rhs){
		return !(lhs == rhs);
	}

	//public static Matrix4x4 operator * (Matrix4x4 lhs, Matrix4x4 rhs);

	//public static Vector4 operator * (Matrix4x4 lhs, Vector4 v);

    public override string ToString()
    {
        return String.Format("{0:0.00} {1:0.00} {2:0.00}\n{3:0.00} {4:0.00} {5:0.00}\n{6:0.00} {7:0.00} {8:0.00}",
            this[0, 0], this[0, 1], this[0, 2],
            this[1, 0], this[1, 1], this[1, 2],
            this[2, 0], this[2, 1], this[2, 2]);
    }
}

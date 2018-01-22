using UnityEngine;

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

	public bool isIdentity {
		get{
			return this == identity;
		}
	}

	public Matrix3x3 transpose {
		get {
			Matrix3x3 n = Matrix3x3.zero;
			for (int i=0;i<3;i++){
				for (int j=0;j<3;j++){
					n[j,i] = this[i,j];
				}	
			}
			return n;
		}
	}

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

    public override string ToString()
    {
        return string.Format("{0:0.00} {0:0.00} {0:0.00}\n{0:0.00} {0:0.00} {0:0.00}\n{0:0.00} {0:0.00} {0:0.00}",
            this[0, 0], this[0, 1], this[0, 2],
            this[1, 0], this[1, 1], this[1, 2],
            this[2, 0], this[2, 1], this[2, 2]);
    }
}

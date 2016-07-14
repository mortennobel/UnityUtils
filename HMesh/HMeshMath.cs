using UnityEngine;
using System.Collections;

public static class HMeshMath {
	// determines if the point p3 lies to the left of the line spanned by p1 and p2.
	public static bool LeftOf(Vector3 p1, Vector3 p2, Vector3 p3){
		Matrix3x3 mat = Matrix3x3.zero;
		mat.SetRow(0,new Vector3(1,p1.x, p1.y));
		mat.SetRow(1,new Vector3(1,p2.x, p2.y));
		mat.SetRow(2,new Vector3(1,p3.x, p3.y));
		return mat.determinant > 0;
	}

	// determines if the point p3 lies to the right of the line spanned by p1 and p2.
	public static bool RightOf(Vector3 p1, Vector3 p2, Vector3 p3){
		return LeftOf(p2,p1,p3);
	}

	// InCircle(p1,p2,p3,p4) determines if point p4 lies inside of the circumcircle of points p1, p2 and p3, 
	// where it is assumed that the points p1, p2 and p3 are in counterclockwise order.
	public static bool InCircle(Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4){
		Matrix4x4 m = new Matrix4x4();
		Vector3[] a = new Vector3[]{
			p1,p2,p3,p4
		};
		for (int i=0;i<4;i++){
			m.SetRow(i, new Vector4(a[i].x,a[i].y,a[i].x*a[i].x+a[i].y*a[i].y,1));
		}
		return m.determinant<0;
	}
}

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

public static class HMeshMath {
	// determines if the point p3 lies to the left of the line spanned by p1 and p2.
    public static bool LeftOfXY(Vector3D p1, Vector3D p2, Vector3D p3){
		Matrix3x3D mat = Matrix3x3D.zero;
		mat.SetRow(0,new Vector3D(1,p1.x, p1.y));
		mat.SetRow(1,new Vector3D(1,p2.x, p2.y));
		mat.SetRow(2,new Vector3D(1,p3.x, p3.y));
		return mat.determinant > 0;
	}

    // determines if the point p3 lies to the left of the line spanned by p1 and p2.
	public static bool LeftOfXZ(Vector3D p1, Vector3D p2, Vector3D p3){
		Matrix3x3D mat = Matrix3x3D.zero;
		mat.SetRow(0,new Vector3D(1,p1.x, p1.z));
		mat.SetRow(1,new Vector3D(1,p2.x, p2.z));
		mat.SetRow(2,new Vector3D(1,p3.x, p3.z));
		return mat.determinant > 0;
	}

	// determines if the point p3 lies to the right of the line spanned by p1 and p2.
	public static bool RightOfXY(Vector3D p1, Vector3D p2, Vector3D p3){
		return LeftOfXY(p2,p1,p3);
	}

    // determines if the point p3 lies to the right of the line spanned by p1 and p2.
	public static bool RightOfXZ(Vector3D p1, Vector3D p2, Vector3D p3){
		return LeftOfXZ(p2,p1,p3);
	}

	// InCircle(p1,p2,p3,p4) determines if point p4 lies inside of the circumcircle of points p1, p2 and p3, 
	// where it is assumed that the points p1, p2 and p3 are in counterclockwise order.
	public static bool InCircleXY(Vector3D p1,Vector3D p2,Vector3D p3,Vector3D p4){
		Matrix4x4D m = Matrix4x4D.identity;
		Vector3D[] a = {
			p1,p2,p3,p4
		};
		for (int i=0;i<4;i++){
			m.SetRow(i, new Vector4D(a[i].x,a[i].y,a[i].x*a[i].x+a[i].y*a[i].y,1));
		}
		return m.determinant<0;
	}

    public static bool InCircleXZ(Vector3D p1,Vector3D p2,Vector3D p3,Vector3D p4){
        Matrix4x4D m = Matrix4x4D.identity;
        Vector3D[] a = {
            p1,p2,p3,p4
        };
        for (int i=0;i<4;i++){
            m.SetRow(i, new Vector4D(a[i].x,a[i].z,a[i].x*a[i].x+a[i].z*a[i].z,1));
        }
        return m.determinant<0;
    }
}

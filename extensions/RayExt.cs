using UnityEngine;
using System.Collections;

public static class RayExt {

	public static bool ClosestPoints(this Ray ray, Ray otherRay, out Vector3 point1, out Vector3 point2){
		return ClosestPoints(ray.origin, ray.direction, otherRay.origin, otherRay.direction, out point1, out point2);
	}

	public static bool ClosestPoints(Vector3 rayOrigin, Vector3 rayDirection, Vector3 otherRayOrigin, Vector3 otherRayDirection, out Vector3 point1, out Vector3 point2){
		bool isParallel = Mathf.Abs(Vector3.Dot(rayDirection.normalized, otherRayDirection.normalized)) > 1 - Mathf.Epsilon;
		if (isParallel){
			point1 = Vector3.zero;
			point2 = Vector3.zero;
			return false;
		}

		// loosely based on http://www.youtube.com/watch?v=HC5YikQxwZA
		// t is distance traveled at ray and s distance traveled at otherRay
		// PQ is vector between rays
		Vector3 PQT = -rayDirection;
		Vector3 PQS = otherRayDirection;
		Vector3 PQVal = -rayOrigin + otherRayOrigin;

		float PQu1S = Vector3.Dot(PQS, rayDirection);
		float PQu1T = Vector3.Dot(PQT, rayDirection);
		float PQu1Val = Vector3.Dot(PQVal, rayDirection) * -1;

		float PQu2S = Vector3.Dot(PQS, otherRayDirection);
		float PQu2T = Vector3.Dot(PQT, otherRayDirection);
		float PQu2Val = Vector3.Dot(PQVal, otherRayDirection) * -1;

		// maybe not the most efficient way to solve the system of linear equations
		Matrix4x4 mat = Matrix4x4.identity;
		mat[0,0] = PQu1S;
		mat[0,1] = PQu1T;
		mat[1,0] = PQu2S;
		mat[1,1] = PQu2T;

		mat = mat.inverse;
		Vector4 res = mat * new Vector4(PQu1Val, PQu2Val, 0,0);

		point1 = rayOrigin + rayDirection * res[1];
		point2 = otherRayOrigin + otherRayDirection * res[0];

		return true;
	}
}

using UnityEngine;

public static class DebugExt {
	public static void DrawPlane (Plane plane, Vector3 centerPosition, Color color, float duration = 0, float length = 10.0f)
	{
#if UNITY_EDITOR
		// pick a valid tangent value
		Vector3 tangent = Vector3.Dot( plane.normal, Vector3.up) < 0.5f ? Vector3.up : Vector3.left;


		Vector3 t1 = Vector3.Cross(plane.normal, tangent).normalized;
		Vector3 t2 = Vector3.Cross(plane.normal, t1).normalized;

		Vector3 position = centerPosition;

		DebugExt.DrawCross(position, color, length/10, duration);
		Debug.DrawLine(position +(  t1 + t2)*length, position +(  t1 - t2)*length,color,duration);
		Debug.DrawLine(position +(- t1 + t2)*length, position +(- t1 - t2)*length,color,duration);
		Debug.DrawLine(position +(  t1 + t2)*length, position +(- t1 + t2)*length,color,duration);
		Debug.DrawLine(position +(  t1 - t2)*length, position +(- t1 - t2)*length,color,duration);
#endif
	}

	public static void DrawBounds (Bounds bounds, Color color, float duration = 0)
	{
		#if UNITY_EDITOR
		DrawBox(bounds.min, bounds.max, color, duration);
		#endif
	}

	public static void DrawBox (Vector3 from, Vector3 to, Color color, float duration = 0)
	{
#if UNITY_EDITOR
		Debug.DrawLine(from, new Vector3(from.x, from.y, to.z),color,duration);
		Debug.DrawLine(from, new Vector3(from.x, to.y, from.z),color,duration);
		Debug.DrawLine(from, new Vector3(to.x, from.y, from.z),color,duration);

		Debug.DrawLine(new Vector3(to.x, to.y, from.z), to,color,duration);
		Debug.DrawLine(new Vector3(to.x, from.y, to.z), to,color,duration);
		Debug.DrawLine(new Vector3(from.x, to.y, to.z), to,color,duration);

		Debug.DrawLine(new Vector3(to.x, from.y, from.z), new Vector3(to.x, from.y, to.z),color,duration);
		Debug.DrawLine(new Vector3(to.x, from.y, to.z),new Vector3(from.x, from.y, to.z), color,duration);
		Debug.DrawLine(new Vector3(from.x, from.y, to.z), new Vector3(from.x, to.y, to.z),color,duration);

	    Debug.DrawLine(new Vector3(from.x, to.y, from.z),new Vector3(from.x, to.y, from.z), color,duration);
		Debug.DrawLine(new Vector3(from.x, to.y, from.z), new Vector3(to.x, to.y, from.z),color,duration);
		Debug.DrawLine(new Vector3(to.x, to.y, from.z),new Vector3(to.x, from.y, from.z), color,duration);
	    Debug.DrawLine(new Vector3(from.x, to.y, from.z),new Vector3(from.x, to.y, to.z), color,duration);
#endif
	}

	public static void DrawCross(Vector3 position, Color color, float size = 1, float duration = 0){
#if UNITY_EDITOR
		Debug.DrawLine(position+Vector3.down*size, position+Vector3.up*size,color,duration);
		Debug.DrawLine(position+Vector3.left*size, position+Vector3.right*size,color,duration);
		Debug.DrawLine(position+Vector3.forward*size, position+Vector3.back*size,color,duration);
#endif
	}
}

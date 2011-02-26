using UnityEngine;
using System.Collections;
using System;

public class UUnitAssert  {

	public static void True(bool boolean, string msg){
		if (boolean) return; 
		Error(true, false, msg);
	}
	
	public static void False(bool boolean, string msg){
		if (!boolean) return; 
		Error(false, true, msg);
	}
		
	public static void Equals(int expected, int actual,string msg){
		if (expected == actual) return;
		Error(expected, actual, msg);
	}
	
	public static void Equals(float expected, float actual){
		Equals(expected, actual, Mathf.Epsilon, "");
	}
	
	public static void Equals(float expected, float actual,float delta){
		Equals(expected, actual, delta, "");
	}
	
	public static void Equals(float expected, float actual,string msg){
		Equals(expected, actual, Mathf.Epsilon, msg);
	}
	
	public static void Equals(float expected, float actual,float delta,string msg){
		if (Mathf.Abs(expected-actual)<=delta) return;
		Error(expected, actual, msg);
	}
	
	public static new void Equals(object expected, object actual){
		Equals(expected, actual,"");
	}
	
	public static void Equals(object expected, object actual, string msg){
		if (expected==null && actual == null || expected.Equals(actual)) return;
		Error(expected, actual, msg);
	}
	
	public static new void Equals(Vector3 expected, Vector3 actual){
		Equals(expected, actual,"");
	}
	
	public static void Equals(Vector3 expected, Vector3 actual, string msg){
		Equals(expected, actual, 0.001f, msg);
	}
	
	public static void Equals(Vector3 expected, Vector3 actual, float delta){
		Equals(expected, actual, delta, "");
	}
	
	public static void Equals(Vector3 expected, Vector3 actual, float delta, string msg){
		Vector3 dif = expected-actual;
		if (Mathf.Abs(dif.x)<delta &&
		    Mathf.Abs(dif.y)<delta &&
		    Mathf.Abs(dif.z)<delta) return;
		Error(expected, actual, msg);
	}
	
	public static void Same(object expected, object actual){
		Same(expected, actual, "");
	}
	
	public static void Same(object expected, object actual, string msg){
		if (expected == actual) return;
		Error(expected, actual, msg);
	}

	public static void IsNull(object actual){
		IsNull(actual, "");
	}
	
	public static void IsNull(object actual, string msg){
		if (actual==null) return;
		Error(null, actual, msg);
	}
	
	public static void IsNotNull(object actual){
		IsNotNull(actual, "");
	}
	
	public static void IsNotNull(object actual, string msg){
		if (actual!=null) return;
		Error("not null", actual, msg);
	}
	
	private static void Error(object expected, object actual, string msg){
		throw new UUnitAssertException(expected, actual, msg);
	}
}

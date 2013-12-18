using UnityEngine;
using System.Collections;

public static class HashtableExt {
	public static object TryGet(this Hashtable hashtable, object key, object notFound){
		if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			return hashtable[key];
		}
		return notFound;
	}
	
	public static double TryGetDouble(this Hashtable hashtable, object key, double notFound){
#if UNITY_EDITOR
		try{
#endif	
		if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			System.Double res = (System.Double)  hashtable[key];
			return res;
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif					
			
		return notFound;
	}
	
	public static string TryGetString(this Hashtable hashtable, object key, string notFound){
#if UNITY_EDITOR
		try{
#endif			
		if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			string res = (string)  hashtable[key];
			return res;
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif			
		return notFound;
	}
	
	public static bool TryGetBool(this Hashtable hashtable, object key, bool notFound){
#if UNITY_EDITOR
		try{
#endif				
		if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			System.Boolean res = (System.Boolean)   hashtable[key];
			return res;
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif			
		return notFound;
	}
	
	public static int TryGetInt(this Hashtable hashtable, object key, int notFound){
#if UNITY_EDITOR
		try{
#endif			
		if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			System.Double res = (System.Double)  hashtable[key];
			return Mathf.RoundToInt((float)res);
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif
		return notFound;
	}
	
	public static long TryGetLong(this Hashtable hashtable, object key, long notFound){
#if UNITY_EDITOR
		try{
#endif		
			if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			System.Double res = (System.Double)  hashtable[key];
			return (long)System.Math.Round(res);
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif			
		return notFound;
	}
	
	public static float TryGetFloat(this Hashtable hashtable, object key, float notFound){
#if UNITY_EDITOR
		try{
#endif				
			if (hashtable == null) return notFound;
		if (hashtable.ContainsKey(key)){
			System.Double res = (System.Double)  hashtable[key];
			return (float)res;
		}
#if UNITY_EDITOR
		} catch (System.Exception e){
			Debug.LogError(e);
			Debug.LogError("Object was "+hashtable[key]);
		}
#endif			
		return notFound;
	}
	
}

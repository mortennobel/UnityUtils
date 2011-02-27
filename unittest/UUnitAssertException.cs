using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Exception thrown when assertions are false
/// </summary>
public class UUnitAssertException : Exception {
	
	private string msg;
	public UUnitAssertException(object expected, object actual, string msg){
		this.Expected = expected;
		this.Actual = actual;
		this.msg = msg;
	}	
	
	
	public object Actual {
		get;
		set;
	}
	
	public object Expected {
		get;
		set;
	}
	
	public string Details(){
		return "UUnitAssertException: Expected: "+Expected+" actual "+Actual+" msg: "+msg+" \n"+base.ToString();
	}
}

/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Exception thrown when assertions are false
/// </summary>
public class UUnitAssertException : Exception {
	
	public UUnitAssertException(object expected, object actual, string msg)
			:base(msg){
		this.Expected = expected;
		this.Actual = actual;
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
		return "UUnitAssertException: Expected: "+Expected+" actual "+Actual+" msg: "+Message+" \n"+base.ToString();
	}
}

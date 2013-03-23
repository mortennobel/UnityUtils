/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Unit test result
/// </summary>
public class UUnitTestResult {
	private int runCount;
	private int failedCount;
	private double duration = 0;
	private List<TestResultEntry> resultList = new List<TestResultEntry>();
	
	public void AddTestResult(string className,string methodName,bool success,UUnitAssertException exception,double duration, string errormsg){
		resultList.Add(new TestResultEntry(className, methodName, success, exception, duration,errormsg));
		if (success){
			runCount ++;
		} else {
			failedCount ++;
		}
		this.duration += duration;
	}
	
	public string Summary()
    {
        string summary = "";
		summary += "TestResult: success: "+runCount+" failed: "+failedCount+" duration: "+duration+"\n";
		summary += "-------------------------- Details ---------------------\n";
		foreach (TestResultEntry tre in  resultList){
			if (!tre.IsSuccess())
				summary += tre.ToString()+"\n";
		}
		foreach (TestResultEntry tre in  resultList){
			if (tre.IsSuccess())
				summary += tre.ToString()+"\n";
		}
		
		return summary;
    }
	
	public string ShortSummary(){
		string summary = "";
		foreach (TestResultEntry tre in  resultList){
			if (!tre.IsSuccess())
				summary += tre.ToString(true)+"\n";
		}
		foreach (TestResultEntry tre in  resultList){
			if (tre.IsSuccess())
				summary += tre.ToString(true)+"\n";
		}
		
		return summary;
	}
}

class TestResultEntry{
	string className;
	string methodName;
	bool success;
	UUnitAssertException exception;
	double duration;
	string errormsg;
	
	public TestResultEntry(string className, string methodName,bool success,UUnitAssertException exception, double duration, string errormsg){
		this.className = className;
		this.methodName = methodName;
		this.success = success;
		this.exception = exception;
		this.duration = duration;
		this.errormsg = errormsg;
	}
	
	public new string ToString(bool shortSummary = false){
		string res = (success?
		        "SUCCESS ":
		        "FAIL    ")+className+"."+methodName+" duration: "+duration+" ";
		if (!shortSummary) {
			res += "\n"+(exception!=null?exception.Details():(errormsg!=null?errormsg:""));
		} else {
			bool hasErrorMsg = errormsg != null && errormsg.Length > 0;
			if (hasErrorMsg) {
				res += "\n"+errormsg;
			}
		}
		return res;
	}
				
	
	
	public bool IsSuccess(){
		return success;
	}
}

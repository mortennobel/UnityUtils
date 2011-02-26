using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

public class UUnitTestSuite {
	private List<Type> testCases = new List<Type>();
	
	public void Add(UUnitTestCase o){
		Add(o.GetType());
	}
	
	public void Add(Type type){
		if (!type.IsSubclassOf(new UUnitTestCase().GetType())){
			throw new Exception("Invalid type - must subclass UUnitTestCase");
		}
		testCases.Add(type);
	}
	
	public UUnitTestResult RunAll(){
		UUnitTestResult res = new UUnitTestResult();
		foreach (Type t in testCases){
			string typeName = t.Name;
			foreach (MethodInfo m in t.GetMethods()){
				if (m.Name.StartsWith("Test") && m.GetParameters().Length==0){
					ConstructorInfo[] p = t.GetConstructors();
					
	        		for (int i=0;i<p.Length;i++) {
						if (p[i].GetParameters().Length==0){
							DateTime startTime1 = DateTime.Now; 
							DateTime stopTime1 = DateTime.Now;
							
							UUnitAssertException ae = null;
							string errMsg = null;
							bool success = true;
							try {
								UUnitTestCase unitTest = (UUnitTestCase)p[i].Invoke(new object[0]);	
								unitTest.Setup();
								startTime1 = DateTime.Now;
								m.Invoke(unitTest, new object[0]);
								stopTime1 = DateTime.Now;
								unitTest.TearDown();
							} catch (System.Reflection.TargetInvocationException e){
								stopTime1 = DateTime.Now;
								ae = e.InnerException as UUnitAssertException;
								success = false;
								if (ae==null){
									errMsg = "\nException during execution:\n"+e.InnerException.ToString();
								}
							} catch (System.Exception ex) {
								stopTime1 = DateTime.Now;
								success = false;
								errMsg = "\nException during execution:\n"+ex.ToString();
								
							}
							TimeSpan duration1 = stopTime1 - startTime1;
							double timeSpendMillis = duration1.TotalMilliseconds;
						
							res.AddTestResult(typeName,m.Name,success, ae, timeSpendMillis, errMsg);
						}
	        		}
				}
			}
		}
		return res;
	}
}

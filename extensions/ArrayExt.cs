using System;
using UnityEngine;


public static class ArrayExt {
	public static int BinarySearch(this int[] array, int low, int high, int val){
		while (low <= high)
        {
            int midpoint = (int)(((uint)high + (uint)low) >> 1);
			int v = array[midpoint];
            if (val == v)
            {
            	return midpoint;
            }
            else if (val < v)
                    high = midpoint - 1;
            else
                    low = midpoint + 1;
        }
        return -1;
	}
	
	public static void Fill(this float[] array, float v)
    {
        for (int i=0;i<array.Length;i++){
			array[i] = v;
		}
    }

	public static void Fill(this double[] array, double v)
    {
        for (int i=0;i<array.Length;i++){
			array[i] = v;
		}
    }
	
	
	public static void Write2Matlab(this float[] array, string filename, string varname){
		System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
 		file.WriteLine(varname+"=[");
		for(int c=0;c<array.Length;c++)
		{
     			file.WriteLine(array[c] ); 
			
		}
		file.WriteLine("];");
		file.Close();
    }
		
	
	public static void CopyTo(this float[] array, float[] dest){
		Array.Copy(array, dest, array.Length);
	}
	
	public static void CopyTo(this double[] array, double[] dest){
		Array.Copy(array, dest, array.Length);
	}
	
	
	public static float Sum(this float[] array){
		float sum = 0.0f;
		for (int i=0;i<array.Length;i++){
			sum += array[i];
		}
		return sum;
	}
	
	public static double Sum(this double[] array){
		double sum = 0.0f;
		for (int i=0;i<array.Length;i++){
			sum += array[i];
		}
		return sum;
	}
	
	
	public static float Max(this float[] array){
		float max = -float.MaxValue;
		for (int i = 0; i < array.Length; i++){
			max = Mathf.Max(array[i], max);
		}
		return max;
	}
	
	public static float MaxDelta(this float[] x, float[] y){
		float change = 0.0f;
		for (int i=0;i<x.Length;i++){
			change = Mathf.Max(Math.Abs(x[i]-y[i]), change);
		}
		return change;
	}
	
	public static double Max(this double[] array){
		double max = -float.MaxValue;
		for (int i = 0; i < array.Length; i++){
			max = Math.Max(array[i], max);
		}
		return max;
	}
	
	public static double MaxDelta(this double[] x, double[] y){
		double change = 0.0f;
		for (int i=0;i<x.Length;i++){
			change = Math.Max(Math.Abs(x[i]-y[i]), change);
		}
		return change;
	}
	
	public static void MultiplyWith(this float[] x, float[] y){
		for (int i=0;i<x.Length;i++){
			x[i] *= y[i];
		}
	}
	
	public static void MultiplyWith(this float[] x, float y){
		for (int i=0;i<x.Length;i++){
			x[i] *= y;
		}
	}
	
	public static void DivideBy(this float[] x, float[] y){
		for (int i=0;i<x.Length;i++){
			x[i] /= y[i];
		}
	}
	
	public static float Dot(this float[] x, float[] y){
		float dotProduct = 0.0f;
		for (int i=0;i<x.Length;i++){
			dotProduct += x[i] * y[i]; 
		}
		return dotProduct;
	}
	
	public static void MultiplyWith(this double[] x, double[] y){
		for (int i=0;i<x.Length;i++){
			x[i] *= y[i];
		}
	}
	
	public static void MultiplyWith(this double[] x, double y){
		for (int i=0;i<x.Length;i++){
			x[i] *= y;
		}
	}
	
	public static void DivideBy(this double[] x, double[] y){
		for (int i=0;i<x.Length;i++){
			x[i] /= y[i];
		}
	}
	
	public static double Dot(this double[] x, double[] y){
		double dotProduct = 0.0f;
		for (int i=0;i<x.Length;i++){
			dotProduct += x[i] * y[i]; 
		}
		return dotProduct;
	}
	
	public static string ToStringPretty(this float[] x){
		string res = "[";
		for (int i=0;i<x.Length;i++){
			res += x[i]+", ";
		}
		res += "]";
		return res;
	}
}

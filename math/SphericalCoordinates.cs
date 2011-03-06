/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)


using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// In mathematics, a spherical coordinate system is a coordinate system for 
/// three-dimensional space where the position of a point is specified by three numbers: 
/// the radial distance of that point from a fixed origin, its inclination angle measured 
/// from a fixed zenith direction, and the azimuth angle of its orthogonal projection on 
/// a reference plane that passes through the origin and is orthogonal to the zenith, 
/// measured from a fixed reference direction on that plane. 
/// 
/// The zenith direction is the up vector (0,1,0) and the azimuth is the right vector (1,0,0)
/// 
/// (From http://en.wikipedia.org/wiki/Spherical_coordinate_system )
/// </summary>
[Serializable]
public class SphericalCoordinates  {
	/// <summary>
	/// the radial distance of that point from a fixed origin.
	/// Radius must be >= 0
	/// </summary>
	public float radius;
	/// <summary>
	/// azimuth angle of its orthogonal projection on 
	/// a reference plane that passes through the origin and is orthogonal to the zenith
	/// </summary>
	public float polar;
	/// <summary>
	/// elevation angle from the reference plane 
	/// </summary>
	public float elevation;
	
	/// <summary>
	/// Converts a point from Spherical coordinates to Cartesian (using positive
    /// * Y as up)
	/// </summary>
	public Vector3 ToCartesian(){
		Vector3 res = new Vector3();
		SphericalToCartesian(radius, polar, elevation, out res);
		return res;
	}
	
	/// <summary>
	/// Converts a point from Cartesian coordinates (using positive Y as up) to
    /// Spherical and stores the results in the store var. (Radius, Azimuth,
    /// Polar)
	/// </summary>
    public static SphericalCoordinates CartesianToSpherical(Vector3 cartCoords) {
		SphericalCoordinates store = new SphericalCoordinates();        
		CartesianToSpherical(cartCoords, out store.radius, out store.polar, out store.elevation);
		return store;
    }
	
	/// <summary>
	/// Converts a point from Spherical coordinates to Cartesian (using positive
    /// * Y as up)
	/// </summary>
	public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart){
		float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
		outCart.y =	radius * Mathf.Sin(elevation);
		outCart.z = a * Mathf.Sin(polar);
	}
	
	/// <summary>
	/// Converts a point from Cartesian coordinates (using positive Y as up) to
    /// Spherical and stores the results in the store var. (Radius, Azimuth,
    /// Polar)
	/// </summary>
	public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation){
		if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                        + (cartCoords.y * cartCoords.y)
                        + (cartCoords.z * cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
	 		outPolar += Mathf.PI;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
	}
}
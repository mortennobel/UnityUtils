/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

using UnityEngine;
using System.Collections;

public struct Ray2DD{
	
		//
		// Properties
		//
		public Vector2D direction {
			get;
			set;
		}

		public Vector2D origin {
			get;
			set;
		}

		//
		// Constructors
		//
	public Ray2DD (Vector2D origin, Vector2D direction){
		this.origin = origin;
		this.direction = direction;
	}

		//
		// Methods
		//
	public Vector2D GetPoint (double distance){
		return origin + direction*distance;
	}
}

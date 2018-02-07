/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

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

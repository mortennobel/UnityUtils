using UnityEngine;
using System.Collections;

public static class ColorExt {
	/// <summary>
	/// Set the jet color (based on the Jet color map) ( http://www.metastine.com/?p=7 )
	/// val should be normalized between 0 and 1
	/// </summary>
	public static Color GetJetColor(float val) {
        float fourValue = 4.0f * val;
		float red   = Mathf.Min(fourValue - 1.5f, -fourValue + 4.5f);
		float green = Mathf.Min(fourValue - 0.5f, -fourValue + 3.5f);
		float blue  = Mathf.Min(fourValue + 0.5f, -fourValue + 2.5f);
		Color newColor = new Color();
		newColor.r = Mathf.Clamp01(red);		
		newColor.g = Mathf.Clamp01(green);
		newColor.b = Mathf.Clamp01(blue);
		newColor.a = 1;
		return newColor;
    }
}

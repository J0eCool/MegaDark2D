using UnityEngine;
using System.Collections.Generic;

public static class VectorUtil {
	//public static void SetX(this Vector3 vec, float x) {
	//	vec.x = x;
	//}

	public static Vector2 Unit(float radians) {
		return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
	}
}

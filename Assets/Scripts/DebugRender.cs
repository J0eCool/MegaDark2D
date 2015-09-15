using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRender {
    public static void Circle(float radius, Vector3 center) {
        Circle(radius, center, Color.white);
    }

    public static void Circle(float radius, Vector3 center, Color color) {
        const int numSegments = 16;
        for (int i = 0; i < numSegments; ++i) {
            float a1 = Mathf.PI * 2 * i / numSegments;
            float a2 = Mathf.PI * 2 * (i + 1) / numSegments;
            Vector2 p1 = radius * VectorUtil.Unit(a1) + (Vector2)center;
            Vector2 p2 = radius * VectorUtil.Unit(a2) + (Vector2)center;
            Debug.DrawLine(p1, p2, color);
        }
    }

    public static void Cross(float size, Vector3 center) {
        Cross(size, center, Color.white);
    }

    public static void Cross(float size, Vector3 center, Color color) {
        Debug.DrawLine(
            center + new Vector3(-size, -size),
            center + new Vector3(size, size),
            color
        );
        Debug.DrawLine(
            center + new Vector3(-size, size),
            center + new Vector3(size, -size),
            color
        );
    }
}

using UnityEngine;

public static class BezierUtility
{
    public static Vector3 GetPoint(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t)
    {
        t = Mathf.Clamp01(t);

        float u = 1f - t;

        return
            u * u * u * p0 +
            3 * u * u * t * p1 +
            3 * u * t * t * p2 +
            t * t * t * p3;
    }
}
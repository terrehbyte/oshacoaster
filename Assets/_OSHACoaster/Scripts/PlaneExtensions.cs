using UnityEngine;

public static class PlaneExtensions
{
    public static Vector3 GetIntersection(this Plane plane, Vector3 rayOrigin, Vector3 rayVector)
    {
        float prod1 = Vector3.Dot(rayOrigin, plane.normal);
        float prod2 = Vector3.Dot(rayVector, plane.normal);
        float prod3 = prod1 / prod2;
        return rayOrigin - rayVector * prod3;
    }

    public static Vector3 GetIntersection(this Plane plane, Ray ray)
    {
        return plane.GetIntersection(ray.origin, ray.direction);
    }
}
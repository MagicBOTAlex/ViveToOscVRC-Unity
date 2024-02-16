using UnityEngine;

public static class QuaternionExtensions
{
    public static float LengthSquared(this Quaternion quaternion)
    {
        Vector3 axis = new Vector3(quaternion.x, quaternion.y, quaternion.z);
        return axis.sqrMagnitude;
    }
}

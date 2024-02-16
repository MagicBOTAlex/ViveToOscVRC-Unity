using System;
using UnityEngine;
using Valve.VR;

public static class HmdMatrix34_tExtensions
{
    public static Vector3 ExtractPosition(this Valve.VR.HmdMatrix34_t mat)
    {
        return new Vector3(mat.m3, mat.m7, -mat.m11);
    }

    public static Quaternion ExtractRotation(this Valve.VR.HmdMatrix34_t mat)
    {
        Quaternion q = default;
        q.w = MathF.Sqrt(MathF.Max(0, 1 + mat.m0 + mat.m5 + mat.m10)) / 2;
        q.x = MathF.Sqrt(MathF.Max(0, 1 + mat.m0 - mat.m5 - mat.m10)) / 2;
        q.y = MathF.Sqrt(MathF.Max(0, 1 - mat.m0 + mat.m5 - mat.m10)) / 2;
        q.z = MathF.Sqrt(MathF.Max(0, 1 - mat.m0 - mat.m5 + mat.m10)) / 2;
        q.x = (float)CopySign(q.x, mat.m9 - mat.m6);
        q.y = (float)CopySign(q.y, mat.m2 - mat.m8);
        q.z = (float)CopySign(q.z, mat.m1 - mat.m4);

        var scale = 1 / q.LengthSquared();
        return new Quaternion(q.x * -scale, q.y * -scale, q.z * -scale, q.w * scale);
    }

    public static double CopySign(double x, double y) // GPT
    {
        // Check if the sign of x and y are the same or not
        if (Math.Sign(x) == Math.Sign(y))
        {
            // If signs are the same, return x
            return x;
        }
        else
        {
            // If signs are different, return -x
            return -x;
        }
    }

    public static Matrix4x4 ToMatrix4x4(this HmdMatrix34_t matrix)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = matrix.m0;
        m.m01 = matrix.m1;
        m.m02 = matrix.m2;
        m.m03 = matrix.m3;
        m.m10 = matrix.m4;
        m.m11 = matrix.m5;
        m.m12 = matrix.m6;
        m.m13 = matrix.m7;
        m.m20 = matrix.m8;
        m.m21 = matrix.m9;
        m.m22 = matrix.m10;
        m.m23 = matrix.m11;
        return m;
    }
}
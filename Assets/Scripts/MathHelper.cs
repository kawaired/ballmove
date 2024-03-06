using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    public static Vector2 Factorization(float a, float b, float c)
    {
        if(a!=0)
        {
            float root1 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            float root2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            if (root1 > root2)
            {
                return new Vector2(root2, root1);
            }
            else
            {
                return new Vector2(root1, root2);
            }
        }
        else
        {
            return new Vector2((-c) / b, (-c) / b);
        }
    }

    public static bool CheckFacRoot(float a ,float b,float c)
    {
        float rootfac = b * b - 4 * a * c;
        return (rootfac >= 0);
    }

    public static Vector3 CalculateSpeed(Vector3 initspeed,Vector3 accelerate,float t)
    {
        return initspeed + accelerate * t;
    }

    public static Vector3 CalculatePos(Vector3 startpos,Vector3 initspeed,Vector3 accelerate,float t)
    {
        return startpos + initspeed * t + 0.5f * accelerate * t * t;
    }

    public static Vector3 ReflectVector(Vector3 indir,Vector3 normal)
    {
        //Debug.Log("reflect");
        normal = normal.normalized;
        return indir - 2 * normal * Vector3.Dot(indir, normal);
    }

    public static float Reciprocal(float num)
    {
        if(num==0)
        {
            return 1;
        }
        else
        {
            return 1 / num;
        }
    }
}

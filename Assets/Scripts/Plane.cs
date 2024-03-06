using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct PlaneInputData
{
    public Vector3 dir;
    public float len;
}

public class Plane
{
    public Vector4 planedata;
    public Plane(PlaneInputData inputdata)
    {
        inputdata.dir = inputdata.dir.normalized;
        planedata = new Vector4(inputdata.dir.x,inputdata.dir.y,inputdata.dir.z,inputdata.dir.x * inputdata.len + inputdata.dir.y * inputdata.len + inputdata.dir.z * inputdata.len);
    }
    
    public bool CheckFocus(PredicData predicdata,out float firstfoctime)
    {
        float facA = 0.5f * predicdata.accelerate3.x * planedata.x + 0.5f * predicdata.accelerate3.y * planedata.y + 0.5f * predicdata.accelerate3.z * planedata.z;
        float facB = predicdata.startspeed.x * planedata.x + predicdata.startspeed.y * planedata.y + predicdata.startspeed.z * planedata.z;
        float facC = predicdata.startpos.x * planedata.x + predicdata.startpos.y * planedata.y + predicdata.startpos.z * planedata.z - planedata.w;
   
        if (MathHelper.CheckFacRoot(facA, facB, facC))
        {
            if(facA==0)
            {
                firstfoctime = -facC / facB;
            }
            else
            {
                Vector2 root = MathHelper.Factorization(facA, facB, facC);
                
                if(root.x>0)
                { 
                    firstfoctime = root.x;
                }
                else if(root.y>0)
                {
                    firstfoctime = root.y;
                }
                else
                {
                    firstfoctime = -1;
                }
            }
        }
        else
        {
            firstfoctime = -1;
        }

        return (firstfoctime > 0);
    }
}

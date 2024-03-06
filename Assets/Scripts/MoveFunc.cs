using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ParaData
{
    public Vector3 startpos;
    public Vector3 endpos;
    public Vector3 startspeed;
    public Vector3 accelerate3;
    public float tfac;
    public float tspeedatten;
}


public class MoveFunc
{
    public ParaData data;
    public Vector3 diffvec;
    public Vector3 horivec;
    public float len;

    public MoveFunc(ParaData paradata)
    {
        data = paradata;
        diffvec = data.endpos - data.startpos;
        len = 0;

        float ytoptime;

        if (data.accelerate3.y == 0)
        {
            ytoptime = 0.5f * paradata.tfac;
        }
        else
        {
            ytoptime = data.startspeed.y / Mathf.Abs(data.accelerate3.y);
        }

        if (ytoptime < 0)
        {
            len = (data.endpos - data.startpos).magnitude;
        }
        else if (paradata.tfac > ytoptime)
        {
            Vector3 toppos = data.startpos + data.startspeed * ytoptime + 0.5f * data.accelerate3 * Mathf.Pow(ytoptime, 2);
            len = (toppos - data.startpos).magnitude + (data.endpos - toppos).magnitude;
        }
        else
        {
            len = (data.endpos - data.startpos).magnitude;
        }
    }

    public Vector3 Parabola( float dis,Vector3 hvecdir)
    {
        float curfac = (dis / data.tfac) * data.tfac;
        Vector3 finalvec = data.startspeed * curfac + 0.5f * data.accelerate3 * curfac * curfac + data.startpos;
        return finalvec;
    }
}

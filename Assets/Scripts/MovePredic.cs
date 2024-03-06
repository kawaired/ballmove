using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PredicData
{
    public float flytime;
    public Vector3 startpos;
    public Vector3 endpos;
    public float starttime;
    public float height;
    public float hpower;
    public Vector3 accelerate3;
    public float angle;
    public Vector3 startspeed;
    public Vector3 endspeed;
    public Plane collideplane;
    public bool collided;
    public float recordtime;
    public float timespeedatten;
}

public class MovePredic
{
    public List<ParaData> paralist;
    public Vector3 vecdiff;
    public Vector3 vecprojection;
    Vector3 initspeed;
    Vector3 inita;
    public float timefac;
    public Vector3 horivec;
    public MovePredic(Vector3 startpos, Vector3 endpos, float vpower, float hpower, List<Plane> planelist)
    {
        float rotateangle;
        vecdiff = endpos - startpos;
        vecprojection = vecdiff;
        vecprojection.y = 0;
        if (Vector3.Dot(vecprojection, vecdiff) == 0)
        {
            rotateangle = 0;
            horivec = Vector3.zero;
        }
        else
        {
            rotateangle = 180 * Mathf.Acos(Mathf.Clamp01(Vector3.Dot(vecdiff, vecprojection) / (vecdiff.magnitude * vecprojection.magnitude))) / Mathf.PI;
            Vector3 orivector = Quaternion.AngleAxis(rotateangle, Vector3.up) * vecprojection;
            horivec = Vector3.Cross(Vector3.up, orivector).normalized;
         }
        

        timefac = vecdiff.magnitude / 5;
        inita = new Vector3(hpower * horivec.x, vpower, hpower * horivec.z);
        initspeed = (vecdiff - 0.5f * inita * timefac * timefac) / timefac;
        Vector2 falltime;
        if (vpower == 0)
        {
            falltime = new Vector2(0, timefac);
        }
        else
        {
            falltime = MathHelper.Factorization(0.5f * vpower, initspeed.y, startpos.y);
        }

        Vector3 fallpos2 = startpos + initspeed * falltime.y + 0.5f * inita * falltime.y * falltime.y;
        timefac = falltime.y;
        PredicData predicdata = new PredicData();
        predicdata.hpower = hpower;
        predicdata.accelerate3 = new Vector3(hpower * horivec.x, vpower, hpower * horivec.z);
        predicdata.startspeed = initspeed;
        predicdata.startpos = startpos;
        predicdata.endpos = fallpos2;
        predicdata.angle = rotateangle;
        predicdata.starttime = 0;
        predicdata.flytime = falltime.y;
        predicdata.recordtime = predicdata.flytime;
        predicdata.timespeedatten = 1;
        predicdata.accelerate3 = inita;
        predicdata.height = 0.5f * (-predicdata.accelerate3.y) * Mathf.Pow(0.5f * timefac, 2);
        predicdata.endspeed = initspeed + predicdata.accelerate3 * predicdata.flytime;
        predicdata = CollideHandle(planelist, predicdata);
        paralist = new List<ParaData>();

        ParaData everydata = new ParaData();
        everydata.startpos = startpos;
        everydata.endpos = predicdata.endpos;
        everydata.startspeed = predicdata.startspeed;
        everydata.accelerate3 = predicdata.accelerate3;
        everydata.tfac = predicdata.flytime;
        everydata.tspeedatten = predicdata.timespeedatten;

        paralist.Add(everydata);
        

        //球还在弹跳阶段
        //|| predicdata.collided这段代码删掉是否有后遗症需要在证实一下。
        while ((predicdata.height > 0.5f)  || (predicdata.collideplane != null))
        {
            predicdata.collided = (predicdata.collideplane != null);
            predicdata = Predic(predicdata);
            predicdata = CollideHandle(planelist, predicdata);
            everydata.startpos = predicdata.startpos;
            everydata.endpos = predicdata.endpos;
            everydata.accelerate3 = predicdata.accelerate3;
            everydata.startspeed = predicdata.startspeed;
            everydata.tfac = predicdata.flytime;
            everydata.tspeedatten = predicdata.timespeedatten;

            paralist.Add(everydata);
        }

        //球进入滚动阶段 
        predicdata = Predic(predicdata);

        predicdata = CollideHandle(planelist, predicdata);
        everydata.startpos = predicdata.startpos;
        everydata.endpos = predicdata.endpos;
        everydata.startspeed = predicdata.startspeed;
        everydata.tfac = predicdata.flytime;
        everydata.accelerate3 = Vector3.zero;
        everydata.tspeedatten = predicdata.timespeedatten;
        paralist.Add(everydata);

        while (predicdata.collideplane != null)
        {
            predicdata = Predic(predicdata);
            predicdata = CollideHandle(planelist, predicdata);
            everydata.startpos = predicdata.startpos;
            everydata.endpos = predicdata.endpos;
            everydata.startspeed = predicdata.startspeed;
            everydata.tfac = predicdata.flytime;
            everydata.accelerate3 = Vector3.zero;
            everydata.tspeedatten = predicdata.timespeedatten;
            paralist.Add(everydata);
        }
    }

    private PredicData Predic(PredicData predicdata)
    {
        PredicData nextdata = new PredicData();
        nextdata.timespeedatten=1;
        if (predicdata.collideplane != null)
        {
            nextdata.collided = true;
            nextdata.startpos = predicdata.endpos;
            
            Vector3 speedrotateaxle = Vector3.Cross(predicdata.endspeed.normalized, predicdata.collideplane.planedata);
            nextdata.startspeed = MathHelper.ReflectVector(predicdata.endspeed, predicdata.collideplane.planedata) * 0.75f;
            nextdata.startspeed = Quaternion.AngleAxis(nextdata.angle, speedrotateaxle) * nextdata.startspeed;
            nextdata.hpower = 0;
            if (predicdata.height > 0.5f)
            {
                nextdata.accelerate3 = new Vector3(0, predicdata.accelerate3.y, 0);
                nextdata.angle = predicdata.angle * 0.5f;
                nextdata.flytime = MathHelper.Factorization(0.5f * nextdata.accelerate3.y, nextdata.startspeed.y, nextdata.startpos.y).y;
                nextdata.endspeed = nextdata.startspeed + nextdata.accelerate3 * nextdata.flytime;
                float heightime = Mathf.Abs(predicdata.startspeed.y) / Mathf.Abs(predicdata.accelerate3.y);
                nextdata.height = predicdata.startpos.y + 0.5f * Mathf.Abs(predicdata.accelerate3.y) * heightime * heightime;
                nextdata.endpos = MathHelper.CalculatePos(nextdata.startpos, nextdata.startspeed, nextdata.accelerate3, nextdata.flytime);
                nextdata.recordtime = predicdata.recordtime;
            }
            else
            {
                nextdata.accelerate3 = Vector3.zero;
                nextdata.flytime = predicdata.recordtime - predicdata.flytime;
                nextdata.angle = 0;
                nextdata.endspeed = nextdata.startspeed + nextdata.accelerate3 * nextdata.flytime;
                nextdata.height = 0;
                nextdata.recordtime = predicdata.recordtime;
                nextdata.endpos = MathHelper.CalculatePos(nextdata.startpos, nextdata.startspeed, nextdata.accelerate3, nextdata.flytime);
            }
        } 
        else
        {
            nextdata.collided = false;
            if ((predicdata.height > 0.5f))
            {
                nextdata.startpos = predicdata.endpos;
                nextdata.startspeed = predicdata.endspeed;
                nextdata.startspeed.y = -nextdata.startspeed.y;
                nextdata.startspeed = nextdata.startspeed * 0.75f;
                nextdata.angle = predicdata.angle * 0.5f;
                Vector3 vecdir = Vector3.Cross(Vector3.up, Quaternion.AngleAxis((nextdata.angle) * (-CheckStep(predicdata.hpower)), Vector3.up) * new Vector3(nextdata.startspeed.x, 0, nextdata.startspeed.z).normalized);
              
                nextdata.hpower = 0.25f * predicdata.hpower;
                nextdata.accelerate3 = new Vector3(nextdata.hpower * vecdir.x, predicdata.accelerate3.y, nextdata.hpower * vecdir.z);
                nextdata.startspeed = Quaternion.AngleAxis(nextdata.angle * (-CheckStep(predicdata.hpower)), Vector3.up) * nextdata.startspeed;
                nextdata.flytime = 2 * nextdata.startspeed.y / (-nextdata.accelerate3.y);
                float heightime = Mathf.Abs(predicdata.startspeed.y) / Mathf.Abs(predicdata.accelerate3.y);
                nextdata.height = predicdata.startpos.y + 0.5f * Mathf.Abs(predicdata.accelerate3.y) * heightime * heightime;
                nextdata.endspeed = nextdata.startspeed + nextdata.accelerate3 * nextdata.flytime;
                nextdata.endpos = nextdata.startpos + nextdata.startspeed * nextdata.flytime + 0.5f * nextdata.accelerate3 * nextdata.flytime * nextdata.flytime;
                nextdata.recordtime = nextdata.flytime;
            }
            else
            {
                nextdata.startpos = predicdata.endpos;
                nextdata.startspeed = predicdata.endspeed * 0.75f;
                nextdata.startspeed.y = 0;
                nextdata.endspeed = nextdata.startspeed;
                nextdata.angle = 0;
                //nextdata.accelerate3 = Vector3.zero;
                nextdata.accelerate3 = -nextdata.startspeed * 0.2f;
                nextdata.flytime =  predicdata.recordtime*5;
                nextdata.recordtime = nextdata.flytime;
                //nextdata.endpos = nextdata.startpos + nextdata.startspeed * nextdata.flytime;
                nextdata.endpos = MathHelper.CalculatePos(nextdata.startpos, nextdata.startspeed, nextdata.accelerate3, nextdata.flytime);
            }
        }

        if(nextdata.collided)
        {
            nextdata.recordtime = predicdata.recordtime;
        }
        else
        {
            nextdata.recordtime = nextdata.flytime;
        }
        nextdata.collideplane = null;
        return nextdata;
    }

    private float CheckStep(float fac)
    {
        if (fac > 0)
        {
            return 1;
        }
        else if (fac < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public PredicData CollideHandle(List<Plane> planelist, PredicData predicdata)
    {
        float firstrecordtime = 999;
        Plane firstcollideplane = null;
        foreach (Plane plane in planelist)
        {
            if (plane.CheckFocus(predicdata, out float focustime))
            {
                if ((focustime < firstrecordtime)&&(focustime>0.0001f)&&(focustime<=predicdata.flytime))
                {
                    firstrecordtime = focustime;
                    firstcollideplane = plane;
                }
            }
        }
        
        if (firstcollideplane != null && MathHelper.CalculatePos(predicdata.startpos, predicdata.startspeed, predicdata.accelerate3, firstrecordtime).y >= 0)
        {
            predicdata.collideplane = firstcollideplane;
            predicdata.flytime = firstrecordtime;
            predicdata.endspeed = MathHelper.CalculateSpeed(predicdata.startspeed, predicdata.accelerate3, predicdata.flytime);
            predicdata.endpos = MathHelper.CalculatePos(predicdata.startpos, predicdata.startspeed, predicdata.accelerate3, predicdata.flytime);
            float heightime = Mathf.Abs(predicdata.startspeed.y) / Mathf.Abs(predicdata.accelerate3.y);
            predicdata.height = predicdata.startpos.y + 0.5f * Mathf.Abs(predicdata.accelerate3.y) * heightime * heightime;
        }
        else
        {
            predicdata.collideplane = null;
        }
        return predicdata;
    }
}

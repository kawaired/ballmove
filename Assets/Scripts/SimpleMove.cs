using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct pointmsg
{
    public Vector3 endpos;
    public float movetime;
}

public struct handledata
{
    public Vector3 curpos;
    public float starttime;
}

public class SimpleMove : MonoBehaviour
{
    private float starruntime = 2;
    public float G;
    [SerializeField]
    public List<pointmsg> points = new List<pointmsg>();
    private List<Func<float, Vector3>> funclist;
    private List<float> timedatas=new List<float>();
    float counttime = 0;
    private 

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[0].endpos;
        funclist = new List<Func<float, Vector3>>();
        //timedatas=new List<float>()
        timedatas.Add(starruntime);
        for (int i=1;i<points.Count;i++)
        {
            
            Vector3 startpos = points[i-1].endpos;
            Vector3 endpos = points[i].endpos;
            Vector3 vecdiff = (endpos - startpos);
            float hdis = Mathf.Sqrt(Mathf.Pow(vecdiff.x, 2) + Mathf.Pow(vecdiff.y, 2));
            float vdis = vecdiff.y;
            float totaltime = points[i].movetime;
            Func<float,Vector3> movefunc = new Func<float,Vector3>((curtime) => {
                //Debug.Log(hdis * (curtime / totaltime) * vecdiff);
                //Debug.Log((curtime / totaltime));
                Vector3 finvec = (curtime / totaltime) * vecdiff + startpos;
                //Debug.Log(finvec);
                finvec.y = CalculateYSpeed(vecdiff.y, totaltime, G) * curtime + 0.5f * G * curtime * curtime + startpos.y;
                return finvec;
            });
            funclist.Add(movefunc);
            starruntime += points[i].movetime;
            timedatas.Add(starruntime);
            //Debug.Log(starruntime);
        }
    }

    float CalculateYSpeed(float dis, float t, float a)
    {
        return (dis - 0.5f * a * t * t) / t;
    }

    // Update is called once per frame
    void Update()
    {
        int step = -1;
        float runedtime = 0;
        for(int i=0;i<timedatas.Count;i++)
        {
            //Debug.Log((counttime > timedatas[i]) && (counttime < timedatas[timedatas.Count - 1]));
            if((counttime > timedatas[i])&&(counttime<timedatas[timedatas.Count-1]))
            {
                step = i;
                runedtime = counttime - timedatas[i];
            }
            else
            {
                
                break;
            }
        }
        if(step>=0)
        {
            //Debug.Log(runedtime);
            transform.position = funclist[step](runedtime);
            Debug.Log(transform.position);
            //Debug.Log(funclist[step](runedtime));
        }
        counttime += Time.deltaTime;
        //Debug.Log(counttime);
    }

    //public float MoveFunction()
    //{

    //}
}

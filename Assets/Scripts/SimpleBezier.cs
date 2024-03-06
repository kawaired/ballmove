using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct BezierData
{
    public List<Vector3> PosList;
    public float spendtime;
}

public class SimpleBezier : MonoBehaviour
{
    [SerializeField]
    public List<BezierData> bezierdatas;

    List<float> recordtimes;

    private float starttime = 2;
    //public List<Vector3> OneBezier = new List<Vector3>();
    //public float onetotaltime = 3;
    // Start is called before the first frame update
    void Start()
    {
        float recordtime = 0;
        recordtimes = new List<float>();
        for(int i=0;i<bezierdatas.Count;i++)
        {
            recordtime += bezierdatas[i].spendtime;
            Debug.Log(recordtime);
            recordtimes.Add(recordtime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int step = -1;
        float steptime = 0;
        float movetime = Time.time - starttime;
        if (Time.time>starttime && movetime <= recordtimes[bezierdatas.Count - 1])
        {
            step = 0;
            
            for(int i=0;i<bezierdatas.Count;i++)
            {
                if(movetime>recordtimes[step])
                {
                    steptime = movetime - recordtimes[step];
                    step += 1;
                }
                else
                {
                    continue;
                }
            }
            //Debug.Log(step);
            transform.position = Bezier(bezierdatas[step], movetime - steptime);
            //bezierdatas[step]
        }
        
    }

    float MyFactorial(int a)
    {
        float answer = 1;
        for (int i = a; i > 0; i--)
        {
            answer = answer * i;
        }
        return answer;
    }

    Vector3 Bezier(BezierData bezierdata,float currenttime)
    {
        int poscount = bezierdata.PosList.Count;
        Vector3 finalpos = Vector3.zero;
        for(int i=0;i<poscount;i++)
        {
            Debug.Log(currenttime);
            finalpos += ParamentB(i, poscount - 1, currenttime/bezierdata.spendtime) * bezierdata.PosList[i];
        }
        return finalpos;
    }

    float ParamentB(int i,int n, float t)
    {
        return Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * MyFactorial(n) / (MyFactorial(i) * MyFactorial(n - i));
    }

}

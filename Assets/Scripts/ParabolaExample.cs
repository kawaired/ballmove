using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParabolaExample : MonoBehaviour
{
    [SerializeField]
    public List<ParaData> datalist = new List<ParaData>();
    [SerializeField]
     GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        //float timecount = 0;
        //Debug.Log(Time.deltaTime);
        //MoveFunc movefunc = new MoveFunc();
        //while (timecount < data.totaltime)
        //{
        //    Vector3 ballpos = movefunc.Parabola(data, timecount);
        //    ballpos=Quaternion.AngleAxis(data.angle, data.end.position - data.start.position)* ballpos ;
        //    timecount += Time.deltaTime;
        //    Instantiate<GameObject>(ball).transform.position = ballpos;
        //}
    }
}

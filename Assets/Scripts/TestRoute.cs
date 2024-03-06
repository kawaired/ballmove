using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRoute : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public float vpower;
    public float hpower;
    public float initspeed = 0;
    public Button btn;

    public float focustime = 0;
    //public float frametime;

    List<MoveFunc> mflist = new List<MoveFunc>();

    [SerializeField]
    public List<PlaneInputData> inputdatas;

    //[SerializeField]
    GameObject ball1;
    GameObject ball2;
    List<Plane> planelist;
    MovePredic mp;
    float total=0;

    public void Start()
    {
        GameObject ballres = Resources.Load<GameObject>("Ball");
        ball1 = Instantiate(ballres);
        ball2=Instantiate(ballres);
        planelist = new List<Plane>();

        foreach(PlaneInputData data in inputdatas)
        {
            planelist.Add(new Plane(data));
        }

        mp = new MovePredic(start.position, end.position, vpower, hpower, planelist);

        foreach(ParaData data in mp.paralist)
        {
            MoveFunc tempmf = new MoveFunc(data);
            total += tempmf.data.tfac;
            mflist.Add(tempmf);
        }

        btn.onClick.AddListener(() => {
            CalculatePos(ball2,focustime);
        });
    }

    float recordtime = -3;

    float speed = 1;
    public void Update()
    {
        if(recordtime>=0)
        {
            speed = CalculatePos(ball1, recordtime) * initspeed;
        }
        recordtime += (Time.deltaTime * speed);
        //Debug.Log(recordtime);
    }

    private float CalculatePos(GameObject obj,float temptime)
    {
        MoveFunc mf = mflist[0];
        //Debug.Log(total);
        if (temptime < total)
        {
            for (int i = 0; i < mflist.Count; i++)
            {
                mf = mflist[i];
                if (temptime > mf.data.tfac)
                {
                    temptime -= mf.data.tfac;
                }
                else
                {
                    break;
                }
            }
            obj.transform.position = mf.Parabola(temptime, mp.horivec);
            //Debug.Log(mf.data.tspeedatten);
        }
        return mf.data.tspeedatten;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(PlaneInputData data in transform.GetComponent<TestRoute>().inputdatas)
        {
            //Debug.Log(data.dir);
            Vector3 center = data.dir * data.len;
            Vector3 point0=new Vector3();
            Vector3 point1=new Vector3();
            Vector3 point2=new Vector3();
            Vector3 point3=new Vector3();
            int zerocount=0;
            if(center.x==0)
            {
                zerocount += 1;
            }
            if(center.y==0)
            {
                zerocount += 1;
            }
            if(center.z==0)
            {
                zerocount += 1;
            }

            switch (zerocount)
            {
                case 0:
                    point0 = center + 50 * new Vector3(-(1 / data.dir.x), 0, (1 / data.dir.z));
                    point1 = center + 50 * new Vector3((1 / data.dir.x), 0, -(1 / data.dir.z));
                    point2 = center + 50 * new Vector3(-(1 / data.dir.x), (1 / data.dir.y), 0);
                    point3 = center + 50 * new Vector3((1 / data.dir.x), -(1 / data.dir.y), 0);
                    break;
                case 1:
                    point0 = center + 50 * new Vector3((data.dir.x == 0) ? 1 : 0, (data.dir.y == 0) ? 1 : 0, (data.dir.z == 0) ? 1 : 0).normalized;
                    point1 = center + 50 * new Vector3((data.dir.x == 0) ? -1 : 0, (data.dir.y == 0) ? -1 : 0, (data.dir.z == 0) ? -1 : 0).normalized;

                    point2 = center + 50 * Vector3.Cross((point1 - point0).normalized, data.dir);
                    point3 = center + 50 * Vector3.Cross((point0 - point1).normalized, data.dir);
                    break;
                case 2:
                    point0 = center + 50 * new Vector3((data.dir.x == 0) ? 1 : 0, (data.dir.y == 0) ? 1 : 0, (data.dir.z == 0) ? 1 : 0).normalized;
                    point1 = center + 50 * new Vector3((data.dir.x == 0) ? -1 : 0, (data.dir.y == 0) ? -1 : 0, (data.dir.z == 0) ? -1 : 0).normalized;
                    point2 = center + 50 * Vector3.Cross((point1 - point0).normalized, data.dir);
                    point3 = center + 50 * Vector3.Cross((point0 - point1).normalized, data.dir);
                    break;
            }

            Gizmos.DrawLine(point0, point1);
            Gizmos.DrawLine(point0, point2);
            Gizmos.DrawLine(point0, point3);
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point1, point3);
            Gizmos.DrawLine(point2, point3);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class MassRoute : MonoBehaviour
{
    private static MovePredic mp;
    private static List<Plane> planelist;
    [MenuItem("Tools/Mass Draw Predic")]
    static void DrawMass()
    {
        CleanMass();
        GameObject ball = Resources.Load<GameObject>("Ball");
        GameObject predicobj = GameObject.Find("MassPredic");
        TestRoute testroute = predicobj.GetComponent<TestRoute>();
        planelist = new List<Plane>();
        foreach(PlaneInputData data in testroute.inputdatas)
        {
            planelist.Add(new Plane(data));
        }

        mp = new MovePredic(testroute.start.position,testroute.end.position,testroute.vpower,testroute.hpower,planelist);

        foreach(ParaData data in mp.paralist)
        {
            MoveFunc mf = new MoveFunc(data);
            GameObject box = new GameObject();
            box.name = "mass predic";
            float curdis = 0;
            while(curdis<mf.data.tfac)
            {
                Vector3 ballpos = mf.Parabola(curdis, mp.horivec);
                curdis += 0.02f * 10;
                GameObject realball = Instantiate<GameObject>(ball);
                realball.transform.position = ballpos;
                realball.transform.SetParent(box.transform);
            }
        }
    }

    [MenuItem("Tools/Clean Mass Predic")]
    static void CleanMass()
    {
        GameObject ballroute = GameObject.Find("mass predic");
        while (ballroute)
        {
            DestroyImmediate(ballroute);
            ballroute = GameObject.Find("mass predic");
        }
    }
}

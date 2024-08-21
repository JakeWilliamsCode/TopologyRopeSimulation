using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorList
{
    public Vector3 Item1 { get; set; }
    public Vector3 Item2 { get; set; }
    public Vector3 Item3 { get; set; }

    public VectorList(Vector3 item1, Vector3 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
    public VectorList(Vector3 item1, Vector3 item2, Vector3 item3)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }
}

public class VectorManager : MonoBehaviour
{

    double calcDistance(List<double> v1, List<double> v2)
    {
        List<double> temp = new List<double>(v1.Count);
        for (int i = 0; i < v1.Count; i++)
        {
            temp.Add(v1[i] - v2[i]);
        }
        double distance = 0.0;
        for (int i = 0; i < temp.Count; i++)
        {
            distance = distance + Mathf.Pow((float)temp[i], 2);
        }
        distance = Mathf.Sqrt((float)distance);
        return distance;
    }


    public Vector3 calcMidpoint(Vector3 v1, Vector3 v2)
    {
        return (v1 + v2) / 2.0f;
    }

    Vector3 calcDesiredPos(Vector3 v, Vector3 midpoint, double desiredLength, double distance)
    {
        return ((v - midpoint) * (float)(desiredLength / distance) + midpoint);
    }

    public VectorList ropeSegmentRecommendation(Vector3 v1, Vector3 v2, double desiredLength)
    {
        Vector3 midpoint = calcMidpoint(v1, v2);
        double distance = Vector3.Distance(v1, v2);

        //Debug.Log("distance" + distance);
        //Debug.Log(v1);
        //Debug.Log(v2);
        Vector3 Recv1 = calcDesiredPos(v1, midpoint, desiredLength, distance);
        Vector3 Recv2 = calcDesiredPos(v2, midpoint, desiredLength, distance);

        Vector3 deltav1 = Recv1 - v1;
        Vector3 deltav2 = Recv2 - v2;

        return new VectorList(deltav1, deltav2);
    }
    public VectorList ropeSegmentAngle(Vector3 p1, Vector3 p2, Vector3 p3, double desiredAngle)
    {
        
        Vector3 u = (p1 - p2).normalized;
        Vector3 v = (p3 - p2).normalized;
        float CosTheta = Mathf.Cos(Vector3.Angle(u, v));
        Vector3 n = (Vector3.Cross(u, v)).normalized;
        Vector3 w = Vector3.Cross(n, v);
        Vector3 x = Vector3.Cross(-1 * n, u);
        Vector3 y = -x - w;
        float scalingFactor = Mathf.Pow(CosTheta + 1, 2.0f);

        return new VectorList(x * scalingFactor, y * scalingFactor, w * scalingFactor);
    }

    public bool ropeSegmentAngleTest(Vector3 p1, Vector3 p2, Vector3 p3, float epsilon)
    {
        Vector3 u = (p1 - p2);
        Vector3 v = (p3 - p2);
        float Theta = Vector3.Angle(u, v);
        if (Mathf.Cos(Theta) > -1 + epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public VectorList ropeSegmentCollisionPointToPoint(Vector3 v1, Vector3 v2, float weight)
    {
        Vector3 difference = v1 - v2;

        Vector3 dNorm = difference.normalized;
        float distance = Vector3.Distance(v1, v2);

        v1 = v1 + distance * dNorm * weight;
        v2 = v2 - distance * dNorm * weight;
        
        return new VectorList(v1, v2);
    }

    //figure out how to properly split function, Calculate T, then do stuff later? or x1 and x2 at the same time then check? 
    public Vector3 ropeSegmentCollisionLineToLineCheck(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector3 p1 = a;
        Vector3 v1 = b - p1;
        Vector3 p2 = c;
        Vector3 v2 = d - p2;
        Vector3 n = Vector3.Cross(v1, v2);
        Vector3 n1 = Vector3.Cross(n, v1);
        float t = ((Vector3.Dot(n1 ,(p1 - p2))) / Vector3.Dot(n1 , v2));
        if (t < 0 || t > 1)
        {
            return new Vector3(0, 0, 0);
        }
        else
        {
            Vector3 x = p2 + t * v2;

            return x;
        }

        
    }



    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }
}
// Start is called before the first frame update
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TubeRendererInternals;


public class TubeTesting : MonoBehaviour
{

    public GameObject prefab;
    public int numPoints;

    public GameObject collisionPrefab;
    public GameObject collisionPrefab2;
    public GameObject collisionPrefab3;
    public GameObject collisionPrefab4;
    public GameObject collisionPrefab5;

    VectorManager VectorFunctions;
    float recStrength = .05f;
    float angleStrength = .0155f;
    float distanceApart = 5f;
    float desiredAngle = 90f;
    float epsilon = .1f;
    float ptpWeight = .1f;
    float lineToPointWeight = .025f;

    float radius = 1f;
    int pTorus = 2;
    int qTorus = 3;
    
    


    // Start is called before the first frame update
    GameObject[] PointArray;
    Vector3[] PointPositionsArray;
    TubeRenderer tube;

    private bool isDragging = false;
    private int selectedPointIndex = -1;
    public Camera mainCamera;
    float inputDistance = 8f;

    // refactor loops into their own functions soon
    void changePoints()
    {
        // The below loop is for point length
        for (int i = 0; i < PointArray.Length; i++)
        {
            VectorList Rec = VectorFunctions.ropeSegmentRecommendation(new Vector3(PointArray[i].transform.position.x, PointArray[i].transform.position.y, PointArray[i].transform.position.z), new Vector3(PointArray[((i + 1) % PointArray.Length)].transform.position.x, PointArray[((i + 1) % PointArray.Length)].transform.position.y, PointArray[((i + 1) % PointArray.Length)].transform.position.z), distanceApart);
            PointArray[i].transform.position = Rec.Item1 * recStrength + PointArray[i].transform.position;
            PointArray[((i+1) % PointArray.Length)].transform.position = Rec.Item2 * recStrength + PointArray[((i + 1) % PointArray.Length)].transform.position;
        }

        // The below loop is for angles
        for (int i = 0; i < PointArray.Length; i++)
            {
                bool AngleRec = VectorFunctions.ropeSegmentAngleTest(new Vector3(PointArray[i].transform.position.x, PointArray[i].transform.position.y, PointArray[i].transform.position.z), new Vector3(PointArray[((i + 1) % PointArray.Length)].transform.position.x, PointArray[((i + 1) % PointArray.Length)].transform.position.y, PointArray[((i + 1) % PointArray.Length)].transform.position.z), new Vector3(PointArray[((i + 2) % PointArray.Length)].transform.position.x, PointArray[((i + 2) % PointArray.Length)].transform.position.y, PointArray[((i + 2) % PointArray.Length)].transform.position.z), epsilon);
                if (AngleRec)
                {
                    VectorList Rec = VectorFunctions.ropeSegmentAngle(new Vector3(PointArray[i].transform.position.x, PointArray[i].transform.position.y, PointArray[i].transform.position.z), new Vector3(PointArray[((i + 1) % PointArray.Length)].transform.position.x, PointArray[((i + 1) % PointArray.Length)].transform.position.y, PointArray[((i + 1) % PointArray.Length)].transform.position.z), new Vector3(PointArray[((i + 2) % PointArray.Length)].transform.position.x, PointArray[((i + 2) % PointArray.Length)].transform.position.y, PointArray[((i + 2) % PointArray.Length)].transform.position.z), desiredAngle);
                    PointArray[i].transform.position = Rec.Item1 * angleStrength + PointArray[i].transform.position;
                    PointArray[((i + 1) % PointArray.Length)].transform.position = Rec.Item2 * angleStrength + PointArray[((i + 1) % PointArray.Length)].transform.position;
                    PointArray[((i + 2) % PointArray.Length)].transform.position = Rec.Item3 * angleStrength + PointArray[((i + 2) % PointArray.Length)].transform.position;
                }
            }
        // The below loop is for point collisions
        for (int i = 0; i < numPoints; i++)
        {

            //point to point
            for (int j = 0; j < i; j++)
            {
                float DistBetweenPoints = Vector3.Distance(PointArray[i].transform.position, PointArray[j].transform.position);
                if (DistBetweenPoints > 5)
                {
                    continue;
                }
                else if (DistBetweenPoints < 2 * radius)
                {
                    Instantiate(collisionPrefab, PointArray[i].transform.position, Quaternion.identity);
                    Instantiate(collisionPrefab2, PointArray[j].transform.position, Quaternion.identity);
                    VectorList positionRec = VectorFunctions.ropeSegmentCollisionPointToPoint(PointArray[i].transform.position, PointArray[j].transform.position, ptpWeight);
                    Debug.Log("Point Collision!!");
                    PointArray[i].transform.position = positionRec.Item1;
                    PointArray[j].transform.position = positionRec.Item2;
                }
            }
           // point to line
            for (int j = 0; j < numPoints -1; j++){
                Vector3 u = PointArray[j].transform.position;
                Vector3 v = PointArray[j + 1].transform.position;
                Vector3 w = PointArray[i].transform.position;
                if (j == i || j+1 == i)
                {
                    continue;
                }

                
                Vector3 W = w - u;
                Vector3 V = v - u;
                Vector3 p = (Vector3.Dot(W, V)/Vector3.Dot(V, V)) * V + u;
                float Direction = Vector3.Dot((p - u), (p - v));


                
                if (Direction < 0){
                    if((w-p).magnitude < 2 * radius)
                    {
                        Instantiate(collisionPrefab3, PointArray[j].transform.position, Quaternion.identity);
                        Instantiate(collisionPrefab4, PointArray[j + 1].transform.position, Quaternion.identity);
                        Instantiate(collisionPrefab5, PointArray[i].transform.position, Quaternion.identity);
                        Debug.Log("PointToLine Collision Changed!!");
                        // move u, v away from w
                        //VectorFunctions.ropeSegmentRecommendation()
                        
                        VectorList positionRec = VectorFunctions.ropeSegmentCollisionPointToPoint(u, w, lineToPointWeight);
                        PointArray[j].transform.position = positionRec.Item1;
                        positionRec = VectorFunctions.ropeSegmentCollisionPointToPoint(v, w, lineToPointWeight);
                        PointArray[j + 1].transform.position = positionRec.Item1;
                        positionRec = VectorFunctions.ropeSegmentCollisionPointToPoint(w, VectorFunctions.calcMidpoint(u, v), lineToPointWeight);
                        PointArray[i].transform.position = positionRec.Item1;
                    }
                }
            }
            
        }
        for (int i = 0; i < numPoints - 4; i++)
        {
            // Split this up into a function to get called twice for x1, x2, then solve

            for (int j = 2; j < numPoints - 2; j++)
            {
                Vector3 x2 = VectorFunctions.ropeSegmentCollisionLineToLineCheck(PointArray[i].transform.position, PointArray[i + 1].transform.position, PointArray[j].transform.position, PointArray[j + 1].transform.position);
                if(x2 == Vector3.zero)
                {
                    continue;
                }

                Vector3 x1 = VectorFunctions.ropeSegmentCollisionLineToLineCheck(PointArray[j].transform.position, PointArray[j + 1].transform.position, PointArray[i].transform.position, PointArray[i + 1].transform.position);
                //if(Mathf.Abs(x1 - x2) < 2 * radius)
                //{
                    // fix the points
                //}
                
            }
        }
        for (int i = 0; i < PointArray.Length; i++)
        {
            PointPositionsArray[i] = PointArray[i].transform.position;
        }
        PointPositionsArray[PointArray.Length] = PointArray[0].transform.position;

        if (Input.GetMouseButtonDown(0))
        {

        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast to find the point
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float closestDistance = inputDistance;
            int closestIndex = -1;

            for (int i = 0; i < PointArray.Length; i++)
            {
                Vector3 pointPosition = PointArray[i].transform.position;
                float distanceToRay = Vector3.Cross(ray.direction, pointPosition - ray.origin).magnitude;

                if (distanceToRay < closestDistance)
                {
                    closestDistance = distanceToRay;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                isDragging = true;
                selectedPointIndex = closestIndex;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop dragging
            isDragging = false;
            selectedPointIndex = -1;
        }

        if (isDragging && selectedPointIndex != -1)
        {
            // Update point position based on mouse movement
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero); // Assuming y-axis as up
            float distance = 10f;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(10f);
                PointArray[selectedPointIndex].transform.position = point;
                PointPositionsArray[selectedPointIndex] = point;
            }
        }
    }


    void Start()
    {

        //Following code creates arrays to hold the points the rope is made out of
        PointArray = new GameObject[numPoints];
        PointPositionsArray = new Vector3[numPoints + 1];

        for (var i = 0; i < numPoints; i++)
        {
            float t = ((i) * 2 * Mathf.PI / (numPoints));
            float scalar = 10 / (Mathf.Sqrt(2) - Mathf.Sin(qTorus * t));
            // make 2, 2, 3 to variables
            PointArray[i] = Instantiate(prefab, new Vector3(scalar * Mathf.Cos(pTorus * t), scalar*Mathf.Sin(pTorus * t), scalar *Mathf.Cos(qTorus*t)), Quaternion.identity);
            PointPositionsArray[i] = PointArray[i].transform.position;
            //Debug.Log(PointPositionsArray[i]);
        }
        // Make this better later, more organized
        float avg = 0f;
        for(int i = 0; i < numPoints; i++)
        {
            avg += (PointArray[i].transform.position - PointArray[(i+1) % numPoints].transform.position).magnitude;
        }
        avg = avg / numPoints;
        distanceApart = avg;
        Debug.Log(distanceApart);

        PointPositionsArray[numPoints] = PointPositionsArray[0];
        //this line above, sets the last point to the first point, so it will render as a loop.

        VectorFunctions = GameObject.Find("GameObject").GetComponent<VectorManager>();

        //InvokeRepeating("changePoints", .25f, .25f);
        
        tube = gameObject.AddComponent<TubeRenderer>();
        tube.MarkDynamic();


        
    }



    // Update is called once per frame
    void Update()
    {
        tube.points = PointPositionsArray;

        changePoints();

        HandleMouseInput();

        tube.radiuses = new float[] { radius };
        
        tube.ForceUpdate();
    }
}

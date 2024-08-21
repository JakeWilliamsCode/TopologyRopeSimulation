using UnityEngine;

public class CubeController : MonoBehaviour
{
    public GameObject cubePrefab;
    private GameObject selectedCube;
    private bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedCube == null)
            {
                CreateCube();
            }
            else
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && selectedCube != null)
        {
            MoveCube();
        }
    }

    void CreateCube()
    {
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * 10f;
        selectedCube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
    }

    void MoveCube()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 planePoint = ray.GetPoint(15f);  // Get the point 10 units away from the camera along the ray

        // Move the cube to the calculated point while keeping it 10 units away from the camera
        selectedCube.transform.position = planePoint;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLISION DETECTED!!");
        if (other.CompareTag("RopePoint"))
        {
            Debug.Log("Cube intersected with a point that has the tag 'SpecificTag'");
        }
    }
}

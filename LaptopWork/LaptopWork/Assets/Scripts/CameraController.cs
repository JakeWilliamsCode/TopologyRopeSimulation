using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float rotationSpeed = 5.0f;
    
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float raycastDistance = 1000f;

    private LineRenderer lineRenderer;

    void Start()
    {
        // Hide and lock the cursor at the start of the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

    }
    // Update is called once per frame
    void Update()
    {

        
        float horizontal = Input.GetAxis("Horizontal"); // A and D keys
        float vertical = Input.GetAxis("Vertical"); // W and S keys

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        movement = transform.TransformDirection(movement);
        transform.position += movement * moveSpeed * Time.deltaTime;


        yaw += rotationSpeed * Input.GetAxis("Mouse X");
        pitch -= rotationSpeed * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (Input.GetMouseButtonDown(0))
        {
            ShootRaycast();
        }
    }
    void ShootRaycast()
    {
        Vector3 offSetPosition = new Vector3(0f, 0.5f, 0f);
        Ray ray = new Ray(transform.position - offSetPosition, transform.forward);
        RaycastHit hit;

        // Enable the LineRenderer to make the ray visible
        lineRenderer.enabled = true;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // If the ray hits an object, draw a line to the hit point
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // If the ray doesn't hit an object, draw a line to the maximum distance
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * raycastDistance);
        }

        // Disable the LineRenderer after a short delay
        Invoke("HideRay", 1f);
    }

    void HideRay()
    {
        lineRenderer.enabled = false;
    }
}

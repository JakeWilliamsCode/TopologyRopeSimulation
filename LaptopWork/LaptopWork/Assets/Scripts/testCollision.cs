using UnityEngine;

public class testCollision : MonoBehaviour
{
    // This function is called when another collider enters the trigger collider attached to the object
    void OnCollisionEnter(Collision collision)
    {
        // Print a message to the console
        Debug.Log("Collision detected with " + collision.gameObject.name);
    }
}

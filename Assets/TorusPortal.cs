using UnityEngine;
using UnityEngine.SceneManagement;

public class TorusPortal : MonoBehaviour
{
    public string nextSceneName = "NextLevel"; // Set in Inspector, or leave blank to just move player
    public Transform teleportTarget; // Optional: assign a Transform to move player instead of loading scene

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TorusPortal: Triggered by " + other.name);
        if (other.CompareTag("Player"))
        {
            if (teleportTarget != null)
            {
                Debug.Log("TorusPortal: Teleporting player to target position");
                other.transform.position = teleportTarget.position;
                // Optionally reset velocity if using Rigidbody
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null) rb.velocity = Vector3.zero;
            }
            else
            {
                Debug.LogWarning("TorusPortal: No teleport target set!");
            }
        }
    }
}

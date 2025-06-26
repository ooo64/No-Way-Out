using UnityEngine;

public class KeyCollector : MonoBehaviour
{
    public bool hasRedKey = false;
    public bool hasBlueKey = false;
    public bool hasGreenKey = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered: " + other.name);
        if (other.CompareTag("RedKey"))
        {
            hasRedKey = true;
            Destroy(other.gameObject);
            Debug.Log("Red Key collected!");
        }
        else if (other.CompareTag("BlueKey"))
        {
            hasBlueKey = true;
            Destroy(other.gameObject);
            Debug.Log("Blue Key collected!");
        }
        else if (other.CompareTag("GreenKey"))
        {
            hasGreenKey = true;
            Destroy(other.gameObject);
            Debug.Log("Green Key collected!");
        }
    }

    public bool HasAllKeys()
    {
        return hasRedKey && hasBlueKey && hasGreenKey;
    }
}

using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public enum KeyType { Red, Blue, Green }
    public KeyType keyType;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KeyCollector collector = other.GetComponent<KeyCollector>();
            if (collector != null)
            {
                switch (keyType)
                {
                    case KeyType.Red:
                        collector.hasRedKey = true;
                        Debug.Log("Red Key collected!");
                        break;
                    case KeyType.Blue:
                        collector.hasBlueKey = true;
                        Debug.Log("Blue Key collected!");
                        break;
                    case KeyType.Green:
                        collector.hasGreenKey = true;
                        Debug.Log("Green Key collected!");
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}

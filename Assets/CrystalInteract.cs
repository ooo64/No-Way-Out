using UnityEngine;

public class CrystalInteract : MonoBehaviour
{
    public enum CrystalType { Red, Blue, Green }
    public CrystalType crystalType;
    public CrystalManager crystalManager;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        KeyCollector collector = other.GetComponent<KeyCollector>();
        if (collector != null && crystalManager != null)
        {
            Debug.Log($"CrystalInteract: Calling TryBreakCrystal for {gameObject.name}");
            crystalManager.TryBreakCrystal(gameObject, collector);
        }
        else
        {
            Debug.LogWarning("CrystalInteract: Missing KeyCollector or CrystalManager reference!");
        }
    }
}

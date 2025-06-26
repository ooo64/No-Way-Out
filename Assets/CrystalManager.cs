using UnityEngine;
using UnityEngine.SceneManagement;

public class CrystalManager : MonoBehaviour
{
    public GameObject redCrystal;
    public GameObject blueCrystal;
    public GameObject greenCrystal;
    public GameObject torusPortal;

    private int crystalsDestroyed = 0;
    private bool portalActivated = false;

    void Start()
    {
        if (torusPortal != null)
            torusPortal.SetActive(false);
    }

    public void TryBreakCrystal(GameObject crystal, KeyCollector collector)
    {
        Debug.Log($"CrystalManager: TryBreakCrystal called for {crystal.name}");
        if (crystal == redCrystal && collector.hasRedKey)
        {
            Debug.Log("CrystalManager: Breaking Red Crystal");
            Destroy(redCrystal);
            crystalsDestroyed++;
        }
        else if (crystal == blueCrystal && collector.hasBlueKey)
        {
            Debug.Log("CrystalManager: Breaking Blue Crystal");
            Destroy(blueCrystal);
            crystalsDestroyed++;
        }
        else if (crystal == greenCrystal && collector.hasGreenKey)
        {
            Debug.Log("CrystalManager: Breaking Green Crystal");
            Destroy(greenCrystal);
            crystalsDestroyed++;
        }
        else
        {
            Debug.Log("CrystalManager: Player does not have the required key for this crystal.");
        }
        CheckPortal();
    }

    public void BreakCrystalWithSpell(GameObject crystal)
    {
        Debug.Log($"CrystalManager: BreakCrystalWithSpell called for {crystal?.name}");
        if (crystal != null)
        {
            if (crystal == redCrystal) Debug.Log("CrystalManager: Spell breaking Red Crystal");
            if (crystal == blueCrystal) Debug.Log("CrystalManager: Spell breaking Blue Crystal");
            if (crystal == greenCrystal) Debug.Log("CrystalManager: Spell breaking Green Crystal");
            Destroy(crystal);
            crystalsDestroyed++;
            CheckPortal();
        }
    }

    void CheckPortal()
    {
        Debug.Log($"CrystalManager: {crystalsDestroyed} crystals destroyed");
        if (!portalActivated && crystalsDestroyed >= 3)
        {
            Debug.Log("CrystalManager: Activating portal!");
            if (torusPortal != null)
                torusPortal.SetActive(true);
            portalActivated = true;
        }
    }
}

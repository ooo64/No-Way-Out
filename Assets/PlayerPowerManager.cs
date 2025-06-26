using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPowerManager : MonoBehaviour
{
    public int maxPH = 100;
    public int currentPH;

    public int visionCost = 40;
    public int breakCost = 50;
    public int teleportCost = 80;

    public int visionUses = 0;
    public int maxVisionUses = 2;

    public ParticleSystem visionFX;
    public ParticleSystem breakFX;
    public ParticleSystem teleportFX;

    public GameObject[] hiddenObjects; // for vision hint
    public CrystalManager crystalManager; // Assign in Inspector
    public float breakSpellRange = 10f; // Max distance to break a crystal

    private bool showMenu = false;
    private bool isGameOver = false;
    private int menuIndex = 0; // For keyboard navigation
    private readonly string[] menuOptions = { "Resume", "Restart", "Quit" };

    void Start()
    {
        currentPH = maxPH;
    }

    void Update()
    {
        if (currentPH <= 0)
        {
            currentPH = 0;
            isGameOver = true;
            showMenu = true;
            Time.timeScale = 0f;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu = !showMenu;
            if (showMenu)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
        if (showMenu)
        {
            // Keyboard navigation
            if (!isGameOver) // Only show Resume if not game over
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    menuIndex = (menuIndex + menuOptions.Length - 1) % menuOptions.Length;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    menuIndex = (menuIndex + 1) % menuOptions.Length;
                }
            }
            else // Game over: only Restart and Quit
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    menuIndex = menuIndex == 1 ? 0 : 1;
                }
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                HandleMenuSelection();
            }
        }
        if (!showMenu && !isGameOver)
        {
            Time.timeScale = 1f;
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseVision();
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseBreakSpell();
            if (Input.GetKeyDown(KeyCode.Alpha3)) UseTeleport();
        }
    }

    void HandleMenuSelection()
    {
        if (!isGameOver)
        {
            if (menuIndex == 0) // Resume
            {
                showMenu = false;
                Time.timeScale = 1f;
            }
            else if (menuIndex == 1) // Restart
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (menuIndex == 2) // Quit
            {
                Application.Quit();
            }
        }
        else
        {
            // Game over: only Restart and Quit
            if (menuIndex == 0) // Restart
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (menuIndex == 1) // Quit
            {
                Application.Quit();
            }
        }
    }

    void UseVision()
    {
        if (visionUses < maxVisionUses && currentPH >= visionCost)
        {
            visionUses++;
            currentPH -= visionCost;

            visionFX.Play(); // Trigger particle FX
            Debug.Log("Used Vision Power");

            StartCoroutine(ShowHints());
        }
        else
        {
            Debug.Log("Not enough PH or max vision uses reached.");
        }
    }

    void UseBreakSpell()
    {
        if (currentPH >= breakCost)
        {
            currentPH -= breakCost;
            breakFX.Play(); // Trigger red burst
            Debug.Log("Break Spell triggered!");
            // Find and break nearest crystal
            if (crystalManager != null)
            {
                GameObject nearest = FindNearestCrystal();
                if (nearest != null)
                {
                    crystalManager.BreakCrystalWithSpell(nearest);
                }
                else
                {
                    Debug.Log("No crystals in range to break.");
                }
            }
            // TODO: Trigger stone break logic
        }
        else
        {
            Debug.Log("Not enough PH for Break Spell.");
        }
    }

    void UseTeleport()
    {
        if (currentPH >= teleportCost)
        {
            currentPH -= teleportCost;
            teleportFX.Play(); // Teleport FX
            Debug.Log("Teleport Spell activated!");
            // TODO: Move player to teleport gate
        }
        else
        {
            Debug.Log("Not enough PH for Teleportation.");
        }
    }

    System.Collections.IEnumerator ShowHints()
    {
        foreach (GameObject obj in hiddenObjects)
        {
            obj.SetActive(true); // Temporarily highlight
        }
        yield return new WaitForSeconds(3f);
        foreach (GameObject obj in hiddenObjects)
        {
            obj.SetActive(false); // Hide again
        }
    }

    void OnGUI()
    {
        // PH
        GUI.Label(new Rect(10, 10, 200, 20), "PH: " + currentPH);

        // Keys collected
        KeyCollector kc = GetComponent<KeyCollector>();
        int keys = 0;
        if (kc != null)
        {
            if (kc.hasRedKey) keys++;
            if (kc.hasBlueKey) keys++;
            if (kc.hasGreenKey) keys++;
            GUI.Label(new Rect(10, 30, 200, 20), "Keys: " + keys + "/3");
        }
        else
        {
            GUI.Label(new Rect(10, 30, 200, 20), "Keys: N/A");
        }

        // Vision Power uses left
        string visionStatus = (maxVisionUses - visionUses) > 0 && currentPH >= visionCost ? (maxVisionUses - visionUses) + " left" : "Not available";
        GUI.Label(new Rect(10, 50, 250, 20), "Vision Power: " + visionStatus);

        // Break Spell
        string breakStatus = currentPH >= breakCost ? "Ready" : "Not enough PH";
        GUI.Label(new Rect(10, 70, 250, 20), "Break Spell: " + breakStatus);

        // Teleport Spell
        string teleportStatus = currentPH >= teleportCost ? "Ready" : "Not enough PH";
        GUI.Label(new Rect(10, 90, 250, 20), "Teleport Spell: " + teleportStatus);

        // Game Over
        if (currentPH <= 0)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2, 200, 40), "GAME OVER");
        }
        // Menu
        if (showMenu)
        {
            int optionCount = isGameOver ? 2 : 3;
            string[] options = isGameOver ? new string[] { "Restart", "Quit" } : menuOptions;
            GUI.Box(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 60, 150, 120), isGameOver ? "Game Over" : "Menu");
            for (int i = 0; i < optionCount; i++)
            {
                Rect btnRect = new Rect(Screen.width / 2 - 55, Screen.height / 2 - 30 + i * 30, 110, 30);
                bool selected = (menuIndex == i);
                GUI.SetNextControlName("MenuOption" + i);
                if (GUI.Button(btnRect, (selected ? "> " : "") + options[i]))
                {
                    menuIndex = i;
                    HandleMenuSelection();
                }
            }
            // Set keyboard focus to selected button
            GUI.FocusControl("MenuOption" + menuIndex);
        }
    }

    GameObject FindNearestCrystal()
    {
        GameObject[] crystals = new GameObject[] { crystalManager.redCrystal, crystalManager.blueCrystal, crystalManager.greenCrystal };
        GameObject nearest = null;
        float minDist = breakSpellRange;
        foreach (GameObject c in crystals)
        {
            if (c != null)
            {
                float dist = Vector3.Distance(transform.position, c.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = c;
                }
            }
        }
        return nearest;
    }
}

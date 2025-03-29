using UnityEngine;

public class HeadsetPromptTrigger : MonoBehaviour
{
    public GameObject uiCanvas;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the trigger: " + other.name);

        // Check if the object has a CharacterController (usually the player)
        if (other.GetComponent<CharacterController>())
        {
            Debug.Log("Player (with CharacterController) entered the trigger.");

            // ✅ Show UI canvas if assigned
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(true);
                Debug.Log("Canvas should now be active. Active state = " + uiCanvas.activeSelf);
            }
            else
            {
                Debug.LogWarning("uiCanvas reference is missing in the Inspector!");
            }

            // ✅ Tell GazeSpatialLogger to log total playtime
            GazeSpatialLogger logger = FindObjectOfType<GazeSpatialLogger>();
            if (logger != null)
            {
                logger.LogPlaytimeAndClose();
                Debug.Log("GazeSpatialLogger: Logged total playtime.");
            }
            else
            {
                Debug.LogWarning("GazeSpatialLogger not found in the scene!");
            }
        }
    }
}

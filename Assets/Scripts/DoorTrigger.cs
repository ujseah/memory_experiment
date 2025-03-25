using UnityEngine;

public class HeadsetPromptTrigger : MonoBehaviour
{
    public GameObject uiCanvas;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the trigger: " + other.name);

        // Check if the object that entered has a CharacterController
        if (other.GetComponent<CharacterController>())
        {
            Debug.Log("Player (with CharacterController) entered the trigger.");

            // Try activating the canvas
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(true);
                Debug.Log("Canvas should now be active. Active state = " + uiCanvas.activeSelf);
            }
            else
            {
                Debug.LogWarning("uiCanvas reference is missing in the Inspector!");
            }
        }
    }
}

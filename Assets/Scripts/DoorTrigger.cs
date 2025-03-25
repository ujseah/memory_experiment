using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject uiCanvas;

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone has a CharacterController (i.e. is the player)
        if (other.GetComponent<CharacterController>())
        {
            uiCanvas.SetActive(true);
        }
    }
}

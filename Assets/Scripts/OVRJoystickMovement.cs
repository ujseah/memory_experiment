using UnityEngine;

public class OVRJoystickMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float turnSpeed = 60.0f; // degrees per second
    public Transform cameraTransform; // OVRCameraRig's HMD
    public CharacterController characterController;

    void Update()
    {
        // Get input from left joystick for movement
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // Move relative to the headset's forward direction
        Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        moveDirection.y = 0; // Stay horizontal
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Get input from right joystick for rotation
        Vector2 turnInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float turnAmount = turnInput.x * turnSpeed * Time.deltaTime;

        // Rotate the parent object (PlayerMover)
        transform.Rotate(0, turnAmount, 0);
    }
}



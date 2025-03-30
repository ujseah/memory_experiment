using UnityEngine;

public class HeadProximityBounce : MonoBehaviour
{
    public Transform playerRig;           // XR Rig root (e.g., PlayerMover)
    public Transform headTransform;       // VR headset (e.g., CenterEyeAnchor)
    public float pushRadius = 0.3f;       // Personal space radius around head
    private LayerMask obstacleLayer;      // Layer to detect furniture/walls

    void Start()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");

        if (playerRig == null)
            Debug.LogWarning("[Bounce] Missing playerRig reference!");
        if (headTransform == null)
            Debug.LogWarning("[Bounce] Missing headTransform reference!");

        Debug.Log("[Bounce] Script initialized. pushRadius = " + pushRadius);
    }

    void Update()
    {
        if (playerRig == null || headTransform == null)
        {
            Debug.LogWarning("[Bounce] Missing references. Skipping update.");
            return;
        }

        Debug.Log("[Bounce] Head position: " + headTransform.position);

        Collider[] hits = Physics.OverlapSphere(headTransform.position, pushRadius, obstacleLayer);
        Debug.Log("[Bounce] Nearby obstacles found: " + hits.Length);

        foreach (Collider hit in hits)
        {
            Debug.Log("[Bounce] Colliding with: " + hit.name);

            Vector3 closestPoint = hit.ClosestPoint(headTransform.position);
            Vector3 pushDir = (headTransform.position - closestPoint).normalized;

            float distanceToSurface = Vector3.Distance(headTransform.position, closestPoint);
            float desiredDistance = pushRadius;
            float distanceToPush = desiredDistance - distanceToSurface;

            Debug.Log($"[Bounce] Distance to surface: {distanceToSurface:F3}, Distance to push: {distanceToPush:F3}");

            if (distanceToPush > 0f)
            {
                Vector3 push = pushDir * distanceToPush;
                push.y = 0f; // Stay on ground level

                playerRig.position += push;

                Debug.Log("[Bounce] Pushed player by: " + push);
            }
            else
            {
                Debug.Log("[Bounce] No push needed, already outside radius.");
            }
        }
    }

    // Optional: visualize in Scene view
    void OnDrawGizmosSelected()
    {
        if (headTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headTransform.position, pushRadius);
        }
    }
}

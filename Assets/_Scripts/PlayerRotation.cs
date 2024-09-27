using UnityEngine;

public class PlayerRotation : MonoBehaviour {
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask RotationMask;
    [SerializeField] private float rayLength = 1f;

    private void Update() {
        SnapToSlope();
    }

    private void SnapToSlope() {
        RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, Vector2.down, rayLength, RotationMask);

        if (hit.collider != null) {
            Vector2 groundNormal = hit.normal;
            // Calculate the angle between the upward direction of the player and the ground's normal
            float angle = Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg - 90f;

            // Instantly rotate the player to match the ground's angle
            playerTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        } else {
            playerTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}

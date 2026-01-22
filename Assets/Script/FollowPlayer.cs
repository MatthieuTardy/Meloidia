using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float heightAbovePlayer = 2f;
    public bool followRotation = true;

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Instantly follow the player position with height offset
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + heightAbovePlayer, player.position.z);
        transform.position = targetPosition;

        // Instantly follow the player rotation if enabled
        if (followRotation)
            transform.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
    }
}

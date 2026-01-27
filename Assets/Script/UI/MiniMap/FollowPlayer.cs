using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }
}
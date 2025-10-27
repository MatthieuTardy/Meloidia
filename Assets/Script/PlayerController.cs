using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{



    [Header("Move")]
    float vertical;
    float horizontal;
    public float speed;
    float fast;
    public Rigidbody body;
    public float maxZ = 4.2f;
    public float maxX = 8f;
    public float jumpForce;

    public LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer))
            {
                body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
 
        }
    }

    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0 && vertical != 0)
        {
            fast = speed/1.41f;
        }
        else
            fast = speed;
        body.velocity = new Vector3(horizontal * speed, body.velocity.y, vertical * fast);



    }
}

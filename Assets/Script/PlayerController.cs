using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody body;
    public Transform playerVisuals;
    public Transform cameraTransform;
    public ParticleSystem sprintParticles;
    
    [Tooltip("Glisse ton composant Animator ici")]
    public Animator animator; 

    [Header("Mouvement")]
    public float speed = 5f;
    public LayerMask groundLayer;
    [Tooltip("Contrôle la glissade à l'arrêt. 0 = arrêt net. 0.2 = légère glissade.")]
    public float decelerationSmoothness = 0.15f;

    [Header("Saut & Physique")]
    public float jumpForce = 25f;
    public float jumpCooldown = 0.5f;
    public float jumpDelay = 0.1f;
    public float fallMultiplier = 3f;
    public float jumpHoldTime = 0.3f;
    public float maxJumpHoldForce = 35f;
    
    private float lastJumpTime;
    private float jumpHoldTimer;
    private bool isJumping;

    [Header("Sprint")]
    public float sprintSpeed = 10f;
    private bool isSprinting;

    [Header("Rotation")]
    public float rotationSpeed = 720f;
    public float rotationSmoothTime = 0.1f;

    [Header("Idle Spécial (AFK)")]
    public float minIdleTime = 5f; 
    public float maxIdleTime = 10f;
    private float idleTimer;
    private float currentIdleThreshold;

    private Vector3 inputDirection;
    private Vector3 lastValidDirection;
    private Vector3 stoppingVelocity;
    private bool isGrounded;
    private float currentRotationVelocity;
    private float targetRotation;
    private float currentRotation;

    private CameraShake _cameraShake;


    void OnSomething()
    {
        if (_cameraShake != null)
        {
            _cameraShake.Shake();
        }
        else
        {
            Debug.LogWarning("Impossible de trouver le composant CameraShake dans la scène !");
        }
    }

    void Start()
    {
        _cameraShake = FindObjectOfType<CameraShake>();
        OnSomething();
        if (body == null) body = GetComponent<Rigidbody>();
        if (playerVisuals == null) playerVisuals = transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

        if(playerVisuals != transform)
        {
            playerVisuals.localPosition = Vector3.zero;
            playerVisuals.localRotation = Quaternion.identity;
        }

        currentRotation = transform.eulerAngles.y;
        body.freezeRotation = true;

        ResetIdleTimer();
    }

    void Update()
    {
        //if (GameManager.Instance.playerManager.Lock)
        {
            HandleInputs();
            CheckGrounded();

            if (GameManager.Instance.playerManager.Lock)
            {
                if (Input.GetButtonDown("Jump") && isGrounded && Time.time >= lastJumpTime + jumpCooldown)
                {
                    Jump();
                }
                else if (Input.GetButton("Jump") && isJumping)
                {
                    jumpHoldTimer += Time.deltaTime;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    jumpHoldTimer = 0f;
                    isJumping = false;
                }

                HandleAnimations();
                if (!isGrounded)
                {
                    HandleMovement();
                }
                HandleRotation();
            }
            else
            {
                animator.SetBool("isgrounded", true);
                animator.SetBool("iswalking", false);
                animator.SetBool("isidle", true);
                animator.SetBool("isjumping", false);
            }

            //HandleToolsInput();
            HandleSprintVisuals();
        }
    }

    void FixedUpdate()
    {
        ApplyBetterGravity();
    }

    private void ApplyBetterGravity()
    {
        if (body.velocity.y < 0)
        {
            body.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (body.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            body.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier * 0.5f) * Time.fixedDeltaTime;
        }
    }

    private void HandleInputs()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        inputDirection = (forward * vertical + right * horizontal);

        if (inputDirection.magnitude >= 0.1f)
        {
            inputDirection.Normalize();
            lastValidDirection = inputDirection;
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && Input.GetKey(KeyCode.Alpha7))
        {
            if (animator != null)
            {
                animator.SetTrigger("is67");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad6) && Input.GetKey(KeyCode.Keypad7))
        {
            if (animator != null)
            {
                animator.SetTrigger("is67");
            }
        }

        isSprinting = Input.GetButton("Fire3");
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        bool isMoving = inputDirection.magnitude >= 0.1f;

        animator.SetBool("isgrounded", isGrounded);
        animator.SetBool("iswalking", isMoving);
        animator.SetBool("isidle", !isMoving);
        
        if (Time.time >= lastJumpTime + jumpCooldown) 
        {
            animator.SetBool("isjumping", !isGrounded);
        }

        float animSpeedMultiplier = 1f;
        if (isSprinting && isMoving && isGrounded)
        {
            animSpeedMultiplier = sprintSpeed / speed;
        }
        animator.SetFloat("animSpeed", animSpeedMultiplier);

        // Logique Idle Spécial
        if (!isMoving && isGrounded)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= currentIdleThreshold)
            {
                // Choisir aléatoirement entre les 2 idles
                int randomIdle = UnityEngine.Random.Range(0, 2);
                if (randomIdle == 0)
                {
                    animator.SetTrigger("specialIdle");
                }
                else
                {
                    animator.SetTrigger("specialIdle2");
                }
                ResetIdleTimer();
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }

    private void ResetIdleTimer()
    {
        idleTimer = 0f;
        currentIdleThreshold = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }

    private void HandleMovement(Vector2? floor = null)
    {
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        if (!GameManager.Instance.playerManager.ragdoll)
        {
            if (inputDirection.magnitude >= 0.1f)
            {
                // Retour au mouvement simple sans Raycast ni projection
                Vector3 targetVelocity = inputDirection * currentSpeed;
                body.velocity = new Vector3(targetVelocity.x, body.velocity.y, targetVelocity.z);
            }
            else 
            {
                Vector3 horizontalVelocity = new Vector3(body.velocity.x, 0, body.velocity.z);
                Vector3 smoothedHorizontal = Vector3.SmoothDamp(horizontalVelocity, Vector3.zero, ref stoppingVelocity, decelerationSmoothness);
                body.velocity = new Vector3(smoothedHorizontal.x, body.velocity.y, smoothedHorizontal.z);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        if (Vector2.Dot(Vector2.up, collision.contacts[0].normal) > 0.8f)
        {
            HandleMovement();

            
        }
            // On est bien sur un sol presque hori
        /*        else
        {
            float currentSpeed = isSprinting ? sprintSpeed : speed;
            Vector3 targetVelocity = inputDirection * currentSpeed;
            body.velocity = new Vector3(-targetVelocity.x, body.velocity.y, body.velocity.z);
        }
        */
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
    private void HandleSprintVisuals()
    {
        if (sprintParticles != null)
        {
            if (isSprinting && inputDirection.magnitude > 0.1f && isGrounded)
            {
                if (!sprintParticles.isPlaying) sprintParticles.Play();
            }
            else
            {
                if (sprintParticles.isPlaying) sprintParticles.Stop();
            }
        }
    }

    private void HandleRotation()
    {
        if (inputDirection.magnitude >= 0.1f)
        {
            currentRotation = Mathf.SmoothDampAngle(
                currentRotation,
                targetRotation,
                ref currentRotationVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
        }
    }

    private void CheckGrounded()
    {
       // isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    private void Jump()
    {
        lastJumpTime = Time.time;
        isJumping = true;
        jumpHoldTimer = 0f;

        if (animator != null)
        {
            animator.SetTrigger("jump");
            animator.SetBool("isjumping", true); 
            
            StopCoroutine("ResetJumpTriggerRoutine");
            StartCoroutine("ResetJumpTriggerRoutine");
        }

        StartCoroutine(DelayedJumpPhysics());
    }

    private IEnumerator DelayedJumpPhysics()
    {
        yield return new WaitForSeconds(jumpDelay);
        body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        
        float currentJumpForce = jumpForce;
        
        while (isJumping && jumpHoldTimer < jumpHoldTime)
        {
            yield return null;
            if (jumpHoldTimer > 0)
            {
                currentJumpForce = Mathf.Lerp(jumpForce, maxJumpHoldForce, jumpHoldTimer / jumpHoldTime);
            }
        }
        
        body.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
        
        if (sprintParticles != null)
        {
            sprintParticles.Play();
        }
    }

    private IEnumerator ResetJumpTriggerRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
        {
            animator.ResetTrigger("jump");
        }
    }

    /*
        private void HandleToolsInput()
        {
            // (Conditions d'input inchangées)
            if (Input.GetButtonDown("Outils 1") && GameManager.Instance.playerManager.outils != 0 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsY_Xbox") >= 0.8 && GameManager.Instance.playerManager.outils != 0 && GameManager.Instance.playerManager.havingTools == true)
            {
                SetTool(0, true, false, false, false);
            }
            else if (Input.GetButtonDown("Outils 2") && GameManager.Instance.playerManager.outils != 1 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsX_Xbox") >= 0.8 && GameManager.Instance.playerManager.outils != 1 && GameManager.Instance.playerManager.havingTools == true)
            {
                SetTool(1, false, true, false, false);
            }
            else if (Input.GetButtonDown("Outils 3") && GameManager.Instance.playerManager.outils != 2 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsY_Xbox") <= -0.8 && GameManager.Instance.playerManager.outils != 2 && GameManager.Instance.playerManager.havingTools == true)
            {
                SetTool(2, false, false, true, false);
            }
            else if (Input.GetButtonDown("Outils 4") && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsX_Xbox") <= -0.8 && GameManager.Instance.playerManager.havingTools == true && !GameManager.Instance.playerManager.isBuildMode)
            {
                if (GameManager.Instance.playerManager.outils != 3 && GameManager.Instance.playerManager.havingTools == true)
                {
                    GameManager.Instance.playerManager.outils = 3;
                    GameManager.Instance.playerManager.isBuildMode = true;
                    SetTool(3, false, false, false, true);
                }
                if (!GameManager.Instance.buildManager.isBuilding)
                {
                   // SetTool(0, true, false, false, false);
                }
                OnBuildMode.Invoke();
                Debug.Log("Build Mode Enable");
            }
        }
    */

    private void SetTool(int outilIndex, bool gant, bool pelle, bool arrosoir, bool marteau)
    {
        if(outilIndex >= 0) GameManager.Instance.playerManager.outils = outilIndex;
        GameManager.Instance.playerManager.ChangeSpriteToMainTool();
    }

    // --- ANIMATION CONSTRUCTION ---
    public void TriggerBuildAnimation()
    {
        if (animator != null)
        {
            StopCoroutine(BuildAnimRoutine());
            StartCoroutine(BuildAnimRoutine());
        }
    }

    private IEnumerator BuildAnimRoutine()
    {
        animator.SetBool("isbuilding", true);
        yield return new WaitForSeconds(0.5f); 
        animator.SetBool("isbuilding", false);
    }
    
    // --- ANIMATION ACTION (EAU / TERRE) ---
    public void TriggerActionAnimation()
    {
        if (animator != null)
        {
            StopCoroutine(ActionAnimRoutine());
            StartCoroutine(ActionAnimRoutine());
        }
    }

    private IEnumerator ActionAnimRoutine()
    {
        animator.SetBool("isaction", true);
        yield return new WaitForSeconds(0.5f); 
        animator.SetBool("isaction", false);
    }
}
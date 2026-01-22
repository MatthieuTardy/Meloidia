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

    [Header("Mouvement")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    [Tooltip("Contrôle la glissade à l'arrêt. 0 = arrêt net. 0.2 = légère glissade.")]
    public float decelerationSmoothness = 0.15f;

    [Header("Sprint")]
    public float sprintSpeed = 10f;
    private bool isSprinting;

    [Header("Rotation")]
    [Tooltip("Vitesse de rotation en degrés/sec")]
    public float rotationSpeed = 720f;
    [Tooltip("Temps de lissage pour la rotation")]
    public float rotationSmoothTime = 0.1f;

    [Header("Juice - Effets visuels")]
    public bool enableJuice = true;
    public float visualSmoothTime = 0.1f;

    [Header("Juice - Idle (Respiration)")]
    public float idleBreatheFrequency = 1f;
    public float idleBreatheAmplitude = 0.05f;

    [Header("Juice - Mouvement (Sautillement)")]
    public float bobbingFrequency = 10f;
    public float bobbingAmplitude = 0.1f;
    // AJOUT: Paramètres d'oscillation pour le sprint
    public float sprintBobbingFrequency = 15f;
    public float sprintBobbingAmplitude = 0.15f;


    [Header("Juice - Saut (Déformation)")]
    public Vector3 jumpSquash = new Vector3(1.25f, 0.75f, 1.25f);
    public Vector3 landSquash = new Vector3(1.5f, 0.5f, 1.5f);
    public float squashReturnSpeed = 8f;

    private Vector3 inputDirection;
    private Vector3 lastValidDirection;
    private Vector3 stoppingVelocity;
    private bool isGrounded;
    private Vector3 initialVisualsScale;
    private Vector3 targetVisualsPos;
    private Vector3 targetVisualsScale;
    private Vector3 visualsPosVelocity;
    private Vector3 visualsScaleVelocity;
    private bool isSquashing = false;
    private float currentRotationVelocity;
    private float targetRotation;
    private float currentRotation;



    void Start()
    {
        if (body == null) body = GetComponent<Rigidbody>();
        if (playerVisuals == null) playerVisuals = transform;
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

        initialVisualsScale = playerVisuals.localScale;
        targetVisualsScale = initialVisualsScale;
        targetVisualsPos = Vector3.zero;
        currentRotation = transform.eulerAngles.y;
        body.freezeRotation = true;
    }

    void Update()
    {
        HandleInputs();
        CheckGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (enableJuice)
        {
            CalculateJuiceTargets();
        }

        if (Input.GetButtonDown("Outils 1") && GameManager.Instance.playerManager.outils != 0 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsY_Xbox") >= 0.8 && GameManager.Instance.playerManager.havingTools == true)
        {
            GameManager.Instance.playerManager.outils = 0;
            GameManager.Instance.playerManager.Gant.SetActive(true);
            GameManager.Instance.playerManager.Pelle.SetActive(false);
            GameManager.Instance.playerManager.Houe.SetActive(false);
            GameManager.Instance.playerManager.Arrosoir.SetActive(false);
        }
        else if (Input.GetButtonDown("Outils 2") && GameManager.Instance.playerManager.outils != 1 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsX_Xbox") >= 0.8 && GameManager.Instance.playerManager.havingTools == true)
        {
            GameManager.Instance.playerManager.outils = 1;
            GameManager.Instance.playerManager.Gant.SetActive(false);
            GameManager.Instance.playerManager.Pelle.SetActive(false);
            GameManager.Instance.playerManager.Houe.SetActive(false);
            GameManager.Instance.playerManager.Arrosoir.SetActive(true);
        }
        else if (Input.GetButtonDown("Outils 3") && GameManager.Instance.playerManager.outils != 2 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsY_Xbox") <= -0.8 && GameManager.Instance.playerManager.havingTools == true)
        {
            GameManager.Instance.playerManager.outils = 2;
            GameManager.Instance.playerManager.Gant.SetActive(false);
            GameManager.Instance.playerManager.Pelle.SetActive(true);
            GameManager.Instance.playerManager.Houe.SetActive(false);
            GameManager.Instance.playerManager.Arrosoir.SetActive(false);
        }
        else if (Input.GetButtonDown("Outils 4") && GameManager.Instance.playerManager.outils != 3 && GameManager.Instance.playerManager.havingTools == true || Input.GetAxis("OutilsX_Xbox") <= -0.8 && GameManager.Instance.playerManager.havingTools == true)
        {
            GameManager.Instance.playerManager.outils = 3;
            GameManager.Instance.playerManager.Gant.SetActive(false);
            GameManager.Instance.playerManager.Pelle.SetActive(false);
            GameManager.Instance.playerManager.Houe.SetActive(true);
            GameManager.Instance.playerManager.Arrosoir.SetActive(false);
        }
        else if(Input.GetButtonDown("Build"))
        {
            if (GameManager.Instance.playerManager.outils != 5 && GameManager.Instance.playerManager.havingTools == true)
            {
                GameManager.Instance.playerManager.outils = 5;
                GameManager.Instance.playerManager.isBuildMode = true;
                GameManager.Instance.playerManager.Gant.SetActive(false);
                GameManager.Instance.playerManager.Pelle.SetActive(false);
                GameManager.Instance.playerManager.Houe.SetActive(false);
                GameManager.Instance.playerManager.Arrosoir.SetActive(false);
            }
            if (!GameManager.Instance.buildManager.isBuilding)
            {
                GameManager.Instance.playerManager.outils = 0;
                GameManager.Instance.playerManager.Gant.SetActive(true);
                GameManager.Instance.playerManager.Pelle.SetActive(false);
                GameManager.Instance.playerManager.Houe.SetActive(false);
                GameManager.Instance.playerManager.Arrosoir.SetActive(false);
            }
            OnBuildMode.Invoke();
            Debug.Log("Build Mode Enable");
        }
        
        ApplyJuiceSmoothly();
        HandleSprintVisuals();
    }






    public event Action OnBuildMode = delegate { };

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
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

        isSprinting = Input.GetButton("Fire3");


    }

    private void HandleMovement()
    {
        float currentSpeed = isSprinting && isGrounded ? sprintSpeed : speed;
        if (!GameManager.Instance.playerManager.ragdoll)
        {
            if (inputDirection.magnitude >= 0.1f)
            {
                Vector3 targetVelocity = inputDirection * currentSpeed;
                body.velocity = new Vector3(targetVelocity.x, body.velocity.y, targetVelocity.z);
            }
            else if (isGrounded)
            {
                Vector3 horizontalVelocity = new Vector3(body.velocity.x, 0, body.velocity.z);
                Vector3 smoothedHorizontal = Vector3.SmoothDamp(horizontalVelocity, Vector3.zero, ref stoppingVelocity, decelerationSmoothness);
                body.velocity = new Vector3(smoothedHorizontal.x, body.velocity.y, smoothedHorizontal.z);
            }
        }

    }

    private void HandleSprintVisuals()
    {
        if (sprintParticles != null)
        {
            if (isSprinting && inputDirection.magnitude > 0.1f && isGrounded)
            {
                if (!sprintParticles.isPlaying)
                {
                    sprintParticles.Play();
                }
            }
            else
            {
                if (sprintParticles.isPlaying)
                {
                    sprintParticles.Stop();
                }
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
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        if (!wasGrounded && isGrounded && enableJuice)
        {
            StartCoroutine(AnimateSquash(landSquash));
        }
    }

    private void Jump()
    {
        if (enableJuice)
        {
            StartCoroutine(AnimateSquashAndJump(jumpSquash));
        }
        else
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CalculateJuiceTargets()
    {
        if (isGrounded)
        {
            if (body.velocity.magnitude < 0.2f)
            {
                float breathe = Mathf.Sin(Time.time * idleBreatheFrequency) * idleBreatheAmplitude;
                targetVisualsScale = new Vector3(initialVisualsScale.x, initialVisualsScale.y + breathe, initialVisualsScale.z);
                targetVisualsPos = Vector3.zero;
            }
            else
            {
                // MODIFICATION: Choisir les bons paramètres d'oscillation si on sprinte ou non
                float currentBobbingFrequency = (isSprinting && isGrounded) ? sprintBobbingFrequency : bobbingFrequency;
                float currentBobbingAmplitude = (isSprinting && isGrounded) ? sprintBobbingAmplitude : bobbingAmplitude;

                float bobbingOffset = Mathf.Sin(Time.time * currentBobbingFrequency) * currentBobbingAmplitude;
                targetVisualsPos = new Vector3(0, bobbingOffset, 0);
                targetVisualsScale = initialVisualsScale;
            }
        }
        else
        {
            targetVisualsPos = Vector3.zero;
            targetVisualsScale = initialVisualsScale;
        }
    }

    private void ApplyJuiceSmoothly()
    {
        playerVisuals.localPosition = Vector3.SmoothDamp(playerVisuals.localPosition, targetVisualsPos, ref visualsPosVelocity, visualSmoothTime);
        if (!isSquashing)
        {
            playerVisuals.localScale = Vector3.SmoothDamp(playerVisuals.localScale, targetVisualsScale, ref visualsScaleVelocity, visualSmoothTime);
        }
    }

    private IEnumerator AnimateSquash(Vector3 targetScale)
    {
        isSquashing = true;
        playerVisuals.localScale = targetScale;
        yield return null;

        while (Vector3.Distance(playerVisuals.localScale, initialVisualsScale) > 0.01f)
        {
            playerVisuals.localScale = Vector3.Lerp(playerVisuals.localScale, initialVisualsScale, squashReturnSpeed * Time.deltaTime);
            yield return null;
        }
        playerVisuals.localScale = initialVisualsScale;
        isSquashing = false;
    }

    private IEnumerator AnimateSquashAndJump(Vector3 targetScale)
    {
        isSquashing = true;
        playerVisuals.localScale = targetScale;
        yield return new WaitForSeconds(0.05f);

        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        while (Vector3.Distance(playerVisuals.localScale, initialVisualsScale) > 0.01f)
        {
            playerVisuals.localScale = Vector3.Lerp(playerVisuals.localScale, initialVisualsScale, squashReturnSpeed * Time.deltaTime);
            yield return null;
        }
        playerVisuals.localScale = initialVisualsScale;
        isSquashing = false;
    }
}
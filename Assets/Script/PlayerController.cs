using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody body;
    public Transform playerVisuals;
    public Transform cameraTransform;

    [Header("Mouvement")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    [Tooltip("Contrôle la glissade à l'arrêt. 0 = arrêt net. 0.2 = légère glissade.")]
    public float decelerationSmoothness = 0.15f;

    [Header("Rotation")]
    [Tooltip("Vitesse angulaire maximale en degrés/sec.")]
    public float rotationSpeed = 1200f;
    [Tooltip("Fluidité de la rotation. Plus la valeur est basse, plus le personnage est lent à changer de direction.")]
    public float rotationSmoothness = 0.1f;

    [Header("Juice - Effets visuels")]
    public bool enableJuice = true;
    public float visualSmoothTime = 0.1f;

    [Header("Juice - Idle (Respiration)")]
    public float idleBreatheFrequency = 1f;
    public float idleBreatheAmplitude = 0.05f;

    [Header("Juice - Mouvement (Sautillement)")]
    public float bobbingFrequency = 10f;
    public float bobbingAmplitude = 0.1f;

    [Header("Juice - Saut (Déformation)")]
    public Vector3 jumpSquash = new Vector3(1.25f, 0.75f, 1.25f);
    public Vector3 landSquash = new Vector3(1.5f, 0.5f, 1.5f);
    public float squashReturnSpeed = 8f;

    // --- Variables privées ---
    private Vector3 moveDirection;
    private Vector3 smoothedMoveDirection;
    private Vector3 moveDirectionVelocity; // Pour le lissage de la rotation
    private Vector3 stoppingVelocity; // Pour le lissage de l'arrêt
    private bool isGrounded;
    private Vector3 initialVisualsScale;
    private Vector3 targetVisualsPos;
    private Vector3 targetVisualsScale;
    private Vector3 visualsPosVelocity;
    private Vector3 visualsScaleVelocity;
    private bool isSquashing = false;

    void Start()
    {
        if (body == null) body = GetComponent<Rigidbody>();
        if (playerVisuals == null) playerVisuals = transform;
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

        initialVisualsScale = playerVisuals.localScale;
        targetVisualsScale = initialVisualsScale;
        targetVisualsPos = Vector3.zero;
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

        ApplyJuiceSmoothly();
    }

    void FixedUpdate()
    {
        HandleMovementAndRotation();
    }

    private void HandleInputs()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * vertical + camRight * horizontal).normalized;
    }

    private void HandleMovementAndRotation()
    {
        // --- MOUVEMENT ---
        if (moveDirection.magnitude >= 0.1f)
        {
            // Applique la vélocité pour le mouvement
            Vector3 targetVelocity = moveDirection * speed;
            body.velocity = new Vector3(targetVelocity.x, body.velocity.y, targetVelocity.z);
        }
        // --- ARRÊT PROGRESSIF (GLISSADE) ---
        else if (isGrounded)
        {
            // Réduit progressivement la vélocité horizontale à zéro
            Vector3 horizontalVelocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            Vector3 smoothedHorizontal = Vector3.SmoothDamp(horizontalVelocity, Vector3.zero, ref stoppingVelocity, decelerationSmoothness);
            body.velocity = new Vector3(smoothedHorizontal.x, body.velocity.y, smoothedHorizontal.z);
        }

        // --- ROTATION ---
        // On ne tourne que s'il y a une intention de mouvement
        if (moveDirection.magnitude >= 0.1f)
        {
            // 1. Lisse le vecteur de direction pour la rotation
            smoothedMoveDirection = Vector3.SmoothDamp(
                smoothedMoveDirection,
                moveDirection,
                ref moveDirectionVelocity,
                rotationSmoothness
            );

            // 2. Crée la rotation cible à partir de la direction lissée
            Quaternion targetRotation = Quaternion.LookRotation(smoothedMoveDirection);

            // 3. Applique la rotation avec une vitesse maximale
            body.rotation = Quaternion.RotateTowards(body.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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
            if (body.velocity.magnitude < 0.2f) // Basé sur la vélocité réelle pour plus de précision
            {
                float breathe = Mathf.Sin(Time.time * idleBreatheFrequency) * idleBreatheAmplitude;
                targetVisualsScale = new Vector3(initialVisualsScale.x, initialVisualsScale.y + breathe, initialVisualsScale.z);
                targetVisualsPos = Vector3.zero;
            }
            else
            {
                float bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
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
using UnityEngine;

/// <summary>
/// Platformer Controller - Teaching Edition
/// 
/// Fixed issues from previous version:
/// - Movement now properly isolated from jump code
/// - riseMultiplier logic inverted correctly (lower = MORE gravity, not less)
/// - Consistent vector construction throughout
/// - All gravity states properly documented
/// 
/// Recommended Unity Settings:
/// - Physics.gravity.y = -9.81 (Earth gravity, default)
/// - Time.fixedDeltaTime = 0.02 (50 FPS physics, default)
/// - Rigidbody: Mass = 1, Drag = 0, Angular Drag = 0.05
/// 
/// Units in Unity:
/// - 1 unit = 1 meter (by convention)
/// - jumpForce is in meters/second (m/s)
/// - maxSpeed is in meters/second (m/s)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlatformerPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Max horizontal speed in m/s\n• 8 = jogging pace\n• 12 = sprinting")]
    [SerializeField] private float maxSpeed = 8f;

    [Tooltip("Acceleration when grounded\n• 50 = instant\n• 20 = momentum-based")]
    [SerializeField] private float acceleration = 50f;

    [Tooltip("Air control (60-80% of ground recommended)\n• Too low = frustrating\n• Too high = unrealistic")]
    [SerializeField] private float airAcceleration = 30f;

    [Tooltip("Friction when grounded\n• 20 = tight control\n• 5 = ice")]
    [SerializeField] private float friction = 20f;

    [Tooltip("Air resistance (keep LOW: 1-3)\n• Higher = sticky air movement")]
    [SerializeField] private float airFriction = 2f;

    [Header("Jump - Height")]
    [Tooltip("Initial jump velocity in m/s\n• 12 m/s = ~7.3m jump height\n• 10 m/s = ~5.1m height\n• 15 m/s = ~11.5m height\n\nFormula: height ≈ (velocity²) / (2 × gravity)")]
    [SerializeField] private float jumpForce = 12f;

    [Tooltip("Short hop velocity multiplier\n• 0.5 = tap jump reaches 50% height\n• Gives fine control")]
    [SerializeField, Range(0.3f, 0.8f)] private float jumpCutMultiplier = 0.5f;

    [Header("Jump - Feel (Gravity Multipliers)")]
    [Tooltip("Fall gravity multiplier\n• 2.5 = fall 2.5× faster than normal (snappy)\n• 1.0 = realistic (floaty)\n• Applied when velocity.y < 0")]
    [SerializeField] private float fallMultiplier = 2.5f;

    [Tooltip("Gravity when releasing jump early\n• 2.0 = quick short hops\n• Applied when rising + button released\n• Should be ≤ fallMultiplier")]
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Tooltip("RISE gravity reduction (inverted!)\n• 1.0 = normal gravity during rise\n• 0.8 = MORE gravity (80% reduction from added force)\n• 0.5 = MUCH MORE gravity (snappier arc)\n\nLower value = snappier rise")]
    [SerializeField, Range(0.5f, 1.5f)] private float riseMultiplier = 0.8f;

    [Header("Forgiveness")]
    [Tooltip("Coyote time - jump grace period in seconds\n• 0.15s = fair, invisible to player")]
    [SerializeField] private float coyoteTime = 0.15f;

    [Tooltip("Jump buffer - input memory in seconds\n• 0.15s = responsive feel")]
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [Tooltip("Raycast distance below player\n• Should be slightly > collider radius")]
    [SerializeField] private float groundCheckDistance = 0.15f;

    [Tooltip("Layer mask for ground\n• Set to 'Ground' layer")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private float currentYVelocity;
    [SerializeField] private string jumpState = "Grounded";

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool jumpInput;
    private bool jumpReleased;
    private float lastGroundedTime;
    private float lastJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Verify physics settings
        if (showDebug)
        {
            Debug.Log("=== PLATFORMER CONTROLLER SETUP ===");
            Debug.Log($"Physics gravity: {Physics.gravity.y:F2} m/s² (should be -9.81)");
            Debug.Log($"Fixed timestep: {Time.fixedDeltaTime:F3}s (should be 0.02)");

            // Calculate theoretical jump height
            float theoreticalHeight = (jumpForce * jumpForce) / (2f * Mathf.Abs(Physics.gravity.y));
            Debug.Log($"Jump force {jumpForce} m/s → ~{theoreticalHeight:F1}m theoretical height");
            Debug.Log("(Actual height will be lower due to gravity multipliers)");
            Debug.Log("===================================");
        }
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        UpdateDebugInfo();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyJump();
        ApplyCustomGravity();
    }

    void HandleInput()
    {
        // Get movement input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveInput = (forward * v + right * h).normalized;

        // Jump input with buffering
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
            lastJumpTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpReleased = true;
        }
    }

    void CheckGround()
    {
        // Use SphereCast for reliable ground detection on slopes/edges
        isGrounded = Physics.SphereCast(
            transform.position + Vector3.up * 0.1f,  // Start slightly above ground
            0.3f,                                     // Sphere radius
            Vector3.down,                             // Cast direction
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
        }
    }

    /// <summary>
    /// Apply horizontal movement with acceleration and friction
    /// CRITICAL: This never modifies Y velocity - keeps jump physics clean
    /// </summary>
    void ApplyMovement()
    {
        // Isolate horizontal velocity (X and Z only)
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Choose physics values based on grounded state
        float accel = isGrounded ? acceleration : airAcceleration;
        float fric = isGrounded ? friction : airFriction;

        // ACCELERATING: Move toward target velocity
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 targetVel = moveInput * maxSpeed;
            Vector3 velDiff = targetVel - horizontalVel;
            Vector3 accelVector = velDiff * accel * Time.fixedDeltaTime;

            // Don't overshoot the target
            if (accelVector.magnitude > velDiff.magnitude)
                accelVector = velDiff;

            // Apply acceleration - ONLY to X and Z, preserve Y!
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x + accelVector.x,
                rb.linearVelocity.y,  // NEVER modified here
                rb.linearVelocity.z + accelVector.z
            );
        }
        // DECELERATING: Apply friction
        else if (horizontalVel.magnitude > 0.1f)
        {
            Vector3 frictionVector = -horizontalVel.normalized * fric * Time.fixedDeltaTime;

            // Stop completely if friction would reverse direction
            if (frictionVector.magnitude > horizontalVel.magnitude)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            else
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x + frictionVector.x,
                    rb.linearVelocity.y,  // NEVER modified here
                    rb.linearVelocity.z + frictionVector.z
                );
            }
        }

        // Enforce max speed cap
        horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 clampedVel = horizontalVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                clampedVel.x,
                rb.linearVelocity.y,  // NEVER modified here
                clampedVel.z
            );
        }
    }

    /// <summary>
    /// Handle jump execution with coyote time and buffering
    /// Uses instant velocity override (Bennett Foddy principle)
    /// </summary>
    void ApplyJump()
    {
        // Coyote time: Can we still jump after leaving ground?
        bool canCoyoteJump = Time.time - lastGroundedTime <= coyoteTime;

        // Jump buffer: Did we press jump recently?
        bool hasJumpBuffer = Time.time - lastJumpTime <= jumpBufferTime;

        // EXECUTE JUMP
        if (jumpInput && hasJumpBuffer && (isGrounded || canCoyoteJump))
        {
            // Direct velocity override - instant and responsive!
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpForce,  // Set exact upward velocity
                rb.linearVelocity.z
            );

            isJumping = true;
            jumpInput = false;
            lastGroundedTime = 0;  // Prevent double jump

            if (showDebug)
                Debug.Log($"[JUMP] Initial velocity: {jumpForce:F1} m/s");
        }

        // VARIABLE JUMP HEIGHT: Cut jump when released early
        if (jumpReleased && rb.linearVelocity.y > 0.1f)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier,  // Reduce upward momentum
                rb.linearVelocity.z
            );

            jumpReleased = false;

            if (showDebug)
                Debug.Log($"[JUMP CUT] Velocity reduced to: {rb.linearVelocity.y:F1} m/s");
        }
    }

    /// <summary>
    /// Apply custom gravity multipliers for game feel
    /// 
    /// THREE STATES:
    /// 1. FALLING: Strong extra gravity (snappy descent)
    /// 2. RISING (released): Medium extra gravity (quick short hops)
    /// 3. RISING (holding): Light extra gravity (controlled by riseMultiplier)
    /// 
    /// FIX: riseMultiplier logic is now CORRECT
    /// - Lower riseMultiplier = MORE gravity added
    /// - Higher riseMultiplier = LESS gravity added
    /// </summary>
    void ApplyCustomGravity()
    {
        float yVel = rb.linearVelocity.y;

        // STATE 1: FALLING (after apex)
        if (yVel < -0.1f)
        {
            // Add strong extra downward force for snappy falls
            float extraGravity = Physics.gravity.y * (fallMultiplier - 1f);
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y + extraGravity * Time.fixedDeltaTime,
                rb.linearVelocity.z
            );

            jumpState = "Falling";
        }
        // STATE 2: RISING but released jump (short hop)
        else if (yVel > 0.1f && !Input.GetButton("Jump"))
        {
            // Add medium extra gravity for crisp short hops
            float extraGravity = Physics.gravity.y * (lowJumpMultiplier - 1f);
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y + extraGravity * Time.fixedDeltaTime,
                rb.linearVelocity.z
            );

            jumpState = "Rising (Released)";
        }
        // STATE 3: RISING and holding jump (full jump)
        else if (yVel > 0.1f && Input.GetButton("Jump") && isJumping)
        {
            // FIXED LOGIC: Lower riseMultiplier = MORE added gravity
            // 
            // Think of it as: "What percentage of EXTRA gravity should we add?"
            // riseMultiplier = 1.0 → Add 0% extra gravity (normal rise)
            // riseMultiplier = 0.8 → Add 20% extra gravity (slightly snappier)
            // riseMultiplier = 0.5 → Add 50% extra gravity (very snappy)

            if (riseMultiplier < 1.0f)
            {
                // Calculate extra gravity to add
                // Lower riseMultiplier = higher percentage added
                float extraGravityPercent = 1f - riseMultiplier;
                float extraGravity = Physics.gravity.y * extraGravityPercent;

                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y + extraGravity * Time.fixedDeltaTime,
                    rb.linearVelocity.z
                );
            }

            jumpState = "Rising (Holding)";
        }
        else
        {
            jumpState = "Grounded";
        }
    }

    void UpdateDebugInfo()
    {
        currentYVelocity = rb.linearVelocity.y;
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        // Ground check visualization
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 spherePos = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawWireSphere(spherePos, 0.3f);
        Gizmos.DrawLine(spherePos, spherePos + Vector3.down * groundCheckDistance);

        // Draw theoretical jump height arc
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            float theoreticalHeight = (jumpForce * jumpForce) / (2f * Mathf.Abs(Physics.gravity.y));
            Vector3 peakPos = transform.position + Vector3.up * theoreticalHeight;
            Gizmos.DrawWireSphere(peakPos, 0.5f);
        }
    }
}


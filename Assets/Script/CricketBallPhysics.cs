using UnityEngine;

public class CricketBallPhysics : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Delivery Mode")]
    public DeliveryMode deliveryMode = DeliveryMode.Swing;
    public bool moveRightSide = true;

    [Header("Speed Settings")]
    public float forwardSpeed = 18f;
    public float upwardVelocity = 4f;

    [Header("Swing Settings")]
    public float maxSwingAcceleration = 8f;
    public AnimationCurve swingBuildUp = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Spin Settings")]
    public float maxSpinTurnAngle = 35f;

    [Header("Bounce Settings")]
    public string groundTag = "Ground";
    public float bounceUpVelocity = 2.5f;
    public float afterBounceSpeedMultiplier = 0.9f;

    [Header("Runtime Debug")]
    public bool hasBowled;
    public bool hasBounced;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 forwardDirection;
    private Vector3 sideDirection;

    private float effectStrength;
    private float airTimer;
    private float estimatedAirTime = 1.2f;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        ResetBall();
    }

    private void FixedUpdate()
    {
        if (!hasBowled)
            return;

        if (deliveryMode == DeliveryMode.Swing && !hasBounced)
        {
            ApplySwingInAir();
        }

        ApplyBallVisualRotation();
    }

    public void Bowl(Vector3 bouncePoint, DeliveryMode mode, float meterStrength, bool rightSide)
    {
        ResetPhysics();

        deliveryMode = mode;
        effectStrength = Mathf.Clamp01(meterStrength);
        moveRightSide = rightSide;

        Vector3 target = bouncePoint;
        target.y = transform.position.y;

        forwardDirection = target - transform.position;

        if (forwardDirection.sqrMagnitude < 0.01f)
            forwardDirection = Vector3.forward;

        forwardDirection.Normalize();

        sideDirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;

        if (!moveRightSide)
            sideDirection *= -1f;

        hasBowled = true;
        hasBounced = false;
        airTimer = 0f;

        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 startingVelocity = forwardDirection * forwardSpeed;
        startingVelocity.y = upwardVelocity;

        rb.velocity = startingVelocity;
    }

    private void ApplySwingInAir()
    {
        airTimer += Time.fixedDeltaTime;

        float time01 = Mathf.Clamp01(airTimer / estimatedAirTime);
        float buildValue = swingBuildUp.Evaluate(time01);

        float finalSwingForce = effectStrength * maxSwingAcceleration * buildValue;

        rb.AddForce(sideDirection * finalSwingForce, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasBowled)
            return;

        if (hasBounced)
            return;

        if (!collision.gameObject.CompareTag(groundTag))
            return;

        hasBounced = true;

        if (deliveryMode == DeliveryMode.Swing)
        {
            StopSwingAndContinueStraight();
        }
        else if (deliveryMode == DeliveryMode.Spin)
        {
            ApplySpinTurnOnce();
        }
    }

    private void StopSwingAndContinueStraight()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
            horizontalVelocity = forwardDirection * forwardSpeed;

        Vector3 directionAtBounce = horizontalVelocity.normalized;
        float speedAtBounce = horizontalVelocity.magnitude * afterBounceSpeedMultiplier;

        Vector3 newVelocity = directionAtBounce * speedAtBounce;
        newVelocity.y = bounceUpVelocity;

        rb.velocity = newVelocity;
    }

    private void ApplySpinTurnOnce()
    {
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
            horizontalVelocity = forwardDirection * forwardSpeed;

        Vector3 directionBeforeSpin = horizontalVelocity.normalized;
        float speedAtBounce = horizontalVelocity.magnitude * afterBounceSpeedMultiplier;

        float sideMultiplier = moveRightSide ? 1f : -1f;
        float spinTurnAngle = effectStrength * maxSpinTurnAngle * sideMultiplier;

        Quaternion spinRotation = Quaternion.AngleAxis(spinTurnAngle, Vector3.up);
        Vector3 newDirection = spinRotation * directionBeforeSpin;

        Vector3 newVelocity = newDirection.normalized * speedAtBounce;
        newVelocity.y = bounceUpVelocity;

        rb.velocity = newVelocity;
    }

    private void ApplyBallVisualRotation()
    {
        if (rb.velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
            return;

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, horizontalVelocity.normalized);
        rb.angularVelocity = rotationAxis * horizontalVelocity.magnitude * 2.5f;
    }

    public void ResetBall()
    {
        hasBowled = false;
        hasBounced = false;
        airTimer = 0f;

        transform.position = startPosition;
        transform.rotation = startRotation;

        ResetPhysics();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void ResetPhysics()
    {
        if (rb == null)
            return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
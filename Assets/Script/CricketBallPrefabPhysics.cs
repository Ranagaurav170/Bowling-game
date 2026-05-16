
using UnityEngine;
using System;

public class CricketBallPrefabPhysics : MonoBehaviour
{
    public Action<CricketBallPrefabPhysics> OnBallDestroyed;

    [Header("References")]
    public Rigidbody rb;
    public SphereCollider sphereCollider;

    [Header("Mode")]
    public DeliveryMode deliveryMode = DeliveryMode.Swing;
    public bool moveRightSide = true;

    [Header("Swing Settings")]
    [Tooltip("Maximum side curve in meters for Blue 100%.")]
    public float maxSwingCurveAmount = 1.4f;

    [Tooltip("Ball height in air before bounce.")]
    public float arcHeight = 1.2f;

    [Header("Spin Settings")]
    [Tooltip("Keep low for realistic cricket spin.")]
    public float maxSpinTurnAngle = 16f;

    [Header("After Bounce")]
    public float afterBounceSpeed = 11f;
    public float bounceUpVelocity = 2f;
    public float afterBounceSpeedMultiplier = 0.9f;

    [Header("Life")]
    public float destroyAfterSeconds = 6f;

    [Header("Runtime Debug")]
    public bool isInitialized;
    public bool hasBounced;
    public float effectStrength;
    public float currentT;

    private Vector3 startPoint;
    private Vector3 bouncePoint;
    private Vector3 sideDirection;
    private Vector3 lastPosition;
    private Vector3 velocityBeforeBounce;

    private float timer;
    private float timeToBounce;
    private float ballRadius;
    private Collider ballCollider;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();

        ballCollider = GetComponent<Collider>();

        if (sphereCollider != null)
        {
            float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            ballRadius = sphereCollider.radius * maxScale;
        }
        else
        {
            ballRadius = 0.11f;
        }
    }

    public void InitializeBall(
        Vector3 startPosition,
        Vector3 markerPoint,
        DeliveryMode mode,
        float strength,
        bool rightSide,
        float wantedTimeToBounce)
    {
        deliveryMode = mode;
        effectStrength = Mathf.Clamp01(strength);
        moveRightSide = rightSide;

        timeToBounce = Mathf.Max(0.6f, wantedTimeToBounce);
        timer = 0f;
        currentT = 0f;

        startPoint = startPosition;

        // Marker is on ground, ball center must reach ground + radius.
        bouncePoint = markerPoint;
        bouncePoint.y = markerPoint.y + ballRadius;

        Vector3 forwardDirection = bouncePoint - startPoint;
        forwardDirection.y = 0f;

        if (forwardDirection.sqrMagnitude < 0.01f)
            forwardDirection = Vector3.forward;

        forwardDirection.Normalize();

        sideDirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;

        if (!moveRightSide)
            sideDirection *= -1f;

        transform.position = startPoint;
        transform.rotation = Quaternion.identity;

        lastPosition = startPoint;
        velocityBeforeBounce = Vector3.zero;

        isInitialized = true;
        hasBounced = false;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (ballCollider != null)
            ballCollider.enabled = false;

        gameObject.SetActive(true);

        Destroy(gameObject, destroyAfterSeconds);
    }

    private void Update()
    {
        if (!isInitialized || hasBounced)
            return;

        MoveBallBeforeBounce();
    }

    private void MoveBallBeforeBounce()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / timeToBounce);
        currentT = t;

        Vector3 nextPosition;

        if (deliveryMode == DeliveryMode.Swing)
            nextPosition = GetSwingPosition(t);
        else
            nextPosition = GetStraightPosition(t);

        velocityBeforeBounce = (nextPosition - lastPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = nextPosition;

        transform.position = nextPosition;

        RotateBallVisually(velocityBeforeBounce);

        if (t >= 1f)
        {
            BounceNow();
        }
    }

    private Vector3 GetStraightPosition(float t)
    {
        Vector3 pos = Vector3.Lerp(startPoint, bouncePoint, t);

        float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
        pos.y += arc;

        return pos;
    }

    private Vector3 GetSwingPosition(float t)
    {
        Vector3 pos = Vector3.Lerp(startPoint, bouncePoint, t);

        float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
        pos.y += arc;

        float swingShape = Mathf.Sin(t * Mathf.PI);
        float swingBuild = Mathf.SmoothStep(0f, 1f, t);

        float swingAmount = maxSwingCurveAmount * effectStrength * swingShape * swingBuild;

        pos += sideDirection * swingAmount;

        return pos;
    }

    private void BounceNow()
    {
        hasBounced = true;

        transform.position = bouncePoint;

        rb.isKinematic = false;
        rb.useGravity = true;

        if (ballCollider != null)
            ballCollider.enabled = true;

        Vector3 horizontalVelocity = velocityBeforeBounce;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
        {
            horizontalVelocity = bouncePoint - startPoint;
            horizontalVelocity.y = 0f;
        }

        Vector3 directionAfterBounce = horizontalVelocity.normalized;

        if (deliveryMode == DeliveryMode.Spin)
        {
            float sideMultiplier = moveRightSide ? 1f : -1f;
            float spinAngle = effectStrength * maxSpinTurnAngle * sideMultiplier;

            Quaternion spinRotation = Quaternion.AngleAxis(spinAngle, Vector3.up);
            directionAfterBounce = spinRotation * directionAfterBounce;
        }

        Vector3 finalVelocity = directionAfterBounce.normalized * afterBounceSpeed * afterBounceSpeedMultiplier;
        finalVelocity.y = bounceUpVelocity;

        rb.velocity = finalVelocity;
    }

    private void RotateBallVisually(Vector3 velocity)
    {
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
            return;

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, horizontalVelocity.normalized);
        transform.Rotate(rotationAxis, horizontalVelocity.magnitude * 180f * Time.deltaTime, Space.World);
    }

    private void OnDestroy()
    {
        OnBallDestroyed?.Invoke(this);
    }
}
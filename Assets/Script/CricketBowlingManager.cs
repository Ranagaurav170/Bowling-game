// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class CricketBowlingManager : MonoBehaviour
// {
//     [Header("Ball Prefab")]
//     public CricketBallPrefabPhysics ballPrefab;
//     public Transform ballStartPoint;

//     [Header("Marker")]
//     public BounceMarker bounceMarker;

//     [Header("Meter")]
//     public CenterBasedBowlingMeter meter;

//     [Header("Buttons")]
//     public Button swingButton;
//     public Button spinButton;
//     public Button bowlButton;
//     public Button changeSideButton;
//     public Button resetButton;

//     [Header("Texts")]
//     public TMP_Text modeText;
//     public TMP_Text sideText;
//     public TMP_Text resultText;

//     [Header("Bowling Settings")]
//     public DeliveryMode currentMode = DeliveryMode.Swing;
//     public bool rightSide = true;

//     [Tooltip("Lower value = faster ball. Higher value = slower ball.")]
//     public float timeToBounce = 0.8f;

//     [Header("Runtime")]
//     private CricketBallPrefabPhysics spawnedBall;

//     private void Start()
//     {
//         if (swingButton != null)
//             swingButton.onClick.AddListener(SelectSwing);

//         if (spinButton != null)
//             spinButton.onClick.AddListener(SelectSpin);

//         if (bowlButton != null)
//             bowlButton.onClick.AddListener(Bowl);

//         if (changeSideButton != null)
//             changeSideButton.onClick.AddListener(ChangeSide);

//         if (resetButton != null)
//             resetButton.onClick.AddListener(PrepareNextThrow);

//         if (meter != null)
//             meter.PrepareForNextThrow();

//         UpdateUI();
//     }

//     private void SelectSwing()
//     {
//         currentMode = DeliveryMode.Swing;
//         UpdateUI();
//     }

//     private void SelectSpin()
//     {
//         currentMode = DeliveryMode.Spin;
//         UpdateUI();
//     }

//     private void ChangeSide()
//     {
//         rightSide = !rightSide;
//         UpdateUI();
//     }

//     private void Bowl()
//     {
//         if (ballPrefab == null)
//         {
//             Debug.LogWarning("Ball Prefab is missing.");
//             return;
//         }

//         if (ballStartPoint == null)
//         {
//             Debug.LogWarning("Ball Start Point is missing.");
//             return;
//         }

//         if (bounceMarker == null)
//         {
//             Debug.LogWarning("Bounce Marker is missing.");
//             return;
//         }

//         if (meter == null)
//         {
//             Debug.LogWarning("Meter is missing.");
//             return;
//         }

//         // Destroy only the spawned scene ball, never the prefab asset
//         if (spawnedBall != null)
//         {
//             Destroy(spawnedBall.gameObject);
//             spawnedBall = null;
//         }

//         meter.StopMeter();

//         float strength = meter.GetEffectStrength();
//         string zone = meter.GetCurrentZone();

//         Vector3 startPosition = ballStartPoint.position;
//         Vector3 bouncePoint = bounceMarker.GetBouncePoint();

//         // Instantiate creates a scene object from the prefab
//         CricketBallPrefabPhysics newBall = Instantiate(
//             ballPrefab,
//             startPosition,
//             Quaternion.identity
//         );

//         spawnedBall = newBall;

//         spawnedBall.InitializeBall(
//             startPosition,
//             bouncePoint,
//             currentMode,
//             strength,
//             rightSide,
//             timeToBounce
//         );

//         if (resultText != null)
//             resultText.text = zone + " | " + (strength * 100f).ToString("0") + "%";
//     }

//     private void PrepareNextThrow()
//     {
//         // Destroy only spawned scene object
//         if (spawnedBall != null)
//         {
//             Destroy(spawnedBall.gameObject);
//             spawnedBall = null;
//         }

//         if (meter != null)
//             meter.PrepareForNextThrow();

//         if (resultText != null)
//             resultText.text = "";

//         UpdateUI();
//     }

//     private void UpdateUI()
//     {
//         if (modeText != null)
//             modeText.text = "Mode: " + currentMode.ToString();

//         if (sideText != null)
//             sideText.text = rightSide ? "Side: Right" : "Side: Left";
//     }
// }
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class CricketBowlingManager : MonoBehaviour
// {
//     [Header("Ball Prefab")]
//     public CricketBallPrefabPhysics ballPrefab;

//     [Header("Ball Start Points")]
//     public Transform ballStartPointA;
//     public Transform ballStartPointB;
//     public bool useStartPointB = false;

//     [Header("Marker")]
//     public BounceMarker bounceMarker;

//     [Header("Meter")]
//     public CenterBasedBowlingMeter meter;

//     [Header("Buttons")]
//     public Button swingButton;
//     public Button spinButton;
//     public Button bowlButton;
//     public Button changeSideButton;
//     public Button resetButton;
//     public Button toggleStartPointButton;

//     [Header("Texts")]
//     public TMP_Text modeText;
//     public TMP_Text sideText;
//     public TMP_Text resultText;
//     public TMP_Text startPointText;

//     [Header("Current Mode")]
//     public DeliveryMode currentMode = DeliveryMode.Swing;

//     [Header("Random Side")]
//     public bool useRandomSide = true;
//     public bool rightSide = true;

//     [Header("Speed / Time To Bounce")]
//     public float swingTimeToBounce = 0.65f;
//     public float spinTimeToBounce = 0.95f;

//     [Header("Runtime")]
//     private CricketBallPrefabPhysics waitingBall;
//     private CricketBallPrefabPhysics thrownBall;

//     private void Start()
//     {
//         if (swingButton != null)
//             swingButton.onClick.AddListener(SelectSwing);

//         if (spinButton != null)
//             spinButton.onClick.AddListener(SelectSpin);

//         if (bowlButton != null)
//             bowlButton.onClick.AddListener(Bowl);

//         if (changeSideButton != null)
//             changeSideButton.onClick.AddListener(ChangeSideManually);

//         if (resetButton != null)
//             resetButton.onClick.AddListener(ResetAll);

//         if (toggleStartPointButton != null)
//             toggleStartPointButton.onClick.AddListener(ToggleStartPoint);

//         if (meter != null)
//             meter.PrepareForNextThrow();

//         SpawnWaitingBall();
//         UpdateUI();
//     }

//     private void SelectSwing()
//     {
//         currentMode = DeliveryMode.Swing;
//         UpdateUI();
//     }

//     private void SelectSpin()
//     {
//         currentMode = DeliveryMode.Spin;
//         UpdateUI();
//     }

//     private void ChangeSideManually()
//     {
//         useRandomSide = false;
//         rightSide = !rightSide;
//         UpdateUI();
//     }

//     private void ToggleStartPoint()
//     {
//         useStartPointB = !useStartPointB;

//         MoveWaitingBallToSelectedStartPoint();

//         UpdateUI();
//     }

//     private Transform GetCurrentStartPoint()
//     {
//         return useStartPointB ? ballStartPointB : ballStartPointA;
//     }

//     private float GetTimeToBounceByMode()
//     {
//         return currentMode == DeliveryMode.Swing ? swingTimeToBounce : spinTimeToBounce;
//     }

//     private void SpawnWaitingBall()
//     {
//         if (ballPrefab == null)
//         {
//             Debug.LogWarning("Ball Prefab is missing.");
//             return;
//         }

//         Transform selectedStartPoint = GetCurrentStartPoint();

//         if (selectedStartPoint == null)
//         {
//             Debug.LogWarning("Selected start point is missing.");
//             return;
//         }

//         if (waitingBall != null)
//         {
//             Destroy(waitingBall.gameObject);
//             waitingBall = null;
//         }

//         waitingBall = Instantiate(
//             ballPrefab,
//             selectedStartPoint.position,
//             selectedStartPoint.rotation
//         );

//         PrepareBallAsWaiting(waitingBall, selectedStartPoint);
//     }

//     private void PrepareBallAsWaiting(CricketBallPrefabPhysics ball, Transform startPoint)
//     {
//         if (ball == null || startPoint == null)
//             return;

//         ball.transform.position = startPoint.position;
//         ball.transform.rotation = startPoint.rotation;

//         Rigidbody rb = ball.GetComponent<Rigidbody>();

//         if (rb != null)
//         {
//             rb.isKinematic = true;
//             rb.useGravity = false;
//             rb.velocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//         }

//         Collider col = ball.GetComponent<Collider>();

//         if (col != null)
//             col.enabled = true;

//         ball.gameObject.SetActive(true);
//     }

//     private void MoveWaitingBallToSelectedStartPoint()
//     {
//         Transform selectedStartPoint = GetCurrentStartPoint();

//         if (selectedStartPoint == null)
//             return;

//         if (waitingBall == null)
//         {
//             SpawnWaitingBall();
//             return;
//         }

//         PrepareBallAsWaiting(waitingBall, selectedStartPoint);
//     }

//     private void Bowl()
//     {
//         if (ballPrefab == null)
//         {
//             Debug.LogWarning("Ball Prefab is missing.");
//             return;
//         }

//         Transform selectedStartPoint = GetCurrentStartPoint();

//         if (selectedStartPoint == null)
//         {
//             Debug.LogWarning("Selected Ball Start Point is missing.");
//             return;
//         }

//         if (bounceMarker == null)
//         {
//             Debug.LogWarning("Bounce Marker is missing.");
//             return;
//         }

//         if (meter == null)
//         {
//             Debug.LogWarning("Meter is missing.");
//             return;
//         }

//         if (waitingBall == null)
//         {
//             SpawnWaitingBall();
//         }

//         if (waitingBall == null)
//         {
//             Debug.LogWarning("Waiting ball could not be created.");
//             return;
//         }

//         // Stop meter and lock value for this throw.
//         meter.StopMeter();

//         float strength = meter.GetEffectStrength();
//         string zone = meter.GetCurrentZone();

//         if (useRandomSide)
//             rightSide = Random.value > 0.5f;

//         float selectedTimeToBounce = GetTimeToBounceByMode();

//         Vector3 startPosition = selectedStartPoint.position;
//         Vector3 bouncePoint = bounceMarker.GetBouncePoint();

//         // Optional: remove previous thrown ball when bowling again.
//         if (thrownBall != null)
//         {
//             Destroy(thrownBall.gameObject);
//             thrownBall = null;
//         }

//         // The visible waiting ball now becomes the thrown ball.
//         thrownBall = waitingBall;
//         waitingBall = null;

//         thrownBall.transform.position = startPosition;
//         thrownBall.transform.rotation = selectedStartPoint.rotation;

//         thrownBall.InitializeBall(
//             startPosition,
//             bouncePoint,
//             currentMode,
//             strength,
//             rightSide,
//             selectedTimeToBounce
//         );

//         // Immediately create the next waiting ball at selected start point.
//         SpawnWaitingBall();

//         // Start meter again for next bowl.
//         meter.PrepareForNextThrow();

//         if (resultText != null)
//         {
//             string sideName = rightSide ? "Right" : "Left";
//             string speedName = currentMode == DeliveryMode.Swing ? "Fast Swing" : "Slow Spin";
//             string startPointName = useStartPointB ? "Start B" : "Start A";

//             resultText.text =
//                 zone + " | " +
//                 (strength * 100f).ToString("0") + "% | " +
//                 sideName + " | " +
//                 speedName + " | " +
//                 startPointName;
//         }

//         UpdateUI();
//     }

//     private void ResetAll()
//     {
//         if (thrownBall != null)
//         {
//             Destroy(thrownBall.gameObject);
//             thrownBall = null;
//         }

//         if (waitingBall != null)
//         {
//             Destroy(waitingBall.gameObject);
//             waitingBall = null;
//         }

//         SpawnWaitingBall();

//         if (meter != null)
//             meter.PrepareForNextThrow();

//         if (resultText != null)
//             resultText.text = "";

//         UpdateUI();
//     }

//     private void UpdateUI()
//     {
//         if (modeText != null)
//             modeText.text = "Mode: " + currentMode.ToString();

//         if (sideText != null)
//         {
//             string randomText = useRandomSide ? "Random" : "Manual";
//             string sideName = rightSide ? "Right" : "Left";
//             sideText.text = "Side: " + sideName + " | " + randomText;
//         }

//         if (startPointText != null)
//             startPointText.text = useStartPointB ? "Start Point: B" : "Start Point: A";
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CricketBowlingManager : MonoBehaviour
{
    [Header("Ball Prefab")]
    public CricketBallPrefabPhysics ballPrefab;

    [Header("Ball Start Points")]
    public Transform ballStartPointA;
    public Transform ballStartPointB;
    public bool useStartPointB = false;

    [Header("Marker")]
    public BounceMarker bounceMarker;

    [Header("Meter")]
    public CenterBasedBowlingMeter meter;

    [Header("Buttons")]
    public Button swingButton;
    public Button spinButton;
    public Button bowlButton;
    public Button changeSideButton;
    public Button resetButton;
    public Button toggleStartPointButton;

    [Header("Texts")]
    public TMP_Text modeText;
    public TMP_Text sideText;
    public TMP_Text resultText;
    public TMP_Text startPointText;

    [Header("Current Mode")]
    public DeliveryMode currentMode = DeliveryMode.Swing;

    [Header("Random Side")]
    public bool useRandomSide = true;
    public bool rightSide = true;

    [Header("Speed / Time To Bounce")]
    [Tooltip("Swing should be faster, so keep this lower.")]
    public float swingTimeToBounce = 0.65f;

    [Tooltip("Spin should be slower, so keep this higher.")]
    public float spinTimeToBounce = 0.95f;

    [Header("Runtime")]
    private CricketBallPrefabPhysics waitingBall;
    private CricketBallPrefabPhysics thrownBall;
    private bool isBallInPlay = false;

    private void Start()
    {
        if (swingButton != null)
            swingButton.onClick.AddListener(SelectSwing);

        if (spinButton != null)
            spinButton.onClick.AddListener(SelectSpin);

        if (bowlButton != null)
            bowlButton.onClick.AddListener(Bowl);

        if (changeSideButton != null)
            changeSideButton.onClick.AddListener(ChangeSideManually);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetAll);

        if (toggleStartPointButton != null)
            toggleStartPointButton.onClick.AddListener(ToggleStartPoint);

        if (meter != null)
            meter.PrepareForNextThrow();

        SpawnWaitingBall();
        UpdateUI();
    }

    private void SelectSwing()
    {
        currentMode = DeliveryMode.Swing;
        UpdateUI();
    }

    private void SelectSpin()
    {
        currentMode = DeliveryMode.Spin;
        UpdateUI();
    }

    private void ChangeSideManually()
    {
        useRandomSide = false;
        rightSide = !rightSide;
        UpdateUI();
    }

    private void ToggleStartPoint()
    {
        useStartPointB = !useStartPointB;

        if (!isBallInPlay)
        {
            MoveWaitingBallToSelectedStartPoint();
        }

        UpdateUI();
    }

    private Transform GetCurrentStartPoint()
    {
        return useStartPointB ? ballStartPointB : ballStartPointA;
    }

    private float GetTimeToBounceByMode()
    {
        return currentMode == DeliveryMode.Swing ? swingTimeToBounce : spinTimeToBounce;
    }

    private void SpawnWaitingBall()
    {
        if (isBallInPlay)
            return;

        if (ballPrefab == null)
        {
            Debug.LogWarning("Ball Prefab is missing.");
            return;
        }

        Transform selectedStartPoint = GetCurrentStartPoint();

        if (selectedStartPoint == null)
        {
            Debug.LogWarning("Selected start point is missing.");
            return;
        }

        if (waitingBall != null)
        {
            Destroy(waitingBall.gameObject);
            waitingBall = null;
        }

        waitingBall = Instantiate(
            ballPrefab,
            selectedStartPoint.position,
            selectedStartPoint.rotation
        );

        PrepareBallAsWaiting(waitingBall, selectedStartPoint);
    }

    private void PrepareBallAsWaiting(CricketBallPrefabPhysics ball, Transform startPoint)
    {
        if (ball == null || startPoint == null)
            return;

        ball.transform.position = startPoint.position;
        ball.transform.rotation = startPoint.rotation;

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = ball.GetComponent<Collider>();

        if (col != null)
            col.enabled = true;

        ball.gameObject.SetActive(true);
    }

    private void MoveWaitingBallToSelectedStartPoint()
    {
        Transform selectedStartPoint = GetCurrentStartPoint();

        if (selectedStartPoint == null)
            return;

        if (waitingBall == null)
        {
            SpawnWaitingBall();
            return;
        }

        PrepareBallAsWaiting(waitingBall, selectedStartPoint);
    }

    private void Bowl()
    {
        if (isBallInPlay)
        {
            Debug.Log("Current ball is still in play. Wait until it is destroyed.");
            return;
        }

        if (waitingBall == null)
        {
            SpawnWaitingBall();
        }

        if (waitingBall == null)
        {
            Debug.LogWarning("Waiting ball could not be created.");
            return;
        }

        if (bounceMarker == null)
        {
            Debug.LogWarning("Bounce Marker is missing.");
            return;
        }

        if (meter == null)
        {
            Debug.LogWarning("Meter is missing.");
            return;
        }

        Transform selectedStartPoint = GetCurrentStartPoint();

        if (selectedStartPoint == null)
        {
            Debug.LogWarning("Selected start point is missing.");
            return;
        }

        meter.StopMeter();

        float strength = meter.GetEffectStrength();
        string zone = meter.GetCurrentZone();

        if (useRandomSide)
            rightSide = Random.value > 0.5f;

        float selectedTimeToBounce = GetTimeToBounceByMode();

        Vector3 startPosition = selectedStartPoint.position;
        Vector3 bouncePoint = bounceMarker.GetBouncePoint();

        thrownBall = waitingBall;
        waitingBall = null;
        isBallInPlay = true;

        thrownBall.OnBallDestroyed += HandleThrownBallDestroyed;

        thrownBall.transform.position = startPosition;
        thrownBall.transform.rotation = selectedStartPoint.rotation;

        thrownBall.InitializeBall(
            startPosition,
            bouncePoint,
            currentMode,
            strength,
            rightSide,
            selectedTimeToBounce
        );

        if (resultText != null)
        {
            string sideName = rightSide ? "Right" : "Left";
            string speedName = currentMode == DeliveryMode.Swing ? "Fast Swing" : "Slow Spin";
            string startPointName = useStartPointB ? "Start B" : "Start A";

            resultText.text =
                zone + " | " +
                (strength * 100f).ToString("0") + "% | " +
                sideName + " | " +
                speedName + " | " +
                startPointName;
        }

        UpdateUI();
    }

    private void HandleThrownBallDestroyed(CricketBallPrefabPhysics destroyedBall)
    {
        if (destroyedBall != thrownBall)
            return;

        destroyedBall.OnBallDestroyed -= HandleThrownBallDestroyed;

        thrownBall = null;
        isBallInPlay = false;

        if (meter != null)
            meter.PrepareForNextThrow();

        SpawnWaitingBall();
        UpdateUI();
    }

    private void ResetAll()
    {
        if (thrownBall != null)
        {
            thrownBall.OnBallDestroyed -= HandleThrownBallDestroyed;
            Destroy(thrownBall.gameObject);
            thrownBall = null;
        }

        if (waitingBall != null)
        {
            Destroy(waitingBall.gameObject);
            waitingBall = null;
        }

        isBallInPlay = false;

        if (meter != null)
            meter.PrepareForNextThrow();

        if (resultText != null)
            resultText.text = "";

        SpawnWaitingBall();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (modeText != null)
            modeText.text = "Mode: " + currentMode.ToString();

        if (sideText != null)
        {
            string randomText = useRandomSide ? "Random" : "Manual";
            string sideName = rightSide ? "Right" : "Left";
            sideText.text = "Side: " + sideName + " | " + randomText;
        }

        if (startPointText != null)
            startPointText.text = useStartPointB ? "Start Point: B" : "Start Point: A";

        if (bowlButton != null)
            bowlButton.interactable = !isBallInPlay;
    }
}
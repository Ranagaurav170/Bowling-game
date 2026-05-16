using UnityEngine;
using TMPro;

public class CenterBasedBowlingMeter : MonoBehaviour
{
    [Header("Meter References")]
    public RectTransform meterArea;
    public RectTransform needle;

    [Header("Text References")]
    public TMP_Text percentText;
    public TMP_Text zoneText;

    [Header("Needle Movement")]
    [Tooltip("Decrease this value for slower needle movement.")]
    public float speed = 0.65f;

    public bool isRunning = true;

    [Header("Runtime Value")]
    [Range(0f, 1f)]
    public float meterValue = 0f;

    private int direction = 1;

    private void Update()
    {
        if (isRunning)
        {
            MoveNeedleValue();
        }

        UpdateNeedleUI();
        UpdateMeterText();
    }

    private void MoveNeedleValue()
    {
        meterValue += direction * speed * Time.deltaTime;

        if (meterValue >= 1f)
        {
            meterValue = 1f;
            direction = -1;
        }
        else if (meterValue <= 0f)
        {
            meterValue = 0f;
            direction = 1;
        }
    }

    private void UpdateNeedleUI()
    {
        if (meterArea == null || needle == null)
            return;

        float meterHeight = meterArea.rect.height;

        if (meterHeight <= 0.01f)
            return;

        float bottomY = -meterHeight * 0.5f;
        float topY = meterHeight * 0.5f;

        float yPos = Mathf.Lerp(bottomY, topY, meterValue);

        Vector2 needlePos = needle.anchoredPosition;
        needlePos.y = yPos;
        needle.anchoredPosition = needlePos;
    }

    public float GetEffectStrength()
    {
        float distanceFromCenter = Mathf.Abs(meterValue - 0.5f);
        float strength = 1f - distanceFromCenter * 2f;

        return Mathf.Clamp01(strength);
    }

    public float GetEffectPercent()
    {
        return GetEffectStrength() * 100f;
    }

    public string GetCurrentZone()
    {
        float percent = GetEffectPercent();

        if (percent >= 90f)
            return "Perfect";
        else if (percent >= 65f)
            return "Good";
        else if (percent >= 35f)
            return "Okay";
        else
            return "No Ball";
    }

    private void UpdateMeterText()
    {
        float percent = GetEffectPercent();

        if (percentText != null)
            percentText.text = percent.ToString("0") + "% Spin / Swing";

        if (zoneText != null)
            zoneText.text = GetCurrentZone();
    }

    public void StopMeter()
    {
        isRunning = false;
    }

    public void StartMeter()
    {
        isRunning = true;
    }

    public void ResetMeter()
    {
        meterValue = 0f;
        direction = 1;
        isRunning = true;

        UpdateNeedleUI();
        UpdateMeterText();
    }

    public void PrepareForNextThrow()
    {
        meterValue = 0f;
        direction = 1;
        isRunning = true;

        UpdateNeedleUI();
        UpdateMeterText();
    }
}
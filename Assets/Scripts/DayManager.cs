using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float workDayDuration = 180f; // 3 minutes
    private float elapsedTime = 0f;
    private int startHour = 7;
    private int endHour = 16;

    [Header("UI - HUD")]
    public TextMeshProUGUI clockText;
    public Image fadeImage;

    [Header("UI - Summary Screen")]
    public GameObject summaryPanel; // Drag in inspector
    public TextMeshProUGUI summaryStatsText; // Drag in inspector

    [Header("Day Info")]
    public int currentDay = 1;
    private bool dayActive = true;

    // === TRACKING STATS === //
    private int correctDecisions = 0;
    private int wrongDecisions = 0;
    private bool waitingForNextDay = false;

    void Update()
    {
        if (!dayActive) return;

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / workDayDuration);
        float currentHourFloat = Mathf.Lerp(startHour, endHour, t);

        UpdateClock(currentHourFloat);

        if (elapsedTime >= workDayDuration)
        {
            StartCoroutine(EndDayRoutine());
        }
    }

    void UpdateClock(float hourFloat)
    {
        int hours = Mathf.FloorToInt(hourFloat);
        int minutes = Mathf.FloorToInt((hourFloat - hours) * 60);

        string amPm = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        clockText.text = $"{displayHour:00}:{minutes:00} {amPm}";
    }

    // === NEW FUNCTION: Called by NPC Manager === //
    public void RegisterDecision(bool isCorrect)
    {
        if (isCorrect) correctDecisions++;
        else wrongDecisions++;

        Debug.Log($"Stats Updateed: {correctDecisions} correct, {wrongDecisions} wrong.");
    }

    // NEW FUNCTION: Button Event === //
    public void OnContinueButtonPressed()
    {
        waitingForNextDay = false; // Breaks the loop in EndDayRoutine
    }

    IEnumerator EndDayRoutine()
    {
        dayActive = false;

        // Snap clock
        clockText.text = "4:00 PM";

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        // --- SUMMARY UI --- //
        // 1. Show summary ui
        if (summaryPanel != null)
        {
            // Set the text
            summaryStatsText.text = $"SHIFT COMPLETE\n\n" +
                                    $"Correct Processed: {correctDecisions}\n" +
                                    $"Mistakes Made: {wrongDecisions}\n\n" +
                                    $"Day {currentDay} Concluded.";
            summaryPanel.SetActive(true);

            // 2. Wait for player input
            waitingForNextDay = true;
            while (waitingForNextDay)
            {
                yield return null;  // Wait one frame, then check again
            }

            // Clicked continue...
            summaryPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No Summary Panel assigned in DayManager!");
            yield return new WaitForSeconds(2f);
        }

        // 4. Start Next Day
        currentDay++;
        StartNewDay();

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f, 1.5f));

        dayActive = true;
    }

    void StartNewDay()
    {
        elapsedTime = 0f;

        // RESET STATS FOR NEW DAY
        correctDecisions = 0;
        wrongDecisions = 0;

        clockText.text = "07:00 AM";

        Debug.Log("Starting Day " + currentDay);

        // Later:
        // Reset quota
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}

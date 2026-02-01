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
    public Image fadeImage;

    [Header("UI - DESK")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI quotaText;
    public ShutterController shutterController;

    [Header("UI - Summary Screen")]
    public GameObject summaryPanel; // Drag in inspector
    public TextMeshProUGUI summaryStatsText; // Drag in inspector

    [Header("Day Info / Game Settings")]
    public int currentDay = 1;
    public int[] dailyQuotas = { 3, 5, 7, 9, 12 };
    private bool dayActive = true;
    private int currentQuota;

    // === TRACKING STATS === //
    private int correctDecisions = 0;
    private int wrongDecisions = 0;
    private bool waitingForNextDay = false;
    private int currentYield = 0;

    void Start()
    {
        StartNewDay();
    }

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
    public void RegisterDecision(bool isCorrect, bool addedToQuota)
    {
        if (isCorrect) correctDecisions++;
        else wrongDecisions++;

        // Did player do the right thing AND add meat to the pile...
        if (addedToQuota)
        {
            currentYield++;
        }

        UpdateQuotaUI();

        Debug.Log($"Stats Updateed: {correctDecisions} correct, {wrongDecisions} wrong.");
    }

    void UpdateQuotaUI()
    {
        if (quotaText != null)
        {
            // Format numbers to look like "03 / 05"
            string current = currentYield.ToString("D2");
            string target = currentQuota.ToString("D2");

            if (currentYield >= currentQuota)
            {
                // Green indicates safety/completion
                quotaText.color = Color.green;
                quotaText.text = $"{current} / {target}";
            }
            else
            {
                // Red indicates danger/incomplete
                quotaText.color = Color.red;
                quotaText.text = $"{current} / {target}";
            }
        }
    }

    // NEW FUNCTION: Button Event === //
    public void OnContinueButtonPressed()
    {
        waitingForNextDay = false; // Breaks the loop in EndDayRoutine
    }

    IEnumerator EndDayRoutine()
    {
        dayActive = false;

        if (shutterController != null) shutterController.CloseShutters();

        // Snap clock
        clockText.text = "4:00 PM";

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        // --- CALCULATE PAY ---
        int dailyWage = 15;
        int foodCost = 3;
        int totalEarned = dailyWage;

        // --- CALCULATE BONUS ---
        int bonus = 0;
        int extraYield = currentYield - currentQuota;

        if (extraYield >= 5)
        {
            bonus = (extraYield / 5) * 5;
        }

        totalEarned += bonus;

        // Add wage
        MoneyManager.AddMerits(totalEarned);

        // Deduct Food
        bool couldEat = MoneyManager.SpendMerits(foodCost);

        // Construct the Summary Message
        string foodStatus = couldEat ? "Rations: EATEN (-3)" : "Rations: SKIPPED (STARVING)";

        // --- SUMMARY UI --- //
        // 1. Show summary ui
        if (summaryPanel != null)
        {
            // Set the text & calculate if passed
            bool quotaMet = currentYield >= currentQuota;
            string status = quotaMet ? "<color=green>QUOTA MET</color>" : "<color=red>QUOTA FAILED</color>";
            string message = quotaMet ? "Performance Adequate." : "Productivity Unsatisfactory.\nWARNING ISSUED.";

            string bonusText = "";
            if (bonus > 0) bonusText = $"\nOverproduction Bonus: +{bonus}";

            summaryStatsText.text = $"SHIFT COMPLETE\n\n" +
                                    $"----------------\n" +
                                    $"Base Wage: +{dailyWage}" +
                                    $"{bonusText}\n" +
                                    $"Food Status: +{foodStatus}\n" +
                                    $"Merits: +{MoneyManager.currentMerits}\n" +
                                    $"----------------\n" +
                                    $"Quota: {currentQuota}\n" +
                                    $"Correct Processed: {correctDecisions}\n" +
                                    $"Mistakes Made: {wrongDecisions}\n\n" +
                                    $"----------------\n" +
                                    $"Status: {status}\n" +
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
        currentYield = 0;

        clockText.text = "07:00 AM";

        // --- GET TODAY'S QUOTA
        int quotaIndex = Mathf.Clamp(currentDay - 1, 0, dailyQuotas.Length - 1);
        currentQuota = dailyQuotas[quotaIndex];

        UpdateQuotaUI();

        if (shutterController != null) shutterController.OpenShutters();

        Debug.Log("Starting Day " + currentDay);
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

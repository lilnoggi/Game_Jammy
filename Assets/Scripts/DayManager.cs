using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    [Header("References")]
    public SpriteRandomizerLibrary randomiser;

    [Header("Time Settings")]
    public float workDayDuration = 120f; // <--- Now testing 2 minutes, 3 is too long and makes the game easy
    private float elapsedTime = 0f;
    private int startHour = 7;
    private int endHour = 17;

    [Header("UI - HUD")]
    public Image fadeImage;
    public GameObject gameOverPanel; // Drag in inspector
    public TextMeshProUGUI gameOverReasonText;
    [Header("UI Transitions")]
    public TextMeshProUGUI dayTitleText;
    [Header("Desk Tools")]
    public GameObject magnifyingGlassButton;
    public GameObject scannerPickupButton;

    [Header("UI - DESK")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI quotaText;
    public ShutterController shutterController;
    public Button startShiftButton; // Drag in inspector

    [Header("Day Info / Game Settings")]
    public int currentDay = 1;
    public int[] dailyQuotas = { 3, 5, 7, 9, 12, 14, 16 };
    private bool dayActive = true;
    public bool shiftStarted = false;
    private int currentQuota;
    public int maxDailyMistakes = 2;

    [Header("Economy Settings")]
    public int wagePerPerson = 5; // 5 credits per correct approval
    public int weeklyRentCost = 100; // Fixed rent
    public int dailyFoodCost = 10; // Fixed food 

    [Header("Performance Review")]
    public GameObject performancePanel;
    public TextMeshProUGUI quotaResultText; // "QUOTA MET"
    public TextMeshProUGUI statsText; // "Yield: 5/7"
    public TextMeshProUGUI managementMessageText; // "Do Better"

    [Header("Payslip UI")]
    public GameObject payslipPanel;
    public TextMeshProUGUI wageText;
    public TextMeshProUGUI rentText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI savingsText;

    [Header("Dialogue Connections")]
    public DialogueLibrary dialogueLibrary;
    public DialogueData[] dailyBriefings;

    // === TRACKING STATS === //
    private int correctDecisions = 0;
    private int wrongDecisions = 0;
    private bool waitingForInput = false;
    private int currentYield = 0;

    // === FAIL STATS === //
    private int strikes = 0;
    private int maxStrikes = 3;
    private int daysInDebt = 0;

    void Start()
    {
        // Ensure day title is off at start
        if (dayTitleText != null) dayTitleText.gameObject.SetActive(false);

        // Reset Money on day 1
        if (currentDay == 1)
        {
            MoneyManager.currentCredits = 50;
        }

        StartNewDay();
    }

    void Update()
    {
        // Timer only runs if the Day is Active AND the Shift has started
        if (!dayActive || !shiftStarted) return;

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / workDayDuration);
        float currentHourFloat = Mathf.Lerp(startHour, endHour, t);

        UpdateClock(currentHourFloat);

        if (elapsedTime >= workDayDuration)
        {
            StartCoroutine(EndDayRoutine());
        }
    }

    public void StartShift()
    {
        if (shiftStarted) return;

        shiftStarted = true;

        // Open the shutters
        if (shutterController != null) shutterController.OpenShutters();
    }

    void UpdateClock(float hourFloat)
    {
        int hours = Mathf.FloorToInt(hourFloat);
        int minutes = Mathf.FloorToInt((hourFloat - hours) * 60);

        string amPm = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        clockText.text = $"{displayHour:00}:{minutes:00} {amPm}";

        // --- FLASH RED IF NEAR END OF DAY ---
        if (hourFloat >= endHour - 1)
        {
            float flashSpeed = 3f;

            // PingPong moves values back and forth between 0 and 1
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);

            // Lerp between white and red
            clockText.color = Color.Lerp(Color.white, Color.red, t);
        }
    }

    // === Called by NPC Manager === //
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

    // Called by the "Continue" buttons on BOTH panels (Payslip & Performance Panels)//
    public void OnContinueButtonPressed()
    {
        waitingForInput = false; // Breaks the loop in EndDayRoutine
    }

    IEnumerator EndDayRoutine()
    {
        dayActive = false;
        shiftStarted = false;

        // Reset clock colour in case it ended on red
        clockText.color = Color.white;

        if (shutterController != null) shutterController.CloseShutters();

        // Snap clock
        clockText.text = "5:00 PM";

        // Fade out game to show summary
        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        // ====================================================
        // === CALCULATE ECO & PERFORMANCE ===
        // ====================================================

        // --- QUOTA CHECK ---
        bool quotaMet = currentYield >= currentQuota;
        if (!quotaMet)
        {
            strikes++; // PENATLY APPLIED
        }
        // --- QUALITY CHECK ---
        bool qualityMet = wrongDecisions <= maxDailyMistakes;
        if (!qualityMet)
        {
            strikes++;
        }

        // --- ECONOMY MATH ---
        // Income: Wage * Correct Approvals
        int dailyIncome = correctDecisions * wagePerPerson;
        // Always pay for food
        int todaysExpenses = dailyFoodCost;
        // Check if Rent is due (every 7 days)
        bool isRentDay = (currentDay % 7 == 0);
        if (isRentDay)
        {
            todaysExpenses += weeklyRentCost;
        }

        // Net Profit/Loss for today
        int netChange = dailyIncome - todaysExpenses;
        
        // Apply to player's credits
        MoneyManager.AddCredits(netChange); // If netChange is negative, this will subtract

        // --- RENT CHECK ---
        if (isRentDay && MoneyManager.currentCredits < 0)
        {
            TriggerGameOver("TERMINATION REASON:\nEVICTION.\n(FAILURE TO PAY WEEKLY RENT)");
            yield break;
        }

        // --- DEBT CHECK ---
        if (MoneyManager.currentCredits < 0)
        {
            daysInDebt++;
        }
        else
        {
            daysInDebt = 0;
        }

        // ====================================================
        // === GAME OVER CHECK ===
        // ====================================================

        // Fail State A: Incompetence (Strikes)
        if (strikes >= maxStrikes)
        {
            TriggerGameOver("TERMINATION REASON:\nREPEATED FAILURE TO MEET QUOTAS.");
            yield break;
        }

        // Fail State B: Insolvency (Debt)
        if (daysInDebt >= 2)
        {
             TriggerGameOver("TERMINATION REASON:\nYOU COULD NOT REPAY YOUR DEBT IN TIME.");
            yield break;
        }

        // ====================================================
        // === UPDATE THE UI ===
        // ====================================================

        // --- SHOW PERFORMANCE REVIEW PANEL ---
        if (performancePanel != null)
        {
            string resultString = quotaMet ? "QUOTA MET" : "QUOTA FAILED";
            Color resultColour = quotaMet ? Color.green : Color.red;

            // Management Message
            string managmentMessage = "";
            if (currentYield == 0)
            {
                managmentMessage = $"ZERO OUTPUT. EXPLAIN YOURSELF.\n(STRIKES: {strikes}/{maxStrikes})";
            }
            else if (!quotaMet && !qualityMet)
            {
                // Worst case: Slow AND Wrong
                managmentMessage = $"QUOTA MISSED & QUALITY LOW. TWO STRIKES ISSUES.\n(STRIKES: {strikes}/{maxStrikes})";
            }
            else if (!quotaMet)
            {
                managmentMessage = $"OUTPUT UNSATISFACTORY. WARNING ISSUED.\n(STRIKES: {strikes}/{maxStrikes})";
            }
            else if (!qualityMet)
            {
                // Bad Accuracy
                managmentMessage = $"TOO MANY ERRORS ({wrongDecisions}). QUALITY STRIKE ISSUED.\n(STRIKES: {strikes}/{maxStrikes})";
            }
            else managmentMessage = "PERFORMANCE ADEQUATE.";

            if (quotaResultText != null)
            {
                quotaResultText.text = resultString;
                quotaResultText.color = resultColour;
            }
            if (statsText != null) statsText.text = $"YIELD: {currentYield}/{currentQuota}\nMISTAKES: {wrongDecisions}";
            if (managementMessageText != null) managementMessageText.text = managmentMessage;

            performancePanel.SetActive(true);
        }

        // Wait for player to click "Next" on Performance Panel
        waitingForInput = true;
        while (waitingForInput) yield return null;

        // Hide performance panel
        if (performancePanel != null) performancePanel.SetActive(false);

        // --- SHOW PAYSLIP PANEL ---
        if (payslipPanel != null)
        {
            // Set Texts
            wageText.text = $"+ ${dailyIncome}";

            // Expense Text 
            if (isRentDay)
            {
                rentText.text = $"- ${todaysExpenses} (Food + WEEKLY RENT)";
            }
            else
            {
                int daysUntilRent = 7 - (currentDay % 7);
                rentText.text = $"${todaysExpenses} (Food Only)\n<size=60%>Rent due in {daysUntilRent} days</size>";
            }

            // Total Text
            // Colour code the daily total (Green for profit, Red for loss)
            if (netChange >= 0)
            {
                totalText.text = $"+ ${netChange}";
                totalText.color = Color.green;
            }
            else
            {
                totalText.text = $"- ${-netChange}";
                totalText.color = Color.red;
            }

            // Show final savings + Debt Warning
            if (daysInDebt > 0)
            {
                savingsText.text = $"WARNING Debt: ${MoneyManager.currentCredits}";
                savingsText.color = Color.red;
            }
            else
            {
                savingsText.text = $"SAVINGS: ${MoneyManager.currentCredits}";
                savingsText.color = Color.white;
            }

                // Turn on the panel
                payslipPanel.SetActive(true);
        }

        // Wait for player to click "Next Day" on payslip panel
        waitingForInput = true;
        while (waitingForInput) yield return null;

        // Hide payslip panel
        if (payslipPanel != null) payslipPanel.SetActive(false);

        // === CHECK FOR DEMO COMPLETION ===
        // If day 7 finished, they have paid rent and survived the week
        if (currentDay >= 7)
        {
            // Re-use Game over panel but with a VICTORY message
            // Make specific vic panel later
            TriggerGameOver("WEEK COMPLETED.\nRENT PAID.\n RETAINED FOR NEXT CYCLE.\n\n(THANKS FOR PLAYING THE DEMO!)");
            yield break;
        }

        // Increment day
        currentDay++;

        // ====================================================
        // === SHOW DAY TITLE TEXT ===
        // ====================================================
        // The screen is currently black
        // Show the text "DAY X" for a few seconds
        if (dayTitleText != null)
        {
            dayTitleText.text = $"DAY {currentDay}";
            dayTitleText.gameObject.SetActive(true);

            // Wait for 3 seconds so the player sees the day number
            yield return new WaitForSeconds(2f);

            dayTitleText.gameObject.SetActive(false);
        }

        StartNewDay();

        // Fade back in 
        yield return StartCoroutine(Fade(1f, 0f, 1.5f));
        dayActive = true;
    }

    void TriggerGameOver(string reason)
    {
        Debug.Log("GAME OVER: " + reason);
        if (gameOverPanel != null)
        {
            if (gameOverReasonText != null) gameOverReasonText.text = reason;

            gameOverPanel.SetActive(true);

        }
    }

    void StartNewDay()
    {
        // RESET STATS FOR NEW DAY
        elapsedTime = 0f;
        correctDecisions = 0;
        wrongDecisions = 0;
        currentYield = 0;
        clockText.color = Color.white; // Ensure clock is white

        // Reset States
        dayActive = true;
        shiftStarted = false; // <--- WAIT FOR PLAYER TO OPEN SHUTTERS

        // === DIFFICULT & RULES CONFIG ===
        // This ensures the game stays fair as it gets harder

        // Default everything to OFF first
        randomiser.bloodChance = 0f;
        randomiser.crackChance = 0f;
        randomiser.frownChance = 0f;
        randomiser.smileChance = 0f;
        randomiser.missingBarcodeChance = 0f;
        randomiser.smudgeChance = 0f;
        randomiser.forgeryChance = 0f;
        randomiser.perfectMaskChance = 0f;

        // Default Tools to OFF
        if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(false);
        if (scannerPickupButton != null) scannerPickupButton.SetActive(false);

        // Configure based on day

        switch (currentDay)
        {
            case 1:
                // Day 1: Visual Barcode Check only
                randomiser.perfectMaskChance = 45f;
                randomiser.missingBarcodeChance = 50f;

                // Flavor defects (Player ignores these today)
                randomiser.bloodChance = 2f;
                randomiser.crackChance = 7f;
                randomiser.frownChance = 20f;
                randomiser.smileChance = 10f;
                randomiser.smudgeChance = 3f;
                randomiser.forgeryChance = 0f;

                break;
            
            case 2:
                // Day 2: Add Blood + Unlock Mag Glass
                randomiser.perfectMaskChance = 40f;
                randomiser.bloodChance = 30f; // The main threat
                randomiser.missingBarcodeChance = 30f;

                randomiser.crackChance = 9f;
                randomiser.frownChance = 21f;
                randomiser.smileChance = 11f;
                randomiser.smudgeChance = 7f;
                randomiser.forgeryChance = 0f;

                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);

                break;

            case 3:
                // Day 3: Add cracks
                randomiser.perfectMaskChance = 30f;
                randomiser.crackChance = 30f; // The main threat
                randomiser.bloodChance = 20f;

                randomiser.frownChance = 21f;
                randomiser.smileChance = 11f;
                randomiser.missingBarcodeChance = 20f;
                randomiser.smudgeChance = 20f;
                randomiser.forgeryChance = 0f;

                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);
                
                break;

            case 4:
                // Day 4: Unlock Scanner (Invalid Barcoes) 
                randomiser.perfectMaskChance = 30f;
                randomiser.forgeryChance = 30f; // The main threat
                randomiser.missingBarcodeChance = 20f;
                randomiser.smudgeChance = 20f;

                randomiser.bloodChance = 15f;
                randomiser.crackChance = 15f;
                randomiser.frownChance = 30f;
                randomiser.smileChance = 20f;

                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);
                if (scannerPickupButton != null) scannerPickupButton.SetActive(true);
                
                break;

            case 5:
                // Day 5: reject Frowns only, neutral is okay
                randomiser.perfectMaskChance = 30f;
                randomiser.frownChance = 30f; // Increased threat

                randomiser.bloodChance = 15f;
                randomiser.crackChance = 15f;
                randomiser.smileChance = 20f;
                randomiser.missingBarcodeChance = 20f;
                randomiser.smudgeChance = 20f;
                randomiser.forgeryChance = 30f;

                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);
                if (scannerPickupButton != null) scannerPickupButton.SetActive(true);

                break;

            case 6:
                // Day 6: SMILES ONLY (Reject Neutral + Frown)
                randomiser.perfectMaskChance = 20f;
                randomiser.smileChance = 50f; // High chance of smile, but might have other defects!

                randomiser.frownChance = 20f;
                randomiser.bloodChance = 20f;
                randomiser.crackChance = 15f;
                randomiser.missingBarcodeChance = 10f; // Lower obvious errors
                randomiser.smudgeChance = 20f;
                randomiser.forgeryChance = 20f;



                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);
                if (scannerPickupButton != null) scannerPickupButton.SetActive(true);
                
                break;

            default:
                // Day 7: HELL
                randomiser.perfectMaskChance = 10f; // Almost no free passes

                randomiser.smileChance = 30f; // Lower smiles than day 6 = harder to find valid people
                randomiser.frownChance = 20f;
                randomiser.forgeryChance = 30f;
                randomiser.bloodChance = 20f;
                randomiser.crackChance = 20f;
                randomiser.smudgeChance = 20f;

                if (magnifyingGlassButton != null) magnifyingGlassButton.SetActive(true);
                if (scannerPickupButton != null) scannerPickupButton.SetActive(true);
                
                break;
        }

        if (shutterController != null) shutterController.CloseShutters();

        clockText.text = "07:00 AM";

        // --- GET TODAY'S QUOTA
        int quotaIndex = Mathf.Clamp(currentDay - 1, 0, dailyQuotas.Length - 1);
        currentQuota = dailyQuotas[quotaIndex];
        UpdateQuotaUI();

        // === UNLOCK DESK TOOLS === //
        // Day 3: Magnifying Glass
        if (magnifyingGlassButton != null)
        {
            magnifyingGlassButton.SetActive(currentDay >= 3);
        }

        // Day 4: Scanner Tool
        if (scannerPickupButton != null)
        {
            scannerPickupButton.SetActive(currentDay >= 4);
        }

        // === PLAY DAILY BRIEFING DIALOGUE === //
        int briefingIndex = currentDay - 1;

        if (dailyBriefings != null && briefingIndex < dailyBriefings.Length)
        {
            StartCoroutine(PlayDay1IntroSequence(dailyBriefings[briefingIndex]));
        }
        else
        {
            // Day 7+: You are on your own.
            if (startShiftButton != null)
            {
                startShiftButton.gameObject.SetActive(true);
                startShiftButton.interactable = true;
            }
        }
            Debug.Log("Starting Day " + currentDay);
    }

    IEnumerator PlayDay1IntroSequence(DialogueData briefingData)
    {
        // Disable the button so they can't click it yet
        if (startShiftButton != null)
        {
            startShiftButton.gameObject.SetActive(true);
            startShiftButton.interactable = false;
        }

        // Wait for fade-in to finish
        yield return new WaitForSeconds(1.5f);

        // Play Intro Dialogue
        if (dialogueLibrary != null && briefingData != null)
        {
            yield return StartCoroutine(dialogueLibrary.CreateDialogue(briefingData, false));
        }

        // Unlock button
        if (startShiftButton != null)
        {
            startShiftButton.interactable = true;
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = fadeImage.color;

        // Ensure the fade image is enabled
        fadeImage.gameObject.SetActive(true);

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

        // Only disable if fading out completely (Alpha 0)
        if (endAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

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
    public float workDayDuration = 180f; // 3 minutes
    private float elapsedTime = 0f;
    private int startHour = 7;
    private int endHour = 17;

    [Header("UI - HUD")]
    public Image fadeImage;
    public GameObject gameOverPanel; // Drag in inspector
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
    public int[] dailyQuotas = { 3, 5, 7, 9, 12 };
    private bool dayActive = true;
    public bool shiftStarted = false;
    private int currentQuota;

    [Header("Economy Settings")]
    public int wagePerPerson = 5; // 5 credits per correct approval
    public int rentCost = 20; // Fixed rent
    public int foodCost = 10; // Fixed food 

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

    void Start()
    {
        // Ensure day title is off at start
        if (dayTitleText != null) dayTitleText.gameObject.SetActive(false);
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
            float flashSpeed = 5f;

            // PingPong moves values back and forth between 0 and 1
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);

            // Lerp between white and red
            clockText.color = Color.Lerp(Color.white, Color.red, t);
        }
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

        // --- ECONOMY MATH ---
        // Income: Wage * Correct Approvals
        int dailyIncome = correctDecisions * wagePerPerson;
        // Expenses: Rent + Food
        int dailyExpenses = rentCost + foodCost;
        // Net Profit/Loss for today
        int netChange = dailyIncome - dailyExpenses;
        // Apply to player's credits
        MoneyManager.AddCredits(netChange); // If netChange is negative, this will subtract

        // --- PERFORMANCE MATH ---
        bool quotaMet = currentYield >= currentQuota;
        string resultString = quotaMet ? "QUOTA MET" : "QUOTA FAILED";
        Color resultColour = quotaMet ? Color.green : Color.red;

        string managmentMessage = "";
        if (currentYield == 0) managmentMessage = "ZERO OUTPUT. EXPLAIN YOURSELF.";
        else if (!quotaMet) managmentMessage = "OUTPUT UNSATISFACTORY. WARNING ISSUED";
        else if (wrongDecisions > 2) managmentMessage = "QUOTA MET. QUALITY LOW.";
        else managmentMessage = "PERFORMANCE ADEQUATE.";

        // ====================================================
        // === UPDATE THE UI ===
        // ====================================================

        // --- SHOW PERFORMANCE REVIEW PANEL ---
        if (performancePanel != null)
        {
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

        // --- SHOW ECO PANEL ---
        if (payslipPanel != null)
        {
            // Set Texts
            wageText.text = $"+ ${dailyIncome}";
            rentText.text = $"- ${dailyExpenses} (Rent & Food)";

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

            // Show final savings
            savingsText.text = $"SAVINGS: ${MoneyManager.currentCredits}";

            // Turn on the panel
            payslipPanel.SetActive(true);
        }

        // Wait for player to click "Next Day" on payslip panel
        waitingForInput = true;
        while (waitingForInput) yield return null;

        // Hide payslip panel
        if (payslipPanel != null) payslipPanel.SetActive(false);

        // ====================================================
        // === CHECK GAME OVER OR TRANSITION ===
        // ====================================================

        if (MoneyManager.currentCredits < 0)
        {
            // --- INSOLVENCY ENDING --- //
            Debug.Log("GAME OVER: Insolvency Reached.");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                yield break; // STOP THE GAME
            }
        }

        // Incrememnt day
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

        // === DIFFICULT BALANCING ===
        // This ensures the game stays fair as it gets harder

        if (currentDay == 1)
        {
            // Tutorial: No defects
            randomiser.perfectMaskChance = 10f; // Everyone can be let in
        }
        else if (currentDay == 2)
        {
            // Visuals only
            randomiser.perfectMaskChance = 0f;
            randomiser.bloodChance = 30f; // High chance of blood
            randomiser.crackChance = 10f;
            randomiser.forgeryChance = 0f;
        }
        else if (currentDay ==3)
        {
            // CRACKS
            randomiser.perfectMaskChance = 10f;
            randomiser.bloodChance = 15f;
            randomiser.crackChance = 40f;
            randomiser.forgeryChance = 0f;
        }
        else if (currentDay >= 4)
        {
            randomiser.perfectMaskChance = 40f;
            randomiser.bloodChance = 15f;
            randomiser.crackChance = 15f;
            randomiser.forgeryChance = 25f;
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

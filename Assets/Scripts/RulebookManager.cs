using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulebookManager : MonoBehaviour
{
    [Header("References")]
    public GameObject ruleBookPanel;
    public DayManager dayManager;
    public TextMeshProUGUI todaysRulesText;

    [Header("Page Spreads")]
    public GameObject[] pageSpreads; // Array of page spread GameObjects

    [Header("Navigation")]
    public Button nextButton;
    public Button prevButton;
    public Button exitButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pageTurnSound;

    private int currentIndex = 0;

    private void Start()
    {

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Update the dynamic text (Today's Rules)
        UpdateDailyRules();

        currentIndex = 0;
        // Show the first page
        UpdatePageVisibility();
    }

    public void OpenBook()
    {
        Time.timeScale = 0;
        if (pageTurnSound != null)
        {
            audioSource.PlayOneShot(pageTurnSound);
        }
        UpdateDailyRules();
        ruleBookPanel.SetActive(true);
        currentIndex = 0;
        UpdatePageVisibility();
    }

    void UpdateDailyRules()
    {
        if (todaysRulesText != null && dayManager != null)
        {
            switch (dayManager.currentDay)
            {
                case 1:
                    // DAY 1: FEEDING TIME
                    todaysRulesText.text = "DAY 1 PROTOCOLS:\n\n" +
                                           "1. CHECK BARCODE EXISTENCE.\n" +
                                           "   - If Missing: REJECT.\n" +
                                           "   - If Present: APPROVE.\n" +
                                           "NOTE: Ignore other defects today.\n +" +
                                           "Focus on speed.";
                    break;
                case 2:
                    // DAY 2: HYGIENE (Blood) + MAGNIFIER
                    todaysRulesText.text = "DAY 2 PROTOCOLS:\n\n" +
                                           "1. CHECK BARCODE.\n" +
                                           "2. CHECK HYGIENE (Blood).\n" +
                                           "TOOL UNLOCKED: MAGNIFYING GLASS.\n"+
                                           "Use it to inspect closely.";
                    break;
                case 3:
                    // DAY 3: INTEGRITY (Cracks)
                    todaysRulesText.text = "DAY 3 PROTOCOLS:\n\n" +
                                           "1. CHECK BARCODE (Must exist).\n" +
                                           "2. CHECK HYGIENE (Blood).\n" +
                                           "3. CHECK INTEGRITY (Cracks).\n" +
                                           "   - If Cracked: REJECT.\n\n" +
                                           "WARNING: Cracks may be small.\n" +
                                           "Use your Magnifier.";
                    break;
                case 4:
                    // DAY 4: SCANNER UNLOCK (Invalid Barcodes)
                    todaysRulesText.text = "DAY 4 PROTOCOLS:\n\n" +
                                           "SECURITY BREACH DETECTED.\n\n" +
                                           "1. SCAN EVERY BARCODE.\n" +
                                           "   - If Scanner says 'INVALID': REJECT.\n" +
                                           "   - If 'VALID': Continue.\n" +
                                           "2. CHECK BLOOD & CRACKS.\n\n" +
                                           "TOOL UNLOCKED: ID SCANNER.";
                    break;
                case 5:
                    // DAY 5: MORALE (Frowns)
                    todaysRulesText.text = "DAY 5 PROTOCOLS:\n\n" +
                                           "CITIZEN MORALE INITIATIVE.\n\n" +
                                           "1. CHECK FACIAL EXPRESSION.\n" +
                                           "   - If Frowning: REJECT.\n" +
                                           "2. SCAN BARCODES.\n" +
                                           "3. CHECK BLOOD & CRACKS.\n\n" +
                                           "Only happy citizens may enter.";
                    break;
                case 6:
                    // DAY 6: PROPAGANDA (Rejects Neutral)
                    todaysRulesText.text = "DAY 6: MANDATORY JOY ACT\n\n" +
                                           "Neutrality is complicity.\n\n" +
                                           "REJECT APPLICANT IF:\n" +
                                           "- Face is FROWNING.\n" +
                                           "- Face is NEUTRAL.\n\n" +
                                           "ONLY SMILES PERMITTED.\n" +
                                           "A blank face is a blank mind.\n" +
                                           "(Also Check: Blood, Cracks, Scan ID)";
                    break;
                default:
                    // DAY 7: EVERYTHING
                    todaysRulesText.text = "DAY 7: FINAL JUDGEMENT\n\n" +
                                           "RENT IS DUE: 5:00 PM ($100).\n\n" +
                                           "REJECT FOR ANY DEFECT:\n" +
                                           "- Invalid Barcode.\n" +
                                           "- Blood or Cracks.\n" +
                                           "- Frown OR Neutral Face.\n\n" +
                                           "ZERO TOLERANCE.\n" +
                                           "Survival is mandatory.";
                    break;
            }
        }
    }

    public void NextPage()
    {
        if (currentIndex < pageSpreads.Length - 1)
        {
            if (pageTurnSound != null)
            {
                audioSource.PlayOneShot(pageTurnSound);
            }

            currentIndex++;
            UpdatePageVisibility();
        }
    }

    public void PrevPage()
    {
        if (currentIndex > 0)
        {
            if (pageTurnSound != null)
            {
                audioSource.PlayOneShot(pageTurnSound);
            }
            currentIndex--;
            UpdatePageVisibility();
        }
    }

    void UpdatePageVisibility()
    {
        // Loop through all pages: Turn ON the current one, Turn OFF the rest
        for (int i = 0; i < pageSpreads.Length; i++)
        {
            pageSpreads[i].SetActive(i == currentIndex);
        }

        // Disable prev button on first page
        prevButton.interactable = currentIndex > 0;

        // 3. Disable next button on last page
        nextButton.interactable = (currentIndex < pageSpreads.Length - 1);
    }

    public void CloseBook()
    {
        Time.timeScale = 1;
        if (pageTurnSound != null)
        {
            audioSource.PlayOneShot(pageTurnSound);
        }

        ruleBookPanel.SetActive(false); // Hide the rulebook panel
    }
}

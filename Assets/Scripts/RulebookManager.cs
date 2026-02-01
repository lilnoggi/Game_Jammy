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
                    todaysRulesText.text = "SPECIAL ORDER:\n\n" +
                                           "Total intake required.\n" +
                                           "IGNORE all defects.\n" +
                                           "IGNORE all warnings.\n\n" +
                                           "ACCEPT EVERYONE.";
                    break;
                case 2:
                    // DAY 2: VISUAL CHECKS
                    todaysRulesText.text = "STANDARD PROTOCOL:\n\n" +
                                           "Reject applicant if:\n" +
                                           "- Mask is BLOODY\n\n" +
                                           "(Visual check only)";
                    break;
                case 3:
                    // DAY 3: MAGNIFIER UNLOCK (Blood + Cracks)
                    todaysRulesText.text = "QUALITY CONTROL:\n\n" +
                                           "Reject applicant if:\n" +
                                           "- Mask is BLOODY\n" +
                                           "- Mask is CRACKED\n\n" +
                                           "(Use Magnifying Glass)";
                    break;
                case 4:
                    // DAY 4: SCANNER UNLOCK (Blood + Cracks + Smudges)
                    todaysRulesText.text = "SECURITY UPDATE:\n\n" +
                                           "Reject applicant if:\n" +
                                           "- Mask is BLOODY\n" +
                                           "- Mask is CRACKED\n" +
                                           "- Barcode is SMUDGED\n" +
                                           "(Use ID Scanner)";
                    break;
                default:
                    // DAY 5+: HARD MODE (All Rules Active)
                    todaysRulesText.text = "MAXIMUM SECURITY:\n\n" +
                                           "Reject for ANY defect:\n" +
                                           "- BLOOD\n" +
                                           "- CRACKS\n" +
                                           "- SMUDGED BARCODE\n" +
                                           "(Zero Tolerance Policy)";
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
        if (pageTurnSound != null)
        {
            audioSource.PlayOneShot(pageTurnSound);
        }

        ruleBookPanel.SetActive(false); // Hide the rulebook panel
    }
}

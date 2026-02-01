using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulebookManager : MonoBehaviour
{
    // Book Panel containing all pages
    public GameObject ruleBookPanel;

    [Header("Page Spreads")]
    public GameObject[] pageSpreads; // Array of page spread GameObjects

    [Header("Navigation")]
    public Button nextButton;
    public Button prevButton;
    public Button exitButton;

    [Header("Dynamic Content")]
    // Update the rules text every day
    public TextMeshProUGUI todaysRulesText;
    public DayManager dayManager;

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

        // Show the first page
        UpdatePageVisibility();
    }

    public void OpenBook()
    {
        ruleBookPanel.SetActive(true);
    }

    void UpdateDailyRules()
    {
        if (todaysRulesText != null && dayManager != null)
        {
            switch (dayManager.currentDay)
            {
                case 1:
                    todaysRulesText.text = "Reject if:\n- A mask is CRACKED";
                    break;
                case 2:
                    todaysRulesText.text = "Reject if:\n- A mask is CRACKED\n- Barcode is MISSING";
                    break;
                case 3:
                    todaysRulesText.text = "Reject if:\n- A mask is CRACKED\n- Barcode is MISSING\n- Applicant is SAD";
                    break;
                default:
                    todaysRulesText.text = "Consult standard operating procedures.";
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

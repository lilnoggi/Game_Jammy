using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    // This class can be expanded to manage various button interactions in the game.
    // For now, this is relative to the RuleBookPanel GameObject.
    // Open and close the rule book

    public GameObject ruleBookPanel;

    void Start()
    {
        ruleBookPanel.SetActive(false); // Ensure the rule book panel is hidden at the start
    }

    public void OpenRuleBook()
    {
        ruleBookPanel.SetActive(true);
    }

    public void CloseRuleBook()
    {
        ruleBookPanel.SetActive(false);
    }
}

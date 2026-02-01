using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Button buyScannerButton;
    public Button buyMagnifierButton;
    public TextMeshProUGUI statusText; // Show "Bought!" or "Too Poor!"

    void Update()
    {
        // Disable buttons if already owned
        buyScannerButton.interactable = !MoneyManager.hasScanner;
        buyMagnifierButton.interactable = !MoneyManager.hasMagnifier;
    }

    public void BuyScanner()
    {
        if (MoneyManager.SpendMerits(15))
        {
            MoneyManager.hasScanner = true;
            statusText.text = "Bought Scanner!";
        }
        else
        {
            statusText.text = "Too Poor for Scanner!";
        }
    }

    public void BuyMagnifier()
    {
        if (MoneyManager.SpendMerits(5))
        { 
            MoneyManager.hasMagnifier = true;
            statusText.text = "Bought Magnifier!";
        }
        else
        {
            statusText.text = "Too Poor for Magnifier!";
        }
    }
}

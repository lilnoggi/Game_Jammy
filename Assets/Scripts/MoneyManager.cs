using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int currentMerits = 0; // Starts at 0

    // Track unlocked tools 
    public static bool hasScanner = false;
    public static bool hasMagnifier = false;

    public static void AddMerits(int amount)
    {
        currentMerits += amount;
        Debug.Log($"Paid {amount}. Balance: {currentMerits}");
    }

    public static bool SpendMerits(int amount)
    {
        if (currentMerits >= amount)
        {
            currentMerits -= amount;
            Debug.Log($"Spent {amount}. Balance: {currentMerits}");
            return true;
        }
        else
        {
            Debug.Log("Insufficient Funds!");
            return false;
        }
    }
}

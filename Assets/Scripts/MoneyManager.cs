using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int currentCredits= 50; // Start at 50 so they don't die immediately.

    public static void AddCredits(int amount)
    {
        currentCredits += amount;
        Debug.Log($"Paid {amount}. Balance: {currentCredits}");
    }

    public static void SpendCredits(int amount)
    {
        currentCredits -= amount;
        Debug.Log($"Spent {amount}. Balance: {currentCredits}");
    }
}

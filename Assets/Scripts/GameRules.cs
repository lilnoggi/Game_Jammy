using UnityEngine;

/// <summary>
/// This script handles the logic for who is allowed in and who is not.
/// It doesn't store data, it just checks it :DDDDDDD
/// </summary>
public static class GameRules
{
    // static class so it can be called from anywhere without needing to instantiate it / GetComponent
    public static bool CheckIfRefugeeIsValid(Refugee refugee, int currentDay)
    {
        // ==========================================================
        // DAY 1: THE FEEDING (Tutorial / Easy Mode)
        // ==========================================================
        // Rule: Let everyone in. The billionaires are hungry.
        if (currentDay == 1)
        {
            return true; // Auto-pass everyone
        }

        // ==========================================================
        // ABSOLUTE RULES (Apply from Day 2 onwards)
        // ==========================================================

        // RULE: NO BLOOD
        // This is visible to the naked eye, so it applies immediately from Day 2.
        if (refugee.isBloody)
        {
            Debug.Log("Rule Check: Rejected due to BLOOD.");
            return false;
        }

        // ==========================================================
        // DAY 3+: MAGNIFIER RULES 
        // ==========================================================
        // New Rule: No cracks in the mask.
        if (currentDay >= 3)
        {
            // Check for cracks
            if (refugee.isCracked)
            {
                Debug.Log("Rule Check: Rejected due to SMUDGED BARCODE.");
                return false;
            }
        }

        // ==========================================================
        // DAY 4+: SCANNER RULES 
        // ==========================================================
        // New Rule: No Cracks.
        if (currentDay >= 4)
        {
            if (refugee.isSmudged)
            {
                Debug.Log("Rule Check: Rejected due to SMUDGED BARCODE.");
                return false;
            }

            if (!refugee.hasBarcode)
            {
                Debug.Log("Rule Check: Rejected due to MISSING BARCODE.");
                return false;
            }
        }

        // ==========================================================
        // FINAL VERDICT
        // ==========================================================
        // If they survived all the checks above, they are VALID.
        return true;
    }
}

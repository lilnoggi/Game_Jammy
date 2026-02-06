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
        // UNIVERSAL RULE (Applies from Day 1 onwards)
        // ==========================================================
        // Rulebook: "CHECK BARCODE EXISTENCE... If Missing: REJECT"
        // This is the only rule active on Day 1, but it stays active forever.
        if (!refugee.hasBarcode || refugee.isSmudged)
        {
            Debug.Log($"[Day {currentDay}] Rejected: MISSING BARCODE.");
            return false;
        }

        // ==========================================================
        // DAY 2+: HYGIENE (Blood)
        // ==========================================================
        // Rulebook: "CHECK HYGIENE... If Bloody: REJECT."
        if (currentDay >= 2)
        {
            if (refugee.isBloody)
            {
                Debug.Log($"[Day {currentDay}] Rejected: BLOOD DETECTED.");
                return false;
            }
        }

        // ==========================================================
        // DAY 3+: INTEGRITY (Cracks)
        // ==========================================================
        // Rulebook: "CHECK INTEGRITY... If Cracked: REJECT."
        if (currentDay >= 3)
        {
            if (refugee.isCracked)
            {
                Debug.Log($"[Day {currentDay}] Rejected: MASK CRACKED.");
                return false;
            }
        }

        // ==========================================================
        // DAY 4+: SCANNER (Invalid Barcodes)
        // ==========================================================
        // Rulebook: "SCAN EVERY BARCODE... If Invalid: REJECT."
        // Note: The player must use the tool to see this, but the Logic simply checks the bool.
        if (currentDay >= 4)
        {
            if (!refugee.isValidBarcode)
            {
                Debug.Log($"[Day {currentDay}] Rejected: INVALID ID / FORGERY.");
                return false;
            }

            // but usually Smudged = Invalid in the code logic.
            if (refugee.isSmudged)
            {
                Debug.Log($"[Day {currentDay}] Rejected: SMUDGED BARCODE.");
                return false;
            }
        }

        // ==========================================================
        // DAY 5+: MORALE (Frowns)
        // ==========================================================
        // Rulebook: "If Frowning: REJECT."
        if (currentDay >= 5)
        {
            if (refugee.isSad)
            {
                Debug.Log($"[Day {currentDay}] Rejected: FROWNING (Low Morale).");
                return false;
            }
        }

        // ==========================================================
        // DAY 6+: MANDATORY JOY (Reject Neutrals)
        // ==========================================================
        // Rulebook: "ONLY SMILES PERMITTED... Reject Neutral."
        if (currentDay >= 6)
        {
            // Already rejected "isSad" (Frowns) in the block above.
            // Now check if they are NOT happy. 
            // If they are not happy (and not sad), they must be Neutral.
            if (!refugee.isHappy)
            {
                Debug.Log($"[Day {currentDay}] Rejected: NEUTRAL EXPRESSION (Mandatory Joy Act).");
                return false;
            }
        }

        // ==========================================================
        // FINAL VERDICT
        // ==========================================================
        // If they survived all the gauntlets above, they are VALID.
        return true;

    }
}

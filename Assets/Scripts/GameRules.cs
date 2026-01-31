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
        // --- ABSOLUTE RULES (Apply every single day) ---

        // 1. Blood is immediate rejection.
        if (refugee.isBloody)
        {
            Debug.Log("Rule Check: Refugee rejected due to BLOOD.");
            return false;
        }

        // 2. Smudged barcodes are invalid (cannot be scanned/processed).
        if (refugee.isSmudged)
        {
            Debug.Log("Rule Check: Refugee rejected due to SMUDGE");
            return false;
        }

        // --- DAILY RULES (Get harder as days progress) ---

        switch (currentDay)
        {
            case 1:
                // DAY 1: No Cracks allowed.
                if (refugee.isCracked)
                {
                    Debug.Log("Rule Check: Day 1 - Rejected due to CRACK.");
                    return false;
                }
                break;

            case 2:
                // DAY 2: No Cracks AND Must have Barcode.
                if (refugee.isCracked) return false;

                if (!refugee.hasBarcode)
                {
                    Debug.Log("Rule Check: Day 2 - Rejected due to MISSING BARCODE.");
                    return false;
                }
                break;

            case 3:
                // Day 3: No Cracks, Must have Barcode, NO SADNESS.
                if (refugee.isCracked) return false;
                if (!refugee.hasBarcode) return false;

                if (refugee.isSad)
                {
                    Debug.Log("Rule Check: Day 3 - Rejected due to SADNESS.");
                    return false;
                }
                break;

            default:
                // If past day 3, default to the hardest rules
                if (refugee.isCracked || !refugee.hasBarcode || refugee.isSad) return false;
                break;

        }

        // If we sruvived all the if-statements above, the Refugee is VALID.
        Debug.Log("Rule Check: Refugee PASSED inspection.");
        return true;
    }
}

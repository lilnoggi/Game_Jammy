using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    public DayManager dayManager; // <--- DRAG DAYMANAGER HERE IN INSPECTOR!
    public NameDatabase masterNameList;

    [Header("Prefabs & Spawning")]
    public GameObject refugeePrefab;
    public GameObject refugeeMaskPrefab;
    public Transform spawnRight;
    public Transform standPoint;
    public Transform exitLeft;

    [Header("Current State")]
    public Refugee currentRefugee;
    //public MaskData[] possibleMasks;

    [Header("Dialogue Database")]
    public DialogueData[] genericDialogues;
    public DialogueData[] bloodyDialogues;
    public DialogueData[] crackedDialogues;
    public DialogueData[] smudgedDialogues;
    public DialogueData[] noBarcodeDialogues;
    public DialogueData[] sadDialogues;

    [Header("Feedback")]
    public CameraShake cameraShake;

    [Header("Management Feedback")]
    public DialogueLibrary dialogueLibrary;
    public DialogueData mistakeDialogue;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip approveSound, grinderSFX, errorSFX, callNextSFX, buttonPressedSFX;
    public AudioClip[] gunshotSounds;

    // Flag to prevent spamming buttons
    private bool isBoothOccupied = false;


    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // === Call next refugee === //
    public void OnCallNextPressed()
    {
        if (!dayManager.shiftStarted) return; // No spawning if day is not active

        // 1. Don't do ANYTHING if someone is there
        if (isBoothOccupied || currentRefugee != null) return;

        // 2. Spawn the new person
        SpawnRefugee(1f);

        // 3. Play sound
        if (callNextSFX != null) audioSource.PlayOneShot(callNextSFX);
    }

    void SpawnRefugee(float mask_rarity)
    {
        isBoothOccupied = true; // Mark booth as occupied

        GameObject ref_obj = Instantiate(refugeePrefab, spawnRight.position, Quaternion.identity);

        // Pass the offset from SpriteRandomizerLibrary
        Vector3 maskOffset = new Vector3(0.012f, 1f, 0f);

        // Create the mask using the Randomiser Library
        GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(
            refugeeMaskPrefab,
            ref_obj,
            maskOffset, mask_rarity);

        currentRefugee = ref_obj.GetComponent<Refugee>();

        // ====================================================
        // === GENERATE NAME DATA ===
        // ====================================================

        // 1. Generate name
        if (masterNameList != null)
        {
            currentRefugee.refugeeName = masterNameList.GetRandomName();
        }
        else
        {
            currentRefugee.refugeeName = "Unkown";
        }

        // ====================================================

        currentRefugee.MoveTo(standPoint.position);

        // ====================================================
        // === SELECT CONTEXTUAL DIALOGUE ===
        // ====================================================

        DialogueData selectedDialogue = GetContextualDialogue();

        if (selectedDialogue != null)
        {
            // Pass the name
            StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(selectedDialogue,
                true,
                currentRefugee.refugeeName));
        }
    }

    // === PICK DIALOGUE BASED ON LOOKS ===
    DialogueData GetContextualDialogue()
    {
        // Create a list of potential lists
        // If refugee has a crack, add 'crackedDialogues' array to potential pool
        List<DialogueData[]> validPools = new List<DialogueData[]>();

        // 1. Check for defects
        if (currentRefugee.isBloody && bloodyDialogues.Length > 0)
            validPools.Add(bloodyDialogues);

        if (currentRefugee.isCracked && crackedDialogues.Length > 0)
            validPools.Add(crackedDialogues);

        if (currentRefugee.isSmudged && smudgedDialogues.Length > 0)
            validPools.Add(smudgedDialogues);

        if (!currentRefugee.hasBarcode && noBarcodeDialogues.Length > 0)
            validPools.Add(noBarcodeDialogues);

        if (currentRefugee.isSad && sadDialogues.Length > 0)
            validPools.Add(sadDialogues);

        // 2. Decide what to return
        if (validPools.Count > 0)
        {
            // If NPC has defects, pick a random defect to talk about
            DialogueData[] chosenArray = validPools[Random.Range(0, validPools.Count)];
            return chosenArray[Random.Range(0, chosenArray.Length)];
        }
        else
        {
            // 3. If no defects use Generic
            if (genericDialogues.Length > 0)
            {
                return genericDialogues[Random.Range(0, genericDialogues.Length)];
            }
        }

        // Fallback to prevent crash if lists are empty
        Debug.LogWarning("No dialogue found! Returning to default.");
        if (genericDialogues.Length > 0) return genericDialogues[0];

        return null;
    }


    // === HELPER FUNCTION: Generate Random ID === //
    string GenerateRandomID()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string part1 = "";
        string part2 = "";

        // Generate format XX-XXX
        for (int i = 0; i < 2; i++) part1 += chars[Random.Range(0, chars.Length)];
        for (int i = 0; i < 3; i++) part2 += chars[Random.Range(0, chars.Length)];

        return $"{part1}-{part2}";
    }

    public void ApproveRefugee()
    {
        if (currentRefugee == null) return;

        // GAME RULES CHECK
        bool isValid = GameRules.CheckIfRefugeeIsValid(currentRefugee, dayManager.currentDay);

        // === REPORT TO DAY MANAGER ===
        // 1. isCorrect? Yes, if isValid is true.
        // 2. addedToQuota? Yes, ONLY if isValid is true.
        dayManager.RegisterDecision(isValid, isValid);

        if (isValid)
        {
            // SUCCESS: They were valid and they were let in.
            Debug.Log("DECISION: Correct! Fresh meat aquired *smirk wink face*");
            // Add score / Money here

            if (callNextSFX != null) audioSource.PlayOneShot(approveSound);
        }
        else
        {
            // FAILURE (You let a bad one in)
            Debug.Log("DECISION: Incorrect! Contamination!");
            if (errorSFX != null) audioSource.PlayOneShot(errorSFX);

            if (cameraShake != null)
            {
                cameraShake.Shake(0.25f,02f);
            }

            // Use 'mistakeDialogue' and set isNPC to 'false' so it plays the full text.
            if (mistakeDialogue != null)
            {
                // Pass TRUE (so it picks a random line)
                // Pass "MANAGEMENT" (so it uses the correct name!
                StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(mistakeDialogue, true, "MANAGEMENT"));
            }
        }

        // Cleanup
        StartCoroutine(RemoveCurrentRefugee());
    }

    public void RejectRefugee()
    {
        if (currentRefugee == null) return;

        // GAME RULES CHECK
        bool isValid = GameRules.CheckIfRefugeeIsValid(currentRefugee, dayManager.currentDay);

        // === REPORT TO DAY MANAGER ===
        // 1. isCorrect? Yes, if !isValid.
        // 2. addedToQuota? No, since they were rejected.
        dayManager.RegisterDecision(!isValid, false);

        // Start execution Sequence
        StartCoroutine(ExecuteOffScreenKill(currentRefugee, isValid));

        // Clear booth reference
        currentRefugee = null;
    }

    // === HELPER FUNCTION ===
    void PlayRandomGunShot()
    {
        if (gunshotSounds != null && gunshotSounds.Length > 0)
        {
            // Pick a random index
            int randomIndex = Random.Range(0, gunshotSounds.Length);

            // Play it
            audioSource.PlayOneShot(gunshotSounds[randomIndex]);
        }
        else
        {
            Debug.LogWarning("No Gunshot Sounds assigned in NPCManager!");
        }
    }

    // COROUTINE: NPC Walks, waits, then is shot
    IEnumerator ExecuteOffScreenKill(Refugee victim, bool wasActuallyValid)
    {
        // 1. Tell NPC to walk away
        if (victim != null)
        {
            victim.MoveTo(exitLeft.position);
        }

        // 2. WAIT for them to walk off-screen
        yield return new WaitForSeconds(2f);

        // 3. Play Gunshot
        PlayRandomGunShot();

        // 4. Feedback
        if (wasActuallyValid)
        {
            Debug.Log("DECISION: Incorrect! You rejected a valid product.");

            // Play error sound
            if (errorSFX != null) audioSource.PlayOneShot(errorSFX);

            // Shake the screen violently
            if (cameraShake != null) cameraShake.Shake(0.2f, 0.15f);

            if (mistakeDialogue != null)
            {
                // Pass true for one line + "MANAGEMENT"
                StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(mistakeDialogue, true, "MANAGEMENT"));
            }
        }
        else
        {
            // If they were invalid, right choice was made
            Debug.Log("DECISION: Correct! Trash disposed.");
        }

        // Destroy Evidence
        if (victim != null)
        {
            Destroy(victim.gameObject);
        }

        // Reset booth
        yield return new WaitForSeconds(0.5f);
        isBoothOccupied = false;
    }

    // HELPER FUNCTION
    IEnumerator RemoveCurrentRefugee()
    {
        // Move current refugee away
        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        // Wait for them to leave
        yield return new WaitForSeconds(2f);

        isBoothOccupied = false; // Mark booth as free
    }
}

using UnityEngine;
using System.Collections;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    public DayManager dayManager; // <--- DRAG DAYMANAGER HERE IN INSPECTOR!

    [Header("Prefabs & Spawning")]
    public GameObject refugeePrefab;
    public GameObject refugeeMaskPrefab;
    public Transform spawnRight;
    public Transform standPoint;
    public Transform exitLeft;

    [Header("Current State")]
    public Refugee currentRefugee;
    public MaskData[] possibleMasks;
    public DialogueData[] possibleDialogues;

    [Header("Management Feedback")]
    public DialogueLibrary dialogueLibrary;
    public DialogueData mistakeDialogue;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip approveSound, grinderSFX, errorSFX, callNextSFX;

    // Flag to prevent spamming buttons
    private bool isBoothOccupied = false;


    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // === NEW FUNC: Call next refugee === //
    public void OnCallNextPressed()
    {
        if (!dayManager.shiftStarted) return; // No spawning if day is not active

        // 1. Don't do ANYTHING if someone is there
        if (isBoothOccupied || currentRefugee != null) return;

        // 2. Spawn the new person
        SpawnRefugee(.8f);

        // 3. Play sound
        if (callNextSFX != null) audioSource.PlayOneShot(callNextSFX);
    }

    void SpawnRefugee(float mask_rarity)
    {
        isBoothOccupied = true; // Mark booth as occupied

        GameObject ref_obj = Instantiate(refugeePrefab, spawnRight.position, Quaternion.identity);
        //GameObject ref_mask = GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, ref_obj, (spawnRight.position + new Vector3(.012f,1.65f,0)), mask_rarity);

        // Calculate the offset for the mask
        Vector3 maskPos = spawnRight.position + new Vector3(.012f, 1.65f, 0);

        // Create the mask using the Randomiser Library
        GameObject ref_mask = GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, ref_obj, maskPos, mask_rarity);

        currentRefugee = ref_obj.GetComponent<Refugee>();
        currentRefugee.MoveTo(standPoint.position);

        StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(
    possibleDialogues[Random.Range(0, possibleDialogues.Length)],
    true));


    }


    public void ApproveRefugee()
    {
        if (currentRefugee == null) return;

        /*
        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        SpawnRefugee(.8f);
        */

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

            // Use 'mistakeDialogue' and set isNPC to 'false' so it plays the full text.
            if (mistakeDialogue != null)
            {
                StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(mistakeDialogue, true));
            }
        }

        // Cleanup
        StartCoroutine(RemoveCurrentRefugee());
    }

    public void RejectRefugee()
    {
        /*
        if (currentRefugee == null) return;

        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        SpawnRefugee(.8f);
        */

        // GAME RULES CHECK
        bool isValid = GameRules.CheckIfRefugeeIsValid(currentRefugee, dayManager.currentDay);

        // === REPORT TO DAY MANAGER ===
        // 1. isCorrect? Yes, if !isValid.
        // 2. addedToQuota? No, since they were rejected.
        dayManager.RegisterDecision(!isValid, false);

        // Inverse logic for rejection
        if (!isValid)
        {
            // SUCCESS (You rejected a bad one)
            Debug.Log("DECISION: Correct! Trash disposed.");
            if (callNextSFX != null) audioSource.PlayOneShot(approveSound);
        }
        else
        {
            // FAILURE (You rejected a good one)
            Debug.Log("DECISION: Incorrect! Waste of resources.");
            if (errorSFX != null) audioSource.PlayOneShot(errorSFX);

            // Use 'mistakeDialogue' and set isNPC to 'false'.
            if (mistakeDialogue != null)
            {
                StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(mistakeDialogue, true));
            }

        }

        // Cleanup
        StartCoroutine(RemoveCurrentRefugee());
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

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


    void Start()
    {
        SpawnRefugee(.8f);
    }

    void SpawnRefugee(float mask_rarity)
    {
        GameObject ref_obj = Instantiate(refugeePrefab, spawnRight.position, Quaternion.identity);
        //GameObject ref_mask = GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, ref_obj, (spawnRight.position + new Vector3(.012f,1.65f,0)), mask_rarity);

        // Calculate the offset for the mask
        Vector3 maskPos = spawnRight.position + new Vector3(.012f, 1.65f, 0);

        // Create the mask using the Randomiser Library
        GameObject ref_mask = GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, ref_obj, maskPos, mask_rarity);

        currentRefugee = ref_obj.GetComponent<Refugee>();
        currentRefugee.MoveTo(standPoint.position);
        
        StartCoroutine(GetComponent<DialogueLibrary>().CreateDialogue(possibleDialogues[Random.Range(0, possibleDialogues.Length)], 2f));

        /* DEPRECATED / OLD
        GameObject obj = Instantiate(refugeePrefab,spawnRight.position,Quaternion.identity);
        currentRefugee = obj.GetComponent<Refugee>();
        GetComponent<SpriteRandomizerLibrary>().InstantiateRandomMask(refugeeMaskPrefab, obj,new Vector3(0,0,0));
        refugeeMaskPrefab.gameObject.transform.position = new Vector3(0,0,0);
        currentRefugee.MoveTo(standPoint.position);
        */
        /* DEPRECATED / OLD
        // Assign random mask
        MaskData randomMask = possibleMasks[Random.Range(0, possibleMasks.Length)];
        currentRefugee.SetMask(randomMask);

        currentRefugee.MoveTo(standPoint.position);
        */
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

        if (isValid)
        {
            // SUCCESS: They were valid and they were let in.
            Debug.Log("DECISION: Correct! Fresh meat aquired *smirk wink face*");
            // Add score / Money here
        }
        else
        {
            // FAILURE: They were bad but you let them in D:
            Debug.Log("DECISION: Incorrect! You let in a bad refugee... oops.");
            // Subtract Score
        }

        // Cleanup
        StartCoroutine(RemoveCurrentAndSpawnNew());
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

        // Inverse logic for rejection
        if (!isValid)
        {
            // SUCCESS: They were bad and you rejected them.
            Debug.Log("DECISION: Correct! You rejected a bad refugee.");
            // Add score / Money here
        }
        else
        {
            // FAILURE: They were good but you rejected them D:
            Debug.Log("DECISION: Incorrect! You rejected a good refugee... oops.");
            // Subtract Score

        }

        // Cleanup
        StartCoroutine(RemoveCurrentAndSpawnNew());
    }

    // HELPER FUNCTION
    IEnumerator RemoveCurrentAndSpawnNew()
    {
        currentRefugee.MoveTo(exitLeft.position);
        Destroy(currentRefugee.gameObject, 2f);
        currentRefugee = null;

        yield return new WaitForSeconds(2f);

        SpawnRefugee(.8f);
    }
}

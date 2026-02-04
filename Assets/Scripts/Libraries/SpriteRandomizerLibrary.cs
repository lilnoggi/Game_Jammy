using UnityEngine;
using System.Collections.Generic;

public class SpriteRandomizerLibrary : MonoBehaviour
{

    // ============================================== //
    /* 

                The way this works is that when you call the function, you can give it a prefab, and regardless of
                what sprites are on that prefab, it will randomize their visibility

                The way it'll work is again, it will be a component in the Game Master object,
                and when called, it will create a mask and set it as a child of whatever game object you give it.

                
    !!!                IMPORTANT: Make sure that the sprites you want to randomize have their "Tag" set properly
                (e.g. Barcode, Crack, Frown, Blood) so that the script can identify them and set properties accordingly
    
    !!!         When you call the function, the "rarity" parameter is a FLOAT between 0 and 1.
                A rarity of 0 means all sprites are enabled, while a rarity of 1 means all sprites are disabled.     

    */

    // ============================================== //
    /* 
     * NEW SYSTEM LOGIC - By Mani
     * - Uses Sliders in the Inspector to control rarity for EACH individual feature
     * - Still scans the prefab for tags ("Blood", "Crack", etc).
     * - Applies the result to the Refugee script automatically.
     */
    // ============================================== //

    /*
    // ========== Sprite Randomizer Class =========== //
    // COMMENTED OUT FOR NEW VERSION
    public GameObject mask_prefab;
    //private GameObject canvas;



    [Header("Properties")]
    public bool hasBarcode;
    public bool isNormal;
    public bool isSad;
    public bool isCracked;
    public bool isSmudged;
    public bool isBloody;
    */

    [Header("Feature Rarity (Chance % 0-100)")]
    [Range(0, 100)] public float bloodChance = 10f;
    [Range(0, 100)] public float crackChance = 10f;
    [Range(0, 100)] public float frownChance = 20f;
    [Range(0, 100)] public float smileChance = 20f;
    [Header("Barcode Rarity")]
    [Range(0, 100)] public float missingBarcodeChance = 5f;
    [Range(0, 100)] public float smudgeChance = 10f;
    [Range(0, 100)] public float forgeryChance = 20f;

    [Header("Perfect Mask Override")]
    [Tooltip("If true, this % chance ignores all defects.")]
    [Range(0, 100)] public float perfectMaskChance = 0f;

    // Internal flags to track what is decided
    private bool _shouldBeBloody;
    private bool _shouldBeCracked;
    private bool _shouldHaveBarcode;
    private bool _shouldBeSmudged;
    private bool _shouldFrown;
    private bool _shouldSmile;
    private bool _isForgery;

    // Called by NPCManager
    public GameObject InstantiateRandomMask(GameObject prefab,
                                            GameObject parent, 
                                            Vector3 localOffset,
                                            float unusedRarity = 0f)
    {
        // === DECIDE FIRST ===
        RollDice();

        // === INSTANTIATE DIRECTLY AS CHILD ===
        GameObject inst_mask = Instantiate(prefab, parent.transform);

        // Set the LOCAL pos (offset from face)
        inst_mask.transform.localPosition = localOffset;

        // Ensure rotation is zeroed out relative to parent
        inst_mask.transform.localRotation = Quaternion.identity;

        // === GATHER THE PARTS ===
        List<SpriteRenderer> cracks = new List<SpriteRenderer>();
        List<SpriteRenderer> bloods = new List<SpriteRenderer>();
        List<SpriteRenderer> frowns = new List<SpriteRenderer>();
        List<SpriteRenderer> barcodes = new List<SpriteRenderer>();
        List<SpriteRenderer> smiles = new List<SpriteRenderer>();

        // === APPLY VISUALS ===
        // Loop through children and enable based on dice roll
        SpriteRenderer[] allRenderers = inst_mask.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in allRenderers)
        {
            if (sr.CompareTag("Crack")) cracks.Add(sr);
            else if (sr.CompareTag("Blood")) bloods.Add(sr);
            else if (sr.CompareTag("Frown")) frowns.Add(sr);
            else if (sr.CompareTag("Smile")) smiles.Add(sr);
            else if (sr.CompareTag("Barcode")) barcodes.Add(sr);

            // Ensure everything starts invisible/disabled so we can turn on only what we want
            // Exception: Don't disable the main mask background (assumed it has no tag or "Untagged")
            if (!sr.CompareTag("Untagged"))
            {
                sr.enabled = false;
                // Also turn off SpriteMasks if they exist
                var mask = sr.GetComponent<SpriteMask>();
                if (mask) mask.enabled = false;
            }
        }

        // === ACTIVATE PARTS BASED ON DICE ROLLS ===

        // --- CRACKS ---
        if (_shouldBeCracked && cracks.Count > 0)
        {
            // Pick ONE random crack variation
            SpriteRenderer chosenCrack = cracks[Random.Range(0, cracks.Count)];

            // Your logic: Cracks use SpriteMasks, not Renderers
            if (chosenCrack.GetComponent<SpriteMask>() != null)
            {
                chosenCrack.GetComponent<SpriteMask>().enabled = true;
                chosenCrack.enabled = false; // Hide the sprite itself, show the mask
            }
        }

        // --- BLOOD ---
        if (_shouldBeBloody && bloods.Count > 0)
        {
            // Pick ONE random blood splatter
            SpriteRenderer chosenBlood = bloods[Random.Range(0, bloods.Count)];
            chosenBlood.enabled = true;
        }

        // --- FROWN ---
        if (_shouldFrown && frowns.Count > 0)
        {
            SpriteRenderer chosenFrown = frowns[Random.Range(0, frowns.Count)];
            chosenFrown.enabled = true;
        }

        // --- SMILE ---
        if (_shouldSmile && smiles.Count > 0)
        {
            SpriteRenderer chosenSmile = smiles[Random.Range(0, smiles.Count)];
            chosenSmile.enabled = true;
        }

        // --- BARCODE ---
        if (_shouldHaveBarcode && barcodes.Count > 0)
        {
            SpriteRenderer normalBarcode = null;
            SpriteRenderer smudgeBarcode = null;

            // Find the specific parts
            foreach (var b in barcodes)
            {
                if (b.gameObject.name.Contains("Smudge")) smudgeBarcode = b;
                else normalBarcode = b;
            }

            if (_shouldBeSmudged && smudgeBarcode != null)
            {
                smudgeBarcode.enabled = true;
                if (normalBarcode) normalBarcode.enabled = false; // Hide normal if smudged
            }
            else
            {
                // Normal case
                if (normalBarcode) normalBarcode.enabled = true;
                if (smudgeBarcode) smudgeBarcode.enabled = false;
            }
        }

        // === APPLY DATA TO REFUGEE SCRIPT ===
        Refugee refugeeScript = parent.GetComponent<Refugee>();
        if (refugeeScript != null)
        {
            refugeeScript.isBloody = _shouldBeBloody;
            refugeeScript.isCracked = _shouldBeCracked;
            refugeeScript.isSad = _shouldFrown;
            refugeeScript.isHappy = _shouldSmile;
            refugeeScript.hasBarcode = _shouldHaveBarcode;
            refugeeScript.isSmudged = _shouldBeSmudged;

            // Generate ID
            refugeeScript.idNumber = GenerateRandomID();
            refugeeScript.isValidBarcode = !_isForgery; // Valid if NOT forgery

            Debug.Log($"Stats Applied: Blood:{_shouldBeBloody}, Crack:{_shouldBeCracked}, Fake:{_isForgery}");
        }

        // === CRITICAL FORCE MAIN MASK ON ===
        // Ensures the main face is visible
        if (inst_mask.GetComponent<SpriteRenderer>() != null)
        {
            inst_mask.GetComponent<SpriteRenderer>().enabled = true;
        }
        SpriteRenderer mainFace = inst_mask.GetComponent<SpriteRenderer>();
        if (mainFace != null)
        {
            mainFace.enabled = true;
        }

        return inst_mask;
    }

    void RollDice()
    {
        // === Perfect Mask Override ===
        if (Random.Range(0f, 100f) < perfectMaskChance)
        {
            _shouldBeBloody = false;
            _shouldBeCracked = false;
            _shouldBeSmudged = false;
            _shouldHaveBarcode = true;
            _shouldFrown = false;
            _shouldSmile = true;
            _isForgery = false;
            return;
        }

        // === Roll for Visuals ===
        _shouldBeBloody = Random.Range(0f, 100f) < bloodChance;
        _shouldBeCracked = Random.Range(0f, 100f) < crackChance;
        _shouldFrown = Random.Range(0f, 100f) < frownChance;
        _shouldSmile = Random.Range(0f, 100f) < smileChance;

        // --- Roll for Barcode Presence ---
        // (If chance is 5, roll > 5 means 95% success rate)
        _shouldHaveBarcode = Random.Range(0f, 100f) > missingBarcodeChance;

        if (_shouldHaveBarcode)
        {
            _shouldBeSmudged = Random.Range(0f, 100f) < smudgeChance;
            _isForgery = Random.Range(0f, 100f) < forgeryChance;
        }
        else
        {
            _shouldBeSmudged = false;
            _isForgery = false;
        }
    }

    string GenerateRandomID()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string part1 = "";
        string part2 = "";
        for (int i = 0; i < 3; i++) part1 += chars[Random.Range(0, chars.Length)];
        for (int i = 0; i < 3; i++) part2 += chars[Random.Range(0, chars.Length)];
        return $"{part1}-{part2}";
    }
}








    /*
     * OLD METHOD
    private void Start()
    {

        hasBarcode = false;
    //InstantiateRandomMask(mask_prefab,FindFirstObjectByType<Canvas>().gameObject, new Vector3(0,0,0));  // test call
    }
    public GameObject InstantiateRandomMask(GameObject prefab, GameObject parent, Vector3 position, float rarity)
    {
        // ========= RESET LOGIC =========== //
        hasBarcode = false;isNormal = false; isSad = false; isCracked = false; isSmudged = false; isBloody = false; // resets all the properties each time a new mask is instantiated



        // ========== INSTANTIATION LOGIC =========== //
        GameObject inst_mask = Instantiate(prefab, position, Quaternion.identity);           // Instantiates a new mask, modify later for position and wtv
        inst_mask.transform.SetParent(parent.transform);                                     // makes sure the mask is a child of the main canvas 



        // ========== RANDOMIZATION LOGIC =========== //
        SpriteRenderer[] spriteRenderers = inst_mask.GetComponentsInChildren<SpriteRenderer>(); // get all sprite renderer components in the prefab and puts them in a list

        for (int i = 0; i < spriteRenderers.Length; i++)                                     // this SHUFFLES the list of sprite renderers using Fisher-Yates algorithm
        {
            int randomIndex = Random.Range(i, spriteRenderers.Length);
            SpriteRenderer temp = spriteRenderers[i];
            spriteRenderers[i] = spriteRenderers[randomIndex];
            spriteRenderers[randomIndex] = temp;
        }


        foreach (SpriteRenderer spriteRenderer in spriteRenderers)                           // iterates through each sprite renderer in the list
        {
            spriteRenderer.enabled = Random.value > rarity;                                    // randomly enables or disables the sprite renderer



            // ========== CHECKS FOR TAGS =========== //
            if (spriteRenderer.CompareTag("Barcode") && hasBarcode == false)                 // checks if the sprite renderer is tagged as "Barcode" and if hasBarcode is false
            {
                hasBarcode = true;                                                           // sets hasBarcode to true, so that the next iterations don't sset any other barcodes visible
                spriteRenderer.enabled = true;                                               // disables the barcode sprite renderer

                if (spriteRenderer.gameObject.name.Contains("Smudge"))                      // if the barcode is smudged, set isSmudged to true
                {
                    isSmudged = true;
                }
                continue;                                                                    // skips to the next iteration of the loop

            }
            else if (spriteRenderer.CompareTag("Barcode") && hasBarcode == true)             // if a barcode is checked again but one has already been made visible;
            {
                spriteRenderer.enabled = false;                                              // disables the barcode sprite renderer since one is already visible
                continue;                                                                    // skips to the next iteration of the loop

            }
            else if (spriteRenderer.CompareTag("Crack") && !spriteRenderer.enabled)          // if the sprite renderer is tagged as "Crack" but is disabled, turn spritemask off 
            {
                spriteRenderer.GetComponent<SpriteMask>().enabled = false;
                continue;

            }
            else if (spriteRenderer.CompareTag("Crack") && spriteRenderer.enabled)           // if the sprite renderer is tagged as "Crack" and is enabled, turn spritemask on and main sprite off
            {
                spriteRenderer.GetComponent<SpriteMask>().enabled = true;
                spriteRenderer.enabled = false;
                isCracked = true;
                continue;
            }
            else if (spriteRenderer.CompareTag("Frown") && spriteRenderer.enabled)           // if the sprite renderer is tagged as "Frown" and is enabled, set isSad to true
            {
                isSad = true;
                continue;
            }
            else if (spriteRenderer.CompareTag("Blood") && spriteRenderer.enabled)           // if the sprite renderer is tagged as "Blood" and is enabled, set isBloody to true
            {
                isBloody = true;
                continue;
            }
            
        }
            inst_mask.GetComponent<SpriteRenderer>().enabled = true;                         // makes sure the main sprite (mask) is always on

        // ========== SET REFUGEE PROPERTIES =========== //
        // After the randomization is done, set the properties on the Refugee script accordingly
        // This way, the refugee script can know what kind of mask it has
        // ============================================= //

        // Get the Refugee script from the object
        Refugee refugeeScript = parent.GetComponentInChildren<Refugee>();

        if (refugeeScript != null)                                                        // if the refugee script is found 
        {
            // Take the boolean flags that were calculated during the randomisation loop.
            // And save them into the Refugee's "Stats" variables.
            refugeeScript.hasBarcode = hasBarcode;                                        // sets the refugee script's hasBarcode property to the local hasBarcode property
            refugeeScript.isCracked = isCracked;                                          // sets the refugee script's isCracked property to the local isCracked property
            refugeeScript.isSad = isSad;                                                  // sets the refugee script's isSad property to the local isSad property
            refugeeScript.isSmudged = isSmudged;                                          // sets the refugee script's isSmudged property to the local isSmudged property
            refugeeScript.isBloody = isBloody;

            Debug.Log($"Stats Set! Cracked: {isCracked}, Sad: {isSad}, Barcode: {hasBarcode}, Smudged: {isSmudged}, Bloody: {isBloody}");

            // Now the Refugee object "knows" what it is wearing :D
        }
        else
        {
            Debug.LogError("Could not find Refugee script on object!");
        }

            return inst_mask;
        }

    // ========== UTILITIES =========== //

    }
    */

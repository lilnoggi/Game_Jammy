using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    // ========== Sprite Randomizer Class =========== //

    public GameObject mask_prefab;
    //private GameObject canvas;



    [Header("Properties")]
    public bool hasBarcode;
    public bool isNormal;
    public bool isSad;
    public bool isCracked;
    public bool isSmudged;
    public bool isBloody;
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

            return inst_mask;
        }

        // ========== UTILITIES =========== //




    }

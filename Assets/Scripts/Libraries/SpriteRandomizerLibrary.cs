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

    */
    // ========== Sprite Randomizer Class =========== //

    public GameObject mask_prefab;
    //private GameObject canvas;



    [Header("Properties")]
    public bool hasBarcode;
    public bool isNormal;
    public bool isSad;
    private void Start()
    {

    
    //InstantiateRandomMask(mask_prefab,FindFirstObjectByType<Canvas>().gameObject, new Vector3(0,0,0));  // test call
    }
    public GameObject InstantiateRandomMask(GameObject prefab, GameObject parent, Vector3 position)
    {
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
            spriteRenderer.enabled = Random.value > 0.5f;                                    // randomly enables or disables the sprite renderer




            if (spriteRenderer.CompareTag("Barcode") && hasBarcode == false)                 // checks if the sprite renderer is tagged as "Barcode" and if hasBarcode is false
            {
                hasBarcode = true;                                                           // sets hasBarcode to true, so that the next iterations don't sset any other barcodes visible
                spriteRenderer.enabled = true;                                               // disables the barcode sprite renderer
                continue;                                                                    // skips to the next iteration of the loop

            }
            else if (spriteRenderer.CompareTag("Barcode") && hasBarcode == true)                // if a barcode is checked again but one has already been made visible;
            {
                spriteRenderer.enabled = false;                                              // disables the barcode sprite renderer since one is already visible
                continue;                                                                    // skips to the next iteration of the loop

            }
            else if (spriteRenderer.CompareTag("Crack") && !spriteRenderer.enabled)
            {
                spriteRenderer.GetComponent<SpriteMask>().enabled = false;
                continue;

            }
            else if (spriteRenderer.CompareTag("Crack") && spriteRenderer.enabled)
            {
                spriteRenderer.GetComponent<SpriteMask>().enabled = true;
                spriteRenderer.enabled = false;
                continue;
            }
        }
            inst_mask.GetComponent<SpriteRenderer>().enabled = true;                             // makes sure the main sprite (mask) is always on

            return inst_mask;
        }

        // ========== UTILITIES =========== //




    }

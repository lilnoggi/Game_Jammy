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

    public GameObject prefab_test; // finds the prefab in the scene to test the function

    private void Start()
    {
 

        RandomizeSprites(prefab_test); // calls the randomize function on the prefab and sets this object as the parent
    }
    public void RandomizeSprites(GameObject prefab)
    {
        SpriteRenderer[] spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(); // get all sprite renderer components in the prefab and puts them in a list
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)                           // iterates through each sprite renderer in the list
        {
            spriteRenderer.enabled = Random.value > 0.5f;                                    // randomly enables or disables the sprite renderer
        }

        prefab.GetComponent<SpriteRenderer>().enabled = true;                                // makes sure the main sprite (mask) is always on
    }
}

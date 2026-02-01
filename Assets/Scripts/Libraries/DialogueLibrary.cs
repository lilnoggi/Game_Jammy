using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.UIElements;



/*
        This class serves as a centralized repository for dialogue-related constants
        and configurations used throughout the game. It should work as a stand-alone, 
        drag into a Game Master Actor and call and bam it works.

        It's very basic, but it'll get the job done and I'll try comment it properly xd
        so you guys can alter it if need be.
    
        I'm making this so it saves memory, cause all the dialogue is gonna be made only 
        for as long as its needed, and then deleted :D

        If we get to making dialogue better, then I'll alter this, but for now im gonna
        keep it simple and aim to just make it one function lomao.

        It'll follow the structure of
        
        Panel
            Text

          very basic ik but it's good enough   
    */

// =========== Dialogue Library Class =========== //

public class DialogueLibrary : MonoBehaviour
{

    [Header("Configuration")]
    public GameObject dialoguePrefab;
    public Canvas ui_canvas;
    public DialogueData d_data;

    [Header("Settings")]
    public AudioClip typing_sound;
    public float typingSpeed = 0.07f;

    public IEnumerator CreateDialogue(DialogueData dia_data, float timer)
    {

        string nameContent = dia_data.GetRandomName();
        string bodyContent = dia_data.GetRandomWords();

        /*

        //=========== INSTANTIATION ===========//
        // Create Panel & Apply explorer components.
        GameObject d_Panel = new GameObject("DialoguePanel");               // creates the panel object
        RectTransform d_p_Rect = d_Panel.AddComponent<RectTransform>();     // gives it a transform
        d_Panel.AddComponent<SpriteRenderer>();                             // gives it the canvas renderer component idk what it does but every ui thing has it
        d_Panel.GetComponent<SpriteRenderer>().sortingOrder = 15;
        Image d_p_Image = d_Panel.AddComponent<Image>();                    // gives it the image, no actual image, but gives it a color32 value thingymabob
        d_Panel.layer = LayerMask.NameToLayer("UI");                        // sets the layer to UI
        d_Panel.transform.SetParent(ui_canvas.transform); // makes sure the panel's in the main canvas
        d_p_Rect.localScale = new Vector3(9.5f, 2.5f, 1f);                  // scales it down, the Z value is 1 since it's a 2D game
        d_p_Rect.sizeDelta = new Vector2(55, 65);                           // sets the size of the panel
        d_p_Rect.anchoredPosition = new Vector2(0f, -175f);                 // positions it at the bottom center of the screen
        d_p_Image.color = new Color32(0, 0, 0, 200);                        // Color32 is RGBA, the A is Alpha, which is opacity. 255 is opaque, 0 is invisible


        // Create Text & Apply explorer components to it
        GameObject d_Text = new GameObject("DialogueText");                 // creates the text object
        RectTransform d_t_Rect = d_Text.AddComponent<RectTransform>();      // gives it a transform
        d_Text.AddComponent<CanvasRenderer>();                              // gives it the canvas renderer component
        TextMeshProUGUI d_t_Text = d_Text.AddComponent<TextMeshProUGUI>();  // gives it the text component
        d_Text.layer = LayerMask.NameToLayer("UI");                         // sets the layer to UI
        d_Text.transform.SetParent(d_Panel.transform);                      // makes sure the text is a child of the panel
        d_Text.transform.localScale = new Vector2(.35f,.85f);               // scales it to normal size
        d_t_Rect.sizeDelta = new Vector2(135, 50);                          // sets the size of the text box
        d_t_Rect.anchoredPosition = new Vector2(2,-7);                      // positions it at the center of the panel
        //d_t_Text.text = dialogueText;                                     // sets the text to the dialogueText parameter -- DONT USE we have typewriter effect :)
        d_t_Text.fontSize = 5;                                              // sets the font size
        d_t_Text.alignment = TextAlignmentOptions.TopLeft;                  // centers the text
        d_t_Text.color = Color.white;                                       // sets the text color to white
        d_t_Text.font = fonte;                                               // Loads a TMP_FontAsset from Resources folder
        d_t_Text.textWrappingMode = TextWrappingModes.Normal;               // enables word wrapping

        // Create Text but its the name of the character
        GameObject n_Text = new GameObject("NameText");                     // creates the text object
        RectTransform n_t_Rect = n_Text.AddComponent<RectTransform>();      // gives it a transform
        n_Text.AddComponent<CanvasRenderer>();                              // gives it the canvas renderer component
        TextMeshProUGUI n_t_Text = n_Text.AddComponent<TextMeshProUGUI>();  // gives it the text component
        n_Text.layer = LayerMask.NameToLayer("UI");                         // sets the layer to UI
        n_Text.transform.SetParent(d_Panel.transform);                      // makes sure the text is a child of the panel
        n_Text.transform.localScale = new Vector2(.35f, .85f);              // scales it to normal size
        n_t_Rect.sizeDelta = new Vector2(150, 35);                          // sets the size of the text box
        n_t_Rect.anchoredPosition = new Vector2(0, 13);                     // positions it at the center of the panel
        n_t_Text.text = nameText;                                           // sets the text to the dialogueText parameter -- DONT USE we have typewriter effect :)
        n_t_Text.fontSize = 7;                                              // sets the font size
        n_t_Text.alignment = TextAlignmentOptions.TopLeft;                  // centers the text
        n_t_Text.color = Color.wheat;                      // sets the text color to a bit gray
        n_t_Text.font = fonte; // Loads a TMP_FontAsset from Resources folder
        n_t_Text.textWrappingMode = TextWrappingModes.Normal;               // enables word wrapping


        //Lerps the position of the panel from below the screen to its position
        d_Panel.transform.localPosition = new Vector3(0, -425, 0);          // sets the initial position below the screen
        float elapsedTime = 0f;                                             // elapsed time counter
        float lerpDuration = 1f;                                            // duration of the lerp in seconds
        Vector3 startPos = d_Panel.transform.localPosition;                 // starting position
        Vector3 endPos = new Vector3(0, -175f, 0);                          // target position
        while (elapsedTime < lerpDuration)                                  // starts a while loop for the thing bro just read it 
        {
            d_Panel.transform.localPosition = Vector3.Lerp(startPos, endPos, (elapsedTime / lerpDuration)); // lerps the position
            elapsedTime += Time.deltaTime;                                  // increments the elapsed time with time since last frame (deltatime)
            yield return null;                                              // waits for the next frame (needed cause its a coroutine)
        }
        d_Panel.transform.localPosition = endPos;                           // ensure it ends exactly at the target position


        // =========== TYPEWRITER EFFECT =========== //

        foreach (char letter in dialogueText.ToCharArray())                 // for each character in the dialogueText string
        {
            d_t_Text.text += letter;                                        // adds the character to the text component
            AudioSource.PlayClipAtPoint(typing_sound, Camera.main.transform.position, 0.05f); // plays the typing sound at the camera's position
            yield return new WaitForSeconds(0.07f);                         // just waits .07 seconds before adding the next character
        }
        
        
        yield return new WaitForSeconds(timer);                             // waits for the timer duration
        Destroy(d_Panel);   
        */// destroys the panel and text child object


        // =========== INSTANTIATION VIA PREFAB ===========//
        // Due to canvas sizing issues, we will use a prefab
        // for the dialogue box instead of creating it via code.
        // 1. Spawn inside the UI Canvas so it scales correctly
        GameObject d_Instance = Instantiate(dialoguePrefab, ui_canvas.transform);

        // 2. Get references to the panel and text components
        DialoguePanel panelScript = d_Instance.GetComponent<DialoguePanel>();
        RectTransform panelRect = d_Instance.GetComponent<RectTransform>();

        if (panelScript == null)
        {
            Debug.LogError("DialoguePanel script not found on the dialogue prefab.");
            yield break;
        }

        // 3. SETUP TEXT
        // Set name immediately
        panelScript.nameText.text = nameContent;
        // Clear body text for typewriter effect
        panelScript.bodyText.text = "";

        // 4. ANIMATION: SLIDE UP
        // FLORINS ANIMATION CODE
        Vector3 finalPos = panelRect.anchoredPosition; // Placed in editor
        Vector3 startPos = finalPos + new Vector3(0, -300f, 0); // Start 300 pixels lower

        panelRect.anchoredPosition = startPos;

        float elapsedTime = 0f;
        float lerpDuration = 0.5f; // Faster lerp for better feel

        while (elapsedTime < lerpDuration)                                  // starts a while loop for the thing bro just read it 
        {
            panelRect.anchoredPosition = Vector3.Lerp(startPos, finalPos, (elapsedTime / lerpDuration)); // lerps the position
            elapsedTime += Time.deltaTime;                                  // increments the elapsed time with time since last frame (deltatime)
            yield return null;                                              // waits for the next frame (needed cause its a coroutine)
        }
        panelRect.anchoredPosition = finalPos;                           // ensure it ends exactly at the target position

        // 5. TYPEWRITER EFFECT
        foreach (char letter in bodyContent.ToCharArray())                 // for each character in the dialogueText string
        {
            panelScript.bodyText.text += letter;                                        // adds the character to the text component

            AudioSource.PlayClipAtPoint(typing_sound, Camera.main.transform.position, 0.05f); // plays the typing sound at the camera's position
            
            yield return new WaitForSeconds(typingSpeed);                         // just waits .07 seconds before adding the next character
        }

        // 6. WAITING PERIOD
        yield return new WaitForSeconds(timer);                             // waits for the timer duration
        Destroy(d_Instance);
    }
}

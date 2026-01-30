using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;  -- i had issues with this fsr

// ============================================== //
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

    private void Start()
    {
        StartCoroutine(CreateDialogue("hello i am dialogue box", 5));
    }
    public IEnumerator CreateDialogue(string dialogueText, float timer)
    {
        //=========== INSTANTIATION ===========//
        // Create Panel & Apply explorer components.
        GameObject d_Panel = new GameObject("DialoguePanel");               // creates the panel object
        RectTransform d_p_Rect = d_Panel.AddComponent<RectTransform>();     // gives it a transform
        d_Panel.AddComponent<CanvasRenderer>();                             // gives it the canvas renderer component idk what it does but every ui thing has it
        Image d_p_Image = d_Panel.AddComponent<Image>();                    // gives it the image, no actual image, but gives it a color32 value thingymabob
        d_Panel.layer = LayerMask.NameToLayer("UI");                        // sets the layer to UI
        d_Panel.transform.SetParent(FindFirstObjectByType<Canvas>().transform); // makes sure the panel's in the main canvas
        d_p_Rect.localScale = new Vector3(9.5f, 2.5f, 1f);                  // scales it down, the Z value is 1 since it's a 2D game
        d_p_Rect.anchoredPosition = new Vector2(0f, -350f);                 // positions it at the bottom center of the screen
        d_p_Image.color = new Color32(0, 0, 0, 200);                        // Color32 is RGBA, the A is Alpha, which is opacity. 255 is opaque, 0 is invisible

        // Create Text & Apply explorer components to it
        GameObject d_Text = new GameObject("DialogueText");                 // creates the text object
        RectTransform d_t_Rect = d_Text.AddComponent<RectTransform>();      // gives it a transform
        d_Text.AddComponent<CanvasRenderer>();                              // gives it the canvas renderer component
        Text d_t_Text = d_Text.AddComponent<Text>();                        // gives it the text component
        d_Text.layer = LayerMask.NameToLayer("UI");                         // sets the layer to UI
        d_Text.transform.SetParent(d_Panel.transform);                      // makes sure the text is a child of the panel
        d_t_Rect.localScale = new Vector3(1f, 1f, 1f);                      // scales it to normal size
        d_t_Rect.anchoredPosition = new Vector2(0f, 0f);                    // positions it at the center of the panel
        d_t_Text.text = dialogueText;                                       // sets the text to the dialogueText parameter
        d_t_Text.fontSize = 24;                                             // sets the font size
        d_t_Text.alignment = TextAnchor.MiddleCenter;                          // centers the text
        d_t_Text.color = Color.white;                                       // sets the text color to white
        d_t_Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // sets the font to Arial until we get a new one
        d_t_Text.horizontalOverflow = HorizontalWrapMode.Wrap;              // makes the text wrap to the next line if it's too long
        d_t_Text.verticalOverflow = VerticalWrapMode.Truncate;              // truncates the text if it's too long vertically

        yield return new WaitForSeconds(timer);                             // waits for the timer duration
        Destroy(d_Panel); // destroys the panel and text child object

    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLibrary : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject dialoguePrefab;
    public Canvas ui_canvas;

    [Header("Settings")]
    public AudioClip typing_sound;
    public float typingSpeed = 0.05f; // Made slightly faster default

    // We changed the signature slightly to remove 'timer' because we now use clicking to advance
    public IEnumerator CreateDialogue(DialogueData dia_data, bool isNPC, string speakerName = "")
    {
        // 1. Instantiate Prefab
        GameObject d_Instance = Instantiate(dialoguePrefab, ui_canvas.transform);
        DialoguePanel panelScript = d_Instance.GetComponent<DialoguePanel>();
        RectTransform panelRect = d_Instance.GetComponent<RectTransform>();

        // 2. Setup Name (Uses the first name in the list, or "Management")
        // If your data has multiple names, you can change this to loop too.
        if (!isNPC)
        {
            // If not NPC, force the name to MANAGEMENT
            panelScript.nameText.text = "MANAGEMENT";
        }
        else
        {
            // Is an NPC.
            if (string.IsNullOrEmpty(speakerName))
                panelScript.nameText.text = "Unknown";
            else
                panelScript.nameText.text = speakerName;
        }

        panelScript.bodyText.text = "";

        // 3. Animation: Slide Up
        Vector3 finalPos = panelRect.anchoredPosition;
        Vector3 startPos = finalPos + new Vector3(0, -300f, 0);
        panelRect.anchoredPosition = startPos;

        float animTime = 0f;
        while (animTime < 0.5f)
        {
            panelRect.anchoredPosition = Vector3.Lerp(startPos, finalPos, animTime / 0.5f);
            animTime += Time.deltaTime;
            yield return null;
        }
        panelRect.anchoredPosition = finalPos;

        // ==========================================================
        // DETERMINE WHAT TO PLAY
        // ==========================================================
        string[] sentencesToPlay;

        if (isNPC)
        {
            // If NPC: Pick ONE random line from the list
            string randomLine = dia_data.c_words[Random.Range(0, dia_data.c_words.Length)];
            sentencesToPlay = new string[] { randomLine };
        }
        else
        {
            // IF MANAGEMENT: Play ALL lines in sequence
            sentencesToPlay = dia_data.c_words;
        }

        // ==========================================================
        // MAIN DIALOGUE LOOP (Iterates through ALL sentences)
        // ==========================================================

        // Loop through every sentence in the c_words array
        foreach (string sentence in sentencesToPlay)
            {
                panelScript.bodyText.text = ""; // Clear previous sentence
                bool hasSkipped = false;

                // --- TYPEWRITER LOOP ---
                foreach (char letter in sentence.ToCharArray())
                {
                    // CHECK FOR SKIP INPUT (Left Mouse Click)
                    if (Input.GetMouseButtonDown(0))
                    {
                        panelScript.bodyText.text = sentence; // Show full text instantly
                        hasSkipped = true;
                        break; // Break out of the 'foreach char' loop
                    }

                    panelScript.bodyText.text += letter;

                    // Audio
                    if (typing_sound != null)
                        AudioSource.PlayClipAtPoint(typing_sound, Camera.main.transform.position, 0.05f);

                    float waitTimer = 0f;
                    while (waitTimer < typingSpeed)
                    {
                        waitTimer += Time.deltaTime;

                        // CHECK FOR SKIP INPUT (Left Mouse Click)
                        if (Input.GetMouseButtonDown(0))
                        {
                            panelScript.bodyText.text = sentence; // Show full text instantly
                            hasSkipped = true;
                            break;
                        }
                        yield return null;
                    }
                    if (hasSkipped) break; // Break out of the 'foreach char' loop
                }

                // If we skipped, we need a tiny delay so the same click doesn't skip the NEXT sentence too
                if (hasSkipped) yield return null;

                // --- WAIT FOR PLAYER TO CONTINUE ---
                // Wait until the player clicks again to move to the next sentence
                // (Or if a timer was provided, we could implement that, but clicking is better)
                while (!Input.GetMouseButtonDown(0))
                {
                    yield return null;
                }

                // Frame buffer to prevent double-clicking
                yield return null;
            }

        // ==========================================================
        // CLOSING ANIMATION
        // ==========================================================

        // Slide down before destroying
        animTime = 0f;
        while (animTime < 0.3f)
        {
            panelRect.anchoredPosition = Vector3.Lerp(finalPos, startPos, animTime / 0.3f);
            animTime += Time.deltaTime;
            yield return null;
        }

        Destroy(d_Instance);
    }
}
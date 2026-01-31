using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip click_sound;
    public AudioClip menu_ambiance;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(menu_ambiance, Camera.main.transform.position, 0.05f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            PlayClickSound();                                       // plays click sound on mouse click
        }
    }


    // ========== OTHER FUNCTIONS ========== //
    public void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(click_sound, Camera.main.transform.position, 0.05f);
    }


    // ========== BUTTON FUNCTIONS ========== //
    public void StartButton()
    {
                Debug.Log("Start Button Pressed");              // js debug
                SceneManager.LoadScene("Game_Demo");                // loads scene
    }
    public void OptionsButton()
    {                   
                Debug.Log("Options Button Pressed");            // js debug
                SceneManager.LoadScene("mm_Options");                // loads scene
    }
    public void CreditsButton()
    {
                Debug.Log("Credits Button Pressed");            // js debug   
                SceneManager.LoadScene("mm_Credits");                // loads scene
    }
    public void BackButton() 
    {           
                Debug.Log("Back Button Pressed");               // js debug
                SceneManager.LoadScene("mm_Main");             // loads scene
    }
    public void QuitButton()
    {
                Debug.Log("Quit Button Pressed");               // js debug
                Application.Quit();                                 // cmon now what do you think this does?
    }




}

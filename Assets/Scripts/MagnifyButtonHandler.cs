using UnityEngine;

public class MagnifyButtonHandler : MonoBehaviour
{
    public ZoomController zoomController;
    public NPCManager npcManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip zoom;

    public void OnMagnifyPressed()
    {

        if (zoomController == null || npcManager == null) return;
        if (npcManager.currentRefugee == null) return;

        audioSource.PlayOneShot(zoom);
        zoomController.ToggleZoom(npcManager.currentRefugee.maskTransform);
    }
}

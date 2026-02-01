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
        if (!MoneyManager.hasMagnifier)
        {
            Debug.Log("You don't own the Magnifier yet!");
            return;
        }

        if (zoomController == null || npcManager == null) return;
        if (npcManager.currentRefugee == null) return;

        audioSource.PlayOneShot(zoom);
        zoomController.ToggleZoom(npcManager.currentRefugee.maskTransform);
    }
}

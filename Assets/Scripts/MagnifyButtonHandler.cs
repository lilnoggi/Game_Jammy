using UnityEngine;

public class MagnifyButtonHandler : MonoBehaviour
{
    public ZoomController zoomController;
    public NPCManager npcManager;

    public void OnMagnifyPressed()
    {
        if (!MoneyManager.hasMagnifier)
        {
            Debug.Log("You don't own the Magnifier yet!");
            return;
        }

        if (zoomController == null || npcManager == null) return;
        if (npcManager.currentRefugee == null) return;

        zoomController.ToggleZoom(npcManager.currentRefugee.maskTransform);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ZoomController : MonoBehaviour
{
    [Header("Camera Zoom")]
    public float normalOrthoSize = 5f;     // default camera size
    public Vector2 zoomOffset = new Vector2(15f, 0.5f);
    public float zoomOrthoSize = 2.5f;     // zoomed-in camera size
    public float moveSmooth = 10f;         // camera follow smoothness
    public float zoomSmooth = 10f;         // zoom smoothness

    [Header("Exit Zoom Input ")]
    public bool allowExitWithEscOrRightClick = true;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("UI Lock ")]
    public Button approveButton;
    public Button rejectButton;

    private Camera cam;
    private bool isZoomed = false;

    private Vector3 normalCamPos;
    private Transform zoomTarget;

    void Awake()
    {
        cam = GetComponent<Camera>();
        normalCamPos = transform.position;

        // Ensure we start in normal state
        cam.orthographicSize = normalOrthoSize;
    }

    void Update()
    {
        // exit input
        if (allowExitWithEscOrRightClick && isZoomed)
        {
            if (Input.GetKeyDown(exitKey) || Input.GetMouseButtonDown(1))
            {
                ExitZoom();
            }
        }

        // Smooth zoom size
        float targetSize = isZoomed ? zoomOrthoSize : normalOrthoSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSmooth);

        // Smooth camera position
        Vector3 targetPos = normalCamPos;
        if (isZoomed && zoomTarget != null)
        {
            targetPos = new Vector3(zoomTarget.position.x + zoomOffset.x, zoomTarget.position.y + zoomOffset.y, normalCamPos.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSmooth);

        // lock UI buttons while zoomed
        if (approveButton != null) approveButton.interactable = !isZoomed;
        if (rejectButton != null) rejectButton.interactable = !isZoomed;
    }


    /// Zoom to a target transform (the NPC mask).

    public void ZoomTo(Transform target)
    {
        if (target == null) return;
        zoomTarget = target;
        isZoomed = true;
    }

    /// Toggle zoom: if zoomed, exits; if not zoomed, zooms to target.

    public void ToggleZoom(Transform target)
    {
        if (isZoomed)
        {
            ExitZoom();
        }
        else
        {
            ZoomTo(target);
        }
    }

    
    /// Exit zoom back to normal camera state.
    public void ExitZoom()
    {
        isZoomed = false;
        zoomTarget = null;
    }

    public bool IsZoomed()
    {
        return isZoomed;
    }
}

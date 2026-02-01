using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScannerTool : MonoBehaviour
{
    [Header("Scanner State")]
    private bool isActive = false;

    [Header("UI References")]
    public Image scannerCursorImage; // The one that follows the mouse
    public TextMeshProUGUI barcodeText;// The actual text component

    [Header("Sprites")]
    public Sprite scannerOffSprite; // The grey back
    public Sprite scannerOnSprite; // The red laser one

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scanBeepSFX;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        // Hide the cursor tool and text by default
        scannerCursorImage.gameObject.SetActive(false);

        if (barcodeText != null) barcodeText.text = "SCANNER READY";
    }

    void Update()
    {
        // Toggle Tool ON/OFF with Right Click (to drop it)
        if (isActive && Input.GetMouseButtonDown(1))
        {
            DropScanner();
        }

        if (isActive)
        {
            // Make the scanner image follow the mouse
            MoveScannerWithMouse();

            // Check what is being hovered over
            CheckForMask();
        }
    }

    // Called by the Desk Button
    public void PickupScanner()
    {
        isActive = true;
        scannerCursorImage.gameObject.SetActive(true);
        Cursor.visible = false;
    }

    public void DropScanner()
    {
        isActive = false;
        scannerCursorImage.gameObject.SetActive(false);
        Cursor.visible = true;

        if (barcodeText != null) barcodeText.text = "";
    }

    void MoveScannerWithMouse()
    {
        // Move the UI Image to the mouse pos
        scannerCursorImage.transform.position = Input.mousePosition;
    }

    void CheckForMask()
    {
        // Raycast from mouse pos into the world
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // HIT SOMETHING
            Refugee refugee = hit.collider.GetComponentInParent<Refugee>();

            if (refugee != null)
            {
                // YES: Turn Scanner ON (red light)
                scannerCursorImage.sprite = scannerOnSprite;

                //audioSource.PlayOneShot(scanBeepSFX);

                // Show the barcode
                ShowBarcode(refugee);
            }
            else
            {
                // NO: Turn Scanner OFF (Grey)
                scannerCursorImage.sprite = scannerOffSprite;
                barcodeText.text = "SCANNING...";
                barcodeText.color = Color.white;
            }
        }
        else
        {
            scannerCursorImage.sprite = scannerOffSprite;
            barcodeText.text = "SCANNING...";
            barcodeText.color = Color.white;
        }
    }

    void ShowBarcode(Refugee refScript)
    {
        // Check if barcode exists
        if (!refScript.hasBarcode)
        {
            barcodeText.text = "NO TAG FOUND";
            barcodeText.color = Color.red;
            return;
        }

        // Check if Smudged
        if (refScript.isSmudged)
        {
            barcodeText.text = "ERROR: SMUDGED";
            barcodeText.color = Color.red;
            return;
        }

        // Check Validity
        if (refScript.isValidBarcode)
        {
            // VALID -> GREEN
            barcodeText.text = "ID: " + refScript.idNumber;
            barcodeText.color = Color.green;
        }
        else
        {
            // INVALID -> RED
            barcodeText.text = "ID: " + refScript.idNumber;
            barcodeText.color = Color.red;

        }
    }
}

using UnityEngine;

public class Refugee : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Mask")]
    public SpriteRenderer maskRenderer;
    public MaskData currentMask;
    public Transform maskTransform;

    // ========== GENERATED STATS =========== //
    // These variables act as an "inventory" for the Refugee.
    // The SpriteRandomiser fills these out, and the NPCManager reads them later
    // to decide if the player made the right chocie.
    [Header("Generated Stats")]
    public bool isCracked;
    public bool isSad;
    public bool hasBarcode;
    public bool isSmudged;
    public bool isBloody;
    public string idNumber; // For scanner

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // Generate a random ID formatted like "ID: 123-45-A"
        idNumber = $"ID: {Random.Range(100, 999)}-{Random.Range(10, 99)}-{(char)Random.Range('A', 'Z')}";
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    public void SetMask(MaskData mask)
    {
        currentMask = mask;
        maskRenderer.sprite = mask.sprite;
    }
}

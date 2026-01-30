using UnityEngine;

public class Refugee : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Mask")]
    public SpriteRenderer maskRenderer;
    public MaskData currentMask;

    private Vector3 targetPosition;
    private bool isMoving = false;

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

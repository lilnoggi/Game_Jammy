using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.15f;
    public float dampingSpeed = 20f;

    private Vector3 originalPos;
    private float currentDuration;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (currentDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeStrength;
            currentDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentDuration = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalPos,
                Time.deltaTime * dampingSpeed
            );
        }
    }

    public void Shake(float duration, float strength)
    {
        currentDuration = duration;
        shakeStrength = strength;
    }
}

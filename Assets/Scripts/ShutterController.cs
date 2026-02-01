using System.Collections;
using UnityEngine;

public class ShutterController : MonoBehaviour
{
    [Header("References")]
    public Transform movingShutterPart; // BlindBottomBit

    [Header("Positions")]
    public Vector3 closedPosition;
    public Vector3 openPosition;

    [Header("Settings")]
    public float animationDuration = 2.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip motorSound;
    public AudioClip slamSound;

    [ContextMenu("Set Current Pos as Closed")]
    void SetClosed() { closedPosition = movingShutterPart.localPosition; }

    [ContextMenu("Set Current Pos as Open")]
    void SetOpen() { openPosition = movingShutterPart.localPosition; }

    public void OpenShutters()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(openPosition));
        if (audioSource && motorSound) audioSource.PlayOneShot(motorSound);
    }

    public void CloseShutters()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(closedPosition));
        if (audioSource && slamSound) audioSource.PlayOneShot(slamSound);
    }

    IEnumerator MoveRoutine(Vector3 targetPos)
    {
        Vector3 startPos = movingShutterPart.localPosition;
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            t = t * t * (3f - 2f * t);

            movingShutterPart.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        movingShutterPart.localPosition = targetPos;
    }
}

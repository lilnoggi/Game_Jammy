using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float workDayDuration = 180f; // 3 minutes
    private float elapsedTime = 0f;

    private int startHour = 7;
    private int endHour = 16;

    [Header("UI")]
    public TextMeshProUGUI clockText;
    public Image fadeImage;

    [Header("Day Info")]
    public int currentDay = 1;

    private bool dayActive = true;

    void Update()
    {
        if (!dayActive) return;

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / workDayDuration);
        float currentHourFloat = Mathf.Lerp(startHour, endHour, t);

        UpdateClock(currentHourFloat);

        if (elapsedTime >= workDayDuration)
        {
            StartCoroutine(EndDayRoutine());
        }
    }

    void UpdateClock(float hourFloat)
    {
        int hours = Mathf.FloorToInt(hourFloat);
        int minutes = Mathf.FloorToInt((hourFloat - hours) * 60);

        string amPm = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        clockText.text = $"{displayHour:00}:{minutes:00} {amPm}";
    }

    IEnumerator EndDayRoutine()
    {
        dayActive = false;

        // Snap clock
        clockText.text = "4:00 PM";

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, 2.5f));

        // Small pause in darkness
        yield return new WaitForSeconds(1f);

        // Next day
        currentDay++;
        StartNewDay();

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f, 2.5f));

        dayActive = true;
    }

    void StartNewDay()
    {
        elapsedTime = 0f;
        clockText.text = "07:00 AM";

        Debug.Log("Starting Day " + currentDay);

        // Later:
        // Update rules
        // Reset quota
        // Spawn first refugee
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}

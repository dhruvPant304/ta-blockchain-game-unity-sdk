using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {
    public TextMeshProUGUI timerText; // Assign the TextMeshPro object in the inspector
    public int remainingSeconds;

    private Coroutine countdownCoroutine;

    void Start() {
        StartCountdown();
    }

    void StartCountdown() {
        if (countdownCoroutine != null) {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    public void StartTimer(int time){
        remainingSeconds = time;
        StartCountdown();
    }

    private IEnumerator CountdownCoroutine() {
        while (remainingSeconds > 0) {
            timerText.text = FormatTime(remainingSeconds);
            yield return new WaitForSeconds(1);
            remainingSeconds--;
        }

        StartTimer(86400);
    }

    private string FormatTime(int seconds) {
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int secs = seconds % 60;
        return $"{hours:D2}:{minutes:D2}:{secs:D2}";
    }
}

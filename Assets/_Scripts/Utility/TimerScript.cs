using System;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour {
    [NonSerialized] public bool IsTimerRunning = false;
    private float _elapsedTime = 0f;
    private TextMeshProUGUI _timerText;
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start() {
        GameObject textObject = GameObject.FindGameObjectWithTag("TimerText");
        _timerText = textObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if (_timerText != null) {
            if (IsTimerRunning) {
                _elapsedTime += Time.deltaTime;
            }
            _timerText.text = _elapsedTime.ToString("F2") + "s";
        }
    }

    public void StartTimer() {
        IsTimerRunning = true;
    }
}

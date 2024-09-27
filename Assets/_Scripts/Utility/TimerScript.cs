using System;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour {
    [NonSerialized] public bool IsTimerRunning = false;
    private float _elapsedTime = 0f;
    private TMP_Text _timerText;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private GameObject _timerPrefab;

    private void Start() {
        _timerText = Instantiate(_timerPrefab).GetComponentInChildren<TMP_Text>();
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

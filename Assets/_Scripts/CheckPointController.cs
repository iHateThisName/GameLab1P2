using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    [SerializeField] private bool _isStartPosition = false;
    void Start() {
        if (_isStartPosition) {
            GameManager.Instance.SetLevelStartPosition(new Vector2(transform.position.x, transform.position.y));
        }
    }
}

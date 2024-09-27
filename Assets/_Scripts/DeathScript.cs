using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !GameManager.Instance.IsPlayerDead)
        {
            GameManager.Instance.Death(other.gameObject);
        }
    }

}

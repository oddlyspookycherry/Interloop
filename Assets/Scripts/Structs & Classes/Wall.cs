﻿using UnityEngine;

public class Wall : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.GameEnd(GameOverType.Hit);
        }
    }
}
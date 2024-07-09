using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private UIManager uiManager;
    
    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>(); // Находим менеджера UI
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiManager.LevelComplete(true);
        }
    }
}

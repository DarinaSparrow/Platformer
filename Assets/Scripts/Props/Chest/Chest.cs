using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] private int requiredKeyID; // Уникальный идентификатор требуемого ключа
    [SerializeField] private int containsHealthValue; // Количество хранимого значения здоровья
    [SerializeField] private int containsCoinValue; // Количество хранимого значения монет
    [SerializeField] private Animator chestAnimator; // Аниматор для сундука

    private bool isPlayerNearby = false;
    private PlayerInventory playerInventory;
    private Health playerHealth;
    private CoinBalance playerBalance;

    private void Update() {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) {
            if (playerInventory != null && playerInventory.HasKey(requiredKeyID)) {
                OpenChest(); // Открыть сундук
                playerInventory.UseKey(requiredKeyID); // Использовать ключ
                playerHealth.AddHealth(containsHealthValue);
                playerBalance.AddCoin(containsCoinValue);
            }
        }
    }

    private void OpenChest() {
        chestAnimator.SetTrigger("Open"); // Запустить анимацию открытия сундука
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            isPlayerNearby = true;
            playerInventory = collision.GetComponent<PlayerInventory>();
            playerHealth = collision.GetComponent<Health>();
            playerBalance = collision.GetComponent<CoinBalance>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            isPlayerNearby = false;
            playerInventory = null;
            playerHealth = null;
            playerBalance = null;
        }
    }
}
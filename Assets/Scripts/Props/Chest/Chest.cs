using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] private int requiredKeyID; // ���������� ������������� ���������� �����
    [SerializeField] private int containsHealthValue; // ���������� ��������� �������� ��������
    [SerializeField] private int containsCoinValue; // ���������� ��������� �������� �����
    [SerializeField] private Animator chestAnimator; // �������� ��� �������

    private bool isPlayerNearby = false;
    private PlayerInventory playerInventory;
    private Health playerHealth;
    private CoinBalance playerBalance;

    private void Update() {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) {
            if (playerInventory != null && playerInventory.HasKey(requiredKeyID)) {
                OpenChest(); // ������� ������
                playerInventory.UseKey(requiredKeyID); // ������������ ����
                playerHealth.AddHealth(containsHealthValue);
                playerBalance.AddCoin(containsCoinValue);
            }
        }
    }

    private void OpenChest() {
        chestAnimator.SetTrigger("Open"); // ��������� �������� �������� �������
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
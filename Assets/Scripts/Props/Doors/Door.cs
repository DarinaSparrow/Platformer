using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField] private Transform[] destinations;
    [SerializeField] private int randomSeed = 0; // ��� ��������� ��������� �����
    [SerializeField] private int delayFramesBeforeTeleport = 5; // �������� ����� ������������� � ������
    [SerializeField] private int delayFramesAfterTeleport = 5; // �������� ����� ������������ � ������

    private bool isPlayerNearby = false;
    private bool isMoving = false;
    private Transform playerTransform;
    private System.Random random;

    private void Awake() {
        random = new System.Random(randomSeed); // ������������� ���������� ��������� �����
    }

    private void Update() {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) {
            if (playerTransform != null && destinations.Length > 0) {
                StartCoroutine(TeleportWithDelay());
            }
        }
    }

    private IEnumerator TeleportWithDelay() {
        isMoving = true;
        // ������ ���������
        playerTransform.GetComponent<SpriteRenderer>().enabled = false;

        // �������� ����� �������������
        for (int i = 0; i < delayFramesBeforeTeleport; i++) {
            yield return null;
        }

        // ������� ������ ����� �������, ������� ���������� �� �������
        Vector3 currentPosition = transform.position;
        int randomIndex;
        do {
            randomIndex = random.Next(destinations.Length);
        } while (destinations.Length > 1 && destinations[randomIndex].position == currentPosition);

        // ��������������� ���������
        playerTransform.position = destinations[randomIndex].position;

        // �������� ����� ������������
        for (int i = 0; i < delayFramesAfterTeleport; i++) {
            yield return null;
        }

        // �������� ���������
        playerTransform.GetComponent<SpriteRenderer>().enabled = true;
        isMoving = false;
        isPlayerNearby = false;
        playerTransform = null;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            isPlayerNearby = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !isMoving) {
            isPlayerNearby = false;
            playerTransform = null;
        }
    }
}
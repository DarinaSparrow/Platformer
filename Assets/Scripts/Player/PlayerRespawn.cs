using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour {
    [SerializeField] private AudioClip checkpoint; // ���� ����������� �����
    [SerializeField] private int numOfLifes; // ���������� ������
    [SerializeField] private Image[] lifesImage;
    [SerializeField] private Transform spawnpoint;

    public int Lifes { get; set; }

    private Transform currentCheckpoint; // ������� ����������� �����
    private Health playerHealth; // �������� ������
    private UIManager uiManager; // �������� UI

    private void Awake() {
        playerHealth = GetComponent<Health>(); // �������� ��������� ��������
        uiManager = FindObjectOfType<UIManager>(); // ������� ��������� UI
        Lifes = numOfLifes;
        currentCheckpoint = spawnpoint;
        for (int i = 0; i < numOfLifes; i++) {
            lifesImage[i].enabled = true;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            Respawn();
    }

    public void RespawnCheck() {
        if (Lifes > 0) 
            lifesImage[Lifes - 1].enabled = false;
        Lifes--;

        if (currentCheckpoint == null || Lifes == 0) { // ���� ����������� ����� �� �����������
            uiManager.GameOver(); // ���������� ����� ��������� ����
            return;
        }

        playerHealth.Respawn(); // ��������������� �������� ������ � ���������� ��������
        transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z); ; // ���������� ������ � ����������� �����
    }

    public void Respawn() {
        if (currentCheckpoint == null || Lifes == 0) { // ���� ����������� ����� �� �����������
            uiManager.GameOver(); // ���������� ����� ��������� ����
            return;
        }

        playerHealth.Respawn(); // ��������������� �������� ������ � ���������� ��������
        transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z); ; // ���������� ������ � ����������� �����
    }


    public void OnTriggerEnter2D_Respawn(Collider2D collision) {
        if (collision.gameObject.tag == "Checkpoint") { // ���� ������ ����� ��� "Checkpoint"
            currentCheckpoint = collision.transform; // ������������� ������� ����������� �����
            SoundManager.instance.PlaySound(checkpoint); // ������������� ���� ����������� �����
            collision.GetComponent<Collider2D>().enabled = false; // ��������� ��������� ����������� �����
            collision.GetComponent<Animator>().SetTrigger("activate"); // ��������� �������� ����������� �����
        }
    }

    public void OnDrawGizmos_Respawn() {
        if (!currentCheckpoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z), 0.1f);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
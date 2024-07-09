using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawn : MonoBehaviour {
    [SerializeField] private AudioClip checkpoint; // Звук контрольной точки
    [SerializeField] private int numOfLifes; // Количество жизней
    [SerializeField] private Image[] lifesImage;
    [SerializeField] private Transform spawnpoint;

    public int Lifes { get; set; }

    private Transform currentCheckpoint; // Текущая контрольная точка
    private Health playerHealth; // Здоровье игрока
    private UIManager uiManager; // Менеджер UI

    private void Awake() {
        playerHealth = GetComponent<Health>(); // Получаем компонент здоровья
        uiManager = FindObjectOfType<UIManager>(); // Находим менеджера UI
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

        if (currentCheckpoint == null || Lifes == 0) { // Если контрольная точка не установлена
            uiManager.GameOver(); // Показываем экран окончания игры
            return;
        }

        playerHealth.Respawn(); // Восстанавливаем здоровье игрока и сбрасываем анимацию
        transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z); ; // Перемещаем игрока к контрольной точке
    }

    public void Respawn() {
        if (currentCheckpoint == null || Lifes == 0) { // Если контрольная точка не установлена
            uiManager.GameOver(); // Показываем экран окончания игры
            return;
        }

        playerHealth.Respawn(); // Восстанавливаем здоровье игрока и сбрасываем анимацию
        transform.position = new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z); ; // Перемещаем игрока к контрольной точке
    }


    public void OnTriggerEnter2D_Respawn(Collider2D collision) {
        if (collision.gameObject.tag == "Checkpoint") { // Если объект имеет тег "Checkpoint"
            currentCheckpoint = collision.transform; // Устанавливаем текущую контрольную точку
            SoundManager.instance.PlaySound(checkpoint); // Воспроизводим звук контрольной точки
            collision.GetComponent<Collider2D>().enabled = false; // Отключаем коллайдер контрольной точки
            collision.GetComponent<Animator>().SetTrigger("activate"); // Активация анимации контрольной точки
        }
    }

    public void OnDrawGizmos_Respawn() {
        if (!currentCheckpoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(currentCheckpoint.position.x, currentCheckpoint.position.y - transform.localScale.y + currentCheckpoint.transform.localScale.y, currentCheckpoint.position.z), 0.1f);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
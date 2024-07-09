using UnityEngine;

public class CameraController : MonoBehaviour {
    // Камера комнаты
    [SerializeField] private float speed; // Скорость
    private float currentPosX; // Текущая позиция по оси X
    private Vector3 velocity = Vector3.zero; // Скорость

    // Следование за игроком
    [SerializeField] private Transform player; // Игрок
    [SerializeField] private float aheadDistance; // Расстояние вперед
    [SerializeField] private float cameraSpeed; // Скорость камеры
    private float lookAhead; // Движение вперед

    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Update() {
        // Камера комнаты
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);

        // Следование за игроком
        if (transform.position.y > minY && transform.position.y < maxY)
            transform.position = new Vector3(player.position.x + lookAhead, player.position.y, transform.position.z);
        else
            transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);

        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }

    public void MoveToNewRoom(Transform _newRoom) {
        currentPosX = _newRoom.position.x; // Обновление позиции по оси X для новой комнаты
    }
}

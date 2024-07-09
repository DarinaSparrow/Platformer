using UnityEngine;

public class CameraController : MonoBehaviour {
    // ������ �������
    [SerializeField] private float speed; // ��������
    private float currentPosX; // ������� ������� �� ��� X
    private Vector3 velocity = Vector3.zero; // ��������

    // ���������� �� �������
    [SerializeField] private Transform player; // �����
    [SerializeField] private float aheadDistance; // ���������� ������
    [SerializeField] private float cameraSpeed; // �������� ������
    private float lookAhead; // �������� ������

    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Update() {
        // ������ �������
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);

        // ���������� �� �������
        if (transform.position.y > minY && transform.position.y < maxY)
            transform.position = new Vector3(player.position.x + lookAhead, player.position.y, transform.position.z);
        else
            transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);

        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }

    public void MoveToNewRoom(Transform _newRoom) {
        currentPosX = _newRoom.position.x; // ���������� ������� �� ��� X ��� ����� �������
    }
}

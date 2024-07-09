using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlight : MonoBehaviour
{
    private Collider2D _touchingCollider; // Collider �����
    private Rigidbody2D _rigidBody; // Rigidbody �����

    private Transform _player; // Transform ������

    private float _flightSpeed = 3f; // �������� ����� �����
    [SerializeField] private float _searchDistance; // ����������, �� ������� ���� �������� ������������ ������

    [SerializeField] private List<Transform> _waypoints; // ������ ������� �����
    private int _nextWaypointNumber = 0; // ����� ��������� ������� �����
    private Transform _nextWaypoint; // ��������� ������� �����
    private float _waypointReachedDistance = 0.1f; // ���������� �� ������� �����, ��� ������� ��� ��������� �����������

    private bool _isAlive = true; // ����, ������������, ��� �� ����
    private bool _canMove = true; // ����, ������������, ����� �� ���� ������������

    Animator _animator; // �������� �����

    private void Awake()
    {
        _touchingCollider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player").transform;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _nextWaypoint = _waypoints[_nextWaypointNumber]; // ������������� ������ ����� ����
    }

    private void Update()
    {
        _isAlive = CheckIfAlive(); // ���������, ��� �� ����
        _canMove = CheckCanMove(); // ���������, ����� �� ���� ������������
    }

    private void FixedUpdate()
    {
        if (_isAlive)
        {
            if (_canMove)
            {
                if (Vector2.Distance(transform.position, (_player.position + new Vector3(0, -2f, 0))) > _searchDistance) FlightThroughWaypoints(); // ���� ����� ������, ���������� ����� � ��������� ������� �����
                else if (Vector2.Distance(transform.position, (_player.position + new Vector3(0, -2f, 0))) > 1.5f) FlightAfterPlayer(); // ���� ����� ������ (�� �� �������), ���������� ���
                else _rigidBody.velocity = Vector3.zero; // ���� ����� ������� ������, ���������������� ����������� �����
            }
            else _rigidBody.velocity = Vector3.zero; // ���� ���� �� ����� ������������, ������������� ���
        }
        else OnDeath(); // ���� ���� �����, ������������ ��� ������
    }

    private void FlightThroughWaypoints()
    {
        Vector2 _directionToWaypoint = (_nextWaypoint.position - transform.position).normalized; // ��������� ����������� � ��������� ������� �����
        _rigidBody.velocity = _directionToWaypoint * _flightSpeed; // ������������� �������� ��������
        UpdateDirection(); // ��������� ����������� �������

        float _distance = Vector2.Distance(_nextWaypoint.position, transform.position); // ��������� ���������� �� ��������� ������� �����

        if (_distance <= _waypointReachedDistance)
        {
            _nextWaypointNumber += 1; // ��������� � ��������� ������� �����
            if (_nextWaypointNumber >= _waypoints.Count) _nextWaypointNumber = 0; // ���� �������� ��������� �����, ��������� � ������
            _nextWaypoint = _waypoints[_nextWaypointNumber]; // ������������� ��������� ����� ����
        }
    }

    private void FlightAfterPlayer()
    {
        Vector2 _directionToWaypoint = ((_player.position + new Vector3(0, -2f, 0)) - transform.position).normalized; // ��������� ����������� � ������
        _rigidBody.velocity = _directionToWaypoint * _flightSpeed; // ������������� �������� ��������
        UpdateDirection(); // ��������� ����������� �������
    }

    private void UpdateDirection()
    {
        Vector3 _localScale = transform.localScale; // �������� ������� ������� �����

        if (((transform.localScale.x > 0) && (_rigidBody.velocity.x < 0)) || ((transform.localScale.x < 0) && (_rigidBody.velocity.x > 0))) transform.localScale = new Vector3(-1 * _localScale.x, _localScale.y, _localScale.z); // ���� ���� �������� � ��������������� �����������, ������������� ���
    }

    private bool CheckIfAlive()
    {
        return _animator.GetBool("isAlive"); // ���������, ��� �� ����
    }
    private bool CheckCanMove()
    {
        return _animator.GetBool("canMove"); // ��������� ����������� �����������
    }

    private void OnDeath()
    {
        _rigidBody.gravityScale = 2f; // ������������� ���������� ��� �������
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y); // ������������� �������������� ��������
    }
}
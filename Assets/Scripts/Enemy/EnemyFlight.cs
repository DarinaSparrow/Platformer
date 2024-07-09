using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlight : MonoBehaviour
{
    private Collider2D _touchingCollider; // Collider врага
    private Rigidbody2D _rigidBody; // Rigidbody врага

    private Transform _player; // Transform игрока

    private float _flightSpeed = 3f; // Скорость полёта врага
    [SerializeField] private float _searchDistance; // Расстояние, на котором враг начинает преследовать игрока

    [SerializeField] private List<Transform> _waypoints; // Список путевых точек
    private int _nextWaypointNumber = 0; // Номер следующей путевой точки
    private Transform _nextWaypoint; // Следующая путевая точка
    private float _waypointReachedDistance = 0.1f; // Расстояние до путевой точки, при котором она считается достигнутой

    private bool _isAlive = true; // Флаг, показывающий, жив ли враг
    private bool _canMove = true; // Флаг, показывающий, может ли враг перемещаться

    Animator _animator; // Аниматор врага

    private void Awake()
    {
        _touchingCollider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player").transform;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _nextWaypoint = _waypoints[_nextWaypointNumber]; // Устанавливаем первую точку пути
    }

    private void Update()
    {
        _isAlive = CheckIfAlive(); // Проверяем, жив ли враг
        _canMove = CheckCanMove(); // Проверяем, может ли враг перемещаться
    }

    private void FixedUpdate()
    {
        if (_isAlive)
        {
            if (_canMove)
            {
                if (Vector2.Distance(transform.position, (_player.position + new Vector3(0, -2f, 0))) > _searchDistance) FlightThroughWaypoints(); // Если игрок далеко, перемещаем врага к следующей путевой точке
                else if (Vector2.Distance(transform.position, (_player.position + new Vector3(0, -2f, 0))) > 1.5f) FlightAfterPlayer(); // Если игрок близко (но не слишком), преследуем его
                else _rigidBody.velocity = Vector3.zero; // Если игрок слишком близко, приостанавливаем перемещение врага
            }
            else _rigidBody.velocity = Vector3.zero; // Если враг не может перемещаться, останавливаем его
        }
        else OnDeath(); // Если враг мертв, обрабатываем его смерть
    }

    private void FlightThroughWaypoints()
    {
        Vector2 _directionToWaypoint = (_nextWaypoint.position - transform.position).normalized; // Вычисляем направление к следующей путевой точке
        _rigidBody.velocity = _directionToWaypoint * _flightSpeed; // Устанавливаем скорость движения
        UpdateDirection(); // Обновляем направление спрайта

        float _distance = Vector2.Distance(_nextWaypoint.position, transform.position); // Вычисляем расстояние до следующей путевой точки

        if (_distance <= _waypointReachedDistance)
        {
            _nextWaypointNumber += 1; // Переходим к следующей путевой точке
            if (_nextWaypointNumber >= _waypoints.Count) _nextWaypointNumber = 0; // Если достигли последней точки, переходим с первой
            _nextWaypoint = _waypoints[_nextWaypointNumber]; // Устанавливаем следующую точку пути
        }
    }

    private void FlightAfterPlayer()
    {
        Vector2 _directionToWaypoint = ((_player.position + new Vector3(0, -2f, 0)) - transform.position).normalized; // Вычисляем направление к игроку
        _rigidBody.velocity = _directionToWaypoint * _flightSpeed; // Устанавливаем скорость движения
        UpdateDirection(); // Обновляем направление спрайта
    }

    private void UpdateDirection()
    {
        Vector3 _localScale = transform.localScale; // Получаем текущий масштаб врага

        if (((transform.localScale.x > 0) && (_rigidBody.velocity.x < 0)) || ((transform.localScale.x < 0) && (_rigidBody.velocity.x > 0))) transform.localScale = new Vector3(-1 * _localScale.x, _localScale.y, _localScale.z); // Если враг движется в противоположном направлении, разворачиваем его
    }

    private bool CheckIfAlive()
    {
        return _animator.GetBool("isAlive"); // Проверяем, жив ли враг
    }
    private bool CheckCanMove()
    {
        return _animator.GetBool("canMove"); // Проверяем возможность перемещения
    }

    private void OnDeath()
    {
        _rigidBody.gravityScale = 2f; // Устанавливаем гравитацию для падения
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y); // Останавливаем горизонтальное движение
    }
}
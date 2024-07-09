using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Collider2D _touchingCollider; // Collider врага
    private Rigidbody2D _rigidBody; // Rigidbody врага

    [SerializeField] private Vector2 _currentDirectionVector; // Вектор текущего направления движения

    private float _groundDistance = 0.1f; // Дистанция для проверки земли
    private float _wallDistance = 0.1f; // Дистанция для проверки стены

    private bool _isGrounded = true; // Флаг, показывающий, находится ли враг на земле
    private bool _isOnWall = false; // Флаг, показывающий, касается ли враг стены
    private bool _canMove; // Флаг, показывающий, может ли враг перемещаться

    [SerializeField] private float _walkSpeed; // Скорость движения врага
    private float _brakingSpeed = 0.05f; // Скорость остановки движения врага

    [SerializeField] private float _offset; // Смещение врага при повороте

    Animator _animator; // Аниматор врага

    private void Awake()
    {
        _touchingCollider = GetComponent<BoxCollider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _isGrounded = CheckIfGrounded(); // Проверяем, находится ли враг на земле
        _isOnWall = CheckIfOnWall(); // Проверяем, касается ли враг стены
        _canMove = CheckCanMove(); // Проверяем, может ли враг перемещаться

        // Обновляем параметры анимации
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isOnWall", _isOnWall);
    }

    private void FixedUpdate()
    {
        if ((!_isGrounded) || (_isOnWall)) FlipDirection(); // Если враг не на земле, меняем направление

        // Устанавливаем скорость врага
        if (_canMove) _rigidBody.velocity = new Vector2(_walkSpeed * _currentDirectionVector.x, _rigidBody.velocity.y);
        else _rigidBody.velocity = new Vector2(Mathf.Lerp(_rigidBody.velocity.x, 0, _brakingSpeed), _rigidBody.velocity.y);
    }

    private void FlipDirection()
    {
        _currentDirectionVector *= -1f; // Меняем направление врага
        transform.position += new Vector3(_currentDirectionVector.x * _offset, 0, 0); // Смещаем врага (в силу его несимметричного спрайта)
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y); // Меняем масштаб по оси X для разворота врага
    }

    private bool CheckIfGrounded()
    {
        // Получаем локальные координаты начальной и конечной точек коллайдера врага
        Vector2 originLeft = new Vector2(_touchingCollider.bounds.min.x, _touchingCollider.bounds.min.y);
        Vector2 originRight = new Vector2(_touchingCollider.bounds.max.x, _touchingCollider.bounds.min.y);

        // Пускаем лучи вниз и проверяем, сталкиваются ли они с землёй
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, _groundDistance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, _groundDistance, LayerMask.GetMask("Ground"));

        return ((hitLeft.collider != null) && (hitRight.collider != null)); // Возвращаем true, если хотя бы один из лучей сталкивается с коллайдером
    }

    private bool CheckIfOnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_touchingCollider.bounds.center, _touchingCollider.bounds.size, 0, _currentDirectionVector, _wallDistance, LayerMask.GetMask("Walls") | LayerMask.GetMask("Ground")); // Проверяем, сталкивается ли коллайдер врага со стенами

        return raycastHit.collider != null; // Возвращаем true, если хотя бы один из лучей сталкивается с коллайдером
    }
    private bool CheckCanMove()
    {
        return _animator.GetBool("canMove"); // Проверяем возможность перемещения
    }
}
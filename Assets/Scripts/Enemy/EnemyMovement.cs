using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Collider2D _touchingCollider; // Collider �����
    private Rigidbody2D _rigidBody; // Rigidbody �����

    [SerializeField] private Vector2 _currentDirectionVector; // ������ �������� ����������� ��������

    private float _groundDistance = 0.1f; // ��������� ��� �������� �����
    private float _wallDistance = 0.1f; // ��������� ��� �������� �����

    private bool _isGrounded = true; // ����, ������������, ��������� �� ���� �� �����
    private bool _isOnWall = false; // ����, ������������, �������� �� ���� �����
    private bool _canMove; // ����, ������������, ����� �� ���� ������������

    [SerializeField] private float _walkSpeed; // �������� �������� �����
    private float _brakingSpeed = 0.05f; // �������� ��������� �������� �����

    [SerializeField] private float _offset; // �������� ����� ��� ��������

    Animator _animator; // �������� �����

    private void Awake()
    {
        _touchingCollider = GetComponent<BoxCollider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _isGrounded = CheckIfGrounded(); // ���������, ��������� �� ���� �� �����
        _isOnWall = CheckIfOnWall(); // ���������, �������� �� ���� �����
        _canMove = CheckCanMove(); // ���������, ����� �� ���� ������������

        // ��������� ��������� ��������
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isOnWall", _isOnWall);
    }

    private void FixedUpdate()
    {
        if ((!_isGrounded) || (_isOnWall)) FlipDirection(); // ���� ���� �� �� �����, ������ �����������

        // ������������� �������� �����
        if (_canMove) _rigidBody.velocity = new Vector2(_walkSpeed * _currentDirectionVector.x, _rigidBody.velocity.y);
        else _rigidBody.velocity = new Vector2(Mathf.Lerp(_rigidBody.velocity.x, 0, _brakingSpeed), _rigidBody.velocity.y);
    }

    private void FlipDirection()
    {
        _currentDirectionVector *= -1f; // ������ ����������� �����
        transform.position += new Vector3(_currentDirectionVector.x * _offset, 0, 0); // ������� ����� (� ���� ��� ��������������� �������)
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y); // ������ ������� �� ��� X ��� ��������� �����
    }

    private bool CheckIfGrounded()
    {
        // �������� ��������� ���������� ��������� � �������� ����� ���������� �����
        Vector2 originLeft = new Vector2(_touchingCollider.bounds.min.x, _touchingCollider.bounds.min.y);
        Vector2 originRight = new Vector2(_touchingCollider.bounds.max.x, _touchingCollider.bounds.min.y);

        // ������� ���� ���� � ���������, ������������ �� ��� � �����
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, _groundDistance, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, _groundDistance, LayerMask.GetMask("Ground"));

        return ((hitLeft.collider != null) && (hitRight.collider != null)); // ���������� true, ���� ���� �� ���� �� ����� ������������ � �����������
    }

    private bool CheckIfOnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(_touchingCollider.bounds.center, _touchingCollider.bounds.size, 0, _currentDirectionVector, _wallDistance, LayerMask.GetMask("Walls") | LayerMask.GetMask("Ground")); // ���������, ������������ �� ��������� ����� �� �������

        return raycastHit.collider != null; // ���������� true, ���� ���� �� ���� �� ����� ������������ � �����������
    }
    private bool CheckCanMove()
    {
        return _animator.GetBool("canMove"); // ��������� ����������� �����������
    }
}
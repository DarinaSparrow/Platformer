using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth; // Максимальное значение здоровья
    private int _health; // Текущий уровень здоровья

    private bool _isHit = false; // Флаг, показывающий, нанесён ли врагу урон
    private bool _isAlive = true; // Флаг, показывающий, жив ли враг

    private bool _isInvincible = false; // Флаг, показывающий, неуязвим ли враг
    private float _timeSinceHit = 0; // Время, прошедшее с момента получения урона
    [SerializeField] private float _invincibilityTimer; // Время неуязвимости после получения урона

    Animator _animator; // Аниматор врага

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _health = _maxHealth;
    }

    private void Update()
    {
       if (_isInvincible)
       {
            if (_timeSinceHit > _invincibilityTimer)
            {
                _isInvincible = false; // Сбрасываем неуязвимость
                _isHit = false; // Сбрасываем флаг получения урона
                _timeSinceHit = 0; // Сбрасываем таймер
            }

            _timeSinceHit += Time.deltaTime; // Обновляем таймер, фиксирующий время последнего получения урона
        }

        // Обновляем параметры анимации
        _animator.SetBool("isHit", _isHit);
        _animator.SetBool("isAlive", _isAlive);
    }

    public bool Hit(int damage)
    {
        if (_isAlive && !_isInvincible)
        {
            _isHit = true; // Устанавливаем флаг получения урона

            _health -= damage; // Уменьшаем здоровье врага
            _isInvincible = true; // Делаем врага временно неуязвимым

            if (_health <= 0) {
                _isAlive = false; // Если у врага закончилось здоровье, фиксируем его смерть
                return true;
            } 
        }
        return false;
    }

    public EnemyData GetEnemyData() {
        return new EnemyData {
            position = transform.position,
            health = _health,
            isAlive = _isAlive,
            isHit = _isHit
        };
    }

    public void LoadEnemyData(EnemyData data) {
        transform.position = data.position;
        _health = data.health;
        _isAlive = data.isAlive;
        _isHit = data.isHit;

        // Обновляем анимации и состояния
        _animator.SetBool("isAlive", _isAlive);
        _animator.SetBool("isHit", _isHit);
    }
}

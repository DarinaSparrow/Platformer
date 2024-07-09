using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _fadeDelay; // Время задержки перед удалением объекта
    private float _delayElapsed = 0f; // Время, прошедшее с начала смерти врага
    private float _fadeTime = 0.75f; // Время, за которое объект полностью исчезнет
    private float _timeElapsed = 0f; // Время, прошедшее с начала исчезновения
    SpriteRenderer _spriteRenderer;  // SpriteRenderer врага
    GameObject _objectToRemove; // Удаляемый объект
    Color _startColor; // Исходный цвет спрайта

    private EnemiesElimination enemiesElimination;

    private void Awake()
    {
        enemiesElimination = FindObjectOfType<EnemiesElimination>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed = 0f; // Сбрасываем счетчик времени
        _spriteRenderer = animator.GetComponent<SpriteRenderer>(); // Получаем компонент SpriteRenderer
        _startColor = _spriteRenderer.color; // Сохраняем исходный цвет спрайта
        _objectToRemove = animator.gameObject; // Сохраняем ссылку на удаляемый объект
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_fadeDelay > _delayElapsed) _delayElapsed += Time.deltaTime; // Увеличиваем счетчик времени
        else
        {
            _timeElapsed += Time.deltaTime; // Увеличиваем счетчик времени

            float _newAlpha = _startColor.a * (1 - _timeElapsed / _fadeTime); // Вычисляем новое значение альфа-канала для постепенного исчезновения
            _spriteRenderer.color = new Color(_startColor.r, _startColor.g, _startColor.b, _newAlpha); // Устанавливаем новый цвет спрайта

            if (_timeElapsed > _fadeTime)
            {
                Destroy(_objectToRemove); // Если время исчезновения истекло, удаляем объект
                enemiesElimination.EnemyKilled();
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float speedValue;
    [SerializeField] private float duration;
    [SerializeField] private AudioClip pickupSound;

    [SerializeField] private float recoveryTime; // Время восстановления

    private SpriteRenderer render; // Ссылка на рендер
    private bool isPickedUp = false; // Флаг для проверки, подобран ли предмет

    // Список дочерних объектов с тегом "Effects"
    private GameObject[] effectObjects;

    private void Awake() {
        render = GetComponent<SpriteRenderer>();
        // Найти все дочерние объекты с тегом "Effects"
        effectObjects = GameObject.FindGameObjectsWithTag("Effect");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player" && !isPickedUp) {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player) {
                player.SetAdditionalSpeed(speedValue, duration);
                SoundManager.instance.PlaySound(pickupSound);
                render.color = new Color(render.color.r, render.color.g, render.color.b, 0);
                isPickedUp = true;

                // Отключить дочерние объекты с тегом "Effect"
                SetEffectsActive(false);

                StartCoroutine(RestoreVisibility());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player" && !isPickedUp) {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player) {
                player.SetAdditionalSpeed(speedValue, duration);
                SoundManager.instance.PlaySound(pickupSound);
                render.color = new Color(render.color.r, render.color.g, render.color.b, 0);
                isPickedUp = true;

                // Отключить дочерние объекты с тегом "Effect"
                SetEffectsActive(false);

                StartCoroutine(RestoreVisibility());
            }
        }
    }

    // Корутина для восстановления видимости спрайта
    private IEnumerator RestoreVisibility() {
        float elapsedTime = 0f;

        while (elapsedTime < recoveryTime) {
            float alpha = elapsedTime / recoveryTime; // Вычисление альфа-канала
            render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
            elapsedTime += Time.deltaTime; // Увеличение времени
            yield return null; // Ожидание следующего кадра
        }

        render.color = new Color(render.color.r, render.color.g, render.color.b, 1); // Установка окончательного значения альфа-канала
        isPickedUp = false; // Сброс флага

        // Включить дочерние объекты с тегом "Effects"
        SetEffectsActive(true);
    }

    // Метод для включения/отключения дочерних объектов с тегом "Effects"
    private void SetEffectsActive(bool state) {
        foreach (GameObject effectObj in effectObjects) {
            if (effectObj) {
                // Убедиться, что объект является дочерним текущего
                if (effectObj.transform.IsChildOf(transform)) {
                    effectObj.SetActive(state);
                }
            }
        }
    }
}

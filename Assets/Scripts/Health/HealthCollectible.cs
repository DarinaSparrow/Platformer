using System.Collections;
using UnityEngine;

public class HealthCollectible : MonoBehaviour {
    [SerializeField] private float healthValue; // �������� ��������
    [SerializeField] private AudioClip pickupSound; // ���� �������
    [SerializeField] private float recoveryTime; // ����� ��������������

    private SpriteRenderer render; // ������ �� ������
    private bool isPickedUp = false; // ���� ��� ��������, �������� �� �������

    // ������ �������� �������� � ����� "Effects"
    private GameObject[] effectObjects;

    private void Awake() {
        render = GetComponent<SpriteRenderer>();
        // ����� ��� �������� ������� � ����� "Effects"
        effectObjects = GameObject.FindGameObjectsWithTag("Effect");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player" && !isPickedUp) {
            Health player = collision.GetComponent<Health>();
            if (player) {
                bool complete = player.AddHealth(healthValue);
                if (complete) {
                    SoundManager.instance.PlaySound(pickupSound);
                    render.color = new Color(render.color.r, render.color.g, render.color.b, 0);
                    isPickedUp = true;

                    // ��������� �������� ������� � ����� "Effects"
                    SetEffectsActive(false);

                    StartCoroutine(RestoreVisibility());
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "Player" && !isPickedUp) {
            Health player = collision.GetComponent<Health>();
            if (player) {
                bool complete = player.AddHealth(healthValue);
                if (complete) {
                    SoundManager.instance.PlaySound(pickupSound);
                    render.color = new Color(render.color.r, render.color.g, render.color.b, 0);
                    isPickedUp = true;

                    // ��������� �������� ������� � ����� "Effects"
                    SetEffectsActive(false);

                    StartCoroutine(RestoreVisibility());
                }
            }
        }
    }

    // �������� ��� �������������� ��������� �������
    private IEnumerator RestoreVisibility() {
        float elapsedTime = 0f;

        while (elapsedTime < recoveryTime) {
            float alpha = elapsedTime / recoveryTime; // ���������� �����-������
            render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
            elapsedTime += Time.deltaTime; // ���������� �������
            yield return null; // �������� ���������� �����
        }

        render.color = new Color(render.color.r, render.color.g, render.color.b, 1); // ��������� �������������� �������� �����-������
        isPickedUp = false; // ����� �����

        // �������� �������� ������� � ����� "Effects"
        SetEffectsActive(true);
    }

    // ����� ��� ���������/���������� �������� �������� � ����� "Effects"
    private void SetEffectsActive(bool state) {
        foreach (GameObject effectObj in effectObjects) {
            if (effectObj) {
                // ���������, ��� ������ �������� �������� ��������
                if (effectObj.transform.IsChildOf(transform)) {
                    effectObj.SetActive(state);
                }
            }
        }
    }
}
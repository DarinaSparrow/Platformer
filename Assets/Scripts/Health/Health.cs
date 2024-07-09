using UnityEngine;
using TMPro;
using System.Collections;

public class Health : MonoBehaviour {
    [Header("Здоровье")]
    [SerializeField] private float startingHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject healTextPrefab;
    [SerializeField] private Canvas gameCanvas;

    public float currentHealth { get; private set; }

    public float StartingHealth { get { return startingHealth; }
        set { startingHealth = value; }
        }

    private Animator anim;
    private bool dead;
    [Header("Интервалы неуязвимости")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Компоненты")]
    [SerializeField] private Behaviour[] components;
    [SerializeField] private PlayerMovement playerMovement;
    private bool invulnerable;

    [Header("Звуки смерти и урона")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake() {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage) {
        if (invulnerable) return;
        float damageReceived = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, maxHealth);

        playerMovement.StopMovement();
        playerMovement.ResetCollider();

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(transform.position);
        TextMeshProUGUI tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TextMeshProUGUI>();
        damageReceived = damageReceived - currentHealth;
        tmpText.text = damageReceived.ToString();

        if (currentHealth > 0) {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            SoundManager.instance.PlaySound(hurtSound);
        } else {
            if (!dead) {
                StartCoroutine(Invulnerability());
                foreach (Behaviour component in components)
                    component.enabled = false;

                anim.SetBool("grounded", true);
                anim.SetTrigger("die");

                dead = true;
                SoundManager.instance.PlaySound(deathSound);
                playerMovement.ResetCollider();
            }
        }
        SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
    }

    public void Kill() {
        currentHealth = 0;
        playerMovement.StopMovementForAttack();
        playerMovement.ResetCollider();

        if (!dead) {
            foreach (Behaviour component in components)
                component.enabled = false;

            anim.SetBool("grounded", true);
            anim.SetTrigger("die");

            dead = true;
            SoundManager.instance.PlaySound(deathSound);
            playerMovement.ResetCollider();
        }
        SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
    }

    public bool AddHealth(float _value) {
        if (currentHealth == maxHealth) return false;
        float healthRestored = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, maxHealth);

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(transform.position);
        TextMeshProUGUI tmpText = Instantiate(healTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TextMeshProUGUI>();
        healthRestored = currentHealth - healthRestored;
        tmpText.text = healthRestored.ToString();

        SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
        return true;
    }

    public void SetHealth(float health) {
        StartingHealth = health;
        currentHealth = health;
    }

    private IEnumerator Invulnerability() {
        invulnerable = true;
        for (int i = 0; i < numberOfFlashes; i++) {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        invulnerable = false;
    }

    public void Respawn() {
        playerMovement.ResumeMovementAfterAttack();
        playerMovement.ResetCollider();
        currentHealth = startingHealth;
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invulnerability());
        dead = false;

        foreach (Behaviour component in components)
            component.enabled = true;
    }

    public bool IsDead() {
        return dead;
    }
}
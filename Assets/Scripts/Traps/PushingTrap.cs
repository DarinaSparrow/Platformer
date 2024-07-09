using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingTrap : MonoBehaviour
{
    [Header("Time variables")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activityTime;

    [SerializeField] private float damage = 0f;

    private Animator trapAnimator;
    private Animator fireAnimator;


    private bool is_active;
    private bool is_triggered;

    private void Awake()
    {
        trapAnimator = GetComponent<Animator>();
        fireAnimator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!is_triggered)
                StartCoroutine(ActivateTrap());
            if (is_active)
                collision.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private IEnumerator ActivateTrap()
    {
        is_triggered = true;
        trapAnimator.SetBool("triggered", true);
        yield return new WaitForSeconds(activationDelay);
        trapAnimator.SetBool("triggered", false);
        fireAnimator.SetBool("fire", true);
        is_active = true;
        yield return new WaitForSeconds(activityTime);
        fireAnimator.SetBool("fire", false);
        is_active = false;
        is_triggered = false;
    }
}

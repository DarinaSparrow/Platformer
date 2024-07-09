using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingTrap : EnemyDamage
{
    [Header("Trap attributes")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float range = 7f;
    [SerializeField] private float attackDelay = 0.25f;
    [SerializeField] private LayerMask playerLayer;

    private float delayTimer;
    private Vector3 destination;
    private bool stalking;
    private Vector3[] directions = new Vector3[4];

    private bool contact;

    private void Update()
    {
        if (!contact)
        {
            if (stalking)
                transform.Translate(destination * speed * Time.deltaTime);
            else
            {
                delayTimer += Time.deltaTime;
                if (delayTimer > attackDelay)
                    FindPurpose();
            }
        }
    }

    private void FindPurpose()
    {
        CalculateDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);
            if (hit.collider != null && !stalking)
            {
                stalking = true;
                destination = directions[i];
                delayTimer = 0;
            }
        }        
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < directions.Length; i++)
        {
            Gizmos.DrawRay(transform.position, directions[i]);
        }
    }

    private void Stop()
    {
        destination = transform.position;
        stalking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Stop();
    }

    private new void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>()?.TakeDamage(damage);
            contact = true;
        }
        Stop();  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contact = false;
        }
    }
}

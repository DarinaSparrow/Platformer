using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SidewaysMovement : MonoBehaviour
{
    [SerializeField] private float damage = 0f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float movementRange = 3f;

    private float leftBorder;
    private float rightBorder;
    private bool movingLeft;

    private void Awake()
    {
        leftBorder = transform.position.x - movementRange;
        rightBorder = transform.position.x + movementRange;
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftBorder)
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y);
            else
                movingLeft = false;
        }
        else
        {
            if (transform.position.x < rightBorder)
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y);
            else
                movingLeft = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 leftPoint = new Vector3(transform.position.x - movementRange, transform.position.y, transform.position.z);
        Vector3 rightPoint = new Vector3(transform.position.x + movementRange, transform.position.y, transform.position.z);

        Gizmos.DrawLine(transform.position, leftPoint);
        Gizmos.DrawLine(transform.position, rightPoint);
    }
}

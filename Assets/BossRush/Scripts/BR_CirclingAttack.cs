using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_CirclingAttack : BR_Attack
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [ContextMenu("attack")]
    void AttackFromEditor()
    {
    }

    public override void ManageAttack(BR_AttackData data)
    {
        StartCoroutine(Attack(data));
    }

    private Vector3 centerPoint;
    private GameObject centerObject;
    IEnumerator Attack(BR_AttackData data)
    {
        damage = data.damage;
        var speedDirection = data.direction;
        if (data.target != null)
        {
            speedDirection = data.target.position - transform.position;
            speedDirection.Normalize();
        }
        centerPoint = data.center;
        centerObject = new GameObject("center");
        centerObject.transform.position = centerPoint;
        gameObject.transform.parent = centerObject.transform;
        yield return new WaitForSeconds(data.delay);
        float elapsed = 0f;
        var radius = Vector2.Distance(centerPoint, transform.position);
        var length = 2 * Mathf.PI * radius;
        var rotationSpeed = length * data.frequency;
        while (elapsed < data.duration)
        {
            Debug.Log("radius: " + Vector3.Distance(centerObject.transform.position, transform.position));
            
            var moveCenter = speedDirection * data.speed;
            centerObject.transform.position += moveCenter * Time.deltaTime;
            Vector2 dir = (Vector2)transform.position - (Vector2)centerObject.transform.position;
            Vector3 perpendicularDirection = new Vector3(-dir.y, dir.x).normalized;
            Vector3 velocity = perpendicularDirection * rotationSpeed;
            rb.velocity = velocity;
            yield return null;
            
            elapsed += Time.deltaTime;
        }
        Destroy(centerObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            col.gameObject.GetComponent<BR_PlayerController>().GetDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        if (centerObject == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(centerObject.transform.position, 0.2f);
        Gizmos.DrawLine(centerObject.transform.position, transform.position);
    }
}

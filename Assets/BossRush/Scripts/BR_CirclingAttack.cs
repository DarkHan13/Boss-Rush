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
        centerObject = new GameObject("center");
        centerObject.transform.position = data.center;
        gameObject.transform.parent = centerObject.transform;

        if (data.needTrail)
        {
            GetComponent<TrailRenderer>().enabled = true;
        }
        
        yield return new WaitForSeconds(data.delay);
        float elapsed = 0f;
        var moveCenter = speedDirection * data.speed;
        var centerPoint = centerObject.transform;
        while (elapsed < data.duration)
        {
            transform.RotateAround(centerPoint.position, Vector3.forward, data.degrees * Time.deltaTime);
            centerPoint.position += moveCenter * Time.deltaTime;
            centerObject.transform.position = centerPoint.position;
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

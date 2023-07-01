using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_SimpleProjectile : BR_Attack
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

    IEnumerator Attack(BR_AttackData data)
    {
        damage = data.damage;
        yield return new WaitForSeconds(data.delay);
        Vector3 dir;
        if (data.target != null)
        {
            dir = data.target.position - transform.position;
            dir.Normalize();
        }
        else
            dir = data.direction;
        dir.Normalize();
        float elapsed = 0f;
        
        GetComponent<AudioSource>().Play();
        
        while (elapsed < data.duration)
        {
            rb.velocity = data.speed * dir;
            yield return null;
            elapsed += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            col.gameObject.GetComponent<BR_PlayerController>().GetDamage(damage);
        }
    }
}

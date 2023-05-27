using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_BouncingAttack : BR_Attack
{
    public Vector3 dir;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool isEnter = false;
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            var player = col.gameObject.GetComponent<BR_PlayerController>();
            if (player.IsInvincible()) return;
            player.GetDamage(damage);
            Destroy(gameObject);

        } else if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            dir = Vector2.Reflect(dir, col.GetContact(0).normal);
        }
    }

    public override void ManageAttack(BR_AttackData data)
    {
        StartCoroutine(Attack(data));
    }
    
    IEnumerator Attack(BR_AttackData data)
    {
        damage = data.damage;
        yield return new WaitForSeconds(data.delay);
        if (data.target != null)
        {
            dir = data.target.position - transform.position;
            dir.Normalize();
        }
        else
            dir = data.direction;
        dir.Normalize();
        float elapsed = 0f;
        while (elapsed < data.duration)
        {
            rb.velocity = data.speed * dir;
            yield return null;
            elapsed += Time.deltaTime;
        }
        Destroy(gameObject);
    }
}

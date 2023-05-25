using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_SinAttack : BR_Attack
{
    public bool isTest = false;
    public Vector2 direction = Vector2.one;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    float _elapsed = 0f;

    // Update is called once per frame
    void Update()
    {
        float frequency = 5f;
        float amplitude = 20f;
        if (isTest)
        {
            float time = _elapsed * frequency;
            var ySin = Mathf.Sin(time + Mathf.PI / 2) * amplitude * direction.x;
            var xSin = Mathf.Sin(time + Mathf.PI / 2) * amplitude * -direction.y;
            var newDir = new Vector3(xSin, ySin);
            newDir.Normalize();
            rb.velocity = 5f  *  newDir;
            _elapsed += Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            col.gameObject.GetComponent<BR_PlayerController>().GetDamage(damage);
        }
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
        float frequency = data.frequency; 
        float amplitude = data.amplitude;
        yield return new WaitForSeconds(data.delay);
        Vector3 dir;
        if (data.target != null)
        {
            dir = data.target.position - transform.position;
            dir.Normalize();
        }
        else
            dir = data.direction;
        float elapsed = 0f;
        while (elapsed < data.duration)
        {
            float time = elapsed * frequency;
            var ySin = Mathf.Sin(time + Mathf.PI / 2) * amplitude * dir.x;
            var xSin = Mathf.Sin(time + Mathf.PI / 2) * amplitude * -dir.y;
            var newDir = new Vector3(xSin, ySin);
            rb.velocity = newDir + data.speed  *  dir;
            yield return null;
            elapsed += Time.deltaTime;
        }
        Destroy(gameObject);
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float duration = 7f;
    public float speed = 5f;
    private float elapsed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        _rb.velocity = Vector2.up * speed;
        if (elapsed >= duration) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EnemyRed"))
        {
            col.GetComponent<BR_Boss>().GetDamage(1);
            Destroy(gameObject);
        }
    }
}

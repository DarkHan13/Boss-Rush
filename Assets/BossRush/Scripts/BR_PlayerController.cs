using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_PlayerController : MonoBehaviour
{
    public float speed = 1f;
    public float health = 20f;

    public GameObject bulletPrefab;
    public BR_Slider healthBar;
    public Transform gun;

    public float duration = 0.5f;
    public float elapsed = 0;
    
    private float horizontalInput, verticalInput;
    private bool _isInvincible = false;
    private Animator _animator;

    private Rigidbody2D _rb;

    private static readonly int Invincible = Animator.StringToHash("invincible");
    public event Action onDead;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        healthBar.SetSliderValues(health, health, 0);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;
        if (elapsed >= duration)
        {
            PrefabManager.InstantiateAndSetActive(bulletPrefab, gun.position, Quaternion.identity);
            elapsed = 0;
        }
        
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        _rb.velocity = movement * speed;
    }

    public void GetDamage(int damage = 1)
    {
        if (_isInvincible) return;
        health -= damage;
        healthBar.SetSliderValues(health);
        if (health <= 0)
        {
            Dead();
            return;
        }
        _isInvincible = true;
        _animator.SetBool(Invincible, _isInvincible);
    }

    private void EndInvincible()
    {
        _isInvincible = false;
        _animator.SetBool(Invincible, _isInvincible);
    }

    private void Dead()
    {
        onDead?.Invoke();
        Destroy(gameObject);
    }
}

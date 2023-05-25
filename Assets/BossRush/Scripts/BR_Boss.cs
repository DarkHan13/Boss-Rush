using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BR_Boss : MonoBehaviour
{
    private Vector3 startPos;
    private float health;
    
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private BR_Slider healthbar;
    [SerializeField] private TextMeshProUGUI healthText;
    
    public List<GameObject> attackObjects = new ();
    public event Action onDead;


    private List<FuncAttacks> attacks = new();


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthbar.SetSliderValues(maxHealth, maxHealth, 0);
        startPos = transform.position;
        
        attacks.Add(new FuncAttacks(SinTargetOneShoot, 2f));

        
        attacks.Add(new FuncAttacks(WallSinWaveShoot, 7f));
        attacks.Add(new FuncAttacks(WaveShoot, 12f));
        attacks.Add(new FuncAttacks(SinWaveShoot, 14f));
        attacks.Add(new FuncAttacks(RainShoot, 6f));
        attacks.Add(new FuncAttacks(TargetBouncingOneShoot, 2f));
        attacks.Add(new FuncAttacks(WallRainShoot, 6f));
        attacks.Add(new FuncAttacks(CirclingAttack, 3f));
        attacks.Add(new FuncAttacks(CirclingTargetAttack, 3f));
        attacks.Add(new FuncAttacks(TargetOneShoot, 2f));
        StartCoroutine(AttackController());
    }

    


    private void Update()
    {
        var x = Mathf.Sin(Time.time) * 3f;
        transform.position = new Vector3(x, transform.position.y);
    }

    IEnumerator AttackController()
    {
        int oldIndex = 0;
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            var playerObject = GameObject.Find("Player");
            if (playerObject == null) break;
            var index = Random.Range(0, attacks.Count);
            if (index == oldIndex) index++;
            index = Math.Clamp(index, 0, attacks.Count - 1);
            var fa = attacks[index];
            fa.func();
            oldIndex = index;
            var delay = fa.delay;
            if (health / maxHealth <= 0.3f)
            {
                Debug.Log("DANGER");
                delay--;
            }
            yield return new WaitForSeconds(delay);
        }
        
    }
    
    public void GetDamage(int damage)
    {
        health -= damage;
        healthbar.SetSliderValues(health);
        healthText.text = $"{(health / maxHealth) * 100}%";
        if (health <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        onDead?.Invoke();
        StopAllCoroutines();
        Destroy(gameObject);
    }
    
    void WaveShoot()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        for (int i = 0; i < 40; i++)
        {
            attackInstance.Add(PrepareAttack(attackObjects[0], startPos + new Vector3(0, 2)));
        }
        var delay = 0f;
        foreach (var attack in attackInstance)
        {
            var data = new BR_AttackData()
            {
                direction = new Vector3(Mathf.Sin(delay * 2), -1),
                speed = 7f,
                delay = delay,
                duration = 5f,
                frequency = 7f,
                amplitude = 15f
            };
            attack.ManageAttack(data);
            delay += 0.25f;
        }
    }
    
    void SinWaveShoot()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        for (int i = 0; i < 60; i++)
        {
            attackInstance.Add(PrepareAttack(attackObjects[1], startPos + new Vector3(0, 2)));
        }
        var delay = 0f;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            float sign = i % 2 == 0 ? -1 : 1;
            var data = new BR_AttackData()
            {
                damage = 3,
                direction = new Vector3( sign * Mathf.Sin(delay), -1),
                speed = 7f,
                delay = delay,
                duration = 5f,
                frequency = 5f,
                amplitude = 10f
            };
            attack.ManageAttack(data);
            delay += 0.2f;
        }
    }
    
    private void SinTargetOneShoot()
    {
        BR_Attack attack =PrepareAttack(attackObjects[1], transform.position);
        
        
        var player = GameObject.Find("Player");
        if (player == null) return;
        var data = new BR_AttackData()
        {
            damage = 3,
            speed = Random.Range(7f, 14f),
            target = player.transform,
            duration = 5f,
            frequency = Random.Range(10f, 15f),
            amplitude = Random.Range(10f, 15f)
        };
        attack.ManageAttack(data);
        
    }

    void WallSinWaveShoot()
    {
        var side = 6.88f;
        if (Random.Range(0, 2) == 0) side = -side;
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        for (int i = 0; i < 30; i++)
        {
            attackInstance.Add(PrepareAttack(attackObjects[1], startPos + new Vector3(side, -3.6f)));
        }
        var delay = 0f;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            float sign = i % 2 == 0 ? -1 : 1;
            var data = new BR_AttackData()
            {
                damage = 3,
                direction = new Vector3( -Math.Sign(side), sign * Mathf.Sin(delay)),
                speed = 7f,
                delay = delay,
                duration = 5f,
                frequency = 5f,
                amplitude = 10f
            };
            attack.ManageAttack(data);
            delay += 0.2f;
        }
    }
    
    void RainShoot()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        for (int i = -5; i <= 5; i++)
        {
            attackInstance.Add(PrepareAttack(attackObjects[0], startPos + new Vector3(i / 1.1f, 2)));
            attackInstance.Add(PrepareAttack(attackObjects[0], startPos + new Vector3(i / 1.1f, 3)));
        }
        ShuffleList(attackInstance);
        var delay = 1f;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            var data = new BR_AttackData()
            {
                direction = Vector3.down,
                speed = 10f,
                delay = delay,
                duration = 8f,
            };
            attack.ManageAttack(data);
            if (i % 3 == 0)
                delay += 0.3f;
        }
    }
    
    void WallRainShoot()
    {
        List<BR_Attack> attackInstance1 = new List<BR_Attack>();
        List<BR_Attack> attackInstance2 = new List<BR_Attack>();
        for (int i = -5; i < -2; i++)
        {
            attackInstance1.Add(PrepareAttack(attackObjects[0], startPos + new Vector3(-6.43f, i)));
            attackInstance2.Add(PrepareAttack(attackObjects[0], startPos + new Vector3(6.43f, i)));
        }
        ShuffleList(attackInstance1);
        ShuffleList(attackInstance2);
        var delay = 1f;
        for (var i = 0; i < attackInstance1.Count; i++)
        {
            var attack1 = attackInstance1[i];
            var attack2 = attackInstance2[i];
            var data1 = new BR_AttackData()
            {
                direction = Vector3.right,
                speed = 10f,
                delay = delay,
                duration = 8f,
            };
            var data2 = new BR_AttackData()
            {
                direction = Vector3.left,
                speed = 7f,
                delay = delay,
                duration = 8f,
            };
            attack1.ManageAttack(data1);
            attack2.ManageAttack(data2);
            delay += 0.5f;
        }
    }
    
    void TargetOneShoot()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        var position = transform.position;
        attackInstance.Add(PrepareAttack(attackObjects[0], position));
        var player = GameObject.Find("Player");
        if (player == null) return;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            var data = new BR_AttackData()
            {
                target = player.transform,
                speed = 15f,
                duration = 5f,
                delay = 0
            };
            attack.ManageAttack(data);
        }
    }

    void TargetBouncingOneShoot()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        var position = transform.position;
        attackInstance.Add(PrepareAttack(attackObjects[2], position));
        var player = GameObject.Find("Player");
        if (player == null) return;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            var data = new BR_AttackData()
            {
                target = player.transform,
                speed = 10f,
                duration = 5f,
                delay = 0
            };
            attack.ManageAttack(data);
        }
    }
    
    void CirclingAttack()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        for (int i = -4; i < 1; i++)
        {
            attackInstance.Add(PrepareAttack(attackObjects[3], startPos + new Vector3(i * 1.9f , 2)));
        }
        ShuffleList(attackInstance);
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            var data = new BR_AttackData()
            {
                center = startPos + new Vector3(0, 2),
                degrees = 144,
                delay = 1f,
                duration = 2f,
                needTrail = true
            };
            attack.ManageAttack(data);
        }
    }
    
    void CirclingTargetAttack()
    {
        List<BR_Attack> attackInstance = new List<BR_Attack>();
        attackInstance.Add(PrepareAttack(attackObjects[3], transform.position));

        ShuffleList(attackInstance);
        var player = GameObject.Find("Player");
        if (player == null) return;
        for (var i = 0; i < attackInstance.Count; i++)
        {
            var attack = attackInstance[i];
            var data = new BR_AttackData()
            {
                speed = 4f,
                target = player.transform,
                center = transform.position + new Vector3(0, 1.5f),
                degrees = 720,
                duration = 15f,
            };
            attack.ManageAttack(data);
        }
    }
    
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    BR_Attack PrepareAttack(GameObject go, Vector3 pos, Quaternion rotation = default)
    {
        return PrefabManager.InstantiateAndSetActive(go, pos, rotation).GetComponent<BR_Attack>();
    }

    class FuncAttacks
    {
        public Action func;
        public float delay;

        public FuncAttacks(Action func, float delay)
        {
            this.func = func;
            this.delay = delay;
        }
    } 
}

public class BR_AttackData
{
    public Transform target;
    public Vector3 direction = Vector3.zero;
    public int damage = 5;
    public float speed = 5;
    public float delay = 0;
    public float duration = 5;
    
    
    // for sinus
    public float frequency = 1f;
    public float amplitude = 15f;
    
    // for circle
    public Vector3 center;
    public float degrees = 0;
    
    // trail setting
    public bool needTrail = false;
}

public static class PrefabManager
{
    public static GameObject InstantiateAndSetActive(GameObject o, Vector3 pos, Quaternion r)
    {
        var createdObject = GameObject.Instantiate(o, pos, r);
        createdObject.SetActive(true);
        return createdObject;
    }
}
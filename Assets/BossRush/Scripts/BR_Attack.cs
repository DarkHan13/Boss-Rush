using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BR_Attack : MonoBehaviour
{
    protected Rigidbody2D rb;
    public int damage = 5;

    public abstract void ManageAttack(BR_AttackData data);
    
}

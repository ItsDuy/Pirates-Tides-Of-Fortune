using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerAttack : MonoBehaviour,IAttack
{
    // Start is called before the first frame update
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackTImeOut = 0.5f; // Default timeout value in seconds

    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;
    private Animator animator;
    private LayerMask enemyLayers;
    private float lastAttackTime = 0f; // To track when the last attack was performed

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);       
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackTImeOut)
        {
            // Update the last attack time
            lastAttackTime = Time.time;
            
            // Trigger the attack animation
            animator.SetTrigger("isAttack1");
            
            // Check for enemies in attack range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyLayers = LayerMask.GetMask("Enemy"); // Make sure you have an "Enemy" layer set up in Unity
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }
}

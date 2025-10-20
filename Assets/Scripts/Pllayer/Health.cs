using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour,IDamageable
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public void Die()
    {
        isDead = true;
        Debug.Log("Character has died.");
    }
    public bool IsDead()
    {
        return isDead;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

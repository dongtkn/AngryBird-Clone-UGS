using UnityEngine;

public class PigBad : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold = 0.2f;
    [SerializeField] private GameObject pigBadDeathParticle;
    [SerializeField] private AudioClip deathClip;
    private float currentHealth;
    private void Awake()
    {
        currentHealth = maxHealth;

    }
    public void DamagePigBad(float damageAmount)
    {
        currentHealth -= damageAmount;
        if(currentHealth <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        GameManager.instance.RemovePigBad(this);
        Instantiate(pigBadDeathParticle, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if(impactVelocity > damageThreshold)
        {
            DamagePigBad(impactVelocity);
        }
    }
}

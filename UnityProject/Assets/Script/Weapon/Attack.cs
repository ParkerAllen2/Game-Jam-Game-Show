using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float attackRate;

    public float delayTime;
    public float attackActiveTime;
    public float stunTime;

    public int damage;
    public float range;

    public bool destroyOnHit;
    public bool triggerImmune;

    public string tagToHit;

    [HideInInspector] public bool canUse = true;

    public IEnumerator Cooldown()
    {
        canUse = false;
        yield return new WaitForSeconds(attackRate);
        canUse = true;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(tagToHit))
        {
            collision.GetComponent<HealthController>().TakeDamage(-damage, triggerImmune);
            if (destroyOnHit)
            {
                gameObject.SetActive(false);
            }
        }
    }
}

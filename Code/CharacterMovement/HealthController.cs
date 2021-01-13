using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    PlatformerMovement movement;
    public HealthBarUI healthUI;
    public Stage1Controller overlord;
    public AudioSource hitSound;

    public int health = 3;
    public bool dead;

    public float immuneTime = .05f;
    public bool canBeHit;
    public bool immortal;

    SpriteRenderer sprite;
    Color original;

    void Start()
    {
        movement = GetComponent<PlatformerMovement>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        healthUI = GetComponentInChildren<HealthBarUI>();
        healthUI.AddHearts(health);
        canBeHit = true;
        original = sprite.color;
    }

    public virtual bool TakeDamage(int damage, bool triggerImmune)
    {
        if(canBeHit)
        {
            hitSound.Play();
            if (healthUI.SetHealth(damage) && !immortal)
            {
                StopAllCoroutines();
                Die();

                if(this.CompareTag("MyPlayer"))
                    overlord.EndStage(0);
                else
                    overlord.KillEnemy();
            }
            else
            {
                StartCoroutine(TakeHit());

                if (triggerImmune)
                    StartCoroutine(ImmuneTime());
            }

            return true;
        }
        return false;
    }

    public void Die()
    {
        canBeHit = false;
        dead = true;
        StartCoroutine(Dissolve());
    }

    public IEnumerator TakeHit()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sprite.color = original;
    }

    public IEnumerator ImmuneTime()
    {
        canBeHit = false;
        yield return new WaitForSeconds(immuneTime);
        canBeHit = true;
    }

    IEnumerator Dissolve()
    {
        Material mat = sprite.material;

        float i = 1;
        while(i > 0)
        {
            i -= .02f;
            mat.SetFloat("_Fade", i);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}

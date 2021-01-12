using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlatformerMovement platformerMovement;
    public SpriteRenderer sprite;
    public Sprite pickUpSprite;

    public bool canAttack;
    public bool dropped;

    public Attack[] attacks;

    public Vector3 target;
    public string tagToHit;

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        platformerMovement = GetComponentInParent<PlatformerMovement>();
        canAttack = true;

        foreach(Attack a in attacks)
        {
            a.gameObject.SetActive(false);
        }
    }

    public string Attack(int i , Vector3 t)
    {
        target = t;
        if(canAttack && i < attacks.Length)
        {
            if (i == 0 && attacks[0].canUse)
            {
                return Attack1();
            }
            else if (i == 1 && attacks[1].canUse)
            {
                return Attack2();
            }
            else if (i == 2 && attacks[2].canUse)
            {
                return Attack3();
            }
        }
        return "";
    }

    public virtual string Attack1()
    {
        return "";
    }

    public virtual string Attack2()
    {
        return "";
    }

    public virtual string Attack3()
    {
        return "";
    }

    public void StartActionCooldown(float t)
    {
        canAttack = false;
        StartCoroutine(ActionCooldown(t));
    }

    IEnumerator ActionCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        canAttack = true;
    }

    public void StartAttackCooldown(Attack att)
    {
        StartCoroutine(att.Cooldown());
    }

    public void SetTags(string t)
    {
        if (t.Equals("MyPlayer"))
        {
            t = "Enemy";
        }
        else if(t.Equals("Enemy"))
        {
            t = "MyPlayer";
        }
        tagToHit = t;
        foreach(Attack a in attacks)
        {
            a.tagToHit = t;
        }
    }
}

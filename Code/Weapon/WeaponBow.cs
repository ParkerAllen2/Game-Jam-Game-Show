using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : Weapon
{
    public Projectile arrow;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float speed, superSpeed;
    [SerializeField] float incAngle;
    public AudioSource noise;

    float angle;

    public override string Attack1()
    {
        base.Attack1();
        angle = GetMouseAngle();
        StartCoroutine(StartAttack1());
        return "Bow1";
    }

    public override string Attack2()
    {
        base.Attack2();
        angle = GetMouseAngle();
        StartCoroutine(StartAttack2());
        return "Bow2";
    }

    public override string Attack3()
    {
        base.Attack3();
        angle = GetMouseAngle();
        StartCoroutine(StartAttack3());
        return "Bow3";
    }

    IEnumerator StartAttack1()
    {
        Attack att = attacks[0];
        StartActionCooldown(att.stunTime);

        yield return new WaitForSeconds(att.delayTime);

        FireArrow(att, angle, speed);

        StartAttackCooldown(att);
    }

    IEnumerator StartAttack2()
    {
        Attack att = attacks[1];
        StartActionCooldown(att.stunTime);
        platformerMovement.timeScale = 0.4f;

        angle = angle - incAngle * 2;
        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(att.delayTime);
            FireArrow(att, angle, speed);
            angle += incAngle;
        }

        platformerMovement.timeScale = 1;

        StartAttackCooldown(att);
    }

    IEnumerator StartAttack3()
    {
        Attack att = attacks[2];
        StartActionCooldown(att.stunTime);

        yield return new WaitForSeconds(att.delayTime);

        noise.volume = .3f;
        FireArrow(att, angle, superSpeed);
        noise.volume = .25f;

        StartAttackCooldown(att);
    }

    public void FireArrow(Attack att, float angle, float sp)
    {
        noise.Play();
        Projectile temp = Instantiate(arrow, transform.position, Quaternion.Euler(0, 0, angle));
        temp.SetValues(att.damage, sp, att.attackActiveTime, att.destroyOnHit, att.triggerImmune, collisionMask, tagToHit);
    }

    public float GetMouseAngle()
    {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        camPos.z = 0;
        float angle = Vector3.SignedAngle(Vector3.right, target - transform.position, Vector3.forward);

        platformerMovement.DirectionalInput = (Mathf.Abs(angle) < 90) ? Vector2.right : Vector2.left;
        platformerMovement.flipDaSprite = true;
        return angle;
    }
}

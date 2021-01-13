using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : Weapon
{
    public ParticleSystem particleTrail;
    public AudioSource noise;

    public override string Attack1()
    {
        base.Attack1();
        StartCoroutine(StartAttack1());
        return "Sword1";
    }

    public override string Attack2()
    {
        base.Attack2();
        StartCoroutine(StartAttack2());
        return "Sword2";
    }

    public override string Attack3()
    {
        base.Attack3();
        StartCoroutine(StartAttack3());
        return "Sword3";
    }

    IEnumerator StartAttack1()
    {
        Attack att = attacks[0];
        StartActionCooldown(att.stunTime);

        yield return new WaitForSeconds(att.delayTime);

        particleTrail.Play();
        noise.Play();
        att.gameObject.SetActive(true);
        yield return new WaitForSeconds(att.attackActiveTime);
        att.gameObject.SetActive(false);
        particleTrail.Stop();

        StartAttackCooldown(att);
    }

    IEnumerator StartAttack2()
    {
        Attack att = attacks[1];
        StartActionCooldown(att.stunTime);

        yield return new WaitForSeconds(att.delayTime);

        platformerMovement.timeScale = .5f;
        particleTrail.Play();

        att.gameObject.SetActive(true);
        noise.Play();
        yield return new WaitForSeconds(att.attackActiveTime);
        noise.Play();
        att.gameObject.SetActive(false);

        particleTrail.Stop();
        platformerMovement.timeScale = 1;
        StartAttackCooldown(att);
    }

    IEnumerator StartAttack3()
    {
        Attack att = attacks[2];
        StartActionCooldown(att.stunTime);

        yield return new WaitForSeconds(att.delayTime);

        platformerMovement.timeScale = 8f;
        particleTrail.Play();
        noise.Play();

        att.gameObject.SetActive(true);
        yield return new WaitForSeconds(att.attackActiveTime);
        att.gameObject.SetActive(false);

        particleTrail.Stop();
        platformerMovement.timeScale = 1;
        StartAttackCooldown(att);
    }
}

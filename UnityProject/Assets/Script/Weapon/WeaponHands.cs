using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHands : Weapon
{
    public HandProjectile projectile;
    public HandProjectile bigProjectile;
    public Transform handPosition;
    public AudioSource noise;

    public override string Attack1()
    {
        base.Attack1();
        StartCoroutine(StartAttack1());
        return "Hand1";
    }

    public override string Attack2()
    {
        base.Attack2();
        StartCoroutine(StartAttack2());
        return "Hand2";
    }

    public override string Attack3()
    {
        base.Attack3();
        StartCoroutine(StartAttack3());
        return "Hand3";
    }

    IEnumerator StartAttack1()
    {
        Attack att = attacks[0];
        StartActionCooldown(att.stunTime);
        platformerMovement.timeScale = .3f;

        yield return new WaitForSeconds(att.delayTime);

        att.gameObject.SetActive(true);
        noise.Play();
        yield return new WaitForSeconds(att.attackActiveTime);
        att.gameObject.SetActive(false);

        platformerMovement.timeScale = 1;
        StartAttackCooldown(att);
    }

    IEnumerator StartAttack2()
    {
        Attack att = attacks[1];
        StartActionCooldown(att.stunTime);
        platformerMovement.timeScale = .5f;

        yield return new WaitForSeconds(att.delayTime);

        att.gameObject.SetActive(true);
        StartCoroutine(SpamFists());
        yield return new WaitForSeconds(att.attackActiveTime);
        att.gameObject.SetActive(false);

        platformerMovement.timeScale = 1;
        StartAttackCooldown(att);
    }

    IEnumerator StartAttack3()
    {
        Attack att = attacks[2];
        StartActionCooldown(att.stunTime);
        platformerMovement.timeScale = .5f;

        yield return new WaitForSeconds(att.delayTime);

        att.gameObject.SetActive(true);
        noise.Play();
        SpawnProjectile(bigProjectile);
        yield return new WaitForSeconds(att.attackActiveTime);
        att.gameObject.SetActive(false);

        platformerMovement.timeScale = 1;
        StartAttackCooldown(att);
    }

    IEnumerator SpamFists()
    {
        for(int i = 0; i < 8; i++)
        {
            noise.Play();
            SpawnProjectile(projectile);
            yield return null;
        }
    }

    void SpawnProjectile(HandProjectile hp)
    {
        float angle = platformerMovement.FaceDir.x;
        angle = (angle == -1) ? 180 : 0;
        HandProjectile temp = Instantiate(hp, handPosition.position + Vector3.up * .5f, Quaternion.Euler(0, 0, angle));
        temp.tagToHit = tagToHit;
    }
}

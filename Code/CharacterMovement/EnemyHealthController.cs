using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{

    public override bool TakeDamage(int damage, bool triggerImmune)
    {
        if (canBeHit)
        {
            if (healthUI.SetHealth(damage) && !immortal)
            {
                StopAllCoroutines();
                overlord.KillEnemy();
                Die();
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
}

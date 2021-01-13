using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Stage1Controller overlord;
    private PlatformerMovement playerMovement;
    HealthController healthController;
    Transform playerPosition;

    Animator[] animators;
    SpriteRenderer[] spriteRenderers;
    Transform[] spriteTransforms;
    Vector3 spriteOffset;

    public Transform handTransform;
    public Weapon currentWeapon;
    public Weapon defaultWeapon;

    public PickUpWeapon thrownWeapon;
    public float maxPickUpRange = 2;

    [HideInInspector] public bool dead;

    bool aiCanAttack;

    void Start()
    {
        playerMovement = GetComponentInParent<PlatformerMovement>();
        healthController = GetComponentInParent<HealthController>();
        playerPosition = GameObject.FindGameObjectsWithTag("MyPlayer")[0].transform;

        animators = transform.parent.GetComponentsInChildren<Animator>();
        spriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();

        spriteTransforms = new Transform[2];
        spriteTransforms[0] = spriteRenderers[0].transform;
        spriteTransforms[1] = spriteRenderers[2].transform;

        spriteOffset = spriteTransforms[0].localPosition;

        if (currentWeapon == null)
            currentWeapon = defaultWeapon;
        else
        {
            currentWeapon.transform.parent = handTransform;
        }
        currentWeapon.SetTags(tag);

        aiCanAttack = true;
    }

    void Update()
    {
        
        if (!(dead = healthController.dead))
        {
            if(aiCanAttack)
                AttackInput();
            if (currentWeapon.canAttack)
            {
                PickupWeapon();
            }
        }
        else
        {
            if (currentWeapon != defaultWeapon)
                ThrowWeapon();
        }

        playerMovement.FlipSprite(spriteTransforms[0], spriteOffset);
        playerMovement.FlipSprite(spriteTransforms[1], spriteOffset);
        playerMovement.AnimateCharacter(animators[0], currentWeapon.canAttack);
        playerMovement.AnimateCharacter(animators[1], currentWeapon.canAttack);
    }

    void AttackInput()
    {
        string anim = "";
        float dis = Vector3.Distance(playerPosition.position, transform.position);
        if (dis < currentWeapon.attacks[0].range)
        {
            anim = currentWeapon.Attack(0, GetTargetPosition());
            StartCoroutine(AttackCooldown());
        }

        if (!anim.Equals(""))
        {
            animators[0].Play(anim);
            animators[1].Play(anim);
        }
    }

    void PickupWeapon()
    {
        if (currentWeapon == defaultWeapon)
        {
            GameObject go = ClosestWeapon();
            if(go != null)
            {
                currentWeapon = go.GetComponent<PickUpWeapon>().PickUp(handTransform, playerMovement);
                currentWeapon.transform.localPosition = Vector3.zero;
                currentWeapon.transform.localRotation = Quaternion.identity;
                currentWeapon.transform.localScale = Vector3.one;
                currentWeapon.SetTags(tag);
            }
        }
    }

    GameObject ClosestWeapon()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("PickUps_W");

        GameObject rtn = null;
        float closestDis = maxPickUpRange;

        Vector3 pos = transform.position;

        foreach (GameObject go in gos)
        {
            float dis = Vector3.Distance(pos, go.transform.position);
            if (dis < closestDis)
            {
                closestDis = dis;
                rtn = go;
            }
        }

        return rtn;
    }

    void ThrowWeapon()
    {
        PickUpWeapon temp = Instantiate(thrownWeapon, transform.position, Quaternion.identity);
        temp.MyStart(currentWeapon, playerMovement.FaceDir.x);
        currentWeapon.transform.parent = temp.transform;
        currentWeapon = defaultWeapon;
    }

    Vector3 GetTargetPosition()
    {
        Vector3 rtn = playerPosition.position + (Vector3)Random.insideUnitCircle * 1.5f;
        return rtn;
    }

    IEnumerator AttackCooldown()
    {
        aiCanAttack = false;
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        aiCanAttack = true;
    }
}

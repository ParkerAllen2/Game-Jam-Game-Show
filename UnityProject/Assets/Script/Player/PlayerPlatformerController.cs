using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPlatformerController : MonoBehaviour
{
    private PlatformerMovement playerMovement;
    HealthController healthController;

    Animator[] animators;
    SpriteRenderer[] spriteRenderers;
    Transform[] spriteTransforms;
    Vector3 spriteOffset;

    public Transform handTransform;
    public Weapon currentWeapon;
    public Weapon defaultWeapon;

    public PickUpWeapon thrownWeapon;
    public float maxPickUpRange = 2;

    Vector3 MOUSEADJUST = Vector3.up * -1f;

    void Start()
    {
        playerMovement = GetComponentInParent<PlatformerMovement>();
        healthController = GetComponentInParent<HealthController>();

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
            currentWeapon.SetTags(tag);
            currentWeapon.transform.parent = handTransform;
        }
        defaultWeapon.SetTags(tag);
    }

    void Update()
    {
        if(!healthController.dead)
        {
            GetMovementInput();
            if(!EventSystem.current.IsPointerOverGameObject())
                AttackInput();
            if (currentWeapon.canAttack)
            {
                PickupWeapon();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerMovement.WalkInput = false;
            }
        }
        else
        {
            playerMovement.DirectionalInput = Vector3.zero;
        }

        playerMovement.FlipSprite(spriteTransforms[0], spriteOffset);
        playerMovement.FlipSprite(spriteTransforms[1], spriteOffset);
        playerMovement.AnimateCharacter(animators[0], currentWeapon.canAttack);
        playerMovement.AnimateCharacter(animators[1], currentWeapon.canAttack);
    }

    private void GetMovementInput()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerMovement.DirectionalInput = directionalInput;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.GetCharacterMovement().collideWithPlatforms = true;
            playerMovement.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerMovement.GetCharacterMovement().collideWithPlatforms = false;
            playerMovement.OnJumpInputUp();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerMovement.WalkInput = true;
        }
    }

    void AttackInput()
    {
        string anim = "";
        if (Input.GetMouseButton(0))
        {
            anim = currentWeapon.Attack(0, GetMousePosition());
        }
        else if (Input.GetMouseButton(1))
        {
            anim = currentWeapon.Attack(1, GetMousePosition());
        }
        else if (Input.GetKey(KeyCode.E))
        {
            anim = currentWeapon.Attack(2, GetMousePosition());
        }

        if (!anim.Equals(""))
        {
            animators[0].Play(anim);
            animators[1].Play(anim);
        }
    }

    void PickupWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentWeapon == defaultWeapon)
            {
                GameObject go = ClosestWeapon();
                if (go != null)
                {
                    currentWeapon = go.GetComponent<PickUpWeapon>().PickUp(handTransform, playerMovement);
                    currentWeapon.transform.localPosition = Vector3.zero;
                    currentWeapon.transform.localRotation = Quaternion.identity;
                    currentWeapon.transform.localScale = Vector3.one;
                    currentWeapon.SetTags(tag);
                }
            }
            else
            {
                ThrowWeapon();
            }
        }
    }

    GameObject ClosestWeapon()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("PickUps_W");

        GameObject rtn = null;
        float closestDis = maxPickUpRange;

        Vector3 pos = transform.position;

        foreach(GameObject go in gos)
        {
            float dis = Vector3.Distance(pos, go.transform.position);
            if(dis < closestDis)
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

    Vector3 GetMousePosition()
    {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        camPos.z = 0;
        return camPos + MOUSEADJUST;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    Rigidbody2D rb;
    public Weapon weapon;
    SpriteRenderer sprite;
    public float force;

    public void Start()
    {
        MyStart(weapon, 0);
    }

    public void MyStart(Weapon w, float dir)
    {
        sprite = GetComponent<SpriteRenderer>();

        weapon = w;
        sprite.sprite = weapon.pickUpSprite;
        weapon.dropped = true;
        weapon.gameObject.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector3.right * dir * force);
    }

    public Weapon PickUp(Transform parent, PlatformerMovement pm)
    {
        weapon.transform.parent = parent;
        weapon.dropped = false;
        weapon.gameObject.SetActive(true);
        weapon.platformerMovement = pm;

        Destroy(this.gameObject);
        return weapon;
    }
}

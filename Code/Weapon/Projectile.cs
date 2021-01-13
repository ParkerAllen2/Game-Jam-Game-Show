using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    int damage;
    float speed;
    float time;

    bool destroyOnHit;
    bool triggerImmune;
    public string tagToHit;

    [SerializeField] LayerMask collisionMask;

    public FakeProjectile fake;

    Vector3 up;
    Vector3 pos;
    Transform cacheTransform;
    Vector3 velocity;

    public void Start()
    {
        cacheTransform = transform;
        up = cacheTransform.right;
        pos = cacheTransform.position;
        StartCoroutine(SelfDestruct());
        velocity = speed * up;
    }

    public void SetValues(int d, float s, float t, bool dh, bool ti, LayerMask l, string tag)
    {
        damage = d;
        speed = s;
        time = t;
        destroyOnHit = dh;
        triggerImmune = ti;
        collisionMask = l;
        tagToHit = tag;
    }

    public void FixedUpdate()
    {
        Move();
        cacheTransform.position = pos;
    }

    private void Move()
    {
        Vector3 v = velocity * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(pos, up, v.magnitude, collisionMask);

        if (hit && !hit.transform.CompareTag("Platform"))
        {
            if (hit.collider.CompareTag(tagToHit))
            {
                HealthController  hc= hit.collider.GetComponent<HealthController>();
                hc.TakeDamage(-damage, triggerImmune);
                if (destroyOnHit)
                {
                    FakeProjectile temp = Instantiate(fake, transform.position, transform.rotation);
                    temp.SetValues(speed, GetComponent<SpriteRenderer>().sprite);
                    Destroy(this.gameObject);
                }
            }
            else if(hit.collider.CompareTag("Ground"))
            {
                FakeProjectile temp = Instantiate(fake, transform.position, transform.rotation);
                temp.SetValues(GetComponent<SpriteRenderer>().sprite, hit.point);
                Destroy(this.gameObject);
            }
        }
        pos += v;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}

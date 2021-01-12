using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem hitParticles;

    [SerializeField] float speed;
    [SerializeField] float dist;

    public string tagToHit;
    [SerializeField] LayerMask collisionMask;

    Vector3 up;
    Vector3 pos;
    Transform cacheTransform;
    Vector3 velocity;

    bool hit;

    public void Start()
    {
        cacheTransform = transform;
        up = cacheTransform.right;
        pos = cacheTransform.position;
        StartCoroutine(SelfDestruct());
        velocity = speed * up;
    }

    public void FixedUpdate()
    {
        if (!hit)
        {
            Move();
            cacheTransform.position = pos;
        }
    }

    private void Move()
    {
        Vector3 v = velocity * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(pos, up, v.magnitude, collisionMask);

        if (hit && !hit.transform.CompareTag("Platform"))
        {
            if (hit.collider.CompareTag(tagToHit) || hit.collider.CompareTag("Ground"))
            {
                transform.position = hit.point;
                StopCoroutine(SelfDestruct());
                StartCoroutine(BlowUp());
            }
        }
        pos += v;
    }

    IEnumerator BlowUp()
    {
        hitParticles.Play();
        hit = true;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    IEnumerator SelfDestruct()
    {
        float t = dist / speed;
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}

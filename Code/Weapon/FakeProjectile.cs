using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeProjectile : MonoBehaviour
{
    float speed;
    public float time;

    [SerializeField] LayerMask collisionMask;
    bool stop;

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

    public void SetValues(float s, Sprite sp)
    {
        speed = s;
        GetComponent<SpriteRenderer>().sprite = sp;
    }

    public void SetValues(Sprite sp, Vector2 pos)
    {
        GetComponent<SpriteRenderer>().sprite = sp;
        Stop(pos);
    }

    public void FixedUpdate()
    {
        if(!stop)
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
            Stop(hit.point);
        }
        pos += v;
    }

    public void Stop(Vector2 pos)
    {
        transform.position = pos;
        stop = true;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

    public void OnDestroy()
    {

    }
}

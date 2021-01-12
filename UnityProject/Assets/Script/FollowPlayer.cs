using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offset = new Vector3(0,0,0);
    public float minY, maxY, minX, maxX;

    void Update()
    {
        Vector3 temp = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        temp.y = Mathf.Clamp(temp.y, minY, maxY);
        temp.x = Mathf.Clamp(temp.x, minX, maxX);
        this.transform.position = temp;
    }
}

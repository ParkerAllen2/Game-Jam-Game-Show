using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject heartPrefab;
    GameObject[] hearts;

    public Transform heartParent;

    public int maxNumHearts = 3;
    int numHearts;

    public void AddHearts(int h)
    {
        numHearts = maxNumHearts = h;
        hearts = new GameObject[maxNumHearts];
        for (int i = 0; i < maxNumHearts; i++)
        {
            hearts[i] = Instantiate(heartPrefab, heartParent);
        }
    }

    //return true if dead
    public bool SetHealth(int h)
    {
        numHearts += h;
        numHearts = Mathf.Min(numHearts, maxNumHearts);
        for (int i = 0; i < maxNumHearts; i++)
        {
            hearts[i].SetActive(i < numHearts);
        }
        return numHearts <= 0;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class EnemyGroups : MonoBehaviour
{
    public List<GameObject> Group = new List<GameObject>();
    public void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Group.Add(transform.GetChild(i).gameObject);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : MonoBehaviour
{
    private GameObject[] spawns;

    void Start()
    {
        spawns = new GameObject[transform.childCount];

        for (int i = 0; i < spawns.Length; i++)
        {
            spawns[i] = transform.GetChild(i).gameObject;
        }
    }

    public GameObject getSpawn(string prevScene)
    {
        foreach (GameObject spawn in spawns)
        {
            if (spawn.name == prevScene)
            {
                return spawn;
            }
        }
        return null;
    }
}

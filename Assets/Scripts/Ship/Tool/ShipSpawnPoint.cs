using System;
using UnityEngine;

public class ShipSpawnPoint : MonoBehaviour
{
    public GameObject ShipPrefab;

    public void Start()
    {
        Instantiate(ShipPrefab, transform);
    }
}
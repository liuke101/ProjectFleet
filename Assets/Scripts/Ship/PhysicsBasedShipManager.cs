using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PhysicsBasedShipManager : MonoSingleton<PhysicsBasedShipManager>
{
    public List<ShipInfo> ShipInfos;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDestroy()
    {
        ShipInfos.Clear();
    }

    public void DeactiveAllShips()
    {
        PhysicsBasedShipController[] shipController  = GameObject.FindObjectsOfType<PhysicsBasedShipController>();
        foreach (var sc in shipController)
        {
            sc.enabled = false;
        }
    }
    
    public ShipInfo GetShipInfo(int id)
    {
        return ShipInfos.Find(x => x.ID == id);
    }
}

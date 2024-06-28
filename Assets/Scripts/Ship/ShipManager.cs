using System;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Events;

public class ShipManager : MonoSingleton<ShipManager>
{
    public List<ShipInfo> ShipInfos;
    
    //注册船只事件
    public UnityEvent<ShipController> OnShipInfoRegister;
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
        ShipController[] shipController  = GameObject.FindObjectsOfType<ShipController>();
        foreach (var sc in shipController)
        {
            sc.enabled = false;
        }
    }
    
    public void RegisterShipInfo(ShipController shipController)
    {
        //注册到Manager
        ShipInfos.Add(shipController.shipInfo);
        
        //广播委托
        OnShipInfoRegister.Invoke(shipController);
    }
    
    public ShipInfo GetShipInfo(int id)
    {
        return ShipInfos.Find(x => x.ID == id);
    }
}

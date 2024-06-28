using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vbertz.PBSC;

public class ShipInfoPanel : MonoBehaviour
{
    private VerticalLayoutGroup VerticalLayout;
    
    public List<ShipController> ShipControllers;
    
    public ShipInfoText ShipInfoTextPrefab;
    private List<ShipInfoText> ShipInfoTexts = new List<ShipInfoText>();
    private void Awake()
    {
        VerticalLayout = GetComponent<VerticalLayoutGroup>();
    }

    
    //OnShipInfoRegister事件回调
    public void InitShipInfo(ShipController shipController)
    {
        if (ShipControllers.Contains(shipController)) return;
            
        //收集
        ShipControllers.Add(shipController);
        
        //实例化ShipInfo
        ShipInfoText info = Instantiate(ShipInfoTextPrefab, transform);
        info.ID = shipController.shipInfo.ID;
        info.SetID(shipController.shipInfo.ID);
        ShipInfoTexts.Add(info);
        
        //加入VerticalLayout
        info.transform.SetParent(VerticalLayout.transform);
        
        //监听速度变化
        shipController.OnChangeForwardVelocity.AddListener((speed) =>
        {
            info.SetForwardSpeed(speed);
        });
            
        shipController.OnChangeTurnVelocity.AddListener((speed) =>
        {
            info.SetTurnSpeed(speed);
        });
    }
    
    public void UpdateSelectedShipInfos(List<GameObject> ships)
    {
        //清除所有旧信息
        for (int i = 0; i < this.transform.childCount; i++) 
        {
            //恢复原色
            this.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        }
        
        //更新选中船只对应ID的颜色
        foreach (var ship in ships)
        {
            ShipController shipController = ship.GetComponent<ShipController>();
            if (shipController)
            {
                ShipInfoText info = GetShipInfoText(shipController.shipInfo.ID);
                if (info)
                {
                    info.GetComponent<Image>().color = shipController.shipInfo.HighlightColor;
                }
            }
        }
    }
    
    public ShipInfoText GetShipInfoText(int id)
    {
        return ShipInfoTexts.Find(x => x.ID == id);
    }
}
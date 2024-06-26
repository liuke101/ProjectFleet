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
    
    public List<PhysicsBasedShipController> ShipControllers;
    
    public ShipInfoText ShipInfoTextPrefab;
    private List<ShipInfoText> ShipInfoTexts = new List<ShipInfoText>();
    private void Awake()
    {
        VerticalLayout = GetComponent<VerticalLayoutGroup>();
    }

    private void Start()
    {
        InitShipInfos();
    }
    
    private void InitShipInfos()
    {
        ShipControllers = new List<PhysicsBasedShipController>(GameObject.FindObjectsOfType<PhysicsBasedShipController>());
        
        foreach (var shipController in ShipControllers)
        {
            //实例化ShipInfo
            ShipInfoText info = Instantiate(ShipInfoTextPrefab, transform);
            info.ID = shipController.shipInfo.ID;
            ShipInfoTexts.Add(info);
            
            //加入VerticalLayout
            info.transform.SetParent(VerticalLayout.transform);
            
            //设置船名UI
            info.SetName(shipController.shipInfo.ShipName);
                
            //监听速度变化
            shipController.m_onChangeForwardVelocity.AddListener((speed) =>
            {
                info.SetForwardSpeed(speed);
            });
                
            shipController.m_onChangeTurnVelocity.AddListener((speed) =>
            {
                info.SetTurnSpeed(speed);
            });
        }
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
            PhysicsBasedShipController shipController = ship.GetComponent<PhysicsBasedShipController>();
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
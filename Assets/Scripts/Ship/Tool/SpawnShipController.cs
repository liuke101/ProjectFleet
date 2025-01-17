﻿using System;
using JsonStruct;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SpawnShipController : MonoSingleton<SpawnShipController>
{
    public GameObject ShipPrefab;
    
    public UnityEvent<float> OnAgentMaxForwardSpeedChanged;
    public UnityEvent<float> OnAgentMaxAngularSpeedChanged;
    
    public void Start()
    {
        
    }

    public void SpawnShip(ShipWebSocketData data)
    {
        //GameObject Ship = null;
        
        //如果不存存在该船只，则创建并初始化
        if(!ShipManager.Instance.ShipInfos.Exists(x => x.ID == (int)data.ship_id))
        {
            GameObject Ship = Instantiate(ShipPrefab, new Vector3((float)data.x_coordinate, 0, (float)data.y_coordinate),
                Quaternion.identity);

            ShipController shipController = Ship.GetComponent<ShipController>();
            if(shipController)
            {
                shipController.InitShipInfo((int)data.ship_id);
            }
        }
        else //如果已存在则进行位置控制
        {
            ShipNavController shipNavController = ShipManager.Instance.GetShipController((int)data.ship_id).GetComponent<ShipNavController>();
            
            if (shipNavController)
            {
                //设置速度
                shipNavController.SetNavMaxSpeed((float)data.speed);
            
                //广播速度信息到UI
                OnAgentMaxForwardSpeedChanged?.Invoke((float)data.speed);
                OnAgentMaxAngularSpeedChanged?.Invoke(shipNavController.Agent.angularSpeed); //角速度 目前是写死的，在NavMeshAgent中设置
                
                //规定99999时，为转向操作（目的地数据无效）
                if (data.des_x_coordinate >= 90000.0 && data.des_y_coordinate >= 90000.0)
                {
                    shipNavController.TurnTo((float)data.heading);
                }
                else //否则就是有目的地移动（heading数据无效）
                {
                    shipNavController.MoveTo(new Vector3((float)data.des_x_coordinate, 0, (float)data.des_y_coordinate));
                }
            }
        }
    }
}
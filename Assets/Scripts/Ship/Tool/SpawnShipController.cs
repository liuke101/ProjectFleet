using System;
using JsonStruct;
using UnityEngine;

public class SpawnShipController : MonoSingleton<SpawnShipController>
{
    public GameObject ShipPrefab;
    
    public void Start()
    {
        
    }

    public void SpawnShip(ShipWebSocketData data)
    {
        GameObject Ship = null;
        
        //如果不存存在该船只，则创建并初始化
        if(!ShipManager.Instance.ShipInfos.Exists(x => x.ID == data.ship_id))
        {
            Ship = Instantiate(ShipPrefab, new Vector3((float)data.x_coordinate, 0, (float)data.y_coordinate),
                Quaternion.identity);

            ShipController shipController = Ship.GetComponent<ShipController>();
            if(shipController)
            {
                shipController.InitShipInfo(data.ship_id);
            }
        }
        
        //位置控制
        if (Ship != null)
        {
            ShipNavController shipNavController = Ship.GetComponent<ShipNavController>();
            if (shipNavController)
            {
                //获取速度
                shipNavController.Agent.speed = (float)data.speed;
            
                //规定99999时，为转向操作（目的地数据无效）
                if (data.des_x_coordinate >= 99990.0 && data.des_y_coordinate >= 99990.0)
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
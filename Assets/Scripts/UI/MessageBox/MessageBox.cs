using JsonStruct;
using TMPro;
using UnityEngine;

public class MessageBox : MonoSingleton<MessageBox>
{
    public TMP_Text _textMeshPro;
    
    public void PrintShipWebSocketData(ShipWebSocketData data)
    {
        PrintMessage($"接收到船只数据! ID:{data.ship_id}; 初始坐标:({data.x_coordinate:F3},{data.y_coordinate:F3}); 目的地:({data.des_x_coordinate:F3},{data.des_y_coordinate:F3})");
    }

    public void PrintMessage(string message)
    {
        if (_textMeshPro)
        {
            //换行
            _textMeshPro.text += "\n";
                
            _textMeshPro.text += $"<color=blue>{System.DateTime.Now} </color> <color=black>{message}</color>";
            //设置字体颜色
            _textMeshPro.color = Color.red;
            
            //todo:清理多余的行
        }
    }
}
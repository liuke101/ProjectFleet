using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipInfoText : MonoBehaviour
{
    public int ID; //该ID和船的ID对应
    public TMP_Text ShipID;
    public TMP_Text MaxForwardSpeed;
    public TMP_Text MaxTurnSpeed;
    public TMP_Text CurrentForwardSpeed;
    
    
    public void SetID(int id)
    {
        if (ShipID != null)
        {
            ShipID.text = $"Ship ID: {id}";
        }
    }
    
    public void SetForwardSpeed(float speed)
    {
        if (MaxForwardSpeed != null)
        {
            MaxForwardSpeed.text = $"最大前进速度: {speed}";
        }
    }
    
    public void SetTurnSpeed(float speed)
    {
        if (MaxTurnSpeed != null)
        {
            MaxTurnSpeed.text = $"最大转向速度: {speed}";
        }
    }
    
    public void SetCurrentForwardSpeed(float speed)
    {
        if (CurrentForwardSpeed != null)
        {
            CurrentForwardSpeed.text = $"当前前进速度: {speed:F1}";
        }
    }
}
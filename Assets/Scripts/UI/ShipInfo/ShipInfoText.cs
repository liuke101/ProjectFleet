using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipInfoText : MonoBehaviour
{
    public int ID; //该ID和船的ID对应
    public TMP_Text ShipID;
    public TMP_Text ForwardSpeed;
    public TMP_Text TurnSpeed;
    
    
    public void SetID(int id)
    {
        if (ShipID != null)
        {
            ShipID.text = $"Ship ID: {id}";
        }
    }
    
    public void SetForwardSpeed(float speed)
    {
        if (ForwardSpeed != null)
        {
            ForwardSpeed.text = $"前进速度: {speed}";
        }
    }
    
    public void SetTurnSpeed(float speed)
    {
        if (TurnSpeed != null)
        {
            TurnSpeed.text = $"转向速度: {speed}";
        }
    }
}
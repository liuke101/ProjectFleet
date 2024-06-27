using System.Collections;
using UnityEngine;


/// <summary>
/// 船舵控制器
/// </summary>
public class RudderController : MonoBehaviour
{
    [SerializeField] public PhysicsBasedShipController shipController;
    [SerializeField] private float maxRudderAngle = 30f; // 舵可以转动的最大角度

    [SerializeField]
    private float anticipationFactor = 0.5f; // 你希望舵提前多少秒开始转动

    private float previousTurnSpeed;

    private void Update()
    {
        // 保存船的转弯速度以供下一帧使用
        float turnSpeed = previousTurnSpeed;
        previousTurnSpeed = shipController.CurrentTurnSpeed;

        // 等待
        StartCoroutine(WaitAndTurn(turnSpeed));
    }

    private IEnumerator WaitAndTurn(float turnSpeed)
    {
        yield return new WaitForSeconds(anticipationFactor);

        // 根据船的转弯速度计算舵角
        float rudderAngle = Mathf.Clamp(turnSpeed, -1, 1) * (-maxRudderAngle);

        // 沿 Y 轴旋转方向舵
        transform.localRotation = Quaternion.Euler(0, rudderAngle, 0);
    }
}

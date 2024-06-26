using UnityEngine;

public class ShipAutopilotController : MonoBehaviour
{
    
    private float TargetDistance; //距离目标的距离
    private Vector3 TargetPosition;  //目标位置
    private bool isAutopilotActive = false; //是否激活自动驾驶
    
    public float CloseEnoughDistance = 1.0f;  //足够接近时停止
    public float DecelerationDistance = 10.0f; //减速距离
    
    
    private void Awake()
    {
    }

    private void Start()
    {
    }
    
    private void Update()
    {
        //检测前方是否有障碍船
         RaycastHit hit;
         if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
         {
             if (hit.collider.gameObject.CompareTag("Ship"))
             {
                 Stop();
             }
         }
         else
         {
             StartMove(TargetPosition);
         }
        
        //如果没有激活自动驾驶，不移动
        if (!isAutopilotActive)
        {
            return;
        }
        
        Vector3 directionToTarget = (TargetPosition - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        
        //不改变y轴防止下沉
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, 0f);
        //绕y轴逐渐旋转
      
        Quaternion rotate = Quaternion.AngleAxis(targetAngle, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, 5.0f);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime);
        
        //
        // // 计算飞船当前方向与目标方向之间的角度差。
        // //从前往右0~180 从前往左0~-180
        // float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        //
        // //转向系数, sin函数关系，当angleDifference=0时，turnFactor=0；当angleDifference=180时，turnFactor=1
        // //这样可以让大角度差转向更快，小角度差转向更慢
        // float turnFactor = Mathf.Abs(Mathf.Sin(angleDifference * Mathf.Deg2Rad));
        //
        // // 如果角度差为正，则向右转；如果为负，则向左转。
        // if (angleDifference > 0)
        // {
        //     TurnRight(turnFactor);
        // }
        // else if (angleDifference < 0)
        // {
        //     TurnLeft(turnFactor);
        // }
        //
        // // 计算到目标的距离
        // float distanceToTarget = Vector3.Distance(transform.position, TargetPosition);
        // // 如果船已足够接近目标，则停止船并解除自动驾驶。
        // if (distanceToTarget <= CloseEnoughDistance)
        // {
        //     Stop(); //BUG:不容易触发
        // }
        // else if (distanceToTarget <= DecelerationDistance) // 如果=船在减速距离内，则开始减速。
        // {
        //     // 根据飞船与目标的距离计算减速因子。
        //     float decelerationFactor = distanceToTarget / DecelerationDistance;
        //
        //     // 根据减速因子调整船速
        //     Decelerate(decelerationFactor);
        // }
        // else //否则全速前进
        // {
        //     MoveForward();
        // }

    }

    public void StartMove(Vector3 targetPosition)
    {
        isAutopilotActive = true;
        TargetPosition = targetPosition;
    }

    
    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        isAutopilotActive = false; 
    }
}

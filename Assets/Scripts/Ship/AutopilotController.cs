using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class AutopilotController : MonoBehaviour
{
    public Text TargetDistanceText;
    [SerializeField] private PhysicsBasedShipController shipController;

    //NavMesh
    private NavMeshAgent Agent;

    //目标
    [SerializeField] public Vector3 TargetPositon;

    [SerializeField] private float closeEnoughDistance = 1f; //足够接近时停止
    [SerializeField] private KeyCode autopilotKey = KeyCode.P; //激活自动驾驶的按键

    [SerializeField] private float decelerationDistance = 10f; /// 船开始减速的距离desacelerar

    private bool isAutopilotActive = false; 

    private int originalForwardVelocityIndex; //初始前进速度
    private int originalTurnVelocityIndex; // 初始转向速度
    
    public float lastTurnTime;
    public float turnCooldown;
    public float detectionDistance;
    public Toggle ActivationToggle;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        //保存原始速度值
        originalForwardVelocityIndex = shipController.CurrentForwardVelocityIndex;
        originalTurnVelocityIndex = shipController.CurrentTurnVelocityIndex;
    }

    private void Update()
    {
         Debug.DrawRay(transform.position, transform.forward * detectionDistance, Color.red);
         
         //按键控制
         if (Input.GetKeyDown(autopilotKey))
         {
             isAutopilotActive = !isAutopilotActive;
             ActivationToggle.isOn = !ActivationToggle.isOn;
         }
    }

    public void AIMove(Vector3 pos)
    {
        if (isAutopilotActive)
        {
            Agent.SetDestination(pos);
        }
    }
    
    public void MoveTest()
    {
        Vector3 directionToTarget = (TargetPositon - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        
        // 计算船当前forward方向与目标方向之间的角度差。
        // 从前往右0~180 从前往左0~-180
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        
        // 如果角度差为正，则向右转；如果为负，则向左转。
        if (angleDifference == 0)
        {
            shipController.CurrentTurnSpeed = 0;
            shipController.CurrentTurnVelocityIndex = originalTurnVelocityIndex;
            shipController.MoveForward();
        }
        if (angleDifference is > 0 and < 90)
        {
            shipController.TurnRight();
            shipController.MoveForward();
        }
        else if (angleDifference is >= 90 and <= 180)
        {
            shipController.TurnLeft();
            shipController.MoveBackward();
        }
        else if (angleDifference is < 0 and > -90)
        {
            shipController.TurnLeft();
            shipController.MoveForward();
        }
        else if (angleDifference is <= -90 and >= -180)
        {
            shipController.TurnRight();
            shipController.MoveBackward();
        }
        
        //转向系数, sin函数关系，当angleDifference=0时，turnFactor=0；当angleDifference=180时，turnFactor=1
        //这样可以让大角度差转向更快，小角度差转向更慢
        //float turnFactor = Mathf.Abs(Mathf.Sin(angleDifference * Mathf.Deg2Rad));
        
        // 计算到目标的距离
        float distanceToTarget = Vector3.Distance(transform.position, TargetPositon);

        // 如果船已足够接近目标，则停止船并解除自动驾驶。
        if (distanceToTarget <= closeEnoughDistance)
        {
            shipController.MoveBackward();
            shipController.CurrentForwardVelocityIndex = originalForwardVelocityIndex;
            shipController.CurrentTurnVelocityIndex = originalTurnVelocityIndex;
            isAutopilotActive = false; 
            ActivationToggle.isOn = false;
        }
        else if (distanceToTarget <= decelerationDistance) // 如果船在减速距离内，则开始减速。
        {
            // 根据飞船与目标的距离计算减速因子。
            float decelerationFactor = distanceToTarget / decelerationDistance;

            // 根据减速因子调整船速
            shipController.CurrentForwardSpeed *= decelerationFactor;
        }
        else //否则全速前进
        {
            //shipController.MoveForward();
        }
        
        //UI
        TargetDistanceText.text = distanceToTarget.ToString("f0") + "meters To target";
        
        
        if (Time.time - lastTurnTime > turnCooldown)
        {
            // 如果角度差为正，则右转；如果为负，则左转
            if (angleDifference > 0)
            {
                shipController.TurnRight();
                lastTurnTime = Time.time;
            }
            else if (angleDifference < 0)
            {
                shipController.TurnLeft();
                lastTurnTime = Time.time;
            }
        }
    }

    public void Move()
    {
        Vector3 directionToTarget = (TargetPositon - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        
        // 计算船当前forward方向与目标方向之间的角度差。
        // 从前往右0~180 从前往左0~-180
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        print(angleDifference);
        
        // 如果角度差为正，则向右转；如果为负，则向左转。
        if (angleDifference is > 0 and < 90)
        {
            shipController.TurnRight();
        }
        else if (angleDifference is > 90 and <= 180)
        {
            shipController.MoveBackward();
        }
        else if (angleDifference is < 0 and > -90)
        {
            shipController.TurnLeft();
        }
        else if (angleDifference is < -90 and >= -180)
        {
            shipController.MoveBackward();
        }
        
        
        //转向系数, sin函数关系，当angleDifference=0时，turnFactor=0；当angleDifference=180时，turnFactor=1
        //这样可以让大角度差转向更快，小角度差转向更慢
        //float turnFactor = Mathf.Abs(Mathf.Sin(angleDifference * Mathf.Deg2Rad));

        
        // 计算到目标的距离
        float distanceToTarget = Vector3.Distance(transform.position, TargetPositon);

        // 如果船已足够接近目标，则停止船并解除自动驾驶。
        if (distanceToTarget <= closeEnoughDistance)
        {
            shipController.MoveBackward();
            shipController.CurrentForwardVelocityIndex = originalForwardVelocityIndex;
            shipController.CurrentTurnVelocityIndex = originalTurnVelocityIndex;
            isAutopilotActive = false; 
            ActivationToggle.isOn = false;
        }
        else if (distanceToTarget <= decelerationDistance) // 如果船在减速距离内，则开始减速。
        {
            // 根据飞船与目标的距离计算减速因子。
            float decelerationFactor = distanceToTarget / decelerationDistance;

            // 根据减速因子调整船速
            shipController.CurrentForwardSpeed *= decelerationFactor;
        }
        else //否则全速前进
        {
            shipController.MoveForward();
        }
        
        //UI
        TargetDistanceText.text = distanceToTarget.ToString("f0") + "meters To target";
        
        
        if (Time.time - lastTurnTime > turnCooldown)
        {
            // 如果角度差为正，则右转；如果为负，则左转
            if (angleDifference > 0)
            {
                shipController.TurnRight();
                lastTurnTime = Time.time;
            }
            else if (angleDifference < 0)
            {
                shipController.TurnLeft();
                lastTurnTime = Time.time;
            }
        }
    }
}
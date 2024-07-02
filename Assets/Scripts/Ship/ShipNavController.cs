using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// 船导航控制器
/// </summary>
public class ShipNavController : MonoBehaviour
{
    public NavMeshAgent Agent;
    
    private RaycastHit MountainHit;
    private bool isHitMountain = false; //是否检测到山
    public float PreventCollisionRayCastDistance = 50.0f; //预防碰撞检测距离
    
    public UnityEvent<float> OnAgentCurrentVelocityChanged;

    [Header("目标线")] 
    public LineRenderer TargetLine;
    
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        TargetLine = GetComponent<LineRenderer>();
        if (TargetLine)
        {
            TargetLine.positionCount = 2;//设置两点
            TargetLine.widthCurve = AnimationCurve.Linear(0, 0.1f, 1, 0.1f); //宽度
        }
    }

    private void Update()
    {
        //TODO:根据角度差、到目的地的距离调整速度（同时调整UI)
        //判断当前船方向距离Agent目的地的角度差
        // 从前往右0~180 从前往左0~-180
        // Vector3 directionToTarget = (Agent.destination - transform.position).normalized;
        // float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        // float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        //print(angleDifference);
        
        //防止与山体碰撞
        PreventCollisionWithMountains();

        //广播当前速度
        OnAgentCurrentVelocityChanged.Invoke(Agent.velocity.magnitude);
    }

    public void MoveTo(Vector3 pos)
    {
        //清空目标知识点
        BoxSelectController.Instance?.ClearTargetPoints();
        //清空指示线
        TargetLine.positionCount = 0; 
        
        if (Agent != null)
        {
            Agent.SetDestination(pos);
            BoxSelectController.Instance?.SpwanTargetPoint(pos);
            
            //设置指示线的起点和终点
            TargetLine.positionCount = 2;
            TargetLine.SetPosition(0, transform.position);
            TargetLine.SetPosition(1, pos);
        }
    }

    public void TurnTo(float angle)
    {
        //绕Y轴逐渐旋转
        //transform.rotation *= Quaternion.Euler(0, angle, 0);
        
        //计算一个位置，该位置与forward的夹角为angle，距离为不可达距离，模拟转向（因为navmesh没有很大，所以可能超过范围导致不移动);
        Vector3 targetPos = transform.position + Quaternion.Euler(0, angle, 0) * transform.forward * 200;
        
        MoveTo(targetPos);
    }
    
    private void Stop()
    {
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;
        Agent.angularSpeed = 0;
    }
    
    public void SetNavMaxSpeed(float speed)
    {
        Agent.speed = speed;
    }
    
    private void PreventCollisionWithMountains()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * PreventCollisionRayCastDistance, Color.red);
        
        //检测是否有山，有则避开
        if (isHitMountain == false)
        {
            if (Physics.Raycast(transform.position, transform.forward, out MountainHit, PreventCollisionRayCastDistance))
            {
                if (MountainHit.collider.gameObject.CompareTag("Mountain"))
                {
                    //后退一段距离
                    Agent.velocity = Vector3.zero;
                    
                    MoveTo(transform.position - transform.forward * 30);
                    isHitMountain = true;
                }
            }
        }

        //如果检测不到山了，就重新开始检测
        if (isHitMountain)
        {
            if (!Physics.Raycast(transform.position, transform.forward, out MountainHit, 30) || !MountainHit.collider.gameObject.CompareTag("Mountain"))
            {
                isHitMountain = false;
            }
        }
    }
}
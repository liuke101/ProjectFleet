using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 船导航控制器
/// </summary>
public class ShipNavController : MonoBehaviour
{
    private PhysicsBasedShipController ShipController;
    private NavMeshAgent Agent;

    private void Awake()
    {
        ShipController = GetComponent<PhysicsBasedShipController>();
        Agent = GetComponent<NavMeshAgent>();
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
        
        
        //如果到达目标就停止
        if (Agent.isStopped)
        {
            
        }
        
    }

    public void MoveTo(Vector3 pos)
    {
        Agent?.SetDestination(pos);
    }
}
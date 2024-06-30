using System;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//TODO:框选后给命令
public class BoxSelectController : MonoSingleton<BoxSelectController>
{
    [Header("LineRenderer")]
    public LineRenderer lineRenderer;
    public float lineDepth = 5.0f; //绘制线框的深度
    public float lineWidth = 0.1f; //线框的宽度
   
    [FormerlySerializedAs("BoxSelectkey")] [Header("键位")] 
    public KeyCode BoxSelectKey = KeyCode.Mouse2;
    public KeyCode EndSelectKey = KeyCode.Escape;
    public KeyCode MoveKey = KeyCode.Mouse0;
    
    private bool isMouseDown = false;
    
    //鼠标框选的四个点
    private Vector3 leftUpPoint; 
    private Vector3 rightUpPoint;
    private Vector3 rightDownPoint;
    private Vector3 leftDownPoint;
    
    //上一次鼠标点击的位置
    private Vector3 frontMousePos = Vector3.zero;
    
    //射线检测
    private RaycastHit hitInfo;
    private Vector3 rayBeginWorldPos;
    
    [HideInInspector]public List<GameObject> selectedShips = new List<GameObject>();
    
    //船只间隔距离
    public float IntervalDistance = 20.0f;
    
    [Header("事件")]
    public UnityEvent<List<GameObject>> OnSelected = new UnityEvent<List<GameObject>>();

    protected override void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        if (lineRenderer)
        {
            lineRenderer.loop = true; //设置线框为闭合的
            lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, lineWidth); //设置线框的宽度    
        }
    }

    void Update()
    {
        //鼠标中键框选
        if(Input.GetKeyDown(BoxSelectKey))
        {
            leftUpPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, lineDepth);
            isMouseDown = true;
            
            if (Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out hitInfo,
                    2000,
                    1 << LayerMask.NameToLayer("Water")))
            {
                rayBeginWorldPos = hitInfo.point;
            }
        }
        else if(Input.GetKeyUp(BoxSelectKey))  //鼠标中键抬起
        {
            isMouseDown = false;
            //将线框的点数设置为0,就不会绘制线框
            lineRenderer.positionCount = 0;

            //每次选择士兵，清空上次选择
            frontMousePos = Vector3.zero;

            //框选对象
            BoxSelect();
        }
        
        //鼠标左键处于按下状态时，绘制线框
        if (isMouseDown)
        {
            DrawBoxLine();
        }
        
        //左键点击移动
        if(Input.GetKeyDown(MoveKey))
        {
            MoveTo();
        }

        
        //清空上次的选择
        if (Input.GetKeyDown(EndSelectKey))
        {
            foreach (var ship in selectedShips)
            {
                ship.GetComponent<HighlightEffect>().SetHighlighted(false);
            }
            selectedShips.Clear();
        }
    }

    private void DrawBoxLine()
    {
        //屏幕坐标
        rightUpPoint = new Vector3(rightDownPoint.x, leftUpPoint.y, lineDepth);
        rightDownPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, lineDepth);
        leftDownPoint = new Vector3(leftUpPoint.x, rightDownPoint.y, lineDepth);

        //屏幕坐标转换为世界坐标
        lineRenderer.positionCount = 4;
        if (Camera.main != null)
        {
            lineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(leftUpPoint));
            lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(rightUpPoint));
            lineRenderer.SetPosition(2, Camera.main.ScreenToWorldPoint(rightDownPoint));
            lineRenderer.SetPosition(3, Camera.main.ScreenToWorldPoint(leftDownPoint));
        }
    }

    private void BoxSelect()
    {
        //框选对象
        if(Physics.Raycast(
               Camera.main.ScreenPointToRay(Input.mousePosition), 
               out hitInfo, 
               2000, 
               1<<LayerMask.NameToLayer("Water"))) //注意设置水体的layer
        {
            Vector3 RayEndWorldPos = hitInfo.point;
                
            //中心点
            Vector3 Center = new Vector3((rayBeginWorldPos.x + RayEndWorldPos.x) / 2, 0, (rayBeginWorldPos.z + RayEndWorldPos.z) / 2);   
                
            //长宽高的一半
            Vector3 HalfExtents = new Vector3(Mathf.Abs(hitInfo.point.x - rayBeginWorldPos.x) / 2, 10, Mathf.Abs(hitInfo.point.z - rayBeginWorldPos.z) / 2);
                
            //检测盒
            Collider[] colliders = Physics.OverlapBox(Center, HalfExtents);
            
            //关闭对所有船只的控制，只恢复选中的船只
            ShipManager.Instance.DeactiveAllShips();
            
            foreach (var tmp in colliders)
            {
                ShipController shipController = tmp.gameObject.GetComponent<ShipController>();
                if(shipController)
                {
                    if(selectedShips.Contains(tmp.gameObject)) continue;
                    
                    selectedShips.Add(tmp.gameObject);

                    //恢复控制
                    shipController.enabled = true;

                    //高亮
                    HighlightEffect HighlightShip = tmp.gameObject.GetComponent<HighlightEffect>();
                    if(HighlightShip)
                    {
                     HighlightShip.SetHighlighted(true);
                    }
                }
            }

             //广播
             if (selectedShips.Count > 0)
             {
                 OnSelected?.Invoke(selectedShips);
             }
        }
    }
    
    public void MoveTo()
    {
        if (selectedShips.Count <= 0) return;

        //目标位置
        if (Physics.Raycast(
                Camera.main.ScreenPointToRay(Input.mousePosition),
                out hitInfo,
                2000,
                1 << LayerMask.NameToLayer("Water")))
        {
            //计算阵列目标点
            List<Vector3> TargetPositions = GetTargetPositions(hitInfo.point);
            
            for(int i = 0; i<selectedShips.Count; i++)
            {
                ShipNavController navController = selectedShips[i].GetComponent<ShipNavController>();
                if (navController)
                {
                    //Debug.DrawLine(TargetPositions[i], TargetPositions[i] + Vector3.up * 100, Color.red, 100.0f);
                    //船朝向阵列目标移动
                    navController.MoveTo(TargetPositions[i]);
                }
            }
        }
    }

    /// <summary>
    /// 在目标位置周围生成多个目标点
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private List<Vector3> GetTargetPositions(Vector3 targetPos)
    {
        //需要计算目标点的面朝向和右朝向
        Vector3 nowForward = Vector3.zero;
        Vector3 nowRight = Vector3.zero;

        //如果已经移动过
        if (frontMousePos != Vector3.zero)
        {
            //有上一次的点，就直接计算
            nowForward = (targetPos - frontMousePos).normalized;
        }
        else
        {
            //没有就用第一个士兵的位置
            nowForward = (targetPos - selectedShips[0].transform.position).normalized;
        }
        
        //绕Y轴旋转90度得到右朝向
        nowRight = Quaternion.Euler(0, 90, 0) * nowForward;

        List<Vector3> targetPositions = new List<Vector3>();
        
        //有几个士兵就生成几个目标点, 数组下标一一对应
        //只控制了位置，TODO:控制朝向？
        switch (selectedShips?.Count)
        {
        case 1:
            targetPositions.Add(targetPos);
            break;
        case 2:
            targetPositions.Add(targetPos + nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowRight * IntervalDistance/2);
            break;
        case 3:
            targetPositions.Add(targetPos);
            targetPositions.Add(targetPos + nowRight * IntervalDistance);
            targetPositions.Add(targetPos - nowRight * IntervalDistance);
            break;
        case 4: 
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2 - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2 + nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance / 2 - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance / 2 + nowRight * IntervalDistance/2);
            break;
        case 5:
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2);
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2 - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2 + nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance / 2 - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance / 2 + nowRight * IntervalDistance/2);
            break;
        case 6:
            targetPositions.Add(targetPos + nowForward * IntervalDistance / 2);
            targetPositions.Add(targetPos + nowForward * IntervalDistance - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos + nowForward * IntervalDistance + nowRight * IntervalDistance/2);
            
            targetPositions.Add(targetPos - nowForward * IntervalDistance - nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance + nowRight * IntervalDistance/2);
            targetPositions.Add(targetPos - nowForward * IntervalDistance / 2);
            break;
        case 7:
            targetPositions.Add(targetPos + nowForward * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance + nowRight * IntervalDistance);
            targetPositions.Add(targetPos - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowRight * IntervalDistance);
            targetPositions.Add(targetPos);
            targetPositions.Add(targetPos - nowForward * IntervalDistance);
            break;
        case 8:
            targetPositions.Add(targetPos + nowForward * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance + nowRight * IntervalDistance);
            
            targetPositions.Add(targetPos);
            targetPositions.Add(targetPos - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowRight * IntervalDistance);
            
            targetPositions.Add(targetPos - nowForward * IntervalDistance - nowRight * IntervalDistance);
            targetPositions.Add(targetPos - nowForward * IntervalDistance + nowRight * IntervalDistance);
            break;
        case 9:
            targetPositions.Add(targetPos + nowForward * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowForward * IntervalDistance + nowRight * IntervalDistance);
            
            targetPositions.Add(targetPos);
            targetPositions.Add(targetPos - nowRight * IntervalDistance);
            targetPositions.Add(targetPos + nowRight * IntervalDistance);

            targetPositions.Add(targetPos - nowForward * IntervalDistance);
            targetPositions.Add(targetPos - nowForward * IntervalDistance - nowRight * IntervalDistance);
            targetPositions.Add(targetPos - nowForward * IntervalDistance + nowRight * IntervalDistance);
            break;
        //略。。。。
        default:    
            break;
        }
        
        //记录当前次位置
        frontMousePos = targetPos;

        return targetPositions;
    }
}

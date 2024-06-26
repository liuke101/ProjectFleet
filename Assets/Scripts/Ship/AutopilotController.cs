using UnityEngine;
using UnityEngine.UI;


namespace Vbertz.PBSC
{
    public class AutopilotController : MonoBehaviour
    {
        public Text TargetDistanceText;
        [SerializeField] private PhysicsBasedShipController shipController;

        //目标
        [SerializeField] private Transform target;
        [SerializeField] public Vector3 TargetPositon;

        [SerializeField] private float closeEnoughDistance = 1f; //足够接近时停止
        [SerializeField] private KeyCode autopilotKey = KeyCode.P; //激活自动驾驶的按键

        [SerializeField] private float decelerationDistance = 10f; /// 船开始减速的距离desacelerar

        private bool isAutopilotActive = false; 

        private int originalForwardVelocity; //初始前进速度
        private int originalTurnVelocity; // 初始转向速度
        
        public float lastTurnTime;
        public float turnCooldown;
        public float detectionDistance;
        public Toggle ActivationToggle;

        private void Start()
        {
            //保存原始速度值
            originalForwardVelocity = shipController.CurrentForwardVelocityIndex;
            originalTurnVelocity = shipController.CurrentTurnVelocityIndex;
        }

        private void Update()
        {
            //检测前方是否有障碍
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
            {
                if (hit.collider.gameObject.CompareTag("Ship"))
                {
                    shipController.TurnRight();
                }
            }

            // 按键控制
            if (Input.GetKeyDown(autopilotKey))
            {
                isAutopilotActive = !isAutopilotActive;
                ActivationToggle.isOn = !ActivationToggle.isOn;
            }
            
            if (!isAutopilotActive)
            {
                return;
            }

            Vector3 directionToTarget = (TargetPositon - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

            // 计算船当前方向与目标方向之间的角度差。
            // 从前往右0~180 从前往左0~-180
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

            //转向系数, sin函数关系，当angleDifference=0时，turnFactor=0；当angleDifference=180时，turnFactor=1
            //这样可以让大角度差转向更快，小角度差转向更慢
            //float turnFactor = Mathf.Abs(Mathf.Sin(angleDifference * Mathf.Deg2Rad));
            
            // // 如果角度差为正，则向右转；如果为负，则向左转。
            if (angleDifference > 0)
            {
                shipController.TurnRight();
            }
            else if (angleDifference < 0)
            {
                shipController.TurnLeft();
            }
            else
            {
                shipController.CurrentTurnSpeed = 0;
            }

            // 计算到目标的距离
            float distanceToTarget = Vector3.Distance(transform.position, TargetPositon);

            // 如果船已足够接近目标，则停止船并解除自动驾驶。
            if (distanceToTarget <= closeEnoughDistance)
            {
                shipController.MoveBackward();
                shipController.CurrentForwardVelocityIndex = originalForwardVelocity;
                shipController.CurrentTurnVelocityIndex = originalTurnVelocity;
                isAutopilotActive = false; // Desactiva el piloto automático
                ActivationToggle.isOn = false;
            }
            else if (distanceToTarget <=
                     decelerationDistance) // 如果船在减速距离内，则开始减速。
            {
                // 根据飞船与目标的距离计算减速因子。
                float decelerationFactor = distanceToTarget / decelerationDistance;

                // 根据减速因子调整船速
                shipController.CurrentSpeed *= decelerationFactor;
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
}

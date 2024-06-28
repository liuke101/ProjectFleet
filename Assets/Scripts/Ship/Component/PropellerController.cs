using UnityEngine;

/// <summary>
/// 螺旋桨控制器
/// </summary>
public class PropellerController : MonoBehaviour
{
	[SerializeField] private ShipController shipController;

	[SerializeField] private bool isRightPropeller;
	public float PropellerSpeed = 0f;
	float rotationSpeed;

	private void Update()
	{

		float speed = shipController.CurrentForwardSpeed;
		float turnSpeed = shipController.CurrentTurnSpeed;
		rotationSpeed = speed * PropellerSpeed;

		// 如果船正在向前移动或转向，则打开螺旋桨
		if (speed > 0 || (isRightPropeller && turnSpeed > 0) || (!isRightPropeller && turnSpeed < 0))
		{
			if (turnSpeed > 0 && !isRightPropeller)
			{
				RotatePropeller(speed / PropellerSpeed);
			}
			else
			{
				RotatePropeller(speed);
			}
		}
		// 如果船反向行驶，螺旋桨就会反向旋转
		else if (speed < 0)
		{
			RotatePropeller(-speed);
		}
	}

	private void RotatePropeller(float speed)
	{
		// 根据需要调整旋转速度
		transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
	}
}

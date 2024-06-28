using UnityEngine;

/// <summary>
/// 倾斜控制器
/// </summary>
public class BoatTiltController : MonoBehaviour
{
	[SerializeField] private ShipController shipController;
	public Transform ShipRerefence;

	[SerializeField] private float maxTiltAngle = 5f;

	[SerializeField] private float tiltSmoothness = 1f;

	private float currentTiltAngle;

	private void Update()
	{
		float turnSpeed = shipController.CurrentTurnSpeed;

		//根据船的转弯速度计算所需的倾斜角度
		float desiredTiltAngle = Mathf.Clamp(turnSpeed, -1, 1) * maxTiltAngle;

		// 在当前倾斜角度和所需倾斜角度之间平滑插值
		currentTiltAngle = Mathf.LerpAngle(currentTiltAngle, desiredTiltAngle, Time.deltaTime * tiltSmoothness);

		transform.localRotation = Quaternion.Euler(0, 0, currentTiltAngle);
	}
}
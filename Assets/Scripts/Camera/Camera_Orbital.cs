﻿using UnityEngine;

namespace Vbertz.PBSC
{
	public class Camera_Orbital : MonoBehaviour
	{
		public Transform target;
		public float distance = 10.0f;
		public float xSpeed = 120.0f;
		public float ySpeed = 120.0f;
		public float yMinLimit = -20f;
		public float yMaxLimit = 80f;
		public float distanceMin = 10f;
		public float distanceMax = 20f;
		public KeyCode RotateKey;

		float x = 0.0f;
		float y = 0.0f;

		// Use this for initialization
		void Start()
		{
			Vector3 angles = transform.eulerAngles;
			x = angles.y;
			y = angles.x;

			;
			// Make the rigid body not change rotation
			//GetComponent<Rigidbody>().freezeRotation = true;
		}

		void LateUpdate()
		{

			if (target)
			{
				if (Input.GetKey(RotateKey))
				{
					x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
					y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
				}

				y = ClampAngle(y, yMinLimit, yMaxLimit);

				Quaternion rotation = Quaternion.Euler(y, x, 0);

				distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

				RaycastHit hit;
				if (Physics.Linecast(target.position, transform.position, out hit))
				{
					distance -= hit.distance;
				}

				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation * negDistance + target.position;

				transform.rotation = rotation;
				transform.position = position;
			}

		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}
	}
}

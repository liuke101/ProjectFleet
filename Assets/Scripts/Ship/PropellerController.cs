using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Vbertz.PBSC
{
	public class PropellerController : MonoBehaviour
	{
		[SerializeField] private Physics_Based_Ship_Controller shipController;

		[SerializeField] private bool isRightPropeller;
		public float PropellerSpeed = 0f;
		float rotationSpeed;

		private void Update()
		{

			float speed = shipController.CurrentSpeed;
			float turnSpeed = shipController.CurrentTurnSpeed;
			rotationSpeed = speed * PropellerSpeed;

			// Si el barco está avanzando o girando hacia el lado definido, enciende la hélice
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
			// Si el barco va en reversa, invierte la rotación de la hélice
			else if (speed < 0)
			{
				RotatePropeller(-speed);
			}
		}

		private void RotatePropeller(float speed)
		{
			// Ajusta la velocidad de rotación según sea necesario

			transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
		}
	}
}

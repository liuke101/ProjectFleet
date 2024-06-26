using UnityEngine;

namespace Vbertz.PBSC
{
	public class BoatTiltController : MonoBehaviour
	{
		[SerializeField] private Physics_Based_Ship_Controller shipController;
		public Transform ShipRerefence;

		[SerializeField] private bool IsSubmarine = false; // Añade un booleano para verificar si es un submarino

		[SerializeField] private float maxTiltAngle = 5f;
		[SerializeField] private float Submarine_maxTiltAngle = 5f;

		[SerializeField] private float tiltSmoothness = 1f;

		private float currentTiltAngle;
		private float submarineTiltAngle; // Añade una variable separada para la inclinación del submarino

		private void Update()
		{
			float turnSpeed = shipController.CurrentTurnSpeed;

			// Calcula el ángulo de inclinación deseado basado en la velocidad de giro del barco
			float desiredTiltAngle = Mathf.Clamp(turnSpeed, -1, 1) * maxTiltAngle;

			// Interpola suavemente entre el ángulo de inclinación actual y el deseado
			currentTiltAngle = Mathf.LerpAngle(currentTiltAngle, desiredTiltAngle, Time.deltaTime * tiltSmoothness);

			// Si es un submarino, calcula la inclinación basada en la velocidad vertical
			if (IsSubmarine)
			{
				float verticalSpeed = shipController.GetComponent<Rigidbody>().velocity.y;
				float
					direction = verticalSpeed > 0
						? 1
						: -1; // Si sube, se inclina hacia adelante (1), si baja, se inclina hacia atrás (-1)

				// Si la velocidad vertical es diferente de 0, calcula el ángulo de inclinación
				if (Mathf.Abs(verticalSpeed) > 0.01f)
				{
					float desiredSubmarineTiltAngle = direction * Submarine_maxTiltAngle;
					submarineTiltAngle = Mathf.LerpAngle(submarineTiltAngle, desiredSubmarineTiltAngle,
						Time.deltaTime * tiltSmoothness);
				}
				else
				{
					// Si la velocidad vertical es 0, vuelve a la rotación 0
					submarineTiltAngle = Mathf.LerpAngle(submarineTiltAngle, 0, Time.deltaTime * tiltSmoothness);
				}

				// Gira el submarino en su eje X local
				transform.localRotation = Quaternion.Euler(0, 0, submarineTiltAngle);
			}
			else
			{
				transform.localRotation = Quaternion.Euler(0, 0, currentTiltAngle);
			}
		}

	}
}
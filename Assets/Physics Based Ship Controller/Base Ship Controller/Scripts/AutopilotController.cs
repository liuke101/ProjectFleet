using UnityEngine;
using UnityEngine.UI;


namespace Vbertz.PBSC
{
    public class AutopilotController : MonoBehaviour
    {
        public Text TargetDistanceText;
        [SerializeField] private Physics_Based_Ship_Controller shipController;

        [SerializeField] private Transform target; // El objetivo al que el barco debe dirigirse

        [SerializeField]
        private float
            closeEnoughDistance = 1f; // La distancia a la que se considera que el barco ha llegado al objetivo

        [SerializeField]
        private KeyCode autopilotKey = KeyCode.P; // La tecla para activar/desactivar el piloto automático

        [SerializeField]
        private float decelerationDistance = 10f; // La distancia a la que el barco comienza a desacelerar

        private bool isAutopilotActive = false; // Si el piloto automático está activo o no

        private int originalForwardVelocity; // La velocidad hacia adelante original del barco
        private int originalTurnVelocity; // La velocidad de giro original del barco
        public float lastTurnTime;
        public float turnCooldown;
        public float detectionDistance;
        public Toggle ActivationToggle;

        private void Start()
        {
            // Guarda los valores originales de las velocidades
            originalForwardVelocity = shipController.CurrentForwardVelocity;
            originalTurnVelocity = shipController.CurrentTurnVelocity;
        }

        private void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
            {
                // Si el rayo golpea un obstáculo, gira el barco para evitarlo
                if (hit.collider.gameObject.CompareTag("Obstacle"))
                {
                    shipController.TurnRight();
                }
            }

            // Si el jugador pulsa la tecla del piloto automático, cambia el estado del piloto automático
            if (Input.GetKeyDown(autopilotKey))
            {
                isAutopilotActive = !isAutopilotActive;
                ActivationToggle.isOn = !ActivationToggle.isOn;
            }

            // Si el piloto automático está desactivado, no hacemos nada
            if (!isAutopilotActive)
            {
                return;
            }

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

            // Calcula la diferencia de ángulo entre la dirección actual del barco y la dirección al objetivo
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

            // Si la diferencia de ángulo es positiva, gira a la derecha; si es negativa, gira a la izquierda
            if (angleDifference > 0)
            {
                shipController.TurnRight();
            }
            else if (angleDifference < 0)
            {
                shipController.TurnLeft();
            }

            // Calcula la distancia al objetivo
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Si el barco está lo suficientemente cerca del objetivo, detén el barco y desactiva el piloto automático
            if (distanceToTarget <= closeEnoughDistance)
            {
                shipController.MoveBackward();
                shipController.CurrentForwardVelocity = originalForwardVelocity;
                shipController.CurrentTurnVelocity = originalTurnVelocity;
                isAutopilotActive = false; // Desactiva el piloto automático
                ActivationToggle.isOn = false;
            }
            else if
                (distanceToTarget <=
                 decelerationDistance) // Si el barco está dentro de la distancia de desaceleración, comienza a desacelerar
            {
                // Calcula un factor de desaceleración basado en cuán cerca está el barco del objetivo
                float decelerationFactor = distanceToTarget / decelerationDistance;

                // Ajusta la velocidad del barco en función del factor de desaceleración
                shipController.CurrentSpeed *= decelerationFactor;
            }
            else // De lo contrario, avanza a toda velocidad
            {
                shipController.MoveForward();
            }

            TargetDistanceText.text = distanceToTarget.ToString("f0") + "meters To target";
            if (Time.time - lastTurnTime > turnCooldown)
            {
                // Si la diferencia de ángulo es positiva, gira a la derecha; si es negativa, gira a la izquierda
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

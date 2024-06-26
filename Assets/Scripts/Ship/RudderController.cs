using System.Collections;
using UnityEngine;


public class RudderController : MonoBehaviour
{
    [SerializeField] public PhysicsBasedShipController shipController;
    [SerializeField] private float maxRudderAngle = 30f; // El ángulo máximo que puede girar el timón

    [SerializeField]
    private float anticipationFactor = 0.5f; // Cuántos segundos antes quieres que el timón comience a girar

    private float previousTurnSpeed;

    private void Update()
    {
        // Guarda la velocidad de giro del barco para usarla en el próximo frame
        float turnSpeed = previousTurnSpeed;
        previousTurnSpeed = shipController.CurrentTurnSpeed;

        // Espera durante el factor de anticipación antes de actualizar la velocidad de giro
        StartCoroutine(WaitAndTurn(turnSpeed));
    }

    private IEnumerator WaitAndTurn(float turnSpeed)
    {
        yield return new WaitForSeconds(anticipationFactor);

        // Calcula el ángulo del timón basado en la velocidad de giro del barco
        float rudderAngle = Mathf.Clamp(turnSpeed, -1, 1) * -maxRudderAngle;

        // Gira el timón en su eje Y
        transform.localRotation = Quaternion.Euler(0, rudderAngle, 0);
    }
}

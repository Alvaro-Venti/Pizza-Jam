using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoBus : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float maxSpeedReverse = 5f;
    public float multiplicadorFreno = 2f;
    public float acceleration = 5f;
    public float braking = 10f;
    //public float minSpeedForRotation = 1f; // Velocidad m�nima para permitir la rotaci�n
    public float maxTurnSpeed = 100f; // Velocidad m�xima de giro
    public float minTurnSpeed = 0f; // Velocidad m�nima de giro
    public float turnSpeedMultiplier = 0.1f; // Multiplicador de velocidad de giro

    private Rigidbody rb;
    private float currentSpeed = 0f;

    void Start()
    {
        // Verificar si hay un Rigidbody adjunto, si no, crear uno
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
        }

        rb = GetComponent<Rigidbody>();
        //rb.interpolation = RigidbodyInterpolation.Interpolate; // Opcional: para mejorar la calidad de la interpolaci�n de la posici�n
        // Opcional: para mejorar la detecci�n de colisiones con objetos m�viles --> rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.mass = 1000f; // Ajusta la masa del Rigidbody seg�n sea necesario
    }

    void Update()
    {
        // Control de la aceleraci�n y el frenado
        float inputVertical = Input.GetAxis("Vertical");

        // Aplicar fuerza de frenado constante cuando no se est� acelerando ni frenando activamente
        if (Mathf.Approximately(inputVertical, 0f))
        {
            if (currentSpeed > 0f)
                currentSpeed -= braking * Time.deltaTime;
            else if (currentSpeed < 0f)
                currentSpeed += braking * Time.deltaTime;

            // Ajustar la velocidad a 0 si es cercana
            if (Mathf.Abs(currentSpeed) < 0.1f)
                currentSpeed = 0f;
        }
        else
        {
            // Aplicar aceleraci�n o frenado seg�n la entrada del jugador
            if(inputVertical < 0 && currentSpeed > 0f)
            {
                currentSpeed += inputVertical * acceleration * Time.deltaTime * multiplicadorFreno;
            }
            else
            {
                currentSpeed += inputVertical * acceleration * Time.deltaTime;
            }
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeedReverse, maxSpeed);
        }

        // Calcular la velocidad de rotaci�n en funci�n de la velocidad actual del autob�s
        float turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, Mathf.Clamp01(Mathf.Abs(currentSpeed) * turnSpeedMultiplier));

        // Control de la direcci�n
        float inputHorizontal = Input.GetAxis("Horizontal");
        if (currentSpeed < 0f)
            inputHorizontal *= -1f;

        transform.Rotate(Vector3.up * inputHorizontal * turnSpeed * Time.deltaTime);

        // Aplicar movimiento
        rb.velocity = transform.forward * currentSpeed;
    }
}


using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(Rigidbody))]
public class SplineMover : MonoBehaviour
{
    [SerializeField] public SplineContainer splineContainer; // Referencia al Spline
    [SerializeField] private float baseDuration = 5f; // Duración base cuando la carga está al 50%
    [SerializeField] private bool loop = false; // Si debe repetir el recorrido
    [SerializeField] private float lerpSpeed = 2f; // 🔹 Velocidad de Lerp para suavizar cambios

    private Rigidbody rb;
    private float elapsedTime = 0f;
    private bool reachedEnd = false;

    private float adjustedSpeed; // Velocidad ajustada en base al Shield Charge
    private float targetSpeed; // Velocidad objetivo para hacer Lerp

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Para moverse con MovePosition()

        adjustedSpeed = 1f / baseDuration; // Inicializar velocidad base
        UpdateSpeed(); // Ajustar la velocidad inicial
    }

    void FixedUpdate()
    {
        if (splineContainer == null || reachedEnd) return;

        UpdateSpeed(); // Se recalcula progresivamente en cada frame

        // 🔹 Incrementamos `elapsedTime` con la velocidad ajustada
        elapsedTime += adjustedSpeed * Time.fixedDeltaTime;

        float t = Mathf.Clamp01(elapsedTime); // Normaliza el tiempo de 0 a 1

        // Obtener la posición en el spline
        Vector3 targetPosition = splineContainer.EvaluatePosition(t);

        // Obtener la tangente para la rotación
        Vector3 tangent = (Vector3)splineContainer.EvaluateTangent(t);
        if (tangent == Vector3.zero) tangent = transform.forward; // Previene errores si la tangente es (0,0,0)
        Quaternion targetRotation = Quaternion.LookRotation(tangent.normalized, Vector3.up);

        // Mover usando física
        rb.MovePosition(targetPosition);
        rb.MoveRotation(targetRotation); // Alinear con la ruta

        // Detectar si llega al final
        if (t >= 1f)
        {
            Debug.Log("🚀 La nave ha llegado al final del recorrido.");

            if (loop)
            {
                elapsedTime = 0f; // Reinicia el recorrido si está en loop
            }
            else
            {
                reachedEnd = true;
            }
        }
    }

    private void UpdateSpeed()
    {
        if (ShieldController.THIS == null) return; // Evita errores si el ShieldController aún no está en escena

        float shieldCharge = ShieldController.THIS.shieldCharge; // Obtener la carga actual

        float chargeFactor = shieldCharge / 50f; // 50% de carga = velocidad normal
        chargeFactor = Mathf.Clamp(chargeFactor, 0.3f, 2f); // Mínimo 0.3x, máximo 2x más rápido

        targetSpeed = (1f / (baseDuration / chargeFactor)); // 🔹 Nueva velocidad objetivo

        // 🔹 Suavizamos el cambio con Lerp
        adjustedSpeed = Mathf.Lerp(adjustedSpeed, targetSpeed, Time.deltaTime * lerpSpeed);
    }
}

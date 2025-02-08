using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics; // Necesario para convertir float3 a Vector3

[RequireComponent(typeof(Rigidbody))]
public class SplineMover : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer; // Referencia al Spline
    [SerializeField] private float duration = 5f; // Duración total del recorrido en segundos
    [SerializeField] private bool loop = false; // Si debe repetir el recorrido

    private Rigidbody rb;
    private float elapsedTime = 0f;
    private bool reachedEnd = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Mantenemos el Rigidbody cinemático para usar MovePosition()
    }

    void FixedUpdate()
    {
        if (splineContainer == null || reachedEnd) return;

        elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration); // Normaliza el tiempo de 0 a 1

        // Obtener la posición en el spline
        Vector3 targetPosition = splineContainer.EvaluatePosition(t);

        // Obtener la tangente para la rotación y convertir float3 a Vector3
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
}
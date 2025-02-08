using System.Collections;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public static ShieldController THIS;

    public float shieldSize;
    public float shieldCharge;
    public float chargeLoss;
    public float shieldTime;
    public float shieldTimeLimit;
    public float maxShieldCharge; // Nuevo límite máximo de carga de escudo

    [Header("Bounce Settings")]
    public float maxBounceMultiplier = 1.2f; // 1.2 significa 120% del tamaño final
    public float bounceSpeed = 2.0f; // Controla la velocidad del rebote

    private Transform shieldSizeTransform;
    private int enemyDamage;
    private int resourceCharge;

    private Coroutine sizeLerpCoroutine;

    private void Awake()
    {
        THIS = this;
    }

    void Start()
    {
        shieldSizeTransform = transform.GetChild(1);

        shieldCharge = 100f;
        shieldSize = shieldCharge * 0.1f;
        shieldTime = 0f;
        shieldTimeLimit = 0.5f;
        chargeLoss = 0.99f;
        shieldSizeTransform.localScale = Vector3.one * shieldSize;

        enemyDamage = 5;
        resourceCharge = 5;

        if (maxShieldCharge <= 0) maxShieldCharge = 100f; // Valor por defecto si no se asigna en el inspector
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) EnemyCollision(enemyDamage);
        if (Input.GetKeyDown(KeyCode.E)) AddResource();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space)) ShieldTimer();
        if (Input.GetKeyUp(KeyCode.Space)) shieldTime = 0f;
    }

    private void ShieldTimer()
    {
        if (shieldTime <= shieldTimeLimit)
        {
            shieldTime += Time.deltaTime;
        }
        else
        {
            shieldTime = 0f;
            shieldCharge *= chargeLoss;
            UpdateShieldSize();
        }
    }

    public void EnemyCollision(int damage)
    {
        shieldCharge -= damage;
        UpdateShieldSize();
    }

    private void AddResource()
    {
        shieldCharge += resourceCharge;
        shieldCharge = Mathf.Min(shieldCharge, maxShieldCharge); // Limitar la carga al máximo permitido
        UpdateShieldSize(true);
    }

    private void UpdateShieldSize(bool doBounce = false)
    {
        shieldSize = shieldCharge * 0.1f;
        if (sizeLerpCoroutine != null) StopCoroutine(sizeLerpCoroutine);
        sizeLerpCoroutine = StartCoroutine(AnimateShieldSize(shieldSize, doBounce));
    }

    private IEnumerator AnimateShieldSize(float targetSize, bool doBounce)
    {
        float elapsedTime = 0f;
        float duration = 1.0f / bounceSpeed; // Cuanto mayor sea bounceSpeed, más rápido es el rebote

        Vector3 initialScale = shieldSizeTransform.localScale;
        Vector3 targetScale = Vector3.one * targetSize;

        if (doBounce)
        {
            // Pequeño rebote al crecer (se pasa un poco del tamaño objetivo)
            Vector3 overshootScale = targetScale * maxBounceMultiplier;

            // Fase 1: Crecimiento con overshoot
            while (elapsedTime < duration * 0.5f)
            {
                shieldSizeTransform.localScale = Vector3.Lerp(initialScale, overshootScale, (elapsedTime / (duration * 0.5f)));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Fase 2: Volver al tamaño objetivo
            elapsedTime = 0f;
            while (elapsedTime < duration * 0.5f)
            {
                shieldSizeTransform.localScale = Vector3.Lerp(overshootScale, targetScale, (elapsedTime / (duration * 0.5f)));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Lerp normal sin rebote
            while (elapsedTime < duration)
            {
                shieldSizeTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        shieldSizeTransform.localScale = targetScale; // Asegurar el tamaño final
    }
}

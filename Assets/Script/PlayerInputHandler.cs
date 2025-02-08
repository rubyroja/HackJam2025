using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float speed = 7f;

    // Rango de velocidad basado en el peso
    public float maxSpeed = 10f; // Velocidad máxima cuando el peso es 0
    public float minSpeed = 3f;  // Velocidad mínima cuando el peso es 100

    // Peso del jugador (afecta la velocidad)
    public int peso = 0; // Entre 0 y 100

    // Dirección del movimiento
    private Vector2 movementInput;

    // Referencias
    private Camera mainCamera;
    public Animator animator;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        AdjustSpeedBasedOnWeight(); // Ajustar la velocidad según el peso
        MovePlayer(); // Aplicar movimiento
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void AdjustSpeedBasedOnWeight()
    {
        // Interpolación entre minSpeed y maxSpeed según el peso
        speed = Mathf.Lerp(maxSpeed, minSpeed, peso / 100f);
    }

    public void AddWeight(int amount)
    {
        peso = Mathf.Clamp(peso + amount, 0, 100);
    }

    public void ReduceWeight(int amount)
    {
        peso = Mathf.Clamp(peso - amount, 0, 100);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnCall(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Call input detected");
        }
    }
}

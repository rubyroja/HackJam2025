using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class EnemyController : MonoBehaviour
{
    public int enemyDamage = 5;
    public float detectionRange = 50f; // Rango de detección del Player
    public float updatePathInterval = 0.2f; // Frecuencia de actualización de la ruta
    public string playerTag = "Player"; // Tag del Player
    public NavMeshSurface navMeshSurface; // Referencia al NavMesh dinámico

    private NavMeshAgent agent;
    private Transform playerTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscar al Player en la escena
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Si no hay NavMeshSurface asignado, lo busca en la escena
        if (navMeshSurface == null)
        {
            navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
        }

        // Regenerar el NavMesh cuando aparece un enemigo
        if (navMeshSurface != null)
        {
            StartCoroutine(GenerateNavMesh());
        }

        // Actualizar la ruta hacia el Player periódicamente
        InvokeRepeating(nameof(UpdatePath), 0.1f, updatePathInterval);
    }

    void UpdatePath()
    {
        if (playerTransform != null && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private IEnumerator GenerateNavMesh()
    {
        yield return new WaitForEndOfFrame(); // Espera un frame antes de generar el NavMesh
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Shield"))
        {
            ShieldController.THIS.EnemyCollision(enemyDamage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}

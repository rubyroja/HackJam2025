using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo a instanciar
    public int numberOfEnemies = 3; // Número de enemigos a spawnear

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // Detecta al Player por Layer
        {
            SpawnEnemies();
            Destroy(gameObject); // Se elimina después de activar el spawn
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"SpawnPoint ({gameObject.name}): Falta asignar el prefab del enemigo.");
            return;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}

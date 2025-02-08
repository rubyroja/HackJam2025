using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform[] spawnPoints; 

    private void Start()
    {
        // Asegurar que cada punto de spawn tenga un Collider en modo Trigger
        foreach (Transform spawnPoint in spawnPoints)
        {
            Collider col = spawnPoint.GetComponent<SphereCollider>();

            spawnPoint.GetComponent<SphereCollider>().radius = 10;
            if (col == null)
            {
                col = spawnPoint.gameObject.AddComponent<SphereCollider>(); 
            }
            
            col.isTrigger = true; // Se asegura de que sea un Trigger
        }
    }
}

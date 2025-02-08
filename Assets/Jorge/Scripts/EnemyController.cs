using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int enemyDamage;
    void Start()
    {
        enemyDamage = 5;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Shield")){
            ShieldController.THIS.EnemyCollision(enemyDamage);
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other) {
        
    }
}

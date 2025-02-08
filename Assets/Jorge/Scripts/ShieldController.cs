using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

    public static ShieldController THIS;

    public float shieldSize;
    public float shieldCharge;
    public float chargeLoss;
    public float shieldTime;
    public float shieldTimeLimit;

    private Transform shieldSizeTransform;


    private int enemyDamage;
    private int resourceCharge;
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
        shieldSizeTransform.localScale = new UnityEngine.Vector3(shieldSize, shieldSize, shieldSize);

        enemyDamage = 5;
        resourceCharge = 5;
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

    private void OnCollisionEnter(Collision other)
    {

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

    private void EnemyCollision(int damage)
    {
        shieldCharge -= damage;
        UpdateShieldSize();
    }

    private void AddResource()
    {
        shieldCharge += resourceCharge;
        UpdateShieldSize();


    }

    private void UpdateShieldSize()
    {
        shieldSize = shieldCharge * 0.1f;
        shieldSizeTransform.localScale = new UnityEngine.Vector3(shieldSize, shieldSize, shieldSize);
    }
}

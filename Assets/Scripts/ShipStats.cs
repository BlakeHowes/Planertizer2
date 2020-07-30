using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStats : MonoBehaviour
{
    [SerializeField]
    public float Health;
    [SerializeField]
    private float MaxHealth;
    [SerializeField]
    private float RegenSpeed;

    void Update()
    {
        if (Health <= MaxHealth)
        {
            Health += Time.deltaTime * RegenSpeed / 10f;
        }

        if (Health < 0f)
        {
            Destroy(gameObject);
        }
    }

    public void RemoveHealth(float Damage)
    {
        Health -= Damage;
    }
}
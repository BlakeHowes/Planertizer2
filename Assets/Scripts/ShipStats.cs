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
    [SerializeField]
    private bool AmIATurret;
    void OnEnable()
    {
        
    }

    void Update()
    {
        if(Health <= MaxHealth)
        {
            Health += Time.deltaTime * RegenSpeed/10f;
        }

        if (Health < 0f)
        {
            Destroy(gameObject);
            if (AmIATurret == false)
            {
                GetComponent<ShipAI>().RemoveFromPlanet();
            }
        }
    }
}

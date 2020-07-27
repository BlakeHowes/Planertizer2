using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAi : MonoBehaviour
{
    [SerializeField]
    private string WhatIAttack;
    [SerializeField]
    private LineRenderer Gun;
    [SerializeField]
    private float Damage;
    public List<GameObject> EnemysInRange = new List<GameObject>();
    public Collider other;
    private float nearestDistance;
    private GameObject nearestEnemy;

    void OnTriggerEnter(Collider collider)
    {
        if (collider != this)
        {
            if (collider.tag == (WhatIAttack))
            {
                EnemysInRange.Add(collider.attachedRigidbody.gameObject);
            }

        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == (WhatIAttack))
        {
            EnemysInRange.Remove(collider.attachedRigidbody.gameObject);
        }
    }

    private void Update()
    {
        nearestEnemy = null;
        nearestDistance = Mathf.Infinity;
        List<GameObject> ShipsToRemove = new List<GameObject>();
        foreach (GameObject Ship in EnemysInRange)
        {
            if (Ship != this)
            {
                if (Ship == null)
                {
                    ShipsToRemove.Add(Ship);
                    continue;
                }

                if (Vector3.Distance(transform.position, Ship.transform.position) < nearestDistance)
                {
                    nearestDistance = Vector3.Distance(transform.position, Ship.transform.position);
                    nearestEnemy = Ship;
                }
            }
            if (EnemysInRange.Count > 0)
            {
                Gun.enabled = true;
                Gun.SetPosition(0, transform.position);
                Gun.SetPosition(1, nearestEnemy.transform.position);
                Ship.GetComponent<ShipStats>().Health -= Time.deltaTime * Damage;
            }

        }
        foreach (GameObject Ship in ShipsToRemove)
        {
            EnemysInRange.Remove(Ship);
        }

        if (EnemysInRange.Count == 0)
        {
            Gun.enabled = false;
        }
    }
}

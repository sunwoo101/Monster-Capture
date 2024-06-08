using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    #region Variables

    // Attributes
    [SerializeField] private float m_AttackDamage;

    // Optimization
    [SerializeField] private float m_ShootRange = 100;

    // References
    private Camera m_Camera;

    #endregion

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot(m_AttackDamage);
        }
    }

    private void Shoot(float damage)
    {
        RaycastHit hit;
        if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit, m_ShootRange))
        {
            IDamagable target = hit.transform.GetComponent<IDamagable>();
            if (target != null)
            {
                target.TakeDamage(damage, transform);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional Require Components
[RequireComponent(typeof(LineRenderer))]

public class AggressiveEnemy : Enemy
{
    #region Variables

    [SerializeField] private float m_MoveTowardsSpeedMultiplier;
    [SerializeField] private float m_AttackDamagePerSecond;
    private LineRenderer m_LineRenderer;

    #endregion

    protected override void Start()
    {
        base.Start();

        // More references
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.startWidth = 0.1f;
        m_LineRenderer.endWidth = 0.1f;
        m_LineRenderer.alignment = LineAlignment.View;
        m_LineRenderer.startColor = Color.red;
        m_LineRenderer.endColor = Color.red;
        m_LineRenderer.enabled = false;
    }

    protected override void Action()
    {
        m_MeshRenderer.material = m_ActionMaterial;
        if (Time.time - m_ActionStartTime >= m_ActionDuration)
        {
            m_LineRenderer.enabled = false;
            m_StateMachine = StateMachine.Wandering;
            return;
        }

        // Look towards
        Vector3 direction = m_AttackedByTransform.position - transform.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.eulerAngles.x, lookRotation.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = lookRotation;

        // Move towards
        // Prevent falling off platforms while moving
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward, Vector3.down, out hit, 1.1f))
        {
            Vector3 velocity = transform.forward * m_MovementSpeed * m_MoveTowardsSpeedMultiplier;
            velocity.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = velocity;
        }
        else
        {
            Vector3 velocity = Vector3.zero;
            velocity.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = velocity;
        }

        // Render lazer
        m_LineRenderer.enabled = true;
        m_LineRenderer.SetPosition(0, transform.position + (Vector3.up * 0.75f));
        m_LineRenderer.SetPosition(1, m_AttackedByTransform.position);

        // Shoot lazer
        m_AttackedByTransform.GetComponent<IDamagable>().TakeDamage(m_AttackDamagePerSecond * Time.deltaTime, transform);
    }
}

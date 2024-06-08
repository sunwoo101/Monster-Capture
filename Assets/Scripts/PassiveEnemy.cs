using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : Enemy
{
    #region Variables

    [SerializeField] private float m_RunAwaySpeedMultiplier;

    #endregion

    protected override void Action()
    {
        m_MeshRenderer.material = m_ActionMaterial;
        if (Time.time - m_ActionStartTime >= m_ActionDuration)
        {
            m_StateMachine = StateMachine.Wandering;
        }

        // Look away
        Vector3 direction = m_AttackedByTransform.position - transform.position;
        direction = -direction;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = Quaternion.Euler(transform.eulerAngles.x, lookRotation.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = lookRotation;

        // Run away
        Vector3 velocity = transform.forward * m_MovementSpeed * m_RunAwaySpeedMultiplier;
        velocity.y = m_Rigidbody.velocity.y;
        m_Rigidbody.velocity = velocity;
    }
}

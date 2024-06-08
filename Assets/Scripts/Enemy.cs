using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour, IDamagable
{
    #region Variables

    // Attributes
    [SerializeField] private float m_BaseHealth;
    private float m_CurrentHealth;
    [SerializeField] protected float m_MovementSpeed;

    // References
    [SerializeField] private GameObject m_SoulPrefab;
    protected Rigidbody m_Rigidbody;
    protected MeshRenderer m_MeshRenderer;
    protected ParticleSystem m_TakeDamageParticle;

    [SerializeField] protected Material m_WanderingMaterial;
    [SerializeField] protected Material m_AlertMaterial;
    [SerializeField] protected Material m_ActionMaterial;

    // AI
    protected enum StateMachine
    {
        Wandering,
        Alert,
        Action,
    }
    protected StateMachine m_StateMachine;
    //Wandering
    [SerializeField] protected float m_SecondsBeforeChangingDirection;
    protected float m_ChangeDirectionTime;
    protected float m_CurrentDirection;
    //Alert
    [SerializeField] protected float m_ActionPrepareTime;
    protected bool m_PreparingAction;
    protected float m_PrepareActionStartTime;
    //Action
    [SerializeField] protected float m_ActionDuration;
    protected float m_ActionStartTime;
    protected Transform m_AttackedByTransform;

    #endregion

    protected virtual void Start()
    {
        // Attributes
        m_CurrentHealth = m_BaseHealth;

        // References
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_TakeDamageParticle = GetComponent<ParticleSystem>();

        // AI
        m_StateMachine = StateMachine.Wandering;
    }

    private void Update()
    {
        switch (m_StateMachine)
        {
            case StateMachine.Wandering:
                Wander();
                break;
            case StateMachine.Alert:
                Alert();
                break;
            case StateMachine.Action:
                Action();
                break;
        }
    }

    protected void Wander()
    {
        m_MeshRenderer.material = m_WanderingMaterial;
        // Wander
        if (Time.time - m_ChangeDirectionTime >= m_SecondsBeforeChangingDirection)
        {
            m_CurrentDirection = ChangeDirection();
            m_ChangeDirectionTime = Time.time;
        }

        Vector3 rotation = new Vector3(0, m_CurrentDirection, 0);
        transform.rotation = Quaternion.Euler(rotation);
        
        Vector3 velocity = transform.forward * m_MovementSpeed;
        velocity.y = m_Rigidbody.velocity.y;
        m_Rigidbody.velocity = velocity;

        // Prevent falling off platforms while moving
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + transform.forward, Vector3.down, out hit, 1.1f))
        {
            m_CurrentDirection += 180;
        }
    }

    protected float ChangeDirection()
    {
        float direction = UnityEngine.Random.Range(0.0f, 360.0f);

        return direction;
    }

    protected void Alert()
    {
        m_MeshRenderer.material = m_AlertMaterial;
        if (!m_PreparingAction)
        {
            m_PreparingAction = true;
            m_PrepareActionStartTime = Time.time;
        }

        if (Time.time - m_PrepareActionStartTime >= m_ActionPrepareTime)
        {
            m_PreparingAction = false;
            m_ActionStartTime = Time.time;
            m_StateMachine = StateMachine.Action;
        }
    }
    int index = 0;
    protected virtual void Action()
    {
        m_MeshRenderer.material = m_ActionMaterial;
        if (Time.time - m_ActionStartTime >= m_ActionDuration)
        {
            m_StateMachine = StateMachine.Wandering;
        }
        // Do action
        Vector3 lookAtTransform = m_AttackedByTransform.position;
        lookAtTransform.y = 0;
        transform.LookAt(lookAtTransform);
        Debug.Log(index);
        index++;
    }

    #region Taking Damage

    public void TakeDamage(float damage, Transform transform)
    {
        m_TakeDamageParticle.Play();
        m_AttackedByTransform = transform;
        
        if (m_CurrentHealth >= damage)
        {
            m_CurrentHealth -= damage;
        }
        else
        {
            m_CurrentHealth = 0;
        }

        if (m_StateMachine == StateMachine.Wandering)
        {
            m_StateMachine = StateMachine.Alert;
        }

        if (m_CurrentHealth == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Instantiate(m_SoulPrefab, transform.position + (Vector3.down * 0.7f), Quaternion.identity);
        Destroy(gameObject);
    }

    #endregion
}

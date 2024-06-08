using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Require componenets
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour, IDamagable
{
    #region Variables

    // Attributes
    private float m_DamageTaken;
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_AttackDamage;
    [SerializeField] private float m_MaxShootRange;

    // Collectables
    [SerializeField] private float m_InteractRange;
    [HideInInspector] public int iAggressiveSoulsCollected;
    [HideInInspector] public int iPassiveSoulsCollected;

    // References
    private Rigidbody m_RigidBody;
    private Camera m_Camera;

    // Input variables
    private Vector2 m_Movement;
    private bool m_Jump;
    private bool m_Shoot;
    private bool m_Interact;

    // Checks
    [SerializeField] private LayerMask m_GroundLayerMask;
    private float m_GroundCheckRayDistance;
    private bool m_IsGrounded;

    // GUI
    [SerializeField] private Text m_AggressiveSoulsText;
    [SerializeField] private Text m_PassiveSoulsText;
    [SerializeField] private Text m_DamageTakenText;
    [SerializeField] private GameObject m_InteractPanel;
    [SerializeField] private Text m_InteractText;

    #endregion

    private void Start()
    {
        // References
        m_RigidBody = GetComponent<Rigidbody>();
        m_Camera = Camera.main;

        // Checks
        m_GroundCheckRayDistance = (GetComponent<CapsuleCollider>().height / 2) + 0.1f;
    }

    private void Update()
    {
        GetInput();
        Move();
        GroundCheck();
        Jump();
        Shoot();
        Interact();
        GUI();
    }

    private void GetInput()
    {
        m_Movement.x = Input.GetAxisRaw("Horizontal");
        m_Movement.y = Input.GetAxisRaw("Vertical");

        m_Jump = Input.GetButtonDown("Jump");

        m_Shoot = Input.GetMouseButtonDown(0);

        m_Interact = Input.GetKeyDown(KeyCode.E);
    }

    private void GUI()
    {
        m_AggressiveSoulsText.text = "Aggressive Souls Collected: " + iAggressiveSoulsCollected;
        m_PassiveSoulsText.text = "Passive Souls Collected: " + iPassiveSoulsCollected;
        m_DamageTakenText.text = "Damage Taken: " + (int)m_DamageTaken;
    }

    #region Movement

    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, m_GroundCheckRayDistance, m_GroundLayerMask))
        {
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
        }
    }

    private void Jump()
    {
        if (m_Jump && m_IsGrounded)
        {
            // Reset y velocity
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);

            // Jump
            m_RigidBody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        // Get inputs into vector
        Vector3 forwardMovement = transform.forward * m_Movement.y;
        Vector3 sideMovement = transform.right * m_Movement.x;
        Vector3 movementVector = forwardMovement + sideMovement;

        // Normalize movement vector
        if (movementVector.magnitude > 1)
        {
            movementVector.Normalize();
        }

        // Speed
        movementVector *= m_MovementSpeed;

        // Fix y velocity
        movementVector.y = m_RigidBody.velocity.y;

        // Move
        m_RigidBody.velocity = movementVector;
    }

    #endregion

    #region Taking Damage

    public void TakeDamage(float damage, Transform transform)
    {
        m_DamageTaken += damage;
    }

    public void Death()
    {
        Debug.Log("No death");
    }

    #endregion

    #region Attacks

    private void Shoot()
    {
        if (m_Shoot)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit, m_MaxShootRange))
            {
                IDamagable target = hit.transform.GetComponent<IDamagable>();
                if (target != null)
                {
                    target.TakeDamage(m_AttackDamage, transform);
                }
            }
        }
    }

    #endregion

    #region Interactions

    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit, m_InteractRange))
        {
            Interactable item = hit.transform.GetComponent<Interactable>();
            if (item != null)
            {
                m_InteractPanel.SetActive(true);
                m_InteractText.text = "Collect (E): " + item.name.Replace("(Clone)", "");
                if (m_Interact)
                {
                    item.Interact(this);
                }
            }
            else
            {
                m_InteractPanel.SetActive(false);
            }
        }
        else
        {
            m_InteractPanel.SetActive(false);
        }
    }

    #endregion
}

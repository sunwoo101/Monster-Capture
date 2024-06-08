using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    // Preferences
    [SerializeField] private float m_MouseSensitivity = 2;

    // References
    [SerializeField] private Transform m_PlayerTransform;

    private float m_XRotation;

    #endregion

    private void Start()
    {
        // Lock mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        float m_YMouse = Input.GetAxisRaw("Mouse Y") * m_MouseSensitivity;
        float m_XMouse = Input.GetAxisRaw("Mouse X") * m_MouseSensitivity;

        // Left/Right camera move
        m_PlayerTransform.Rotate(Vector3.up * m_XMouse);

        // Up/Down camera move
        m_XRotation -= m_YMouse;
        m_XRotation = Mathf.Clamp(m_XRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(m_XRotation, 0f, 0f);
    }
}

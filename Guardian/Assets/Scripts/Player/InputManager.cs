using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Input Manager Script from YouTube @Sasquatch B Studios
public class InputManager : MonoBehaviour
{
    public static PlayerInput m_PlayerInput;

    public static Vector2 v2Movement;
    public static bool bJumpPressed;
    public static bool bJumpHeld;
    public static bool bJumpReleased;
    public static bool bRunHeld;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_RunAction;

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();

        m_MoveAction = m_PlayerInput.actions["Move"];
        m_JumpAction = m_PlayerInput.actions["Jump"];
        m_RunAction = m_PlayerInput.actions["Run"];
    }

    // Update is called once per frame
    private void Update()
    {
        v2Movement = m_MoveAction.ReadValue<Vector2>();

        bJumpPressed = m_JumpAction.WasPressedThisFrame();
        bJumpHeld = m_JumpAction.IsPressed();
        bJumpReleased = m_JumpAction.WasReleasedThisFrame();

        bRunHeld = m_RunAction.IsPressed();
    }
}

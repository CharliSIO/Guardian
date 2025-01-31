using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D m_FeetCollider;
    [SerializeField] private Collider2D m_BodyCollider;

    private Rigidbody2D m_PlayerRigidBody;

    // movement variables
    private Vector2 m_MoveVelocity;
    private bool m_bFacingRight;

    // collision checking variables
    private RaycastHit2D m_GroundHit;
    private RaycastHit2D m_HeadHit;
    private bool m_bGrounded;
    private bool m_bBumpedHead;

    private void Awake()
    {
        m_bFacingRight = true;
        m_PlayerRigidBody = GetComponent<Rigidbody2D>();
    }

    #region Movement

    private void Move(float _fAcceleration, float _fDeceleration, Vector2 _moveInput)
    {
        if (_moveInput != Vector2.zero)
        {
            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.bRunHeld)
            {
                targetVelocity = new Vector2(_moveInput.x, 0.0f) * MoveStats.MaxRunSpeed;
            }
            else
            {
                targetVelocity = new Vector2(_moveInput.x, 0.0f) * MoveStats.MaxWalkSpeed;
            }

            m_MoveVelocity = Vector2.Lerp(m_MoveVelocity, targetVelocity, _fAcceleration * Time.fixedDeltaTime);
            m_PlayerRigidBody.velocity = new Vector2(m_MoveVelocity.x, m_PlayerRigidBody.velocity.y);
        }
        else if (_moveInput == Vector2.zero)
        {
            m_MoveVelocity = Vector2.Lerp(m_MoveVelocity, Vector2.zero, _fDeceleration * Time.fixedDeltaTime);
            m_PlayerRigidBody.velocity = new Vector2(m_MoveVelocity.x, m_PlayerRigidBody.velocity.y);
        }
    }

    #endregion
}

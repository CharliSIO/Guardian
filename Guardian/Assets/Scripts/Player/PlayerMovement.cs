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

    // jump variables
    public float VerticalVelocity { get; private set; }
    private bool m_bJumping;
    private bool m_bFastFalling;
    private bool m_bFalling;
    private float m_fFastFallTime;
    private float m_fFastFallReleaseSpeed;
    private int m_iNumberOfJumpsUsed;

    // apex of jump curve variables
    private float m_fApexPoint;
    private float m_fTimePastApexThreshold;
    private bool m_bIsPastApexThreshold;

    // jump buffer variables
    private float m_fJumpBufferTimer;
    private float m_fJumpReleasedDuringBuffer;

    // coyote time variables
    private float m_fCoyoteTimer;

    private void Awake()
    {
        m_bFacingRight = true;
        m_PlayerRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (m_bGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.v2Movement);
        }
        else { Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.v2Movement); }
    }

    #region Movement

    private void Move(float _fAcceleration, float _fDeceleration, Vector2 _moveInput)
    {
        if (_moveInput != Vector2.zero)
        {
            TurnCheck(_moveInput);

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
    
    private void TurnCheck(Vector2 _moveInput)
    {
        if (m_bFacingRight && _moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!m_bFacingRight && _moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool _bTurnRight)
    {
        if (_bTurnRight)
        {
            m_bFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            m_bFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion
    #region Jump

    private void JumpChecks()
    {
        // WHEN PLAYER PRESSES THE JUMP BUTTON

        // WHEN PLAYER RELEASES THE JUMP BUTTON

        // INITIATE THE JUMP WITH BUFFERING AND COYOTE TIME
    }

    private void Jump()
    {

    }

    #endregion

    #region Collision Checks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(m_FeetCollider.bounds.center.x, m_FeetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(m_FeetCollider.bounds.size.x, MoveStats.GroundDetectionRayLength);

        // casts a box through the world and returns a raycast holding the collider of the ground hit, to the distance 'GroundDetectionRayLength' set in the scriptable object.
        // Layer is used to only check for ground colliders not all colliders
        m_GroundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0.0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);

        if (m_GroundHit.collider != null)
        {
            m_bGrounded = true;
        }
        else { m_bGrounded = false; }
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }


    #endregion
}

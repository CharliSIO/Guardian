using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
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
    //public float m_MoveVelocity.y { get; private set; }
    //private bool m_bJumping;
    //private bool m_bFastFalling;
    //private bool m_bFalling;
    //private float m_fFastFallTime;
    //private float m_fFastFallReleaseSpeed;
    private int m_iNumberOfJumpsUsed;

    private bool m_bEndedJumpEarly;
    private bool m_bJumpToUse = false;

    // apex of jump curve variables
    private float m_fTimePastApexThreshold;
    private bool m_bIsPastApexThreshold;

    // jump buffer variables
    private float m_fJumpBufferTimer;

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
        CountTimers();
    }

    private void FixedUpdate()
    {
        CollisionChecks();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    #region Movement

    private void HandleDirection()
    {
        TurnCheck(InputManager.v2Movement);

        if (InputManager.v2Movement.x == 0)
        {
            var deceleration = m_bGrounded ? MoveStats.GroundDeceleration : MoveStats.AirDeceleration;
            m_MoveVelocity.x = Mathf.MoveTowards(m_MoveVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            var currentMaxSpeed = InputManager.bRunHeld ? MoveStats.MaxRunSpeed : MoveStats.MaxWalkSpeed;
            m_MoveVelocity.x = Mathf.MoveTowards(m_MoveVelocity.x, InputManager.v2Movement.x * currentMaxSpeed, MoveStats.GroundAcceleration * Time.fixedDeltaTime);
        }
        
        /*
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
        */
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
        if (InputManager.bJumpPressed)
        {
            m_fJumpBufferTimer = MoveStats.JumpBufferTime;
            m_bJumpToUse = true;
        }
    }
    /*
        // WHEN PLAYER RELEASES THE JUMP BUTTON
        if (InputManager.bJumpReleased)
        {
            if (m_fJumpBufferTimer > 0.0f)
            {
                m_bJumpReleasedDuringBuffer = true;
            }

            if (m_bJumping && m_MoveVelocity.y > 0.0f)
            {
                if (m_bIsPastApexThreshold)
                {
                    m_bIsPastApexThreshold = false;
                    m_bFastFalling = true;
                    m_fFastFallTime = MoveStats.TimeForUpwardsCancel;
                    m_MoveVelocity.y = 0.0f;
                }
                else
                {
                    m_bFastFalling = true;
                    m_fFastFallReleaseSpeed = m_MoveVelocity.y;
                }
            }
        }

        // INITIATE THE JUMP WITH BUFFERING AND COYOTE TIME
        if (m_fJumpBufferTimer > 0.0f && !m_bJumping && (m_bGrounded || m_fCoyoteTimer > 0.0f))
        {
            InitiateJump(1);

            if (m_bJumpReleasedDuringBuffer)
            {
                m_bFastFalling = true;
                m_fFastFallReleaseSpeed = m_MoveVelocity.y;
            }
        }
        else if (m_fJumpBufferTimer > 0.0f && m_bJumping && m_iNumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed) // JUMP WITH DOUBLE JUMP
        {
            m_bFastFalling = false;
            InitiateJump(1);
        }
        else if (m_fJumpBufferTimer > 0.0f && m_bFalling && m_iNumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed) // AIR JUMP AFTER COYOTE TIME LAPSED
        {
            InitiateJump(2);
            m_bFastFalling = false;
        }

        // LANDING
        if ((m_bJumping || m_bFalling) && m_bGrounded && m_MoveVelocity.y <= 0.0f)
        {
            m_bJumping = false;
            m_bFalling = false;
            m_bFastFalling = false;
            m_fFastFallTime = 0.0f;
            m_bIsPastApexThreshold = false;
            m_iNumberOfJumpsUsed = 0;

            m_MoveVelocity.y = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int _iNumberOfJumpsUsed)
    {
        if (!m_bJumping)
        {
            m_bJumping = true;
        }

        m_fJumpBufferTimer = 0.0f;
        m_iNumberOfJumpsUsed += _iNumberOfJumpsUsed;
        m_MoveVelocity.y = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // Apply gravity while jumping
        if (m_bJumping)
        {
            if (m_bBumpedHead) // check for head nump
            {
                m_bFastFalling = true;
            }

            // gravity on ascending
            if (m_MoveVelocity.y >= 0.0f)
            {
                // apex controls
                m_fApexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0.0f, m_MoveVelocity.y);
                Debug.Log(m_fApexPoint > MoveStats.ApexThreshold);
                if (m_fApexPoint > MoveStats.ApexThreshold)
                {
                    if (!m_bIsPastApexThreshold)
                    {
                        m_bIsPastApexThreshold = true;
                        m_fTimePastApexThreshold = 0.0f;
                    }
                    else
                    {
                        m_fTimePastApexThreshold += Time.deltaTime;
                        if (m_fTimePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            m_MoveVelocity.y = 0.0f;
                        }
                        else
                        {
                            m_MoveVelocity.y = -0.01f;
                        }
                    }
                }

                // gravity on ascending but not yet at apex (top of jump curve/parabola)
                else
                {
                    m_MoveVelocity.y += MoveStats.Gravity * Time.deltaTime;
                    if (m_bIsPastApexThreshold)
                    {
                        m_bIsPastApexThreshold = false;
                    }
                }
            }

            // gravity on descending
            else if (!m_bFastFalling)
            {
                m_MoveVelocity.y += MoveStats.Gravity * MoveStats.JumpEndEarlyGravityModifier * Time.fixedDeltaTime;
            }
            else if (m_MoveVelocity.y < 0.0f)
            {
                if (!m_bFalling)
                {
                    m_bFalling = true;
                }
            }
        }

        // jump cut
        if (m_bFastFalling)
        {
            if (m_fFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                m_MoveVelocity.y += MoveStats.Gravity * MoveStats.JumpEndEarlyGravityModifier * Time.fixedDeltaTime;
            }
            else if (m_fFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                m_MoveVelocity.y = Mathf.Lerp(m_fFastFallReleaseSpeed, 0.0f, (m_fFastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            m_fFastFallTime += Time.fixedDeltaTime;
        }
        // normal gravity while falling
        if (!m_bGrounded && !m_bJumping)
        {
            if (!m_bFalling)
            {
                m_bFalling = true;
            }

            m_MoveVelocity.y += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        // clamp fall speed
        m_MoveVelocity.y = Mathf.Clamp(m_MoveVelocity.y, -MoveStats.MaxFallSpeed, 50f);

        m_PlayerRigidBody.velocity = new Vector2(m_PlayerRigidBody.velocity.x, m_MoveVelocity.y);
    }
    */

    private void HandleJump()
    {
        if ((!m_bEndedJumpEarly && !m_bGrounded && !InputManager.bJumpHeld && m_PlayerRigidBody.velocity.y > 0) || m_bBumpedHead)
        {
            m_bEndedJumpEarly = true;
        }

        if (m_iNumberOfJumpsUsed > MoveStats.NumberOfJumpsAllowed && !(m_fJumpBufferTimer > 0.0f)) return;

        if (m_bJumpToUse && (m_bGrounded || m_fCoyoteTimer > 0.0f || m_iNumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)) ExecuteJump();
    }

    private void ExecuteJump()
    {
        m_bJumpToUse = false;
        m_bEndedJumpEarly = false;
        m_fJumpBufferTimer = 0.0f;
        m_MoveVelocity.y = MoveStats.JumpPower;
        m_iNumberOfJumpsUsed++;
    }

    private void HandleGravity()
    {
        if (m_bGrounded && m_MoveVelocity.y <= 0.0f)
        {
            m_MoveVelocity.y = MoveStats.GroundingForce;
        }
        else
        {
            var inAirGravity = MoveStats.FallAcceleration;
            if (m_bEndedJumpEarly && m_MoveVelocity.y > 0)
            {
                inAirGravity *= MoveStats.JumpEndEarlyGravityModifier;
            }

            m_MoveVelocity.y = Mathf.MoveTowards(m_MoveVelocity.y, -MoveStats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
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
            m_iNumberOfJumpsUsed = 0;
        }
        else { m_bGrounded = false; }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(m_FeetCollider.bounds.center.x, m_BodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(m_FeetCollider.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        // casts a box through the world and returns a raycast holding the collider of the ground hit, to the distance 'GroundDetectionRayLength' set in the scriptable object.
        // Layer is used to only check for ground colliders not all colliders
        m_HeadHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0.0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);

        if (m_HeadHit.collider != null)
        {
            m_bBumpedHead = true;
        }
        else { m_bBumpedHead = false; }
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        m_fJumpBufferTimer -= Time.deltaTime;
        if (!m_bGrounded)
        {
            m_fCoyoteTimer -= Time.deltaTime;
        }
        else
        {
            m_fCoyoteTimer = MoveStats.JumpCoyoteTime;
        }
    }

    #endregion

    private void ApplyMovement()
    {
        m_PlayerRigidBody.velocity = m_MoveVelocity;
    }
}

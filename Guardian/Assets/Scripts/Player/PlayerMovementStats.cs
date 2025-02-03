using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movemement Stats")]
// Player movement scriptable object script from YouTube @Sasquatch B Studios
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walking")]

    [Tooltip("The maximum horizontal speed when walking")] 
    public float MaxWalkSpeed = 6.5f;

    [Tooltip("The horizontal acceleration when grounded")] 
    public float GroundAcceleration = 120.0f;

    [Tooltip("The horizontal deceleration when grounded")] 
    public float GroundDeceleration = 100.0f;

    [Tooltip("The horizontal deceleration when in the air after input has stopped")] 
    public float AirDeceleration = 35.0f;

    [Header("Running")]
    [Tooltip("The maximum horizontal speed when running")] 
    public float MaxRunSpeed = 10.0f;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.1f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0.0f, 1.0f)] public float HeadWidth = 0.75f;


    [Header("Jump")]

    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 20.0f;

    [Tooltip("The gravity multiplier added when jump is released early")]
    [Range(0.01f, 5.0f)] public float JumpEndEarlyGravityModifier = 3.0f;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 30.0f;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float FallAcceleration = 50.0f;

    public int NumberOfJumpsAllowed = 2;

    /*
    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1.0f)] public float ApexThreshold = 0.97f;
    [Range(0.5f, 1.0f)] public float ApexHangTime = 0.075f;
    */

    [Header("Jump Buffer"), Tooltip("The amount of time to buffer a jump. This allows jump input before actually hitting the ground")]
    [Range(0.0f, 1.0f)] public float JumpBufferTime = 0.25f;

    [Header("Jump Coyote Time"), Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    [Range(0.0f, 1.0f)] public float JumpCoyoteTime = 0.15f;

    private void OnValidate()
    {
        LayerMask.NameToLayer("Ground");  
    }

    private void OnEnable()
    {
        LayerMask.NameToLayer("Ground");   
    }
}

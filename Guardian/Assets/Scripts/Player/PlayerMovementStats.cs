using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movemement Stats")]
// Player movement scriptable object script from YouTube @Sasquatch B Studios
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1.0f, 100.0f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50.0f)] public float GroundAcceleration = 5.0f;
    [Range(0.25f, 50.0f)] public float GroundDeceleration = 20.0f;
    [Range(0.25f, 50.0f)] public float AirAcceleration = 5.0f;
    [Range(0.25f, 50.0f)] public float AirDeceleration = 5.0f;

    [Header("Run")]
    [Range(1.0f, 100.0f)] public float MaxRunSpeed = 20.0f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0.0f, 1.0f)] public float HeadWidth = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1.0f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5.0f)] public float GravityOnReleaseMultiplier = 2.0f;
    public float MaxFallSpeed = 26.0f;
    [Range(1.0f, 5.0f)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1.0f)] public float ApexThreshold = 0.97f;
    [Range(0.5f, 1.0f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0.0f, 1.0f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0.0f, 1.0f)] public float JumpCoyoteTime = 0.1f;

    [Header("Debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;

    [Header("JumpVisualisation Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualisationSteps = 90;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    private void OnValidate()
    {
        CalculateValues();   
    }

    private void OnEnable()
    {
        CalculateValues();   
    }

    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * JumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }

}

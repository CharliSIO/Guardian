using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

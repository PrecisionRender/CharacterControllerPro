// Copyright © 2022 PrecisionRender. All rights reserved.

#pragma once

#include "Engine/Scripting/Script.h"
#include "Engine/Core/Common.h"

class CharacterController;

API_CLASS() class GAME_API CharacterControllerPro : public Script
{
    API_AUTO_SERIALIZATION();
    DECLARE_SCRIPTING_TYPE(CharacterControllerPro);


public:
    API_ENUM()
    enum class MovementModes
    {
        Stopped,
        Walking,
        Running,
        Crouching
    };


    API_FIELD(Attributes = "ExpandGroups, EditorOrder(0), EditorDisplay(\"Movement\")")
    /// <summary>
    /// Determines how fast the character reaches its maximum speed.
    /// </summary>
    float Acceleration;

    API_FIELD(Attributes = "EditorOrder(1), EditorDisplay(\"Movement\")")
    /// <summary>
    /// Determines how fast the character will slow down.
    /// </summary>
    float Deceleration;

    API_FIELD(Attributes = "EditorOrder(2), EditorDisplay(\"Movement\")")
    /// <summary>
    /// The multiplier for how much control the character will have on the ground.
    /// </summary>
    float Friction;

    API_FIELD(Attributes = "EditorOrder(3), EditorDisplay(\"Movement\")")
    /// <summary>
    /// The speed which the character visuals will rotate at to match the input direction.
    /// </summary>
    float VisualsRotationSpeed;

    API_FIELD(Attributes = "EditorOrder(4), EditorDisplay(\"Movement\")")
    /// <summary>
    /// The MovementMode of the character controls its speed.
    /// </summary>
    MovementModes MovementMode;

    API_FIELD(Attributes = "ExpandGroups, EditorOrder(5), EditorDisplay(\"Walking\")")
    /// <summary>
    /// Determines how fast the character will move when its MovementMode is set to Walking.
    /// </summary>
    float MaxSpeedWalk;

    API_FIELD(Attributes = "EditorOrder(6), EditorDisplay(\"Walking\")")
    /// <summary>
    /// Determines how fast the character will move when its MovementMode is set to Running.
    /// </summary>
    float MaxSpeedRun;

    API_FIELD(Attributes = "EditorOrder(7), EditorDisplay(\"Walking\")")
    /// <summary>
    /// Determines how fast the character will move when its MovementMode is set to Crouching.
    /// </summary>
    float MaxSpeedCrouch;

    API_FIELD(Attributes = "ExpandGroups, EditorOrder(8), EditorDisplay(\"Jumping\")")
    /// <summary>
    /// Determines how powerful the character's jumps are.
    /// </summary>
    float JumpForce;

    API_FIELD(Attributes = "EditorOrder(9), EditorDisplay(\"Jumping\")")
    /// <summary>
    /// Determines how strong the character's gravity is.
    /// </summary>
    float GravityForce;

    API_FIELD(Attributes = "EditorOrder(10), EditorDisplay(\"Jumping\")")
    /// <summary>
    /// The multiplier for how much control the character will have in the air.
    /// </summary>
    float AirControl;

    API_FIELD(Attributes = "EditorOrder(11), EditorDisplay(\"Jumping\")")
    /// <summary>
    /// Determines how long the character can jump. Non-zero values require calling 
    /// StopJumping() when the player lets go of the jump button to allow for multi-height jumps.
    /// </summary>
    float MaxJumpHoldTime;


private:

    Vector3 _appliedVelocity;   // The velocity that controls this character's movement
    Vector3 _characterRotation; // The virtual roation of this character

    Vector3 _inputDirection;    // The normalized input direction
    Vector3 _visualsDirection;  // Direction that this character's visuals will face

    bool _isJumping;            // Determines if this character is jumping
    float _jumpHoldTime;        // How long the player has been holding the jump button

    CharacterController* _characterController;
    Actor* _visuals;


public:

    API_FUNCTION()
    /// <summary>
    /// Adds input to the character to be used in movement, such as walking.
    /// </summary>
    /// <param name="direction">Normalized direction of input.</param>
    /// <param name="scale">Scale to apply to input.</param>
    void AddMovementInput(Vector3 direction, float scale);

    API_FUNCTION()
    /// <summary>
    /// Adds a value to this character's CharacterRotation.
    /// </summary>
    /// <param name="rotation">The value added to the character's CharacterRotation.</param>
    void AddCharacterRotation(Vector3 rotation);

    API_FUNCTION()
    /// <summary>
    /// Causes the character to jump. If MaxJumpHoldTime is a non-zero value, the character will continue jumping
    /// until the jump has lasted longer than MaxJumpHoldTime or when StopJumping() is called.
    /// </summary>
    void Jump();

    API_FUNCTION()
    /// <summary>
    /// If the character is jumping and MaxJumpHoldTime is a non-zero value, this ends the jump.
    /// </summary>
    void StopJumping();

    API_FUNCTION()
    /// <summary>
    /// Resets the character's velocity immediately.
    /// </summary>
    void StopMovementImmediately();

    API_FUNCTION()
    /// <summary>
    /// Modifies the character's velocity directily. Best used in one-off use cases such as dashing.
    /// </summary>
    /// <param name="newVelocity">The velocity that will be applied to the character.</param>
    /// <param name="isAdditive">If true, newVelocity will be added to the 
    /// character's velocity instead of replacing it.</param>
    void LaunchCharacter(Vector3 newVelocity, bool isAdditive = false);


    API_FUNCTION()
    /// <summary>
    /// Gets the CharacterController of the character.
    /// </summary>
    FORCE_INLINE CharacterController* GetCharacterController() const { return _characterController; };

    API_FUNCTION()
    /// <summary>
    /// Gets the CharacterRotation of the character, which can be used to determine the direction of the character.
    /// </summary>
    FORCE_INLINE Vector3 GetCharacterRotation() const { return _characterRotation; };

    API_FUNCTION()
    /// <summary>
    /// Returns true of the character is jumping, false if otherwise.
    /// </summary>
    FORCE_INLINE bool IsJumping() const { return _isJumping; };

private:

    // [Script]
    void OnStart() override;
    void OnFixedUpdate() override;

    void HandleLateralMovement();
    void HandleVerticalMovement();
    void HandleRotation();
};

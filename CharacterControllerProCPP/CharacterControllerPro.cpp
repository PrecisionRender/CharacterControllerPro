// Copyright © 2022 PrecisionRender. All rights reserved.

#include "CharacterControllerPro.h"
#include "Engine/Debug/DebugLog.h"
#include "Engine/Engine/Time.h"
#include "Engine/Physics/Colliders/CharacterController.h"
#include "Engine/Debug/DebugLog.h"

CharacterControllerPro::CharacterControllerPro(const SpawnParams& params)
    : Script(params)
{
    // Enable ticking OnUpdate and OnFixedUpdate functions
    _tickUpdate = true;
    _tickFixedUpdate = true;

    // Initialize variables
    Acceleration = 25;
    Deceleration = 40;
    Friction = 1;
    VisualsRotationSpeed = 10;
    MovementMode = MovementModes::Walking;

    MaxSpeedWalk = 600;
    MaxSpeedRun = 1000;
    MaxSpeedCrouch = 300;

    JumpForce = 900;
    GravityForce = 3500;
    AirControl = 0.2f;
    MaxJumpHoldTime = 0.2f;


    _appliedVelocity = Vector3::Zero;
    _characterRotation = Vector3::Zero;

    _inputDirection = Vector3::Zero;
    _visualsDirection = Vector3::Forward;

    _isJumping = false;
    _jumpHoldTime = 0;
}

void CharacterControllerPro::OnStart()
{
    // Get Actor refrences
    _characterController = Cast<CharacterController>(GetActor());
    _visuals = GetActor()->GetChild(TEXT("Visuals"));
}

void CharacterControllerPro::OnFixedUpdate()
{
    if (_characterController == nullptr)
    {
        String debugString = String::Format(TEXT("CharacterControllerPro script should be attached to a CharacetrController Actor, instead is attached to a {0}."), GetActor()->GetType().ToString());
        DebugLog::Log(LogType::Error, debugString);
        return;
    }

    // Normalize input
    if (_inputDirection.Length() > 1)
    {
        _inputDirection = _inputDirection.GetNormalized();
    }

    // Handle movement and rotation logic
    HandleLateralMovement();
    HandleVerticalMovement();
    HandleRotation();
    
    // Move character
    _characterController->Move(_appliedVelocity * Time::GetDeltaTime());

    // If we are on the ground, apply small downward force to keep us grounded
    if (_characterController->IsGrounded())
    {
        _appliedVelocity.Y = -200;
    }

    // Reset input
    _inputDirection = Vector3::Zero;
}


void CharacterControllerPro::AddMovementInput(Vector3 direction, float scale)
{
    _inputDirection += direction * scale;
}

void CharacterControllerPro::AddCharacterRotation(Vector3 rotation)
{
    _characterRotation += rotation;
}

void CharacterControllerPro::Jump()
{
    if (_characterController->IsGrounded() && MovementMode != MovementModes::Stopped)
    {
        _isJumping = true;
    }
}

void CharacterControllerPro::StopJumping()
{
    _isJumping = false;
    _jumpHoldTime = 0;
}

void CharacterControllerPro::StopMovementImmediately()
{
    _appliedVelocity = Vector3::Zero;
}

void CharacterControllerPro::LaunchCharacter(Vector3 newVelocity, bool isAdditive)
{
    if (isAdditive)
    {
        _appliedVelocity += newVelocity;
    }
    else
    {
        _appliedVelocity = newVelocity;
    }
}


void CharacterControllerPro::HandleLateralMovement()
{
    Vector3 movementVector = Vector3::Zero;

    // Decide what speed to use based on the character's MovementMode
    switch (MovementMode)
    {
    case MovementModes::Stopped:
        movementVector = Vector3::Zero;
        break;
    case MovementModes::Walking:
        movementVector = _inputDirection * MaxSpeedWalk;
        break;
    case MovementModes::Running:
        movementVector = _inputDirection * MaxSpeedRun;
        break;
    case MovementModes::Crouching:
        movementVector = _inputDirection * MaxSpeedCrouch;
        break;
    default:
        break;
    }

    float realAcceleration = Acceleration;
    float realDeceleration = Deceleration;

    if (_characterController->IsGrounded())
    {
        // Apply friction on the ground
        realAcceleration *= Friction;
        realDeceleration *= Friction;
    }
    else
    {
        // Reduce control in the air
        realAcceleration *= AirControl;
        realDeceleration *= AirControl;
    }

    // Don't reset the character's gravity
    movementVector.Y = _appliedVelocity.Y;

    // Interpolate to the desired speed
    if (movementVector.Length() > _appliedVelocity.Length())
    {
        // If our desired speed is higher than our current speed, use acceleration
        Vector3::SmoothStep(_appliedVelocity, movementVector, realAcceleration * Time::GetDeltaTime(), _appliedVelocity);
    }
    else
    {
        // If our desired speed is lower than our current speed, use deceleration
        Vector3::SmoothStep(_appliedVelocity, movementVector, realDeceleration * Time::GetDeltaTime(), _appliedVelocity);
    }
}

void CharacterControllerPro::HandleVerticalMovement()
{
    // Apply gravity
    _appliedVelocity.Y -= GravityForce * Time::GetDeltaTime();

    // Handle Jumping
    if (_isJumping)
    {
        // Apply jump force to vertical velocity
        _appliedVelocity.Y = JumpForce;
        // Increase jumpHoldTime
        _jumpHoldTime += Time::GetDeltaTime();
    }
    // If jumpHoldTime is greater than MaxJumpHoldTime, stop the jump
    if (_jumpHoldTime >= MaxJumpHoldTime)
    {
        StopJumping();
    }
}

void CharacterControllerPro::HandleRotation()
{
    if (_visuals == nullptr)
    {
        DebugLog::Log(LogType::Error, TEXT("Unable to find Visuals Actor."));
        return;
    }

    // Don't rotate character visuals if we are stopped
    if (MovementMode == MovementModes::Stopped)
    {
        return;
    }

    // If we're giving input, change the visuals target rotation direction to the input direction
    if (_inputDirection.Length() > 0)
    {
        _visualsDirection = _inputDirection.GetNormalized();
    }

    // Rotate visuals (e.g. charcater mesh) to rotate towards target rotation direction
    Quaternion NewVisualsRotation = Quaternion::Lerp(_visuals->GetOrientation(), Quaternion::LookRotation(_visualsDirection, Vector3::Up), VisualsRotationSpeed * Time::GetDeltaTime());
    _visuals->SetOrientation(NewVisualsRotation);
}
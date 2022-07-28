// Copyright © 2022 PrecisionRender. All rights reserved.

#include "CameraArm.h"
#include "CharacterControllerPro.h"
#include "Engine/Level/Actor.h"
#include "Engine/Level/Actors/Camera.h"
#include "Engine/Engine/Time.h"
#include "Engine/Physics/Physics.h"
#include "Engine/Input/Input.h"
#include "Engine/Debug/DebugLog.h"


CameraArm::CameraArm(const SpawnParams& params)
    : Script(params)
{
    // Enable ticking OnUpdate and OnFixedUpdate functions
    _tickUpdate = true;
    _tickFixedUpdate = true;

    // Initialize variables
    ArmLength = 600;
    ShouldUseCollision = true;
    ArmCollisionMask;
    CollisionSphereRadius = 20;
    CameraOffset = Vector3::Zero;
    CameraSmoothSpeed = 30;
    PitchLimit = Vector2(-89, 89);

    _targetPitch = 0;
    _currentPitch = 0;
    _currentYaw = 0;
}

void CameraArm::OnStart()
{
    // Get Actor references
    _playerController = GetActor()->GetParent()->GetScript<CharacterControllerPro>();
    _camera = GetActor()->GetChild<Camera>();
}

void CameraArm::OnUpdate()
{
    if (_playerController == nullptr)
    {
        DebugLog::Log(LogType::Error, TEXT("CameraArm must be a child of a CharacterControllerPro Actor."));
        return;
    }

    // Get look input
    Vector2 lookInput = Vector2(Input::GetAxis(TEXT("Mouse X")), Input::GetAxis(TEXT("Mouse Y")));

    _targetPitch += lookInput.Y;
    // Clamp target pitch to keep from looking upside down
    _targetPitch = Math::Clamp(_targetPitch, PitchLimit.X, PitchLimit.Y);

    // Add character rotation
    _playerController->AddCharacterRotation(Vector3(0, lookInput.X, 0));

    // Interpolate camera arm towards the desired rotation
    _currentPitch = Math::Lerp(_currentPitch, _targetPitch, CameraSmoothSpeed * Time::GetDeltaTime());
    _currentYaw = Math::Lerp(_currentYaw, _playerController->GetCharacterRotation().Y, CameraSmoothSpeed * Time::GetDeltaTime());

    // Apply rotation
    GetActor()->SetOrientation(Quaternion::Euler(_currentPitch, _currentYaw, 0));
}

void CameraArm::OnFixedUpdate()
{
    if (_camera == nullptr)
    {
        DebugLog::Log(LogType::Error, TEXT("CameraArm must have a Camera as a child."));
        return;
    }

    RayCastHit hitInfo;
    // Cast sphere
    if (ShouldUseCollision && Physics::SphereCast(GetActor()->GetPosition(), CollisionSphereRadius, GetActor()->GetTransform().GetBackward(), hitInfo, ArmLength, ArmCollisionMask))
    {
        // If sphere hits something, move camera out of collided body
        _camera->SetPosition(GetActor()->GetPosition() + GetActor()->GetTransform().GetForward() * ArmLength * (hitInfo.Distance / ArmLength));
    }
    else
    {
        // Set camera position to desired arm length
        _camera->SetPosition(GetActor()->GetPosition() + GetActor()->GetTransform().GetBackward() * ArmLength);
    }

    // Offset camera using CameraOffset
    _camera->SetLocalPosition(_camera->GetLocalPosition() + CameraOffset);
}

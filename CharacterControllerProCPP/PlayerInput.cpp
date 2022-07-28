// Copyright © 2022 PrecisionRender. All rights reserved.

#include "PlayerInput.h"
#include "CharacterControllerPro.h"
#include "Engine/Core/Common.h"
#include "Engine/Level/Actor.h"
#include "Engine/Input/Input.h"
#include "Engine/Engine/Screen.h"
#include "Engine/Debug/DebugLog.h"

PlayerInput::PlayerInput(const SpawnParams& params)
    : Script(params)
{
    // Enable ticking OnUpdate function
    _tickUpdate = true;
}

void PlayerInput::OnStart()
{
    // Get CharacterControllerPro script
    _playerController = GetActor()->GetScript<CharacterControllerPro>();
}

void PlayerInput::OnUpdate()
{
    if (_playerController == nullptr)
    {
        DebugLog::Log(LogType::Error, TEXT("PlayerInput cannot find CharacterControllerPro script."));
        return;
    }

    // Lock mouse cursor
    Screen::SetCursorVisible(false);
    Screen::SetCursorLock(CursorLockMode::Locked);

    // Get forward and right direction based on the charcater's CharcaterRotation
    Vector3 forwardDirection = Vector3::Transform(Vector3::Forward, Quaternion::Euler(_playerController->GetCharacterRotation()));
    Vector3 rightDirection = Vector3::Transform(Vector3::Right, Quaternion::Euler(_playerController->GetCharacterRotation()));

    // Add movement in those directions
    _playerController->AddMovementInput(forwardDirection, Input::GetAxis(TEXT("Vertical")));
    _playerController->AddMovementInput(rightDirection, Input::GetAxis(TEXT("Horizontal")));

    // Trigger jumping
    if (Input::GetAction(TEXT("Jump")))
    {
        _playerController->Jump();
    }
    if (Input::GetAction(TEXT("Stop Jump")))
    {
        _playerController->StopJumping();
    }

    // Trigger running
    if (Input::GetAction(TEXT("Run")))
    {
        _playerController->MovementMode = CharacterControllerPro::MovementModes::Running;
    }
    if (Input::GetAction(TEXT("Stop Run")))
    {
        _playerController->MovementMode = CharacterControllerPro::MovementModes::Walking;
    }
}

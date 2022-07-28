// Copyright © 2022 PrecisionRender. All rights reserved.

#pragma once

#include "Engine/Scripting/Script.h"
#include "Engine/Core/Common.h"
#include "Engine/Core/Types/LayersMask.h"

class Camera;
class CharacterControllerPro;

API_CLASS() class GAME_API CameraArm : public Script
{
API_AUTO_SERIALIZATION();
DECLARE_SCRIPTING_TYPE(CameraArm);


public:

    API_FIELD(Attributes = "EditorOrder(0)")
    /// <summary>
    /// Length of the camera arm.
    /// </summary>
    float ArmLength;

    API_FIELD(Attributes = "EditorOrder(1)")
    /// <summary>
    /// If enabled, the CameraArm will keep the camera from clipping into colliders.
    /// </summary>
    bool ShouldUseCollision;

    API_FIELD(Attributes = "EditorOrder(2), VisibleIf(\"ShouldUseCollision\")")
    /// <summary>
    /// Mask for the arm collision. Useful for preventing the camera from colliding with the player.
    /// </summary>
    LayersMask ArmCollisionMask;

    API_FIELD(Attributes = "EditorOrder(3), VisibleIf(\"ShouldUseCollision\")")
    /// <summary>
    /// Size of the sphere to be used used when sphere casting.
    /// </summary>
    float CollisionSphereRadius;

    API_FIELD(Attributes = "EditorOrder(4)")
    /// <summary>
    /// Additional positional offset to apply to camera after collision checks.
    /// </summary>
    Vector3 CameraOffset;

    API_FIELD(Attributes = "EditorOrder(4)")
    /// <summary>
    /// Controls how quickly camera reaches target position.
    /// </summary>
    float CameraSmoothSpeed;

    API_FIELD(Attributes = "EditorOrder(5)")
    /// <summary>
    /// Determines the minimum and maximum pitch values. 
    /// Used to prevent the camera from going upside-down.
    /// </summary>
    Vector2 PitchLimit;


private:

    float _targetPitch;
    float _currentPitch;
    float _currentYaw;

    Camera* _camera;
    CharacterControllerPro* _playerController;


private:

    // [Script]
    void OnStart() override;
    void OnUpdate() override;
    void OnFixedUpdate() override;
};

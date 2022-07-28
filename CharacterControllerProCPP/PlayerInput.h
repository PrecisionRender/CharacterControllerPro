// Copyright © 2022 PrecisionRender. All rights reserved.

#pragma once

#include "Engine/Scripting/Script.h"

API_CLASS() class GAME_API PlayerInput : public Script
{
API_AUTO_SERIALIZATION();
DECLARE_SCRIPTING_TYPE(PlayerInput);

private:

    class CharacterControllerPro* _playerController;


private:

    // [Script]
    void OnStart() override;
    void OnUpdate() override;
};

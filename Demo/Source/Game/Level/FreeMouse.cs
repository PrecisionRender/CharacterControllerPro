// Copyright © 2022 PrecisionRender. All rights reserved.

using FlaxEngine;

namespace Game
{
    /// <summary>
    /// FreeMouse Script.
    /// </summary>
    public class FreeMouse : Script
    {
        /// <inheritdoc/>
        public override void OnUpdate()
        {
            Screen.CursorVisible = true;
            Screen.CursorLock = CursorLockMode.None;
        }
    }
}

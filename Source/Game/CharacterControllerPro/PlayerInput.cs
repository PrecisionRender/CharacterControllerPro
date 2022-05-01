using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// PlayerInput Script. Can be used to send input to a CharcaterControllerPro.
    /// </summary>
    public class PlayerInput : Script
    {
        private CharacterControllerPro controller;

        /// <inheritdoc/>
        public override void OnStart()
        {
            controller = Actor.GetScript<CharacterControllerPro>();
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            // Lock mouse cursor
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            // Get forward and right direction based on the charcater's CharcaterRotation
            Vector3 forwardDirection = Vector3.Transform(Vector3.Forward, Quaternion.Euler(controller.GetCharacterRotation()));
            Vector3 rightDirection = Vector3.Transform(Vector3.Right, Quaternion.Euler(controller.GetCharacterRotation()));

            // Add movement in those direcitons
            controller.AddMovementInput(forwardDirection, Input.GetAxis("Vertical"));
            controller.AddMovementInput(rightDirection, Input.GetAxis("Horizontal"));

            // Trigger jumping
            if (Input.GetAction("Jump"))
            {
                controller.Jump();
            }
            if (Input.GetAction("Stop Jump"))
            {
                controller.StopJumping();
            }

            // Trigger running
            if (Input.GetAction("Run"))
            {
                controller.SetMovementMode(CharacterControllerPro.MovementModes.Running);
            }
            if (Input.GetAction("Stop Run"))
            {
                controller.SetMovementMode(CharacterControllerPro.MovementModes.Walking);
            }
        }
    }
}

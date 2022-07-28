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
        private CharacterControllerPro _playerController;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _playerController = Actor.GetScript<CharacterControllerPro>();
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            // Lock mouse cursor
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            // Get forward and right direction based on the charcater's CharcaterRotation
            Vector3 forwardDirection = Vector3.Transform(Vector3.Forward, Quaternion.Euler(_playerController.CharacterRotation));
            Vector3 rightDirection = Vector3.Transform(Vector3.Right, Quaternion.Euler(_playerController.CharacterRotation));

            // Add movement in those directions
            _playerController.AddMovementInput(forwardDirection, Input.GetAxis("Vertical"));
            _playerController.AddMovementInput(rightDirection, Input.GetAxis("Horizontal"));

            // Trigger jumping
            if (Input.GetAction("Jump"))
            {
                _playerController.Jump();
            }
            if (Input.GetAction("Stop Jump"))
            {
                _playerController.StopJumping();
            }

            // Trigger running
            if (Input.GetAction("Run"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Running;
            }
            if (Input.GetAction("Stop Run"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Walking;
            }
        }
    }
}

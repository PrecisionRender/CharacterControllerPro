using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// FirstPersonCamera Script.
    /// </summary>
    public class FirstPersonCamera : Script
    {
        public float CameraSmoothSpeed = 30;

        [Serialize, ShowInEditor, EditorOrder(5), EditorDisplay(name: "Pitch Limit")]
        private Vector2 pitchLimit = new Vector2(-89, 89);

        private CharacterControllerPro _playerController;

        private float _targetPitch = 0;
        private float _currentPitch = 0;
        private float _currentYaw = 0;


        /// <inheritdoc/>
        public override void OnStart()
        {
            _playerController = Actor.Parent.GetScript<CharacterControllerPro>();
        }

        public override void OnUpdate()
        {
            // Get look input
            Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            _targetPitch += lookInput.Y;
            // Clamp target pitch to keep from looking upside down
            _targetPitch = Mathf.Clamp(_targetPitch, pitchLimit.X, pitchLimit.Y);

            // Add character rotation
            _playerController.AddCharacterRotation(new Vector3(0, lookInput.X, 0));

            // Interpolate camera arm towards the desired rotation
            _currentPitch = Mathf.SmoothStep(_currentPitch, _targetPitch, CameraSmoothSpeed * Time.DeltaTime);
            _currentYaw = Mathf.SmoothStep(_currentYaw, _playerController.CharacterRotation.Y, CameraSmoothSpeed * Time.DeltaTime);

            // Apply rotation
            Actor.EulerAngles = new Vector3(_currentPitch, _currentYaw, 0);
        }
    }
}

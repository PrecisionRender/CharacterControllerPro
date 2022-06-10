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

        private CharacterControllerPro playerController;

        private float targetPitch = 0;
        private float currentPitch = 0;
        private float currentYaw = 0;


        /// <inheritdoc/>
        public override void OnStart()
        {
            playerController = Actor.Parent.GetScript<CharacterControllerPro>();
        }

        public override void OnUpdate()
        {
            // Get look input
            Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            targetPitch += lookInput.Y;
            // Clamp target pitch to keep from looking upside down
            targetPitch = Mathf.Clamp(targetPitch, pitchLimit.X, pitchLimit.Y);

            // Add character rotation
            playerController.AddCharacterRotation(new Vector3(0, lookInput.X, 0));

            // Interpolate camera arm towards the desired rotation
            currentPitch = Mathf.SmoothStep(currentPitch, targetPitch, CameraSmoothSpeed * Time.DeltaTime);
            currentYaw = Mathf.SmoothStep(currentYaw, playerController.CharacterRotation.Y, CameraSmoothSpeed * Time.DeltaTime);

            // Apply rotation
            Actor.EulerAngles = new Vector3(currentPitch, currentYaw, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// CameraArm Script.
    /// </summary>
    public class CameraArm : Script
    {
        [EditorOrder(0)]
        public float ArmLength = 600;
        [EditorOrder(1)]
        public bool ShouldUseCollision = true;
        [EditorOrder(2), VisibleIf("ShouldUseCollision")]
        public LayersMask ArmCollisionMask;
        [EditorOrder(3), VisibleIf("ShouldUseCollision")]
        public float CollisionOffset = 12f;
        [EditorOrder(4)]
        public Vector3 CameraOffset = Vector3.Zero;
        [EditorOrder(5)]
        public float CameraSmoothSpeed = 30;

        [Serialize, ShowInEditor, EditorOrder(6), EditorDisplay(name: "Pitch Limit")]
        private Vector2 pitchLimit = new Vector2(-89, 89);

        private Camera camera;
        private CharacterControllerPro playerController;

        private float targetPitch = 0;
        private float currentPitch = 0;
        private float currentYaw = 0;


        /// <inheritdoc/>
        public override void OnStart()
        {
            camera = Actor.GetChild<Camera>();
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
            currentYaw = Mathf.SmoothStep(currentYaw, playerController.GetCharacterRotation().Y, CameraSmoothSpeed * Time.DeltaTime);

            Actor.EulerAngles = new Vector3(currentPitch, currentYaw, 0);
        }

        /// <inheritdoc/>
        public override void OnFixedUpdate()
        {
            if (ShouldUseCollision && Physics.RayCast(Actor.Position, Actor.Transform.Backward, out RayCastHit hit, ArmLength, ArmCollisionMask))
            {
                // If ray hits something, move camera out of collided body
                camera.Position = hit.Point;
            }
            else
            {
                // Set camera position to desired arm length, and offset it slightly to keep from clipping into walls
                camera.Position = Actor.Position + Transform.Backward * (ArmLength + -CollisionOffset);
            }

            camera.LocalPosition += CameraOffset;
        }
    }
}

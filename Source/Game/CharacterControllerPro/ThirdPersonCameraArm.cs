using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// ThirdPersonCameraArm Script.
    /// </summary>
    public class ThirdPersonCameraArm : Script
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

        [Serialize, ShowInEditor, EditorOrder(5), EditorDisplay(name: "Pitch Limit")]
        private Vector2 _pitchLimit = new Vector2(-89, 89);

        private Camera _camera;
        private CharacterControllerPro _playerController;

        private float _targetPitch = 0;
        private float _currentPitch = 0;
        private float _currentYaw = 0;


        /// <inheritdoc/>
        public override void OnStart()
        {
            _camera = Actor.GetChild<Camera>();
            _playerController = Actor.Parent.GetScript<CharacterControllerPro>();
        }

        public override void OnUpdate()
        {
            // Get look input
            Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            _targetPitch += lookInput.Y;
            // Clamp target pitch to keep from looking upside down
            _targetPitch = Mathf.Clamp(_targetPitch, _pitchLimit.X, _pitchLimit.Y);

            // Add character rotation
            _playerController.AddCharacterRotation(new Vector3(0, lookInput.X, 0));

            // Interpolate camera arm towards the desired rotation
            _currentPitch = Mathf.SmoothStep(_currentPitch, _targetPitch, CameraSmoothSpeed * Time.DeltaTime);
            _currentYaw = Mathf.SmoothStep(_currentYaw, _playerController.CharacterRotation.Y, CameraSmoothSpeed * Time.DeltaTime);

            // Apply rotation
            Actor.EulerAngles = new Vector3(_currentPitch, _currentYaw, 0);
        }

        /// <inheritdoc/>
        public override void OnFixedUpdate()
        {
            // Cast ray
            if (ShouldUseCollision && Physics.RayCast(Actor.Position, Actor.Transform.Backward, out RayCastHit hit, ArmLength, ArmCollisionMask))
            {
                // If ray hits something, move camera out of collided body, and offset it slightly to keep from clipping into walls
                _camera.Position = hit.Point + Transform.Forward * CollisionOffset;
            }
            else
            {
                // Set camera position to desired arm length
                _camera.Position = Actor.Position + Transform.Backward * ArmLength;
            }

            // Offset camera using CameraOffset
            _camera.LocalPosition += CameraOffset;
        }
    }
}

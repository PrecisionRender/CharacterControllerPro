// Copyright © 2022 PrecisionRender. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// CharacterControllerPro Script. A powerful and customizable way to control a charcater.
    /// </summary>
    public class CharacterControllerPro : Script
    {
        public enum MovementModes
        {
            Stopped,
            Walking,
            Running,
            Crouching
        }

        [ExpandGroups]
        [EditorOrder(0), EditorDisplay("Movement")]
        public float Acceleration = 25;
        [EditorOrder(1), EditorDisplay("Movement")]
        public float Deceleration = 40;
        [EditorOrder(2), EditorDisplay("Movement")]
        public float Friction = 1;
        [EditorOrder(3), EditorDisplay("Movement")]
        public float VisualsRotationSpeed = 10;
        [EditorOrder(4), EditorDisplay("Movement")]
        public MovementModes MovementMode = MovementModes.Walking;

        [ExpandGroups]
        [EditorOrder(5), EditorDisplay("Walking")]
        public float MaxSpeedWalk = 600;
        [EditorOrder(6), EditorDisplay("Walking")]
        public float MaxSpeedRun = 1000;
        [EditorOrder(7), EditorDisplay("Walking")]
        public float MaxSpeedCrouch = 300;

        [ExpandGroups]
        [EditorOrder(8), EditorDisplay("Jumping")]
        public float JumpForce = 900;
        [EditorOrder(9), EditorDisplay("Jumping")]
        public float GravityForce = 3500;
        [EditorOrder(10), EditorDisplay("Jumping")]
        public float AirControl = 0.2f;
        [EditorOrder(11), EditorDisplay("Jumping")]
        public float MaxJumpHoldTime = 0.2f;


        [HideInEditor]
        public Vector3 CharacterRotation
        {
            get { return _characterRotation; }
        }

        [HideInEditor]
        public bool IsJumping
        {
            get { return _isJumping; }
        }


        private Vector3 _appliedVelocity = Vector3.Zero;
        private Vector3 _characterRotation = Vector3.Zero;

        private Vector3 _inputDirection = Vector3.Zero;
        private Vector3 _visualsDirection = Vector3.Forward;

        private bool _isJumping = false;
        private float _jumpHoldTime = 0;

        private CharacterController _characterController;
        private Actor _visuals;


        /// <inheritdoc/>
        public override void OnStart()
        {
            _characterController = Actor.As<CharacterController>();
            _visuals = Actor.GetChild("Visuals");
        }

        /// <inheritdoc/>
        public override void OnFixedUpdate()
        {
            // Normalize input
            if (_inputDirection.Length > 1)
            {
                _inputDirection = _inputDirection.Normalized;
            }

            // Handle movement and rotation logic
            HandleLateralMovement();
            HandleVerticalMovement();
            HandleRotation();

            // Move character
            _characterController.Move(_appliedVelocity * Time.DeltaTime);

            // If we are on the ground, apply small downward force to keep us grounded
            if (_characterController.IsGrounded)
            {
                _appliedVelocity.Y = -200;
            }

            // Reset input
            _inputDirection = Vector3.Zero;
        }


        public void AddMovementInput(Vector3 direction, float scale)
        {
            _inputDirection += direction * scale;
        }

        public void AddCharacterRotation(Vector3 rotation)
        {
            _characterRotation += rotation;
        }

        public void Jump()
        {
            if (_characterController.IsGrounded && MovementMode != MovementModes.Stopped)
            {
                _isJumping = true;
            }
        }

        public void StopJumping()
        {
            _isJumping = false;
            _jumpHoldTime = 0;
        }

        public void StopMovementImmediately()
        {
            _appliedVelocity = Vector3.Zero;
        }

        public CharacterController GetCharacterController()
        {
            return _characterController;
        }

        public void LaunchCharacter(Vector3 newVelocity, bool isAdditive = false)
        {
            if (isAdditive)
            {
                _appliedVelocity += newVelocity;
            }
            else
            {
                _appliedVelocity = newVelocity;
            }
        }


        private void HandleLateralMovement()
        {
            Vector3 movementVector = Vector3.Zero;

            // Decide what speed to use based on the character's MovementMode
            switch (MovementMode)
            {
                case MovementModes.Stopped:
                    movementVector = Vector3.Zero;
                    break;
                case MovementModes.Walking:
                    movementVector = _inputDirection * MaxSpeedWalk;
                    break;
                case MovementModes.Running:
                    movementVector = _inputDirection * MaxSpeedRun;
                    break;
                case MovementModes.Crouching:
                    movementVector = _inputDirection * MaxSpeedCrouch;
                    break;
                default:
                    break;
            }


            float realAccel = Acceleration;
            float realDeceleration = Deceleration;


            if (_characterController.IsGrounded)
            {
                // Apply friction on the ground
                realAccel *= Friction;
                realDeceleration *= Friction;
            }
            else
            {
                // Reduce control in the air
                realAccel *= AirControl;
                realDeceleration *= AirControl;
            }

            // Don't reset the character's gravity
            movementVector.Y = _appliedVelocity.Y;

            // Interpolate to the desired speed
            if (movementVector.Length > _appliedVelocity.Length)
            {
                // If our desired speed is higher than our current speed, use acceleration
                _appliedVelocity = Vector3.SmoothStep(_appliedVelocity, movementVector, realAccel * Time.DeltaTime);
            }
            else
            {
                // If our desired speed is lower than our current speed, use deceleration
                _appliedVelocity = Vector3.SmoothStep(_appliedVelocity, movementVector, realDeceleration * Time.DeltaTime);
            }
        }

        private void HandleVerticalMovement()
        {
            // Apply gravity
            _appliedVelocity.Y -= GravityForce * Time.DeltaTime;

            // Handle Jumping
            if (_isJumping)
            {
                // Apply jump force to vertical velocity
                _appliedVelocity.Y = JumpForce;
                // Increase jumpHoldTime
                _jumpHoldTime += Time.DeltaTime;
            }
            // If jumpHoldTime is greater than MaxJumpHoldTime, stop the jump
            if (_jumpHoldTime >= MaxJumpHoldTime)
            {
                StopJumping();
            }
        }

        private void HandleRotation()
        {
            // Don't rotate character visuals if we are stopped
            if (MovementMode == MovementModes.Stopped)
            {
                return;
            }

            // If we're giving input, change the visuals target rotation direction to the input direction
            if (_inputDirection.Length > 0)
            {
                _visualsDirection = _inputDirection.Normalized;
            }

            // Rotate visuals (e.g. charcater mesh) to rotate towards input direction
            _visuals.Orientation = Quaternion.Lerp(_visuals.Orientation, Quaternion.LookRotation(_visualsDirection), VisualsRotationSpeed * Time.DeltaTime);
        }
    }
}

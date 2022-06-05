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
        public Vector3 Velocity
        {
            get { return characterController.Velocity; }
        }

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

        [HideInEditor]
        public bool IsOnGround
        {
            get { return _isOnGround; }
        }


        private Vector3 _velocity = Vector3.Zero;
        private Vector3 _characterRotation = Vector3.Zero;

        private Vector3 inputDirection = Vector3.Zero;
        private Vector3 movementDirection = Vector3.Forward;

        private bool _isJumping = false;
        private bool _isOnGround = false;
        private float jumpHoldTime = 0;

        private CharacterController characterController;
        private Actor visuals;


        /// <inheritdoc/>
        public override void OnStart()
        {
            characterController = Actor.As<CharacterController>();
            visuals = Actor.GetChild("Visuals");
        }

        /// <inheritdoc/>
        public override void OnFixedUpdate()
        {
            // Normalize input
            if (inputDirection.Length > 1)
            {
                inputDirection = inputDirection.Normalized;
            }

            HandleLateralMovement();
            HandleVerticalMovement();
            HandleRotation();

            // Move character
            characterController.Move(_velocity * Time.DeltaTime);

            // If we are on the ground, apply small downward force to keep us grounded
            if (_isOnGround)
            {
                _velocity.Y = -200;
            }

            // Reset input
            inputDirection = Vector3.Zero;
        }


        public void AddMovementInput(Vector3 direction, float scale)
        {
            inputDirection += direction * scale;
        }

        public void AddCharacterRotation(Vector3 rotation)
        {
            _characterRotation += rotation;
        }

        public void Jump()
        {
            if (_isOnGround && MovementMode != MovementModes.Stopped)
            {
                _isJumping = true;
            }
        }

        public void StopJumping()
        {
            _isJumping = false;
            jumpHoldTime = 0;
        }

        public void StopMovementImmediately()
        {
            _velocity = Vector3.Zero;
        }

        public void LaunchCharacter(Vector3 newVelocity, bool isAdditive)
        {
            if (isAdditive)
            {
                _velocity += newVelocity;
            }
            else
            {
                _velocity = newVelocity;
            }
        }


        private void HandleLateralMovement()
        {
            Vector3 movementVector = Vector3.Zero;

            // Decide what speed to use
            switch (MovementMode)
            {
                case MovementModes.Stopped:
                    movementVector = Vector3.Zero;
                    break;
                case MovementModes.Walking:
                    movementVector = inputDirection * MaxSpeedWalk;
                    break;
                case MovementModes.Running:
                    movementVector = inputDirection * MaxSpeedRun;
                    break;
                case MovementModes.Crouching:
                    movementVector = inputDirection * MaxSpeedCrouch;
                    break;
                default:
                    break;
            }


            float realAccel = Acceleration;
            float realDeceleration = Deceleration;


            if (!_isOnGround)
            {
                // Reduce control in the air
                realAccel *= AirControl;
                realDeceleration *= AirControl;
            }
            else
            {
                realAccel *= Friction;
                realDeceleration *= Friction;
            }


            movementVector.Y = _velocity.Y;

            // Interpolate to the desired speed
            if (movementVector.Length > _velocity.Length)
            {
                _velocity = Vector3.SmoothStep(_velocity, movementVector, realAccel * Time.DeltaTime);
            }
            else
            {
                _velocity = Vector3.SmoothStep(_velocity, movementVector, realDeceleration * Time.DeltaTime);
            }
        }

        private void HandleVerticalMovement()
        {
            // Apply gravity
            _velocity.Y -= GravityForce * Time.DeltaTime;

            // Handle Jumping
            if (_isJumping)
            {
                _velocity.Y = JumpForce;
                jumpHoldTime += Time.DeltaTime;
            }
            if (jumpHoldTime >= MaxJumpHoldTime)
            {
                StopJumping();
            }

            // Check if we are on the ground
            _isOnGround = characterController.IsGrounded;
        }

        private void HandleRotation()
        {
            if (inputDirection.Length > 0)
            {
                movementDirection = inputDirection.Normalized;
            }

            // Rotate visuals (e.g. charcater mesh) to rotate towards input direction
            visuals.Orientation = Quaternion.Lerp(visuals.Orientation, Quaternion.LookRotation(movementDirection), VisualsRotationSpeed * Time.DeltaTime);
        }
    }
}

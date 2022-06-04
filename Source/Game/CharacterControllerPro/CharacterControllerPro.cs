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

        [Serialize, ShowInEditor, EditorOrder(0), ReadOnly]
        private Vector3 Velocity;

        [Serialize, ShowInEditor, EditorOrder(1), ReadOnly]
        private Vector3 CharacterRotation = Vector3.Zero;

        [ExpandGroups]
        [Serialize, ShowInEditor, EditorOrder(2), EditorDisplay("Movement")]
        public float Acceleration = 25;
        [Serialize, ShowInEditor, EditorOrder(3), EditorDisplay("Movement")]
        public float Friction = 40;
        [Serialize, ShowInEditor, EditorOrder(4), EditorDisplay("Movement")]
        public float VisualsRotationSpeed = 10;
        [Serialize, ShowInEditor, EditorOrder(5), EditorDisplay("Movement")]
        public MovementModes MovementMode = MovementModes.Walking;

        [ExpandGroups]
        [Serialize, ShowInEditor, EditorOrder(6), EditorDisplay("Walking")]
        public float MaxSpeedWalk = 600;
        [Serialize, ShowInEditor, EditorOrder(7), EditorDisplay("Walking")]
        public float MaxSpeedRun = 1000;
        [Serialize, ShowInEditor, EditorOrder(8), EditorDisplay("Walking")]
        public float MaxSpeedCrouch = 300;

        [ExpandGroups]
        [Serialize, ShowInEditor, EditorOrder(9), EditorDisplay("Jumping")]
        public float JumpForce = 900;
        [Serialize, ShowInEditor, EditorOrder(10), EditorDisplay("Jumping")]
        public float GravityForce = 3500;
        [Serialize, ShowInEditor, EditorOrder(11), EditorDisplay("Jumping")]
        public float AirControl = 0.2f;
        [Serialize, ShowInEditor, EditorOrder(12), EditorDisplay("Jumping")]
        public float MaxJumpHoldTime = 0.2f;

        [HideInEditor]
        public bool IsOnGround = false;

        private Vector3 inputDirection = Vector3.Zero;
        private Vector3 movementDirection = Vector3.Forward;

        private bool isJumping = false;
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
            characterController.Move(Velocity * Time.DeltaTime);

            // If we are on the ground, apply small downward force to keep us grounded
            if (IsOnGround)
            {
                Velocity.Y = -200;
            }

            // Reset input
            inputDirection = Vector3.Zero;
        }


        public void AddMovementInput(Vector3 direction, float scale)
        {
            direction = direction.Normalized;
            scale = Mathf.Clamp(scale, -1, 1);
            inputDirection += direction * scale;
        }

        public Vector3 GetCharacterVelocity()
        {
            return Velocity;
        }

        public void AddCharacterRotation(Vector3 rotation)
        {
            CharacterRotation += rotation;
        }

        public Vector3 GetCharacterRotation()
        {
            return CharacterRotation;
        }

        public void Jump()
        {
            if (IsOnGround)
            {
                isJumping = true;
            }
        }

        public void StopJumping()
        {
            isJumping = false;
            jumpHoldTime = 0;
        }

        public bool GetIsJumping()
        {
            return isJumping;
        }

        public void StopMovementImmediately()
        {
            Velocity = Vector3.Zero;
        }

        public void SetMovementMode(MovementModes mode)
        {
            MovementMode = mode;
        }

        public void LaunchCharacter(Vector3 newVelocity)
        {
            Velocity = newVelocity;
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
            float realFriction = Friction;

            // Reduce control in the air
            if (!IsOnGround)
            {
                realAccel *= AirControl;
                realFriction *= AirControl;
            }


            movementVector.Y = Velocity.Y;

            // Interpolate to the desired speed
            if (movementVector.Length > Velocity.Length)
            {
                Velocity = Vector3.SmoothStep(Velocity, movementVector, realAccel * Time.DeltaTime);
            }
            else
            {
                Velocity = Vector3.SmoothStep(Velocity, movementVector, realFriction * Time.DeltaTime);
            }
        }

        private void HandleVerticalMovement()
        {
            // Apply gravity
            Velocity.Y -= GravityForce * Time.DeltaTime;

            // Handle Jumping
            if (isJumping)
            {
                Velocity.Y = JumpForce;
                jumpHoldTime += Time.DeltaTime;
            }
            if (jumpHoldTime >= MaxJumpHoldTime)
            {
                StopJumping();
            }

            // Check if we are on the ground
            IsOnGround = characterController.IsGrounded;
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

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
        [Serialize, ShowInEditor, EditorOrder(0), EditorDisplay("Movement")]
        public float Acceleration = 25;
        [Serialize, ShowInEditor, EditorOrder(1), EditorDisplay("Movement")]
        public float Deceleration = 40;
        [Serialize, ShowInEditor, EditorOrder(2), EditorDisplay("Movement")]
        public float Friction = 1;
        [Serialize, ShowInEditor, EditorOrder(3), EditorDisplay("Movement")]
        public float VisualsRotationSpeed = 10;
        [Serialize, ShowInEditor, EditorOrder(4), EditorDisplay("Movement")]
        public MovementModes MovementMode = MovementModes.Walking;

        [ExpandGroups]
        [Serialize, ShowInEditor, EditorOrder(5), EditorDisplay("Walking")]
        public float MaxSpeedWalk = 600;
        [Serialize, ShowInEditor, EditorOrder(6), EditorDisplay("Walking")]
        public float MaxSpeedRun = 1000;
        [Serialize, ShowInEditor, EditorOrder(7), EditorDisplay("Walking")]
        public float MaxSpeedCrouch = 300;

        [ExpandGroups]
        [Serialize, ShowInEditor, EditorOrder(8), EditorDisplay("Jumping")]
        public float JumpForce = 900;
        [Serialize, ShowInEditor, EditorOrder(9), EditorDisplay("Jumping")]
        public float GravityForce = 3500;
        [Serialize, ShowInEditor, EditorOrder(10), EditorDisplay("Jumping")]
        public float AirControl = 0.2f;
        [Serialize, ShowInEditor, EditorOrder(11), EditorDisplay("Jumping")]
        public float MaxJumpHoldTime = 0.2f;


        private Vector3 motionVelocity = Vector3.Zero;
        private Vector3 characterRotation = Vector3.Zero;

        private Vector3 inputDirection = Vector3.Zero;
        private Vector3 movementDirection = Vector3.Forward;

        private bool isJumping = false;
        private float jumpHoldTime = 0;
        private bool isOnGround = false;

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
            characterController.Move(motionVelocity * Time.DeltaTime);

            // If we are on the ground, apply small downward force to keep us grounded
            if (isOnGround)
            {
                motionVelocity.Y = -200;
            }

            // Reset input
            inputDirection = Vector3.Zero;
        }


        public void AddMovementInput(Vector3 direction, float scale)
        {
            inputDirection += direction * scale;
        }

        public Vector3 GetCharacterVelocity()
        {
            return characterController.Velocity;
        }

        public void AddCharacterRotation(Vector3 rotation)
        {
            characterRotation += rotation;
        }

        public Vector3 GetCharacterRotation()
        {
            return characterRotation;
        }

        public void Jump()
        {
            if (isOnGround && MovementMode != MovementModes.Stopped)
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

        public bool GetIsOnGround()
	{
            return isOnGround;
        }

        public void StopMovementImmediately()
        {
            motionVelocity = Vector3.Zero;
        }

        public void SetMovementMode(MovementModes mode)
        {
            MovementMode = mode;
        }

        public void LaunchCharacter(Vector3 newVelocity, bool isAdditive)
        {
            if (isAdditive)
	    {
                motionVelocity += newVelocity;
            }
	    else
	    {
                motionVelocity = newVelocity;
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

            
            if (!isOnGround)
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


            movementVector.Y = motionVelocity.Y;

            // Interpolate to the desired speed
            if (movementVector.Length > motionVelocity.Length)
            {
                motionVelocity = Vector3.SmoothStep(motionVelocity, movementVector, realAccel * Time.DeltaTime);
            }
            else
            {
                motionVelocity = Vector3.SmoothStep(motionVelocity, movementVector, realDeceleration * Time.DeltaTime);
            }
        }

        private void HandleVerticalMovement()
        {
            // Apply gravity
            motionVelocity.Y -= GravityForce * Time.DeltaTime;

            // Handle Jumping
            if (isJumping)
            {
                motionVelocity.Y = JumpForce;
                jumpHoldTime += Time.DeltaTime;
            }
            if (jumpHoldTime >= MaxJumpHoldTime)
            {
                StopJumping();
            }

            // Check if we are on the ground
            isOnGround = characterController.IsGrounded;
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

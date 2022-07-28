// Copyright © 2022 PrecisionRender. All rights reserved.

using FlaxEngine;

namespace Game
{
    /// <summary>
    /// AnimationController Script for Robot model and CharacterControllerPro.
    /// </summary>
    public class AnimationController : Script
    {
        private CharacterControllerPro _characterControllerPro;

        private AnimGraphParameter _velocity;
        private AnimGraphParameter _falling;

        public override void OnStart()
        {
            _characterControllerPro = Actor.Parent.Parent.GetScript<CharacterControllerPro>();

            // Cache parameters
            _velocity = Actor.As<AnimatedModel>().GetParameter("Velocity");
            _falling = Actor.As<AnimatedModel>().GetParameter("Falling");
        }

        public override void OnUpdate()
        {
            // Update the values
            _velocity.Value = _characterControllerPro.GetCharacterController().Velocity;
            _falling.Value = !_characterControllerPro.GetCharacterController().IsGrounded;
        }
	}
}

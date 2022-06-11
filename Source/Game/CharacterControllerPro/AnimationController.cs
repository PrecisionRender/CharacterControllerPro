using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// AnimationController Script for Robot model and CharacterControllerPro.
    /// </summary>
    public class AnimationController : Script
    {
        [Serialize, ShowInEditor, EditorDisplay(name: "Character Controller Pro")]
        private CharacterControllerPro _characterControllerPro;

        private AnimGraphParameter _velocity;
        private AnimGraphParameter _falling;

        public override void OnStart()
        {
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

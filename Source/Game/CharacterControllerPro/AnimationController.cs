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
        private CharacterControllerPro characterControllerPro;

        private AnimGraphParameter velocity;
        private AnimGraphParameter falling;

        public override void OnStart()
        {
            // Cache parameters
            velocity = Actor.As<AnimatedModel>().GetParameter("Velocity");
            falling = Actor.As<AnimatedModel>().GetParameter("Falling");
        }

        public override void OnUpdate()
        {
            // Update the values
            velocity.Value = characterControllerPro.GetCharacterController().Velocity;
            falling.Value = !characterControllerPro.GetCharacterController().IsGrounded;
        }
    }
}

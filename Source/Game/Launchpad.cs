using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Launchpad Script.
    /// </summary>
    public class Launchpad : Script
    {
        /// <inheritdoc/>
        public override void OnStart()
        {
            // Here you can add code that needs to be called when script is created, just before the first game update
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            Actor.As<Collider>().TriggerEnter += OnTriggerEnter;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Actor.As<Collider>().TriggerEnter -= OnTriggerEnter;
        }

        public void OnTriggerEnter(PhysicsColliderActor other)
		{
            CharacterControllerPro characterControllerPro = other.GetScript<CharacterControllerPro>();
            if (characterControllerPro != null)
			{
                characterControllerPro.LaunchCharacter(new Vector3(0, 2000, 0));
			}
		}
    }
}

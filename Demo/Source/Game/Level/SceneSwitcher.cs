// Copyright © 2022 PrecisionRender. All rights reserved.

using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    /// <summary>
    /// SceneSwitcher Script.
    /// </summary>
    public class SceneSwitcher : Script
    {
        [Serialize, ShowInEditor, EditorDisplay(name: "Next Scene"), EditorOrder(0)]
        private SceneReference nextScene;
        [Serialize, ShowInEditor, EditorDisplay(name: "Keybinding"), EditorOrder(1)]
        private KeyboardKeys key = KeyboardKeys.None;

        /// <inheritdoc/>
        public override void OnStart()
        {
            // Here you can add code that needs to be called when script is created, just before the first game update
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            if (Actor.As<UIControl>().Get<Button>() != null)
			{
                Actor.As<UIControl>().Get<Button>().Clicked += SwitchScene;
            }
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            if (Actor.As<UIControl>().Get<Button>() != null)
            {
                Actor.As<UIControl>().Get<Button>().Clicked -= SwitchScene;
            }
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (key == KeyboardKeys.None)
            {
                return;
			}
            if (Input.GetKey(key))
			{
                SwitchScene();
			}
        }

        private void SwitchScene()
		{
            Level.ChangeSceneAsync(nextScene);
		}
    }
}

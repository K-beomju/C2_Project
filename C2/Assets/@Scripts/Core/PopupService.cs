using System.Collections.Generic;
using C2Project.Addressable;
using UnityEngine;
using Zenject;

namespace C2Project.Core
{
    public class PopupService : IInitializable
    {

        private UI_Base _sceneUI = null;
        public UI_Base SceneUI
        {
            get { return _sceneUI; }
            set { _sceneUI = value; }
        }

        public void Initialize()
        {

        }

        public void SetCurrentSceneUI(UI_Base sceneUI)
        {
            SceneUI = sceneUI;
        }

    }

}
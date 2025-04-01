using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;


namespace C2Project.Core
{
    public class SceneService
    {
        public void LoadScene(EScene type, bool isAsync = true)
        {
            if (isAsync)
            {
                SceneManager.LoadSceneAsync(GetSceneName(type));
            }
            else
            {
                SceneManager.LoadScene(GetSceneName(type));
            }
        }

        private string GetSceneName(EScene type)
        {
            string name = Enum.GetName(typeof(EScene), type);
            return name;
        }
    }
}
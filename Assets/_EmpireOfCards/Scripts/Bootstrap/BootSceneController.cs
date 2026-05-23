using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Minimal splash screen controller. Shows splash for a configurable
    /// duration then loads the MainMenu scene.
    /// Place on a GameObject in Boot.unity.
    /// </summary>
    public class BootSceneController : MonoBehaviour
    {
        [SerializeField] private float splashDuration = 2f;
        [SerializeField] private string nextScene = "MainMenu";

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= splashDuration)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}

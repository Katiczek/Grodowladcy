using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Grodowladcy.Presentation
{
    /// <summary>
    /// Main menu actions for this slice. New game loads MapView. Resume reports that no save exists and does not load a scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private Text messageText;

        public void NewGame()
        {
            SceneManager.LoadScene("MapView");
        }

        public void Resume()
        {
            messageText.text = "Brak zapisu do wznowienia.";
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Grodowladcy.Presentation
{
    /// <summary>
    /// Main menu actions for this slice. Resume reports that no save exists and does not load a scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private Text messageText;

        public void Resume()
        {
            messageText.text = "Brak zapisu do wznowienia.";
        }
    }
}

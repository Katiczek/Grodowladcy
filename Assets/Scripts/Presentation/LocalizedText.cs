using UnityEngine;
using UnityEngine.UI;

namespace Grodowladcy.Presentation
{
    /// <summary>
    /// Writes the catalog string for <see cref="key"/> onto the sibling UI Text at startup.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string key;

        private void Awake()
        {
            GetComponent<Text>().text = GameText.Get(key);
        }
    }
}

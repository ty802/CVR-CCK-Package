using UnityEngine;
using UnityEngine.UI;

namespace ABI.CCK.Scripts
{
    [AddComponentMenu("")]
    public class CCKLocalizationReplacer : MonoBehaviour
    {
        public string identifier;

        public void Start()
        {
            var text = gameObject.GetComponent<Text>();
            if (text != null)
                text.text = CCKLocalizationProvider.GetLocalizedText(identifier);
        }
    }
}
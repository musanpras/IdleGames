using TMPro;
using UnityEngine;

    public class TextLocalize : MonoBehaviour
    {
        public string key;
        public string sheet;
        private void Start()
        {
            Localize();
        }
        public void SwitchLanguage(LanguageCode code)
        {
            Localize();
        }
        public void Localize()
        {
            TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
            string text = string.IsNullOrEmpty(key) ? "" : ((!string.IsNullOrEmpty(sheet)) ? Language.Get(key, sheet) : Language.Get(key));
            if (component != null)
            {
                component.text = text;
            }
        }
    }

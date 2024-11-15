using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Rendering;
using UnityEngine;
namespace DIALOGUE
{
    [System.Serializable]
    public class NameContainer
    {
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private TextMeshProUGUI nameText;
        public void Show(string nameToShow = "")
        {
            //Debug.Log("check3");
            root.SetActive(true);
            if (nameToShow != string.Empty)
            {
                nameText.text = nameToShow;
            }
        }
        public void Hide()
        {
            root.SetActive(false);
        }
        public void SetNameColor(Color color) => nameText.color = color;
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
    }
}
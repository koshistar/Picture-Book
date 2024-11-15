using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer namecontainer;
        public TextMeshProUGUI dialogueText;
        public void SetDialogueColor(Color color)=>dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font)=>dialogueText.font = font;
    }
}
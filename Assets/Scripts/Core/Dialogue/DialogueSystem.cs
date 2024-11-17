using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;
using System.Xml.Serialization;
namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField]
        private DialogueConfigurationSO _config;
        public DialogueConfigurationSO config => _config;
        public DialogueContainer dialogueContainer = new DialogueContainer();
       // public NameContainer nameContainer = new NameContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;
        [SerializeField] private CanvasGroup mainCanvas;
        public static DialogueSystem instance { get; private set; }
        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;
        public bool isRunningConversation => conversationManager.isRunning;
        public DialogueContinuePrompt prompt;
        private CanvasGroupController cgController;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
            cgController = new CanvasGroupController(this, mainCanvas);
            dialogueContainer.Initialize();
        }
        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }
        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character =CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);
            ApplySpeakerDataToDialogueContainer(config);
        }
        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            //Debug.Log("check4");
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            float fontSize = this.config.defaultDialogueFontSize * this.config.dialogueFontSacle * config.dialogueFontScale;
            dialogueContainer.SetDialogueFontSize(fontSize);
            dialogueContainer.namecontainer.SetNameColor(config.nameColor);
            dialogueContainer.namecontainer.SetNameFont(config.nameFont);
            fontSize = this.config.defaultNameFontSize * config.nameFontScale;
            dialogueContainer.namecontainer.SetNameFontSize(fontSize);
        }
        public void ShowSpeakerName(string speakerName = "")
        {
            //Debug.Log("check2");
            if (speakerName.ToLower() != "narrator")
            {
                dialogueContainer.namecontainer.Show(speakerName);
            }
            else
                HideSpeakerName();
        }
        public void HideSpeakerName()=>dialogueContainer.namecontainer.Hide();
        public Coroutine Say(string speaker,string dialogue)
        {
            List<string> conversation=new List<string>() { $"{speaker}\"{dialogue}\"" };
            return Say(conversation);
        }
        public Coroutine Say(List<string> conversation)
        {
            //Debug.Log("check5");
            return conversationManager.StartConversation(conversation);
        }
        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
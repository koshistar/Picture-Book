using CHARACTERS;
using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning =>process != null;
        private TextArchitect architect = null;
        private bool userPrompt = false;
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
        }
        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }
        public Coroutine StartConversation(List<string> conversation)
        {
            //Debug.Log("check6");
            StopConversation();
            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
            return process;
        }
        public void StopConversation() 
        {
            //Debug.Log("check7");
            if (!isRunning)
            {
                return;
            }
            dialogueSystem.StopCoroutine(process);
            process = null;
        }
        IEnumerator RunningConversation(List<string> conversation)
        {
            //Debug.Log("check8");
            for (int i = 0; i < conversation.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;
                DIALOGUE_LINE line = Dialoguepaser.Parse(conversation[i]);
                if (line.hasDialogue)
                {
                    yield return LINE_RunningDialogue(line);
                }
                if (line.hasCommands)
                {
                    yield return LINE_RunningCommand(line);
                }
                if (line.hasDialogue)
                {
                    yield return WaitForUserInput();
                    CommandManager.instance.StopAllProcess();
                }
            }
        }
        IEnumerator LINE_RunningDialogue(DIALOGUE_LINE line)
        {
            //Debug.Log("check10");
            if (line.hasSpeaker)
            {
                HandleSpeakerLogic(line.speakerData);
            }
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();
            //else
            //    dialogueSystem.HideSpeakerName();
            yield return BuildLineSegments(line.dialogueData);
            //yield return WaitForUserInput();
        }
        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);
            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);
            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
            {
                character.Show();
                //if (character == null)
                //    CharacterManager.instance.CreateCharacter(speakerData.name, revealAfterCreation: true);
                //else
                //    character.Show();
            }
            dialogueSystem.ShowSpeakerName(speakerData.displayname);
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);
            if (speakerData.isCastingPosition)
                character.MoveToPosition(speakerData.castPosition);
            if(speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
            }
        }
        IEnumerator LINE_RunningCommand(DIALOGUE_LINE line)
        {
            //Debug.Log("check14");
            //Debug.Log(line.commandsData.commands[0].name);
            List<DL_COMMAND_DATA.Command> commands = line.commandsData.commands;
            foreach(DL_COMMAND_DATA.Command command in commands)
            {
                //Debug.Log(command.name);
                //Debug.Log(command.arguments);
                //Debug.Log(command.waitForCompletion);
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while(!cw.isDone)
                    {
                        if(userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }
        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            //Debug.Log("check11");
            for (int i=0;i<line.segments.Count;i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];
                yield return WaitForDialogueSignalToBeTriggered(segment);
                yield return BuildDialogue(segment.dialogue,segment.appendText);
            }
        }
        IEnumerator WaitForDialogueSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            //Debug.Log("check12");
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay); 
                    break;
                default: break;
            }
        }
        IEnumerator BuildDialogue(string dialogue,bool append = false)
        {
            //Debug.Log("check13");
            if (!append)
            {
                architect.Build(dialogue);
            }
            else
            {
                architect.Append(dialogue);
            }
           // architect.Build(dialogue);
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                    userPrompt = false;
                }
                yield return null;
            }
        }
        IEnumerator WaitForUserInput()
        {
            dialogueSystem.prompt.Show();
            //Debug.Log("check16");
            while (!userPrompt)
            {
                yield return null;
            }
            dialogueSystem.prompt.Hide();
            userPrompt = false;
        }
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
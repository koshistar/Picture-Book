using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace DIALOGUE
{
    public class Dialoguepaser
    {
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";
        public static DIALOGUE_LINE Parse(string rawline)
        {
            //Debug.Log("check9");
            //Debug.Log($"Parsing line - '{rawline}'");
            (string speaker, string dialogue, string commands) = RipContent(rawline);
            //Debug.Log($"Speaker='{speaker}'\nDialogue='{dialogue}'\nCommands='{commands}'");
            //Debug.Log("check9-1");
            return new DIALOGUE_LINE(speaker, dialogue, commands);
        }
        private static (string,string,string) RipContent(string rawline)
        {
            string speaker = "", dialogue = "", commands = "";
            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscape = false;
            for(int i=0;i<rawline.Length;i++)
            {
                char current = rawline[i];
                if(current=='\\')
                {
                    isEscape = !isEscape;
                }
                else if(current=='"'&&!isEscape)
                {
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                else
                    isEscape = false;
            }
            Regex commandRegex=new Regex(commandRegexPattern);
            MatchCollection matches = commandRegex.Matches(rawline);
            int commandStart = -1;
            //int commandEnd = -1;
            foreach (Match match in matches)
            {
                if (match.Index < dialogueStart || match.Index > dialogueEnd)
                {
                    commandStart = match.Index;
                    break;
                }
            }
            //Debug.Log($"{speaker},{dialogue},{commands}");
            Debug.Log($"{dialogueStart},{dialogueEnd},{commandStart}");
            if (commandStart == -1 && (dialogueStart == -1 && dialogueEnd == -1))
                return ("", rawline.Trim(), "");
            if(dialogueStart!=-1&&dialogueEnd!=-1&&(commandStart==-1||commandStart>dialogueEnd)) 
            {
                //Debug.Log($"{dialogueStart} - {dialogueEnd} - {commandStart}");
                speaker = rawline.Substring(0, dialogueStart).Trim();
                dialogue = rawline.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if (commandStart != -1)
                {
                    commands=rawline.Substring(commandStart).Trim();
                }
            }
            //-1,-1,0
            else if(commandStart!=-1 && (dialogueStart>commandStart||(dialogueStart==-1&&dialogueEnd==-1)))
            {
                commands = rawline;
            }
            else
            {
                dialogue = rawline;
            }
            Debug.Log(commands);
            //Debug.Log(rawline.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1));
            return (speaker, dialogue, commands);
        }
    }
}
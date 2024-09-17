using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField]
    private TextAsset fileToRoad = null;
    // Start is called before the first frame update
    void Start()
    {
        StartConversation();
    }
    void StartConversation()
    {
        //List<string> lines = FileManager.ReadTextAsset("testFile");
        List<string> lines = FileManager.ReadTextAsset(fileToRoad);
        foreach (string line in lines)
        {
            Debug.Log($"Segmenting line '{line}'");
            //DIALOGUE_LINE dlLine = Dialoguepaser.Parse(line);
            //int i = 0;
            //foreach (DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
            //{
            //    Debug.Log($"Segment [{i++}] = '{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $"{segment.signalDelay}" : $"")}]");
            //}
        }

        //for(int i = 0;i<lines.Count; i++)
        //{
        //    string line = lines[i];
        //    if(string.IsNullOrWhiteSpace(line)) continue;
        //    DIALOGUE_LINE dl = Dialoguepaser.Parse(line);
        //    Debug.Log($"{dl.speaker.name} as [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}]at {dl.speaker.castPosition}");
        //    List<(int l, string ex)> expr = dl.speaker.CastExpression;
        //    for(int c=0;c<expr.Count;c++)
        //    {
        //        Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
        //    }
        //}

        //foreach (string line in lines)
        //{
        //    if(string.IsNullOrWhiteSpace(line)) 
        //        continue;
        //    DIALOGUE_LINE dl=Dialoguepaser.Parse(line);
        //    for(int i = 0;i<dl.commandsData.commands.Count;i++)
        //    {
        //        DL_COMMAND_DATA.Command command = dl.commandsData.commands[i];
        //        Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
        //    }
        //}

        DialogueSystem.instance.Say(lines);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

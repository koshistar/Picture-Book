using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TESTING
{
    public class TestParsing : MonoBehaviour
    {
        //[SerializeField]
        //private TextAsset file;
        // Start is called before the first frame update
        void Start()
        {
            //string line = "Speaker \"Dialogue Goes In \\\"Here\\\"!\" Command(arguments here)";
            //Dialoguepaser.Parse(line);
            SendFileToParse();
        }

        void SendFileToParse()
        {
            List<string> list = FileManager.ReadTextAsset("testFile");
            foreach (string s in list) 
            {
                if (s == string.Empty)
                    continue;
                DIALOGUE_LINE dl =Dialoguepaser.Parse(s);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
namespace TESTING
{
    public class Testing1 : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architext;
        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;
        string[] lines = new string[5]
        {
            "よこうそいらっしゃいました",
            "低挫",
            "低垳吭才厘怏匯渦徨赤錦宅",
            "tatakai!tatakai!!",
            "What's coming on."
        };
        // Start is called before the first frame update 
        void Start()
        {
            ds = DialogueSystem.instance;
            architext = new TextArchitect(ds.dialogueContainer.dialogueText);
            architext.buildMethod = TextArchitect.BuildMethod.fade;
            architext.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            //string longLine = "What's up,this is a long line, may be.";
            if (bm != architext.buildMethod)
            {
                architext.buildMethod = bm;
                architext.Stop();
            }
            if (Input.GetKeyDown(KeyCode.S)) 
            {
                architext.Stop();
            }
           if (Input.GetKeyDown(KeyCode.Space))
                if (architext.isBuilding)
                {
                    if (!architext.hurryUp)
                        architext.hurryUp = true;
                    else
                        architext.ForceComplete();
                }
                else
                    //architext.Build(longLine);
                    architext.Build(lines[Random.Range(0, lines.Length)]);
           else if(Input.GetKeyDown(KeyCode.A))
            {
                //architext.Append(longLine);
                architext.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }
}
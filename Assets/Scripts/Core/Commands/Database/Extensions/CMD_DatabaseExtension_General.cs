using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DataBaseExtension
    {
        private static string[] PARAM_SPEED = new string[] { "-s", "-spd", "speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        new public static void Extend(CommandDataBase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("showdb", new Func<string[],IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[],IEnumerator>(HideDialogueBox));
            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));
            database.AddCommand("nextscene", new Action<string>(ChangeToScene));
        }
        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time)) 
            {
                yield return new WaitForSeconds(time);
            }
        }
        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            yield return DialogueSystem.instance.dialogueContainer.Show(speed,immediate);
        }
        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            yield return DialogueSystem.instance.dialogueContainer.Hide(speed,immediate);
        }
        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            yield return DialogueSystem.instance.Show(speed, immediate);
        }
        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            yield return DialogueSystem.instance.Hide(speed, immediate);
        }
        private static void ChangeToScene(string data)
        {
            int num = Convert.ToInt32(data);
            SceneManager.LoadScene(num);
        }
    }
}
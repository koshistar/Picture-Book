using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DataBaseExtension
    {
        new public static void Extend(CommandDataBase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));
        }
        private static IEnumerator Wait(string data)
        {
            if(float.TryParse(data, out float time)) 
            {
                yield return new WaitForSeconds(time);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{
    public class DL_COMMAND_DATA
    {
        public List<Command> commands;
        private const char COMMADSPLITTER_ID = ',';
        private const char ARGUMENTSCONTAINER_ID = '(';
        private const string WAITCOMMAND_ID = "[wait]";
        public struct Command
        {
            public string name;
            public string[] arguments;
            public bool waitForCompletion;
        }
        public DL_COMMAND_DATA(string rawCommands)
        {
            //Debug.Log("check9-6");
            commands = RipCommands(rawCommands);
        }
        private List<Command> RipCommands(string rawCommands)
        {
            //Debug.Log("check9-7");
            string[] data = rawCommands.Split(COMMADSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> result = new List<Command>();
            foreach (string cmd in data)
            {
                Command command = new Command();
                int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
            //Debug.Log("check9-7-1");
            //Debug.Log($"{cmd}  {index}");
                if(index!=-1)
                    command.name = cmd.Substring(0, index).Trim();
            //Debug.Log("check9-8");
                if (command.name.ToLower().StartsWith(WAITCOMMAND_ID))
                {
                    command.name = command.name.Substring(WAITCOMMAND_ID.Length);
                    command.waitForCompletion = true;
                }
                else
                    command.waitForCompletion = false;
                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
                Debug.Log(command.arguments[0]);
                result.Add(command);
            }
            //Debug.Log("check9-9");
            return result;
        }
        private string[] GetArgs(string args)
        {
            List<string> argList = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQutes = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '"')
                {
                    inQutes = !inQutes;
                    continue;
                }
                if (!inQutes && args[i] == ' ')
                {
                    argList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }
                currentArg.Append(args[i]);
            }
            if (currentArg.Length > 0)
            {
                argList.Add(currentArg.ToString());
            }
            return argList.ToArray();
        }
    }
}
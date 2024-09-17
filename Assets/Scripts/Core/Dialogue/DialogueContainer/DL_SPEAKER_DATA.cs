using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    public class DL_SPEAKER_DATA
    {
        public string name, castName;
        public string displayname => isCastingName ? castName : name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }
        public bool isCastingName => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;
        public bool makeCharacterEnter = false;
        private const string NAMECAST_ID = " as ";
        private const string POSITIONCAST_ID = " at ";
        private const string EXPRESSIONCAST_ID = " [";
        private const char AXISDELIMITER_ID = ':';
        private const char EXPRESSIONLAYER_DELIMITER = ':';
        private const char EXPRESSIONLAYER_JOINER = ',';
        private const string ENTER_KEYWORD = "enter ";
        private string ProcessKeywords(string rawSpeaker)
        {
            if(rawSpeaker.StartsWith(ENTER_KEYWORD))
            {
                rawSpeaker=rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharacterEnter = true;
            }
            return rawSpeaker;
        }
        public DL_SPEAKER_DATA(string rawspeaker)
        {
            rawspeaker = ProcessKeywords(rawspeaker);
            //×¢Òâ¿Õ¸ñ
            string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawspeaker, pattern);
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();
            if (matches.Count == 0)
            {
                name = rawspeaker;
                return;
            }
            int index = matches[0].Index;
            name = rawspeaker.Substring(0, index);
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;
                if (match.Value == NAMECAST_ID)
                {
                    Debug.Log("Cheak1");
                    startIndex = match.Index + NAMECAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawspeaker.Length;
                    castName = rawspeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;
                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawspeaker.Length;
                    string castPos = rawspeaker.Substring(startIndex, endIndex - startIndex);
                    string[] axis = castPos.Split(AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);
                    float.TryParse(axis[0], out castPosition.x);
                    if (axis.Length > 1)
                    {
                        float.TryParse(axis[1], out castPosition.y);
                    }
                }
                else if (match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawspeaker.Length;
                    string castExp = rawspeaker.Substring(startIndex, endIndex - startIndex - 1);
                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER).Select(x =>
                    {
                        var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);
                        if (parts.Length == 2)
                            return (int.Parse(parts[0]), parts[1]);
                        else
                            return (0, parts[0]);
                    }).ToList();
                }
            }
        }
    }
}
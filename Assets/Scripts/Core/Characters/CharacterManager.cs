using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CHARACTERS
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance {  get; private set; }
        public Character[] allCharacters => characters.Values.ToArray();
        private Dictionary<string,Character> characters= new Dictionary<string,Character>();
        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;
        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";
        public string CharacterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";
        public string characterPrefabPathFormat => $"{CharacterRootPathFormat}/{characterPrefabNameFormat}";
        [SerializeField]
        private RectTransform _characterpanel = null;
        [SerializeField]
        private RectTransform _characterpanel_Live2D = null; 
        [SerializeField]
        private Transform _characterpanel_Model3D = null;
        public RectTransform characterPanel => _characterpanel;
        public RectTransform characterPanel_Live2D => _characterpanel_Live2D;
        public Transform characterPanel_Model3D => _characterpanel_Model3D;
        private void Awake()
        {
            instance = this;
        }
        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }
        public Character GetCharacter(string characterName,bool createIfDoesNotExist = false)
        {
            if(characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if(createIfDoesNotExist)
            {
                return CreateCharacter(characterName);
            }
            return null;
        }
        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());
        public Character CreateCharacter(string characterName,bool revealAfterCreation = false)
        {
            if(characters.ContainsKey(characterName.ToLower())) 
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists.");
                return null;
            }
            CHARACTER_INFO info = GetCharacterInfo(characterName);
            Character character = CreateCharacterFromInfo(info);
            characters.Add(info.name.ToLower(), character);
            if (revealAfterCreation)
                character.Show();
            return character;
        }
        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result=new CHARACTER_INFO();
            string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            //Debug.Log(result.castingName);
            result.config = config.GetConfig(result.castingName);
            result.prefab=GetPrefabForCharacter(result.castingName);
            result.rootCharacterFolder = FormatCharacterPath(CharacterRootPathFormat, result.castingName);
            return result;
        }
        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }
        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);
        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;
            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config,info.prefab, info.rootCharacterFolder);
                default:
                    return null;
            }
        }
        public void SortCharacters()
        {
            List<Character> activeCharacters = characters.Values.Where(c => c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<Character> inactiveCharacters = characters.Values.Except(activeCharacters).ToList();
            activeCharacters.Sort((a, b) => a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inactiveCharacters);
            SortCharacters(activeCharacters);
        }
        public void SortCharacters(string[] characterNames)
        {
            List<Character> sortedCharacters = new List<Character>();
            sortedCharacters = characterNames.Select(name => GetCharacter(name)).Where(character => character != null).ToList();
            List<Character> remainingCharacters = characters.Values.Except(sortedCharacters).OrderBy(character => character.priority).ToList();
            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c => c.priority) : 0;
            for(int i=0;i<sortedCharacters.Count;i++)
            {
                Character character = sortedCharacters[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharacterOnUI: false);
            }
            sortedCharacters.Reverse();
            List<Character> allCharacters = remainingCharacters.Concat(remainingCharacters).ToList();
            SortCharacters(allCharacters);
        }
        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach(Character character in charactersSortingOrder)
            {
                Debug.Log($"{character.name}/{character.priority}");
                character.root.SetSiblingIndex(i++);
                character.OnSort(i);
            }
        }
        public int GetCharacterCountFromCharacterType(Character.CharacterType charType)
        {
            int count = 0;
            foreach(var c in characters.Values)
            {
                if (c.config.characterType == charType)
                {
                    count++;
                }
            }
            return count;
        }
        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";
            public string rootCharacterFolder = "";
            public CharacterConfigData config = null;
            public GameObject prefab = null;
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
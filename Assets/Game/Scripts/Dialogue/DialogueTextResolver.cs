using Assets.Game.Scripts.Dialogue.Enums;
using Assets.Game.Scripts.Dialogue.Enums.Attributes;
using System;

namespace Assets.Game.Scripts.Dialogue
{ 
    public static class DialogueTextResolver
    {
        public static string Resolve(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw.IndexOf('{') < 0) return raw;

            foreach (Character character in Enum.GetValues(typeof(Character)))
            {
                string token = "{" + character + "}";
                if (raw.Contains(token))
                    raw = raw.Replace(token, StringValueAttribute.GetStringValue(character));
            }

            return raw;
        }
    }
}

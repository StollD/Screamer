using System;
using System.Collections.Generic;

namespace Screamer
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.TRACKSTATION, GameScenes.EDITOR, GameScenes.FLIGHT)]
    public class GameStorage : ScenarioModule
    {
        /// <summary>
        /// All values that are stored in the savegame
        /// </summary>
        public Dictionary<String, String> Values;

        public override void OnLoad(ConfigNode node)
        {
            Values = new Dictionary<String, String>();
            foreach (ConfigNode.Value value in node.values)
            {
                Values.Add(value.name, value.value);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            foreach (KeyValuePair<String, String> kVP in Values)
            {
                node.AddValue(kVP.Key, kVP.Value);
            }
        }
    }

    public static class GameStorageExtension
    {
        public static String Get(this Game game, String key)
        {
            GameStorage storage = (GameStorage)game.scenarios.Find(s => s.moduleName == "GameStorage").moduleRef;
            if (storage.Values.ContainsKey(key))
            {
                return storage.Values[key];
            }
            else
            {
                return null;
            }
        }

        public static void Set(this Game game, String key, String value)
        {
            GameStorage storage = (GameStorage)game.scenarios.Find(s => s.moduleName == "GameStorage").moduleRef;
            if (storage.Values.ContainsKey(key))
            {
                storage.Values[key] = value;
            }
            else
            {
                storage.Values.Add(key, value);
            }
        }
    }
}

using Kopernicus;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Screamer
{
    [RequireConfigType(ConfigType.Node)]
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ScreamBehaviour : MonoBehaviour, IParserEventSubscriber
    {
        /// <summary>
        /// A list of all loaded scream configs
        /// </summary>
        [ParserTargetCollection("self")]
        public List<Scream> screams { get; set; }

        /// <summary>
        /// A list with all implemented conditions
        /// </summary>
        public static Dictionary<String, Func<Boolean>> Conditions { get; set; }

        /// <summary>
        /// A list with all implemented variables
        /// </summary>
        public static Dictionary<String, Func<String>> Variables { get; set; }

        void Start()
        {
            // Keep this alive
            DontDestroyOnLoad(this);

            // Get the config file from GameData
            ConfigNode config = GameDatabase.Instance.GetConfigs("SCREAMER")[0].config;

            // Load everything from it
            Parser.LoadObjectFromConfigurationNode(this, config);
        }

        void IParserEventSubscriber.Apply(ConfigNode node)
        {
            // Create the collection
            Conditions = new Dictionary<String, Func<Boolean>>();
            Variables = new Dictionary<String, Func<String>>();

            // Select all methods that match
            foreach (Type type in Parser.ModTypes)
            {
                foreach (MethodInfo info in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    // The method is a scream condition
                    ScreamConditionAttribute[] conditionAttributes = info.GetCustomAttributes(typeof(ScreamConditionAttribute), false) as ScreamConditionAttribute[];
                    if (conditionAttributes != null && conditionAttributes.Length > 0)
                    {
                        // The method has to return a boolean
                        if (info.ReturnType != typeof(Boolean))
                        {
                            continue;
                        }

                        // The method has to be parameterless
                        if (info.GetParameters().Length > 0)
                        {
                            continue;
                        }

                        // Store the method
                        Func<Boolean> dlg = (Func<Boolean>)Delegate.CreateDelegate(typeof(Func<Boolean>), info);
                        Conditions.Add(conditionAttributes[0].Name, dlg);
                        Debug.Log("[Screamer] Found Condition: " + conditionAttributes[0].Name);
                        continue;
                    }


                    // The method is a scream variable
                    ScreamVariableAttribute[] variableAttributes = info.GetCustomAttributes(typeof(ScreamVariableAttribute), false) as ScreamVariableAttribute[];
                    if (variableAttributes != null && variableAttributes.Length > 0)
                    {
                        // The method has to return a string
                        if (info.ReturnType != typeof(String))
                        {
                            continue;
                        }

                        // The method has to be parameterless
                        if (info.GetParameters().Length > 0)
                        {
                            continue;
                        }

                        // Store the method
                        Func<String> dlg = (Func<String>)Delegate.CreateDelegate(typeof(Func<String>), info);
                        Variables.Add(variableAttributes[0].Name, dlg);
                        Debug.Log("[Screamer] Found Variable: " + variableAttributes[0].Name);
                        continue;
                    }


                    // The method is a scream trigger
                    ScreamTriggerAttribute[] triggerAttributes = info.GetCustomAttributes(typeof(ScreamTriggerAttribute), false) as ScreamTriggerAttribute[];
                    if (triggerAttributes != null && triggerAttributes.Length > 0)
                    {
                        // The method has to return a void
                        if (info.ReturnType != typeof(void))
                        {
                            continue;
                        }

                        // The method needs one parameter
                        if (info.GetParameters().Length == 1)
                        {
                            continue;
                        }

                        // The parameter needs to be an action
                        if (info.GetParameters()[0].ParameterType != typeof(Action))
                        {
                            continue;
                        }

                        // Pass the ProcessScreams method to the event
                        info.Invoke(null, new[] { new Action(ProcessScreams) });
                        Debug.Log("[Screamer] Found Trigger: " + triggerAttributes[0].Name);
                        continue;
                    }
                }
            }
        }

        void IParserEventSubscriber.PostApply(ConfigNode node)
        {
            // Invoke the screams for main menu
            ProcessScreams();
        }

        public void ProcessScreams()
        {
            // Trigger all screams
            foreach (Scream scream in screams)
            {
                scream.Process();
            }
        }
    }
}

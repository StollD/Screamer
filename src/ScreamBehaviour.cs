using Kopernicus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// A list with all implemented actions
        /// </summary>
        public static Dictionary<String, Action> Actions { get; set; }
        
        /// <summary>
        /// The Instance of the Scream Behaviour
        /// </summary>
        public static ScreamBehaviour Instance { get; set; }

        void Start()
        {
            // If this assembly wasn't elected, shut down
            if (!SelectAssembly())
            {
                Destroy(this);
                return;
            }
            
            // Keep this alive
            DontDestroyOnLoad(this);
            
            // Assign Instance
            Instance = this;

            // Get the config file from GameData
            ConfigNode[] configs = GameDatabase.Instance.GetConfigs("SCREAM").Select(c => c.config).ToArray();
            ConfigNode node = new ConfigNode();
            for (Int32 i = 0; i < configs.Length; i++)
            {
                node.AddNode(configs[i]);
            }

            // Load everything from it
            Parser.LoadObjectFromConfigurationNode(this, node);
        }
        
        /// <summary>
        /// Selects whether the assembly should get executed
        /// </summary>
        /// <returns></returns>
        public bool SelectAssembly()
        {
            // Select all screamer assemblies
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            AssemblyLoader.LoadedAssembly[] list = AssemblyLoader.loadedAssemblies
                .Where(a => a.assembly.GetName().Name == thisAssembly.GetName().Name).ToArray();
            
            // Select the most recent one
            AssemblyLoader.LoadedAssembly assembly = list.OrderByDescending(a => BuiltTime(a.assembly)).First();

            return Equals(thisAssembly, assembly.assembly);
        }

        void IParserEventSubscriber.Apply(ConfigNode node)
        {
            // Create the collection
            Conditions = new Dictionary<String, Func<Boolean>>();
            Variables = new Dictionary<String, Func<String>>();
            Actions = new Dictionary<String, Action>();

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
                        if (info.GetParameters().Length != 1)
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


                    // The method is a scream action
                    ScreamActionAttribute[] actionAttributes = info.GetCustomAttributes(typeof(ScreamActionAttribute), false) as ScreamActionAttribute[];
                    if (actionAttributes != null && actionAttributes.Length > 0)
                    {
                        // The method has to return a void
                        if (info.ReturnType != typeof(void))
                        {
                            continue;
                        }

                        // The method needs no parameters
                        if (info.GetParameters().Length != 0)
                        {
                            continue;
                        }

                        // Pass the ProcessScreams method to the event
                        Action dlg = (Action)Delegate.CreateDelegate(typeof(Action), info);
                        Actions.Add(actionAttributes[0].Name, dlg);
                        Debug.Log("[Screamer] Found Action: " + actionAttributes[0].Name);
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
        
        /// <summary>
        /// Returns the time when the assembly was built
        /// </summary>
        /// <returns></returns>
        public static DateTime BuiltTime(Assembly assembly)
        {
            String filePath = assembly.Location;
            const Int32 peHeaderOffset = 60;
            const Int32 linkerTimestampOffset = 8;
            Byte[] b = new Byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            Int32 i = BitConverter.ToInt32(b, peHeaderOffset);
            Int32 secondsSince1970 = BitConverter.ToInt32(b, i + linkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToUniversalTime();
            return dt;
        }
    }
}

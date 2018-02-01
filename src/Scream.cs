using Kopernicus;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Screamer
{
    [RequireConfigType(ConfigType.Node)]
    public class Scream
    {
        /// <summary>
        /// How should the message get displayed
        /// </summary>
        public enum ScreamMessageType
        {
            PopupDialog,
            ScreenMessage,
            Debug
        }

        [ParserTarget("name", optional = false)]
        public String name;

        // Should the scream get displayed multiple times?
        [ParserTarget("once")]
        public NumericParser<Boolean> once = false;

        // How long (in seconds) should the program wait after a scene change before it displays the scream?
        [ParserTarget("delay")]
        public NumericParser<Single> delay = 0f;

        // When should the scream get displayed?
        [ParserTarget("condition", optional = false)]
        public StringCollectionParser condition;

        // How should the scream get displayed?
        [ParserTarget("type", optional = false)]
        public EnumParser<ScreamMessageType> type;

        // The position of the ScreenMessage on the screen
        [ParserTarget("style")]
        public EnumParser<ScreenMessageStyle> style = ScreenMessageStyle.UPPER_CENTER;

        // How long (in seconds) should the screen message get displayed?
        [ParserTarget("duration")]
        public NumericParser<Single> duration = 2f;

        // The position of the PopupDialog on the screen
        [ParserTarget("position")]
        public Vector2Parser position = Vector2.one * 100f;

        // Which title should the popup dialog have?
        [ParserTarget("title", optional = false)]
        public String title;

        // The message that will be displayed
        [ParserTarget("message", optional = false)]
        public String message;

        [ParserTarget("button")]
        public String button = "OK";

        /// <summary>
        /// Whether the message was already shown in this KSP session
        /// </summary>
        private Boolean _shown { get; set; }

        /// <summary>
        /// Displays the scream if the condition evals to true
        /// </summary>
        public void Process()
        {
            // Was the scream already shown?
            if (once && _shown)
            {
                return;
            }

            // Build the evaluator variables
            Boolean canExecute = true;
            foreach (String s in condition.value)
            {
                canExecute &= ScreamBehaviour.Conditions[s]();
            }
            
            // Should we continue?
            if (!canExecute)
            {
                return;
            }

            // Assemble the message
            String _message = message.Replace("\\n", "\n");
            String _title = title;
            foreach (KeyValuePair<String, Func<String>> kVP in ScreamBehaviour.Variables)
            {
                _message = _message.Replace("@" + kVP.Key, kVP.Value());
                _title = _title.Replace("@" + kVP.Key, kVP.Value());
            }

            HighLogic.fetch.StartCoroutine(DelayExecution(delay, () =>
            {
                // Display it
                if (type == ScreamMessageType.ScreenMessage)
                {
                    ScreenMessages.PostScreenMessage(_message, duration, style);
                }

                if (type == ScreamMessageType.PopupDialog)
                {
                    Single x = position.value.x / Screen.width;
                    Single y = (Screen.height - position.value.y) / Screen.height;
                    MultiOptionDialog dialog = new MultiOptionDialog(Guid.NewGuid().ToString(), _message,
                        _title, UISkinManager.GetSkin("MainMenuSkin"), new Rect(x, y, 300f, 100f),
                        new DialogGUIButton(button, () => { }, true));
                    PopupDialog.SpawnPopupDialog(new Vector2(0f, 1f), new Vector2(0f, 1f), dialog, false,
                        UISkinManager.GetSkin("MainMenuSkin"));
                }

                if (type == ScreamMessageType.Debug)
                {
                    Debug.Log("[" + _title + "] " + _message);
                }
                _shown = true;
                Debug.Log("[Screamer] Displayed scream \"" + name + "\"");
            }));
        }

        public IEnumerator<WaitForSeconds> DelayExecution(Single seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
    }
}

using Dahomey.ExpressionEvaluator;
using Kopernicus;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screamer
{
    [RequireConfigType(ConfigType.Node)]
    public class Scream : IParserEventSubscriber
    {
        public enum ScreamMessageType
        {
            PopupDialog,
            ScreenMessage
        }

        public enum OnceMode
        {
            PerGameCreation,
            PerGameSession,
            PerKSPSession,
            None
        }

        /// <summary>
        /// The expression parser that is responsible for evaluating the scream conditions
        /// </summary>
        private static ExpressionParser _parser { get; set; }

        [ParserTarget("name", optional = false)]
        public String name;

        // Should the scream get displayed multiple times?
        [ParserTarget("once")]
        public EnumParser<OnceMode> once = OnceMode.None;

        // How long (in seconds) should the program wait after a scene change before it displays the scream?
        [ParserTarget("delay")]
        public NumericParser<Single> delay = 0f;

        // When should the scream get displayed?
        [ParserTarget("condition", optional = false)]
        public String condition
        {
            set { _expression = _parser.ParseBooleanExpression(value); }
        }

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

        /// <summary>
        /// The expression that determines whether the scream should be displayed
        /// </summary>
        private IBooleanExpression _expression;

        /// <summary>
        /// A list of games where this scream was shown
        /// </summary>
        private List<Game> _shown { get; set; }

        void IParserEventSubscriber.Apply(ConfigNode node)
        {
            if (_parser == null)
            {
                _parser = new ExpressionParser();
                foreach (String key in ScreamBehaviour.Conditions.Keys)
                {
                    _parser.RegisterVariable<Boolean>(key);
                }
            }
            _shown = new List<Game>();
        }

        void IParserEventSubscriber.PostApply(ConfigNode node)
        {
            
        }

        /// <summary>
        /// Displays the scream if the condition evals to true
        /// </summary>
        public void Process()
        {
            // Was the scream already shown?
            if (once == OnceMode.PerKSPSession && _shown.Any())
            {
                return;
            }
            if (once == OnceMode.PerGameSession)
            {
                if (HighLogic.CurrentGame == null)
                {
                    return;
                }
                if (_shown.Any(g => g == HighLogic.CurrentGame))
                {
                    return;
                }
            }
            if (once == OnceMode.PerGameCreation)
            {
                if (HighLogic.CurrentGame == null)
                {
                    return;
                }
                if (HighLogic.CurrentGame.Get(name) == "True")
                {
                    return;
                }
            }

            // Build the evaluator variables
            Dictionary<String, System.Object> data = new Dictionary<String, System.Object>();
            foreach (KeyValuePair<String, Func<Boolean>> kVP in ScreamBehaviour.Conditions)
            {
                data.Add(kVP.Key, kVP.Value());
            }
            Boolean canExecute = _expression.Evaluate(data);
            
            // Should we continue?
            if (!canExecute)
            {
                return;
            }

            // Assemble the message
            String _message = message;
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
                else
                {
                    Single x = position.value.x / Screen.width;
                    Single y = (Screen.height - position.value.y) / Screen.height;
                    MultiOptionDialog dialog = new MultiOptionDialog(Guid.NewGuid().ToString(), _message,
                        _title, UISkinManager.GetSkin("MainMenuSkin"), new Rect(x, y, -1f, -1f),
                        new DialogGUIButton("OK", () => { }, true));
                    PopupDialog.SpawnPopupDialog(new Vector2(0f, 1f), new Vector2(0f, 1f), dialog, false, UISkinManager.GetSkin("MainMenuSkin"));
                }
                _shown.Add(HighLogic.CurrentGame);
                if (once == OnceMode.PerGameCreation)
                {
                    HighLogic.CurrentGame.Set(name, "True");
                }
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

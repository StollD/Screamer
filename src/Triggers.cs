using System;

namespace Screamer
{
    /// <summary>
    /// A class that contains methods defining events that trigger the screams
    /// </summary>
    public static class Triggers
    {
        [ScreamTrigger("OnLevelWasLoadedGUIReady")]
        public static void OnLevelWasLoadedGUIReady(Action action)
        {
            GameEvents.onLevelWasLoadedGUIReady.Add(s => action());
        }

        [ScreamTrigger("OnMapEntered")]
        public static void OnMapEntered(Action action)
        {
            GameEvents.OnMapEntered.Add(() => action());
        }

        [ScreamTrigger("OnMapExited")]
        public static void OnMapExited(Action action)
        {
            GameEvents.OnMapExited.Add(() => action());
        }
    }
}

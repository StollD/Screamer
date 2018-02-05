using UnityEngine;

namespace Screamer
{
    /// <summary>
    /// A class containing action definitions for screams
    /// </summary>
    public static class Actions
    {
        [ScreamAction("Dismiss")]
        public static void Dismiss()
        {
            // Do nothing
        }
        
        [ScreamAction("Quit")]
        public static void Quit()
        {
            Application.Quit();
        }
    }
}

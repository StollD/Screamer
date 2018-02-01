using System;

namespace Screamer
{
    /// <summary>
    /// A class that contains multiple definitions for scream conditions
    /// </summary>
    public static class Conditions
    {
        [ScreamCondition("IsInMainMenu")]
        public static Boolean IsInMainMenu()
        {
            return HighLogic.LoadedScene == GameScenes.MAINMENU;
        }

        [ScreamCondition("IsInSpaceCenter")]
        public static Boolean IsInSpaceCenter()
        {
            return HighLogic.LoadedScene == GameScenes.SPACECENTER;
        }

        [ScreamCondition("IsInFlight")]
        public static Boolean IsInFlight()
        {
            return HighLogic.LoadedScene == GameScenes.FLIGHT;
        }

        [ScreamCondition("IsInTrackingStation")]
        public static Boolean IsInTrackingStation()
        {
            return HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }

        [ScreamCondition("IsInMapView")]
        public static Boolean IsInMapView()
        {
            return MapView.MapIsEnabled;
        }

        [ScreamCondition("IsInPlanetarium")]
        public static Boolean IsInPlanetarium()
        {
            return IsInTrackingStation() || IsInMapView();
        }

        [ScreamCondition("IsInEditor")]
        public static Boolean IsInEditor()
        {
            return HighLogic.LoadedScene == GameScenes.EDITOR;
        }

        [ScreamCondition("IsInVAB")]
        public static Boolean IsInVAB()
        {
            return EditorDriver.editorFacility == EditorFacility.VAB && IsInEditor();
        }

        [ScreamCondition("IsInSPH")]
        public static Boolean IsInSPH()
        {
            return EditorDriver.editorFacility == EditorFacility.SPH && IsInEditor();
        }

        [ScreamCondition("IsInGame")]
        public static Boolean IsInGame()
        {
            return !IsInMainMenu();
        }

        [ScreamCondition("IsSandbox")]
        public static Boolean IsSandbox()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX;
        }

        [ScreamCondition("IsScience")]
        public static Boolean IsScience()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX;
        }

        [ScreamCondition("IsCareer")]
        public static Boolean IsCareer()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.CAREER;
        }

        private static Boolean _isNewGame;

        [ScreamCondition("IsNewGame")]
        public static Boolean IsNewGame()
        {
            return _isNewGame;
        }

        /// <summary>
        /// Figure out whether a game was newly created
        /// </summary>
        static Conditions()
        {
            GameEvents.onGameStateCreated.Add(g => _isNewGame = true);
            GameEvents.onGameSceneSwitchRequested.Add((ac) =>
            {
                if (ac.from != GameScenes.MAINMENU || ac.to != GameScenes.SPACECENTER)
                {
                    _isNewGame = false;
                }
            });
        }
    }
}

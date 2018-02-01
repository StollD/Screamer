using System;

namespace Screamer
{
    /// <summary>
    /// A class that contains multiple definitions for scream conditions
    /// </summary>
    public static class Conditions
    {
        [ScreamCondition("InMainMenu")]
        public static Boolean IsInMainMenu()
        {
            return HighLogic.LoadedScene == GameScenes.MAINMENU;
        }

        [ScreamCondition("InSpaceCenter")]
        public static Boolean IInSpaceCenter()
        {
            return HighLogic.LoadedScene == GameScenes.SPACECENTER;
        }

        [ScreamCondition("InFlight")]
        public static Boolean IsInFlight()
        {
            return HighLogic.LoadedScene == GameScenes.FLIGHT;
        }

        [ScreamCondition("InTrackingStation")]
        public static Boolean IsInTrackingStation()
        {
            return HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }

        [ScreamCondition("InMapView")]
        public static Boolean IsInMapView()
        {
            return MapView.MapIsEnabled;
        }

        [ScreamCondition("InPlanetarium")]
        public static Boolean IsInPlanetarium()
        {
            return IsInTrackingStation() || IsInMapView();
        }

        [ScreamCondition("InEditor")]
        public static Boolean IsInEditor()
        {
            return HighLogic.LoadedScene == GameScenes.EDITOR;
        }

        [ScreamCondition("InVAB")]
        public static Boolean IsInVAB()
        {
            return EditorDriver.editorFacility == EditorFacility.VAB && IsInEditor();
        }

        [ScreamCondition("InSPH")]
        public static Boolean IsInSPH()
        {
            return EditorDriver.editorFacility == EditorFacility.SPH && IsInEditor();
        }

        [ScreamCondition("InGame")]
        public static Boolean IsInGame()
        {
            return !IsInMainMenu();
        }

        [ScreamCondition("Sandbox")]
        public static Boolean IsSandbox()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX;
        }

        [ScreamCondition("Science")]
        public static Boolean IsScience()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX;
        }

        [ScreamCondition("Career")]
        public static Boolean IsCareer()
        {
            return IsInGame() && HighLogic.CurrentGame.Mode == Game.Modes.CAREER;
        }
    }
}

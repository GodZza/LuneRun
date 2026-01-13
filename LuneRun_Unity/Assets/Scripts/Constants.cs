using UnityEngine;

namespace LuneRun
{
    public static class Constants
    {
        public const string Version = "v0.82 beta 2";
        public const string Name = "Lunerun";
        public const float OriginalX = 800f;
        public const float OriginalY = 600f;
        public const string GameApiUrl = "http://api.playchilla.com/";
        public const string RunnerApiUrl = "http://api.playchilla.com/runner/";
        public static bool SkipToGame => false; // Show menu for UI testing
        
        // Level configuration
        public const int TotalLevels = 60;
        public const int LevelsPerTab = 32;
        public const int Tab1Levels = 32;
        public const int Tab2Levels = 28; // 33-60
        public const int Tab3InfiniteLevelId = 999;
        
        // Tutorial
        public const bool EnableTutorial = true;
    }
}
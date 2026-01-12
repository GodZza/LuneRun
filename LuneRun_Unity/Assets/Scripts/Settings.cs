using UnityEngine;

namespace LuneRun
{
    public class Settings
    {
        private const string PREFS_KEY_SOUND = "SoundEnabled";
        private const string PREFS_KEY_MUSIC = "MusicEnabled";
        private const string PREFS_KEY_SEEN_TUTORIAL = "SeenTutorial";
        
        private bool soundEnabled = true;
        private bool musicEnabled = true;
        private bool seenTutorial = false;
        
        public Settings()
        {
            Load();
        }
        
        public static Settings Load()
        {
            Settings settings = new Settings();
            settings.LoadFromPlayerPrefs();
            return settings;
        }
        
        private void LoadFromPlayerPrefs()
        {
            soundEnabled = PlayerPrefs.GetInt(PREFS_KEY_SOUND, 1) == 1;
            musicEnabled = PlayerPrefs.GetInt(PREFS_KEY_MUSIC, 1) == 1;
            seenTutorial = PlayerPrefs.GetInt(PREFS_KEY_SEEN_TUTORIAL, 0) == 1;
        }
        
        public void Save()
        {
            PlayerPrefs.SetInt(PREFS_KEY_SOUND, soundEnabled ? 1 : 0);
            PlayerPrefs.SetInt(PREFS_KEY_MUSIC, musicEnabled ? 1 : 0);
            PlayerPrefs.SetInt(PREFS_KEY_SEEN_TUTORIAL, seenTutorial ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        public bool GetSoundOn()
        {
            return soundEnabled;
        }
        
        public bool GetMusicOn()
        {
            return musicEnabled;
        }
        
        public void ToggleSound()
        {
            soundEnabled = !soundEnabled;
            Save();
        }
        
        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            Save();
        }
        
        public bool HasSeenTutorial()
        {
            return seenTutorial;
        }
        
        public void SetSeenTutorial()
        {
            seenTutorial = true;
            Save();
        }
    }
}
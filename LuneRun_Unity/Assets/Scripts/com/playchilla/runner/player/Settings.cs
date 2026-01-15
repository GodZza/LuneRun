namespace com.playchilla.runner.player
{
    public class Settings
    {
        private string _version;
        private bool _soundOn;
        private bool _musicOn;
        private bool _hasSeenTutorial;
        
        public Settings(string version, bool soundOn, bool musicOn, bool hasSeenTutorial)
        {
            _version = version;
            _soundOn = soundOn;
            _musicOn = musicOn;
            _hasSeenTutorial = hasSeenTutorial;
        }
        
        public void Save()
        {
            // Save to PlayerPrefs
            UnityEngine.PlayerPrefs.SetInt("SoundEnabled", _soundOn ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt("MusicEnabled", _musicOn ? 1 : 0);
            UnityEngine.PlayerPrefs.SetInt("SeenTutorial", _hasSeenTutorial ? 1 : 0);
            UnityEngine.PlayerPrefs.Save();
        }
        
        public static Settings Load()
        {
            bool soundOn = UnityEngine.PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
            bool musicOn = UnityEngine.PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            bool hasSeenTutorial = UnityEngine.PlayerPrefs.GetInt("SeenTutorial", 0) == 1;
            string version = com.playchilla.runner.Constants.Version;
            return new Settings(version, soundOn, musicOn, hasSeenTutorial);
        }
        
        public static Settings Create(object obj)
        {
            // Simplified: assume obj is anonymous type with properties version, soundOn, musicOn, hasSeenTutorial
            // For now, return default settings
            return Load();
        }
        
        public string GetVersion() => _version;
        public bool GetSoundOn() => _soundOn;
        public bool GetMusicOn() => _musicOn;
        public bool HasSeenTutorial() => _hasSeenTutorial;
        
        public void SetSoundOn(bool value) => _soundOn = value;
        public void SetMusicOn(bool value) => _musicOn = value;
        
        public object ToObject()
        {
            return new { version = _version, soundOn = _soundOn, musicOn = _musicOn, hasSeenTutorial = _hasSeenTutorial };
        }
        
        // Helper methods to toggle settings (for UI)
        public void ToggleSound()
        {
            _soundOn = !_soundOn;
            Save();
        }
        
        public void ToggleMusic()
        {
            _musicOn = !_musicOn;
            Save();
        }
        
        public void SetSeenTutorial()
        {
            _hasSeenTutorial = true;
            Save();
        }
    }
}
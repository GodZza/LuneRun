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
            // Stub implementation for saving settings
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
    }
}
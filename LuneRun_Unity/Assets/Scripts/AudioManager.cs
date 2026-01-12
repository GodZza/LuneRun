using UnityEngine;
using System.Collections.Generic;

namespace LuneRun
{
    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = false;
    }
    
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [SerializeField] private SoundClip[] soundClips;
        [SerializeField] private SoundClip[] musicClips;
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioSource musicSource;
        
        private Dictionary<string, SoundClip> soundDict = new Dictionary<string, SoundClip>();
        private Dictionary<string, SoundClip> musicDict = new Dictionary<string, SoundClip>();
        
        private float soundVolume = 1f;
        private float musicVolume = 1f;
        private bool soundEnabled = true;
        private bool musicEnabled = true;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize dictionaries
            foreach (SoundClip clip in soundClips)
            {
                if (!string.IsNullOrEmpty(clip.name))
                {
                    soundDict[clip.name] = clip;
                }
            }
            
            foreach (SoundClip clip in musicClips)
            {
                if (!string.IsNullOrEmpty(clip.name))
                {
                    musicDict[clip.name] = clip;
                }
            }
            
            // Ensure audio sources
            if (soundSource == null)
            {
                soundSource = gameObject.AddComponent<AudioSource>();
                soundSource.playOnAwake = false;
            }
            
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
            }
        }
        
        private void Start()
        {
            // Load settings
            // TODO: Load from player prefs or settings file
            soundEnabled = true;
            musicEnabled = true;
            UpdateVolumes();
        }
        
        // Play a sound effect
        public void PlaySound(string soundName, float volumeScale = 1f)
        {
            if (!soundEnabled || !soundDict.ContainsKey(soundName)) return;
            
            SoundClip clip = soundDict[soundName];
            soundSource.PlayOneShot(clip.clip, clip.volume * volumeScale * soundVolume);
        }
        
        // Play music
        public void PlayMusic(string musicName, float fadeTime = 0f)
        {
            if (!musicEnabled || !musicDict.ContainsKey(musicName)) return;
            
            SoundClip clip = musicDict[musicName];
            musicSource.clip = clip.clip;
            musicSource.volume = clip.volume * musicVolume;
            musicSource.loop = clip.loop;
            
            if (fadeTime > 0)
            {
                // TODO: Implement fade-in
            }
            
            musicSource.Play();
        }
        
        // Stop music
        public void StopMusic(float fadeTime = 0f)
        {
            if (fadeTime > 0)
            {
                // TODO: Implement fade-out
            }
            else
            {
                musicSource.Stop();
            }
        }
        
        // Set sound volume (0-1)
        public void SetSoundVolume(float volume, float fadeTime = 0f)
        {
            soundVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        // Set music volume (0-1)
        public void SetMusicVolume(float volume, float fadeTime = 0f)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        // Toggle sound on/off
        public void ToggleSound()
        {
            soundEnabled = !soundEnabled;
            UpdateVolumes();
        }
        
        // Toggle music on/off
        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            UpdateVolumes();
        }
        
        private void UpdateVolumes()
        {
            soundSource.volume = soundEnabled ? soundVolume : 0f;
            musicSource.volume = musicEnabled ? musicVolume : 0f;
        }
        
        // Update method for handling timed audio events (like in original game)
        public void UpdateAudio(int currentTime)
        {
            // In original game, audio updates based on timer for synchronization
            // We can implement similar logic if needed
        }
        
        // Get sound clip by name (for direct control)
        public SoundClip GetSoundClip(string name)
        {
            return soundDict.ContainsKey(name) ? soundDict[name] : null;
        }
        
        // Get music clip by name
        public SoundClip GetMusicClip(string name)
        {
            return musicDict.ContainsKey(name) ? musicDict[name] : null;
        }
        
        // Helper to play footstep sounds (random from set)
        public void PlayFootstep()
        {
            // Original game has 5 footstep sounds
            int footstepIndex = Random.Range(1, 6);
            PlaySound($"Footstep{footstepIndex}");
        }
        
        // Helper to play breath sound
        public void PlayBreath()
        {
            PlaySound("Breath");
        }
    }
}
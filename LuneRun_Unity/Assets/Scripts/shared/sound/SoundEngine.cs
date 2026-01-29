using System;

namespace shared.sound
{
    public class SoundEngine
    {
        public void update(int time)
        {
            // Stub implementation
        }
        
        public Sound getSound(string name)
        {
            return new Sound();
        }
        
        public void stop(int fadeOutTime) { }
        
        public void setVolume(double volume, int fadeTime) { }
        
        public void setVolumeMod(double mod, int fadeTime) { }
        
        public void Play(double speedAlpha) { }
    }
    
    public class Sound
    {
        public void setVolume(double volume, int fadeTime) { }
        
        public void setVolumeMod(double mod, int fadeTime) { }
        
        public void stop(int fadeOutTime) { }
        
        public void Play(double speedAlpha) { }

        public void loop(double speed) { } //?speed?


        public static string STrack1 { get; set; }
    }
}
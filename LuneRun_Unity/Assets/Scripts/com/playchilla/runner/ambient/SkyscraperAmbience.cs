using System.Collections.Generic;
using UnityEngine;

namespace com.playchilla.runner.ambient
{
    public class SkyscraperAmbience : MonoBehaviour
    {
        private com.playchilla.runner.Level _level;
        private bool _isHardware;
        private com.playchilla.runner.player.Player _player;
        private com.playchilla.runner.track.Track _track;
        private object _gameCont;
        private com.playchilla.runner.Materials _materials;
        private List<Skyscraper> _skyscrapers = new List<Skyscraper>();

        // TODO: Implement skyscraper ambience
        public void Initialize(com.playchilla.runner.Level level, bool isHardware)
        {
            _level = level;
            _isHardware = isHardware;
            _player = level.GetPlayer();
            _track = level.GetTrack();
            _gameCont = level.GetGameCont();
            _materials = level.GetMaterials();
        }

        public void Update()
        {
            // Update skyscrapers based on player position
        }
    }
}
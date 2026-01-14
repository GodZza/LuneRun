using System.Collections.Generic;

namespace com.playchilla.runner.api
{
    public class Highscore
    {
        private int _levelId;
        private List<Score> _scores;
        
        public Highscore(int levelId, List<Score> scores)
        {
            _levelId = levelId;
            _scores = scores;
        }
        
        public int GetLevelId() => _levelId;
        public List<Score> GetScores() => _scores;
    }
}
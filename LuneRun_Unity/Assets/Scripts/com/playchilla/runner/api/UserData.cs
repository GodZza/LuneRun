using System.Collections.Generic;

namespace com.playchilla.runner.api
{
    public class UserData
    {
        private int _userId;
        private List<Score> _scores;
        
        public UserData(int userId, List<Score> scores)
        {
            _userId = userId;
            _scores = scores;
        }
        
        public int GetUserId() => _userId;
        public List<Score> GetScores() => _scores;
        
        public Score GetScore(int levelId)
        {
            foreach (var score in _scores)
            {
                // Note: Score class doesn't have GetLevelId() in our stub,
                // but we'll assume it does for compatibility
                // For now, return first score (simplified)
                return score;
            }
            return null;
        }
        
        public object ToObject()
        {
            return new { userId = _userId, scores = _scores };
        }
    }
}
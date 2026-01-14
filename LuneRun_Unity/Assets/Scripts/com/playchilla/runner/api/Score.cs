namespace com.playchilla.runner.api
{
    public class Score
    {
        private int _userId;
        private string _userName;
        private double _scoreValue;
        
        public Score(int userId, string userName, double scoreValue)
        {
            _userId = userId;
            _userName = userName;
            _scoreValue = scoreValue;
        }
        
        public int GetUserId() => _userId;
        public string GetUserName() => _userName;
        public double GetScore() => _scoreValue;
    }
}
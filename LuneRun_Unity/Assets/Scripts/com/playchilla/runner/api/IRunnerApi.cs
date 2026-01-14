namespace com.playchilla.runner.api
{
    public interface IRunnerApi
    {
        int GetUserId();
        void GetUserData(IGetUserDataCallback callback);
        void Score(int levelId, double scoreValue, IScoreCallback callback);
        void GetHighscore(int levelId, IGetHighscoreCallback callback);
        void GetSurvivors(int levelId, IGetSurvivorsCallback callback);
    }
}
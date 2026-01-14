using com.playchilla.gameapi.api;
using com.playchilla.runner.api;

namespace com.playchilla.runner.api
{
    public class RunnerApi : IRunnerApi
    {
        private string _apiUrl;
        private ApiUser _apiUser;

        public RunnerApi(string apiUrl, ApiUser apiUser)
        {
            _apiUrl = apiUrl;
            _apiUser = apiUser;
            if (_apiUrl.Length > 0 && _apiUrl[_apiUrl.Length - 1] != '/')
            {
                _apiUrl += "/";
            }
        }

        public int GetUserId()
        {
            return _apiUser.getId();
        }

        public void GetUserData(IGetUserDataCallback callback)
        {
            // 空实现，单机游戏不需要联网API
        }

        public void Score(int levelId, double scoreValue, IScoreCallback callback)
        {
            // 空实现，单机游戏不需要联网API
        }

        public void GetHighscore(int levelId, IGetHighscoreCallback callback)
        {
            // 空实现，单机游戏不需要联网API
        }

        public void GetSurvivors(int levelId, IGetSurvivorsCallback callback)
        {
            // 空实现，单机游戏不需要联网API
        }
    }
}
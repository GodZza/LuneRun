using com.playchilla.gameapi.api;

namespace com.playchilla.runner.api
{
    public interface IGetHighscoreCallback
    {
        void GetHighscore(Highscore highscore);
        void GetHighscoreError(ErrorResponse error);
    }
}
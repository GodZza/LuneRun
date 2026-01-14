using com.playchilla.gameapi.api;

namespace com.playchilla.runner.api
{
    public interface IScoreCallback
    {
        void Score(Score score, bool isNewHighscore);
        void ScoreError(ErrorResponse error);
    }
}
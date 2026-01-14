using com.playchilla.gameapi.api;

namespace com.playchilla.runner.api
{
    public interface IGetSurvivorsCallback
    {
        void GetSurvivors(Highscore highscore);
        void GetSurvivorsError(ErrorResponse error);
    }
}
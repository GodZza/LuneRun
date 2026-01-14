using com.playchilla.gameapi.api;

namespace com.playchilla.runner.api
{
    public interface IGetUserDataCallback
    {
        void GetUserData(UserData userData);
        void GetUserDataError(ErrorResponse error);
    }
}
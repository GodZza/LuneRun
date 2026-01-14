namespace com.playchilla.gameapi.api
{
    public interface ILoginCallback
    {
        void Login(ApiUser user);
        void LoginError(ErrorResponse error);
    }
}
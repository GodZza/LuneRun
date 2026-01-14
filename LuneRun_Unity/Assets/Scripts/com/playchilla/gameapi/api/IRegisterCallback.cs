namespace com.playchilla.gameapi.api
{
    public interface IRegisterCallback
    {
        void Register(ApiUser user);
        void RegisterError(ErrorResponse error);
    }
}
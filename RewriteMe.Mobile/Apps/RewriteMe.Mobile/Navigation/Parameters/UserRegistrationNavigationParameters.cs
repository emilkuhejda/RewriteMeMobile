namespace RewriteMe.Mobile.Navigation.Parameters
{
    public class UserRegistrationNavigationParameters
    {
        public UserRegistrationNavigationParameters(bool isError)
        {
            IsError = isError;
        }

        public bool IsError { get; }
    }
}

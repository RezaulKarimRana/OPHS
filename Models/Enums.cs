namespace Models
{
    public enum TokenTypeEnum : int
    {
        Undefined = 0,
        AccountActivation = 1,
        ResetPassword = 2,
        ForgotPassword = 3
    }

    public enum ActivityTypeEnum : int
    {
        Undefined = 0,
        Default = 1,
        Error = 2
    }
}

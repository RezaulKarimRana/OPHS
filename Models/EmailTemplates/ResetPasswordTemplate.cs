namespace Models.EmailTemplates
{
    public class ResetPasswordTemplate : BaseTemplate
    {
        public override string Subject => "Reset password request";

        public string ResetPasswordUrl { get; set; }

        #region Constructors

        public ResetPasswordTemplate(string body, string applicationUrl) : base(body, applicationUrl)
        {
        }

        #endregion

        #region Public Methods

        public override string GetHTMLContent()
        {
            // perform replacements
            _body = _body.Replace("{{ResetPassword_Url}}", ResetPasswordUrl);

            return base.GetHTMLContent();
        }

        #endregion
    }
}

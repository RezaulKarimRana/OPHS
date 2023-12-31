namespace Models.EmailTemplates
{
    public class AccountActivationTemplate : BaseTemplate
    {
        public override string Subject => "Please activate your account";

        public string ActivationUrl { get; set; }

        #region Constructors

        public AccountActivationTemplate(string body, string applicationUrl) : base(body, applicationUrl)
        {
        }

        #endregion

        #region Public Methods

        public override string GetHTMLContent()
        {
            // perform replacements
            _body = _body.Replace("{{Activation_Url}}", ActivationUrl);

            return base.GetHTMLContent();
        }

        #endregion
    }
}

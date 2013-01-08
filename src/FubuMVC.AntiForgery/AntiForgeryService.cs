using FubuCore;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryService : IAntiForgeryService
    {
        private readonly IOutputWriter _outputWriter;
        private readonly ISecurityContext _securityContext;
        private readonly IFubuApplicationFiles _fubuApplicationFiles;
        private readonly ICookies _cookies;
        private readonly IAntiForgerySerializer _serializer;
        private readonly IAntiForgeryTokenProvider _tokenProvider;

        public AntiForgeryService(IOutputWriter outputWriter,
                                  IAntiForgeryTokenProvider tokenProvider,
                                  IAntiForgerySerializer serializer,
                                  ISecurityContext securityContext,
                                  IFubuApplicationFiles fubuApplicationFiles,
                                  ICookies cookies)
        {
            _outputWriter = outputWriter;
            _tokenProvider = tokenProvider;
            _serializer = serializer;
            _securityContext = securityContext;
            _fubuApplicationFiles = fubuApplicationFiles;
            _cookies = cookies;
        }

        public AntiForgeryData SetCookieToken(string path, string domain)
        {
            var applicationPath = _fubuApplicationFiles.GetApplicationPath();
            AntiForgeryData token = GetCookieToken();
            string name = _tokenProvider.GetTokenName(applicationPath);
            string cookieValue = _serializer.Serialize(token);

            var newCookie = new Cookie(name, cookieValue) {HttpOnly = true, Domain = domain};
            if (!string.IsNullOrEmpty(path))
            {
                newCookie.Path = path;
            }
            _outputWriter.AppendCookie(newCookie);

            return token;
        }

        public FormToken GetFormToken(AntiForgeryData token, string salt)
        {
            var formToken = new AntiForgeryData(token)
            {
                Salt = salt,
                Username = AntiForgeryData.GetUsername(_securityContext.CurrentUser)
            };
            string tokenString = _serializer.Serialize(formToken);

            return new FormToken
            {
                Name = _tokenProvider.GetTokenName(),
                TokenString = tokenString
            };
        }

        public AntiForgeryData GetCookieToken()
        {
            var applicationPath = _fubuApplicationFiles.GetApplicationPath();
            string name = _tokenProvider.GetTokenName(applicationPath);
            Cookie cookie = _cookies.Get(name);
            AntiForgeryData cookieToken = null;
            if (cookie != null)
            {
                try
                {
                    cookieToken = _serializer.Deserialize(cookie.Value);
                }
                catch (FubuException)
                {
                    // TODO -- log this.  Need a generic tracing mechanism
                }
            }

            return cookieToken ?? _tokenProvider.GenerateToken();
        }
    }
}
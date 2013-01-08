using System;
using System.Threading;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.AntiForgery
{
    public class AntiForgeryValidator : IAntiForgeryValidator
    {
        private readonly ICookies _cookies;
        private readonly IFubuApplicationFiles _fubuApplicationFiles;
        private readonly IRequestData _requestData;
        private readonly IAntiForgerySerializer _serializer;
        private readonly IAntiForgeryTokenProvider _tokenProvider;

        public AntiForgeryValidator(IAntiForgeryTokenProvider tokenProvider, IAntiForgerySerializer serializer,
                                    ICookies cookies, IFubuApplicationFiles fubuApplicationFiles,
                                    IRequestData requestData)
        {
            _tokenProvider = tokenProvider;
            _serializer = serializer;
            _cookies = cookies;
            _fubuApplicationFiles = fubuApplicationFiles;
            _requestData = requestData;
        }

        public bool Validate(string salt)
        {
            var applicationPath = _fubuApplicationFiles.GetApplicationPath();
            string fieldName = _tokenProvider.GetTokenName();
            string cookieName = _tokenProvider.GetTokenName(applicationPath);

            Cookie cookie = _cookies.Get(cookieName);
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return false;
            }
            AntiForgeryData cookieToken = _serializer.Deserialize(cookie.Value);

            var formValue = _requestData.ValuesFor(RequestDataSource.Request).Get(fieldName) as string;
            if (string.IsNullOrEmpty(formValue))
            {
                return false;
            }
            AntiForgeryData formToken = _serializer.Deserialize(formValue);

            if (!string.Equals(cookieToken.Value, formToken.Value, StringComparison.Ordinal))
            {
                return false;
            }

            string currentUsername = AntiForgeryData.GetUsername(Thread.CurrentPrincipal);
            if (!string.Equals(formToken.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(salt ?? string.Empty, formToken.Salt, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
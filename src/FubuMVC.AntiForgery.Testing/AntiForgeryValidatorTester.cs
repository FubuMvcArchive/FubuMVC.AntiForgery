using System;
using System.Security.Principal;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.AntiForgery.Testing
{
    [TestFixture]
    public class AntiForgeryValidatorTester : InteractionContext<AntiForgeryValidator>
    {
        private AntiForgeryData _cookieToken;
        private AntiForgeryData _formToken;
        private ICookies _cookies;
        private IValueSource _valueSource;

        protected override void beforeEach()
        {
            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName()).Return("FormName");
            MockFor<IAntiForgeryTokenProvider>().Stub(x => x.GetTokenName("String")).IgnoreArguments().Return(
                "CookieName");

            MockFor<IRequestData>().Stub(x => x.Value("ApplicationPath")).Return("Path");

            _valueSource = MockFor<IValueSource>();
            MockFor<IRequestData>().Stub(x => x.ValuesFor(RequestDataSource.Request)).Return(_valueSource);

            _cookies = MockFor<ICookies>();

            _formToken = new AntiForgeryData
            {
                CreationDate = new DateTime(2010, 12, 12),
                Salt = "Salty",
                Username = "User",
                Value = "12345"
            };

            _cookieToken = new AntiForgeryData
            {
                CreationDate = new DateTime(2010, 12, 12),
                Salt = "Salty",
                Username = "User",
                Value = "12345"
            };
            MockFor<IAntiForgerySerializer>().Stub(x => x.Deserialize("CookieValue")).Return(_cookieToken);
            MockFor<IAntiForgerySerializer>().Stub(x => x.Deserialize("FormValue")).Return(_formToken);


            MockFor<IPrincipal>().Stub(x => x.Identity).Return(MockFor<IIdentity>());
            MockFor<ISecurityContext>().Stub(x => x.CurrentUser).Return(MockFor<IPrincipal>());
        }

        private void SetupIdentity(bool authenticated, string name)
        {
            MockFor<IIdentity>().Stub(x => x.IsAuthenticated).Return(authenticated);
            MockFor<IIdentity>().Stub(x => x.Name).Return(name);
        }


        [Test]
        public void should_not_validate_with_incorrect_user()
        {
            SetupIdentity(true, "DifferentUser");
            _cookies.Stub(x => x.Get("CookieName")).Return(new Cookie("CookieName", "CookieValue"));
            _valueSource.Stub(x => x.Get("FormName")).Return("FormValue");
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_with_nonmatching_token_values()
        {
            SetupIdentity(true, "User");
            _cookies.Stub(x => x.Get("CookieName")).Return(new Cookie("CookieName", "CookieValue"));
            _valueSource.Stub(x => x.Get("FormName")).Return("FormValue");

            _formToken.Value = "I don't match!";

            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }


        [Test]
        public void should_not_validate_with_unauthenticated_user()
        {
            SetupIdentity(false, "User");
            _cookies.Stub(x => x.Get("CookieName")).Return(new Cookie("CookieName", "CookieValue"));
            _valueSource.Stub(x => x.Get("FormName")).Return("FormValue");
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_without_cookie_token()
        {
            SetupIdentity(true, "User");
            _cookies.Stub(x => x.Get("CookieName")).Return(null);
            _valueSource.Stub(x => x.Get("FormName")).Return("FormValue");
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_not_validate_without_form_token()
        {
            SetupIdentity(true, "User");
            _cookies.Stub(x => x.Get("CookieName")).Return(new Cookie("CookieName", "CookieValue"));
            _valueSource.Stub(x => x.Get("FormName")).Return(null);
            ClassUnderTest.Validate("Salty").ShouldBeFalse();
        }

        [Test]
        public void should_validate_with_correct_request_data()
        {
            MockFor<IIdentity>().Stub(x => x.IsAuthenticated).Return(true);
            MockFor<IIdentity>().Stub(x => x.Name).Return("User");
            _cookies.Stub(x => x.Get("CookieName")).Return(new Cookie("CookieName", "CookieValue"));
            _valueSource.Stub(x => x.Get("FormName")).Return("FormValue");
            ClassUnderTest.Validate("Salty").ShouldBeTrue();
        }
    }
}
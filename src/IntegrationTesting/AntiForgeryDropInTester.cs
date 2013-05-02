using System.Web;
using FubuTestingSupport;
using NUnit.Framework;

namespace IntegrationTesting
{
    [TestFixture]
    public class AntiForgeryDropInTester : SharedHarnessContext
    {
        [Test]
        public void renders_anti_forgery_hidden_element()
        {
            var viewWithForm = endpoints.Get<AntiForgeryEndpoint>(x => x.Get()).ReadAsText();
            viewWithForm.ShouldContain("input type=\"hidden\"");
        }

        [Test]
        public void should_add_url_encoded_cookie()
        {
            var cookies = endpoints.Get<AntiForgeryEndpoint>(x => x.Get()).Cookies;
            var cookieValue = cookies[0].Value;
            var decodedValue = HttpUtility.UrlDecode(cookieValue);

            cookieValue.ShouldNotEqual(decodedValue);
        }
    }

    public class AntiForgeryEndpoint
    {
        public AntiForgeryTestViewModel Get()
        {
            return new AntiForgeryTestViewModel();
        }

        public AntiForgeryPostTestViewModel Post(AntiForgeryTestInputModel input)
        {
            return new AntiForgeryPostTestViewModel();
        }
    }

    public class AntiForgeryPostTestViewModel
    {
    }

    public class AntiForgeryTestInputModel
    {
    }

    public class AntiForgeryTestViewModel
    {
        public string Value { get; set; }
    }
}
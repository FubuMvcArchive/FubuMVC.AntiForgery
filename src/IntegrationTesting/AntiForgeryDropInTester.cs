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
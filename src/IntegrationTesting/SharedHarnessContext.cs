using FubuMVC.Core;
using FubuMVC.Core.Endpoints;

namespace IntegrationTesting
{
    public class SharedHarnessContext
    {
        protected EndpointDriver endpoints
        {
            get
            {
                UrlContext.Stub(SelfHostHarness.Root);

                return SelfHostHarness.Endpoints;
            }
        }
    }
}
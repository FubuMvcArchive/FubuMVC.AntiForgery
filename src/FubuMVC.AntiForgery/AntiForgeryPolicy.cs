using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.AntiForgery
{
    [ConfigurationType(ConfigurationType.InjectNodes)]
    public class AntiForgeryPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var antiForgerySettings = graph.Settings.Get<AntiForgerySettings>();
            graph.Behaviors.Where(antiForgerySettings.AppliesTo)
                .Each(x => x.Prepend(new AntiForgeryNode(x.InputType().FullName)));
        }
    }
}
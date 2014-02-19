using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.AntiForgery
{
	[ConfigurationType(ConfigurationType.InjectNodes)]
	public class AntiForgeryPolicy : IConfigurationAction
	{
		public void Configure(BehaviorGraph graph)
		{
			var antiForgerySettings = graph.Settings.Get<AntiForgerySettings>();
			graph.Behaviors.Where(antiForgerySettings.AppliesTo)
				.Each(x => x.Prepend(ActionFilter.For<IAntiForgeryFilter>(f => f.Filter(x.InputType().FullName))));
		}
	}
}
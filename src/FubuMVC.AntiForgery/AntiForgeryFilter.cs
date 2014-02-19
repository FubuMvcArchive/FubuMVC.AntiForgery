using System.Net;
using FubuMVC.Core.Continuations;

namespace FubuMVC.AntiForgery
{
	public interface IAntiForgeryFilter
	{
		FubuContinuation Filter(string salt);
	}

	public class AntiForgeryFilter : IAntiForgeryFilter
	{
		private readonly IAntiForgeryValidator _validator;

		public AntiForgeryFilter(IAntiForgeryValidator validator)
		{
			_validator = validator;
		}

		public FubuContinuation Filter(string salt)
		{
			if (_validator.Validate(salt))
			{
				return FubuContinuation.NextBehavior();
			}

			return FubuContinuation.EndWithStatusCode(HttpStatusCode.InternalServerError);
		}
	}
}
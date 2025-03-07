using Whim.TestUtils;

namespace Whim.TreeLayout.Tests;

public class BaseTests : LayoutEngineBaseTests
{
	public override Func<ILayoutEngine> CreateLayoutEngine =>
		() =>
		{
			LayoutEngineWrapper wrapper = new();
			return new TreeLayoutEngine(wrapper.Context.Object, wrapper.Plugin.Object, wrapper.Identity);
		};
}

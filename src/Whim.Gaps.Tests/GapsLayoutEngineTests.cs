using FluentAssertions;
using NSubstitute;
using Whim.TestUtils;
using Xunit;

namespace Whim.Gaps.Tests;

public class GapsLayoutEngineTests
{
	private static readonly LayoutEngineIdentity _identity = new();

	public static IEnumerable<object[]> DoLayout_Data()
	{
		IWindow window1 = Substitute.For<IWindow>();
		yield return new object[]
		{
			new GapsConfig() { OuterGap = 10, InnerGap = 5 },
			new IWindow[] { window1 },
			100,
			new IWindowState[]
			{
				new WindowState()
				{
					Window = window1,
					Location = new Location<int>()
					{
						X = 10 + 5,
						Y = 10 + 5,
						Width = 1920 - (10 * 2) - (5 * 2),
						Height = 1080 - (10 * 2) - (5 * 2)
					},
					WindowSize = WindowSize.Normal
				}
			}
		};

		IWindow window2 = Substitute.For<IWindow>();
		IWindow window3 = Substitute.For<IWindow>();
		yield return new object[]
		{
			new GapsConfig() { OuterGap = 10, InnerGap = 5 },
			new IWindow[] { window2, window3 },
			100,
			new IWindowState[]
			{
				new WindowState()
				{
					Window = window2,
					Location = new Location<int>()
					{
						X = 10 + 5,
						Y = 10 + 5,
						Width = 960 - 10 - (5 * 2),
						Height = 1080 - (10 * 2) - (5 * 2)
					},
					WindowSize = WindowSize.Normal
				},
				new WindowState()
				{
					Window = window3,
					Location = new Location<int>()
					{
						X = 960 + 5,
						Y = 10 + 5,
						Width = 960 - 10 - (5 * 2),
						Height = 1080 - (10 * 2) - (5 * 2)
					},
					WindowSize = WindowSize.Normal
				}
			}
		};

		IWindow window4 = Substitute.For<IWindow>();
		yield return new object[]
		{
			new GapsConfig { OuterGap = 10, InnerGap = 5 },
			new IWindow[] { window4 },
			150,
			new IWindowState[]
			{
				new WindowState()
				{
					Window = window4,
					Location = new Location<int>()
					{
						X = 15 + 7,
						Y = 15 + 7,
						Width = 1920 - (15 * 2) - (7 * 2),
						Height = 1080 - (15 * 2) - (7 * 2)
					},
					WindowSize = WindowSize.Normal
				}
			}
		};
	}

	[Theory]
	[MemberData(nameof(DoLayout_Data))]
	public void DoLayout(GapsConfig gapsConfig, IWindow[] windows, int scale, IWindowState[] expectedWindowStates)
	{
		// Given
		ILayoutEngine innerLayoutEngine = new ColumnLayoutEngine(_identity);

		foreach (IWindow w in windows)
		{
			innerLayoutEngine = innerLayoutEngine.AddWindow(w);
		}

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		Location<int> location =
			new()
			{
				X = 0,
				Y = 0,
				Width = 1920,
				Height = 1080
			};

		IMonitor monitor = Substitute.For<IMonitor>();
		monitor.ScaleFactor.Returns(scale);

		// When
		IWindowState[] windowStates = gapsLayoutEngine.DoLayout(location, monitor).ToArray();

		// Then
		windowStates.Should().Equal(expectedWindowStates);
	}

	[Theory, AutoSubstituteData]
	public void Count(ILayoutEngine innerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.Count.Returns(5);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When

		// Then
		Assert.Equal(5, gapsLayoutEngine.Count);
		_ = innerLayoutEngine.Received(1).Count;
	}

	[Theory, AutoSubstituteData]
	public void ContainsWindow(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.ContainsWindow(Arg.Any<IWindow>()).Returns(true);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		bool containsWindow = gapsLayoutEngine.ContainsWindow(window);

		// Then
		Assert.True(containsWindow);
		innerLayoutEngine.Received(1).ContainsWindow(window);
	}

	[Theory, AutoSubstituteData]
	public void FocusWindowInDirection(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		Direction direction = Direction.Left;
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		gapsLayoutEngine.FocusWindowInDirection(direction, window);

		// Then
		innerLayoutEngine.Received(1).FocusWindowInDirection(direction, window);
	}

	#region Add
	[Theory, AutoSubstituteData]
	public void Add_Same(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.AddWindow(window).Returns(innerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.AddWindow(window);

		// Then
		Assert.Same(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).AddWindow(window);
	}

	[Theory, AutoSubstituteData]
	public void Add_NotSame(IWindow window, ILayoutEngine innerLayoutEngine, ILayoutEngine newInnerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.AddWindow(window).Returns(newInnerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.AddWindow(window);

		// Then
		Assert.NotSame(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).AddWindow(window);
	}
	#endregion

	#region Remove
	[Theory, AutoSubstituteData]
	public void Remove_Same(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.RemoveWindow(window).Returns(innerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.RemoveWindow(window);

		// Then
		Assert.Same(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).RemoveWindow(window);
	}

	[Theory, AutoSubstituteData]
	public void Remove_NotSame(IWindow window, ILayoutEngine innerLayoutEngine, ILayoutEngine newInnerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.RemoveWindow(window).Returns(newInnerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.RemoveWindow(window);

		// Then
		Assert.NotSame(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).RemoveWindow(window);
	}
	#endregion

	#region MoveWindowEdgesInDirection
	[Theory, AutoSubstituteData]
	public void MoveWindowEdgesInDirection_Same(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		Direction direction = Direction.Left;
		IPoint<double> deltas = new Point<double>();
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window).Returns(innerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window);

		// Then
		Assert.Same(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).MoveWindowEdgesInDirection(direction, deltas, window);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowEdgesInDirection_NotSame(
		IWindow window,
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine newInnerLayoutEngine
	)
	{
		// Given
		Direction direction = Direction.Left;
		IPoint<double> deltas = new Point<double>();
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window).Returns(newInnerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window);

		// Then
		Assert.NotSame(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).MoveWindowEdgesInDirection(direction, deltas, window);
	}
	#endregion

	#region MoveWindowToPoint
	[Theory, AutoSubstituteData]
	public void MoveWindowToPoint_Same(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		IPoint<double> point = new Point<double>();
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.MoveWindowToPoint(window, point).Returns(innerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.MoveWindowToPoint(window, point);

		// Then
		Assert.Same(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).MoveWindowToPoint(window, point);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowToPoint_NotSame(
		IWindow window,
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine newInnerLayoutEngine
	)
	{
		// Given
		IPoint<double> point = new Point<double>();
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.MoveWindowToPoint(window, point).Returns(newInnerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.MoveWindowToPoint(window, point);

		// Then
		Assert.NotSame(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).MoveWindowToPoint(window, point);
	}
	#endregion

	#region SwapWindowInDirection
	[Theory, AutoSubstituteData]
	public void SwapWindowInDirection_Same(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		Direction direction = Direction.Left;
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.SwapWindowInDirection(direction, window).Returns(innerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.SwapWindowInDirection(direction, window);

		// Then
		Assert.Same(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).SwapWindowInDirection(direction, window);
	}

	[Theory, AutoSubstituteData]
	public void SwapWindowInDirection_NotSame(
		IWindow window,
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine newInnerLayoutEngine
	)
	{
		// Given
		Direction direction = Direction.Left;
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.SwapWindowInDirection(direction, window).Returns(newInnerLayoutEngine);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		ILayoutEngine newLayoutEngine = gapsLayoutEngine.SwapWindowInDirection(direction, window);

		// Then
		Assert.NotSame(gapsLayoutEngine, newLayoutEngine);
		innerLayoutEngine.Received(1).SwapWindowInDirection(direction, window);
	}
	#endregion

	[Theory, AutoSubstituteData]
	public void GetFirstWindow(IWindow window, ILayoutEngine innerLayoutEngine)
	{
		// Given
		GapsConfig gapsConfig = new() { OuterGap = 10, InnerGap = 5 };
		innerLayoutEngine.GetFirstWindow().Returns(window);

		GapsLayoutEngine gapsLayoutEngine = new(gapsConfig, innerLayoutEngine);

		// When
		IWindow? firstWindow = gapsLayoutEngine.GetFirstWindow();

		// Then
		Assert.Same(window, firstWindow);
		innerLayoutEngine.Received(1).GetFirstWindow();
	}
}

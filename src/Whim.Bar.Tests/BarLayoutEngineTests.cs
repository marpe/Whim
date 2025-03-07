using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Whim.TestUtils;

namespace Whim.Bar.Tests;

public class BarLayoutEngineTests
{
	private static BarLayoutEngine CreateSut(ILayoutEngine innerLayoutEngine) =>
		new(
			new BarConfig(
				leftComponents: new List<BarComponent>(),
				centerComponents: new List<BarComponent>(),
				rightComponents: new List<BarComponent>()
			)
			{
				Height = 30
			},
			innerLayoutEngine
		);

	[Theory, AutoSubstituteData]
	public void Count(ILayoutEngine innerLayoutEngine)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);
		innerLayoutEngine.Count.Returns(5);

		// When
		int count = engine.Count;

		// Then
		Assert.Equal(5, count);
	}

	[Theory, AutoSubstituteData]
	public void AddWindow(ILayoutEngine innerLayoutEngine, ILayoutEngine addWindowResult, IWindow window)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.AddWindow(window).Returns(addWindowResult);
		addWindowResult.AddWindow(window).Returns(addWindowResult);

		// When
		ILayoutEngine newEngine = engine.AddWindow(window);
		ILayoutEngine newEngine2 = newEngine.AddWindow(window);

		// Then
		Assert.NotSame(engine, newEngine);
		Assert.Same(newEngine, newEngine2);
	}

	[Theory, AutoSubstituteData]
	public void ContainsWindow(ILayoutEngine innerLayoutEngine, IWindow window)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.ContainsWindow(window).Returns(true);

		// When
		bool contains = engine.ContainsWindow(window);

		// Then
		Assert.True(contains);
	}

	[Theory, AutoSubstituteData]
	public void FocusWindowInDirection(ILayoutEngine innerLayoutEngine, IWindow window, [Frozen] Direction direction)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		// When
		engine.FocusWindowInDirection(direction, window);

		// Then
		innerLayoutEngine.Received(1).FocusWindowInDirection(direction, window);
	}

	[Theory, AutoSubstituteData]
	public void GetFirstWindow(ILayoutEngine innerLayoutEngine, IWindow window)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.GetFirstWindow().Returns(window);

		// When
		IWindow? firstWindow = engine.GetFirstWindow();

		// Then
		Assert.Same(window, firstWindow);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowEdgesInDirection_NotSame(
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine moveWindowEdgesResult,
		IWindow window,
		Direction direction,
		Point<double> deltas
	)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window).Returns(moveWindowEdgesResult);

		// When
		ILayoutEngine newEngine = engine.MoveWindowEdgesInDirection(direction, deltas, window);

		// Then
		Assert.NotSame(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowEdgesInDirection_Same(
		ILayoutEngine innerLayoutEngine,
		IWindow window,
		Direction direction,
		Point<double> deltas
	)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.MoveWindowEdgesInDirection(direction, deltas, window).Returns(innerLayoutEngine);

		// When
		ILayoutEngine newEngine = engine.MoveWindowEdgesInDirection(direction, deltas, window);

		// Then
		Assert.Same(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowToPoint_NotSame(
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine moveWindowToPointResult,
		IWindow window,
		Point<double> point
	)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.MoveWindowToPoint(window, point).Returns(moveWindowToPointResult);

		// When
		ILayoutEngine newEngine = engine.MoveWindowToPoint(window, point);

		// Then
		Assert.NotSame(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void MoveWindowToPoint_Same(ILayoutEngine innerLayoutEngine, IWindow window, Point<double> point)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.MoveWindowToPoint(window, point).Returns(innerLayoutEngine);

		// When
		ILayoutEngine newEngine = engine.MoveWindowToPoint(window, point);

		// Then
		Assert.Same(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void RemoveWindow_NotSame(ILayoutEngine innerLayoutEngine, ILayoutEngine removeWindowResult, IWindow window)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.RemoveWindow(window).Returns(removeWindowResult);

		// When
		ILayoutEngine newEngine = engine.RemoveWindow(window);

		// Then
		Assert.NotSame(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void RemoveWindow_Same(ILayoutEngine innerLayoutEngine, IWindow window)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.RemoveWindow(window).Returns(innerLayoutEngine);

		// When
		ILayoutEngine newEngine = engine.RemoveWindow(window);

		// Then
		Assert.Same(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void SwapWindowInDirection_NotSame(
		ILayoutEngine innerLayoutEngine,
		ILayoutEngine swapWindowInDirectionResult,
		IWindow window,
		Direction direction
	)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.SwapWindowInDirection(direction, window).Returns(swapWindowInDirectionResult);

		// When
		ILayoutEngine newEngine = engine.SwapWindowInDirection(direction, window);

		// Then
		Assert.NotSame(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void SwapWindowInDirection_Same(ILayoutEngine innerLayoutEngine, IWindow window, Direction direction)
	{
		// Given
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		innerLayoutEngine.SwapWindowInDirection(direction, window).Returns(innerLayoutEngine);

		// When
		ILayoutEngine newEngine = engine.SwapWindowInDirection(direction, window);

		// Then
		Assert.Same(engine, newEngine);
	}

	[Theory, AutoSubstituteData]
	public void DoLayout(ILayoutEngine innerLayoutEngine, IWindow window1, IWindow window2, IMonitor monitor)
	{
		// Given
		monitor.ScaleFactor.Returns(100);
		BarLayoutEngine engine = CreateSut(innerLayoutEngine);

		IWindowState[] expectedWindowStates = new[]
		{
			new WindowState()
			{
				Window = window1,
				Location = new Location<int>()
				{
					Y = 30,
					Width = 50,
					Height = 70
				},
				WindowSize = WindowSize.Normal
			},
			new WindowState()
			{
				Window = window2,
				Location = new Location<int>()
				{
					X = 50,
					Y = 30,
					Width = 50,
					Height = 70
				},
				WindowSize = WindowSize.Normal
			}
		};

		Location<int> expectedGivenLocation =
			new()
			{
				Y = 30,
				Width = 100,
				Height = 70
			};

		innerLayoutEngine.DoLayout(expectedGivenLocation, monitor).Returns(expectedWindowStates);

		// When
		IWindowState[] layout = engine.DoLayout(new Location<int>() { Width = 100, Height = 100 }, monitor).ToArray();

		// Then
		Assert.Equal(2, layout.Length);
		innerLayoutEngine.Received(1).DoLayout(expectedGivenLocation, monitor);
		layout.Should().Equal(expectedWindowStates);
	}
}

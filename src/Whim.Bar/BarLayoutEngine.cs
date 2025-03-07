using System.Collections.Generic;

namespace Whim.Bar;

/// <summary>
/// A proxy layout engine to reserve space for the bar in each monitor.
/// </summary>
public record BarLayoutEngine : BaseProxyLayoutEngine
{
	private readonly BarConfig _barConfig;

	/// <summary>
	/// Creates a new instance of the proxy layout engine <see cref="BarLayoutEngine"/>.
	/// </summary>
	/// <param name="barConfig"></param>
	/// <param name="innerLayoutEngine"></param>
	public BarLayoutEngine(BarConfig barConfig, ILayoutEngine innerLayoutEngine)
		: base(innerLayoutEngine)
	{
		_barConfig = barConfig;
	}

	private BarLayoutEngine UpdateInner(ILayoutEngine newInnerLayoutEngine) =>
		InnerLayoutEngine == newInnerLayoutEngine ? this : new BarLayoutEngine(_barConfig, newInnerLayoutEngine);

	/// <inheritdoc />
	public override int Count => InnerLayoutEngine.Count;

	/// <inheritdoc />
	public override ILayoutEngine AddWindow(IWindow window) => UpdateInner(InnerLayoutEngine.AddWindow(window));

	/// <inheritdoc />
	public override bool ContainsWindow(IWindow window) => InnerLayoutEngine.ContainsWindow(window);

	/// <inheritdoc />
	public override IEnumerable<IWindowState> DoLayout(ILocation<int> location, IMonitor monitor)
	{
		double scale = monitor.ScaleFactor / 100.0;
		int height = (int)(_barConfig.Height * scale);

		Location<int> proxiedLocation =
			new()
			{
				X = location.X,
				Y = location.Y + height,
				Width = location.Width,
				Height = location.Height - height
			};
		return InnerLayoutEngine.DoLayout(proxiedLocation, monitor);
	}

	/// <inheritdoc />
	public override void FocusWindowInDirection(Direction direction, IWindow window) =>
		InnerLayoutEngine.FocusWindowInDirection(direction, window);

	/// <inheritdoc />
	public override IWindow? GetFirstWindow() => InnerLayoutEngine.GetFirstWindow();

	/// <inheritdoc />
	public override ILayoutEngine MoveWindowEdgesInDirection(Direction edge, IPoint<double> deltas, IWindow window) =>
		UpdateInner(InnerLayoutEngine.MoveWindowEdgesInDirection(edge, deltas, window));

	/// <inheritdoc />
	public override ILayoutEngine MoveWindowToPoint(IWindow window, IPoint<double> point) =>
		UpdateInner(InnerLayoutEngine.MoveWindowToPoint(window, point));

	/// <inheritdoc />
	public override ILayoutEngine RemoveWindow(IWindow window) => UpdateInner(InnerLayoutEngine.RemoveWindow(window));

	/// <inheritdoc />
	public override ILayoutEngine SwapWindowInDirection(Direction direction, IWindow window) =>
		UpdateInner(InnerLayoutEngine.SwapWindowInDirection(direction, window));
}

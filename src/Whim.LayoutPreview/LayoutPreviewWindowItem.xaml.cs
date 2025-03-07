using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Whim.LayoutPreview;

/// <summary>
/// Interaction logic for LayoutPreviewWindowItem.xaml
/// </summary>
public sealed partial class LayoutPreviewWindowItem : UserControl
{
	/// <summary>
	/// The <see cref="IWindowState"/> that this <see cref="LayoutPreviewWindowItem"/> represents.
	/// </summary>
	public IWindowState WindowState { get; }

	/// <summary>
	/// The icon for the window.
	/// </summary>
	public ImageSource? ImageSource { get; }

	internal LayoutPreviewWindowItem(IContext context, IWindowState windowState, bool isHovered)
	{
		WindowState = windowState;
		ImageSource = windowState.Window.GetIcon();

		InitializeComponent();

		// Set the color for this item.
		Color tintColor;
		if (isHovered)
		{
			tintColor = GetHoverTintColor();
			Title.Foreground = new SolidColorBrush(tintColor.GetTextColor());
		}
		else
		{
			tintColor = context.NativeManager.ShouldSystemUseDarkMode()
				? Color.FromArgb(Colors.Black.A, 33, 33, 33)
				: Color.FromArgb(Colors.White.A, 253, 253, 253);
		}

		Panel.Background = new AcrylicBrush()
		{
			Opacity = 0.8,
			TintLuminosityOpacity = 0.8,
			TintColor = tintColor,
		};
	}

	/// <summary>
	/// Get the current accent color.
	/// </summary>
	/// <returns></returns>
	private static Color GetHoverTintColor()
	{
		Color accentColor = new UISettings().GetColorValue(UIColorType.Accent);
		return Color.FromArgb(255, accentColor.R, accentColor.G, accentColor.B);
	}
}

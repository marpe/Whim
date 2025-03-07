using System.Numerics;

namespace Whim;

/// <summary>
/// Extension methods for <see cref="ILocation{T}"/>.
/// </summary>
public static class LocationExtensions
{
	/// <summary>
	/// Adds the given <paramref name="other"/> to this <see cref="ILocation{T}"/>.
	/// </summary>
	/// <param name="location">The location to add to.</param>
	/// <param name="other">The location to add.</param>
	/// <returns>A new <see cref="ILocation{T}"/> with the given <paramref name="other"/> added to this one.</returns>
	public static ILocation<T> Add<T>(this ILocation<T> location, ILocation<T> other)
		where T : INumber<T> =>
		new Location<T>()
		{
			X = location.X + other.X,
			Y = location.Y + other.Y,
			Width = location.Width + other.Width,
			Height = location.Height + other.Height
		};
}

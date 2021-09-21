#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Contains information about characteristics the formula depends on.
	/// </summary>
	public readonly struct MathDependencyInformation
	{
		public static readonly MathDependencyInformation Empty = new MathDependencyInformation();

		/// <summary>
		/// Creates a new instance of <see ref="MathDependencyInformation"/>.
		/// </summary>
		internal MathDependencyInformation( PathInformationDto path, int start, int length, string text, ushort? key )
		{
			Path = path;
			Start = start;
			Length = length;
			Text = text;
			AttributeKey = key;
		}

		/// <summary>
		/// Gets the characteristic path.
		/// </summary>
		public PathInformationDto Path { get; }

		/// <summary>
		/// Gets the start index of the path definition in the formula.
		/// </summary>
		public int Start { get; }

		/// <summary>
		/// Gets the length of the path definition in the formula.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// Gets the textual path definition in the formula.
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Gets the optional attribute key.
		/// </summary>
		public ushort? AttributeKey { get; }
	}
}
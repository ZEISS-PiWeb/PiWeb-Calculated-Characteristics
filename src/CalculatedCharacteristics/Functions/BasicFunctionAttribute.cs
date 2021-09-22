#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;

	#endregion

	/// <summary>
	/// Attribute used to provide name and syntax information for visualization.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public class BasicFunctionAttribute : Attribute
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="BasicFunctionAttribute"/>.
		/// </summary>
		public BasicFunctionAttribute( string name, string syntax )
		{
			Name = name;
			Syntax = syntax;
		}

		#endregion

		#region properties

		/// <summary>
		/// The function name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The function syntax hint.
		/// </summary>
		public string Syntax { get; }

		/// <summary>
		/// Provides a localized description of the function.
		/// </summary>
		public string Description => LocalizationHelper.Get<BasicFunctionAttribute>( Name );

		#endregion
	}
}
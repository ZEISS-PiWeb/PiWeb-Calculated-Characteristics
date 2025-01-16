#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <summary>
	/// Base class to define a factory to simply create a <see cref="MathInterpreter"/>.
	/// </summary>
	/// <remarks>Once created <see cref="MathInterpreter"/> instance is reused to create <see cref="IMathCalculator"/> instances.</remarks>
	public abstract class SimpleMathInterpreterFactory
	{
		#region members

		private readonly ChildPathsHandler _ChildPathsHandler;
		private readonly PathResolverFactory? _PathResolverFactory;
		private MathInterpreter? _MathInterpreter;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="SimpleMathInterpreterFactory"/>.
		/// </summary>
		/// <param name="childPathsHandler">The delegate to get the paths of the children for a path.</param>
		/// <param name="pathResolverFactory">An optional delegate to provide custom <see cref="IStringToPathResolver"/>.</param>
		protected SimpleMathInterpreterFactory(
			ChildPathsHandler childPathsHandler,
			PathResolverFactory? pathResolverFactory )
		{
			_ChildPathsHandler = childPathsHandler;
			_PathResolverFactory = pathResolverFactory;
		}

		#endregion

		#region methods

		/// <summary>
		/// Provides the <see cref="MathInterpreter"/> that can resolve formulas containing calculated characteristics.
		/// </summary>
		/// <returns>The math interpreter created by this factory.</returns>
		public MathInterpreter GetInterpreter()
		{
			return _MathInterpreter ??= new MathInterpreter(
				GetCharacteristicCalculator,
				_ChildPathsHandler,
				_PathResolverFactory );
		}

		/// <summary>
		/// Provides an <see cref="IMathCalculator"/> for a calculated characteristic.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <returns>The math calculator to calculate the value, or <c>null</c> if the characteristic has no formula.</returns>
		public IMathCalculator? GetCharacteristicCalculator( PathInformation path )
		{
			var formula = GetFormula( path );

			if( string.IsNullOrEmpty( formula ) )
				return null;

			var mathInterpreter = GetInterpreter();
			return mathInterpreter.Parse( formula, path );
		}

		/// <summary>
		/// Provides the formula for an entity.
		/// </summary>
		/// <param name="path">The entity path</param>
		/// <returns>The formula string or <c>null</c>.</returns>
		protected abstract string? GetFormula( PathInformation path );

		#endregion
	}
}
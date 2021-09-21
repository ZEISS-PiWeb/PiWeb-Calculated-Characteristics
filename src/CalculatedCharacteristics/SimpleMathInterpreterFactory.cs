#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Base class to define a factory to simply create a <see cref="MathInterpreter"/>.
	/// </summary>
	/// <remarks>Once created <see cref="MathInterpreter"/> instance is reused to create <see cref="IMathCalculator"/> instances.</remarks>
	public abstract class SimpleMathInterpreterFactory
	{
		#region members

		private readonly ChildPathsHandler _ChildPathsHandler;
		private readonly PathResolverFactory _PathResolverFactory;
		private MathInterpreter _MathInterpreter;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="SimpleMathInterpreterFactory"/>.
		/// </summary>
		/// <param name="childPathsHandler">The delegate to get the paths of the children for a path.</param>
		/// <param name="pathResolverFactory">An optional delegate to provide custom <see cref="IStringToPathResolver"/>.</param>
		protected SimpleMathInterpreterFactory(
			[NotNull] ChildPathsHandler childPathsHandler,
			PathResolverFactory pathResolverFactory )
		{
			_ChildPathsHandler = childPathsHandler;
			_PathResolverFactory = pathResolverFactory;
		}

		#endregion

		#region methods

		public MathInterpreter GetInterpreter()
		{
			return _MathInterpreter ??= new MathInterpreter(
				GetCharacteristicCalculator,
				_ChildPathsHandler,
				_PathResolverFactory );
		}

		public IMathCalculator GetCharacteristicCalculator( [NotNull] PathInformationDto path )
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
		[CanBeNull]
		protected abstract string GetFormula( [NotNull] PathInformationDto path );

		#endregion
	}
}
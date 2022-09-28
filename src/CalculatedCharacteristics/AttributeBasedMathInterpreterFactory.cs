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

	using System;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Contracts;
	using Zeiss.PiWeb.Api.Definitions;

	#endregion

	/// <summary>
	/// Creates a math interpreter based on entity attribute <see cref="WellKnownKeys.Characteristic.LogicalOperationString"/>.
	/// </summary>
	public class AttributeBasedMathInterpreterFactory : SimpleMathInterpreterFactory
	{
		#region members

		private readonly Func<PathInformation, ushort, object> _AttributeResolveHandler;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="AttributeBasedMathInterpreterFactory"/>.
		/// </summary>
		/// <param name="attributeResolveHandler">Defines how to get an entity attribute by path.</param>
		/// <param name="childPathsHandler">The delegate to get the paths of the children for a path.</param>
		/// <param name="pathResolverFactory">An optional delegate to provide custom <see cref="IStringToPathResolver"/>.</param>
		public AttributeBasedMathInterpreterFactory(
			[NotNull] Func<PathInformation, ushort, object> attributeResolveHandler,
			[NotNull] ChildPathsHandler childPathsHandler,
			PathResolverFactory pathResolverFactory = null )
			: base( childPathsHandler, pathResolverFactory )
		{
			_AttributeResolveHandler = attributeResolveHandler;
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		protected override string GetFormula( PathInformation path )
		{
			return _AttributeResolveHandler.Invoke( path, WellKnownKeys.Characteristic.LogicalOperationString ) as string;
		}

		#endregion
	}
}
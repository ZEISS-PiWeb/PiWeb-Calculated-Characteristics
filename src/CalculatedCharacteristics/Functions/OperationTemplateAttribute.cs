#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;

	#endregion

	/// <summary>
	/// Attribute used to provide templates for editor.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = true)]
	public class OperationTemplateAttribute : Attribute
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="OperationTemplateAttribute"/>.
		/// </summary>
		/// <param name="template">Defines the template syntax.</param>
		/// <param name="type">Type of operation.</param>
		public OperationTemplateAttribute( string template, OperationTemplateTypes type )
		{
			Template = template;
			Type = type;
		}

		#endregion

		#region properties

		/// <summary>
		/// Provides the template syntax.
		/// </summary>
		public string Template { get; }

		/// <summary>
		/// Provides the kind of associated operation.
		/// </summary>
		public OperationTemplateTypes Type { get; }

		#endregion
	}
}
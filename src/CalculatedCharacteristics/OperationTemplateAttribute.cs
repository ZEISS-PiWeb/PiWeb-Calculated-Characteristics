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

	using System;

	#endregion

	public class OperationTemplateAttribute : Attribute
	{
		#region constructors

		public OperationTemplateAttribute( string template, OperationTemplateTypes type )
		{
			Template = template;
			Type = type;
		}

		#endregion

		#region properties

		public string Template { get; }

		public OperationTemplateTypes Type { get; }

		#endregion
	}
}
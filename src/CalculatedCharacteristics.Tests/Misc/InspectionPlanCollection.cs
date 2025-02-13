﻿#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc
{
	#region usings

	using System;
	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Defines a collection of <see cref="InspectionPlanDtoBase"/> instances including relations between instances.
	/// </summary>
	public class InspectionPlanCollection
	{
		#region members

		private readonly Dictionary<PathInformation, List<InspectionPlanDtoBase>> _ParentChildTable = new Dictionary<PathInformation, List<InspectionPlanDtoBase>>();
		private readonly Dictionary<PathInformation, InspectionPlanDtoBase> _ItemsByPath = new Dictionary<PathInformation, InspectionPlanDtoBase>();
		private bool _Changed;

		#endregion

		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public InspectionPlanCollection()
		{ }

		/// <summary>
		/// Constructor.
		/// </summary>
		public InspectionPlanCollection( IEnumerable<InspectionPlanDtoBase> plan )
		{
			AddRange( plan );
		}

		#endregion

		#region properties

		private Dictionary<PathInformation, List<InspectionPlanDtoBase>> ParentChildTable
		{
			get
			{
				UpdateTables();
				return _ParentChildTable;
			}
		}

		/// <summary>
		/// Provides the <see cref="InspectionPlanDtoBase"/> instance for a given path from the collection, if exist.
		/// </summary>
		public InspectionPlanDtoBase? this[ PathInformation path ] => _ItemsByPath.GetValueOrDefault( path );

		#endregion

		#region methods

		private void UpdateTables()
		{
			if( _Changed )
			{
				_Changed = false;
				BuildParentChildTable();
			}
		}

		private void BuildParentChildTable()
		{
			_ParentChildTable.Clear();

			var items = new List<InspectionPlanDtoBase>( _ItemsByPath.Values );
			items.Sort( Comparison );

			foreach( var item in items )
			{
				var parent = item.Path.ParentPath;

				if( !_ParentChildTable.TryGetValue( parent, out var children ) )
				{
					children = [];
					_ParentChildTable[ parent ] = children;
				}

				children.Add( item );
			}
		}

		private static int Comparison( InspectionPlanDtoBase e1, InspectionPlanDtoBase e2 )
		{
			return e1.Path.Count.CompareTo( e2.Path.Count );
		}

		/// <summary>
		/// Adds an instance of <see cref="InspectionPlanDtoBase"/> to the collection.
		/// </summary>
		public void Add( InspectionPlanDtoBase entity )
		{
			_ItemsByPath[ entity.Path ] = entity;
			_Changed = true;
		}

		/// <summary>
		/// Adds a range of <see cref="InspectionPlanDtoBase"/> instances to the collection.
		/// </summary>
		public void AddRange( IEnumerable<InspectionPlanDtoBase> entities )
		{
			foreach( var entity in entities )
				Add( entity );
		}

		/// <summary>
		/// Provides the direct child DTOs for a path.
		/// </summary>
		public IEnumerable<InspectionPlanDtoBase> GetDirectChildren( PathInformation parentPath )
		{
			UpdateTables();

			if( ParentChildTable.ContainsKey( parentPath ) )
				return ParentChildTable[ parentPath ];

			return Array.Empty<InspectionPlanDtoBase>();
		}

		#endregion
	}

	internal static class InspectionPlanCollectionExtension
	{
		#region methods

		public static object? GetCharacteristicAttribute(
			this InspectionPlanCollection characteristics,
			ConfigurationDto configuration,
			CatalogCollectionDto catalogCollection,
			PathInformation path,
			ushort key )
		{
			if( characteristics[ path ] is InspectionPlanCharacteristicDto characteristic )
				return characteristic.GetRawAttributeValue( key, configuration, catalogCollection );

			return null;
		}

		#endregion
	}
}
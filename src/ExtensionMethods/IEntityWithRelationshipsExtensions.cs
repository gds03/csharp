using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;


namespace ExtensionMethods {

	
	public static class IEntityWithRelationshipsExtensions {


		/// <summary>
		/// Obtém o ObjectContext a partir de uma Entity. Apenas possível para Entities que tenham um Context associado (not dettached) e pelo menos uma relação com outra Entity.
		/// </summary>
		public static ObjectContext GetContext(this IEntityWithRelationships entity) {
			if (entity == null)
				throw new ArgumentNullException("entity");

			var relationshipManager = entity.RelationshipManager;

			var relatedEnd = relationshipManager.GetAllRelatedEnds()
												.FirstOrDefault();

			if (relatedEnd == null)
				throw new Exception("No relationships found");

			var query = relatedEnd.CreateSourceQuery() as ObjectQuery;

			if (query == null)
				throw new Exception("The Entity is Detached");

			return query.Context;
		}


	}


}

using NHibernate.Criterion;
using NHibernateMVCApp.Repository.Interfaces;
using NHibernateMVCApp.Repository.Mappings;
using NHibernateMVCApp.Repository.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateMVCApp.Repository.Extensions
{
    public static class IQueryOfProductExtensions
    {
        public static IList<Product> GetPriceHigherThan(this Query<Product> iqProd, decimal price)
        {
            var icret = iqProd.Session.CreateCriteria<Product>();
            // icret.SetProjection(Projections.Distinct(Projections.Property<Product>(x => x.Class)));
            icret.Add(Restrictions.Ge(Projections.Property<Product>(x => x.Standardcost), price));

            return icret.List<Product>();
        }
    }
}
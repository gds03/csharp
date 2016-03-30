using System;
using System.Text;
using System.Collections.Generic;


namespace NHibernateMVCApp.Repository.Mappings
{

    public class Product
    {
        public Product() { }
        public virtual int Productid { get; set; }
        //public virtual Unitmeasure Unitmeasure { get; set; }
        //public virtual Productsubcategory Productsubcategory { get; set; }
        //public virtual Productmodel Productmodel { get; set; }
        public virtual string Name { get; set; }
        public virtual string Productnumber { get; set; }
        public virtual bool Makeflag { get; set; }
        public virtual bool Finishedgoodsflag { get; set; }
        public virtual string Color { get; set; }
        public virtual short Safetystocklevel { get; set; }
        public virtual short Reorderpoint { get; set; }
        public virtual decimal Standardcost { get; set; }
        public virtual decimal Listprice { get; set; }
        public virtual string Size { get; set; }
        public virtual decimal? Weight { get; set; }
        public virtual int Daystomanufacture { get; set; }
        public virtual string Productline { get; set; }
        public virtual string Class { get; set; }
        public virtual string Style { get; set; }
        public virtual DateTime Sellstartdate { get; set; }
        public virtual DateTime? Sellenddate { get; set; }
        public virtual DateTime? Discontinueddate { get; set; }
        public virtual System.Guid Rowguid { get; set; }
        public virtual DateTime Modifieddate { get; set; }
    }
}

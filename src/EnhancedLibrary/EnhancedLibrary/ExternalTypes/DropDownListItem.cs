using System;

namespace EnhancedLibrary.ExternalTypes
{
    /// <summary>
    ///     Represents an identifier and his description.
    ///     Typically instances of this class are build to be used in combobox's;
    /// </summary>
    public class DropDownListItem<TKey>
    {
        public TKey ID { get; set; }
        public String Descricao { get; set; }

        public DropDownListItem()
        {

        }

        public DropDownListItem(TKey key, String descricao)
        {
            this.ID = key;
            this.Descricao = descricao;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Descricao))
                return base.ToString();

            return Descricao;
        }
    }
}

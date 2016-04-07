using System;

namespace Repository.ObjectMapper.Types.Mappings
{
    /// <summary>
    ///     Typically used to map the identity column of a type
    /// </summary>
    sealed class KeyMapping
    {
        internal String From;
        internal String To;

        public KeyMapping(String to, String from)
        {
            To = to;
            From = from;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode();
        }
    }
}

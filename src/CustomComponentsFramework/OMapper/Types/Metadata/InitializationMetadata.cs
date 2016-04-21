using System.Diagnostics;

namespace OMapper.Types.Metadata
{
    public class InitializationMetadata
    {
        internal OMapper m_oMapper;

        internal InitializationMetadata(OMapper oMapper)
        {
            Debug.Assert(oMapper != null);
            m_oMapper = oMapper;
        }





        public TypesMetadata<T> For<T>()
        {
            return new TypesMetadata<T>(m_oMapper);
        }
    }
}

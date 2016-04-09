namespace Repository.OMapper.Internal.Metadata
{
    public class InitializationMetadata
    {
        public TypesMetadata<T> For<T>()
        {
            return new TypesMetadata<T>();
        }
    }
}

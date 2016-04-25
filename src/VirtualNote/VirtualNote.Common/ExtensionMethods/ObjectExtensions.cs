
namespace VirtualNote.Common.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public static TEntity CastTo<TEntity>(this object obj)
        {
            return (TEntity)obj;
        }

        public static string ToLowerString(this object obj){
            return obj.ToString().ToLower();
        }
    }
}
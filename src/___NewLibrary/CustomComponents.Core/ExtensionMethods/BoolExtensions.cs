
namespace CustomComponents.Core.ExtensionMethods
{
    public static class BoolExtensions
    {
        public static string ToBitString(this bool value)
        {
            return value ? "1" : "0";
        }
    }
}
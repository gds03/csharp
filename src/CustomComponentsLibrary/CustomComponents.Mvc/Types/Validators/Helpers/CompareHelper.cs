using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Mvc.Types.Validators.Helpers
{
   public static class CompareHelper
    {
        public static bool Compare(object obj1, object obj2, TypeCompareOptions option)
        {
            bool? returnVal = null;     // without any state

            if ( option == TypeCompareOptions.Equal && obj1 == null && obj2 != null )
                returnVal = false;

            if ( returnVal == null && option == TypeCompareOptions.NotEqual && obj1 == null && obj2 == null )
                returnVal = false;

            if ( returnVal == null && option == TypeCompareOptions.Equal && obj1 == null && obj2 == null )
                returnVal = true;

            if ( returnVal == null && option == TypeCompareOptions.NotEqual && ((obj1 == null && obj2 != null) || (obj1 != null && obj2 == null)) )
                returnVal = true;

            if ( returnVal != null )
                return returnVal.Value;

            else
            {
                if ( obj1 == null && obj2 == null )
                    return true;
            }

            // Two types must be comparable to call compareTo method.
            IComparable original = (IComparable) obj1;
            IComparable reference = (IComparable) obj2;

            // property types are equal, we can compare.
            switch ( option )
            {
                case TypeCompareOptions.Equal:
                    if ( !(original.CompareTo(reference) == 0) )
                        return false;
                    break;

                case TypeCompareOptions.Greater:
                    if ( !(original.CompareTo(reference) > 0) )
                        return false;
                    break;

                case TypeCompareOptions.GreaterOrEqual:
                    if ( !(original.CompareTo(reference) >= 0) )
                        return false;
                    break;

                case TypeCompareOptions.Less:
                    if ( !(original.CompareTo(reference) < 0) )
                        return false;
                    break;

                case TypeCompareOptions.LessOrEqual:
                    if ( !(original.CompareTo(reference) <= 0) )
                        return false;
                    break;

                case TypeCompareOptions.NotEqual:
                    if ( !(original.CompareTo(reference) != 0) )
                        return false;
                    break;

                default:
                    throw new InvalidOperationException("option not handled by application");
            }

            return true;
        }
    }
}

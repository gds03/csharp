using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp60
{
    // using static to avoid Static Class name Call.
    using static System.Math;


    public static class UsingStatic
    {

        public static double GetSquareOf(double num)
        {
            return Sqrt(num);
        }

        public static double GetLogOf(double num)
        {
            return Log(num);
        }
    }
}

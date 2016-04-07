using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Algorithms.Recursion.Games.Maze
{
    public struct Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }



        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public Point(Point anotherPoint) : this(anotherPoint.X, anotherPoint.Y)
        {
          
        }

        // X
        public Point SetX(int x)
        {
            this.X = x; return this;
        }

        public Point IncX()
        {
            this.X++; return this;
        }

        public Point DecX()
        {
            this.X--; return this;
        }


        // Y
        public Point SetY(int y)
        {
            this.Y = y; return this;
        }

        public Point IncY()
        {
            this.Y++; return this;
        }

        public Point DecY()
        {
            this.Y--; return this;
        }


        // Other
        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            return this.Equals(p);
        }

        public bool Equals(Point anotherPoint)
        {
            return this.X == anotherPoint.X && this.Y == anotherPoint.Y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public bool IsInvalid()
        {
            return X < 0 || Y < 0;
        }

        public override string ToString()
        {
            return string.Concat("X: ", X, " , Y: ", Y);
        }

        public override int GetHashCode()
        {
            return (this.X.ToString() + "_" + this.Y.ToString()).GetHashCode();
        }
    }
}

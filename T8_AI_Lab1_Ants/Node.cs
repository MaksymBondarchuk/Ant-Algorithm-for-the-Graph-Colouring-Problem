using System;
using System.Collections.Generic;
using System.Windows;

namespace T8_AI_Lab1_Ants
{
    public class Node
    {
        //public int Id;

        /// <summary>
        /// Coordinates on Canvas
        /// </summary>
        public Point Location;
        /// <summary>
        /// Size in pixels
        /// </summary>
        public const int NodeSize = 50;
        /// <summary>
        /// Node index in Canvas.Children
        /// </summary>
        public int CanvasIdx;

        /// <summary>
        /// Contains indexes of nodes
        /// </summary>
        public readonly List<int> ConnectedWith = new List<int>();


        public int ColorNumber;
        public int ConflictsNumber;
      
        /// <summary>
        /// Checks if p is point of node
        /// </summary>
        /// <param name="p">Explored point</param>
        /// <returns></returns>
        public bool IsMyPoint(Point p)
        {
            return Math.Pow(p.X - Location.X, 2) + Math.Pow(p.Y - Location.Y, 2) <= NodeSize * NodeSize * .25;
        }

        /// <summary>
        /// Checks if p is point near node (2 radii distance)
        /// </summary>
        /// <param name="p">Explored point</param>
        /// <returns></returns>
        public bool IsNearPoint(Point p)
        {
            return Math.Pow(p.X - Location.X, 2) + Math.Pow(p.Y - Location.Y, 2) <= NodeSize * NodeSize;
        }
    }
}

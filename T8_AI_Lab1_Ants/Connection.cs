//-----------------------------------------------------------------------
// <copyright file="Connection.cs" company="NTUU 'KPI'">
//     Copyright (c) Max Bondarchuk. All rights reserved.
// </copyright>
// <author>Max Bondarchuk</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;

namespace T8_AI_Lab1_Ants
{
    /// <summary>
    /// Connection on graph
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// First point coordinates
        /// </summary>
        public Point P1;
        /// <summary>
        /// Second point coordinates
        /// </summary>
        public Point P2;
        /// <summary>
        /// First connected node
        /// </summary>
        public int Node1;
        /// <summary>
        /// Second connected node
        /// </summary>
        public int Node2;
        /// <summary>
        /// Index in Canvas.Children
        /// </summary>
        public int CanvasIdx;

        /// <summary>
        /// Checks if p is point of line
        /// </summary>
        /// <param name="p">Explored point</param>
        /// <returns></returns>
        public bool IsMyPoint(Point p)
        {
            var r = Math.Sqrt(Math.Pow(P1.X - P2.X, 2) + Math.Pow(P1.Y - P2.Y, 2));
            var r1 = Math.Sqrt(Math.Pow(p.X - P2.X, 2) + Math.Pow(p.Y - P2.Y, 2));
            var r2 = Math.Sqrt(Math.Pow(P1.X - p.X, 2) + Math.Pow(P1.Y - p.Y, 2));

            return Math.Abs(r1 + r2 - r) < 1;
        }
    }
}

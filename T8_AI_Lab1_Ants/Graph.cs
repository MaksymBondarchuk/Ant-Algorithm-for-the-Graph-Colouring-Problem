using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using T7_Course;

namespace T8_AI_Lab1_Ants
{
    public class Graph
    {
        public readonly List<Node> Nodes = new List<Node>();
        public readonly List<Connection> Connections = new List<Connection>();
        public int ChromaticNumber;
        private readonly Random _rand = new Random();
        public const int M = 1;
        public int Iterations;
        public int AntsNumber;
        public readonly List<int> Ants = new List<int>();
        public bool DebugMode { get; set; }

        /// <summary>
        /// Reads graph from file
        /// </summary>
        /// <param name="fileName">Mame of file</param>
        public void ParseFile(string fileName)
        {
            var m1 = Regex.Match(fileName, @".([\d]+)\.col$");
            ChromaticNumber = Convert.ToInt32(m1.Groups[1].Value);

            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0)
                        continue;

                    if (line[0] == 'p')
                    {
                        var nodesNumber = Convert.ToInt32(Regex.Match(line, @"\d+").Value);
                        for (var i = 0; i < nodesNumber; i++)
                            Nodes.Add(new Node());
                    }

                    if (line[0] == 'e')
                    {
                        var m = Regex.Matches(line, @"\d+");

                        //if (m.Count != 2)
                        //    continue;

                        var node1 = Convert.ToInt32(m[0].Value) - 1;
                        var node2 = Convert.ToInt32(m[1].Value) - 1;

                        Nodes[node1].ConnectedWith.Add(node2);
                        Nodes[node2].ConnectedWith.Add(node1);
                    }
                }
            }
        }

        /// <summary>
        /// Colors graph
        /// </summary>
        public void Color()
        {
            PrepareToColor();

            do OneIteration();
            while (!IsColored());
        }

        public void OneIteration()
        {
            for (var i = 0; i < Ants.Count; i++)
            {
                if (Nodes[Ants[i]].ConnectedWith.Count == 0)
                    continue;

                var maxi = Nodes[Ants[i]].ConnectedWith[0];
                // ReSharper disable once IdentifierTypo
                var confsoverall = 0;

                if (DebugMode)
                {
                    Math.Cos(5);
                }

                foreach (var neighbor in Nodes[Ants[i]].ConnectedWith)
                {
                    confsoverall += Nodes[neighbor].ConflictsNumber;

                    if (Nodes[maxi].ConflictsNumber < Nodes[neighbor].ConflictsNumber)
                        maxi = neighbor;
                }

                // ReSharper disable once IdentifierTypo
                var maxconf = Nodes[maxi].ConflictsNumber;
                var p = confsoverall != 0 ? M * maxconf * 100 / confsoverall : 0;
                if (_rand.Next(101) < p)
                    Ants[i] = maxi;
                else
                    Ants[i] = Nodes[Ants[i]].ConnectedWith[_rand.Next(Nodes[Ants[i]].ConnectedWith.Count)];
                RecolorNode(Ants[i]);
            }
            Iterations++;
        }

        public void PrepareToColor()
        {
            UpdateConflicts();

            Ants.Clear();
            for (var i = 0; i < AntsNumber; i++)
                Ants.Add(_rand.Next(Nodes.Count));
            Iterations = 0;
        }

        public bool IsColored()
        {
            return Nodes.All(node => node.ConflictsNumber == 0);
        }

        public void UpdateConflicts()
        {
            foreach (var node in Nodes)
            {
                node.ConflictsNumber = 0;
                // ReSharper disable once UnusedVariable
                foreach (var neighborIdx in
                        node.ConnectedWith.Where(neighborIdx => Nodes[neighborIdx].ColorNumber == node.ColorNumber))
                    node.ConflictsNumber++;
            }
        }

        public int GetConflictsForNode(int idx)
        {
            var node = Nodes[idx];
            //node.ConflictsNumber = 0;
            // ReSharper disable once UnusedVariable
            return node.ConnectedWith.Count(neighborIdx => Nodes[neighborIdx].ColorNumber == node.ColorNumber);
        }

        public void RecolorNode(int idx)
        {
            //var currColor = Nodes[idx].ColorNumber;
            var min = Nodes[idx].ConflictsNumber;
            var minColor = Nodes[idx].ColorNumber;
            var minColors = new List<int>();
            for (var color = 0; color < ChromaticNumber; color++)
            {
                Nodes[idx].ColorNumber = color;
                var conflicts = GetConflictsForNode(idx);
                if (conflicts < min)
                {
                    min = conflicts;
                    minColor = color;

                    minColors.Clear();
                    minColors.Add(color);
                }
                else if (conflicts == min)
                    minColors.Add(color);
            }

            // For remove possible deadlocks
            //var minColors = new List<int>();
            //for (var color = 0; color < ChromaticNumber; color++)
            //    if (GetConflictsForNode(idx) )

            //Nodes[idx].ColorNumber = minColor;
            Nodes[idx].ColorNumber = _rand.Next(101) < 70 ? minColors[_rand.Next(minColors.Count)] 
                : _rand.Next(ChromaticNumber);
            UpdateConflicts();
            //Nodes[idx].ConflictsNumber = min;

            //foreach (var neighbor in Nodes[idx].ConnectedWith)
            //    Nodes[neighbor].ConflictsNumber = GetConflictsForNode(neighbor);
        }


        public void Clear()
        {
            Nodes.Clear();
            Connections.Clear();
            ChromaticNumber = 0;
            Iterations = 0;
            AntsNumber = 0;
            Ants.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NBLib;

namespace T8_AI_Lab1_Ants
{
    public class Graph
    {
        /// <summary>
        /// Constant for ant moving logic
        /// </summary>
        private const int M = 1;

        /// <summary>
        /// Number of iterations where were no changes
        /// </summary>
        private int _deadIterationsNumber;
        /// <summary>
        /// Flag is some node was recolored on current iteration
        /// </summary>
        private bool _isSomeoneRecoloredOnThisIteration;
        /// <summary>
        /// For generate pseudo-random numbers
        /// </summary>
        private readonly Random _rand = new Random();

        private string _fileName;
        private string _fileNameResults;

        /// <summary>
        /// List of graph nodes
        /// </summary>
        public readonly List<Vertex> Vertices = new List<Vertex>();
        /// <summary>
        /// List of connections between nodes. For visual part only
        /// </summary>
        public readonly List<Edge> Edges = new List<Edge>();
        /// <summary>
        /// Chromatic number - number of colors graph can be colored in
        /// </summary>
        public int ChromaticNumber;

        /// <summary>
        /// Number of current iteration
        /// </summary>
        public int IterationNumber = -1;
        /// <summary>
        /// Number of ants
        /// </summary>
        public int AntsNumber;
        /// <summary>
        /// List of ants. Integer field - ant location (node index)
        /// </summary>
        public readonly List<int> Ants = new List<int>();

        /// <summary>
        /// Reads graph from file
        /// </summary>
        /// <param name="fileName">Mame of file</param>
        public void ParseFile(string fileName)
        {
            _fileName = fileName;
            _fileNameResults = _fileName.Substring(0, _fileName.Length - 4) + ".log";

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
                            Vertices.Add(new Vertex());
                    }

                    if (line[0] == 'e')
                    {
                        var m = Regex.Matches(line, @"\d+");

                        //if (m.Count != 2)
                        //    continue;

                        var node1 = Convert.ToInt32(m[0].Value) - 1;
                        var node2 = Convert.ToInt32(m[1].Value) - 1;

                        Vertices[node1].ConnectedWith.Add(node2);
                        Vertices[node2].ConnectedWith.Add(node1);
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
            while (!GetIsColored());
            Console.WriteLine(@"Graph is colored");
            Console.WriteLine($"Results in file {_fileNameResults}");
            WriteResultToFile();
        }

        /// <summary>
        /// Performs one iteration
        /// </summary>
        public void OneIteration()
        {
            Console.WriteLine($"Iteration #{IterationNumber} -------------------");

            for (var i = 0; i < Ants.Count; i++)
            {
                // For isolated nodes
                if (Vertices[Ants[i]].ConnectedWith.Count == 0)
                    continue;

                // Read README.md
                var maxi = Vertices[Ants[i]].ConnectedWith[0];
                // ReSharper disable once IdentifierTypo
                var confsoverall = 0;

                foreach (var neighbor in Vertices[Ants[i]].ConnectedWith)
                {
                    confsoverall += Vertices[neighbor].ConflictsNumber;

                    if (Vertices[maxi].ConflictsNumber < Vertices[neighbor].ConflictsNumber)
                        maxi = neighbor;
                }

                // ReSharper disable once IdentifierTypo
                var maxconf = Vertices[maxi].ConflictsNumber;
                var p = confsoverall != 0 ? M * maxconf * 100 / confsoverall : 0;

                Console.Write($"Ant #{i,2} moves from {Ants[i],3} to ");
                if (_rand.Next(101) < p)
                    Ants[i] = maxi;
                else
                    Ants[i] = Vertices[Ants[i]].ConnectedWith[_rand.Next(Vertices[Ants[i]].ConnectedWith.Count)];
                Console.Write($"{Ants[i],3} ");
                RecolorNode(Ants[i]);
            }
            Console.WriteLine($"Conflict nodes number is {GetConflictNodesNumber(),3}");

            // Algorithm optimization
            if (!_isSomeoneRecoloredOnThisIteration)
                _deadIterationsNumber++;
            else
                _deadIterationsNumber = 0;

            IterationNumber++;
        }

        /// <summary>
        /// Prepares ants for work
        /// </summary>
        public void PrepareToColor()
        {
            ConsoleManager.Show();
            Console.Clear();
            Console.WriteLine(@"Preparing to color");

            UpdateConflicts();

            Ants.Clear();
            for (var i = 0; i < AntsNumber; i++)
                Ants.Add(_rand.Next(Vertices.Count));
            IterationNumber = 0;
        }

        /// <summary>
        /// Checks is graph colored
        /// </summary>
        /// <returns>Is graph colored</returns>
        public bool GetIsColored()
        {
            return Vertices.All(node => node.ConflictsNumber == 0);
        }

        /// <summary>
        /// Calculates number of nodes that have conflicts
        /// </summary>
        /// <returns>Number of nodes with conflicts</returns>
        public int GetConflictNodesNumber()
        {
            return Vertices.Count(node => node.ConflictsNumber != 0);
        }

        /// <summary>
        /// Recalculates conflicts number for every node
        /// </summary>
        public void UpdateConflicts()
        {
            foreach (var node in Vertices)
            {
                node.ConflictsNumber = 0;
                // ReSharper disable once UnusedVariable
                foreach (var neighborIdx in
                        node.ConnectedWith.Where(neighborIdx => Vertices[neighborIdx].ColorNumber == node.ColorNumber))
                    node.ConflictsNumber++;
            }
        }

        /// <summary>
        /// Calculates number of conflicts for node with specified index
        /// </summary>
        /// <param name="idx">Vertex index</param>
        /// <returns>Number of conflicts</returns>
        public int GetConflictsForNode(int idx)
        {
            var node = Vertices[idx];
            // ReSharper disable once UnusedVariable
            return node.ConnectedWith.Count(neighborIdx => Vertices[neighborIdx].ColorNumber == node.ColorNumber);
        }

        /// <summary>
        /// Ant recolors its node
        /// </summary>
        /// <param name="idx">Index of node</param>
        public void RecolorNode(int idx)
        {
            // Remember color
            var currColor = Vertices[idx].ColorNumber;

            // Algorithm optimization
            if (10 <= _deadIterationsNumber)
                Vertices[idx].ColorNumber = _rand.Next(ChromaticNumber);
            else
            {
                // Find all the best solutions
                var min = Vertices[idx].ConflictsNumber;
                var minColors = new List<int>();
                for (var color = 0; color < ChromaticNumber; color++)
                {
                    Vertices[idx].ColorNumber = color;
                    var conflicts = GetConflictsForNode(idx);
                    if (conflicts < min)
                    {
                        min = conflicts;

                        minColors.Clear();
                        minColors.Add(color);
                    }
                    else if (conflicts == min)
                        minColors.Add(color);
                }

                Vertices[idx].ColorNumber = minColors[_rand.Next(minColors.Count)];
            }

            // Additional stuff
            if (currColor != Vertices[idx].ColorNumber)
            {
                Console.WriteLine($"and recolors it from {currColor,3} to {Vertices[idx].ColorNumber,3}");
                _isSomeoneRecoloredOnThisIteration = true;

                UpdateConflicts();
            }
            else
            {
                Console.WriteLine(@"and doesn't recolor it");
                _isSomeoneRecoloredOnThisIteration = false;
            }
        }

        /// <summary>
        /// Clears graph
        /// </summary>
        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            ChromaticNumber = 0;
            IterationNumber = 0;
            AntsNumber = 0;
            Ants.Clear();
        }

        /// <summary>
        /// Writes colored graph to file
        /// </summary>
        public void WriteResultToFile()
        {
            using (var file = new StreamWriter(_fileNameResults))
            {
                file.WriteLine($"c Colored graph for file {_fileName}");
                file.WriteLine("c Results in format \"n {node number} {node color}\"");
                file.WriteLine(IterationNumber == 1
                    ? "c Graph is colored in 1 iteration"
                    : $"c Graph is colored in {IterationNumber} iterations");
                for (var i = 0; i < Vertices.Count; i++)
                {
                    file.WriteLine($"n {i + 1} {Vertices[i].ColorNumber}");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace T8_AI_Lab1_Ants
{
    public class Graph
    {
        public readonly List<Node> Nodes = new List<Node>();
        public int ChromaticNumber;
        private readonly Random _rand = new Random();
        public const int M = 1;

        public void ParseFile(string fileName)
        {
            var m1 = Regex.Match(fileName, @".([\d]+)\.col$");
            ChromaticNumber = Convert.ToInt32(m1.Groups[1].Value);

            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
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

        public void Color(int antsNumber)
        {
            var ants = new List<int>();
            for (var i = 0; i < antsNumber; i++)
                ants.Add(_rand.Next(Nodes.Count));

            var iter = 0;
            do
            {
                for (var i = 0; i < ants.Count; i++)
                {
                    if (Nodes[ants[i]].ConnectedWith.Count == 0)
                        continue;

                    var maxi = 0;
                    // ReSharper disable once IdentifierTypo
                    var confsoverall = 0;
                    for (var j = 1; j < Nodes[ants[i]].ConnectedWith.Count; j++)
                    //foreach (var neighbor in Nodes[ants[i]].ConnectedWith)
                    {
                        var neighbor = Nodes[ants[i]].ConnectedWith[j];
                        confsoverall += Nodes[neighbor].ConflictsNumber;

                        if (Nodes[maxi].ConflictsNumber < Nodes[neighbor].ConflictsNumber)
                            maxi = neighbor;
                    }
                    
                    // ReSharper disable once IdentifierTypo
                    var maxconf = ants[maxi];
                    var p = M * maxconf * 100 / confsoverall;
                    if (_rand.Next(101) < p)
                        ants[i] = maxi;
                    else
                        ants[i] = Nodes[ants[i]].ConnectedWith[_rand.Next(Nodes[ants[i]].ConnectedWith.Count)];
                }

                iter++;
            } while (!IsColored());
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
                foreach (var neighborIdx in
                        node.ConnectedWith.Where(neighborIdx => Nodes[neighborIdx].ColorNumber == node.ColorNumber))
                    node.ConflictsNumber++;
            }
        }

        public void RecolorNode(int idx)
        {
            
        }
    }
}

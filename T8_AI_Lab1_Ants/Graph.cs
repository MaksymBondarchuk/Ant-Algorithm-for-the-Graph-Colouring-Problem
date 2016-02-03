using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace T8_AI_Lab1_Ants
{
    public class Graph
    {
        public readonly List<Node> Nodes = new List<Node>();
        public int ChromaticNumber;

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
    }
}

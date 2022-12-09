using System;

namespace Program // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        // Buckets
        static List<List<(char, int)>> buckets = new List<List<(char, int)>>();

        // Containing shortest paths to nodes
        static Dictionary<Char, int> shortest = new Dictionary<char, int>();

        // Graph representation
        static (char, char, int)[] graph = {
            ('A', 'B', 3),
            ('A', 'D', 5),
            ('A', 'G', 3),
            ('A', 'E', 3),
            ('B', 'C', 3),
            ('C', 'D', 1),
            ('E', 'F', 5),
            ('F', ' ', 0),
            ('G', ' ', 0)
        };

        // Ranges for buckets
        static int delta = 3;

        /* Helper method to print the contents of a bucket */
        static void PrintBucket(List<(char, int)> bucket, int index)
        {
            Console.WriteLine("BUCKET " + index);

            foreach ((char, int) node in bucket)
            {
                Console.WriteLine(node);
            }

            Console.WriteLine("\n\n");
        }

        /* Place node in bucket */
        static void Relax((char, int) node)
        {
            // Get the current best route score to the node
            int currentBest = shortest[node.Item1];

            if (currentBest > node.Item2)
            {
                // Add node to the right bucket
                int bucketIndex = node.Item2 / delta;
                buckets[bucketIndex].Add(node);

                // Update best score
                shortest[node.Item1] = node.Item2;
            }
        }

        /* Select requested light/heavy nodes */
        static List<(char, int)> FindRequests(List<(char, int)> buckets, string type)
        {
            List<(char, int)> nodes = new List<(char, int)>();

            // Iterate over all nodes in the bucket
            foreach ((char, int) node in buckets)
            {
                char nodeName = node.Item1;
                int currentShortest = node.Item2;

                // For the selected node, search all the edges
                foreach ((char, char, int) edge in graph)
                {
                    int cost = edge.Item3;

                    // Take action when outgoing node is found
                    if (nodeName == edge.Item1)
                    {
                        // Add node connected with light edge
                        if (type == "light" && cost <= delta && edge.Item2 != ' ')
                        {
                            nodes.Add((edge.Item2, currentShortest + cost));
                        }
                        // Add node connected with heavy edge
                        else if (edge.Item2 != ' ')
                        {
                            Console.WriteLine("Heavy: " + node);
                            nodes.Add((edge.Item2, currentShortest + cost));
                        }
                    }
                }
            }

            return nodes;
        }

        /* Expand selection of nodes */
        static void RelaxRequests(List<(char, int)> requestedNodes)
        {
            foreach ((char, int) node in requestedNodes)
            {
                Relax(node);
            }
        }

        /* Delta Stepping algorithm */
        static int DeltaStepping(char startNode, char destNode)
        {
            // Put start in the bucket and update best score of start
            Relax((startNode, 0));

            foreach (var bucket in buckets)
            {
                // DEBUG: log starting state of the current bucket
                PrintBucket(bucket, buckets.IndexOf(bucket));

                // The requested edges of a node (light or heavy) 
                List<(char, int)> reqEdges = new List<(char, int)>();

                while (bucket.Count != 0)
                {
                    // Request light edges
                    reqEdges = FindRequests(bucket, "light");

                    // Empty the current bucket
                    bucket.Clear();

                    // Evaluate light edges
                    RelaxRequests(reqEdges);
                }

                // Request all heavy edges
                reqEdges = FindRequests(bucket, "heavy");

                // Evaluate all heavy edges
                RelaxRequests(reqEdges);
            }

            // Return the length of the most efficient route from start to destination
            return shortest[destNode];
        }

        static void Main(string[] args)
        {
            // Insert nodes into graph
            shortest.Add('A', 100000);
            shortest.Add('B', 100000);
            shortest.Add('C', 100000);
            shortest.Add('D', 100000);
            shortest.Add('E', 100000);
            shortest.Add('F', 100000);
            shortest.Add('G', 100000);

            // Add some (shit ton of) buckets
            for (int i = 0; i < 10; i++)
            {
                buckets.Add(new List<(char, int)>());
            }

            // Find shortest path of a certain node
            Console.WriteLine(DeltaStepping('A', 'F'));
        }
    }
}

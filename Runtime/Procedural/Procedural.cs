
using System;
using System.Collections.Generic;
using Grid;

#nullable enable

namespace Procedural
{
    public class LayoutGenerator
    {
        public static GridNode Generate()
        {
            GridNode root = new(0, 0);

            GridNodeUtils.AddRectangle(root, 15, 20);
            GridNodeUtils.AddRectangle(root, 10, 10);
            GridNodeUtils.AddRectangle(root, 5, 15);

            PrintGraph(new(), root);

            return root;
        }

        private static GridNode RandomlyGenerateSomeRectangles()
        {
            Random random = new();

            GridNode root = new(0, 0);

            GridNode? current;
            for (int i = 0; i < 4; i++)
            {
                int n = random.Next(0, i * 4), counter = 0;
                current = GridNodeUtils.FindFirst(root, (node) =>
                {
                    bool result = n == counter;
                    counter++;
                    return result;
                });

                if (current == null)
                {
                    continue;
                }

                GridNodeUtils.AddRectangle(current, random.Next(5, 10), random.Next(5, 15));
            }

            return root;
        }

        public static void PrintGraph(HashSet<int> visited, GridNode? root)
        {
            if (root == null || visited.Contains(root.GetHashCode()))
            {
                return;
            }

            _ = visited.Add(root.GetHashCode());

            Console.WriteLine($"{root} : {root.GetAdjascentNodesString()}");

            foreach (GridNode? n in root.AdjascentNodes)
            {
                PrintGraph(visited, n);
            }
        }
    }
}



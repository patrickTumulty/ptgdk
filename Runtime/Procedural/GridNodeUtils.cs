using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Grid
{
    public enum Axis
    {
        X,
        Y
    }

    public class AxisUtils
    {
        public static Axis InvertAxis(Axis axis)
        {
            return axis == Axis.X ? Axis.Y : Axis.X;
        }
    }

    public enum Direction : byte
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }

    public class DirectionUtils
    {
        public static bool IsVertical(Direction direction)
        {
            return direction is Direction.Up or Direction.Down;
        }

        public static Axis GetAxis(Direction direction)
        {
            return IsVertical(direction) ? Axis.Y : Axis.X;
        }

        public static Direction InvertDirection(Direction direction)
        {
            return (Direction)InvertDirection((byte)direction);
        }

        public static byte InvertDirection(byte direction)
        {
            return (byte)((direction + 2) % 4);
        }
    }

    public static class GridNodeUtils
    {
        public static void AddRectangle(GridNode root, int width, int height)
        {
            GridNode topRight = GetOrCreateNode(root, root.X + (width - 1), root.Y);
            InsertNode(Direction.Right, root, topRight);

            GridNode bottomRight = GetOrCreateNode(root, topRight.X, topRight.Y + (height - 1));
            InsertNode(Direction.Down, topRight, bottomRight);

            GridNode bottomLeft = GetOrCreateNode(root, bottomRight.X - (width - 1), bottomRight.Y);
            InsertNode(Direction.Left, bottomRight, bottomLeft);

            InsertNode(Direction.Up, bottomLeft, root);
        }

        public static void ConnectAdjascentNode(Direction direction, GridNode node, GridNode connectingNode)
        {
            ConnectAdjascentNode((byte)direction, node, connectingNode);
        }

        public static void ConnectAdjascentNode(byte direction, GridNode node, GridNode connectingNode)
        {
            node.AdjascentNodes[direction] = connectingNode;
            byte invertedDirection = DirectionUtils.InvertDirection(direction);
            GridNode? reverseAdjascentNode = connectingNode.AdjascentNodes[invertedDirection];
            if (reverseAdjascentNode == null || reverseAdjascentNode != node)
            {
                ConnectAdjascentNode(invertedDirection, connectingNode, node);
            }
        }

        /// <summary>
        /// Get an existing node with the given coordinates, or create a new node
        /// with those coordinates
        /// </summary>
        /// <param name="root">root node</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>grid node instance</returns>
        public static GridNode GetOrCreateNode(GridNode root, int x, int y)
        {
            GridNode? exsistingInsertNode = FindFirst(root, (node) => node.X == x && node.Y == y);
            return exsistingInsertNode ?? new(x, y);
        }

        /// <summary>
        /// Insert a node into the network
        /// </summary>
        /// <param name="direction">Insert direction</param>
        /// <param name="root">root node</param>
        /// <param name="insertNode">insert node</param>
        public static void InsertNode(Direction direction, GridNode root, GridNode insertNode)
        {
            GridNode? existingNode = root.AdjascentNodes[(byte)direction];
            if (existingNode != null)
            {
                if (insertNode != existingNode)
                {
                    InsertWithExistingNode(direction, root, insertNode, existingNode);
                }
                return;
            }

            GridNode? oppositeExistingNode = insertNode.AdjascentNodes[(byte)DirectionUtils.InvertDirection(direction)];
            if (oppositeExistingNode != null)
            {
                if (insertNode != oppositeExistingNode)
                {
                    InsertWithExistingNode(DirectionUtils.InvertDirection(direction), insertNode, root, oppositeExistingNode);
                }
                return;
            }

            ConnectAdjascentNode(direction, root, insertNode);

            ConnectNewNodeWithIntersectingLine(root, insertNode);

            ConnectLineIntersections(direction, root, insertNode);
        }

        private static bool HasBeenVisited(HashSet<int> visited, GridNode node)
        {
            int hash = node.GetHashCode();
            if (visited.Contains(hash))
            {
                return true;
            }

            _ = visited.Add(hash);

            return false;
        }

        private static void InsertWithExistingNode(Direction direction, GridNode root, GridNode newNode, GridNode existingNode)
        {
            if (GreaterThan(direction, newNode, existingNode))
            {
                InsertNode(direction, existingNode, newNode);
            }
            else if (LessThan(direction, newNode, existingNode))
            {
                ConnectAdjascentNode(direction, root, newNode);
                ConnectAdjascentNode(DirectionUtils.InvertDirection(direction), existingNode, newNode);
            }
        }

        public static bool GreaterThan(Direction direction, GridNode a, GridNode b)
        {
            return a.GreaterThan(direction, b);
        }

        public static bool LessThan(Direction direction, GridNode a, GridNode b)
        {
            return a.LessThan(direction, b);
        }

        /// <summary>
        /// Node point intersects line between two points
        /// •────•────• 
        /// </summary>
        /// <param name="node">intersection node</param>
        /// <param name="nodeA">node a</param>
        /// <param name="nodeB">node b</param>
        /// <returns>true if node coordinates are between the two points</returns>
        private static bool NodePointIntersectsLine(GridNode node, GridNode nodeA, GridNode nodeB)
        {
            if (node.X == nodeA.X && node.X == nodeB.X)
            {
                return NumberUtils.WithinRangeExclusive(node.Y, nodeA.Y, nodeB.Y);
            }
            else if (node.Y == nodeA.Y && node.Y == nodeB.Y)
            {
                return NumberUtils.WithinRangeExclusive(node.X, nodeA.X, nodeB.X);
            }
            return false;
        }

        private static bool NodePassesThroughLine(Axis axis, GridNode node, GridNode nodeA, GridNode nodeB)
        {
            return axis == Axis.Y
                ? NumberUtils.WithinRangeExclusive(node.Y, nodeA.Y, nodeB.Y)
                : NumberUtils.WithinRangeExclusive(node.X, nodeA.X, nodeB.X);
        }

        /// <summary>
        /// Get the length of the line between two nodes
        /// </summary>
        /// <param name="a">Node A</param>
        /// <param name="b">Node B</param>
        /// <param name="direction">Direction</param>
        /// <return>line length between a and b</returns>
        private static int GetLineLength(GridNode a, GridNode b, Direction direction)
        {
            return direction switch
            {
                Direction.Up or Direction.Down => Math.Abs(a.Y - b.Y),
                Direction.Left or Direction.Right => Math.Abs(a.X - b.X),
                _ => 0,
            };
        }

        /// <summary>
        /// Check if inserted node lands on a perpendicular line. If so, connect
        /// the new node to the line
        /// </summary>
        /// <param name="root">root node</param>
        /// <param name="insertNode">Node that is being inserted</param>
        private static void ConnectNewNodeWithIntersectingLine(GridNode root, GridNode insertNode)
        {
            _ = FindFirst(root, (node) =>
            {
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    GridNode? adjascentNode = node.AdjascentNodes[(byte)direction];
                    if (adjascentNode == null)
                    {
                        continue;
                    }

                    if (NodePointIntersectsLine(insertNode, adjascentNode, node))
                    {
                        InsertNode(direction, node, insertNode);
                        return true;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// The line that is created between root and insertNode, check if that
        /// line intersects with any other line that are perpendicular. If so,
        /// connect those lines to the new line by adding additional nodes.
        /// </summary>
        /// <param name="direction">insert direction</param>
        /// <param name="root">root node</param>
        /// <param name="insertNode">Node that is being inserted</param>
        private static void ConnectLineIntersections(Direction direction, GridNode root, GridNode insertNode)
        {
            int lineLength = GetLineLength(root, insertNode, direction);
            Axis insertAxis = DirectionUtils.GetAxis(direction);

            TraverseBF(root, (node) =>
            {
                foreach (Direction adjascentDirection in Enum.GetValues(typeof(Direction)))
                {
                    GridNode? adjascentNode = node.AdjascentNodes[(byte)adjascentDirection];
                    if (adjascentNode == null)
                    {
                        continue;
                    }

                    Axis perpendicularAxis = DirectionUtils.GetAxis(adjascentDirection);

                    if (NodePassesThroughLine(perpendicularAxis, insertNode, adjascentNode, node) &&
                        TransientNodeWithinLineBounds(node, insertNode, direction, lineLength))
                    {
                        GridNode? newGridNode = null;
                        if (perpendicularAxis == Axis.X && insertAxis == Axis.Y)
                        {
                            newGridNode = GetOrCreateNode(root, insertNode.X, node.Y);
                        }
                        else if (perpendicularAxis == Axis.Y && insertAxis == Axis.X)
                        {
                            newGridNode = GetOrCreateNode(root, node.X, insertNode.Y);
                        }

                        if (newGridNode != null)
                        {
                            InsertNode(adjascentDirection, node, newGridNode);
                            InsertNode(direction, root, newGridNode);
                        }
                    }
                }
            });
        }


        /// <summary>
        /// Check that a transient node exists within the range of the lineNode 
        /// of a particular length in a particular direction
        /// </summary>
        /// <param name="transientNode">transient node</param>
        /// <param name="lineNode">line node</param>
        /// <param name="direction">line direction</param>
        /// <param name="lineLength">line length</param>
        /// <returns>true if transient node exists within the range of the line</returns>
        private static bool TransientNodeWithinLineBounds(GridNode transientNode, GridNode lineNode, Direction direction, int lineLength)
        {
            return direction switch
            {
                Direction.Up => transientNode.Y > lineNode.Y && transientNode.Y < (lineNode.Y + lineLength),
                Direction.Left => transientNode.X > lineNode.X && transientNode.X < (lineNode.X + lineLength),
                Direction.Down => transientNode.Y < lineNode.Y && transientNode.Y > (lineNode.Y - lineLength),
                Direction.Right => transientNode.X < lineNode.X && transientNode.X > (lineNode.X - lineLength),
                _ => false,
            };
        }

        public static (int, int, int, int) GetAreaBounds(GridNode root)
        {
            int xMin = int.MaxValue, xMax = int.MinValue, yMin = int.MaxValue, yMax = int.MinValue;
            HashSet<int> visited = new();

            TraverseBF(visited, root, (node) =>
            {
                xMin = Math.Min(xMin, node.X);
                xMax = Math.Max(xMax, node.X);
                yMin = Math.Min(yMin, node.Y);
                yMax = Math.Max(yMax, node.Y);
            });

            return (xMin, xMax, yMin, yMax);
        }

        public static void TraverseBF(GridNode? node, Action<GridNode> action)
        {
            TraverseBF(new(), node, action);
        }

        private static void TraverseBF(HashSet<int> visisted, GridNode? node, Action<GridNode> action)
        {
            if (node == null)
            {
                return;
            }

            if (HasBeenVisited(visisted, node))
            {
                return;
            }

            action(node);

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                TraverseBF(visisted, node.AdjascentNodes[(byte)direction], action);
            }
        }

        public static void TraverseDF(GridNode? node, Action<GridNode> action)
        {
            TraverseDF(new(), node, action);
        }

        private static void TraverseDF(HashSet<int> visited, GridNode? node, Action<GridNode> action)
        {
            if (node == null)
            {
                return;
            }

            if (HasBeenVisited(visited, node))
            {
                return;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                TraverseDF(visited, node.AdjascentNodes[(byte)direction], action);
            }

            action(node);
        }

        public static GridNode? FindFirst(GridNode? node, Predicate<GridNode> predicate)
        {
            return FindFirst(new(), node, predicate);
        }

        private static GridNode? FindFirst(HashSet<int> visited, GridNode? node, Predicate<GridNode> predicate)
        {
            if (node == null)
            {
                return null;
            }

            if (HasBeenVisited(visited, node))
            {
                return null;
            }

            if (predicate(node))
            {
                return node;
            }

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                GridNode? n = FindFirst(visited, node.AdjascentNodes[(byte)direction], predicate);
                if (n != null)
                {
                    return n;
                }
            }

            return null;
        }

        public static List<GridNode> CollectNodes(GridNode? node, Predicate<GridNode> predicate)
        {
            HashSet<GridNode> nodeSet = new();
            HashSet<int> visited = new();

            TraverseBF(visited, node, (currentNode) =>
            {
                if (predicate(currentNode))
                {
                    _ = nodeSet.Add(currentNode);
                }

            });

            return nodeSet.ToList();
        }
    }
}


﻿using System.Buffers;
using System.Collections.Generic;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms.AStar;

internal class NodeList
{
    private readonly List<Node> nodes = new();
    private readonly Dictionary<Node, int> nodesIndexMap = new();
    private readonly Dictionary<AStarPosition, Node> nodesMap = new();
    private readonly bool[] _openNodes;
    private int currentNode;

    public NodeList(Location location)
    {
        _openNodes = ArrayPool<bool>.Shared.Rent(512);

        currentNode = 1;
        ClosedNodes = 0;
        _openNodes[0] = true;

        var startNode = new Node(location.X, location.Y)
        {
            F = 0
        };

        nodes.Add(startNode);
        nodesIndexMap.Add(startNode, nodes.Count - 1);
        nodesMap.Add(new AStarPosition(location.X, location.Y), nodes[0]);
    }

    public int ClosedNodes { get; private set; }

    public Node GetBestNode()
    {
        var bestNodeF = int.MaxValue;
        var bestNode = -1;
        for (var i = 0; i < currentNode; ++i)
        {
            if (!_openNodes[i]) continue;

            var diffNode = nodes[i].F + nodes[i].Heuristic;

            if (diffNode < bestNodeF)
            {
                bestNodeF = diffNode;
                bestNode = i;
            }
        }

        return bestNode != -1 ? nodes[bestNode] : null;
    }

    internal void CloseNode(Node node)
    {
        int index;
        while (true)
        {
            index = nodesIndexMap[node];
            if (_openNodes[index] == false)
                ++index;
            else
                break;
        }

        if (index >= 512) return;

        _openNodes[index] = false;
        ++ClosedNodes;
    }

    internal Node CreateOpenNode(Node parent, int x, int y, int newF, int heuristic, int extraCost)
    {
        if (currentNode >= 512) return null;

        var retNode = currentNode++;
        _openNodes[retNode] = true;

        var node = new Node(x, y)
        {
            F = newF,
            Heuristic = heuristic,
            ExtraCost = extraCost,
            Parent = parent
        };
        nodes.Add(node);

        nodesIndexMap.Add(node, nodes.Count - 1);
        nodesMap.TryAdd(new AStarPosition(node.X, node.Y), node);
        return node;
    }

    internal Node GetNodeByPosition(Location location)
    {
        nodesMap.TryGetValue(new AStarPosition(location.X, location.Y), out var foundNode);
        return foundNode;
    }

    internal void OpenNode(Node node)
    {
        var index = nodesIndexMap[node];

        if (index >= 512) return;

        ClosedNodes -= _openNodes[index] ? 0 : 1;
        _openNodes[index] = true;
    }

    public void Release()
    {
        ArrayPool<bool>.Shared.Return(_openNodes);
    }
}
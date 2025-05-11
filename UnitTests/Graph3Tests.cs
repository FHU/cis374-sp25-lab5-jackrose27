using Lab5;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace UnitTests;

[TestClass]
public class Graph3WeightedTests
{
    private static UndirectedWeightedGraph CreateGraph3()
    {
        // Create a graph from the graph3.txt structure:
        // Component 1:
        //      a -- 2 -- b -- 5 -- c
        //               /
        //              /
        //             3    1
        //            /     |
        //           e      d (separate from e)
        //
        // Component 2:
        //      h -- 5 -- i -- 6 -- g
        //
        // Component 3:
        //      f (isolated node)
        
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        graph.Nodes.Add(new Node("a"));
        graph.Nodes.Add(new Node("b"));
        graph.Nodes.Add(new Node("c"));
        graph.Nodes.Add(new Node("d"));
        graph.Nodes.Add(new Node("e"));
        graph.Nodes.Add(new Node("f"));
        graph.Nodes.Add(new Node("g"));
        graph.Nodes.Add(new Node("h"));
        graph.Nodes.Add(new Node("i"));
        
        // Add edges with weights
        graph.AddEdge("a", "b", 2);
        graph.AddEdge("b", "c", 5);
        graph.AddEdge("b", "e", 3);
        graph.AddEdge("b", "d", 1);
        graph.AddEdge("h", "i", 5);
        graph.AddEdge("g", "i", 6);
        
        return graph;
    }
    
    [TestMethod]
    public void TestGraph3Weighted_FromFile()
    {
        // This test assumes the file is located at the path "../../../graphs/graph3.txt"
        string filePath = "../../../graphs/graph3.txt";
        
        // Skip test if file doesn't exist
        if (!File.Exists(filePath))
        {
            Assert.Inconclusive($"Test file {filePath} not found. Please place it in the correct location.");
            return;
        }
        
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph(filePath);
        
        // Test nodes count
        Assert.AreEqual(9, graph.Nodes.Count);
        
        // Test edge existence and weights from node b
        bool foundAB = false, foundBC = false, foundBE = false, foundBD = false;
        Node nodeB = graph.GetNodeByName("b");
        
        foreach (var neighbor in nodeB.Neighbors)
        {
            if (neighbor.Node.Name == "a" && neighbor.Weight == 2) foundAB = true;
            if (neighbor.Node.Name == "c" && neighbor.Weight == 5) foundBC = true;
            if (neighbor.Node.Name == "e" && neighbor.Weight == 3) foundBE = true;
            if (neighbor.Node.Name == "d" && neighbor.Weight == 1) foundBD = true;
        }
        
        Assert.IsTrue(foundAB, "Edge a-b with weight 2 not found");
        Assert.IsTrue(foundBC, "Edge b-c with weight 5 not found");
        Assert.IsTrue(foundBE, "Edge b-e with weight 3 not found");
        Assert.IsTrue(foundBD, "Edge b-d with weight 1 not found");
        
        // Test edge existence and weights for the second component
        bool foundHI = false, foundGI = false;
        Node nodeI = graph.GetNodeByName("i");
        
        foreach (var neighbor in nodeI.Neighbors)
        {
            if (neighbor.Node.Name == "h" && neighbor.Weight == 5) foundHI = true;
            if (neighbor.Node.Name == "g" && neighbor.Weight == 6) foundGI = true;
        }
        
        Assert.IsTrue(foundHI, "Edge h-i with weight 5 not found");
        Assert.IsTrue(foundGI, "Edge g-i with weight 6 not found");
        
        // Test that node f is isolated (has no neighbors)
        Node nodeF = graph.GetNodeByName("f");
        Assert.AreEqual(0, nodeF.Neighbors.Count);
    }
    
    [TestMethod]
    public void TestGraph3Weighted_ConnectedComponents()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Graph should have 3 connected components
        Assert.AreEqual(3, graph.ConnectedComponents);
    }
    
    [TestMethod]
    public void TestGraph3Weighted_Reachability_Component1()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // All nodes in component 1 should be reachable from each other
        Assert.IsTrue(graph.IsReachable("a", "c"));
        Assert.IsTrue(graph.IsReachable("a", "d"));
        Assert.IsTrue(graph.IsReachable("a", "e"));
        Assert.IsTrue(graph.IsReachable("b", "c"));
        Assert.IsTrue(graph.IsReachable("b", "d"));
        Assert.IsTrue(graph.IsReachable("b", "e"));
        Assert.IsTrue(graph.IsReachable("c", "d"));
        Assert.IsTrue(graph.IsReachable("c", "e"));
        Assert.IsTrue(graph.IsReachable("d", "e"));
        
        // Test some reverse directions as well
        Assert.IsTrue(graph.IsReachable("c", "a"));
        Assert.IsTrue(graph.IsReachable("d", "a"));
        Assert.IsTrue(graph.IsReachable("e", "a"));
    }
    
    [TestMethod]
    public void TestGraph3Weighted_Reachability_Component2()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // All nodes in component 2 should be reachable from each other
        Assert.IsTrue(graph.IsReachable("g", "h"));
        Assert.IsTrue(graph.IsReachable("g", "i"));
        Assert.IsTrue(graph.IsReachable("h", "i"));
        
        // Test reverse directions as well
        Assert.IsTrue(graph.IsReachable("h", "g"));
        Assert.IsTrue(graph.IsReachable("i", "g"));
        Assert.IsTrue(graph.IsReachable("i", "h"));
    }
    
    [TestMethod]
    public void TestGraph3Weighted_Reachability_BetweenComponents()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Nodes from different components should NOT be reachable from each other
        Assert.IsFalse(graph.IsReachable("a", "g"));
        Assert.IsFalse(graph.IsReachable("b", "h"));
        Assert.IsFalse(graph.IsReachable("c", "i"));
        Assert.IsFalse(graph.IsReachable("d", "g"));
        Assert.IsFalse(graph.IsReachable("e", "h"));
        
        // Node f should not be reachable from any other node
        Assert.IsFalse(graph.IsReachable("a", "f"));
        Assert.IsFalse(graph.IsReachable("g", "f"));
        Assert.IsFalse(graph.IsReachable("h", "f"));
    }
    
    [TestMethod]
    public void TestGraph3Weighted_DFSPathBetween_Component1()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test DFS path from a to c
        int costAC = graph.DFSPathBetween("a", "c", out List<Node> pathAC);
        Assert.AreEqual(7, costAC); // Path should be a -> b -> c with cost 2+5=7
        Assert.AreEqual(3, pathAC.Count);
        Assert.AreEqual("a", pathAC[0].Name);
        Assert.AreEqual("b", pathAC[1].Name);
        Assert.AreEqual("c", pathAC[2].Name);
        
        // Test DFS path from a to d
        int costAD = graph.DFSPathBetween("a", "d", out List<Node> pathAD);
        Assert.AreEqual(3, costAD); // Path should be a -> b -> d with cost 2+1=3
        Assert.AreEqual(3, pathAD.Count);
        Assert.AreEqual("a", pathAD[0].Name);
        Assert.AreEqual("b", pathAD[1].Name);
        Assert.AreEqual("d", pathAD[2].Name);
        
        // Test DFS path from e to c
        int costEC = graph.DFSPathBetween("e", "c", out List<Node> pathEC);
        Assert.AreEqual(8, costEC); // Path should be e -> b -> c with cost 3+5=8
        Assert.AreEqual(3, pathEC.Count);
        Assert.AreEqual("e", pathEC[0].Name);
        Assert.AreEqual("b", pathEC[1].Name);
        Assert.AreEqual("c", pathEC[2].Name);
    }
    
    [TestMethod]
    public void TestGraph3Weighted_DFSPathBetween_Component2()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test DFS path from g to h
        int costGH = graph.DFSPathBetween("g", "h", out List<Node> pathGH);
        Assert.AreEqual(11, costGH); // Path should be g -> i -> h with cost 6+5=11
        Assert.AreEqual(3, pathGH.Count);
        Assert.AreEqual("g", pathGH[0].Name);
        Assert.AreEqual("i", pathGH[1].Name);
        Assert.AreEqual("h", pathGH[2].Name);
    }
    
    [TestMethod]
    public void TestGraph3Weighted_DFSPathBetween_BetweenComponents()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test DFS path from a to g (different components)
        int costAG = graph.DFSPathBetween("a", "g", out List<Node> pathAG);
        Assert.AreEqual(-1, costAG); // Should return -1 as no path exists
        Assert.AreEqual(0, pathAG.Count); // Path list should be empty
        
        // Test DFS path from c to h (different components)
        int costCH = graph.DFSPathBetween("c", "h", out List<Node> pathCH);
        Assert.AreEqual(-1, costCH); // Should return -1 as no path exists
        Assert.AreEqual(0, pathCH.Count); // Path list should be empty
        
        // Test DFS path from any node to f (isolated node)
        int costAF = graph.DFSPathBetween("a", "f", out List<Node> pathAF);
        Assert.AreEqual(-1, costAF); // Should return -1 as no path exists
        Assert.AreEqual(0, pathAF.Count); // Path list should be empty
    }
    
    [TestMethod]
    public void TestGraph3Weighted_BFSPathBetween()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test BFS path from a to c
        int costAC = graph.BFSPathBetween("a", "c", out List<Node> pathAC);
        Assert.AreEqual(7, costAC); // Path should be a -> b -> c with cost 2+5=7
        Assert.AreEqual(3, pathAC.Count);
        Assert.AreEqual("a", pathAC[0].Name);
        Assert.AreEqual("b", pathAC[1].Name);
        Assert.AreEqual("c", pathAC[2].Name);
        
        // Test BFS path from a to d
        int costAD = graph.BFSPathBetween("a", "d", out List<Node> pathAD);
        Assert.AreEqual(3, costAD); // Path should be a -> b -> d with cost 2+1=3
        Assert.AreEqual(3, pathAD.Count);
        Assert.AreEqual("a", pathAD[0].Name);
        Assert.AreEqual("b", pathAD[1].Name);
        Assert.AreEqual("d", pathAD[2].Name);
        
        // Test BFS path between nodes in component 2
        int costGH = graph.BFSPathBetween("g", "h", out List<Node> pathGH);
        Assert.AreEqual(11, costGH); // Path should be g -> i -> h with cost 6+5=11
        Assert.AreEqual(3, pathGH.Count);
        Assert.AreEqual("g", pathGH[0].Name);
        Assert.AreEqual("i", pathGH[1].Name);
        Assert.AreEqual("h", pathGH[2].Name);
        
        // Test BFS path between different components (should fail)
        int costAG = graph.BFSPathBetween("a", "g", out List<Node> pathAG);
        Assert.AreEqual(-1, costAG); // Should return -1 as no path exists
        Assert.AreEqual(0, pathAG.Count); // Path list should be empty
    }
    
    [TestMethod]
    public void TestGraph3Weighted_DijkstraPathBetween()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test Dijkstra path from a to c
        int costAC = graph.DijkstraPathBetween("a", "c", out List<Node> pathAC);
        Assert.AreEqual(7, costAC); // Path should be a -> b -> c with cost 2+5=7
        Assert.AreEqual(3, pathAC.Count);
        Assert.AreEqual("a", pathAC[0].Name);
        Assert.AreEqual("b", pathAC[1].Name);
        Assert.AreEqual("c", pathAC[2].Name);
        
        // Test Dijkstra path from a to d
        int costAD = graph.DijkstraPathBetween("a", "d", out List<Node> pathAD);
        Assert.AreEqual(3, costAD); // Path should be a -> b -> d with cost 2+1=3
        Assert.AreEqual(3, pathAD.Count);
        Assert.AreEqual("a", pathAD[0].Name);
        Assert.AreEqual("b", pathAD[1].Name);
        Assert.AreEqual("d", pathAD[2].Name);
        
        // Test Dijkstra path from c to e
        int costCE = graph.DijkstraPathBetween("c", "e", out List<Node> pathCE);
        Assert.AreEqual(8, costCE); // Path should be c -> b -> e with cost 5+3=8
        Assert.AreEqual(3, pathCE.Count);
        Assert.AreEqual("c", pathCE[0].Name);
        Assert.AreEqual("b", pathCE[1].Name);
        Assert.AreEqual("e", pathCE[2].Name);
        
        // Test Dijkstra path from g to h in component 2
        int costGH = graph.DijkstraPathBetween("g", "h", out List<Node> pathGH);
        Assert.AreEqual(11, costGH); // Path should be g -> i -> h with cost 6+5=11
        Assert.AreEqual(3, pathGH.Count);
        Assert.AreEqual("g", pathGH[0].Name);
        Assert.AreEqual("i", pathGH[1].Name);
        Assert.AreEqual("h", pathGH[2].Name);
        
        // Test Dijkstra path between different components (should fail)
        int costAH = graph.DijkstraPathBetween("a", "h", out List<Node> pathAH);
        Assert.AreEqual(-1, costAH); // Should return -1 as no path exists
        Assert.AreEqual(0, pathAH.Count); // Path list should be empty
        
        // Test Dijkstra path to isolated node f (should fail)
        int costDF = graph.DijkstraPathBetween("d", "f", out List<Node> pathDF);
        Assert.AreEqual(-1, costDF); // Should return -1 as no path exists
        Assert.AreEqual(0, pathDF.Count); // Path list should be empty
    }
    
    [TestMethod]
    public void TestGraph3Weighted_CompareDifferentAlgorithms()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // For paths where there is only one possible route, all algorithms should return the same result
        // Test path from a to c
        int dfsCostAC = graph.DFSPathBetween("a", "c", out List<Node> dfsPathAC);
        int bfsCostAC = graph.BFSPathBetween("a", "c", out List<Node> bfsPathAC);
        int dijkstraCostAC = graph.DijkstraPathBetween("a", "c", out List<Node> dijkstraPathAC);
        
        Assert.AreEqual(7, dfsCostAC);
        Assert.AreEqual(7, bfsCostAC);
        Assert.AreEqual(7, dijkstraCostAC);
        
        // All should find the same path in this case
        Assert.AreEqual(3, dfsPathAC.Count);
        Assert.AreEqual(3, bfsPathAC.Count);
        Assert.AreEqual(3, dijkstraPathAC.Count);
        
        // Similarly, for paths between different components, all algorithms should indicate no path
        // Test path from a to h
        int dfsCostAH = graph.DFSPathBetween("a", "h", out List<Node> dfsPathAH);
        int bfsCostAH = graph.BFSPathBetween("a", "h", out List<Node> bfsPathAH);
        int dijkstraCostAH = graph.DijkstraPathBetween("a", "h", out List<Node> dijkstraPathAH);
        
        Assert.AreEqual(-1, dfsCostAH);
        Assert.AreEqual(-1, bfsCostAH);
        Assert.AreEqual(-1, dijkstraCostAH);
        
        Assert.AreEqual(0, dfsPathAH.Count);
        Assert.AreEqual(0, bfsPathAH.Count);
        Assert.AreEqual(0, dijkstraPathAH.Count);
    }
    
    [TestMethod]
    public void TestGraph3Weighted_GetNodeByName()
    {
        UndirectedWeightedGraph graph = CreateGraph3();
        
        // Test getting existing nodes
        Node nodeA = graph.GetNodeByName("a");
        Node nodeF = graph.GetNodeByName("f");
        Node nodeI = graph.GetNodeByName("i");
        
        Assert.IsNotNull(nodeA);
        Assert.IsNotNull(nodeF);
        Assert.IsNotNull(nodeI);
        Assert.AreEqual("a", nodeA.Name);
        Assert.AreEqual("f", nodeF.Name);
        Assert.AreEqual("i", nodeI.Name);
        
        // Verify that node f is isolated
        Assert.AreEqual(0, nodeF.Neighbors.Count);
        
        // Verify that node i has exactly 2 neighbors (g and h)
        Assert.AreEqual(2, nodeI.Neighbors.Count);
        
        // Test getting non-existent node
        Node nodeX = graph.GetNodeByName("x");
        Assert.IsNull(nodeX);
    }
}
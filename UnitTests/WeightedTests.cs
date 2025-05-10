using Lab5;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace UnitTests;

[TestClass]
public class Graph1WeightedTests
{
    private static UndirectedWeightedGraph CreateGraph1()
    {
        // Create a graph from the graph1-weighted.txt structure:
        // a -- 2 -- b -- 5 -- c
        //          /|\
        //         / | \
        //        3  1  5
        //       /   |   \
        //      e    d     
        
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        graph.Nodes.Add(new Node("a"));
        graph.Nodes.Add(new Node("b"));
        graph.Nodes.Add(new Node("c"));
        graph.Nodes.Add(new Node("d"));
        graph.Nodes.Add(new Node("e"));
        
        // Add edges with weights
        graph.AddEdge("a", "b", 2);
        graph.AddEdge("b", "c", 5);
        graph.AddEdge("b", "e", 3);
        graph.AddEdge("b", "d", 1);
        
        return graph;
    }
    
    [TestMethod]
    public void TestGraph1Weighted_FromFile()
    {
        // This test assumes the file is located at the path "../../../graphs/graph1-weighted.txt"
        string filePath = "../../../graphs/graph1-weighted.txt";
        
        // Skip test if file doesn't exist
        if (!File.Exists(filePath))
        {
            Assert.Inconclusive($"Test file {filePath} not found. Please place it in the correct location.");
            return;
        }
        
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph(filePath);
        
        // Test nodes count
        Assert.AreEqual(5, graph.Nodes.Count);
        
        // Test edge existence and weights
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
    }
    
    [TestMethod]
    public void TestGraph1Weighted_ConnectedComponents()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
        // Graph should have 1 connected component
        Assert.AreEqual(1, graph.ConnectedComponents);
    }
    
    [TestMethod]
    public void TestGraph1Weighted_Reachability()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
        // Every node should be reachable from every other node
        Assert.IsTrue(graph.IsReachable("a", "c"));
        Assert.IsTrue(graph.IsReachable("a", "d"));
        Assert.IsTrue(graph.IsReachable("a", "e"));
        Assert.IsTrue(graph.IsReachable("c", "d"));
        Assert.IsTrue(graph.IsReachable("c", "e"));
        Assert.IsTrue(graph.IsReachable("d", "e"));
        
        // Test reverse directions as well
        Assert.IsTrue(graph.IsReachable("c", "a"));
        Assert.IsTrue(graph.IsReachable("d", "a"));
        Assert.IsTrue(graph.IsReachable("e", "a"));
    }
    
    [TestMethod]
    public void TestGraph1Weighted_DFSPathBetween()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
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
    public void TestGraph1Weighted_BFSPathBetween()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
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
    }
    
    [TestMethod]
    public void TestGraph1Weighted_DijkstraPathBetween()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
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
        
        // Test Dijkstra path from c to d
        int costCD = graph.DijkstraPathBetween("c", "d", out List<Node> pathCD);
        Assert.AreEqual(6, costCD); // Path should be c -> b -> d with cost 5+1=6
        Assert.AreEqual(3, pathCD.Count);
        Assert.AreEqual("c", pathCD[0].Name);
        Assert.AreEqual("b", pathCD[1].Name);
        Assert.AreEqual("d", pathCD[2].Name);
    }
    
    [TestMethod]
    public void TestGraph1Weighted_MultiplePathsFromFile()
    {
        string filePath = "../../../graphs/graph1-weighted.txt";
        
        // Skip test if file doesn't exist
        if (!File.Exists(filePath))
        {
            Assert.Inconclusive($"Test file {filePath} not found. Please place it in the correct location.");
            return;
        }
        
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph(filePath);
        
        // Test multiple paths between nodes to ensure algorithms work correctly
        
        // DFS from a to c
        int dfsCostAC = graph.DFSPathBetween("a", "c", out List<Node> dfsPathAC);
        Assert.AreEqual(7, dfsCostAC);
        
        // BFS from a to c
        int bfsCostAC = graph.BFSPathBetween("a", "c", out List<Node> bfsPathAC);
        Assert.AreEqual(7, bfsCostAC);
        
        // Dijkstra from a to c
        int dijkstraCostAC = graph.DijkstraPathBetween("a", "c", out List<Node> dijkstraPathAC);
        Assert.AreEqual(7, dijkstraCostAC);
        
        // All should find the same path in this case
        Assert.AreEqual(3, dfsPathAC.Count);
        Assert.AreEqual(3, bfsPathAC.Count);
        Assert.AreEqual(3, dijkstraPathAC.Count);
    }
    
    [TestMethod]
    public void TestGraph1Weighted_GetNodeByName()
    {
        UndirectedWeightedGraph graph = CreateGraph1();
        
        // Add a private method accessor for testing the private GetNodeByName method
        // Note: This is using reflection to access the private method - in a real project,
        // you might want to consider making this method public or internal for testing
        var method = typeof(UndirectedWeightedGraph).GetMethod("GetNodeByName", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method == null)
        {
            Assert.Inconclusive("The GetNodeByName method could not be accessed. It might be renamed or removed.");
            return;
        }
        
        // Test getting existing nodes
        var nodeA = method.Invoke(graph, new object[] { "a" }) as Node;
        var nodeC = method.Invoke(graph, new object[] { "c" }) as Node;
        
        Assert.IsNotNull(nodeA);
        Assert.IsNotNull(nodeC);
        Assert.AreEqual("a", nodeA.Name);
        Assert.AreEqual("c", nodeC.Name);
        
        // Test getting non-existent node
        var nodeX = method.Invoke(graph, new object[] { "x" }) as Node;
        Assert.IsNull(nodeX);
    }
}
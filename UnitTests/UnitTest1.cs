using Lab5;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests;

[TestClass]
public class WeightedGraphTests
{
    [TestMethod]
    public void TestDFSPathBetween_SimpleGraph()
    {
        // Create a simple weighted graph
        // A -- 5 -- B
        // |         |
        // 2         3
        // |         |
        // C -- 1 -- D
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        graph.Nodes.Add(new Node("C"));
        graph.Nodes.Add(new Node("D"));
        
        // Add edges
        graph.AddEdge("A", "B", 5);
        graph.AddEdge("A", "C", 2);
        graph.AddEdge("B", "D", 3);
        graph.AddEdge("C", "D", 1);
        
        // Test path A to D
        int cost = graph.DFSPathBetween("A", "D", out List<Node> path);
        
        // In DFS, we would expect the path A -> B -> D (cost 8) because B comes before C alphabetically
        Assert.AreEqual(8, cost);
        Assert.AreEqual(3, path.Count);
        Assert.AreEqual("A", path[0].Name);
        Assert.AreEqual("B", path[1].Name);
        Assert.AreEqual("D", path[2].Name);
        
        // Test path D to A
        cost = graph.DFSPathBetween("D", "A", out path);
        
        // In DFS, we would expect the path D -> B -> A or D -> C -> A depending on order
        // Since B comes before C alphabetically, expect D -> B -> A
        Assert.AreEqual(8, cost);
        Assert.AreEqual(3, path.Count);
        Assert.AreEqual("D", path[0].Name);
        Assert.AreEqual("B", path[1].Name);
        Assert.AreEqual("A", path[2].Name);
    }
    
    [TestMethod]
    public void TestBFSPathBetween_SimpleGraph()
    {
        // Create a simple weighted graph
        // A -- 5 -- B
        // |         |
        // 2         3
        // |         |
        // C -- 1 -- D
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        graph.Nodes.Add(new Node("C"));
        graph.Nodes.Add(new Node("D"));
        
        // Add edges
        graph.AddEdge("A", "B", 5);
        graph.AddEdge("A", "C", 2);
        graph.AddEdge("B", "D", 3);
        graph.AddEdge("C", "D", 1);
        
        // Test path A to D
        int cost = graph.BFSPathBetween("A", "D", out List<Node> path);
        
        // In BFS, we would expect the path A -> C -> D (cost 3) because it has fewer edges
        Assert.AreEqual(3, cost);
        Assert.AreEqual(3, path.Count);
        Assert.AreEqual("A", path[0].Name);
        Assert.AreEqual("C", path[1].Name);
        Assert.AreEqual("D", path[2].Name);
    }
    
    [TestMethod]
    public void TestDijkstraPathBetween_SimpleGraph()
    {
        // Create a simple weighted graph
        // A -- 5 -- B
        // |         |
        // 2         3
        // |         |
        // C -- 1 -- D
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        graph.Nodes.Add(new Node("C"));
        graph.Nodes.Add(new Node("D"));
        
        // Add edges
        graph.AddEdge("A", "B", 5);
        graph.AddEdge("A", "C", 2);
        graph.AddEdge("B", "D", 3);
        graph.AddEdge("C", "D", 1);
        
        // Test path A to D
        int cost = graph.DijkstraPathBetween("A", "D", out List<Node> path);
        
        // Dijkstra should find the shortest path A -> C -> D (cost 3)
        Assert.AreEqual(3, cost);
        Assert.AreEqual(3, path.Count);
        Assert.AreEqual("A", path[0].Name);
        Assert.AreEqual("C", path[1].Name);
        Assert.AreEqual("D", path[2].Name);
    }
    
    [TestMethod]
    public void TestDijkstraPathBetween_ComplexGraph()
    {
        // Create a more complex weighted graph with multiple possible paths
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        for (char c = 'A'; c <= 'H'; c++)
        {
            graph.Nodes.Add(new Node(c.ToString()));
        }
        
        // Add edges to create multiple paths
        graph.AddEdge("A", "B", 4);
        graph.AddEdge("A", "C", 3);
        graph.AddEdge("B", "C", 5);
        graph.AddEdge("B", "D", 2);
        graph.AddEdge("C", "D", 7);
        graph.AddEdge("C", "E", 8);
        graph.AddEdge("D", "E", 4);
        graph.AddEdge("D", "F", 6);
        graph.AddEdge("E", "F", 3);
        graph.AddEdge("E", "G", 9);
        graph.AddEdge("F", "G", 5);
        graph.AddEdge("F", "H", 1);
        graph.AddEdge("G", "H", 2);
        
        // Test path A to H
        int cost = graph.DijkstraPathBetween("A", "H", out List<Node> path);
        
        // Dijkstra should find the shortest path (checking cost only for flexibility)
        Assert.AreEqual(13, cost);
        Assert.AreEqual("A", path[0].Name);
        Assert.AreEqual("H", path[path.Count - 1].Name);
    }
    
    [TestMethod]
    public void TestConnectedComponents_MultipleComponents()
    {
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes for three separate components
        // Component 1: A-B-C
        // Component 2: D-E
        // Component 3: F
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        graph.Nodes.Add(new Node("C"));
        graph.Nodes.Add(new Node("D"));
        graph.Nodes.Add(new Node("E"));
        graph.Nodes.Add(new Node("F"));
        
        // Add edges
        graph.AddEdge("A", "B", 1);
        graph.AddEdge("B", "C", 1);
        graph.AddEdge("D", "E", 1);
        
        // Check number of connected components
        Assert.AreEqual(3, graph.ConnectedComponents);
    }
    
    [TestMethod]
    public void TestIsReachable_WeightedGraph()
    {
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Create two separate components
        // Component 1: A-B-C
        // Component 2: D-E
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        graph.Nodes.Add(new Node("C"));
        graph.Nodes.Add(new Node("D"));
        graph.Nodes.Add(new Node("E"));
        
        // Add edges
        graph.AddEdge("A", "B", 5);
        graph.AddEdge("B", "C", 3);
        graph.AddEdge("D", "E", 2);
        
        // Test reachability within the same component
        Assert.IsTrue(graph.IsReachable("A", "C"));
        Assert.IsTrue(graph.IsReachable("C", "A"));
        Assert.IsTrue(graph.IsReachable("D", "E"));
        
        // Test reachability between different components
        Assert.IsFalse(graph.IsReachable("A", "D"));
        Assert.IsFalse(graph.IsReachable("C", "E"));
    }
    
    [TestMethod]
    public void TestPathToSelf_AllAlgorithms()
    {
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add nodes
        for (char c = 'A'; c <= 'E'; c++)
        {
            graph.Nodes.Add(new Node(c.ToString()));
        }
        
        // Add some edges
        graph.AddEdge("A", "B", 1);
        graph.AddEdge("B", "C", 2);
        graph.AddEdge("C", "D", 3);
        graph.AddEdge("D", "E", 4);
        
        // Test DFS path to self
        int costDFS = graph.DFSPathBetween("C", "C", out List<Node> pathDFS);
        Assert.AreEqual(0, costDFS); // Should be zero cost
        Assert.AreEqual(1, pathDFS.Count); // Should just contain C
        Assert.AreEqual("C", pathDFS[0].Name);
        
        // Test BFS path to self
        int costBFS = graph.BFSPathBetween("C", "C", out List<Node> pathBFS);
        Assert.AreEqual(0, costBFS); // Should be zero cost
        Assert.AreEqual(1, pathBFS.Count); // Should just contain C
        Assert.AreEqual("C", pathBFS[0].Name);
        
        // Test Dijkstra path to self
        int costDijkstra = graph.DijkstraPathBetween("C", "C", out List<Node> pathDijkstra);
        Assert.AreEqual(0, costDijkstra); // Should be zero cost
        Assert.AreEqual(1, pathDijkstra.Count); // Should just contain C
        Assert.AreEqual("C", pathDijkstra[0].Name);
    }
    
    [TestMethod]
    public void TestNoPathExists_AllAlgorithms()
    {
        UndirectedWeightedGraph graph = new UndirectedWeightedGraph();
        
        // Add two disconnected nodes
        graph.Nodes.Add(new Node("A"));
        graph.Nodes.Add(new Node("B"));
        
        // Test DFS when no path exists
        int costDFS = graph.DFSPathBetween("A", "B", out List<Node> pathDFS);
        Assert.AreEqual(-1, costDFS); // Should indicate no path
        Assert.AreEqual(0, pathDFS.Count); // Should be empty
        
        // Test BFS when no path exists
        int costBFS = graph.BFSPathBetween("A", "B", out List<Node> pathBFS);
        Assert.AreEqual(-1, costBFS); // Should indicate no path
        Assert.AreEqual(0, pathBFS.Count); // Should be empty
        
        // Test Dijkstra when no path exists
        int costDijkstra = graph.DijkstraPathBetween("A", "B", out List<Node> pathDijkstra);
        Assert.AreEqual(-1, costDijkstra); // Should indicate no path
        Assert.AreEqual(0, pathDijkstra.Count); // Should be empty
    }
    
    [TestMethod]
    public void TestFromFile_WeightedGraph()
    {
        // Create a temporary file with graph data
        string filePath = "test_weighted_graph.txt";
        System.IO.File.WriteAllText(filePath, "A B C D\nA B 5\nA C 2\nB D 3\nC D 1");
        
        try
        {
            // Create graph from file
            UndirectedWeightedGraph graph = new UndirectedWeightedGraph(filePath);
            
            // Test nodes
            Assert.AreEqual(4, graph.Nodes.Count);
            
            // Test Dijkstra path
            int cost = graph.DijkstraPathBetween("A", "D", out List<Node> path);
            Assert.AreEqual(3, cost); // Path A -> C -> D with cost 2+1=3
            Assert.AreEqual(3, path.Count);
        }
        finally
        {
            // Clean up the temporary file
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
using System.Text.RegularExpressions;

namespace Lab5;

public class UndirectedWeightedGraph
{
    public List<Node> Nodes { get; set; }

    public UndirectedWeightedGraph()
    {
        Nodes = new List<Node>();
    }

    public UndirectedWeightedGraph(string path)
    {
        Nodes = new List<Node>();

        List<string> lines = new List<string>();

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == "")
                    {
                        continue;
                    }
                    if (line[0] == '#')
                    {
                        continue;
                    }

                    lines.Add(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // process the lines
        if (lines.Count < 1)
        {
            // empty file
            Console.WriteLine("Graph file was empty");
            return;
        }

        // Add nodes
        string[] nodeNames = Regex.Split(lines[0], @"\W+");

        foreach (var name in nodeNames)
        {
            Nodes.Add(new Node(name));
        }

        string[] nodeNamesAndWeight;
        // Add edges
        for (int i = 1; i < lines.Count; i++)
        {
            // extract node names
            nodeNamesAndWeight = Regex.Split(lines[i], @"\W+");
            if (nodeNamesAndWeight.Length < 3)
            {
                throw new Exception("Two nodes and a weight are required for each edge.");
            }

            // add edge between those nodes
            AddEdge(nodeNamesAndWeight[0], nodeNamesAndWeight[1], int.Parse(nodeNamesAndWeight[2]));
        }
    }
    private Node FindNode(string nodeName)
{
    return GetNodeByName(nodeName);
}

    public void AddEdge(string node1Name, string node2Name, int weight)
    {
        Node node1 = GetNodeByName(node1Name);
        Node node2 = GetNodeByName(node2Name);

        if (node1 == null || node2 == null)
        {
            throw new Exception("Invalid node name");
        }

        node1.Neighbors.Add(new Neighbor() { Node = node2, Weight = weight });
        node2.Neighbors.Add(new Neighbor() { Node = node1, Weight = weight });
    }

    private Node GetNodeByName(string nodeName)
    {
        var node = Nodes.Find(node => node.Name == nodeName);

        return node;
    }

    public int ConnectedComponents
{
    get
    {
        int numConnectedComponents = 0;
        
        // Reset all nodes to white
        foreach (Node node in Nodes)
        {
            node.Color = Color.White;
        }
        
        // For each unvisited node, do a DFS and increment counter
        foreach (Node node in Nodes)
        {
            if (node.Color == Color.White)
            {
                DFS(node, false); // Use existing DFS without resetting colors
                numConnectedComponents++;
            }
        }
        
        return numConnectedComponents;
    }
}


    public bool IsReachable(string node1name, string node2name)
    {
        Node node1 = GetNodeByName(node1name);
        Node node2 = GetNodeByName(node2name);

        if (node1 == null || node2 == null)
        {
            throw new Exception($"{node1name} or {node2name} does not exist.)");
        }

        // Do a DFS
        var pred = DFS(node1);

        // Was a pred for node2 found?
        return pred[node2] != null;
    }


    /// <summary>
    /// Searches the graph in a depth-first manner, creating a
    /// dictionary of the Node to Predessor Node links discovered by starting at the given node.
    /// Neighbors are visited in alphabetical order. 
    /// </summary>
    /// <param name="startingNode">The starting node of the depth first search</param>
    /// <returns>A dictionary of the Node to Predecessor Node 
    /// for each node in the graph reachable from the starting node
    /// as discovered by a DFS.</returns>
    public Dictionary<Node, Node> DFS(Node startingNode, bool reset = true)
    {
        Dictionary<Node, Node> pred = new Dictionary<Node, Node>();

        if (reset)
        {
            // setup DFS
            foreach (Node node in Nodes)
            {
                pred[node] = null;
                node.Color = Color.White;
            }
        }

        // call the recursive method
        DFSVisit(startingNode, pred);

        return pred;
    }
    private bool DFSVisit(Node current, Node target, Dictionary<string, Node> pred)
{
    // Color node gray
    current.Color = Color.Gray;
    
    // If current node is the target, we found it
    if (current == target)
        return true;
    
    // Sort neighbors for consistent alphabetical traversal
    current.Neighbors.Sort();
    
    // Visit all white neighbors
    foreach (var neighbor in current.Neighbors)
    {
        if (neighbor.Node.Color == Color.White)
        {
            pred[neighbor.Node.Name] = current;
            
            // If found target, return true
            if (DFSVisit(neighbor.Node, target, pred))
                return true;
        }
    }
    
    // Color node black
    current.Color = Color.Black;
    return false;
}

    //TODO
    /// <summary>
    /// Find the first path between the given nodes in a DFS manner 
    /// and return its total cost. Choices/ties are made in alphabetical order. 
    /// </summary>
    /// <param name="node1name">The starting node's name</param>
    /// <param name="node2name">The ending node's name</param>
    /// <param name="pathList">A list of the nodes in the path from the starting node to the ending node</param>
    /// <returns>The total cost of the weights in the path</returns>
   public int DFSPathBetween(string node1name, string node2name, out List<Node> pathList)
{
    pathList = new List<Node>();
    
    // Get nodes by name
    Node node1 = GetNodeByName(node1name);
    Node node2 = GetNodeByName(node2name);
    
    if (node1 == null || node2 == null)
    {
        throw new Exception($"{node1name} or {node2name} does not exist.)");
    }
    
    // Reset all nodes
    foreach (Node node in Nodes)
    {
        node.Color = Color.White;
    }
    
    // Get predecessors using DFS
    Dictionary<Node, Node> pred = DFS(node1);
    
    // Build path if node2 was reached
    if (pred[node2] != null || node1 == node2)
    {
        int totalCost = 0;
        Node current = node2;
        
        // Add end node to path
        pathList.Add(current);
        
        // Traverse back through predecessors
        while (current != node1)
        {
            Node predecessor = pred[current];
            
            // Find edge weight between current and predecessor
            foreach (var neighbor in predecessor.Neighbors)
            {
                if (neighbor.Node == current)
                {
                    totalCost += neighbor.Weight;
                    break;
                }
            }
            
            // Add predecessor to path
            pathList.Add(predecessor);
            current = predecessor;
        }
        
        // Reverse path to get start→end order
        pathList.Reverse();
        return totalCost;
    }
    
    // No path found
    return -1;
}

    private void DFSVisit(Node node, Dictionary<Node, Node> pred)
    {
        // color node gray
        node.Color = Color.Gray;

        // sort the neighbors so that we visit them in alpha order
        node.Neighbors.Sort();

        // visit every neighbor 
        foreach (var neighbor in node.Neighbors)
        {
            if (neighbor.Node.Color == Color.White)
            {
                pred[neighbor.Node] = node;
                DFSVisit(neighbor.Node, pred);
            }
        }

        // color the node black
        node.Color = Color.Black;
    }

    // TODO
    /// <summary>
    /// Searches the graph in a breadth-first manner, creating a
    /// dictionary of the Node to Predecessor and Distance discovered by starting at the given node.
    /// Neighbors are visited in alphabetical order. 
    /// </summary>
    /// <param name="startingNode"></param>
    /// <returns>A dictionary of the Node to Predecessor Node and Distance 
    /// for each node in the graph reachable from the starting node
    /// as discovered by a BFS.</returns>
    public Dictionary<Node, (Node pred, int dist)> BFS(Node startingNode)
    {
        var resultsDictionary = new Dictionary<Node, (Node pred, int dist)>();

        // initialize the dictionary 
        foreach (var node in Nodes)
        {
            node.Color = Color.White;
            resultsDictionary[node] = (null, int.MaxValue);
        }

        // setting up the starting node
        startingNode.Color = Color.Gray;
        resultsDictionary[startingNode] = (null, 0);

        // create a queue
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startingNode);

        // iteratively process the graph (neighbors of the nodes in the queue)
        while (queue.Count > 0)
        {
            // get the front of queue 
            var node = queue.Peek();

            // sort the neighbors so that we visit them in alpha order
            node.Neighbors.Sort();

            foreach (var neighbor in node.Neighbors)
            {
                if (neighbor.Node.Color == Color.White)
                {
                    neighbor.Node.Color = Color.Gray;
                    int distance = resultsDictionary[node].dist;
                    resultsDictionary[neighbor.Node] = (node, distance + 1);
                    queue.Enqueue(neighbor.Node);
                }
            }

            queue.Dequeue();
            node.Color = Color.Black;
        }

        return resultsDictionary;
    }

    /// <summary>
    /// Find the first path between the given nodes in a BFS manner 
    /// and return its total cost. Choices/ties are made in alphabetical order. 
    /// </summary>
    /// <param name="node1name">The starting node's name</param>
    /// <param name="node2name">The ending node's name</param>
    /// <param name="pathList">A list of the nodes in the path from the starting node to the ending node</param>
    /// <returns>The total cost of the weights in the path</returns>
    public int BFSPathBetween(string node1name, string node2name, out List<Node> pathList)
{
    pathList = new List<Node>();
    
    // Get nodes by name
    Node node1 = GetNodeByName(node1name);
    Node node2 = GetNodeByName(node2name);
    
    if (node1 == null || node2 == null)
    {
        throw new Exception($"{node1name} or {node2name} does not exist.)");
    }
    
    // Get BFS results
    var bfsResults = BFS(node1);
    
    // If end node wasn't reached
    if (bfsResults[node2].pred == null && node1 != node2)
    {
        return -1;
    }
    
    // Build path and calculate total cost
    int totalCost = 0;
    Node current = node2;
    
    // Add end node to path
    pathList.Add(current);
    
    // Traverse back through predecessors
    while (current != node1)
    {
        Node predecessor = bfsResults[current].pred;
        
        // Find edge weight between current and predecessor
        foreach (var neighbor in predecessor.Neighbors)
        {
            if (neighbor.Node == current)
            {
                totalCost += neighbor.Weight;
                break;
            }
        }
        
        // Add predecessor to path
        pathList.Add(predecessor);
        current = predecessor;
    }
    
    // Reverse path to get start→end order
    pathList.Reverse();
    return totalCost;
}


    public Dictionary<Node, (Node pred, int cost)> Dijkstra(Node startingNode)
{
    var results = new Dictionary<Node, (Node pred, int cost)>();
    var visited = new HashSet<Node>();
    var priorityQueue = new PriorityQueue<Node, int>();
    
    // Initialize results
    foreach (var node in Nodes)
    {
        results[node] = (null, int.MaxValue);
    }
    
    // Set starting node cost to 0
    results[startingNode] = (null, 0);
    priorityQueue.Enqueue(startingNode, 0);
    
    while (priorityQueue.Count > 0)
    {
        // Get node with smallest cost
        var current = priorityQueue.Dequeue();
        
        // Skip if already visited
        if (visited.Contains(current))
            continue;
            
        // Mark as visited
        visited.Add(current);
        int currentCost = results[current].cost;
        
        // Sort neighbors for consistent results (alphabetical)
        current.Neighbors.Sort();
        
        // Process neighbors
        foreach (var neighbor in current.Neighbors)
        {
            if (visited.Contains(neighbor.Node))
                continue;
                
            // Calculate potential new cost
            int newCost = currentCost + neighbor.Weight;
            
            // Update if new path is shorter
            if (newCost < results[neighbor.Node].cost)
            {
                results[neighbor.Node] = (current, newCost);
                priorityQueue.Enqueue(neighbor.Node, newCost);
            }
        }
    }
    
    return results;
}

    /// <summary>
    /// Find the first path between the given nodes using Dijkstra's algorithm
    /// and return its total cost. Choices/ties are made in alphabetical order. 
    /// </summary>
    /// <param name="node1name">The starting node name</param>
    /// <param name="node2name">The ending node name</param>
    /// <param name="pathList">A list of the nodes in the path from the starting node to the ending node</param>
    /// <returns>The total cost of the weights in the path</returns>
   public int DijkstraPathBetween(string node1name, string node2name, out List<Node> pathList)
{
    pathList = new List<Node>();
    
    // Get nodes by name
    Node node1 = GetNodeByName(node1name);
    Node node2 = GetNodeByName(node2name);
    
    if (node1 == null || node2 == null)
    {
        throw new Exception($"{node1name} or {node2name} does not exist.)");
    }
    
    // Get Dijkstra results
    var results = Dijkstra(node1);
    
    // Check if destination is reachable
    if (results[node2].cost == int.MaxValue)
    {
        return -1;
    }
    
    // Reconstruct path
    Node current = node2;
    
    // Add end node to path
    pathList.Add(current);
    
    // Traverse through predecessors
    while (current != node1)
    {
        Node predecessor = results[current].pred;
        pathList.Add(predecessor);
        current = predecessor;
    }
    
    // Reverse path to get start→end order
    pathList.Reverse();
    
    // Return total cost
    return results[node2].cost;
}

}

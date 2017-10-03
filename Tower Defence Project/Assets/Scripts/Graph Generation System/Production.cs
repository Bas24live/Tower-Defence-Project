using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Production {
    private Graph leftSide, rightSide;
    private List<Graph> candidateGraphs;
    private string label;
    private double xVectorShift, zVectorShift;


    private Node lNodeOrigin, rNodeOrigin, lNodeConnector;

    //------------------------------------------------------------Constructor Methods------------------------------------------------------------//
    public Production(string label) {
        this.label = label;
        candidateGraphs = new List<Graph>();
        leftSide = new Graph();
        rightSide = new Graph();
    }

    public Production(string label, Graph leftSide, Graph rightSide) {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
        this.label = label;
        candidateGraphs = new List<Graph>();
    }

    //---------------------------------------------------------------Mutator Methods---------------------------------------------------------------//
    public bool ApplyToRandom(Graph host) {
        ChooseOrigin();
        SetOrigin();
        SetRelative();
        Random random = new Random();
        int i = random.Next(candidateGraphs.Count);
        return Apply(host, candidateGraphs[i]);
    }

    private bool Apply(Graph host, Graph candidate) {
        List<Node> cNodes = candidate.Nodes;
        List<Edge> cEdges = candidate.Edges;
        List<Edge> cExternalEdges = candidate.GetExternalEdges(cNodes);

        //Find origin and connector then get vectors and adjust right side accourdingly.
        List<Node> corrNodes = findCorrespondingNodes(cNodes, leftSide.Nodes);
        List<Vector3> hostVectors = calculateVectors(corrNodes[0].Position, corrNodes[1].Position);

        //Remove host subgraph internal nodes
        host.RemoveEdges(cEdges);
        for (int i = 0; i < cNodes.Count; ++i) {
            Node lNode = leftSide.Nodes[i];
            Node cNode = cNodes[i];
            if (rightSide.DoesNodeExist(lNode)) {
                host.Replace(cNode, rightSide.GetNode(lNode), hostVectors);
            } else {
                host.RemoveNode(cNode);
                host.CleanEdges(cNode);
            }
        }

        foreach (Node rNode in rightSide.Nodes) {
            if (!leftSide.DoesNodeExist(rNode)) {
                host.AddNode(rNode, hostVectors);
            }
        }

        //Add new internal nodes from production
        host.AddEdges(rightSide.Edges);

        return true;
    }    

    //Shifts the nodes of the left and right side of the production relative to their origin
    private void SetOrigin() {
        TranslateNodes(leftSide.Nodes, lNodeOrigin.position);
        TranslateNodes(rightSide.Nodes, rNodeOrigin.position);
    }

    //Convert the points in the right hand side to be ratios of the x and y vectors of the left hand side
    private void SetRelative() {
        calculateVectorShift();

        foreach(Node node in rightSide.Nodes) {
            if (xVectorShift == 0) {
                node.position.X = 0;
            } else {
                node.position.X /= (float)xVectorShift  ;
            }

            if (zVectorShift == 0) {
                node.position.Z = 0;
            }
            else {
                node.position.Z /= (float)zVectorShift;
            }
        }
    }

    //----------------------------------------------------------------Helper Methods---------------------------------------------------------------//
    //Finds a node that occurs in both sides of the production to use as an origin point
    private bool ChooseOrigin() {
        foreach (Node lNode in leftSide.Nodes) {
            foreach (Node rNode in rightSide.Nodes) {
                if (lNode.CompareExact(rNode)) {
                    lNodeOrigin = lNode;
                    rNodeOrigin = rNode;
                    return true;
                }
            }
        }
        return false;
    }

    //Shift the given nodes by the amount given towards the origin, thus subtract by amount
    private void TranslateNodes(List<Node> nodes, Vector3 amount) {
        foreach (Node node in nodes) {
            node.Position.X -= amount.X;
            node.Position.Y -= amount.Y;
            node.Position.Z -= amount.Z;
        }
    }

    /* Calculate the x and z vectors for the leftSide graph
     * There must be two ndoes for this to work
     */
    private void calculateVectorShift() {
        List<Edge> connectedEdges = leftSide.GetConnectedEdges(lNodeOrigin);
        
        Edge edge = connectedEdges[0];
        if (!edge.Source.CompareExact(lNodeOrigin))
            lNodeConnector = edge.Source;
        else
            lNodeConnector = edge.Target;

        List<Vector3> vectors = calculateVectors(lNodeOrigin.Position, lNodeConnector.Position);
        Vector3 uVector = vectors[0];
        Vector3 vVector = vectors[1];

        xVectorShift = Math.Sqrt(uVector.X * uVector.X);
        zVectorShift = Math.Sqrt(vVector.Z * vVector.Z);
    }

    private List<Node> findCorrespondingNodes(List<Node> hNodes, List<Node> lNodes) {
        List<Node> nodes = new List<Node>();
        Node origin = null, connector = null;

        for (int i = 0; i < lNodes.Count; ++i) {
            if (lNodes[i].CompareExact(lNodeOrigin)) {
                origin = hNodes[i];                
            }
            else if (lNodes[i].CompareExact(lNodeConnector)) {
                connector = hNodes[i];
            }
        }

        nodes.Add(origin);
        nodes.Add(connector);

        return nodes;
    }

    private List<Vector3> calculateVectors(Vector3 position1, Vector3 position2) {
        Vector3 u = new Vector3(position2.X - position1.X, 0, position1.Z - position2.Z);
        Vector3 z;

        if (u.X == 0) {
            if (u.Z > 0) 
                z = new Vector3(1, 0, 0);
            else 
                z = new Vector3(-1, 0, 0);
        } else if (u.Z == 0) {
            if (u.X > 0)
                z = new Vector3(0, 0, 1);
            else 
                z = new Vector3(0, 0, -1);
        } else {
            z = new Vector3(1, 0, -u.X / u.Z);
        }

        List<Vector3> vectors = new List<Vector3>();
        vectors.Add(u);
        vectors.Add(z);

        return vectors;        
    }

    //--------------------------------------------------------------Accessors Methods--------------------------------------------------------------//
    public Graph LeftSide {
        get {
            return leftSide;
        }
    }

    public Graph RightSide {
        get {
            return rightSide;
        }
    }

    public string Label {
        get {
            return label;
        }
    }

    public List<Graph> CandidateGraphs {
        get {
            return candidateGraphs;
        }

        set {
            candidateGraphs = value;
        }
    }

    //------------------------------------------------------------Serialization Methods------------------------------------------------------------//
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue("label", label);
        info.AddValue("left side", leftSide);
        info.AddValue("right side", rightSide);
        info.AddValue("candidate graphs", candidateGraphs);
    }

    public Production(SerializationInfo info, StreamingContext context) {
        label = (string)info.GetValue("label", typeof(string));
        leftSide = (Graph)info.GetValue("left side", typeof(Graph));
        rightSide = (Graph)info.GetValue("right side", typeof(Graph));
        candidateGraphs = (List<Graph>)info.GetValue("candidate graphs", typeof(List<Graph>));
    }
}
    
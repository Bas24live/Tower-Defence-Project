using System.Collections.Generic;

public class GGS {

    Graph graph;
    List<SingleProduciton> singleProductions;

    public GGS() {
        Init();
        GenProducitons();
    }

    private void Init() {
        graph = new Graph();
        singleProductions = new List<SingleProduciton>();
    }

    //----------------Temp Producitons-----------------//
    private void GenProducitons() {
        Graph lhs = new Graph();
        Node start = new Node("Start", "S", new Position(0, 0, 0));
        lhs.AddNode(start);

        Graph rhs = new Graph();
        Node A = new Node("A", "S", new Position(1, 0, 0));
        Node B = new Node("B", "S", new Position(1, 0, 1));
        Node C = new Node("C", "S", new Position(0, 0, 1));

        Edge AB = new Edge(A, B);
        Edge BC = new Edge(B, C);
        Edge CA = new Edge(C, A);

        rhs.AddNode(A);
        rhs.AddNode(B);
        rhs.AddNode(C);

        rhs.AddEdge(AB);
        rhs.AddEdge(BC);
        rhs.AddEdge(CA);

        SingleProduciton temp1 = new SingleProduciton(lhs, rhs, "Triangle");
        singleProductions.Add(temp1);


    }

}

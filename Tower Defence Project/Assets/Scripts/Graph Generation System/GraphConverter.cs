using System.Collections.Generic;
using UnityEngine;

public class GraphConverter : MonoBehaviour {

    public Transform nodePrefab;
    private Graph host;

    void Start () {
        GGS ggs = GGS.GetInstance();
        ggs.Run();
        this.host = ggs.Host;
        GenNodes();
        GenEdges();
	}

    private void GenNodes() {
        List<Node> nodes = host.Nodes;

        string containerName = "Nodes";
        Transform nodesContainer = new GameObject(containerName).transform;
        nodesContainer.parent = transform;

        foreach (Node node in nodes) {
            UnityEngine.Vector3 nodePosition = new UnityEngine.Vector3(node.Position.X, node.Position.Y, node.Position.Z);

            Transform uNode = Instantiate(nodePrefab, nodePosition, Quaternion.Euler(UnityEngine.Vector3.zero)) as Transform;
            uNode.parent = nodesContainer;
            uNode.name = node.label;
        }
    }

    void GenEdges() {
        List<Edge> edges = host.Edges;

        string containerName = "Edges";
        Transform edgesContainer = new GameObject(containerName).transform;
        edgesContainer.parent = transform;

        foreach (Edge edge in edges) {
            Node source = edge.Source;
            Node target = edge.Target;

            UnityEngine.Vector3 sourcePosition = new UnityEngine.Vector3(source.Position.X, source.Position.Y, source.Position.Z);
            UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(target.Position.X, target.Position.Y, target.Position.Z);

            DrawLine(sourcePosition, targetPosition, Color.red, edgesContainer, edge.Id);
        }
    }

    void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, Color color, Transform container, string name, float duration = 0.2f) {
        GameObject myLine = new GameObject();
        myLine.transform.parent = container;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        myLine.name = name;
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }


}

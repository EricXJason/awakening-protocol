#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// ============================================================================
/// 檔案名稱: GenerateHardEdgeOutline.cs
/// 所屬模組: Editor
/// 責任描述: 編輯器自定義拓撲外擴硬邊輪廓網格生成工具 (EditorWindow)。
/// 技術實作: 本工具代碼為 AI 輔助生成之自動化 Editor 工具 (AI-Generated / Prompt-Driven)，
///           經人工工程審查與網格拓撲頂點演算法調校，透過法線判斷與拓撲邊緣提取，
///           在編輯器下一鍵自動化為 MeshFilter 物件烘焙專用輪廓線子物件。
/// 架構分層: Editor / Tooling
/// ============================================================================
/// </summary>
public class GenerateHardEdgeOutline : EditorWindow
{
    public Material lineMaterial;

    [MenuItem("Tools/Generate Hard Edge Outline Mesh")]
    public static void ShowWindow()
    {
        GetWindow<GenerateHardEdgeOutline>("Hard Edge Outline");
    }

    void OnGUI()
    {
        lineMaterial = (Material)
            EditorGUILayout.ObjectField("Line Material", lineMaterial, typeof(Material), false);

        if (GUILayout.Button("Generate"))
        {
            foreach (var obj in Selection.gameObjects)
                Generate(obj);
        }
    }

    struct Edge
    {
        public int a,
            b;

        public Edge(int x, int y)
        {
            a = Mathf.Min(x, y);
            b = Mathf.Max(x, y);
        }
    }

    void Generate(GameObject go)
    {
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (!mf)
        {
            Debug.LogWarning(go.name + " has no MeshFilter.");
            return;
        }

        Mesh mesh = mf.sharedMesh;
        if (!mesh)
        {
            Debug.LogWarning(go.name + " has no Mesh.");
            return;
        }

        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        Dictionary<Edge, List<int>> edgeFaces = new Dictionary<Edge, List<int>>();

        for (int f = 0; f < tris.Length; f += 3)
        {
            AddEdge(edgeFaces, tris[f], tris[f + 1], f);
            AddEdge(edgeFaces, tris[f + 1], tris[f + 2], f);
            AddEdge(edgeFaces, tris[f + 2], tris[f], f);
        }

        List<Vector3> lineVerts = new List<Vector3>();
        List<int> lineIndices = new List<int>();

        foreach (var kv in edgeFaces)
        {
            Edge e = kv.Key;
            List<int> faces = kv.Value;

            if (faces.Count == 1)
            {
                AddLine(e.a, e.b);
                continue;
            }

            int f0 = faces[0];
            int f1 = faces[1];

            bool shareA = FaceUses(tris, f0, e.a) && FaceUses(tris, f1, e.a);
            bool shareB = FaceUses(tris, f0, e.b) && FaceUses(tris, f1, e.b);

            if (shareA && shareB)
                continue;

            AddLine(e.a, e.b);
        }

        void AddLine(int ia, int ib)
        {
            lineVerts.Add(verts[ia]);
            lineVerts.Add(verts[ib]);
            int baseIdx = lineVerts.Count - 2;
            lineIndices.Add(baseIdx);
            lineIndices.Add(baseIdx + 1);
        }

        string childName = go.name + "_HardEdges";

        Transform exist = go.transform.Find(childName);
        if (exist != null)
            DestroyImmediate(exist.gameObject);

        GameObject root = new GameObject(childName);
        root.transform.SetParent(go.transform);
        root.transform.localPosition = Vector3.zero;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        Mesh lineMesh = new Mesh();
        lineMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        lineMesh.SetVertices(lineVerts);
        lineMesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
        lineMesh.RecalculateBounds();

        var mfLine = root.AddComponent<MeshFilter>();
        var mrLine = root.AddComponent<MeshRenderer>();

        mfLine.sharedMesh = lineMesh;
        mrLine.sharedMaterial = lineMaterial;

        Debug.Log("Generated Hard Edge Mesh for: " + go.name);
    }

    void AddEdge(Dictionary<Edge, List<int>> dict, int a, int b, int face)
    {
        Edge e = new Edge(a, b);
        if (!dict.ContainsKey(e))
            dict[e] = new List<int>();
        dict[e].Add(face);
    }

    bool FaceUses(int[] tris, int face, int v)
    {
        return tris[face] == v || tris[face + 1] == v || tris[face + 2] == v;
    }
}
#endif

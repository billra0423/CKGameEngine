using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float Intensity = 1f;
    public float Mass = 1f;
    public float stiffness = 1f;
    public float damping = 0.75f;
    private Mesh OriginalMesh, MeshClone;
    private MeshRenderer _renderer;
    private JellyVertex[] jv;
    private Vector3[] vertexArray;
    private MeshCollider meshCollider;

    private void Start()
    {
        OriginalMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshClone = Instantiate(OriginalMesh);
        GetComponent<MeshFilter>().sharedMesh = MeshClone;
        _renderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        jv = new JellyVertex[MeshClone.vertices.Length];
        for (int i = 0; i < MeshClone.vertices.Length; i++)
            jv[i] = new JellyVertex(i, transform.TransformPoint(MeshClone.vertices[i]));

    }

    private void FixedUpdate()
    {
        vertexArray = OriginalMesh.vertices;
        for (int i = 0; i < jv.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jv[i].ID]);
            float intensity = (1 - (_renderer.bounds.max.y - target.y) / _renderer.bounds.size.y) * Intensity;
            jv[i].Shake(target, Mass, stiffness, damping);
            target = transform.InverseTransformPoint(jv[i].Position);
            vertexArray[jv[i].ID] = Vector3.Lerp(vertexArray[jv[i].ID], target, intensity);

        }
        MeshClone.vertices = vertexArray;
        MeshClone.RecalculateNormals();
       // MeshClone.RecalculateBounds();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null; // 먼저 비워줘야 함
            meshCollider.sharedMesh = MeshClone; // 업데이트된 Mesh 적용
        }
    }
    public class JellyVertex
    {
        public int ID;
        public Vector3 Position;
        public Vector3 velocity, Force;

        public JellyVertex(int _id, Vector3 _pos)
        {
            ID = _id;
            Position = _pos;
        }
        public void Shake(Vector3 target, float m, float s, float d)
        {
            Force = (target - Position) * s;
            velocity = (velocity + Force / m) * d;
            Position += velocity;
            if ((velocity + Force + Force / m).magnitude < 0.001f)
                Position = target;
        }
    }

}

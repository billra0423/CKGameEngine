using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VertexBone
{
    public Vector3 position;

    public int originVertex = 0;

    public List<int> verticies;
    public List<float> weights;

    public VertexBone()
    {
        verticies = new List<int>();
        weights = new List<float>();
    }

}

public class SoftBody : MonoBehaviour
{
    public float radius = 0.001f;
    public float springFactor = 0.001f;
    public int softbodyLayer = 30;
    public float physicsVertexRadius = 0.01f;


    public PhysicsMaterial colliderMaterial;

    private Mesh mesh;
    private Rigidbody rigid;
    private Transform Offset;

    public List<Transform> physicsVerticiesOnVertex;
    public List<VertexBone> vertexBones;

    private Vector3[] vertices;
    private Vector3[] startVertices;
    private Vector2[] uv;
    private int[] triangles;


    [Header("PD/Position")]
    public float PositionProportional = 100f;
    public float PositionDerivative = 10f;


    private MeshFilter meshFilter;
    public List<Transform> colliderActiveStates = new List<Transform>();
    private Dictionary<int, int> vertexDictunery;
    private List<Vector2Int> uniqueSprings = new List<Vector2Int>();
    private List<DistanceConstraint> distanceConstraints = new List<DistanceConstraint>();
    public bool isJoint;
    private float SProprtional = 100;
    private float SDerivative = 8;

    private void Start()
    {
        Physics.IgnoreLayerCollision(softbodyLayer, softbodyLayer, true);
        Physics.IgnoreLayerCollision(softbodyLayer, 8, true);
        Offset = new GameObject("SoftbodyPhysics").transform;
        Offset.parent = transform;
        Offset.position = transform.position;
        Offset.rotation = Quaternion.Euler(0, 0, 0);

        rigid = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        CreateMeshdata();

        GenerateBones();
        GenerateWeights();

        RemoveDuplicateVertices(vertices.ToList());

        CreateAndAddPhysicsVertices();

        var tempListOfSprings = ExtractLinesFromMesh(triangles);

        uniqueSprings = RemoveDuplicateSprings(tempListOfSprings);

        if (isJoint)
        {
            ConnectSprings(uniqueSprings);
        }
        else
        {
            PositionProportional = SProprtional;
            PositionDerivative = SDerivative;
            InitializeDistanceConstraints(uniqueSprings);
        }
    }
    private void GenerateBones()
    {
        int i = 0;
        foreach (Vector3 vertex in vertices)
        {
            Vector3 WorldSpaceVetexPosition = transform.TransformPoint(vertex);

            if (vertexBones.Count == 0)
            {
                VertexBone vertexBone = new VertexBone();

                vertexBone.position = WorldSpaceVetexPosition;
                vertexBones.Add(vertexBone);
            }
            else
            {
                bool notTaken = true;
                foreach (VertexBone vertexBone in vertexBones)
                {
                    if ((vertexBone.position - WorldSpaceVetexPosition).magnitude < radius)
                    {
                        notTaken = false;
                    }
                }

                if (notTaken)
                {
                    VertexBone vertexBone = new VertexBone();

                    vertexBone.originVertex = i;
                    vertexBone.position = WorldSpaceVetexPosition;
                    vertexBones.Add(vertexBone);
                }
            }
            i++;
        }
    }
    private void GenerateWeights()
    {
        if (vertexBones.Count > 0)
        {
            for (int e = 0; e < vertexBones.Count; e++)
            {
                for (int i = 0; i < startVertices.Length; i++)
                {
                    Vector3 WorldSpaceVetexPosition = transform.TransformPoint(startVertices[i]);

                    if ((WorldSpaceVetexPosition - vertexBones[e].position).magnitude < radius * 1.05f)
                    {
                        if ((WorldSpaceVetexPosition - vertexBones[e].position).magnitude != 0)
                        {
                            vertexBones[e].verticies.Add(i);
                            vertexBones[e].weights.Add((-(WorldSpaceVetexPosition - vertexBones[e].position).magnitude + radius * 1.05f) / radius * 1.05f);
                        }
                        else
                        {
                            vertexBones[e].verticies.Add(i);
                            vertexBones[e].weights.Add(1);
                        }
                    }
                }
            }
        }
    }
    private void CreateMeshdata()
    {
        vertices = GetComponent<MeshFilter>().mesh.vertices;
        triangles = GetComponent<MeshFilter>().mesh.triangles;
        uv = GetComponent<MeshFilter>().mesh.uv;

        startVertices = GetComponent<MeshFilter>().mesh.vertices;

        mesh = new Mesh();

        meshFilter.mesh = mesh;
    }
    private void InitializeDistanceConstraints(List<Vector2Int> springs)
    {
        foreach (var spring in springs)
        {
            var particle1 = physicsVerticiesOnVertex[spring.x];
            var particle2 = physicsVerticiesOnVertex[spring.y];

            float distanceBetween = Vector3.Distance(
                particle1.position,
                particle2.position);

         
            distanceConstraints.Add(new DistanceConstraint(particle1, particle2, distanceBetween, 0.2f));
        }
    }
    private void CreateAndAddPhysicsVertices()
    {
        foreach (VertexBone vertexBone in vertexBones)
        {
            CreatePhysicsVertex(vertexBone.position);
        }
    }
    private void CreatePhysicsVertex(Vector3 position)
    {
        GameObject phyVertex = new GameObject("PhysicsVertex");
        phyVertex.transform.position = position;
        phyVertex.transform.rotation = Quaternion.Euler(0, 0, 0);
        phyVertex.transform.parent = Offset;

        Rigidbody rigidBody = phyVertex.AddComponent<Rigidbody>();
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        GameObject target = new GameObject("Traget");

        target.transform.position = position;
        target.transform.rotation = Quaternion.Euler(0, 0, 0);
        target.transform.parent = Offset;

        phyVertex.AddComponent<VertexTracker>();
        phyVertex.GetComponent<VertexTracker>().TransTarget = target.transform;
        phyVertex.GetComponent<VertexTracker>().PositionProportional = PositionProportional;
        phyVertex.GetComponent<VertexTracker>().PositionDerivative = PositionDerivative;
        phyVertex.GetComponent<VertexTracker>().maxDepenetrationVelocity = 20;

        SphereCollider sphereCollider = phyVertex.AddComponent<SphereCollider>();
        sphereCollider.radius = physicsVertexRadius;
        phyVertex.layer = softbodyLayer;

        if (colliderMaterial)
            sphereCollider.material = colliderMaterial;

        physicsVerticiesOnVertex.Add(phyVertex.transform);


    }
    private void RemoveDuplicateVertices(List<Vector3> vertices)
    {
        var optimizedVertices = new List<Vector3>();
        vertexDictunery = new Dictionary<int, int>();

        for (int i = 0; i < vertices.Count; i++)
        {
            bool isVertexDuplicated = false;
            for (int j = 0; j < optimizedVertices.Count; j++)
            {
                if (optimizedVertices[j] == vertices[i])
                {
                    isVertexDuplicated = true;
                    vertexDictunery.Add(i, j);
                    break;
                }
            }
            if (!isVertexDuplicated)
            {
                optimizedVertices.Add(vertices[i]);
                vertexDictunery.Add(i, optimizedVertices.Count - 1);
            }
        }
    }

    private List<Vector2Int> ExtractLinesFromMesh(int[] triangles)
    {
        var tempListOfSprings = new List<Vector2Int>();
        bool isFirstTrisOfQuad = true;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index0 = vertexDictunery[triangles[i]];
            int index1 = vertexDictunery[triangles[i + 1]];
            int index2 = vertexDictunery[triangles[i + 2]];

            tempListOfSprings.Add(new Vector2Int(index1, index2));

            if (isFirstTrisOfQuad)
            {
                tempListOfSprings.Add(new Vector2Int(index0, index1));
                isFirstTrisOfQuad = false;
            }
            else
            {
                tempListOfSprings.Add(new Vector2Int(index2, index0));
                isFirstTrisOfQuad = true;
            }
        }

        return tempListOfSprings;
    }

    private List<Vector2Int> RemoveDuplicateSprings(List<Vector2Int> springs)
    {
        var noDupesListOfSprings = new List<Vector2Int>();

        foreach (var spring in springs)
        {
            bool isDuplicated = false;
            Vector2Int reversed = new Vector2Int(spring.y, spring.x);

            foreach (var uniqueSpring in noDupesListOfSprings)
            {
                if (spring == uniqueSpring || reversed == uniqueSpring)
                {
                    isDuplicated = true;
                    break;
                }
            }

            if (!isDuplicated)
                noDupesListOfSprings.Add(spring);
        }
        return noDupesListOfSprings;
    }

    private void ConnectSprings(List<Vector2Int> springs)
    {
        foreach (var spring in springs)
        {
            var thisGameObject = physicsVerticiesOnVertex[spring.x].gameObject;
            var destinationBody = physicsVerticiesOnVertex[spring.y].GetComponent<Rigidbody>();

            var joint = thisGameObject.AddComponent<CharacterJoint>();
            joint.connectedBody = destinationBody;

            float distanceBetween = Vector3.Distance(thisGameObject.transform.position, destinationBody.transform.position);

            SoftJointLimit highLimit = new SoftJointLimit
            {
                bounciness = 1.1f,
                contactDistance = distanceBetween,
                limit = 10
            };

            SoftJointLimit lowLimit = new SoftJointLimit
            {
                bounciness = 1.1f,
                contactDistance = distanceBetween,
                limit = -10
            };

            joint.highTwistLimit = highLimit;
            joint.lowTwistLimit = lowLimit;
            joint.swing1Limit = lowLimit;
            joint.swing2Limit = highLimit;

            SoftJointLimitSpring springSettings = new SoftJointLimitSpring
            {
                damper = 1000,
                spring = 1
            };

            joint.swingLimitSpring = springSettings;
            joint.twistLimitSpring = springSettings;
        }
    }

    private void UpdateMesh()
    {
        springFactor = (1 / (float)(physicsVerticiesOnVertex.Count));

        int e = 0;
        if (vertexBones.Count > 0)
        {
            foreach (VertexBone vertexbone in vertexBones)
            {
                int f = 0;

                if (vertexbone.verticies.Count > 0)
                {
                    foreach (int i in vertexbone.verticies)
                    {
                        Vector3 localPhysicsVertexPos = transform.InverseTransformPoint(physicsVerticiesOnVertex[e].position);
                        Vector3 localBonePos = transform.InverseTransformPoint(vertexbone.position);

                        if (vertexbone.weights.Count > 0)
                            vertices[i] = startVertices[i] + localPhysicsVertexPos - startVertices[vertexbone.originVertex];

                        f++;


                    }
                    e++;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

    }

    private void Update()
    {
        if (PlayerSkill.Instance.isSlime)
        {
            UpdateMesh();
            if (physicsVerticiesOnVertex.Count > 0)
            {
                foreach (Transform vt in physicsVerticiesOnVertex)
                {
                    vt.GetComponent<VertexTracker>().PositionProportional = PositionProportional;
                    vt.GetComponent<VertexTracker>().PositionDerivative = PositionDerivative;
                }
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                ResetPhysicsVerticesToStartPositions(true);
            }
            rigid.isKinematic = false;
        }


    }
    public void ResetPhysicsVerticesToStartPositions(bool isRe)
    {
        rigid.isKinematic = !isRe;
        rigid.useGravity = isRe;
        for (int i = 0; i < physicsVerticiesOnVertex.Count; i++)
        {
            Rigidbody rb = physicsVerticiesOnVertex[i].GetComponent<Rigidbody>();
            SphereCollider sc = physicsVerticiesOnVertex[i].GetComponent<SphereCollider>();
            if (sc != null)
            {
                sc.enabled = isRe;
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = !isRe;
            }
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < vertexBones.Count; i++)
        {
            rigid.AddForceAtPosition(-physicsVerticiesOnVertex[i].GetComponent<VertexTracker>().Force * springFactor * rigid.mass, physicsVerticiesOnVertex[i].position);
        }
        if (!isJoint)
        {

            foreach (var constraint in distanceConstraints)
            {
                constraint.Solve();
            }

        }
    }

}
[System.Serializable]
public class DistanceConstraint
{
    public Transform particle1;
    public Transform particle2;
    public float restLength;
    public float stiffness;

    public DistanceConstraint(Transform p1, Transform p2, float restLength, float stiffness)
    {
        this.particle1 = p1;
        this.particle2 = p2;
        this.restLength = restLength;
        this.stiffness = stiffness;
    }

    public void Solve()
    {
        Vector3 delta = particle2.position - particle1.position;
        float currentDistance = delta.magnitude;
        float correction = (currentDistance - restLength) * stiffness;

        if (correction != 0)
        {
            Vector3 correctionVector = delta.normalized * correction;

            particle1.position += correctionVector * 0.5f;
            particle2.position -= correctionVector * 0.5f;
        }
    }
}

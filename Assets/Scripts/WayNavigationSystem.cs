using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WayNavigationSystem : MonoBehaviour {
    public static WayNavigationSystem Instance { get; private set; }
    
    [SerializeField] private Transform m_wayTargetTransform = null;

    private MeshRenderer  m_meshRenderer = null;

    private int           m_currentTargetPoint  = 0;
    private List<Vector3> m_wayPointList        = new List<Vector3>();
    private Mesh          m_mesh                = null;
    private Vector3[]     m_wayPointsPosArr     = null;
    private int[]         m_triangles           = null;
    private Vector2[]     m_uvs                 = null;
    private Vector2       m_uvOffset            = Vector3.zero;
    private Vector3       m_targetPos           = Vector3.zero;

    private GameObject  m_currentWayPointObj  = null;
    private ThreadStart m_vertexsThreadFunc   = null;
    private ThreadStart m_trianglesThreadFunc = null;
    private ThreadStart m_uvsThreadFunc       = null;
    private Thread m_vertexsThread   = null;
    private Thread m_trianglesThread = null;
    private Thread m_uvsThread       = null;

    public bool ShowWayNavigation
    {
        get
        {
            return  m_meshRenderer.enabled;
        }
        set
        {
            m_meshRenderer.enabled = value;

            if (value)
                StartCoroutine("WayNavigationUpdate");
            else
                StopCoroutine("WayNavigationUpdate");
        }
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogWarning("WayPointSystem is Exist");
        }
        else
        {
            Instance       = this;
            m_meshRenderer = GetComponent<MeshRenderer>();
            m_meshRenderer.enabled = false;
            m_mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = m_mesh;

            m_vertexsThreadFunc   = new ThreadStart(CreateWayMeshVertexs);
            m_trianglesThreadFunc = new ThreadStart(CreateWayMeshTriangles);
            m_uvsThreadFunc       = new ThreadStart(CreateWayMeshUVs);
        }
    }

    void Start()
    {
        NextWayPointRoad();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ShowWayNavigation = !ShowWayNavigation;

        if (m_meshRenderer.enabled)
        {
            m_uvOffset += (Vector2.up * Time.smoothDeltaTime);
            GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", -m_uvOffset);
        }
    }

    public void NextWayPointRoad()
    {
        StopCoroutine("WayNavigationUpdate");
        AllThreadStop();

        if (m_currentWayPointObj)
            Destroy(m_currentWayPointObj);

        m_currentWayPointObj = Instantiate(Resources.Load<GameObject>(string.Concat("WayPoint/", ++m_currentTargetPoint, " WayPoints"))) as GameObject;
        m_currentWayPointObj.transform.SetParent(transform);

        Transform[] wayPointTransformArr = m_currentWayPointObj.GetComponentsInChildren<Transform>();
        m_wayPointsPosArr = new Vector3[wayPointTransformArr.Length];
        for (int i = 1; i < m_wayPointsPosArr.Length; i++)
            m_wayPointsPosArr[i] = wayPointTransformArr[i].position;

        ScrollMinimapCtrl.Instance.TargetPoint = m_wayPointsPosArr[m_wayPointsPosArr.Length - 1];
    }

    private void CreateWayMeshVertexs()
    {
        lock (this)
        {
            m_wayPointList.Clear();
            
            int nearbyIndex = 1;
            float minDist = (m_wayPointsPosArr[nearbyIndex] - m_targetPos).sqrMagnitude;
            for (int i = 2; i < m_wayPointsPosArr.Length; i++)
            {
                float dist = (m_wayPointsPosArr[i] - m_targetPos).sqrMagnitude;
                if (dist < minDist)
                {
                    nearbyIndex = i;
                    minDist = dist;
                }
            }

            List<Vector3> leftPosList = new List<Vector3>();
            List<Vector3> rightPosList = new List<Vector3>();

            m_wayPointsPosArr[0] = m_targetPos;
            Vector3 prevWayPoint = m_wayPointsPosArr[0];
            Vector3 nextWayPoint = m_wayPointsPosArr[nearbyIndex];
            Vector3 dir = (nextWayPoint - prevWayPoint).normalized;
            Vector3 leftPos = prevWayPoint + new Vector3(-dir.z * 0.1f, 0f, dir.x * 0.1f);
            Vector3 rightPos = prevWayPoint + new Vector3(dir.z * 0.1f, 0f, -dir.x * 0.1f);
            leftPosList.Add(leftPos);
            rightPosList.Add(rightPos);
            leftPosList.Add(leftPos);
            rightPosList.Add(rightPos);

            for (int i = nearbyIndex; i < m_wayPointsPosArr.Length - 1; i++)
            {
                prevWayPoint = m_wayPointsPosArr[i];
                nextWayPoint = m_wayPointsPosArr[i + 1];
                dir = (nextWayPoint - prevWayPoint).normalized;
                leftPos = prevWayPoint + new Vector3(-dir.z * 0.1f, 0f, dir.x * 0.1f);
                rightPos = prevWayPoint + new Vector3(dir.z * 0.1f, 0f, -dir.x * 0.1f);
                leftPosList.Add(leftPos);
                rightPosList.Add(rightPos);

                if (i == (m_wayPointsPosArr.Length - 2))
                {
                    Vector3 lastPos = m_wayPointsPosArr[i + 1];
                    Vector3 lastLeftPos = lastPos + new Vector3(-dir.z * 0.1f, 0f, dir.x * 0.1f);
                    Vector3 rightLeftPos = lastPos + new Vector3(dir.z * 0.1f, 0f, -dir.x * 0.1f);

                    leftPosList.Add(lastLeftPos);
                    rightPosList.Add(rightLeftPos);
                    leftPosList.Add(lastLeftPos);
                    rightPosList.Add(rightLeftPos);
                }
            }

            leftPosList = CatmullRomSpline.CreateCatmullRomList(leftPosList, 0.1f);
            rightPosList = CatmullRomSpline.CreateCatmullRomList(rightPosList, 0.1f);
            for (int i = 0; i < leftPosList.Count - 1; i++)
            {
                m_wayPointList.Add(leftPosList[i]);
                m_wayPointList.Add(rightPosList[i]);
                m_wayPointList.Add(leftPosList[i + 1]);
                m_wayPointList.Add(rightPosList[i + 1]);
            }

            m_trianglesThread = new Thread(m_trianglesThreadFunc);
            m_uvsThread       = new Thread(m_uvsThreadFunc);
            m_trianglesThread.Start();
            m_uvsThread.Start();
        }
    }

    private void CreateWayMeshTriangles()
    {
        lock (this)
        {
            if (m_wayPointList.Count > 2)
                m_triangles = new int[(m_wayPointList.Count - 2) * 3];
            else
                m_triangles = new int[m_wayPointList.Count * 3];

            for (int i = 0; i < m_triangles.Length / 3; i += 2)
            {
                m_triangles[i * 3] = i + 2;
                m_triangles[i * 3 + 1] = i + 1;
                m_triangles[i * 3 + 2] = i;
                m_triangles[i * 3 + 3] = i + 3;
                m_triangles[i * 3 + 4] = i + 1;
                m_triangles[i * 3 + 5] = i + 2;
            }
        }
    }

    private void CreateWayMeshUVs()
    {
        lock (this)
        {
            m_uvs = new Vector2[m_wayPointList.Count];
            for (int i = 0; i < m_uvs.Length; i += 4)
            {
                m_uvs[i] = new Vector2(0f, 0f);
                m_uvs[i + 1] = new Vector2(1f, 0f);
                m_uvs[i + 2] = new Vector2(0f, 1f);
                m_uvs[i + 3] = new Vector2(1f, 1f);
            }
        }
    }

    private void CreateWayMesh()
    {
        m_mesh.Clear();
        m_mesh.vertices = m_wayPointList.ToArray();
        m_mesh.triangles = m_triangles;
        m_mesh.uv = m_uvs;
        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
    }

    private void AllThreadStop()
    {
        if (m_vertexsThread != null)
            m_vertexsThread.Abort();

        if (m_uvsThread != null)
            m_uvsThread.Abort();

        if (m_trianglesThread != null)
            m_trianglesThread.Abort();
    }

    private IEnumerator WayNavigationUpdate()
    {
        while (true)
        {
            m_targetPos = m_wayTargetTransform.position;
            m_vertexsThread = new Thread(m_vertexsThreadFunc);
            m_vertexsThread.Start();

            while (!m_vertexsThread.IsAlive && !m_trianglesThread.IsAlive && !m_uvsThread.IsAlive)
                yield return null;

            yield return null;

            CreateWayMesh();
        }
    }
}

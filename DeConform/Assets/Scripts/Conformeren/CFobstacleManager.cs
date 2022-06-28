using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFobstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject player;
    private CFcolorUpdater playerColorManager;
    public List<GameObject> obstacles = new();
    private List<Rigidbody2D> rigidBodyAnchor = new();
    private List<Rigidbody2D> rigidBodyCollider = new();
    //private List<Vector2> obstacleRandomOffsets = new();

    [Header("Obstacle Spawn Settings")]
    public float scale;
    public Vector2 randomDistanceRange;
    public int gridSize; //Needs to be uneven
    [SerializeField] private float rangePadding;
    public float extraSpaceBetweenObstacles;
    [SerializeField] private Transform obstacleWorldParent;
    private bool isSetUp = false;

    [Header("Obstacle Physics Options")]
    [SerializeField] private float springJointDistance;
    [SerializeField] private float springJointDampingRatio;
    [SerializeField] private float springJointFrequency;
    [SerializeField] private float springJointBreakForce;
    private float rangeNoBuffer;

    [Header("Obstacle Color Behviour")]
    [SerializeField] private Camera renderTexCam;
    [SerializeField] private RenderTexture renderTex;
    [HideInInspector] public Texture2D tex;
    [Space(10)]
    public float colliderSizeMin;
    public float colliderSizeMiddle;
    public float colliderSizeMax;
    public AnimationCurve colliderSizeCurve;
    public float frequencyMin;
    public float frequencyMiddle;
    public float frequencyMax;
    public AnimationCurve frequencyCurve;
    [Range(0,1)] public float tippingPoint;
    public float playerValueProgress;


    public void StartUp()
    {
        playerColorManager = player.GetComponent<CFcolorUpdater>();

        tex = new Texture2D(64, 64, TextureFormat.RGB24, false);

        if (gridSize % 2 == 0) { gridSize++; } //Makes sure gridsize is uneven

        //Spawn obstacles
        Vector2 spawnPosOG = new Vector2(-(scale + extraSpaceBetweenObstacles) * Mathf.Floor((float)gridSize / 2f), -(scale + extraSpaceBetweenObstacles) * Mathf.Floor((float)gridSize / 2f));
		for (int x = 0; x < gridSize; x++)
		{
            for (int y = 0; y < gridSize; y++)
			{
                Vector2 spawnPos = new Vector2(spawnPosOG.x + (scale + extraSpaceBetweenObstacles) * x, spawnPosOG.y + (scale + extraSpaceBetweenObstacles) * y);

                Vector3 SpawnLocation = new Vector3(spawnPos.x, spawnPos.y, 0);
                GameObject GO = Instantiate(obstaclePrefab, SpawnLocation, Quaternion.identity, obstacleWorldParent);
                rigidBodyAnchor.Add(GO.GetComponentsInChildren<Rigidbody2D>()[0]);
                rigidBodyCollider.Add(GO.GetComponentsInChildren<Rigidbody2D>()[1]);
                Vector2 RandomOffset = Random.insideUnitCircle.normalized * Random.Range(randomDistanceRange.x, randomDistanceRange.y);
                GO.transform.GetChild(0).localPosition = new Vector3(RandomOffset.x, RandomOffset.y, 0);
                GO.transform.GetChild(0).localScale = Vector3.one * scale;
                GO.GetComponent<CFobstacleBehaviour>().manager = this;
                if(x != Mathf.FloorToInt((float)gridSize/2f) || y != Mathf.FloorToInt((float)gridSize / 2f)) { GO.GetComponentInChildren<Collider2D>().enabled = true; } //middle one no collision at start
                obstacles.Add(GO);
			}
		}
        rangeNoBuffer = (scale + extraSpaceBetweenObstacles) * ((float)gridSize / 2f);
        renderTexCam.transform.position = transform.position;

        //Update texture to match render texture
        renderTexCam.targetTexture = renderTex;
        renderTexCam.Render();
        RenderTexture.active = renderTex;

        tex.ReadPixels(new Rect(0, 0, 64, 64), 0, 0);
        tex.Apply();

        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].GetComponent<CFobstacleBehaviour>().SetUp(renderTexCam.transform.position);
        }

        isSetUp = true;
    }

	void Update()
    {
		if (!isSetUp) { return; }

        playerValueProgress = playerColorManager.valueProgress;

        CheckIfCameraOutOfRange();
        for (int i = 0; i < obstacles.Count; i++)
        {
            CheckIfOutOfRange(obstacles[i], i);
        }      

        //Update texture to match render texture
        renderTexCam.targetTexture = renderTex;
        renderTexCam.Render();
        RenderTexture.active = renderTex;

        tex.ReadPixels(new Rect(0, 0, 64, 64), 0, 0);
        tex.Apply();
    }

    private void CheckIfOutOfRange(GameObject obstacle, int index)
    {
        if (obstacle.transform.position.x < transform.position.x - rangeNoBuffer - rangePadding)
		{
            float DifferenceWithEdge = Mathf.Abs(obstacle.transform.position.x - transform.position.x + rangeNoBuffer);
            Vector3 spawnPosNew = new Vector3((transform.position.x + rangeNoBuffer) - DifferenceWithEdge, obstacle.transform.position.y, 0);

            MoveObstacle(obstacle, spawnPosNew);
        }
        else if(obstacle.transform.position.x > transform.position.x + rangeNoBuffer + rangePadding)
		{
            float DifferenceWithEdge = Mathf.Abs(obstacle.transform.position.x - transform.position.x - rangeNoBuffer);
            Vector3 spawnPosNew = new Vector3((transform.position.x - rangeNoBuffer) + DifferenceWithEdge, obstacle.transform.position.y, 0);

            MoveObstacle(obstacle, spawnPosNew);
        }
        else if (obstacle.transform.position.y < transform.position.y - rangeNoBuffer - rangePadding)
		{
            float DifferenceWithEdge = Mathf.Abs(obstacle.transform.position.y - transform.position.y + rangeNoBuffer);
            Vector3 spawnPosNew = new Vector3(obstacle.transform.position.x, (transform.position.y + rangeNoBuffer) - DifferenceWithEdge, 0);

            MoveObstacle(obstacle, spawnPosNew);
        }
        else if(obstacle.transform.position.y > transform.position.y + rangeNoBuffer + rangePadding)
		{
            float DifferenceWithEdge = Mathf.Abs(obstacle.transform.position.y - transform.position.y - rangeNoBuffer);
            Vector3 spawnPosNew = new Vector3(obstacle.transform.position.x, (transform.position.y - rangeNoBuffer) + DifferenceWithEdge, 0);

            MoveObstacle(obstacle, spawnPosNew);
        }
    }

    private void CheckIfCameraOutOfRange()
	{
        if (renderTexCam.transform.position.x < transform.position.x - (scale/2 + extraSpaceBetweenObstacles/2) - rangePadding)
        {
            float DifferenceWithEdge = Mathf.Abs(renderTexCam.transform.position.x - transform.position.x + (scale / 2 + extraSpaceBetweenObstacles / 2));
            Vector3 spawnPosNew = new Vector3((transform.position.x + (scale / 2 + extraSpaceBetweenObstacles / 2)) - DifferenceWithEdge, renderTexCam.transform.position.y, 0);

            renderTexCam.transform.position = spawnPosNew;
        }
        else if (renderTexCam.transform.position.x > transform.position.x + (scale / 2 + extraSpaceBetweenObstacles / 2) + rangePadding)
        {
            float DifferenceWithEdge = Mathf.Abs(renderTexCam.transform.position.x - transform.position.x - (scale / 2 + extraSpaceBetweenObstacles / 2));
            Vector3 spawnPosNew = new Vector3((transform.position.x - (scale / 2 + extraSpaceBetweenObstacles / 2)) + DifferenceWithEdge, renderTexCam.transform.position.y, 0);

            renderTexCam.transform.position = spawnPosNew;
        }
        else if (renderTexCam.transform.position.y < transform.position.y - (scale / 2 + extraSpaceBetweenObstacles / 2) - rangePadding)
        {
            float DifferenceWithEdge = Mathf.Abs(renderTexCam.transform.position.y - transform.position.y + (scale / 2 + extraSpaceBetweenObstacles / 2));
            Vector3 spawnPosNew = new Vector3(renderTexCam.transform.position.x, (transform.position.y + (scale / 2 + extraSpaceBetweenObstacles / 2)) - DifferenceWithEdge, 0);

            renderTexCam.transform.position = spawnPosNew;
        }
        else if (renderTexCam.transform.position.y > transform.position.y + (scale / 2 + extraSpaceBetweenObstacles / 2) + rangePadding)
        {
            float DifferenceWithEdge = Mathf.Abs(renderTexCam.transform.position.y - transform.position.y - (scale / 2 + extraSpaceBetweenObstacles / 2));
            Vector3 spawnPosNew = new Vector3(renderTexCam.transform.position.x, (transform.position.y - (scale / 2 + extraSpaceBetweenObstacles / 2)) + DifferenceWithEdge, 0);

            renderTexCam.transform.position = spawnPosNew;
        }
    }

    private void MoveObstacle(GameObject obstacle, Vector3 NewPos)
	{
        Collider2D col = obstacle.GetComponentInChildren<Collider2D>();   
        //Rigidbody2D[] rgb = obstacle.GetComponentsInChildren<Rigidbody2D>();
        col.enabled = false;
        //Move obstacle to new position
        obstacle.transform.position = NewPos;
        Vector2 RandomOffset = Random.insideUnitCircle.normalized * Random.Range(randomDistanceRange.x, randomDistanceRange.y);
        obstacle.transform.GetChild(0).localPosition = new Vector3(RandomOffset.x, RandomOffset.y, 0);

        //SpringJoint2D spring = obstacle.GetComponentInChildren<SpringJoint2D>();
        //if (spring == null)
        //{
        //    col.gameObject.transform.localPosition = Vector3.zero;
        //    SpringJoint2D springJoint = obstacle.transform.GetChild(0).gameObject.AddComponent<SpringJoint2D>();
        //    springJoint.breakForce = springJointBreakForce;
        //    springJoint.distance = springJointDistance;
        //    springJoint.dampingRatio = springJointDampingRatio;
        //    springJoint.frequency = springJointFrequency;
        //    springJoint.connectedBody = rgb[1];
        //}
        //enable collider
        col.enabled = true;
        obstacle.GetComponent<CFobstacleBehaviour>().SetUp(renderTexCam.transform.position);
    }

    public void UpdateTransforms(Vector3 offset)
    {
        //for (int i = 0; i < obstacles.Count; i++)
        //{
        //    obstacles[i].transform.position -= offset;
        //}
    }
}

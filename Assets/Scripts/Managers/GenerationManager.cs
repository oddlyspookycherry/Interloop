using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GenerationManager : MonoBehaviour
{
    #region Values and references 
    public static GenerationManager Instance{get; private set;}
    private const int specialObjectsCount = 5;

    [Header("Node Generation")]
    public Node nodePrefab;

    public FloatRange rotation;

    private int graphIndex = 0;

    [Header("Segment Generation")]

    public Segment segmentPrefab;

    [Header("KeyDot Generation")]

    public KeyDot keyDotPrefab;

    [Header("FalseDot Generation")]

    public FalseDot falseDotPrefab;

    [Header("Player Generation")]

    public Player playerPrefab;

    [Header("Rogue Generation")]
    public Rogue roguePrefab;
    public float rogueDelay;
    private List<Generatable>[] pools;
    private Level currentLevel;

    [Header("Level Generation")]  
    public Vector2 dimensions; 

    [Range(0f, 0.5f)]
    public float deltaX;

    [Range(0f, 0.5f)]
    public float deltaY;

    [Header("Ripple Spawn")]
    public FloatRange rippleDelay;
    public IntRange numberPerBurst;

    private const float minDist = 3f;

    private float currentTime;

    [HideInInspector] public Vector2[] nodes, dots, falseDots;
    [HideInInspector] public int[,] triangles = new int[0, 0];
    private int rogueCount;

    [Header("VFX")]
    public VFX[] prefabs;

    [Header("Custom Levels")]
    public Level[] customLevels;
    private List<Level> customLevelsList = new List<Level>();

    private void Awake() {
        Instance = this;
        currentLevel = ScriptableObject.CreateInstance<Level>();
        SetIndexes();
        CreatePools();
    }

    private void Start() {
        StartCoroutine(RippleSpawn());
    }

    IEnumerator RippleSpawn()
    {
        while(true)
        {
            int numberToSpawn = numberPerBurst.RandowValue;
            for(int i = 0; i < numberToSpawn; i++)
                GenerateVFX(VFXType.Ripple, CustomRandom.inPlayzone);   
            yield return new WaitForSeconds(rippleDelay.RandowValue);
        }
    }

    private void CreatePools()
    {
        pools = new List<Generatable>[specialObjectsCount + prefabs.Length];
        for(int i = 0; i < pools.Length; i++) {
            pools[i] = new List<Generatable>();
        }
    }

    private void SetIndexes()
    {
        nodePrefab.poolIndex = 0;
        segmentPrefab.poolIndex = 1;
        keyDotPrefab.poolIndex = 2;
        falseDotPrefab.poolIndex = 3;
        roguePrefab.poolIndex = 4;
        for(int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i].poolIndex = specialObjectsCount + i;
        }
    }

    public void Reclaim(Generatable generatableToRecycle)
    {
        pools[generatableToRecycle.poolIndex].Add(generatableToRecycle);
        generatableToRecycle.gameObject.SetActive(false);
    }

    private void PopulateCustomList()
    {
        customLevelsList.Clear();
        for(int i = 0; i < customLevels.Length; i++)
            customLevelsList.Add(customLevels[i]);
    }
    #endregion

    #region Level Calculation

    private Vector2 portal;
    public void CalculateLevel()
    {
        if(Random.value <= GameState.customLevelProbability)
        {
            rogueCount = 0;

            if(customLevelsList.Count == 0)
            {
                if(GameState.isPuzzle)
                {
                    currentLevel.nodes = new Vector2[0];
                    currentLevel.keyDots = new Vector2[0];
                    currentLevel.falseDots = new Vector2[0];
                    currentTime = 10000f;
                    GameManager.Instance.GameEnd(GameOverType.Victory);
                }
                return;
            }
               
            int index = Random.Range(0, customLevelsList.Count);
            Level customLevel = customLevelsList[index];
            currentLevel.nodes = customLevel.nodes;
            currentLevel.keyDots = customLevel.keyDots;
            currentLevel.falseDots = customLevel.falseDots;
            portal = customLevel.portal + GameState.totalOffsetVector;
            currentTime = customLevel.time;
            GeneralMethods.RemoveQuick<Level>(ref customLevelsList, index);
        }
        else
        {

            int rows = GameState.rows.RandowValue;
            int columns = GameState.columns.RandowValue;
            int dotCount = GameState.dotCount.RandowValue;

            int falseDotCount = Mathf.RoundToInt(GameState.falseDotPercent * dotCount);
            int keyDotCount = dotCount - falseDotCount;

            nodes = new Vector2[rows * columns];
            dots = new Vector2[keyDotCount];
            falseDots = new Vector2[falseDotCount];

            float rowStep = dimensions.y / rows;
            float columnStep = dimensions.x / columns;

            for(int r = 0, n = 0; r < rows; r++)
            {
                float y = (-0.5f * dimensions.y) + 0.5f * rowStep + rowStep * r;
                for(int c = 0; c < columns; c++, n++)
                {
                    float x = (- 0.5f * dimensions.x) + 0.5f * columnStep + columnStep * c;
                    nodes[n] = new Vector2(x + columnStep * Random.Range(0f, deltaX) * CustomRandom.sign,
                        y + rowStep * Random.Range(0f, deltaY) * CustomRandom.sign);
                }
            }

            int tRows = rows - 1;
            int tColumns = columns - 1;
            triangles = new int[tRows * tColumns * 2, 3];
            int tCnt = triangles.Length / 3;        

            if(dotCount + 1 > tCnt)
                throw new System.Exception("Invalid Input.");

            for(int i = 0; i < tRows; i++)
            {
                if(i % 2 == 0)
                {
                    for(int j = 0; j < tColumns; j++)
                    {
                        int current = i * 2 * tColumns + j * 2;

                        triangles[current, 0] = i * (tColumns + 1) + j;
                        triangles[current, 1] = triangles[current, 0] + 1;
                        triangles[current, 2] = triangles[current, 0] + (tColumns + 1);


                        triangles[current + 1, 0] = triangles[current, 1];
                        triangles[current + 1, 1] = triangles[current, 2];
                        triangles[current + 1, 2] = triangles[current + 1, 1] + 1;
                    }
                }
                else
                {
                    for(int j = 0; j < tColumns; j++)
                    {
                        int current = i * 2 * tColumns + j * 2;

                        triangles[current, 0] = i * (tColumns + 1) + (tColumns - 1 - j);
                        triangles[current, 1] = triangles[current, 0] + 1;
                        triangles[current, 2] = triangles[current, 1] + (tColumns + 1);


                        triangles[current + 1, 0] = triangles[current, 0];
                        triangles[current + 1, 1] = triangles[current, 2];
                        triangles[current + 1, 2] = triangles[current + 1, 1] - 1;
                    }
                }
            }

            List<int> Tindexs = new List<int>();
            List<int> portalPotential = new List<int>();
            for(int i = 0; i < tCnt; i++)
            {
                Tindexs.Add(i);
                portalPotential.Add(i);
            }

            for(int i = 0; i < tCnt; i++)
            {
                int index = Random.Range(0, portalPotential.Count);
                int ti = portalPotential[index];
                portal = CustomMaths.GetCentroid(nodes[triangles[ti, 0]], nodes[triangles[ti, 1]], nodes[triangles[ti, 2]])
                    + GameState.totalOffsetVector;
                if(Player.Instance == null || !Player.Instance.gameObject.activeSelf || 
                    Vector2.Distance(portal, Player.Instance.transform.position) > minDist)
                {
                    Tindexs.RemoveAt(ti);
                    break;
                }
                GeneralMethods.RemoveQuick<int>(ref portalPotential, index);
            }

            if(portalPotential.Count == 0)
            {
                int randIndex = Random.Range(0, Tindexs.Count);
                portal = CustomMaths.GetCentroid(nodes[triangles[randIndex, 0]],
                    nodes[triangles[randIndex, 1]], 
                    nodes[triangles[randIndex, 2]]) + GameState.totalOffsetVector;
                Tindexs.RemoveAt(randIndex);
            }
            
            List<int> redDotPotential = new List<int>();
            
            int spareTriangles = tCnt - dotCount - 1;
            int toDelete = tCnt - 1 - keyDotCount - Random.Range((int)(spareTriangles * 0.5f), spareTriangles + 1);
            int l1 = Random.Range(0, toDelete + 1);
            int l2 = toDelete - l1;

            for(int i = l1; i < tCnt - 1 - l2; i++)
            {
                redDotPotential.Add(Tindexs[i]);
            }
            for(int i = l1; i < tCnt - 1 - l2; i++)
            {
                Tindexs.RemoveAt(l1);
            }

            for(int i = 0; i < keyDotCount; i++)
            {
                int index = Random.Range(0, redDotPotential.Count);
                int ti = redDotPotential[index];
                dots[i] = CustomMaths.GetCentroid(nodes[triangles[ti, 0]], nodes[triangles[ti, 1]], nodes[triangles[ti, 2]]);
                GeneralMethods.RemoveQuick<int>(ref redDotPotential, index);
            }

            for(int i = 0; i < falseDotCount; i++)
            {
                int index = Random.Range(0, Tindexs.Count);
                int ti = Tindexs[index];
                falseDots[i] = CustomMaths.GetCentroid(nodes[triangles[ti, 0]], nodes[triangles[ti, 1]], nodes[triangles[ti, 2]]);
                GeneralMethods.RemoveQuick<int>(ref Tindexs, index);
            }

            currentLevel.nodes = nodes;
            currentLevel.keyDots = dots;
            currentLevel.falseDots = falseDots;
            rogueCount = GameState.rogueCount.RandowValue;
            currentTime = GameState.time;
        }

        if(Player.Instance != null && Player.Instance.gameObject.activeSelf)
        {
            GameState.currentOffsetVector = (Vector2)Player.Instance.transform.position - portal;
        }
    }
    #endregion

    #region Generation
    public void GenerateStart()
    {
        PopulateCustomList();
        CalculateLevel();
        GeneratePlayer(portal);
        DisplayLevel();
    }

    public void DisplayLevel()
    {
        graphIndex = 0;
        for(int i = 0; i < currentLevel.nodeCount; i++)
            GenerateNode(currentLevel.nodes[i]);
        for(int k = 0; k < currentLevel.keyDotCount; k++)
            GenerateKeyDot(currentLevel.keyDots[k]);
        for(int j = 0; j < currentLevel.falseDotCount; j++)
            GenerateFalseDot(currentLevel.falseDots[j]);
        for(int y = 0; y < rogueCount; y++)
            GenerateRogue();
        if(currentTime < 800f)
            Timer.Instance.SetTimer(currentTime);
    }

    private T Generate<T>(T prefab, bool addToGM = true) where T: Generatable
    {
        T generatable;
        List<Generatable> pool = pools[prefab.poolIndex];
        int lastIndex = pool.Count - 1;
        if(lastIndex >= 0)
        {
            generatable = pool[lastIndex] as T;
            generatable.gameObject.SetActive(true);
            pool.RemoveAt(lastIndex);
        }
        else
        {
            generatable = Instantiate(prefab);
        }
        generatable.poolIndex = prefab.poolIndex;
        if(addToGM)
        {
            GameManager.Instance.AddGeneratable(generatable);
        }
        
        return generatable;
    }

    public Vector2 GetNodePosition(Vector2 position, bool isFirstGet)
    {
        if(Player.Instance == null)
            return new Vector2(10f, 5f);
        Vector2 node;
        do {
            node = currentLevel.nodes[Random.Range(0, nodes.Length - 1)] + GameState.totalOffsetVector;
        } while(Vector2.Distance(position, node) < minDist && (!isFirstGet || Vector2.Distance(position, Player.Instance.transform.position) < minDist));
        return node;
    }

    private void GenerateNode(Vector2 position)
    {
        Node node = Generate<Node>(nodePrefab);
      
        node.graphIndex = graphIndex;
        node.isInGraph = false;
        graphIndex++;

        float Rotation = rotation.RandowValue;
        node.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation);

        Vector2 pos = position + GameState.totalOffsetVector;
        node.transform.localPosition = pos;

        GenerateVFX(VFXType.NodeSpawnEffect, pos);
    }

    private void GenerateKeyDot(Vector2 position)
    {
        KeyDot keyDot = Generate<KeyDot>(keyDotPrefab, false);
        keyDot.transform.localPosition = GameState.totalOffsetVector + position;
        keyDot.transform.localScale = keyDotPrefab.transform.localScale;
        GameManager.Instance.AddKeyDot(keyDot);
    }

    private void GenerateFalseDot(Vector2 position)
    {
        FalseDot falseDot = Generate<FalseDot>(falseDotPrefab, false);
        falseDot.transform.localPosition = GameState.totalOffsetVector + position;
        falseDot.transform.localScale = falseDotPrefab.transform.localScale;
        GameManager.Instance.AddFalseDot(falseDot);
    }

    private void GeneratePlayer(Vector2 position)
    {
        GameObject player = Player.Instance == null ? Instantiate(playerPrefab.gameObject): Player.Instance.gameObject;
        player.SetActive(true);
        player.transform.position = GameState.totalOffsetVector + position;
    }
    public void GenerateSegment(Vector2 pos1, Vector2 pos2, Color color, float width)
    {
        Segment segment = Generate<Segment>(segmentPrefab);
        segment.Initialize(pos1, pos2, color, width);
    }

    private void GenerateRogue()
    {
        Rogue rogue = Generate<Rogue>(roguePrefab);
        Vector2 pos = GetNodePosition((Vector2)Player.Instance.transform.localPosition, true);
        rogue.transform.position = pos;
        GenerateVFX(VFXType.RogueEffect, pos);
        rogue.Enable(rogueDelay);
    }
    public void GenerateVFX(VFXType type, Vector2 position)
    {
        VFX sfx = Generate<VFX>(prefabs[(int)type], false);
        sfx.transform.position = position;
    }
    #endregion

    #if UNITY_EDITOR
    public void RemoveMissingObjects()
    {
        int holes = 0;
		for (int i = 0; i < prefabs.Length - holes; i++) {
			if (prefabs[i] == null) {
				holes += 1;
                System.Array.Copy(
					prefabs, i + 1, prefabs, i,
					prefabs.Length - i - holes
				);
                i -= 1;
			}
		}
        System.Array.Resize(ref prefabs, prefabs.Length - holes);
    }
    #endif
}

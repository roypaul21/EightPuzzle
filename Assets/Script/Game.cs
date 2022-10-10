using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    List<string> CorrectPath = new List<string>();
    public Transform EmptyTile;
    private Camera cam;
    public TileScript[] tiles;
    public int EmptyTileIndex = 0;
    public string _state;
    bool isGoalPosTiles = false;
    int countMove = 0;
    public string NumMoveList = "";
    public float DelayMovementBFS;
    public float TilesSensitiveDistance;
    bool isStart;
    bool isTileMove;
    private Animator anim;

    [SerializeField] private GameObject SolvedPanel;
    [SerializeField] private GameObject RestartBtn;
    [SerializeField] private GameObject StartBtn;
    [SerializeField] private GameObject ShowBtn;
    [SerializeField] private GameObject TrainBFSBtn;
    [SerializeField] private GameObject TrainingPanel;
    [SerializeField] private GameObject WaitingText;
    [SerializeField] private GameObject ShuffleBtn;

    [SerializeField] private AudioSource TilesMoveOne;
    [SerializeField] private AudioSource TilesMoveTwo;
    [SerializeField] private AudioSource TileMoveThree;
    [SerializeField] private AudioSource TileMoveFour;
    [SerializeField] private AudioSource WaitTraining;
    [SerializeField] private AudioSource PuzzleSolvedSounds;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {     
        // for user player 
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit && hit.transform.name != "Empty")
            {
                isTileMove = true;
                //Debug.Log(hit.transform.name);

                if (Vector2.Distance(EmptyTile.position, hit.transform.position) < TilesSensitiveDistance)
                {

                    // last empty tile position
                    Vector2 LastEmptyTilePos = EmptyTile.position;
                    TileScript _tile = hit.transform.GetComponent<TileScript>();

                    // tiles movement
                    int soundIndex = Random.Range(0, 2);
                    if (soundIndex == 1)
                    {
                        TileMoveFour.Play();
                    }
                    else
                    {
                        TileMoveThree.Play();
                    }

                    EmptyTile.position = _tile.TargetTilePos;
                    _tile.TargetTilePos = LastEmptyTilePos;

                    int tileIndex = FindIndex(_tile);
                    tiles[EmptyTileIndex] = tiles[tileIndex];
                    tiles[tileIndex] = null;
                    EmptyTileIndex = tileIndex;

                }

            }
 
        }
        
        if (isTileMove) 
        {
            StartBtn.SetActive(true);
        }

        if (!isTileMove)
        {
            StartBtn.SetActive(false);
            
        }

        if (!isStart)
        {
            TrainBFSBtn.SetActive(false);

        }

        if (isStart)
        {
            TrainBFSBtn.SetActive(true);
        }
        

        // Checking if puzzled is sovled
        if (!isGoalPosTiles)
        {
            int correctTiles = 0;
            foreach (var t in tiles)
            {
                if (t != null)
                {
                    if (t.rightTile)
                        correctTiles++;
                }
            }

            if (correctTiles == tiles.Length - 1)
            {
                RestartBtn.SetActive(false);
                PuzzleSolvedSounds.Play();
                Invoke("PuzzleSolved", 3);
                isGoalPosTiles = true;      
            }
        }
            
    }

    public void StartGame()
    {
        StartBtn.SetActive(false);
        RestartBtn.SetActive(true);
        TrainBFSBtn.SetActive(true);
        isStart = true;
    }

    public void PuzzleSolved()
    {
        SolvedPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        RestartBtn.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SolvedPanel.SetActive(false);
        StartBtn.SetActive(true);
    }

    public int FindIndex(TileScript ts)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
            {
                if (tiles[i] == ts)
                {
                    return i;
                }   
            }
        }
        return -1;
    }

    // Shuffle Tiles
    public void ShuffleTiles()
    {
        int invertion;
        if (EmptyTileIndex != 0)
        {
            var firstTile = tiles[0].TargetTilePos;
            tiles[0].TargetTilePos = EmptyTile.position;
            EmptyTile.position = firstTile;
            tiles[EmptyTileIndex] = tiles[0];
            tiles[0] = null;
            EmptyTileIndex = 0;
        }
        do
        {
            for (int i = 1; i < 9; i++)
            {
                if (tiles[i] != null)
                {
                    anim.SetTrigger("cameraShake");
                    int soundIndex = Random.Range(0, 2);
                    if (soundIndex == 1)
                    {
                        TilesMoveOne.Play();
                    }
                    if (soundIndex == 2)
                    {
                        TilesMoveTwo.Play();
                    }
                    if (soundIndex == 3)
                    {
                        TileMoveThree.Play();
                    }
                    else 
                    {
                        TileMoveFour.Play();
                    }
                    var lastPost = tiles[i].TargetTilePos;
                    int randomIndex = Random.Range(1, 9);
                    tiles[i].TargetTilePos = tiles[randomIndex].TargetTilePos;
                    tiles[randomIndex].TargetTilePos = lastPost;
                    var tile = tiles[i];
                    tiles[i] = tiles[randomIndex];
                    tiles[randomIndex] = tile;
                }
            }

            invertion = Getinvertions();
            Debug.Log("Shuffle!");
        }
        while (invertion % 2 != 0);
        //Debug.Log(invertion);
        isTileMove = true;
        Debug.Log("---Shuffle Complete!");
    }

    public int Getinvertions()
    {
        int invertionsSum = 0;

        for (int i = 1; i < tiles.Length; i++)
        {
            int tileInvertion = 0;

            for (int j = i; j < tiles.Length; j++)
            {
                if (tiles[j] != null)
                {
                    if (tiles[i].Number > tiles[j].Number)
                    {
                        tileInvertion++;
                    }  
                }
            }
            Debug.Log(tileInvertion);

            invertionsSum += tileInvertion;
        }
        return invertionsSum;
    }

    //Breadth First Search 
    public void WaitingTrain()
    {
        TrainingPanel.SetActive(true);
        WaitTraining.Play();
        WaitingText.SetActive(true);
        ShowBtn.SetActive(false);
        Invoke("TrainBFS", 2);
    }
    public void TrainBFS()
    {
        string InitialTileList = "";

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
            {
                InitialTileList += 0;
            }
            else
            {
                InitialTileList += tiles[i].Number;
            }
        }

        Debug.Log("Activate ba yawaa uy");
        CorrectPath = BreadthFirstSearch(InitialTileList);
        WaitTraining.Stop();
        WaitingText.SetActive(false);
        ShowBtn.SetActive(true);

    }

    private IEnumerator TilesMovementDelay()
    {
        WaitForSeconds wait = new WaitForSeconds(DelayMovementBFS);

        CorrectPath.Reverse();
        foreach (var paths in CorrectPath)
        {
            countMove++;
        }

        CertainNumMove();
        Debug.Log(NumMoveList);

        foreach (var NumToMove in NumMoveList)
        {
            int num = (int)char.GetNumericValue(NumToMove);

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] != null)
                {
                    if (tiles[i].Number == num)
                    {
                        Debug.Log(tiles[i].TargetTilePos);
                        Vector2 LastEmptyTilePos = EmptyTile.position;
                        if (Vector2.Distance(EmptyTile.position, tiles[i].TargetTilePos) < TilesSensitiveDistance)
                        {
                            // tiles movement
                            int soundIndex = Random.Range(0, 2);
                            if (soundIndex == 1)
                            {
                                TileMoveThree.Play();
                            }
                            else
                            {
                                TileMoveFour.Play();
                            }
                            EmptyTile.position = tiles[i].TargetTilePos;
                            tiles[i].TargetTilePos = LastEmptyTilePos;
                            Debug.Log(tiles[i].Number);
                            Moves.countMoves += 1;
                            yield return wait;
                        }
                    }
                }
            }
        }
        
        TrainBFSBtn.SetActive(true);
        ShuffleBtn.SetActive(true);
    }

    public void TilesMovementToGoal()
    {
        TrainingPanel.SetActive(false);
        StartCoroutine(TilesMovementDelay());
        TrainBFSBtn.SetActive(false);
        ShuffleBtn.SetActive(false);

    }


    public void CertainNumMove()
    {
        //string NumMoveList = "";
        int countIndex = 0;
        
        foreach (var paths in CorrectPath)
        {
            Debug.Log("--" + paths);
            if (countIndex != (countMove - 1))
            {
                var temp_list = CorrectPath[countIndex + 1];

                //Debug.Log(paths + " ===== " + temp_list);
                var twoList = paths.Zip(temp_list, (fp, sp) => new { paths = fp, temp_list = sp });
                foreach (var list_n in twoList)
                {
                    //Debug.Log(list_n.paths + " == " + list_n.temp_list);

                    int first_num = (int)char.GetNumericValue(list_n.paths);
                    int Second_num = (int)char.GetNumericValue(list_n.temp_list);
                    if (first_num != Second_num && first_num != 0)
                    {  
                        //Debug.Log(list_n.paths);
                        NumMoveList += list_n.paths;
                        break;
                    }
                }
                countIndex++;
            }
                
        }
    }

    public static List<string> BreadthFirstSearch(string initialState)
    {
        Debug.Log("BreadthFirstSearch");
        if (IsWinCondition(initialState))
        {
            return new List<string> { initialState };
        }

        Dictionary<string, string> visitedMap = new Dictionary<string, string>();

        Queue<string> queue = new Queue<string>();
        queue.Enqueue(initialState);

        while (queue.Count > 0)
        {
            string node = queue.Dequeue(); //remove from queue

            foreach (string neighbour in GetNeighbourStates(node))
            {
                if (!visitedMap.ContainsKey(neighbour))
                {
                    visitedMap.Add(neighbour, node);
                    queue.Enqueue(neighbour); // add queue 

                    if (IsWinCondition(neighbour))
                    {
                        Debug.Log("Solution to Path Found");
                        return GeneratePath(visitedMap, neighbour);
                        
                    }
                }
            }
            if (!visitedMap.ContainsKey(node))
            {
                visitedMap.Add(node, "");
            }
        }
        Debug.Log("Cannot be Solved!");
        return null;
    }

    public static List<string> GeneratePath(Dictionary<string, string> parentMap, string endState)
    {
        Debug.Log("GeneratePath");
        List<string> path = new List<string>();
        string parent = endState;
        while (parentMap.ContainsKey(parent))
        {
            path.Add(parent);
            parent = parentMap[parent];
        } 
        return path;
    }

    //Goal State Tile Position
    public static bool IsWinCondition(string state)
    {
        return state.Equals("012345678"); 
    }

    public static List<string> GetNeighbourStates(string state)
    {
        int emptyIndex = state.IndexOf("0");

        bool canMoveLeft = emptyIndex % 3 > 0;
        bool canMoveRight = emptyIndex % 3 < 2;
        bool canMoveUp = emptyIndex / 3 > 0;
        bool canMoveDown = emptyIndex / 3 < 2;

        List<string> neighbours = new List<string>();

        if (canMoveLeft)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex - 1];
            sb[emptyIndex] = newChar;
            sb[emptyIndex - 1] = '0';
            neighbours.Add(sb.ToString());
            Debug.Log(newChar);
        }
        if (canMoveRight)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex + 1];
            sb[emptyIndex] = newChar;
            sb[emptyIndex + 1] = '0';
            neighbours.Add(sb.ToString());
            Debug.Log(newChar);
        }
        if (canMoveUp)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex - 3];
            sb[emptyIndex] = newChar;
            sb[emptyIndex - 3] = '0';
            neighbours.Add(sb.ToString());
            Debug.Log(newChar);
        }
        if (canMoveDown)
        {
            StringBuilder sb = new StringBuilder(state);
            char newChar = sb[emptyIndex + 3];
            sb[emptyIndex] = newChar;
            sb[emptyIndex + 3] = '0';
            neighbours.Add(sb.ToString());
            Debug.Log(newChar);
        }

        

        //Debug.Log("---POSSIBLE PATH---");
        foreach (var x in neighbours)
        {
            //Debug.Log(x);
        }
        return neighbours;
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}




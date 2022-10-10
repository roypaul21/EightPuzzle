using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileScript : MonoBehaviour
{
    public Vector3 TargetTilePos;
    public Vector3 GoalTilePos;
    private SpriteRenderer sprite;
    public int Number;
    [SerializeField] private float speed;
    public bool rightTile;
    public bool start;
    // Start is called before the first frame update
    void Awake()
    {
        TargetTilePos = transform.position;
        GoalTilePos = transform.position;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, TargetTilePos, speed * Time.deltaTime);    
        if (start)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (TargetTilePos == GoalTilePos)
        {
            sprite.color = Color.white;
            rightTile = true;
        }

        else
        {
            sprite.color = new Color(1.2f, 0.6f, 0.6f, 0.5f);
            rightTile = false;
        }

       
    }

    public void startTrigger()
    {
        start = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;
public class Moves : MonoBehaviour
{
    public static int countMoves = 0;
    Text moves;
    // Start is called before the first frame update
    void Start()
    {
        moves = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        moves.text = ""+countMoves+"";
    }

    public void restart()
    {
        countMoves = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

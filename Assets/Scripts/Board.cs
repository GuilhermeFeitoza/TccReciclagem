using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serve para limitar o movimento quando o tabuleiro está sendo preenchido novamente e quando ele está validando os acertos
public enum GameState
{
    wait,
    move


}

public enum TileKind
{
    Breakable,
    Blank,
    Normal


}

[System.Serializable]
public class TileType
{

    public int x;
    public int y;
    public TileKind tileKind;

}


public class Board : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    private FindMatches findMatches;
    public GameObject titlePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    private BackgroundTile[,] breakableTiles;
    public TileType[] boardLayout;
    private bool[,] espacosEmBranco;
    public GameObject[,] allDots;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    public int[] scoreGoals;
    public Dot currentDot;
    public GameObject breakableTilePrefab;
    public GameObject destroyEffect;
         

    // Start is called before the first frame update
    void Start()
    {
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        espacosEmBranco = new bool[width, height];
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        scoreManager = FindAnyObjectByType<ScoreManager>();
        SetUp();


    }

    public void CriarEspacosEmBranco()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                espacosEmBranco[boardLayout[i].x, boardLayout[i].y] = true;

            }
        }


    }
    public void CriarFileirasQuebraveis()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab,tempPosition,Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }

        }

    }
    private void SetUp()
    {
        CriarEspacosEmBranco();
        CriarFileirasQuebraveis();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!espacosEmBranco[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                  

                    GameObject backgroundTile = Instantiate(titlePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    //Testar se já tem peças que estão posicionadas corretamente antes de montar o tabuleiro se sim, mudar a peça.
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }

        }



    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !espacosEmBranco[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;

                }

            }

        }



    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }

            }
        }
        return false;


    }
    private IEnumerator FillBoardCo()
    {
        //Preenche o tabuleiro

        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {

            streakValue += 1;
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        //Só pode se mover depois do tabuleiro terminar de preencher os vazios deixados por ter acertado
        streakValue = 1;
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        if (IsDeadLocked())
        {
            //ShuffleBoard();
            Debug.Log("Deeadlocked");
        }
        currentState = GameState.move;

    }


    void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Armazena a segunda peça e salva ela em um lugar
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //trocando a primeira peça pra ser a peça da segunda posição
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];

        //Define a primeira peça para a segunda
        allDots[column, row] = holder;

    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {


                if (allDots[i, j] != null)
                {
                    if (i < width - 2)
                    {


                        //C
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {

                                return true;
                            }


                        }
                    }
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {

                                return true;

                            }

                        }
                    }
                }
            }
        }
        return false;

    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {

            SwitchPieces(column, row, direction);
            return true;

        }
        SwitchPieces(column, row, direction);
        return false;

    }


    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {


                            return false;

                        }
                    }
                    if (j < height - 1)
                    {

                        if (SwitchAndCheck(i, j, Vector2.up))
                        {

                            return false;

                        }
                    }

                }

            }

        }
        return true;
    }

    private void ShuffleBoard()
    {

        List<GameObject> newBoard = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (allDots[i, j] != null)
                {

                    newBoard.Add(allDots[i, j]);

                }
            }

        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //colocar codigo do blank space

                int pieceToUse = Random.Range(0, newBoard.Count);

                int maxIterations = 0;
                //Testar se já tem peças que estão posicionadas corretamente antes de montar o tabuleiro se sim, mudar a peça.
                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxIterations++;
                }
                Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                maxIterations = 0;
                piece.column = 1;

                piece.row = j;
                allDots[i, j] = newBoard[pieceToUse];
                newBoard.Remove(newBoard[pieceToUse]);
            }
        }

    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {


                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;

                }



            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {

                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;

                }
            }
        }
        else if (column <= 1 || row <= 1)
        {

            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {

                        return true;


                    }
                }
            }

            if (column > 1)
            {
                if (allDots[column - 1, row] !=null  && allDots[column - 2, row] !=null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {

                        return true;

                    }
                }
            }
        }

        return false;
    }

    private void CheckToMakeBombs()
    {


        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Bomba do caminhão 

                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();

                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }

        }

    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;

                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;

                }
            }

        }
        return (numberVertical == 5 || numberHorizontal == 5);

    }
    private void DestroyMatchesAt(int column, int row)
    {

        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if (findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }


            if(breakableTiles[column,row] != null)
            {

                breakableTiles[column, row].TakeDamage(1);
                if(breakableTiles[column,row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            Instantiate(destroyEffect,allDots[column,row].transform.position,Quaternion.identity);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
            scoreManager.IncreaseScore(basePieceValue * streakValue);

        }




    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }



            StartCoroutine(DecreaseRowCo2());
        }

    }

    private IEnumerator DecreaseRowCo2()
    {
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(!espacosEmBranco[i,j] && allDots[i, j] == null)
                {
                   
                    for (int k = j+1; k < height; k++)
                    {
                        if(allDots[i,j] != null)
                        { 
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }

                    }
                }
            }

        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator DecreasoRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;

                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;

                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    // Update is called once per frame
    void Update()
    {

    }
}

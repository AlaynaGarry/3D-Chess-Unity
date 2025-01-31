﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; set; }
    private bool[,] allowedMoves { get; set; }

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private Quaternion whiteOrientation = Quaternion.Euler(0, 270, 0);
    private Quaternion blackOrientation = Quaternion.Euler(0, 90, 0);

    public Chessman[,] Chessmans { get; set; }
    private Chessman selectedChessman;

    public bool isWhiteTurn = true;

    private Material previousMat;
    public Material selectedMat;

    public int[] EnPassantMove { set; get; }

    // Use this for initialization
    void Start()
    {
        Instance = this;
        //SpawnAllChessmans960();
        EnPassantMove = new int[2] { -1, -1 };
    }

    public void StartChess()
    {
        //EndGame();
        SpawnAllChessmans();
    }
    public void StartChess960()
    {
        //EndGame();
        SpawnAllChessmans960();
    }

    public void ClearBoard()
    {
        foreach (GameObject go in activeChessman)
        {
            Destroy(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    // Select the chessman
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    // Move the chessman
                    MoveChessman(selectionX, selectionY);
                }
            }
        }

        if (Input.GetKey("escape"))
            Application.Quit();
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null) return;

        if (Chessmans[x, y].isWhite != isWhiteTurn) return;

        bool hasAtLeastOneMove = false;

        allowedMoves = Chessmans[x, y].PossibleMoves();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                    i = 8;
                    break;
                }
            }
        }

        if (!hasAtLeastOneMove)
            return;

        selectedChessman = Chessmans[x, y];
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;

        BoardHighlights.Instance.HighLightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            Chessman c = Chessmans[x, y];

            if (c != null && c.isWhite != isWhiteTurn)
            {
                // Capture a piece

                if (c.GetType() == typeof(King))
                {
                    // End the game
                    EndGame();
                    return;
                }

                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                if (isWhiteTurn)
                    c = Chessmans[x, y - 1];
                else
                    c = Chessmans[x, y + 1];

                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (selectedChessman.GetType() == typeof(Pawn))
            {
                if (y == 7) // White Promotion
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(1, x, y, true);
                    selectedChessman = Chessmans[x, y];
                }
                else if (y == 0) // Black Promotion
                {
                    activeChessman.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessman(7, x, y, false);
                    selectedChessman = Chessmans[x, y];
                }
                EnPassantMove[0] = x;
                if (selectedChessman.CurrentY == 1 && y == 3)
                    EnPassantMove[1] = y - 1;
                else if (selectedChessman.CurrentY == 6 && y == 4)
                    EnPassantMove[1] = y + 1;
            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = !isWhiteTurn;
        }

        selectedChessman.GetComponent<MeshRenderer>().material = previousMat;

        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessman(int index, int x, int y, bool isWhite)
    {
        Vector3 position = GetTileCenter(x, y);
        GameObject go;

        if (isWhite)
        {
            go = Instantiate(chessmanPrefabs[index], position, whiteOrientation) as GameObject;
        }
        else
        {
            go = Instantiate(chessmanPrefabs[index], position, blackOrientation) as GameObject;
        }

        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void SpawnAllChessmans()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];

        /////// White ///////

        // King
        SpawnChessman(0, 3, 0, true);

        // Queen
        SpawnChessman(1, 4, 0, true);

        // Rooks
        SpawnChessman(2, 0, 0, true);
        SpawnChessman(2, 7, 0, true);

        // Bishops
        SpawnChessman(3, 2, 0, true);
        SpawnChessman(3, 5, 0, true);

        // Knights
        SpawnChessman(4, 1, 0, true);
        SpawnChessman(4, 6, 0, true);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1, true);
        }


        /////// Black ///////

        // King
        SpawnChessman(6, 4, 7, false);

        // Queen
        SpawnChessman(7, 3, 7, false);

        // Rooks
        SpawnChessman(8, 0, 7, false);
        SpawnChessman(8, 7, 7, false);

        // Bishops
        SpawnChessman(9, 2, 7, false);
        SpawnChessman(9, 5, 7, false);

        // Knights
        SpawnChessman(10, 1, 7, false);
        SpawnChessman(10, 6, 7, false);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6, false);
        }
    }

    public void SpawnAllChessmans960()
    {
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];

        /////// White ///////
        ValidateWhitePiece();

        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1, true);
        }

        /////// Black ///////
        ValidateBlackPiece();

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6, false);
        }
    }

    //all pieces & places
    public List<int> allRow0Places = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
    public List<int> allWhitePieces = new List<int>() { 2, 0, 2, 3, 3, 4, 4, 1 };
    private void ValidateWhitePiece()
    {

        //int for the placement 0-7
        List<int> validPlaces = new List<int>();
        validPlaces.AddRange(allRow0Places);

        List<int> validPieces = new List<int>();
        validPieces.AddRange(allWhitePieces);

        int currentPlaceNum = -1;

        bool bishopOnWhite = false;
        int kingLocation = -1;
        int rookLocation = -1;

        foreach (var piece in validPieces)
        {
            do
            {
                int i = UnityEngine.Random.Range(0, 8);
                currentPlaceNum = i;
            } while (!validPlaces.Contains(currentPlaceNum));
            //Bishop
            if (piece == 3)
            {
                ValidateBishop(allWhitePieces, validPlaces, ref currentPlaceNum, ref bishopOnWhite);
            }
            //King
            else if (piece == 0)
            {
                ValidateKing(validPlaces, out currentPlaceNum, out kingLocation);
            }
            //Rook
            else if (piece == 2)
            {
                ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);
            }

            SpawnChessman(piece, currentPlaceNum, 0, true);
            validPlaces.Remove(currentPlaceNum);
            allWhitePieces.Remove(piece);
        }
    }

    public static void ValidateKing(List<int> validPlaces, out int currentPlaceNum, out int kingLocation)
    {
        do
        {
            int i = UnityEngine.Random.Range(1, 7);
            currentPlaceNum = i;
        } while (!validPlaces.Contains(currentPlaceNum));
        kingLocation = currentPlaceNum;
    }

    //all pieces & places
    public List<int> allRow7Places = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
    public List<int> allBlackPieces = new List<int>() { 8, 6, 8, 9, 9, 10, 10, 7 };
    private void ValidateBlackPiece()
    {

        //int for the placement 0-7
        List<int> validPlaces = new List<int>();
        validPlaces.AddRange(allRow7Places);

        List<int> validPieces = new List<int>();
        validPieces.AddRange(allBlackPieces);

        int currentPlaceNum = -1;

        bool bishopOnWhite = false;
        int kingLocation = -1;
        int rookLocation = -1;

        foreach (var piece in validPieces)
        {
            do
            {
                int i = UnityEngine.Random.Range(0, 8);
                currentPlaceNum = i;
            } while (!validPlaces.Contains(currentPlaceNum));
            //Bishop
            if (piece == 9)
            {
                ValidateBishop(allBlackPieces, validPlaces, ref currentPlaceNum, ref bishopOnWhite);
            }
            //King
            else if (piece == 6)
            {
                do
                {
                    int i = UnityEngine.Random.Range(1, 7);
                    currentPlaceNum = i;
                } while (!validPlaces.Contains(currentPlaceNum));
                kingLocation = currentPlaceNum;
            }
            //Rook
            else if (piece == 8)
            {
                ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);
            }

            SpawnChessman(piece, currentPlaceNum, 7, true);
            validPlaces.Remove(currentPlaceNum);
            allBlackPieces.Remove(piece);
        }
    }

    public static void ValidateRook(List<int> validPlaces, ref int currentPlaceNum, int kingLocation, ref int rookLocation)
    {
        if (rookLocation == -1)
        {
            rookLocation = currentPlaceNum;
        }
        else
        {
            if (rookLocation < kingLocation)
            {
                //2nd needs to be more
                do
                {
                    int i = UnityEngine.Random.Range(0, 8);
                    currentPlaceNum = i;
                } while (currentPlaceNum < kingLocation || !validPlaces.Contains(currentPlaceNum));
            }
            else
            {
                do
                {
                    int i = UnityEngine.Random.Range(0, 8);
                    currentPlaceNum = i;
                } while (currentPlaceNum > kingLocation || !validPlaces.Contains(currentPlaceNum));
            }
        }
    }

    public static void ValidateBishop(List<int> validPieces, List<int> validPlaces, ref int currentPlaceNum, ref bool bishopOnWhite)
    {
        bool tileColorIsCorrect = false;
        bool tileIsEmpty = false;

        //while (!tileColorIsCorrect || !tileIsEmpty)
        while (!tileColorIsCorrect)
        {
            //get a tile to validate
            do
            {
                int i = UnityEngine.Random.Range(0, 8);
                currentPlaceNum = i;
            } while (!validPlaces.Contains(currentPlaceNum));

            //validate tilecoloriscorrect
            if (validPieces.Count > 4)
            {
                tileColorIsCorrect = true;
            }
            else
            {
                if (bishopOnWhite)
                {
                    tileColorIsCorrect = currentPlaceNum % 2 == 0;
                }
                else
                {
                    tileColorIsCorrect = currentPlaceNum % 2 != 0;
                }
            }
            //validate tileisempty
            //tileIsEmpty = validPlaces.Contains(currentPlaceNum);
        }
        bishopOnWhite = currentPlaceNum % 2 != 0;
    }

    private void EndGame()
    {
        if (isWhiteTurn)
            Debug.Log("White wins");
        else
            Debug.Log("Black wins");

        ClearBoard();

        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        //SpawnAllChessmans960();
    }
}



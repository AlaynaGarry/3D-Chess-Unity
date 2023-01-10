using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Play Time Tests
/// </summary>
public class RunTest
{
/*    [UnityTest]
    public IEnumerator RunTestWithEnumeratorPasses()
    {
        yield return true;
    }*//*
    [UnityTest]
    public IEnumerator Chess960AllPiecesOnBoard()
    {
        //check that list of valid pieces is empty
        var bm  = new BoardManager();
        List<int> allPieces = new List<int>();
        allPieces.AddRange(bm.allBlackPieces);
        allPieces.AddRange(bm.allWhitePieces);

        yield return new WaitUntil(bm.);
            Assert.IsEmpty(allPieces);
    }
*//*    [UnityTest]
    public IEnumerator Chess960AllPositions()
    {
        //check that list of valid places is empty
        var boardmanager = new BoardManager();
        int placesCount = boardmanager.allRow7Places.Count + boardmanager.allRow0Places.Count;

        yield return new (placesCount);
    }
    [UnityTest]
    public void Chess960BishopColors()
    {
        //check that bishop placement is valid
        //check bishop colorIsCorrect
    }*//*
    [UnityTest]
    public void Chess960Rook()
    {
        var bm = new BoardManager();
        bm.
    }*/
}

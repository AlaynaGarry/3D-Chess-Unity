using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Editor Tests
/// </summary>
public class Chess960
{
    [Test]
    public void Chess960CheckKingPlacement()
    {
        //check that list of valid places is empty
        List<int> pieces = new List<int>() { 1, 2, 3, 4, 5, 6};
        int kingLocation = -1;
        int currentPlaceNum = 2;
        BoardManager.ValidateKing(pieces, out currentPlaceNum, out kingLocation);
        Assert.IsTrue(kingLocation == currentPlaceNum);
    }
    [Test]
    public void Chess960BishopCheckValidPos()
    {
        List<int> validPlaces = new List<int> { 1, 2, 5, 6, 7 };
        List<int> validPieces = new List<int> { 3, 3, 4, 4, 1 };
        int currentPlaceNum = -214;
        bool bishopOnWhite2nd = false;
        bool bishopOnWhite1st = false;

        BoardManager.ValidateBishop(validPieces, validPlaces, ref currentPlaceNum, ref bishopOnWhite2nd);
        //bool bishopOnWhite1st = bishopOnWhite2nd;
        BoardManager.ValidateBishop(validPieces, validPlaces, ref currentPlaceNum, ref bishopOnWhite2nd);
        
        Assert.IsTrue(bishopOnWhite1st != bishopOnWhite2nd);
    }
    [Test]
    public void Chess960CheckRookWhenGivenInvalidPosMoreThanKing()
    {
        List<int> validPlaces = new List<int> { 1, 2, 5, 6, 7 };
        int currentPlaceNum = 6;
        int kingLocation = 3;
        int rookLocation = 4;

        BoardManager.ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);

        Assert.IsTrue(currentPlaceNum < kingLocation && rookLocation > kingLocation);
        //check that king is between king
    }
    [Test]
    public void Chess960CheckRookWhenGivenInvalidPosLessThanKing()
    {
        List<int> validPlaces = new List<int> { 1, 2, 5, 6, 7 };
        int currentPlaceNum = 1;
        int kingLocation = 3;
        int rookLocation = 4;

        BoardManager.ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);

        Assert.IsTrue(currentPlaceNum < kingLocation && rookLocation > kingLocation);
        //check that king is between king
    }
     
    [Test]
    public void Chess960CheckRookWhenGivenValidPosMoreThanKing()
    {
        List<int> validPlaces = new List<int> { 1, 2, 5, 6, 7 };
        int currentPlaceNum = 2;
        int kingLocation = 3;
        int rookLocation = 4;

        BoardManager.ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);

        Assert.IsTrue(currentPlaceNum < kingLocation && rookLocation > kingLocation);
        //check that king is between king
    }
    public void Chess960CheckRookWhenGivenValidPosLessThanKing()
    {
        List<int> validPlaces = new List<int> { 0, 1, 3, 4, 5, 7};
        int currentPlaceNum = 7;
        int kingLocation = 6;
        int rookLocation = 2;

        BoardManager.ValidateRook(validPlaces, ref currentPlaceNum, kingLocation, ref rookLocation);

        Assert.IsTrue(currentPlaceNum < kingLocation && rookLocation > kingLocation);
        //check that king is between king
    }
}
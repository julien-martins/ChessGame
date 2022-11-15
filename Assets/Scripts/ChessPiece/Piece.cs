using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PieceType
{
    Pawn = 0,
    Bioshop = 1,
    Knight = 2,
    Rook = 3,
    Queen = 4,
    King = 5
};

public enum Team
{
    White = 0,
    Black = 6
};

public class Piece : MonoBehaviour
{
    [SerializeField] private PieceType type;
    [SerializeField] private Team team;
    public int indexX;
    public int indexY;
    private Board boardOwner;

    public bool Moved { get; set; }

    public void SetIndex(int x, int y)
    {
        this.indexX = x;
        this.indexY = y;
    }

    public int GetIndexX()
    {
        return indexX;
    }

    public int GetIndexY()
    {
        return indexY;
    }

    public Board GetBoard()
    {
        return boardOwner;
    }

    public void SetBoard(Board board)
    {
        this.boardOwner = board;
    }

    public Team GetTeam()
    {
        return team;
    }

    public PieceType GetPieceType()
    {
        return type;
    }
    
    public virtual Piece Init(int x, int y, Board board)
    {
        this.indexX = x;
        this.indexY = y;
        this.boardOwner = board;
        return this;
    }

    public virtual List<Move> CalculateMove()
    {
        return new List<Move>();
    }

    public override bool Equals(object other)
    {
        Piece p = (Piece) other;
        return indexX.Equals(p.GetIndexX()) && indexY.Equals(p.GetIndexY());
    }
}
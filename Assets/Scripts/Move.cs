using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }

    public bool IsAttack { get; set; }

    public Piece AttackedPiece { get; set; }
    public bool EnPassant { get; set; }
    public bool DoubleForward { get; set; }
    public bool FirstMove { get; set; }

    public bool RookMove { get; set; }
    public Piece RookPiece { get; set; }

    public Move(int sx, int sy, int ex, int ey)
    {
        StartX = sx;
        StartY = sy;
        EndX = ex;
        EndY = ey;
        EnPassant = false;
        DoubleForward = false;
        RookMove = false;
        RookPiece = null;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Move m = (Move) obj;
        return m.StartX == StartX && m.StartY == StartY && m.EndX == EndX && m.EndY == EndY;
    }
}

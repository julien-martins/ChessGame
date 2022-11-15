using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Move> CalculateMove()
    {
        List<Move> moves = new List<Move>();

        int[] dir = { 1, 2, -1, 2, 1, -2, -1, -2, 2, -1, 2, 1, -2, -1, -2, 1};

        for (int i = 0; i < dir.Length; i += 2)
        {
            Move move = new Move(GetIndexX(), GetIndexY(),
                GetIndexX() + dir[i],
                GetIndexY() + dir[i + 1]);

            if (!GetBoard().InBounds(move.EndX, move.EndY)) continue;

            Piece piece = GetBoard().GetPiece(move.EndX, move.EndY);
            if (piece == null)
            {
                moves.Add(move);
            }
            else if (piece.GetTeam() != GetTeam())
            {
                moves.Add(move);
            }
        }

        return moves;
    }
}

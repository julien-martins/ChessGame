using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<Move> CalculateMove()
    {
        List<Move> moves = new List<Move>();

        int[] dir = {0, 1, 1, 0, 0, -1, -1, 0};
        for (int i = 0; i < dir.Length; i += 2)
        {
            for (int j = 1; j < 8; ++j)
            {
                Move move = new Move(GetIndexX(), GetIndexY(), 
                                    GetIndexX() + dir[i] * j,
                                    GetIndexY() + dir[i+1] * j);

                if (!GetBoard().InBounds(move.EndX, move.EndY)) break;

                Piece piece = GetBoard().GetPiece(move.EndX, move.EndY);
                if ( piece == null)
                {
                    moves.Add(move);
                }
                else if (piece.GetTeam() != GetTeam())
                {
                    moves.Add(move);
                    break;
                }
                else
                    break;
            }
        }

        return moves;
    }
}

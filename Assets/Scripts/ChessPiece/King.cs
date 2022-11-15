using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Move> CalculateMove()
    {
        int white = (GetTeam() == Team.White) ? 7 : 0;
        List<Move> moves = new List<Move>();
        Move move;
        int[] dir = { 0, 1, 1, 1, 1, 0, 1, -1, 0, -1, -1, -1, -1, 0, -1, 1 };
        for (int i = 0; i < dir.Length; i += 2)
        {
             move = new Move(GetIndexX(), GetIndexY(),
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
        
        //Castling Move

        int[] dirH = {-1, 1};
        for (int j = 0; j < dirH.Length; ++j)
        {
            move = new Move(GetIndexX(), GetIndexY(), GetIndexX() + 2*dirH[j], GetIndexY());
            bool canRook = true;
            Piece rook = null;
            if (dirH[j] == 1)
            {
                rook = GetBoard().GetPiece(7, white);
                for (int i = GetIndexX() + 1; i < 7; ++i)
                {
                    if (GetBoard().GetPiece(i, white) != null)
                    {
                        canRook = false;
                        break;
                    }
                }
            }
            else
            {
                rook = GetBoard().GetPiece(0, white);
                for (int i = GetIndexX() - 1; i > 0; --i)
                {
                    if (GetBoard().GetPiece(i, white) != null)
                    {
                        canRook = false;
                        break;
                    }
                }
            }

            if (canRook)
            {
                move.RookMove = true;
                move.RookPiece = rook;
                moves.Add(move);
            }
        }

        return moves;
    }
}
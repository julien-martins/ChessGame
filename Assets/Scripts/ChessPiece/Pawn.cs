using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public bool EnPassant { get; set; }

    public override List<Move> CalculateMove()
    {
        EnPassant = false;
        List<Move> moves = new List<Move>();

        int dir = (GetTeam() == Team.White) ? -1 : 1;

        Move move;
        Piece piece = null;

        move = new Move(GetIndexX(), GetIndexY(), GetIndexX(), GetIndexY() + dir);

        if (GetBoard().CanMove(move))
        {
            moves.Add(move);
            move = new Move(GetIndexX(), GetIndexY(), GetIndexX(), GetIndexY() + dir * 2);
            int t = (dir == -1) ? 6 : 1;
            if (GetIndexY() == t && GetBoard().CanMove(move))
            {
                move.DoubleForward = true;
                moves.Add(move);
            }
        }

        move = new Move(GetIndexX(), GetIndexY(), GetIndexX() - 1, GetIndexY() + dir);
        if (GetBoard().InBounds(move.EndX, move.EndY))
            piece = GetBoard().GetPiece(move.EndX, move.EndY);
        if (piece != null && piece.GetTeam() != GetTeam())
            moves.Add(move);

            move = new Move(GetIndexX(), GetIndexY(), GetIndexX() + 1, GetIndexY() + dir);
        if (GetBoard().InBounds(move.EndX, move.EndY))
            piece = GetBoard().GetPiece(move.EndX, move.EndY);
        if (piece != null && piece.GetTeam() != GetTeam())
            moves.Add(move);

        // EN PASSANT MOVE
        // check gauche
        // SI un pion adversaire est a cote et a avancee de deux on peut faire le move
        move = new Move(GetIndexX(), GetIndexY(), GetIndexX() + 1, GetIndexY() + dir);
        if (GetBoard().InBounds(move.EndX, move.EndY - dir))
            piece = GetBoard().GetPiece(move.EndX, move.EndY - dir);
        if (piece != null && piece.GetTeam() != GetTeam() && piece.GetPieceType() == PieceType.Pawn)
        {
            if (piece.GetPieceType() == PieceType.Pawn && ((Pawn) piece).EnPassant)
            {
                move.EnPassant = true;
                moves.Add(move);
            }

        }

        move = new Move(GetIndexX(), GetIndexY(), GetIndexX() - 1, GetIndexY() + dir);
        if (GetBoard().InBounds(move.EndX, move.EndY - dir))
            piece = GetBoard().GetPiece(move.EndX, move.EndY - dir);
        if (piece != null && piece.GetTeam() != GetTeam() && piece.GetPieceType() == PieceType.Pawn)
        {
            if (piece.GetPieceType() == PieceType.Pawn && ((Pawn)piece).EnPassant)
            {
                move.EnPassant = true;
                moves.Add(move);
            }

        }


        return moves;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Board : MonoBehaviour
{
    private Camera main;

    [SerializeField] private GameObject board;
    [SerializeField] private GameObject movePrefab;
    [SerializeField] private GameObject[] piecesPrefab;

    private Piece[] boardPieces;

    public Piece PieceFocus { get; set; }

    public float Width { get; set; }
    public float Height { get; set; }
    public float CellWidth { get; set; }
    public float CellHeight { get; set; }

    private List<GameObject> movesObjects;
    public List<Move> moves { get; set; }

    public Piece EnPassantPawn { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        boardPieces = new Piece[64];
        
        Mesh mesh = board.GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;

        Width = board.transform.localScale.x * bounds.size.x;
        Height = board.transform.localScale.z * bounds.size.z;

        CellWidth = Width / 8;
        CellHeight = Height / 8;

        moves = new List<Move>();
        movesObjects = new List<GameObject>();

        PlacePieces("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        //PlacePieces("rnbqkbnr/pp1pppp1/8/2pr4/4P3/5N2/PPPP1PPP/RNBQKB1R");

        moves = CalculateAllMove(Team.White);
    }

    void PlacePiece(PieceType type, Team team, int indexX, int indexY)
    {
        int index = indexY * 8 + indexX;

        Vector3 pos = new Vector3();
        pos.z = indexX * CellWidth - Width / 2 + CellWidth/2;
        pos.x = -indexY * CellHeight + Height / 2 - CellHeight/2;

        int pieceType = (int)type + (int) team;
        GameObject go = Instantiate(piecesPrefab[pieceType], pos, Quaternion.identity);
        boardPieces[index] = go.GetComponent<Piece>().Init(indexX, indexY, this);
    }
    void PlacePieces(string fen)
    {
        int x = 0;
        int y = 0;
        PieceType type = PieceType.Pawn;
        Team team;
        for(int i = 0; i < fen.Length; ++i)
        {
            char c = fen[i];
            if (c == '/')
            {
                x = 0;
                y++;
                continue;
            }
            
            if (char.IsDigit(c))
            {
                x += Int32.Parse(c.ToString());
                if (x >= 8) x = 0;
                continue;
            }

            team = char.IsUpper(c) ? Team.White : Team.Black;

            switch (char.ToLower(c))
            {
                case 'r':
                    type = PieceType.Rook;
                    break;
                case 'n':
                    type = PieceType.Knight;
                    break;
                case 'b':
                    type = PieceType.Bioshop;
                    break;
                case 'k':
                    type = PieceType.King;
                    break;
                case 'q':
                    type = PieceType.Queen;
                    break;
                case 'p':
                    type = PieceType.Pawn;
                    break;
            }
            
            PlacePiece(type, team, x, y);
            x++;
        }

    }

    public void MovePiece(Piece p, int x, int y)
    {
        boardPieces[p.GetIndexY() * 8 + p.GetIndexX()] = null;
        p.SetIndex(x, y);
        boardPieces[y * 8 + x] = p;

        Vector3 pos = new Vector3();
        pos.z = x * CellWidth - Width / 2 + CellWidth / 2;
        pos.x = -y * CellHeight + Height / 2 - CellHeight / 2;

        p.gameObject.transform.position = pos;
    }

    public bool CanMove(Move move)
    {
        if (move.EndX <= 7 && move.EndY <= 7 && move.EndX >= 0 && move.EndY >= 0 && 
            GetPiece(move.EndX, move.EndY) == null)
            return true;
        return false;
    }

    public bool HasPiece(int x, int y)
    { 
        return boardPieces[y * 8 + x] != null;
    }

    public bool InBounds(int x, int y)
    {
        return (x >= 0 && y >= 0 && x <= 7 && y <= 7);
    }
    public Piece GetPiece(int x, int y)
    {
        return boardPieces[y * 8 + x];
    }

    
    
    public bool MakeMove(Move move)
    {
        Piece p = GetPiece(move.StartX, move.StartY);

        bool exist = false;
        foreach (Move m in moves)
        {
            if (m.Equals(move))
            {
                move.EnPassant = m.EnPassant;
                move.DoubleForward = m.DoubleForward;
                move.RookMove = m.RookMove;
                move.RookPiece = m.RookPiece;
                exist = true;
            }
        }

        if (!exist) return false;

        if (!p.Moved && !move.FirstMove)
        {
            move.FirstMove = true;
            p.Moved = true;
            Debug.Log("The first move");
        }

        if (p.GetPieceType() == PieceType.Pawn && move.DoubleForward)
        {
            ((Pawn)p).EnPassant = true;
            EnPassantPawn = p;
        }
        
        if (move.EnPassant)
        {
            move.AttackedPiece = EnPassantPawn;
            Destroy(boardPieces[EnPassantPawn.GetIndexY() * 8 + EnPassantPawn.GetIndexX()].gameObject);
            boardPieces[EnPassantPawn.GetIndexY() * 8 + EnPassantPawn.GetIndexX()] = null;
        }

        if (move.RookMove)
        {
            p.Moved = true;
            move.RookPiece.Moved = true;
            if (move.RookPiece.GetIndexX() == 0)
            {
                boardPieces[move.EndY * 8 + move.EndX + 1] = move.RookPiece;
                MovePiece(move.RookPiece, move.EndX + 1, move.EndY);
            }
            else
            {
                boardPieces[move.EndY * 8 + move.EndX - 1] = move.RookPiece;
                MovePiece(move.RookPiece, move.EndX - 1, move.EndY);
            }
        }

        if (GetPiece(move.EndX, move.EndY) != null)
        {
            Destroy(boardPieces[move.EndY * 8 + move.EndX].gameObject);
            boardPieces[move.EndY * 8 + move.EndX] = null;
        }

        MovePiece(p, move.EndX, move.EndY);
        return true;
    }

    public bool CastlingMove()
    {
        return false;

    }
    public bool EnPassantMove()
    {
        return false;

    }

    public void ShowLegalMove()
    {
        ClearLegalMove();

        foreach (var move in moves)
        {
            if (move.StartX == PieceFocus.GetIndexX() && move.StartY == PieceFocus.GetIndexY())
            {
                Vector3 pos = new Vector3();
                pos.z = move.EndX * CellWidth - Width / 2 + CellWidth / 2;
                pos.x = -move.EndY * CellHeight + Height / 2 - CellHeight / 2;

                GameObject go = Instantiate(movePrefab, pos, Quaternion.identity);
                movesObjects.Add(go);
            }
        }
    }

    public void ClearLegalMove()
    {
        foreach (var go in movesObjects)
        {
            Destroy(go);
        }
        movesObjects.Clear();
    }
    
    public List<Move> CalculateAllMove(Team turn)
    {
        List<Move> moves = new List<Move>();
        for (int i = 0; i < boardPieces.Length; ++i)
        {
            Piece piece = boardPieces[i];

            if (piece != null && piece.GetTeam() == turn) moves.AddRange(piece.CalculateMove());
        }

        return moves;
    }

    public bool MakeFakeMove(Move move)
    {
        Piece startPiece = GetPiece(move.StartX, move.StartY);
        Piece endPiece = GetPiece(move.EndX, move.EndY);

        if (startPiece == null) return false;

        startPiece.SetIndex(move.EndX, move.EndY);

        if (!startPiece.Moved)
        {
            move.FirstMove = true;
            startPiece.Moved = true;
        }

        if (startPiece.GetPieceType() == PieceType.Pawn && move.DoubleForward)
        {
            ((Pawn)startPiece).EnPassant = true;
        }
        
        if (endPiece != null)
            move.AttackedPiece = endPiece;
            
        boardPieces[move.EndY * 8 + move.EndX] = startPiece;
        boardPieces[move.StartY * 8 + move.StartX] = null;
        

        return true;
    }
    public bool UndoFakeMove(Move move)
    {
        Piece p = GetPiece(move.EndX, move.EndY);

        p.SetIndex(move.StartX, move.StartY);
        boardPieces[p.GetIndexY() * 8 + p.GetIndexX()] = p;

        if (p.Moved && move.FirstMove)
        {
            p.Moved = false;
        }

        if (p.GetPieceType() == PieceType.Pawn && move.DoubleForward)
        {
            ((Pawn)p).EnPassant = false;
        }

        if (move.AttackedPiece != null)
            boardPieces[move.AttackedPiece.GetIndexY() * 8 + move.AttackedPiece.GetIndexX()] = move.AttackedPiece;
        else 
            boardPieces[move.EndY * 8 + move.EndX] = null;

        return true;
    }

    public List<Move> CalculateLegalMove(Team turn)
    {
        Team opponentTurn = (turn == Team.Black) ? Team.White : Team.Black;
        List<Move> pseudoLegalMoves = CalculateAllMove(turn);
        List<Move> legalMoves = new List<Move>();

        foreach (Move ally_move in pseudoLegalMoves)
        {
            MakeFakeMove(ally_move);
            List<Move> opponent_moves = CalculateAllMove(opponentTurn);

            if (opponent_moves.Any((res) =>
            {
                Piece piece = GetPiece(res.EndX, res.EndY);
                return piece != null && piece.GetPieceType() == PieceType.King;
            }))
            { }
            else
                legalMoves.Add(ally_move);

            UndoFakeMove(ally_move);
        }

        return legalMoves;
    }
}

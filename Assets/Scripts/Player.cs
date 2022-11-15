using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Board board;

    private Team turn;

    // Start is called before the first frame update
    void Start()
    {
        turn = Team.White;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 3000, LayerMask.GetMask("Board")))
            {
                int cellX = (int)((hit.point.z + board.Height / 2) / board.CellHeight);
                int cellY = -(int)((hit.point.x - board.Width / 2) / board.CellWidth);

                Move move;
                Piece piece = board.GetPiece(cellX, cellY);
                if (piece != null)
                {
                    if (piece.GetTeam() == turn)
                    {
                        if (board.PieceFocus != piece)
                        {
                            board.PieceFocus = piece;
                            board.ShowLegalMove();
                        }
                    }
                    else
                    {
                        //Debug.Log("ATTACK !!!");
                        if (board.PieceFocus != null)
                        {
                            move = new Move(board.PieceFocus.GetIndexX(), board.PieceFocus.GetIndexY(),
                                cellX, cellY);
                            if (board.MakeMove(move))
                            {
                                board.ClearLegalMove();
                                turn = (turn == Team.White) ? Team.Black : Team.White;
                                board.moves = board.CalculateLegalMove(turn);
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log("MOVE !!!");
                    if (board.PieceFocus != null)
                    {
                        move = new Move(board.PieceFocus.GetIndexX(), board.PieceFocus.GetIndexY(),
                            cellX, cellY);
                        if (board.MakeMove(move))
                        {
                            board.ClearLegalMove();
                            turn = (turn == Team.White) ? Team.Black : Team.White;
                            board.moves = board.CalculateLegalMove(turn);
                        }
                        
                    }
                }


            }
        }
    }
}

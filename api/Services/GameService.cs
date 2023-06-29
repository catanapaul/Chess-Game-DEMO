using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.DTO;
using api.Models.Entities;
using api.Models.Entities.Pieces;
using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class GameService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public GameService(DataContext dataContext, IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;
        }

        #region basic requests

        public async Task<IEnumerable<GameDto>> GetAllAsync()
        {
            return await _dataContext.Games
               .ProjectTo<GameDto>(_mapper.ConfigurationProvider)
               .ToListAsync();
        }


        public Board getInitMatrix()
        {
            #region default matrix

            Piece[,] chessBoard = new Piece[8, 8];

            chessBoard[0, 0] = new Rook(PieceColor.White, PieceName.Rook, true);
            chessBoard[0, 1] = new Knight(PieceColor.White, PieceName.Knight, true);
            chessBoard[0, 2] = new Bishop(PieceColor.White, PieceName.Bishop, true);
            chessBoard[0, 3] = new King(PieceColor.White, PieceName.King, true);
            chessBoard[0, 4] = new Queen(PieceColor.White, PieceName.Queen, true);
            chessBoard[0, 5] = new Bishop(PieceColor.White, PieceName.Bishop, true);
            chessBoard[0, 6] = new Knight(PieceColor.White, PieceName.Knight, true);
            chessBoard[0, 7] = new Rook(PieceColor.White, PieceName.Rook, true);

            chessBoard[1, 0] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 1] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 2] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 3] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 4] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 5] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 6] = new Pawn(PieceColor.White, PieceName.Pawn, true);
            chessBoard[1, 7] = new Pawn(PieceColor.White, PieceName.Pawn, true);


            chessBoard[6, 0] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 1] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 2] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 3] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 4] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 5] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 6] = new Pawn(PieceColor.Black, PieceName.Pawn, true);
            chessBoard[6, 7] = new Pawn(PieceColor.Black, PieceName.Pawn, true);

            chessBoard[7, 0] = new Rook(PieceColor.Black, PieceName.Rook, true);
            chessBoard[7, 1] = new Knight(PieceColor.Black, PieceName.Knight, true);
            chessBoard[7, 2] = new Bishop(PieceColor.Black, PieceName.Bishop, true);
            chessBoard[7, 3] = new King(PieceColor.Black, PieceName.King, true);
            chessBoard[7, 4] = new Queen(PieceColor.Black, PieceName.Queen, true);
            chessBoard[7, 5] = new Bishop(PieceColor.Black, PieceName.Bishop, true);
            chessBoard[7, 6] = new Knight(PieceColor.Black, PieceName.Knight, true);
            chessBoard[7, 7] = new Rook(PieceColor.Black, PieceName.Rook, true);

            #endregion

            List<List<Piece>> chessboardList = new List<List<Piece>>();
            int rows = chessBoard.GetLength(0);
            int columns = chessBoard.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                List<Piece> rowList = new List<Piece>();
                for (int j = 0; j < columns; j++)
                {
                    rowList.Add(chessBoard[i, j]);
                }
                chessboardList.Add(rowList);
            }

            Board myboard = new Board();
            myboard.Matrix = chessboardList;
            myboard.Turn = 1;
            myboard.BlackPieces = new List<Piece>();
            myboard.WhitePieces = new List<Piece>();

            return myboard;
        }


        public Cell[] GetLegalMoves(BoardDto board)
        {
            PieceDto selectedPiece = board.SelectedPiece;

            if (selectedPiece == null || selectedPiece.Position == null)
            {
                return null;
            }

            int row = selectedPiece.Position.Row;
            int col = selectedPiece.Position.Col;

            switch (selectedPiece.PieceName)
            {
                case PieceName.Pawn:
                    return GetPawnMoves(board.Matrix, selectedPiece);
                case PieceName.Knight:
                    return GetKnightMoves(board.Matrix, selectedPiece);
                case PieceName.Bishop:
                    return GetBishopMoves(board.Matrix, selectedPiece);
                case PieceName.Rook:
                    return GetRookMoves(board.Matrix, selectedPiece);
                case PieceName.Queen:
                    return GetQueenMoves(board.Matrix, selectedPiece);
                case PieceName.King:
                    return GetKingMoves(board.Matrix, selectedPiece);
                default:
                    return null;
            }
        }


        public bool VerifyIfCheckMatt(BoardDto board)
        {
            PieceColor kingColor = board.Turn == 1 ? PieceColor.White : PieceColor.Black;
            PieceDto king = FindKing(board.Matrix, kingColor);

            List<Cell> allMoves = GetAllPossibleMoves(board, kingColor);

            if (allMoves.Count == 0) return true;

            return false;
        }

        private List<Cell> GetAllPossibleMoves(BoardDto board, PieceColor color)
        {
            List<Cell> allMoves = new List<Cell>();

            foreach (var pieceRow in board.Matrix)
            {
                foreach (var piece in pieceRow)
                {
                    if (piece != null && piece.Color == color)
                    {
                        BoardDto myTempBoard = DeepCopyBoard(board);

                        Cell[] moves = GetLegalMoves(myTempBoard);

                        if (moves != null)
                        {
                            allMoves.AddRange(moves);
                        }
                    }
                }
            }

            return allMoves;
        }

        private BoardDto DeepCopyBoard(BoardDto board)
        {
            BoardDto copiedBoard = new BoardDto();

            List<List<PieceDto>> copiedMatrix = new List<List<PieceDto>>();
            foreach (var pieceRow in board.Matrix)
            {
                List<PieceDto> copiedPieceRow = new List<PieceDto>();
                foreach (var piece in pieceRow)
                {
                    // Create a new PieceDto object and copy its properties
                    PieceDto copiedPiece = new PieceDto();
                    if (piece != null)
                    {
                        copiedPiece.PieceName = piece.PieceName;
                        copiedPiece.Color = piece.Color;

                        if (piece.Position != null)
                        {
                            var copiedPieceCell = new CellDto();
                            copiedPieceCell.Row = piece.Position.Row;
                            copiedPieceCell.Col = piece.Position.Col;

                            copiedPiece.Position = copiedPieceCell;
                        }
                        else
                        {
                            copiedPiece.Position = null;
                        }
                    }
                    else
                    {
                        copiedPiece = null;
                    }
                    // Add the copied piece to the copied row
                    copiedPieceRow.Add(copiedPiece);
                }

                // Add the copied row to the copied matrix
                copiedMatrix.Add(copiedPieceRow);
            }

            copiedBoard.Matrix = copiedMatrix;

            PieceDto selectedPiece = new PieceDto();
            if (board.SelectedPiece != null)
            {
                selectedPiece.PieceName = board.SelectedPiece.PieceName;
                selectedPiece.Color = board.SelectedPiece.Color;

                if (board.SelectedPiece.Position != null)
                {
                    var copiedPieceCell = new CellDto();
                    copiedPieceCell.Row = board.SelectedPiece.Position.Row;
                    copiedPieceCell.Col = board.SelectedPiece.Position.Col;

                    selectedPiece.Position = copiedPieceCell;
                }
                else
                {
                    selectedPiece.Position = null;
                }
            }
            else
            {
                selectedPiece = null;
            }

            copiedBoard.SelectedPiece = selectedPiece;

            return copiedBoard;
        }
        #endregion

        #region Knight

        private Cell[] GetKnightMoves(List<List<PieceDto>> matrix, PieceDto knight)
        {
            int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
            int[] dy = { 1, 2, 2, 1, -1, -2, -2, -1 };

            List<Cell> validMoves = new List<Cell>();

            int currentRow = knight.Position.Row;
            int currentCol = knight.Position.Col;

            for (int i = 0; i < 8; i++)
            {
                int newRow = currentRow + dx[i];
                int newCol = currentCol + dy[i];

                if (IsValidMove(matrix, newRow, newCol, knight.Color))
                {

                    if (!IsMoveCausingCheck(matrix, knight, currentRow, currentCol))
                    {
                        validMoves.Add(new Cell(newRow, newCol));
                    }
                }
            }

            return validMoves.ToArray();
        }


        private bool IsValidMove(List<List<PieceDto>> matrix, int row, int col, PieceColor color)
        {
            int numRows = matrix.Count;
            int numCols = matrix[0].Count;

            if (row < 0 || row >= numRows || col < 0 || col >= numCols)
            {
                return false;
            }

            PieceDto destinationPiece = matrix[row][col];

            return destinationPiece == null || destinationPiece.Color != color;
        }


        #endregion

        #region Pawn

        private Cell[] GetPawnMoves(List<List<PieceDto>> matrix, PieceDto pawn)
        {
            int direction = pawn.Color == PieceColor.White ? 1 : -1;

            List<Cell> legalMoves = new List<Cell>();

            int forwardRow = pawn.Position.Row + direction;
            int forwardCol = pawn.Position.Col;
            if (IsValidPosition(matrix, forwardRow, forwardCol) && matrix[forwardRow][forwardCol] == null)
            {
                if (!IsMoveCausingCheck(matrix, pawn, forwardRow, forwardCol))
                {
                    legalMoves.Add(new Cell(forwardRow, forwardCol));

                    if (!pawn.Moved)
                    {
                        int doubleForwardRow = pawn.Position.Row + (2 * direction);
                        int doubleForwardCol = pawn.Position.Col;
                        if (IsValidPosition(matrix, doubleForwardRow, doubleForwardCol) && matrix[doubleForwardRow][doubleForwardCol] == null)
                        {
                            legalMoves.Add(new Cell(doubleForwardRow, doubleForwardCol));
                        }
                    }
                }
            }

            int captureRow1 = pawn.Position.Row + direction;
            int captureCol1 = pawn.Position.Col - 1;
            int captureRow2 = pawn.Position.Row + direction;
            int captureCol2 = pawn.Position.Col + 1;
            if (IsValidPosition(matrix, captureRow1, captureCol1) && matrix[captureRow1][captureCol1] != null &&
                matrix[captureRow1][captureCol1].Color != pawn.Color && !IsMoveCausingCheck(matrix, pawn, captureRow1, captureCol1))
            {
                legalMoves.Add(new Cell(captureRow1, captureCol1));
            }
            if (IsValidPosition(matrix, captureRow2, captureCol2) && matrix[captureRow2][captureCol2] != null &&
                matrix[captureRow2][captureCol2].Color != pawn.Color && !IsMoveCausingCheck(matrix, pawn, captureRow2, captureCol2))
            {
                legalMoves.Add(new Cell(captureRow2, captureCol2));
            }

            return legalMoves.ToArray();

        }


        private bool IsValidPosition(List<List<PieceDto>> matrix, int row, int col)
        {
            return row >= 0 && row < matrix.Count && col >= 0 && col < matrix[row].Count;
        }


        private bool IsMoveCausingCheck(List<List<PieceDto>> matrix, PieceDto piece, int targetRow, int targetCol)
        {

            PieceDto storePiece = matrix[targetRow][targetCol];

            PieceDto originalPiece = matrix[piece.Position.Row][piece.Position.Col];
            matrix[piece.Position.Row][piece.Position.Col] = null;
            matrix[targetRow][targetCol] = piece;

            var possibleMoveCell = new CellDto();
            possibleMoveCell.Row = targetRow;
            possibleMoveCell.Col = targetCol;

            piece.Position = possibleMoveCell;

            PieceDto king = FindKing(matrix, piece.Color);

            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    if (matrix[i][j] != null && matrix[i][j].Color != piece.Color)
                    {
                        var possibleThreatPosition = new CellDto();
                        possibleThreatPosition.Col = j;
                        possibleThreatPosition.Row = i;

                        matrix[i][j].Position = possibleThreatPosition;

                        if (IsPieceThreateningKing(matrix[i][j], king.Position.Row, king.Position.Col, matrix))
                        {
                            matrix[piece.Position.Row][piece.Position.Col] = storePiece;
                            matrix[originalPiece.Position.Row][originalPiece.Position.Col] = originalPiece;

                            piece.Position = originalPiece.Position;

                            return true;
                        }
                    }
                }
            }

            matrix[piece.Position.Row][piece.Position.Col] = storePiece;
            matrix[originalPiece.Position.Row][originalPiece.Position.Col] = originalPiece;

            piece.Position = originalPiece.Position;

            return false;
        }

        #endregion



        #region Bishop


        public Cell[] GetBishopMoves(List<List<PieceDto>> matrix, PieceDto bishop)
        {
            List<Cell> legalMoves = new List<Cell>();

            int direction = bishop.Color == PieceColor.White ? 1 : -1;

            int[] rowOffsets = { -1, -1, 1, 1 };
            int[] colOffsets = { -1, 1, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int row = bishop.Position.Row + direction * rowOffsets[i];
                int col = bishop.Position.Col + direction * colOffsets[i];

                while (IsValidCell(row, col))
                {
                    PieceDto targetPiece = matrix[row][col];

                    if (targetPiece == null)
                    {
                        if (!IsMoveCausingCheck(matrix, bishop, row, col))
                        {
                            legalMoves.Add(new Cell(row, col));
                        }
                    }
                    else
                    {
                        if (targetPiece.Color != bishop.Color)
                        {
                            if (!IsMoveCausingCheck(matrix, bishop, row, col))
                            {
                                legalMoves.Add(new Cell(row, col));
                            }
                        }

                        break;
                    }

                    row += direction * rowOffsets[i];
                    col += direction * colOffsets[i];
                }
            }

            return legalMoves.ToArray();

        }


        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }


        #endregion



        #region Rook


        private Cell[] GetRookMoves(List<List<PieceDto>> matrix, PieceDto rook)
        {
            List<Cell> legalMoves = new List<Cell>();

            int row = rook.Position.Row;
            int col = rook.Position.Col;

            // Check legal moves in vertical and horizontal directions
            CheckDirection(matrix, rook, -1, 0, legalMoves); // Upward direction
            CheckDirection(matrix, rook, 1, 0, legalMoves);  // Downward direction
            CheckDirection(matrix, rook, 0, -1, legalMoves); // Left direction
            CheckDirection(matrix, rook, 0, 1, legalMoves);  // Right direction

            return legalMoves.ToArray();
        }


        private void CheckDirection(List<List<PieceDto>> matrix, PieceDto rook, int rowOffset, int colOffset, List<Cell> legalMoves)
        {
            int row = rook.Position.Row;
            int col = rook.Position.Col;

            while (true)
            {
                row += rowOffset;
                col += colOffset;

                if (row < 0 || row >= matrix.Count || col < 0 || col >= matrix[row].Count)
                {
                    break;
                }

                PieceDto targetPiece = matrix[row][col];

                if (targetPiece == null)
                {
                    if (!IsMoveCausingCheck(matrix, rook, row, col))
                    {
                        legalMoves.Add(new Cell(row, col));
                    }
                }
                else if (targetPiece.Color != rook.Color)
                {
                    if (!IsMoveCausingCheck(matrix, rook, row, col))
                    {
                        legalMoves.Add(new Cell(row, col));
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        #endregion



        #region Queen


        public Cell[] GetQueenMoves(List<List<PieceDto>> matrix, PieceDto queen)
        {
            List<Cell> legalMoves = new List<Cell>();

            GetDiagonalMoves(matrix, queen, legalMoves);

            GetStraightMoves(matrix, queen, legalMoves);

            return legalMoves.ToArray();
        }


        private void GetDiagonalMoves(List<List<PieceDto>> matrix, PieceDto queen, List<Cell> legalMoves)
        {
            int row = queen.Position.Row;
            int col = queen.Position.Col;

            CheckDirectionQueen(matrix, queen, -1, -1, legalMoves); // Up-Left direction
            CheckDirectionQueen(matrix, queen, -1, 1, legalMoves);  // Up-Right direction
            CheckDirectionQueen(matrix, queen, 1, -1, legalMoves);  // Down-Left direction
            CheckDirectionQueen(matrix, queen, 1, 1, legalMoves);   // Down-Right direction
        }


        private void GetStraightMoves(List<List<PieceDto>> matrix, PieceDto piece, List<Cell> legalMoves)
        {
            int row = piece.Position.Row;
            int col = piece.Position.Col;

            CheckDirectionQueen(matrix, piece, -1, 0, legalMoves);
            CheckDirectionQueen(matrix, piece, 1, 0, legalMoves);
            CheckDirectionQueen(matrix, piece, 0, -1, legalMoves);
            CheckDirectionQueen(matrix, piece, 0, 1, legalMoves);
        }


        private void CheckDirectionQueen(List<List<PieceDto>> matrix, PieceDto queen, int rowOffset, int colOffset, List<Cell> legalMoves)
        {
            int row = queen.Position.Row;
            int col = queen.Position.Col;

            while (true)
            {
                row += rowOffset;
                col += colOffset;

                if (row < 0 || row >= matrix.Count || col < 0 || col >= matrix[row].Count)
                {
                    break;
                }

                PieceDto targetPiece = matrix[row][col];

                if (targetPiece == null)
                {
                    if (!IsMoveCausingCheck(matrix, queen, row, col))
                    {
                        legalMoves.Add(new Cell(row, col));
                    }
                }
                else if (targetPiece.Color != queen.Color)
                {
                    if (!IsMoveCausingCheck(matrix, queen, row, col))
                    {
                        legalMoves.Add(new Cell(row, col));
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        #endregion



        #region King


        public Cell[] GetKingMoves(List<List<PieceDto>> matrix, PieceDto king)
        {
            List<Cell> legalMoves = new List<Cell>();

            int row = king.Position.Row;
            int col = king.Position.Col;

            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int newRow = row + rowOffsets[i];
                int newCol = col + colOffsets[i];

                if (IsValidMove(matrix, king, newRow, newCol))
                {

                    PieceDto originalPiece = matrix[newRow][newCol];
                    matrix[newRow][newCol] = king;
                    matrix[row][col] = null;

                    bool isKingInCheck = IsKingInCheck(matrix, king, newRow, newCol);

                    matrix[row][col] = king;
                    matrix[newRow][newCol] = originalPiece;

                    if (!isKingInCheck)
                    {
                        legalMoves.Add(new Cell(newRow, newCol));
                    }
                }
            }

            return legalMoves.ToArray();
        }


        private bool IsValidMove(List<List<PieceDto>> matrix, PieceDto king, int row, int col)
        {
            if (row < 0 || row >= matrix.Count || col < 0 || col >= matrix[row].Count)
            {
                return false;
            }

            if ((matrix[row][col] != null && matrix[row][col].Color == king.Color))
            {
                return false;
            }

            return true;
        }


        #endregion



        #region King Is In Check, FindKing


        private PieceDto FindKing(List<List<PieceDto>> matrix, PieceColor color)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    if (matrix[i][j] != null && matrix[i][j].PieceName == PieceName.King && matrix[i][j].Color == color)
                    {
                        var kingCell = new CellDto();
                        kingCell.Col = j;
                        kingCell.Row = i;

                        matrix[i][j].Position = kingCell;

                        return matrix[i][j];
                    }
                }
            }

            return null;
        }


        private bool IsKingInCheck(List<List<PieceDto>> matrix, PieceDto king, int kingRow, int kingCol)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    if (matrix[i][j] != null && matrix[i][j].Color != king.Color)
                    {
                        var cell = new CellDto();
                        cell.Col = j;
                        cell.Row = i;
                        matrix[i][j].Position = cell;

                        if (IsPieceThreateningKing(matrix[i][j], kingRow, kingCol, matrix))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        private bool IsPieceThreateningKing(PieceDto piece, int kingRow, int kingCol, List<List<PieceDto>> matrix)
        {
            switch (piece.PieceName)
            {
                case PieceName.Pawn:
                    if (IsPawnThreateningKing(piece, kingRow, kingCol))
                    {
                        return true;
                    }
                    break;
                case PieceName.Rook:
                    if (IsRookThreateningKing(piece, kingRow, kingCol, matrix))
                    {
                        return true;
                    }
                    break;
                case PieceName.Bishop:
                    if (IsBishopThreateningKing(piece, kingRow, kingCol, matrix))
                    {
                        return true;
                    }
                    break;
                case PieceName.Queen:
                    if (IsQueenThreateningKing(piece, kingRow, kingCol, matrix))
                    {
                        return true;
                    }
                    break;
                case PieceName.Knight:
                    if (IsKnightThreateningKing(piece, kingRow, kingCol))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }


        private bool IsQueenThreateningKing(PieceDto queen, int kingRow, int kingCol, List<List<PieceDto>> matrix)
        {
            if (IsRookThreateningKing(queen, kingRow, kingCol, matrix) || IsBishopThreateningKing(queen, kingRow, kingCol, matrix))
            {
                return true;
            }

            return false;
        }


        private bool IsKnightThreateningKing(PieceDto knight, int kingRow, int kingCol)
        {
            int knightRow = knight.Position.Row;
            int knightCol = knight.Position.Col;

            int[] rowOffsets = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] colOffsets = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int targetRow = knightRow + rowOffsets[i];
                int targetCol = knightCol + colOffsets[i];

                if (targetRow == kingRow && targetCol == kingCol)
                {
                    return true;
                }
            }

            return false;
        }


        private bool IsPawnThreateningKing(PieceDto pawn, int kingRow, int kingCol)
        {
            int pawnRow = pawn.Position.Row;
            int pawnCol = pawn.Position.Col;

            int captureDirection = (pawn.Color == PieceColor.White) ? 1 : -1;

            if (pawnRow + captureDirection == kingRow)
            {
                if (pawnCol - 1 == kingCol || pawnCol + 1 == kingCol)
                {
                    return true;
                }
            }

            return false;
        }


        private bool IsRookThreateningKing(PieceDto rook, int kingRow, int kingCol, List<List<PieceDto>> matrix)
        {
            int rookRow = rook.Position.Row;
            int rookCol = rook.Position.Col;

            if (rookRow == kingRow || rookCol == kingCol)
            {
                int startRow = Math.Min(rookRow, kingRow) + 1;
                int endRow = Math.Max(rookRow, kingRow);
                int startCol = Math.Min(rookCol, kingCol) + 1;
                int endCol = Math.Max(rookCol, kingCol);

                if (rookRow == kingRow)
                {
                    for (int col = startCol; col < endCol; col++)
                    {
                        if (matrix[rookRow][col] != null)
                        {
                            return false;
                        }
                    }
                }
                else if (rookCol == kingCol)
                {
                    for (int row = startRow; row < endRow; row++)
                    {
                        if (matrix[row][rookCol] != null)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }


        private bool IsBishopThreateningKing(PieceDto bishop, int kingRow, int kingCol, List<List<PieceDto>> matrix)
        {
            int bishopRow = bishop.Position.Row;
            int bishopCol = bishop.Position.Col;

            if (Math.Abs(bishopRow - kingRow) == Math.Abs(bishopCol - kingCol))
            {
                int rowDirection = (kingRow < bishopRow) ? -1 : 1;
                int colDirection = (kingCol < bishopCol) ? -1 : 1;

                int row = bishopRow + rowDirection;
                int col = bishopCol + colDirection;

                while (row != kingRow && col != kingCol)
                {
                    if (matrix[row][col] != null)
                    {
                        return false;
                    }

                    row += rowDirection;
                    col += colDirection;
                }

                return true;
            }

            return false;
        }


        #endregion

    }
}
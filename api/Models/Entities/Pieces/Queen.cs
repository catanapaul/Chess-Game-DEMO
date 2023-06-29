namespace api.Models.Entities.Pieces
{
    public class Queen : Piece
    {
        
        public Queen(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♕" : "♛", PieceName.Queen, canMove)
        {
        }
    }
}
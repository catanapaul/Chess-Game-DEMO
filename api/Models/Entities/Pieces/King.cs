namespace api.Models.Entities.Pieces
{
    public class King : Piece
    {
        public King(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♔" : "♚", PieceName.King, canMove)
        {
        }
    }
}
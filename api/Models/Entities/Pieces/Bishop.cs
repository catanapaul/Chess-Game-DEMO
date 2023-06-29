namespace api.Models.Entities.Pieces
{
    public class Bishop : Piece
    {

        public Bishop(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♗" : "♝", PieceName.Bishop, canMove)
        {
        }
    }
}
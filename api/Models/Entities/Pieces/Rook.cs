namespace api.Models.Entities.Pieces
{
    public class Rook : Piece
    {
        
         public Rook(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♖" : "♜", PieceName.Rook, canMove)
        {
        }
    }
}
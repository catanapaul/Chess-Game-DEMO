namespace api.Models.Entities.Pieces
{
    public class Pawn : Piece
    {
        
        public Pawn(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♙" : "♟", PieceName.Pawn, canMove)
        {
        }
    }
}
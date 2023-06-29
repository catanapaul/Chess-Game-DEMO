namespace api.Models.Entities.Pieces
{
    public class Knight : Piece
    {
        
        public Knight(PieceColor color, PieceName pieceName, bool canMove) : base(color, color == PieceColor.White ? "♘" : "♞", PieceName.Knight, canMove)
        {
        }
    }
}
namespace api.Models.Entities
{
    public abstract class Piece
    {
        public PieceName PieceName { get; set; }
        public PieceColor Color { get; set; }
        public bool Moved { get; set; }
        public string Icon { get; set; }
        public Cell Position { get; set; }
        public Cell[] LegalMoves { get; set; }
        public bool CanMove { get; set; }
        public Piece (PieceColor color, string icon, PieceName pieceName, bool canMove)
        {
            Color = color;
            Icon = icon;
            PieceName = pieceName;
            Moved = false;
            CanMove = canMove;
        }
    }
}
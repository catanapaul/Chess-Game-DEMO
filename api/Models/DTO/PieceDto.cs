using api.Models.Entities;

namespace api.Models.DTO
{
    public class PieceDto
    {
         public PieceName PieceName { get; set; }
        public PieceColor Color { get; set; }
        public bool Moved { get; set; }
        public string Icon { get; set; }
        public CellDto Position { get; set; }
        public CellDto[] LegalMoves { get; set; }
        public bool CanMove { get; set; }
        
    }
}
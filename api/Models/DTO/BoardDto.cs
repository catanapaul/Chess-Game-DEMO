using System.Collections.Generic;

namespace api.Models.DTO
{
    public class BoardDto
    {
        public List<List<PieceDto>> Matrix { get; set; }
        public List<PieceDto> WhitePieces { get; set; }
        public List<PieceDto> BlackPieces { get; set; }
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public int Turn { get; set; }
        public int Winner { get; set; }
        public PieceDto SelectedPiece { get; set; }
    }
}
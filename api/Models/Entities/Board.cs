using System.Collections.Generic;

namespace api.Models.Entities
{
    public class Board
    {
        public List<List<Piece>> Matrix { get; set; }
        public List<Piece> WhitePieces { get; set; }
        public List<Piece> BlackPieces { get; set; }
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public int Turn { get; set; }
        public int Winner { get; set; }
        public Piece SelectedPiece { get; set; }

    }
}
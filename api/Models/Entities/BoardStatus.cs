using System.Collections.Generic;

namespace api.Models.Entities
{
    public class BoardStatus
    {
        public Piece selecctedPiece { get; set; }
        public List<List<Piece>> Board { get; set; }
    }
}
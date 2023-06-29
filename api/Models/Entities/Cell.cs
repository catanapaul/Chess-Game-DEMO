namespace api.Models.Entities
{
    public class Cell
    {

        public Cell(int captureRow1, int captureCol1)
        {
            Row = captureRow1;
            Col = captureCol1;
        }

        public int Row{ get; set; }
        public int Col { get; set; }
    }
}
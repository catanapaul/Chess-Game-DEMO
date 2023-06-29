using System;

namespace api.Models.DTO
{
    public class GameDto
    {
        public int Id { get; set; }
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public string Winner { get; set; }
        public DateTime Date { get; set; }
    }
}
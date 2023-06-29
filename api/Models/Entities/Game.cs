using System;

namespace api.Models.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public string Winner { get; set; }
        public DateTime Date { get; set; }
    }
}
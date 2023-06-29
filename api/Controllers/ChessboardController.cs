using api.Models.DTO;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/chessboard")]
    public class ChessboardController : ControllerBase
    {
        private readonly GameService _gameService;
        public ChessboardController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public ActionResult<Board> GetChessboard()
        {
            var chessboard = _gameService.getInitMatrix();

            return Ok(chessboard);
        }

        
        [HttpPost("legal-moves")]
        public ActionResult<Cell[]> GetLegalMoves(BoardDto board)
        {
            var result = _gameService.GetLegalMoves(board);
            
            return Ok(result);
        }

           
        [HttpPost("verify-check-mate")]
        public ActionResult<bool> VerifyCheckMate(BoardDto board)
        {
            var result = _gameService.VerifyIfCheckMatt(board);
            
            return Ok(result);
        }

    }
}
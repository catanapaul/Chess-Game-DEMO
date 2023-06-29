using api.Models.DTO;
using api.Models.Entities;
using AutoMapper;

namespace api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<GameDto, Game>();
            CreateMap<Game, GameDto>();

            CreateMap<Piece, PieceDto>();
            CreateMap<PieceDto, Piece>();
            CreateMap<Board, BoardDto>();
            CreateMap<BoardDto, Board>();
        }
    }
}
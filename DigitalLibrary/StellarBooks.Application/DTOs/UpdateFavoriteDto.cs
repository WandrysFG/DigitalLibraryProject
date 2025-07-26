namespace StellarBooks.Applications.DTOs
{
    public class UpdateFavoriteDto : CreateFavoriteDto
    {
        public int Id { get; set; }

        public UpdateUserDto? User { get; set; }
        public UpdateTaleDto? Tale { get; set; }
    }
}
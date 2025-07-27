namespace StellarBooks.Applications.DTOs
{
    public class UpdateTaleDto : CreateTaleDto
    {
        public int Id { get; set; }

        public List<UpdateActivityDto>? Activities { get; set; }
        public List<UpdateFavoriteDto>? Favorites { get; set; }
    }
}

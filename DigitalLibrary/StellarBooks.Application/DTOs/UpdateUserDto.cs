namespace StellarBooks.Applications.DTOs
{
    public class UpdateUserDto : CreateUserDto
    {
        public int Id { get; set; }

        public List<UpdateFavoriteDto>? Favorites { get; set; }
    }
}
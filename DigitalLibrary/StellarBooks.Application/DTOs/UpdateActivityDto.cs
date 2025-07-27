namespace StellarBooks.Applications.DTOs
{
    public class UpdateActivityDto : CreateActivityDto
    {
        public int Id { get; set; }

        public UpdateTaleDto? Tale { get; set; }
    }
}

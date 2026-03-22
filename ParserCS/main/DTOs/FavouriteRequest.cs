namespace main.DTOs
{
    public record FavouriteRequestDto
    {
        public long UserId { get; set; }
        public int ProductId { get; set; }
    }
}

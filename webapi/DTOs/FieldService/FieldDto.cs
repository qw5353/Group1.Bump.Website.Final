using webapi.Models;

namespace webapi.DTOs.FieldService
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        public string? Thumbnail { get; set; }
        public string? ShortDescription { get; set; }
        public string? Name { get; set; }
    }
    public static class FieldExts
    {
        public static FieldDto ToFieldDto( this Field db)
        {
            return new FieldDto
            {
                Id = db.Id,
                Address = db.Address,
                Thumbnail = db.Thumbnail,
                ShortDescription = db.ShortDescription,
                Name = db.Name
            };
        }
    }


}

namespace Igor_AIS_Proj.Models
{
    public class UploadResult : Entity
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? ContentType { get; set; }
        public int UserId { get; set; }
    }
}

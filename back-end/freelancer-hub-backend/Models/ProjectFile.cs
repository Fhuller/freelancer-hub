namespace freelancer_hub_backend.Models
{
    public class ProjectFile
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid FileId { get; set; }
    }
}

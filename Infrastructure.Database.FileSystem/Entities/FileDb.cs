namespace Infrastructure.Database.FileSystem.Entities
{
    public class FileDb
    {
        public int Id { get; set; }
        public int? PathId { get; set; }
        public PathDb? Path { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}

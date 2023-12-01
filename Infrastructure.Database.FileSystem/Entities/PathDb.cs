namespace Infrastructure.Database.FileSystem.Entities
{
    public class PathDb
    {
        public int Id { get; set; }
        public string FolderPath { get; set; } = string.Empty;
        public ICollection<FileDb> Files { get; } = new List<FileDb>();
    }
}

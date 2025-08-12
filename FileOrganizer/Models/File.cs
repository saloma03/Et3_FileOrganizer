namespace FileOrganizer.Models
{
    public class FileModel
    {
        public string Name { get; set; } = default!;
        public string Extension { get; set; } = default!;

        public string Path { get; set; } = default!;

        public string OriginalPath { get; set; } = default!;
    }
}

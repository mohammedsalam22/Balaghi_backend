namespace Application.DTOs;
public class UploadedFileDto
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
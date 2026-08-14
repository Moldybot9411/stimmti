namespace Backend.Dto;

public class PaginatedFolderListDto
{
    public required int FolderCount { get; set; }
    public List<GetFolderResponseDto> folderListInfo { get; set; } = new();
}
namespace MazadZone.Application.Features.Users.Queries.ExportUsers;

public record ExportFileResponse(byte[] FileContents, string ContentType, string FileName);
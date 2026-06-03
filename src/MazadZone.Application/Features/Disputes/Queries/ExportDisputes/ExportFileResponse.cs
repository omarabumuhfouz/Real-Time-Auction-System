namespace MazadZone.Application.Features.Disputes.Queries.ExportDisputes;


public record ExportFileResponse(byte[] FileContents, string ContentType, string FileName);

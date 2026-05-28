namespace MazadZone.Application.Features.Disputes.Queries.ExportSelectedDisputes;

using System;
using System.Collections.Generic;
using MazadZone.Application.Features.Users.Queries.ExportUsers; 

public record ExportSelectedDisputesQuery(List<Guid> SelectedDisputeIds) : IQuery<ExportFileResponse>;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Queries.GetMyNotifications;

public class GetMyNotificationsQuery : IRequest<PagedList<NotificationDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

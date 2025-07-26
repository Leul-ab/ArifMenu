using ArifMenu.Application.DTOs;

public interface IQrLinkService
{
    Task<CreateOrRegenerateQrLinkResponse> GenerateOrGetQrLinkAsync(Guid merchantUserId);
    Task<List<PublicMenuResponse>> GetPublicMenuBySlugAsync(string slug);
}


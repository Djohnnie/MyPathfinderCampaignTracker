using System.Collections.Concurrent;

namespace MyPathfinderCampaignTracker.Web.Services;

public record LoginTicketData(Guid UserId, string Username, string Token, bool IsAdmin, bool IsDarkMode, DateTime ExpiresAt);

public class LoginTicketService
{
    private readonly ConcurrentDictionary<string, LoginTicketData> _tickets = new();

    public string CreateTicket(Guid userId, string username, string token, bool isAdmin, bool isDarkMode)
    {
        var ticketId = Guid.NewGuid().ToString("N");
        _tickets[ticketId] = new LoginTicketData(userId, username, token, isAdmin, isDarkMode, DateTime.UtcNow.AddMinutes(2));
        return ticketId;
    }

    public LoginTicketData? RedeemTicket(string ticketId)
    {
        if (_tickets.TryRemove(ticketId, out var ticket) && ticket.ExpiresAt > DateTime.UtcNow)
            return ticket;
        return null;
    }
}

using System.ComponentModel.Design;

namespace Together.DataAccess.Entities;

public class UserEventRequest
{
    public int UserEventRequestId { get; set; }
    public int UserEventId { get; set; }
    public string OwnerUserId { get; set; }
    public string GuestUserId { get; set; }
    public int EventRequestStatusId { get; set; }
    public DateTime RequestDate { get; set; }
    public virtual UserEvent UserEvent { get; set; }
    public virtual UserInfo GuestUserInfo { get; set; }
}
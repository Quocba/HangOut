using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangOut.Domain.Payload.Response.Business
{
    public class GetAllBusinessResponse
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; }
        public string MainImage { get; set; }
        public DayOfWeek? StartDay { get; set; }
        public DayOfWeek? EndDay { get; set; }

        public string OpeningHours { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string? Description { get; set; }

        public string CategoryName { get; set; }
        public string CategoryIcon {  get; set; }
        public int TotalLike {  get; set; }
        public double Rating { get; set; } = 0;
        public List<EventsOfBusinessResponse> EventsOfBusiness { get; set; } = new List<EventsOfBusinessResponse>();
        public List<UserFavoriteBusiness> userFavorite { get; set; } = new List<UserFavoriteBusiness>();
    }

    public class UserFavoriteBusiness
    {
        public Guid AccountId { get; set; }
        public Guid BusinessId { get; set; }
    }
    public class EventsOfBusinessResponse
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }

        public string MainImage { get; set; }
    }
}

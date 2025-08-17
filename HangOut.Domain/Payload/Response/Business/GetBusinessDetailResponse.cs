using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangOut.Domain.Payload.Response.Image;

namespace HangOut.Domain.Payload.Response.Business
{
    public class GetBusinessDetailResponse
    {
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string? Vibe { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string? Description { get; set; }
        public string? MainImageUrl { get; set; }
        public string? OpeningHours { get; set; }
        public DayOfWeek? StartDay { get; set; }
        public DayOfWeek? EndDay { get; set; }
        public int TotalLike { get; set; }

        public string CategoryIcon {  get; set; }
        public string Category {  get; set; }
        public List<EventsResponse> Events { get; set; } = new List<EventsResponse>();
        public List<ImagesResponse> Images { get; set; } = new List<ImagesResponse>();
        public List<UserFavoriteBusiness> UserFavoriteBusinesses { get; set; } = new List<UserFavoriteBusiness>();

    }
}

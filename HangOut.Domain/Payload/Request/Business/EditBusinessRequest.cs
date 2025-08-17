using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HangOut.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HangOut.Domain.Payload.Request.Business
{
    public class EditBusinessRequest
    {
        public string? Name { get; set; }

        public string? Vibe {  get; set; }

        public string? Latidue {  get; set; }
        public string? Lontidue {  get; set; }

        public string? Address {  get; set; }

        public string? Province {  get; set; }

        public string? Description {  get; set; }

        public IFormFile? MainImage { get; set; }
        public string? OpeningHours {  get; set; }
        public DayOfWeek? StartDay { get; set; }
        public DayOfWeek? EndDay { get;set; }
        public Guid? CategoryId { get; set; }
    }
}

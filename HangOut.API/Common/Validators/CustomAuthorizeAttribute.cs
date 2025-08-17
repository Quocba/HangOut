using HangOut.API.Common.Utils;
using HangOut.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HangOut.API.Common.Validators;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute(params ERoleEnum[] roleEnums)
    {
        var allowedRolesAsString = roleEnums.Select(x => x.GetDescriptionFromEnum());
        Roles = string.Join(",", allowedRolesAsString);
    }
}
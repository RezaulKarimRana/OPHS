﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.Managers.Contracts;

namespace Web.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-authorize")]
    [HtmlTargetElement(Attributes = "asp-permission")]
    [HtmlTargetElement(Attributes = "asp-permissions")]
    [HtmlTargetElement(Attributes = "asp-permission-group")]
    public class AuthorizationTagHelper : TagHelper
    {
        private readonly ISessionManager _sessionManager;

        public AuthorizationTagHelper(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            Permissions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the permission name that determines access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permission")]
        public string? Permission { get; set; }

        /// <summary>
        /// Gets or sets the permission name that determines access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permissions")]
        public List<string> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the role group name that determins access to the HTML block.
        /// </summary>
        [HtmlAttributeName("asp-permission-group")]
        public string? PermissionGroup { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var permissions = await _sessionManager.GetPermissions();
            if (permissions == null)
            {
                output.SuppressOutput();
                return;
            }

            // permission
            if (!string.IsNullOrEmpty(Permission) &&
                !permissions.Select(p => p.Key).Contains(Permission))
            {
                output.SuppressOutput();
                return;
            }

            // list of permissions
            if (Permissions != null && Permissions.Any() &&
                !permissions.Select(p => p.Key).Any(p => Permissions.Contains(p)))
            {
                output.SuppressOutput();
                return;
            }

            // permission group
            if (!string.IsNullOrEmpty(PermissionGroup) &&
                !permissions.Any(p => p.Group_Name.Equals(PermissionGroup)))
            {
                output.SuppressOutput();
                return;
            }
        }
    }
}

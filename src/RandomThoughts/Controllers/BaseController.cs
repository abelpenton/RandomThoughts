﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RandomThoughts.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        ///     Gets or sets the Id of the current user.
        /// </summary>
        public string CurrentUserId { get; set; }

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            CurrentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
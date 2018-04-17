﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RandomThoughts.DataAccess.Repositories.ThoughtHoles;
using RandomThoughts.DataAccess.Repositories.Thoughts;
using RandomThoughts.Domain;
using RandomThoughts.Models.ThoughtHoleViewModels;
using RandomThoughts.Models.ThoughtViewModels;

namespace RandomThoughts.Controllers
{
    public class ThoughtHolesController : BaseController
    {
        private readonly IThoughtHolesRepository _thoughtHolesRepository;
        private readonly IMapper _mapper;
        private readonly IThoughtsRepository _thoughtsRepository;

        public ThoughtHolesController(IHttpContextAccessor httpContextAccessor,
            IThoughtHolesRepository thoughtHolesRepository,
            IMapper mapper,
            IThoughtsRepository thoughtsRepository) : base(httpContextAccessor)
        {
            _thoughtHolesRepository = thoughtHolesRepository;
            _mapper = mapper;
            _thoughtsRepository = thoughtsRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Public Holes";//TODO: Send the amount of thoughts related with each hole
            var thoughtHolesVM = _thoughtHolesRepository.
                ReadAll(_ => true).
                Select(th => new ThoughtHoleIndexViewModel
                {
                    Name = th.Name,
                    Description = th.Description.Length > 50? th.Description.Substring(0, 50) + "..." : th.Description,
                    AmountOfThought = th.Thoughts.Count(),
                    Likes = th.Likes,
                    Views = th.Views,
                    Id = th.Id
                }).
                ToList();

            return View(thoughtHolesVM);
        }

        [HttpGet]
        public IActionResult MyHoles()
        {
            ViewData["Title"] = "My Holes";//TODO: Send the amount of thoughts related with each hole
            var thoughtHolesVM = _thoughtHolesRepository.
                ReadAll(th => th.CreatedBy == this.CurrentUserId).
                Select(th => new ThoughtHoleIndexViewModel
                {
                    Name = th.Name,
                    Description = th.Description.Length > 50 ? th.Description.Substring(0, 50) + "..." : th.Description,
                    AmountOfThought = th.Thoughts.Count(),
                    Likes = th.Likes,
                    Views = th.Views
                }).
                ToList();

            return View("Index" ,thoughtHolesVM);
        }

        /// <summary>
        ///     Returns all the thoughts that belongs to the Hole
        ///     with the given <paramref name="holeId"/>.
        /// </summary>
        /// <returns></returns>
        public IActionResult HoleThoughts(int holeId)
        {
            return RedirectToAction("HoleThoughts", "Thoughts", new { holeId = holeId });
        
        }


    }
}
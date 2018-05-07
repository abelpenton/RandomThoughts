using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RandomThoughts.Business.ApplicationServices.Thoughts;
using RandomThoughts.DataAccess.Repositories.Thoughts;
using RandomThoughts.Domain;
using RandomThoughts.Models.ThoughtViewModels;
using StackExchange.Redis;

namespace RandomThoughts.Controllers
{
    public class ThoughtsController : BaseController
    {
        private readonly IThoughtsApplicationService _thoughtsAppService;
        private readonly IMapper _mapper;

        public ThoughtsController(IHttpContextAccessor httpContextAccessor,
            IThoughtsApplicationService thoughtsAppService,
            IMapper mapper) : base(httpContextAccessor)
        {
            _thoughtsAppService = thoughtsAppService;
            _mapper = mapper;
            //add the value of the default HOle
            ViewData["HoleId"] = 1;
        }

        /// <summary>
        ///     Returns all the thoughts that belongs to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "My Thoughts";
            ViewData["PersonalThoughts"] = true;
            ViewData["MainTitle"] = "My Thoughts";
            var userThoughts = _thoughtsAppService.ReadAllLimit(thought => thought.ApplicationUserId == this.CurrentUserId,10).ToList();

            var userThoughtsVM = _mapper.Map<IEnumerable<Thought>, IEnumerable<ThoughtIndexViewModel>>(userThoughts);

            return View(userThoughtsVM);
        }

        /// <summary>
        ///     Returns all the thoughts that belongs to the current user.
        /// </summary>
        /// <returns></returns>
        public IActionResult PublicThoughts()
        {
            ViewData["Title"] = "Public Thoughts";
            ViewData["MainTitle"] = "Public Thoughts";
            ViewData["PersonalThoughts"] = false;

            var userThoughts = _thoughtsAppService.ReadAllLimit(_ => true,10).ToList();

            var userThoughtsVM = _mapper.Map<IEnumerable<Thought>, IEnumerable<ThoughtIndexViewModel>>(userThoughts);

            return View("Index", userThoughtsVM);
        }

        /// <summary>
        ///     Returns all the thoughts that belongs to the Hole
        ///     with the given <paramref name="holeId"/>.
        /// </summary>
        /// <returns></returns>
        public IActionResult HoleThoughts(int holeId, string holeName)
        {
            ViewData["Title"] = "Public Thoughts";
            ViewData["PersonalThoughts"] = false;
            ViewData["HoleId"] = holeId;
            ViewData["MainTitle"] =$"{holeName}'s Thoughts";
            

            //var holeThoughts = _thoughtsRepository.ReadAll(thought =>
            //{
            //    return thought.ThoughtHoleId == holeId;
            //}).ToList();

            var holeThoughts = _thoughtsAppService.ReadAll(e=>e.ThoughtHoleId == holeId).ToList();
            

            var holeThoughtsVM = _mapper.Map<IEnumerable<Thought>, IEnumerable<ThoughtIndexViewModel>>(holeThoughts);

            return View("Index", holeThoughtsVM);
        }


    }
}
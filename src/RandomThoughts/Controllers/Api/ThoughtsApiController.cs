using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RandomThoughts.Business.ApplicationServices.Thoughts;
using RandomThoughts.DataAccess.Repositories.Thoughts;
using RandomThoughts.Domain;
using RandomThoughts.Models.CommentViewModel;
using RandomThoughts.Models.ThoughtViewModels;
using RandomThoughts.Business.ApplicationServices.ThoughtComment;

namespace RandomThoughts.Controllers.Api
{
    [Produces("application/json")]
    public class ThoughtsController : CommentApiController
    {
        private readonly IThoughtsApplicationService _thoughtsApplicationService;
        private readonly IMapper _mapper;
        private readonly IThoughtCommentApplicationService _thoughtCommentApplicationService;

        public ThoughtsController(IHttpContextAccessor httpContextAccessor,
            IThoughtsApplicationService thoughtsApplicationService,
            IThoughtCommentApplicationService thoughtCommentApplicationService,
            IMapper mapper) : base(httpContextAccessor, thoughtCommentApplicationService, mapper)
        {
            _thoughtsApplicationService = thoughtsApplicationService;
            _mapper = mapper;
            _thoughtCommentApplicationService = thoughtCommentApplicationService;
        }

        // GET: api/Thoughts
        /// <summary>
        ///     Get the <see cref="Thought"/> related to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ThoughtIndexViewModel> GetUserThoughts()
        {
            var userThoughts = _thoughtsApplicationService.ReadAllLimit(thought => thought.ApplicationUserId == CurrentUserId, 10).ToList();

            var userThoughtsVM = _mapper.Map<IEnumerable<Thought>, IEnumerable<ThoughtIndexViewModel>>(userThoughts);

            return userThoughtsVM;
        }

    // GET: api/Thoughts/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var thought = _thoughtsApplicationService.SingleOrDefault(id);

            if (thought != null)
                return Ok(_mapper.Map<Thought, ThoughtIndexViewModel>(thought));

            return NotFound(id);
        }
        
        // POST: api/Thoughts
        [HttpPost]
        public IActionResult Post([FromBody]ThoughtCreateViewModel newThought)
        {
            if (ModelState.IsValid)
            {
                var thought = _mapper.Map<ThoughtCreateViewModel, Thought>(newThought);

                thought.CreatedBy = this.CurrentUserId;
                thought.ApplicationUserId = this.CurrentUserId;

                var createdThought = _thoughtsApplicationService.Add(thought);//TODO: add the auditable and trackable values!!!

                try
                {
                    _thoughtsApplicationService.SaveChanges();
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }

                return Created("", createdThought);
            }

            return BadRequest(ModelState);

        }
        
        // PUT: api/Thoughts/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ThoughtEditViewModel editedThought)
        {
            if (id != editedThought.Id)
            {
                ModelState.AddModelError("Id", "The given Id of the edited model doesn't match with the route Id");
                return BadRequest(ModelState);
            }
            if (!_thoughtsApplicationService.Exists(id))
                return NotFound(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var originalThought = _thoughtsApplicationService.SingleOrDefault(id);
            originalThought.Title = editedThought.Title;
            originalThought.Body = editedThought.Body;
            originalThought.Mood = editedThought.Mood;

            originalThought.ModifiedBy = this.CurrentUserId;

            _thoughtsApplicationService.Update(originalThought);


            try
            {
                _thoughtsApplicationService.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return Ok(originalThought);
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_thoughtsApplicationService.Exists(id))
                return NotFound(id);

            _thoughtsApplicationService.Remove(id);

            try
            {
                _thoughtsApplicationService.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return StatusCode(204);
        }

        
    }
}

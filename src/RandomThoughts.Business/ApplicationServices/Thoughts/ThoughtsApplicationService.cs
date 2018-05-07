using System;
using System.Collections.Generic;
using System.Text;
using RandomThoughts.Business.Base;
using RandomThoughts.DataAccess.Repositories.Base;
using RandomThoughts.DataAccess.Repositories.Thoughts;
using RandomThoughts.Domain;
using System.Linq;

namespace RandomThoughts.Business.ApplicationServices.Thoughts
{
    public class ThoughtsApplicationService : BaseApplicationService<Thought, int>, IThoughtsApplicationService
    {
        /// <summary>
        /// <para>
        ///     Contains the implementation of the  necessary functionalities
        ///     to handle the operations on the <see cref="Thought"/> entity.
        /// </para>
        /// <remarks>
        ///     This object handle the data of the <see cref="Thought"/> entity
        ///     through the <see cref="IThoughtsRepository"/> but when necessary
        ///     add some operations on the data before pass it to the DataAcces layer
        ///     or to the Presentation layer
        /// </remarks>
        /// </summary>
        public ThoughtsApplicationService(IThoughtsRepository repository) : base(repository)
        {
        }

        public IEnumerable<Thought> ReadAllLimit(Func<Thought,bool> filter,int limit)
        {
            var result = (this.ReadAll(filter) as IQueryable<Thought>).Select(
                thought => new Thought
                {
                    ApplicationUser = thought.ApplicationUser,
                    Body = thought.Body.Length > limit ? thought.Body.Substring(0, limit) + "..." : thought.Body,
                    Comments = thought.Comments,
                    CreatedAt = thought.CreatedAt,
                    Id = thought.Id,
                    Likes = thought.Likes,
                    Mood = thought.Mood,
                    Title = thought.Title,
                    ThoughtHole = thought.ThoughtHole,
                    ThoughtHoleId = thought.ThoughtHoleId,
                    ModifiedAt = thought.ModifiedAt,
                    Views = thought.Views,
                    ApplicationUserId = thought.ApplicationUserId,
                    CreatedBy = thought.CreatedBy,
                    ModifiedBy = thought.ModifiedBy
                });

            return result;
           
        }
    }
}

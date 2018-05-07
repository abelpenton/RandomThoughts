using System;
using System.Collections.Generic;
using System.Text;
using RandomThoughts.Business.Base;
using RandomThoughts.DataAccess.Repositories.Thoughts;
using RandomThoughts.Domain;

namespace RandomThoughts.Business.ApplicationServices.Thoughts
{
    /// <summary>
    ///     <para>
    ///         Contains the declaration of the  necessary functionalities
    ///         to handle the operations on the <see cref="Thought" /> entity.
    ///     </para>
    ///     <remarks>
    ///         This object handle the data of the <see cref="Thought" /> entity
    ///         through the <see cref="IThoughtsRepository" /> but when necessary
    ///         adda some data or apply operations on the data before pass it to the DataAcces layer
    ///         or to the Presentation layer
    ///     </remarks>
    /// </summary>
    public interface IThoughtsApplicationService : IBaseApplicationService<Thought, int>
    {
        /// <summary>
        /// This is a functionality for read just a limit number
        /// of character in the body of Thought
        /// </summary>
        /// <returns></returns>
        IEnumerable<Thought> ReadAllLimit(Func<Thought, bool> filter, int limit);
    }
}

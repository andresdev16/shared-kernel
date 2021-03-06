using System;
using System.Linq;
using System.Linq.Expressions;
using SharedKernel.Domain.Entities.Globalization;

namespace SharedKernel.Domain.Specifications.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TTranslation"></typeparam>
    public class ExistsSpecification<TKey, TEntity, TTranslation> : Specification<TEntity>
        where TEntity : class, IEntityIsTranslatable<TKey, TEntity, TTranslation>
        where TTranslation : IEntityTranslated<TKey, TEntity>, IName

    {
        private readonly TKey _id;
        private readonly string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public ExistsSpecification(TKey id, string name)
        {
            _id = id;
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            return l => l.Translations.Any(t => !t.EntityId.Equals(_id) && t.Name.ToUpper() == _name.ToUpper());
        }
    }
}

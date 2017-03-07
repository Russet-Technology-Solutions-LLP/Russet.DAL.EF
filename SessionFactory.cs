using System.Configuration;
using System.ComponentModel.Composition;

namespace Russet.DAL.EF
{
    [Export(typeof(ISessionFactory))]
    public class SessionFactory : ISessionFactory
    {
        private IUnitOfWork uow;
        private object _dbContext;
        public SessionFactory(object dbContext)
        {
            this._dbContext = dbContext;
        }

        #region ISessionFactory Members

        /// <summary>
        /// Gets the current uo W.
        /// </summary>
        /// <value>The current uo W.</value>
        public IUnitOfWork CurrentUoW
        {
            get
            {
                if (uow == null)
                {
                    uow = GetUnitOfWork();
                }

                return uow;
            }
        }

        #endregion

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <returns></returns>
        private IUnitOfWork GetUnitOfWork()
        {
            var orm = _dbContext;
            return new UnitOfWork(orm);
        }
    }
}

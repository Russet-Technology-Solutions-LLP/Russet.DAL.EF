using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Transactions;

namespace Russet.DAL.EF
{
    [Export(typeof (IUnitOfWork))]
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope tx;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="orm">The orm.</param>
        public UnitOfWork(object orm)
        {
            this.Orm = (DbContext)orm;
        }

        #region IUnitOfWork Members

        public object Orm { get; private set; }


        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<T>(T entity) where T : class
        {
            try
            {
                ((DbContext) Orm).Set<T>().Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Add Entity.\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<T>(T entity) where T : class
        {
            try
            {
                ((DbContext)Orm).Set<T>().Attach(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Update Entity.\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<T>(T entity) where T : class
        {
            try
            {
                ((DbContext)Orm).Set<T>().Remove(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Delete Entity.\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                tx = new TransactionScope();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Begin transaction.\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                if (tx == null)
                {
                    throw new TransactionException("The current transaction is not started!");
                }
                ((DbContext) Orm).SaveChanges();
                tx.Complete();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Commit transaction.\r\n{0}", ex.Message));
            }
            finally
            {
                tx.Dispose();
            }
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (tx != null)
                {
                    tx.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured during the Rollback transaction.\r\n{0}", ex.Message));
            }
        }

        #endregion

        private static string EntitySetName<T>()
        {
            return String.Format(@"{0}s", typeof (T).Name);
        }
    }
}

using ExamReg.Authentication.Common;
using ExamReg.Authentication.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();
        IUserRepository UserRepository { get; }
    }
    public class UOW : IUOW
    {
        private ExamRegContext examRegContext;
        public IUserRepository UserRepository { get; }

        public UOW(ExamRegContext examRegContext)
        {
            this.examRegContext = examRegContext;
            UserRepository = new UserRepository(examRegContext);
        }

        public async Task Begin()
        {
            await examRegContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            examRegContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            examRegContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}

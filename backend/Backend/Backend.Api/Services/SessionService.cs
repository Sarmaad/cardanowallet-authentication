namespace Backend.Api.Services
{
    public interface ISessionService
    {
        Session GetOrCreateSession(string accountId);
    }
    public class SessionService : ISessionService
    {
        readonly IRepository _repository;

        public SessionService(IRepository repository)
        {
            _repository = repository;
        }
        public Session GetOrCreateSession(string accountId)
        {
            var session = _repository.Query<Session>()
                .SingleOrDefault(x => x.AccountId == accountId && !x.IsExpired);

            if (session is null)
            {
                session = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                    CreatedOn = DateTime.UtcNow
                };
                _repository.Insert(session);
            }

            return session;
        }
    }
}

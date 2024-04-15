namespace Backend.Api.Features.Login.Operations
{
    public class GenerateNonce : IRequest<GenerateNonceResponse>
    {
        public string PublicAddress { get; set; }
    }

    public class GenerateNonceResponse
    {
        public string Nonce { get; set; }
    }

    public class GenerateNonceResponseHandler : IRequestHandler<GenerateNonce, GenerateNonceResponse>
    {
        readonly IRepository _repository;
        readonly ISessionService _sessionService;

        public GenerateNonceResponseHandler(IRepository repository, ISessionService sessionService)
        {
            _repository = repository;
            _sessionService = sessionService;
        }

        public async Task<GenerateNonceResponse> Handle(GenerateNonce request, CancellationToken cancellationToken)
        {
            // check if the account exists
            var account = _repository.Query<Account>().SingleOrDefault(x => x.Address == request.PublicAddress);

            if (account is null)
            {
                // create a new account
                account = new()
                {
                    Address = request.PublicAddress,
                    Id = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.UtcNow
                };
                _repository.Insert(account);
            }

            // check if a session already exists for account
            var session = _sessionService.GetOrCreateSession(account.Id);

            return new() { Nonce = session.Id };
        }
    }
}

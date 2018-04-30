namespace TSQLLint.Core.Interfaces
{
    namespace Config.Contracts
    {
        public interface IRequestHandler<in TRequest, out TResponse> where TRequest : IRequest<TResponse>
        {
            TResponse Handle(TRequest message);
        }

        public interface IRequestHandler<in TRequest> where TRequest : IRequest
        {
            void Handle(TRequest message);
        }

        public interface IRequest
        {
        }

        public interface IRequest<out TResponse>
        {
        }
    }
}

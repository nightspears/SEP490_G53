using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class HelloWorldRepository : IHelloWorldRepository
    {
        public string GetString()
        {
            return "Hello World!";
        }
    }
}

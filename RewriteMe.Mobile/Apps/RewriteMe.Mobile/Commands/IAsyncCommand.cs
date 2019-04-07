using System.Threading.Tasks;
using System.Windows.Input;

namespace RewriteMe.Mobile.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();

        void ChangeCanExecute();
    }
}

using Blockchainsure.Shared.Integration.Messages.Commands;
using System.Threading.Tasks;

namespace Blockchainsure.PackageTracker.Application.Integration.Producers
{
    public interface IUpdatePackageStatusProducer
    {
        Task CreatePackageAsync(IUpdatePackageStatus updatePackageStatus);
    }
}
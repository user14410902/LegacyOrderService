using OrderService.Common;
using OrderService.Services.AddOrder;
using OrderService.Services.AddOrderCSV;

namespace OrderService.Services.Interfaces;

public interface IOrderCSVCreationService
{
  Task<Result<bool, List<string>>> ExecuteAsync(ICSVRowSource source, CancellationToken cancellationToken);


}
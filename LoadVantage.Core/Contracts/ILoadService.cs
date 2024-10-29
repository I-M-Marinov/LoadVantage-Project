using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadService
    {
        Task<Guid> CreateLoadAsync(LoadViewModel loadViewModel, Guid brokerId);
        Task<bool> PostLoadAsync(Guid loadId);
        Task<bool> EditLoadAsync(Guid loadId, LoadViewModel updatedLoadViewModel);
        Task<bool> DeleteLoadAsync(Guid loadId, Guid brokerId);
        Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId);
        Task<bool> LoadDeliveredAsync(Guid loadId);
        Task<bool> CancelLoadAsync(Guid loadId);
    }
}

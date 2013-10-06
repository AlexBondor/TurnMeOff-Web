using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnMeOff.Repository.Models;

namespace TurnMeOff.Repository
{
    public interface IRepository : IDisposable
    {
        void Dispose();

        bool GetUserMatchPassword(string email, string password);

        bool MatchPassword(Guid id, ChangePasswordModel model);

        User GetUserByEmail(string email);

        Device GetDeviceById(Guid id);

        void UpdateUserPassword(Guid id, ChangePasswordModel model);

        void AddUser(RegisterModel model);

        void EditUser(User model);

        void AddPendingDevice(PendingDevice model);

        void AddDevice(DeviceModel model, Guid userId);

        void RemoveDevice(Guid deviceId);

        void RemoveUser(Guid userId);

        void EditDevice(Device model);

        bool GetMasterDeviceById(string id);

        List<Device> GetAllDevicesByUserId(Guid userId);

        PendingDevice GetPendingDevice();

        void DeletePendingDevices();
    }
}

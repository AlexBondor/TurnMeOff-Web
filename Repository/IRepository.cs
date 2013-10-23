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

        string EncodePassword(string originalPassword);

        User GetUserByEmail(string email);

        bool GetUserMatchPassword(string email, string password);

        MasterDevice GetMasterDeviceById(string id);

        void RemoveMasterDeviceFromMasterDevices(string id);

        void AddUser(RegisterModel model);
        
        void EditUser(User model);

        void RemoveUser(Guid userId);

        List<Device> GetAllDevicesByUserId(Guid userId);

        Device GetDeviceById(Guid id);

        void DeletePendingDevices();

        void AddPendingDevice(PendingDevice model);

        //void AddPendingMasterDevice(PendingMasterDevices model);

        PendingDevice GetPendingDevice();

        void EnableDevice(Guid userId, Guid deviceId, bool isEnabled);

        void TurnOffAllDevices();

        void AddDevice(DeviceModel model, Guid userId);

        void EditDevice(Device model);

        void RemoveDevice(Guid deviceId);
    }
}

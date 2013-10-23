using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using TurnMeOff.Repository.Models;

namespace TurnMeOff.Repository
{
    public class Repository : IRepository
    {
        public Repository()
        {
            Entities = new turnoffdbEntities();
            IsDisposed = false;
        }

        protected turnoffdbEntities Entities
        {
            get;
            private set;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            lock (Entities)
            {
                if (!IsDisposed)
                {
                    Entities.Dispose();
                    IsDisposed = true;
                }
            }
        }

        public string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        public User GetUserByEmail(string email)
        {
            return (from u in Entities.Users
                    where u.Email == email
                    select u).FirstOrDefault();
        }

        public bool GetUserMatchPassword(string email, string password)
        {
            password = EncodePassword(password);
            return (from u in Entities.Users
                    where (u.Email == email && u.Password == password)
                    select u).FirstOrDefault() != null;

        }

        public MasterDevice GetMasterDeviceById(string id)
        {
            return (from u in Entities.MasterDevices
                    where u.masterDeviceID == id
                    select u).FirstOrDefault();

        }

        public void RemoveMasterDeviceFromMasterDevices(string id)
        {
            MasterDevice device = GetMasterDeviceById(id);
            Entities.MasterDevices.Remove(device);
            Entities.SaveChanges();
        }

        public void AddUser(RegisterModel model)
        {

            User user = new User
            {
                userID = Guid.NewGuid(),
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Password = EncodePassword(model.Password),
                masterDeviceID = model.MasterDeviceID,
            };

            Entities.Users.Add(user);
            Entities.SaveChanges();

        }

        public void EditUser(User model)
        {
            User user = (from u in Entities.Users
                         where u.userID == model.userID
                         select u).FirstOrDefault();
            if (user != null)
            {
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                if (user.Email != model.Email)
                {

                    if (GetUserByEmail(model.Email) == null)
                    {
                        FormsAuthentication.SignOut();
                        user.Email = model.Email;
                        FormsAuthentication.SetAuthCookie(user.Email, true);
                    }

                }
                if (model.Password != user.Password)
                {
                    user.Password = EncodePassword(model.Password);
                }
                else
                {
                    user.Password = model.Password;
                }
            }

            Entities.SaveChanges();


        }

        public void RemoveUser(Guid userId)
        {
            var dbDevicesList = GetAllDevicesByUserId(userId);
            foreach (var device in dbDevicesList)
            {
                RemoveDevice(device.deviceID);
            }
            User user = Entities.Users.Find(userId);
            Entities.Users.Remove(user);
            Entities.SaveChanges();
        }

        public List<Device> GetAllDevicesByUserId(Guid userId)
        {
            //Sensors firt then switches.. ordered by name
            var dbDevices = Entities.Devices.Where(d => d.userID == userId).OrderByDescending(d => d.isSensor).ThenBy(d => d.Name).Take(Entities.Devices.Count()).ToList();
            return dbDevices;
        }

        public Device GetDeviceById(Guid id)
        {
            return Entities.Devices.Find(id);
        }

        public void DeletePendingDevices()
        {
            /*
             * this table should always have one record in it 
             * if table is not empty then delete everything before adding another device
             */

            foreach (var item in Entities.PendingDevices)
                Entities.PendingDevices.Remove(item);
            Entities.SaveChanges();
        }

        public void AddPendingDevice(PendingDevice model)
        {
            DeletePendingDevices();

            PendingDevice device = new PendingDevice
            {
                deviceID = model.deviceID,
                userID = model.userID,
            };

            Entities.PendingDevices.Add(device);
            Entities.SaveChanges();
        }

        //public void AddPendingMasterDevice(PendingMasterDevices model)
        //{
        //    PendingMasterDevices masterDevice = new PendingMasterDevices
        //    {
        //        masterDeviceID = model.masterDeviceID,
        //    };
        //    Entities.PendingMasterDevices.Add(masterDevice);
        //    Entities.SaveChanges();
        //}

        public PendingDevice GetPendingDevice()
        {
            return (from d in Entities.PendingDevices
                    select d).FirstOrDefault();

        }

        public void AddDevice(DeviceModel model, Guid userId)
        {
            Device device = new Device
            {
                deviceID = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                userID = userId,
                isSensor = model.isSensor,
                isEnabled = model.isEnabled,
            };

            Entities.Devices.Add(device);
            Entities.SaveChanges();
        }

        public void EditDevice(Device model)
        {
            Entities.Entry(model).State = EntityState.Modified;
            Entities.SaveChanges();
        }

        public void AddToEnabledDevices(string userId, Guid deviceId)
        {
            try
            {
                EnabledDevice device = new EnabledDevice
                {
                    deviceID = deviceId,
                    userID = userId,
                };
                Entities.EnabledDevices.Add(device);
                Entities.SaveChanges();
            }
            catch { }
        }

        public void RemoveFromEnabledDevices(string userId, Guid deviceId)
        {
            try
            {
                EnabledDevice device = (from u in Entities.EnabledDevices
                                        where u.deviceID == deviceId && u.userID == userId
                                        select u).FirstOrDefault();
                Entities.EnabledDevices.Remove(device);
                Entities.SaveChanges();
            }
            catch { }

        }

        public void EnableDevice(Guid userId, Guid deviceId, bool isEnabled)
        {
            Device device = (from u in Entities.Devices
                             where u.deviceID == deviceId && u.userID == userId
                             select u).FirstOrDefault();
            if (device != null)
            {
                device.isEnabled = isEnabled;
            }
            Entities.SaveChanges();
            if (isEnabled)
            {
                AddToEnabledDevices(userId.ToString(), deviceId);
            }
            else
            {
                RemoveFromEnabledDevices(userId.ToString(), deviceId);
            }
        }

        public void TurnOffAllDevices()
        {
            foreach (var device in Entities.EnabledDevices)
            {
                Device d = (from u in Entities.Devices
                            where u.deviceID == device.deviceID
                            select u).FirstOrDefault();
                if (d != null)
                {
                    d.isEnabled = false;
                }
                RemoveFromEnabledDevices(device.userID, device.deviceID);
            }
            Entities.SaveChanges();
        }
        
        public void RemoveDevice(Guid deviceId)
        {
            Device device = Entities.Devices.Find(deviceId);
            Entities.Devices.Remove(device);
            Entities.SaveChanges();
        }
    }
}
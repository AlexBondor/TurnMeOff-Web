using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
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

        public bool GetUserMatchPassword(string email, string password)
        {
            password = EncodePassword(password);
            return (from u in Entities.Users
                    where (u.Email == email && u.Password == password)
                    select u).FirstOrDefault() != null;

        }

        public User GetUserByEmail(string email)
        {
            return (from u in Entities.Users
                    where u.Email == email
                    select u).FirstOrDefault();
        }

        public Device GetDeviceById(Guid id)
        {
            return Entities.Devices.Find(id);
        }

        public bool GetMasterDeviceById(string id)
        {
            return (from u in Entities.MasterDevices
                    where u.masterDeviceID == id
                    select u).FirstOrDefault() != null;

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
            User user = new User
            {
                userID = model.userID,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Password = model.Password,
            };

            Entities.Entry(user).State = EntityState.Modified;
            Entities.SaveChanges();


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

        public PendingDevice GetPendingDevice()
        {
            return (from d in Entities.PendingDevices
                    select d).FirstOrDefault();

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

        public void AddDevice(DeviceModel model, Guid userId)
        {
            Device device = new Device
            {
                deviceID = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                userID = userId,
                isSensor = model.isSensor,
                isOn = model.isOn,
            };

            Entities.Devices.Add(device);
            Entities.SaveChanges();
        }

        public void RemoveDevice(Guid deviceId)
        {
            Device device = Entities.Devices.Find(deviceId);
            Entities.Devices.Remove(device);
            Entities.SaveChanges();
        }

        public void EditDevice(Device model)
        {
            RemoveDevice(model.deviceID);
            Entities.SaveChanges();
            Entities.Devices.Add(model);
            Entities.SaveChanges();
        }

        public List<Device> GetAllDevicesByUserId(Guid userId)
        {
            var dbDevices = Entities.Devices.Where(d => d.userID == userId).OrderBy(d => d.Name).Take(Entities.Devices.Count()).ToList();
            return dbDevices;
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

        public bool MatchPassword(Guid id, ChangePasswordModel model)
        {
            User user = Entities.Users.Find(id);
            if (EncodePassword(model.OldPassword) == EncodePassword(user.Password))
            {
                return true;
            }
            return false;
        }

        public void UpdateUserPassword(Guid id, ChangePasswordModel model)
        {
            User user = Entities.Users.Find(id);
            user.Password = EncodePassword(model.Password);
            Entities.SaveChanges();
        }
    }
}
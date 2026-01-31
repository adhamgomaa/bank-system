using BankSystemDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemBusinessLayer
{
    public class clsUser
    {
        enum enMode { Add, Update }
        enMode mode = enMode.Add;

        public int registerId { get; private set; }
        public int userId { get; private set; }
        public int personId { get; set; }
        public clsPeople personInfo { get; private set; }
        public string username { get; set; }
        public string password { get; set; }
        public int permissions { get; set; }
        public bool isActive { get; set; }

        public clsUser()
        {
            registerId = -1;
            userId = -1;
            personId = -1;
            username = string.Empty;
            password = string.Empty;
            permissions = 0;
            isActive = false;
            mode = enMode.Add;
        }

        private clsUser(int userId, int personId, string username, string password, int permissions, bool isActive)
        {
            this.userId = userId;
            this.personId = personId;
            this.username = username;
            this.password = password;
            this.permissions = permissions;
            this.isActive = isActive;
            personInfo = clsPeople.FindPerson(personId);
            mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            this.userId = clsUserData.AddNewUser(personId, username, password, permissions, isActive);
            return this.userId != -1;
        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(userId, personId, username, password, permissions, isActive);
        }

        public static clsUser FindUser(int userId)
        {
            int personId = 0, permissions = 0;
            string username = string.Empty, password = string.Empty;
            bool isActive = false;
            if (clsUserData.FindUser(userId, ref personId, ref username, ref password, ref permissions, ref isActive))
            {
                return new clsUser(userId, personId, username, password, permissions, isActive);
            }
            return null;
        }

        public static clsUser FindUser(string username)
        {
            int userId = 0, personId = 0, permissions = 0;
            string password = string.Empty;
            bool isActive = false;
            if (clsUserData.FindUser(username, ref userId, ref personId, ref password, ref permissions, ref isActive))
            {
                return new clsUser(userId, personId, username, password, permissions, isActive);
            }
            return null;
        }

        public static clsUser FindUser(string username, string password)
        {
            int userId = 0, personId = 0, permissions = 0;
            bool isActive = false;
            if (clsUserData.FindUser(username, password, ref userId, ref personId, ref permissions, ref isActive))
            {
                return new clsUser(userId, personId, username, password, permissions, isActive);
            }
            return null;
        }

        public static bool DeleteUser(int userId)
        {
            return clsUserData.DeleteUser(userId);
        }

        public static DataView GetAllUsers()
        {
            return clsUserData.GetAllUsers().DefaultView;
        }

        public static bool IsUserExist(int userId)
        {
            return clsUserData.UserIsExist(userId);
        }

        public bool AddNewRegister()
        {
            this.registerId = clsUserData.AddNewRegister(userId);
            return this.registerId != -1;
        }

        public static DataView GetAllRegisters()
        {
            return clsUserData.GetAllRegisters().DefaultView;
        }

        public bool Save()
        {
            switch (mode)
            {
                case enMode.Add:
                    if (_AddNewUser())
                    {
                        mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }
    }
}

using BankSystemDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemBusinessLayer
{
    public class clsPeople
    {
        enum enMode { Add, Update}
        enMode mode = enMode.Add;

        public int personId { get; private set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public byte gender { get; set; }
        public DateTime birthDate { get; set; }
        public clsPeople()
        {
            personId = -1;
            firstName = string.Empty;
            secondName = string.Empty;
            lastName = string.Empty;
            email = string.Empty;
            phone = string.Empty;
            gender = 0;
            birthDate = DateTime.Now;
            mode = enMode.Add;
        }

        private clsPeople(int personId, string firstName, string secondName, string lastName, string email, string phone, byte gender,
            DateTime date)
        {
            this.personId = personId;
            this.firstName = firstName;
            this.secondName = secondName;
            this.lastName = lastName;
            this.email = email;
            this.phone = phone;
            this.gender = gender;
            this.birthDate = date;
            mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            this.personId = clsPeopleData.AddNewPerson(firstName, secondName, lastName, email, phone, gender, birthDate);
            return this.personId != -1;
        }

        private bool _UpdatePerson()
        {
            return clsPeopleData.UpdatePerson(personId, firstName, secondName, lastName, email, phone, gender, birthDate);
        }

        public static clsPeople FindPerson(int personId)
        {
            string fname = "", secName = "", lname = "", email = "", phone = "";
            byte gender = 0;
            DateTime date = DateTime.Now;
            if (clsPeopleData.FindPerson(personId, ref fname, ref secName, ref lname, ref email, ref phone, ref gender, ref date))
            {
                return new clsPeople(personId, fname, secName, lname, email, phone, gender, date);
            }
            return null;
        }

        public static bool DeletePerson(int personId)
        {
            return clsPeopleData.DeletePerson(personId);
        }

        public static bool IsPersonExist(int personId)
        {
            return clsPeopleData.IsPersonExist(personId);
        }

        public static DataView GetAllPeople()
        {
            return clsPeopleData.GetAllPeople().DefaultView;
        }

        public bool Save()
        {
            switch(mode)
            {
                case enMode.Add:
                    if(_AddNewPerson())
                    {
                        mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdatePerson();
            }
            return false;
        }
    }
}

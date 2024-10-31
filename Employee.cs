using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTMKtask
{
    public class Employee
    {
        public string FullName { get; }
        public DateOnly DateOfBirth { get; }
        public Gender Gender { get; }

        public Employee(string fullName, DateOnly dateOfBirth, Gender gender)
        {
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public int GetAge()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}

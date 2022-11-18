using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMScanner
{
    class Patient
    {

        private String lastName;
        private String firstName;
        private String middleName;
        private String namePrefix;
        private String nameSuffix;
        private String birthDate;
        private String sex;
        private String id;

        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string MiddleName { get => middleName; set => middleName = value; }
        public string NamePrefix { get => namePrefix; set => namePrefix = value; }
        public string NameSuffix { get => nameSuffix; set => nameSuffix = value; }
        public string BirthDate { get => birthDate; set => birthDate = value; }
        public string Sex { get => sex; set => sex = value; }
        public string ID { get => id; set => id = value; }
    }
}

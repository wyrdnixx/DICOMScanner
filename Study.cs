using Dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMScanner
{
    class Study
    {
        private String instanceUID;
        private DicomDataset[] series;
        private String dateTime;
        //private String instanceUIDTEST;
        private String description;
        private String id;

        public string InstanceUID { get => instanceUID; set => instanceUID = value; }
        public DicomDataset[] Series { get => series; set => series = value; }
        public string DateTime { get => dateTime; set => dateTime = value; }
        public string Description { get => description; set => description = value; }
        public string ID { get => id; set => id = value; }
    }
}

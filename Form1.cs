using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DICOMScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


    


        private void btnTestConvert_Click(object sender, EventArgs e)
        {


            Encapsulate();



        }
        public void EncapsulatePDF()
        {
            string dicom_file = @"C:\pc_inst\test.dcm";
            string pdf_file = @"C:\pc_inst\example.pdf";
            byte[] pdf = System.IO.File.ReadAllBytes(pdf_file);

            var studyUID = new DicomUID("1234", "Study Instance UID", DicomUidType.SOPInstance);
            var generator = new DicomUIDGenerator();
            var dataset = new DicomDataset
{
      { DicomTag.SpecificCharacterSet,  "ISO_IR 6" },
      { DicomTag.InstanceNumber, 1 },
      { DicomTag.InstanceCreationDate, DateTime.Now },
      { DicomTag.InstanceCreationTime, DateTime.Now },
      { DicomTag.SOPClassUID, DicomUID.EncapsulatedPDFStorage },
      { DicomTag.SOPInstanceUID, "" },
      { DicomTag.ContentDate, DateTime.Now },
      { DicomTag.ContentTime, DateTime.Now },
      { DicomTag.AcquisitionDateTime, DateTime.Now },
      { DicomTag.AccessionNumber, "1234"},
      { DicomTag.Modality, "DOC" },
      { DicomTag.ConversionType, "WSD" },
      { DicomTag.ImageLaterality, "" },
      { DicomTag.Manufacturer, "Manufacturer" },
      { DicomTag.ManufacturerModelName, "Product" },

      { DicomTag.ReferringPhysicianName, (string)null },
      { DicomTag.PerformingPhysicianName, "physician" },

      { DicomTag.PatientName, "Patient Name"},
      { DicomTag.PatientID, "" },
      { DicomTag.PatientBirthDate, "" },
      { DicomTag.PatientSex, "M" },
      { DicomTag.PatientAge, "" },
      { DicomTag.PatientWeight, "" },
      { DicomTag.PatientAddress, "" },

      { DicomTag.StudyID, "2345" },
      { DicomTag.StudyDate, "20221118" },
      { DicomTag.StudyTime, "093900" },
      { DicomTag.StudyDescription, "" },

      { DicomTag.SeriesNumber, "1234" },
      { DicomTag.SeriesDate, DateTime.Now },
      { DicomTag.SeriesTime, DateTime.Now },
      { DicomTag.SeriesDescription, ""},
      { DicomTag.SeriesInstanceUID, generator.Generate( studyUID ) },

      { DicomTag.BurnedInAnnotation, "YES" },
      { DicomTag.VerificationFlag, "VERIFIED" },
      { DicomTag.MIMETypeOfEncapsulatedDocument, "application/pdf" },
      { DicomTag.EncapsulatedDocument, pdf }
};

            DicomFile file = new DicomFile();
            file.Dataset.Add(dataset);
            file.FileMetaInfo.TransferSyntax = DicomTransferSyntax.ImplicitVRLittleEndian; //Specify transfer syntax
            file.Save(dicom_file);

        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


    }
}

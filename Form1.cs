using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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


            string doctorName = " Dr. medTest";

                Dicom.DicomDataset data = new Dicom.DicomDataset() { };

                data.Add(Dicom.DicomTag.SOPClassUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
                data.Add(Dicom.DicomTag.SOPInstanceUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
                data.Add(Dicom.DicomTag.StudyDate, DateTime.Now);
                data.Add(Dicom.DicomTag.StudyTime, DateTime.Now);
                data.Add(Dicom.DicomTag.AccessionNumber, "");
                data.Add(Dicom.DicomTag.ReferringPhysicianName, doctorName);
                data.Add(Dicom.DicomTag.StudyID, "1");
                data.Add(Dicom.DicomTag.SeriesNumber, "1");
                data.Add(Dicom.DicomTag.ModalitiesInStudy, "CR");
                data.Add(Dicom.DicomTag.Modality, "CR");
                data.Add(Dicom.DicomTag.NumberOfStudyRelatedInstances, "1");
                data.Add(Dicom.DicomTag.NumberOfStudyRelatedSeries, "1");
                data.Add(Dicom.DicomTag.NumberOfSeriesRelatedInstances, "1");
                data.Add(Dicom.DicomTag.PatientOrientation, "F/A");
                data.Add(Dicom.DicomTag.ImageLaterality, "U");

                data.Add(new Dicom.DicomOtherWord(Dicom.DicomTag.PixelData, new Dicom.IO.Buffer.CompositeByteBuffer()));
                Dicom.Imaging.DicomPixelData pixelData = Dicom.Imaging.DicomPixelData.Create(data);

            ushort imgwidth = 50;
            ushort imgheight = 50;


            pixelData.Width = imgwidth;
                pixelData.Height = imgheight;
                pixelData.SamplesPerPixel = 1;

                pixelData.HighBit = 9;
                pixelData.BitsStored = 10;
                //pixelData.BitsAllocated = 16;

                pixelData.PhotometricInterpretation = Dicom.Imaging.PhotometricInterpretation.Monochrome1;

            Image slideImage =    pictureBox1.Image;
            //byte[] uncompressedBitmapByteArray = BitmapUtils.GetPixelData(slideImage);
            byte[] uncompressedBitmapByteArray = ImageToByte(slideImage);
            var byteBuffer = new Dicom.IO.Buffer.MemoryByteBuffer(uncompressedBitmapByteArray);
                pixelData.AddFrame(byteBuffer);

                var file = new Dicom.DicomFile(data);
                //file.Save(filePath);
            

        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


    }
}

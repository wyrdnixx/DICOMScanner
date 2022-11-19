using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// // using fo-dicom 5.0.3
/// // https://github.com/fo-dicom/fo-dicom
/// </summary>



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


            //EncapsulatePDF();

            //MessageBox.Show(dcmJpeg(), "Result...");
            Image myBmp = Image.FromFile(@"C:\pc_inst\test.jpg");
            //MakeDicom(myBmp, 50,50, @"C:\pc_inst\test_app.dcm", "hans");
            //MakeDicom(myBmp, @"C:\pc_inst\test_app.dcm", "hans");

            // Convert via dcmtk
            string res = convertJPG();
            Console.WriteLine("Out: " + res);
            tbresult.Text += res;


            // Update DICOM Tags
            var file = DicomFile.Open(@"C:\pc_inst\test_app.dcm");             // Alt 1
            //var file = await DicomFile.OpenAsync(@"test.dcm");  // Alt 2            

            var newFile = file.Clone(DicomTransferSyntax.JPEGProcess14SV1);
            newFile.Dataset.AddOrUpdate(Dicom.DicomTag.PatientID, "10001");
            newFile.Save(file.File.Name);
            var patientid = newFile.Dataset.GetString(DicomTag.PatientID);

            tbresult.Text += "PatientID: " + patientid;
            IImage testImg = new DicomImage(@"C:\pc_inst\test_app.dcm").RenderImage();
            

            Bitmap bmp = testImg.AsBitmap();
            pictureBox1.Image = bmp;
            //pictureBox1.Size = bmp.Size;
            
            
            



        }

        static String  convertJPG()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\pc_inst\TestSCU\dcmtk\bin\img2dcm.exe";
            process.StartInfo.Arguments = @"C:\pc_inst\test.jpg C:\pc_inst\test_app.dcm -v"; // Note the /c command (*)
            //process.StartInfo.Arguments = @"/C C:\pc_inst\TestSCU\dcmtk\bin\img2dcm.exe C:\pc_inst\test.jpg C:\pc_inst\test_dcm.dcm -ll info"; // Note the /c command (*)
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            
            process.Start();
            //* Read the output (or the error)
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine("#--: "+ output);
            string err = process.StandardError.ReadToEnd();
            Console.WriteLine("E--: " + err);
            process.WaitForExit();

            if (err != null)
            {
                return err;

            } else
            {
                return output;
            }
            
        }
    #region asyn test
        public void convertJPGasync()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = @"/C C:\pc_inst\TestSCU\dcmtk\bin\img2dcm.exe C:\pc_inst\test.jpg2 C:\pc_inst\test_dcm.dcm -v",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            //* Set your output and error (asynchronous) handlers
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandlerError);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        

        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine("#-: "+ outLine.Data);

        }
        static void OutputHandlerError(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine("E-: " + outLine.Data);

        }
        //public static void MakeDicom(Image slideImage, ushort imgwidth, ushort imgheight, string filePath, string doctorName)

        #endregion

        public static void MakeDicom(Image slideImage,  string filePath, string doctorName)
        {
            Dicom.DicomDataset data = new Dicom.DicomDataset() { };

            data.Add(Dicom.DicomTag.SOPClassUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
            data.Add(Dicom.DicomTag.SOPInstanceUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
            //data.Add(Dicom.DicomTag.StudyDate, DateTime.Now);
            //data.Add(Dicom.DicomTag.StudyTime, DateTime.Now);
            //data.Add(Dicom.DicomTag.AccessionNumber, "");
            //data.Add(Dicom.DicomTag.ReferringPhysicianName, doctorName);
            //data.Add(Dicom.DicomTag.StudyID, "1");
            //data.Add(Dicom.DicomTag.SeriesNumber, "1");
            //data.Add(Dicom.DicomTag.ModalitiesInStudy, "CR");
            //data.Add(Dicom.DicomTag.Modality, "CR");
            //data.Add(Dicom.DicomTag.NumberOfStudyRelatedInstances, "1");
            //data.Add(Dicom.DicomTag.NumberOfStudyRelatedSeries, "1");
            //data.Add(Dicom.DicomTag.NumberOfSeriesRelatedInstances, "1");

            data.Add(Dicom.DicomTag.BitsAllocated, "8");
            data.Add(Dicom.DicomTag.BitsStored, "8");
            data.Add(Dicom.DicomTag.HighBit, "7");
            data.Add(Dicom.DicomTag.LossyImageCompressionMethod, "ISO_10918_1");

            //data.Add(Dicom.DicomTag.PatientOrientation, "F/A");
            //data.Add(Dicom.DicomTag.ImageLaterality, "U");

            data.Add(new Dicom.DicomOtherWord(Dicom.DicomTag.PixelData, new Dicom.IO.Buffer.CompositeByteBuffer()));
            Dicom.Imaging.DicomPixelData pixelData = Dicom.Imaging.DicomPixelData.Create(data);


            //pixelData.Width = (ushort)slideImage.Width;
            //pixelData.Height = (ushort)slideImage.Height;
            pixelData.Width = 138;
            pixelData.Height = 139;
            
            //pixelData.Width = imgwidth;
            //pixelData.Height = imgheight;
            pixelData.SamplesPerPixel = 3;

            //pixelData.HighBit = 9;
            //pixelData.BitsStored = 10;
            //pixelData.BitsAllocated = 16;
            //MessageBox.Show("Bits: " + pixelData.BitsAllocated);
            //pixelData.PhotometricInterpretation = Dicom.Imaging.PhotometricInterpretation.Monochrome1;
            pixelData.PhotometricInterpretation = Dicom.Imaging.PhotometricInterpretation.YbrFull422;
            //pixelData.PhotometricInterpretation = Dicom.Imaging.PhotometricInterpretation.PaletteColor;


            byte[] uncompressedBitmapByteArray = GetPixelData(slideImage);
            var byteBuffer = new Dicom.IO.Buffer.MemoryByteBuffer(uncompressedBitmapByteArray);
            pixelData.AddFrame(byteBuffer);

            //DicomFile file = new Dicom.DicomFile(data);

            // --------
            DicomFile file = new Dicom.DicomFile();
            file.Dataset.Add(data); //Add main dataset to DicomFile
            file.FileMetaInfo.TransferSyntax = DicomTransferSyntax.ImplicitVRLittleEndian; //Specify transfer syntax
            //file.FileMetaInfo.TransferSyntax = DicomTransferSyntax.JPEGProcess1; //Specify transfer syntax
            //file.Save(filePath); //Save file to disk

            var compressFile = file.ChangeTransferSyntax(DicomTransferSyntax.ExplicitVRLittleEndian);
            
            //file.Save(filePath);
            //compressFile.Save(filePath);
        }

        //convert image to bytearray
        public static byte[] GetPixelData(Image img)
        {
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                arr = ms.ToArray();
            }

            return arr;

            /*using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }*/

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

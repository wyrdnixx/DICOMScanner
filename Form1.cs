using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.IO.Buffer;
using Dicom.Log;
using Dicom.Network;
using FellowOakDicom.Network.Client;
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

        //private Patient[] PatientList;
        private List<Patient> PatientList;
       
        public Form1()
        {
            InitializeComponent();
        }
            


        private void btnTestConvert_Click(object sender, EventArgs e)
        {


            //EncapsulatePDF();


            Image myBmp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.DefaultExt = "jpg";
            openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myBmp = Image.FromFile(openFileDialog1.FileName);

                // Convert via dcmtk
                string res = convertJPG(openFileDialog1.FileName);
                Console.WriteLine("Out: " + res);
                tbresult.Text += res;


                // Update DICOM Tags
                var file = DicomFile.Open(@"C:\pc_inst\test_app.dcm");             // Alt 1
                                                                                   //var file = await DicomFile.OpenAsync(@"test.dcm");  // Alt 2            

                var newFile = file.Clone(DicomTransferSyntax.JPEGProcess14SV1);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.PatientID, tbPatId.Text);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.PatientName, tbPatName.Text);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.SOPClassUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.SOPInstanceUID, Dicom.DicomUID.ComputedRadiographyImageStorage);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.StudyDate, DateTime.Now);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.StudyTime, DateTime.Now);
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.AccessionNumber, "");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.ReferringPhysicianName, "Hans-Doktor");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.StudyID, "1");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.SeriesNumber, "1");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.ModalitiesInStudy, "CR");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.Modality, "CR");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.NumberOfStudyRelatedInstances, "1");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.NumberOfStudyRelatedSeries, "1");
                newFile.Dataset.AddOrUpdate(Dicom.DicomTag.NumberOfSeriesRelatedInstances, "1");
                newFile.Save(file.File.Name);
                var patientid = newFile.Dataset.GetString(DicomTag.PatientID);

                tbresult.Text += "PatientID: " + patientid;
                IImage testImg = new DicomImage(@"C:\pc_inst\test_app.dcm").RenderImage();


                Bitmap bmp = testImg.AsBitmap();
                pictureBox1.Image = bmp;
                //pictureBox1.Size = bmp.Size;


            }


        }

        private void btnFindPatient_Click(object sender, EventArgs e)
        {
            cfind();
        }

        private void btnStoreSCU_Click(object sender, EventArgs e)
        {
            StoreSCU();
        }

        private async void StoreSCU()
        {
            var client = DicomClientFactory.Create("192.168.1.210", 104, false, "GETSCU", "ARCHIVE");
            var request = new FellowOakDicom.Network.DicomCStoreRequest(@"C:\pc_inst\test_app.dcm");

            request.OnResponseReceived += (FellowOakDicom.Network.DicomCStoreRequest req, FellowOakDicom.Network.DicomCStoreResponse rsp) =>
             {
                 Console.WriteLine("--> "+ rsp.ToString());
             };

            await client.AddRequestAsync(request);
            await client.SendAsync();
            
        }
        static String  convertJPG(String _file)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\pc_inst\TestSCU\dcmtk\bin\img2dcm.exe";
            process.StartInfo.Arguments = _file + @" C:\pc_inst\test_app.dcm -v"; // Note the /c command (*)
            //process.StartInfo.Arguments = @"C:\pc_inst\test.jpg C:\pc_inst\test_app.dcm -v"; // Note the /c command (*)
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

        public delegate void emptyFunction();
        private  void cfind()
        {
            
            List<DicomDataset> searchResults = new List<DicomDataset>();
            //var request = DicomCFindRequest.CreateStudyQuery(patientId: "12345");
            //var request = DicomCFindRequest.CreateStudyQuery(patientId: tbSearchName.Text);
            var request = DicomCFindRequest.CreatePatientQuery(patientId: tbPatId.Text, patientName: tbPatName.Text);
            
            request.OnResponseReceived += (DicomCFindRequest req, DicomCFindResponse rsp) => {
                if (rsp.HasDataset)
                {
                    Console.WriteLine("C-Find Response:\n" + rsp.Dataset.WriteToString());
                    searchResults.Add(rsp.Dataset);
                    //tbresult.Invoke(new emptyFunction(delegate () { tbresult.Text += ":-)"; }));
                    //tbresult.Text += "C-Find Response:\n" + rsp.Dataset.WriteToString();
                    //tbresult.Invoke(new MethodInvoker(delegate () { tbresult.Text += rsp.Dataset.WriteToString(); }));
                } else
                {
                    //tbresult.Invoke(new MethodInvoker(delegate () { tbresult.Text += "nothing found"; }));
                    
                    Console.WriteLine("nothing found");
                }
                
            };

            var client = new Dicom.Network.DicomClient();
            client.AddRequest(request);
            client.Send("192.168.1.210", 104, false, "GETSCU", "ARCHIVE");

            listBox1.Items.Clear();

            PatientList = new List<Patient>();

            if (searchResults.Count != 0)
            {
                tbresult.Text = "";
                foreach (DicomDataset datasetRes in searchResults)
                {
                    try
                    {
                        Console.WriteLine("RTESRasdfsf: " + datasetRes.ToString());

                        String PatientId = datasetRes.Get<String>(Dicom.DicomTag.PatientID);
                        String PatientName = datasetRes.Get<String>(Dicom.DicomTag.PatientName);
                        //String PatientName = datasetRes.GetSingleValue<String>(Dicom.DicomTag.PatientName);

                        tbresult.Text += PatientId + " - " + PatientName;
                        tbresult.Text += System.Environment.NewLine;
                        listBox1.Items.Add(PatientId + ":" + PatientName);

                        Patient p = new Patient();
                        p.ID = PatientId;
                        p.FirstName = PatientName;
                        PatientList.Add(p);

                        ListViewItem item = new ListViewItem(PatientId);
                        item.SubItems.Add(PatientName);
                        listView1.Items.Add(item);

                    }
                    catch (Exception e) { }
                }

            } else
            {
                tbresult.Text = "nothing found";
            }

    
            // test
            foreach (Patient p in PatientList)
            {
                tbresult.Text += "Object: " + p.FirstName;
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;
            tbPatId.Text = listView1.SelectedItems[0].SubItems[0].Text;
            tbPatName.Text = listView1.SelectedItems[0].SubItems[1].Text;
        }

       
    }
}

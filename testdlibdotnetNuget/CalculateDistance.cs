using DlibDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testdlibdotnetNuget
{
    public partial class CalculateDistance : Form
    {
        private readonly string imageDirectory = Application.StartupPath + @"\images\";
        dynamic detector;
        dynamic net;
        dynamic sp;
        Dictionary<string, List<double>> map = new Dictionary<string, List<double>>();

        public CalculateDistance()
        {
            InitializeComponent();
            detector = Dlib.GetFrontalFaceDetector();
            sp = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat");
            net = DlibDotNet.Dnn.LossMetric.Deserialize("dlib_face_recognition_resnet_model_v1.dat");
        }

        private void pic_image1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Choose Image1";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.jpg*)|*.jpg*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                pic_image1.Image = Image.FromFile(fdlg.FileName);
                if (File.Exists(imageDirectory + @"/image1.jpg"))
                {
                    File.Delete(imageDirectory + @"/image1.jpg");
                }
                pic_image1.Image.Save(imageDirectory + @"/image1.jpg");
            }
        }

        private void pic_image2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Choose Image2";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.jpg*)|*.jpg*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                pic_image2.Image = Image.FromFile(fdlg.FileName);
                if (File.Exists(imageDirectory + @"/image2.jpg"))
                {
                    File.Delete(imageDirectory + @"/image2.jpg");
                }
                pic_image2.Image.Save(imageDirectory + @"/image2.jpg");
            }
        }

        private void btn_enroll_Click(object sender, EventArgs e)
        {
            this.Close();
            //new Form1().ShowDialog();
        }

        private void btn_calDistance_Click(object sender, EventArgs e)
        {
            List<double> description1 = getDescription("image1.jpg");
            List<double> description2 = getDescription("image2.jpg");

            if (description1 != null && description2 != null)
            {
                Console.WriteLine(string.Join(",", description1.ToArray()));
                Console.WriteLine(string.Join(",", description2.ToArray()));
                double distance = calculateDistance(description1, description2);
                lbl_status.Text = "Distance= " + distance;
            }
        }

        #region FaceMatching
        private List<double> getDescription(string image)
        {
            try
            {
                if (!File.Exists(imageDirectory + @"/"+image))
                {
                    MessageBox.Show(image+" doesn't exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(imageDirectory + @"/"+image))
                using (img)
                {
                    var faces = new List<Matrix<RgbPixel>>();
                    foreach (var face in detector.Operator(img))
                    {
                        var shape = sp.Detect(img, face);
                        var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.40);
                        var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                        faces.Add(faceChip);
                    }

                    if (!faces.Any())
                    {
                        MethodInvoker inv = delegate
                        {
                            this.lbl_status.Text = "Error: No face found";
                            lbl_status.ForeColor = Color.DarkRed;
                        };
                        this.Invoke(inv);
                        return null;
                    }
                    if (faces.Count > 1)
                    {
                        MethodInvoker inv = delegate
                        {
                            this.lbl_status.Text = "Error: More than one face found";
                            lbl_status.ForeColor = Color.DarkRed;
                        };
                        this.Invoke(inv);
                        return null;
                    }

                    var faceDescriptors = net.Operator(faces);

                    List<double> descriptionList = new List<double>();
                    foreach (var face in faceDescriptors[0])
                    {
                        descriptionList.Add(face);
                    }

                    //save image
                    Dlib.SaveJpeg(faces[0], imageDirectory+@"/face_"+image);                    
                    foreach (var descriptor in faceDescriptors)
                        descriptor.Dispose();

                    foreach (var face in faces)
                        face.Dispose();
                    return descriptionList;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private double calculateDistance(List<double> desc1, List<double> desc2)
        {            
            double sum = 0;
            for (int i = 0; i < desc1.Count; i++)
            {
                double d = desc1[i] - desc2[i];
                sum += (d * d);
            }
            
            return Math.Sqrt(sum);
        }
        #endregion FaceMatching

    }
}

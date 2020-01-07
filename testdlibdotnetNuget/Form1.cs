using DlibDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;

namespace testdlibdotnetNuget
{
    public partial class Form1 : Form, IDisposable
    {
        private Image image;
        private string imageName = "image.jpg";
        private readonly string DIR = Application.StartupPath + @"\images\";
        private CancellationTokenSource _canceller;
        private bool camStatus = false;
        private int counter = 1;
        private Dictionary<string, Image> imageList = new Dictionary<string, Image>();
        private PictureBox[] pictureBoxList = new PictureBox[15];
        private List<double> descriptionList;
        private string imageDir;
        private List<Description> list;
        private static dynamic detector;
        private static dynamic net;
        private static dynamic sp;

        private string rollNo;
        private string course;
        private string name;
        private string dob;
        private string email;
        private string institute;
        private string seq = "";
        private System.Collections.Generic.Queue<Description> queue;

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(DIR)) {
                Directory.CreateDirectory(DIR);
            }
            imageDir = ConfigurationManager.AppSettings["imageDirPath"];
            //imageDir = Application.StartupPath + @"\student_images";
            rdbtn_webcam.Checked = true;

            list = DBHandler.getRecognitionDetails();
            detector = Dlib.GetFrontalFaceDetector();
            sp = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat");
            net = DlibDotNet.Dnn.LossMetric.Deserialize("dlib_face_recognition_resnet_model_v1.dat");

            queue = new System.Collections.Generic.Queue<Description>(3);
        }

        #region recognitionButton
        //Recognition button function
        private async void btn_recognition_Click(object sender, EventArgs e)
        {
            rdbtn_fileUpload.Visible = false;
            rdbtn_webcam.Visible = false;

            btn_enrollFromFile.Enabled = false;
            btn_submit.Enabled = false;
            lbl_camtureFaces.Text = "Face Recognition";

            btn_recognition.Enabled = false;
            btn_stopRecognition.Visible = true;
            btn_enrollement.Enabled = false;
            tab_1.Visible = true;

            tab_1.SelectedIndex = 1;
            ((Control)tab_enrollment).Enabled = false;
            pnl_viewList.Visible = false;

            lblStatus("Recognition view is running.....","WARNING");
            _canceller = new CancellationTokenSource();
            camStatus = false;
            lblStatus("Camera started","WARNING");
            camStatus = false;
            await Task.Run(() =>
            {
                //while (!camStatus)
                //{
                    recognitionWebcam();
                //}
            });
        }

        private void btn_stopRecognition_Click(object sender, EventArgs e)
        {
            btn_enrollement.Enabled = true;
            btn_recognition.Enabled = true;
            btn_enrollFromFile.Enabled = true;
            btn_stopRecognition.Visible = false;
            pic_camera.Image = null;
            if (_canceller != null)
            {
                camStatus = true;
                _canceller.Dispose();
            }
        }

        private void recognitionWebcam()
        {
            dynamic cap = null;
            try
            {
                int X = 0, Y = 0;
                cap = new OpenCvSharp.VideoCapture(0);                
                if (!cap.IsOpened())
                {
                    MethodInvoker inv = delegate
                    {
                        lblStatus("Unable to connect to camera");
                    };
                    this.Invoke(inv);
                    return;
                }

                while (!camStatus)
                {
                    // Grab a frame
                    var temp = new OpenCvSharp.Mat();
                    //Console.WriteLine("size: " + temp.ElemSize() + " " + temp.ElemSize1() + " " + temp);
                    if (!cap.Read(temp))
                    {
                        camStatus = true;
                        //return;
                        break;
                    }
                    var array = new byte[temp.Width * temp.Height * temp.ElemSize()];
                    Marshal.Copy(temp.Data, array, 0, array.Length);
                    using (var cimg = Dlib.LoadImageData<BgrPixel>(array, (uint)temp.Height, (uint)temp.Width, (uint)(temp.Width * temp.ElemSize())))
                    {
                        // Detect faces 
                        var faces = detector.Operator(cimg);
                        // Find the pose of each face.
                        var shapes = new List<FullObjectDetection>();
                        for (var i = 0; i < faces.Length; ++i)
                        {
                            var det = sp.Detect(cimg, faces[i]);
                            shapes.Add(det);
                            var point = det.GetPart((uint)i);
                            X = point.X;
                            Y = point.Y;
                        }

                        //delete previous image file
                        if (File.Exists(DIR + imageName))
                        {
                            File.Delete(DIR + imageName);
                        }

                        //image = convertArray2DToImage(cimg);
                        //image.Save(DIR + imageName, ImageFormat.Jpeg);
                        Dlib.SaveJpeg(cimg, DIR + imageName);

                        FileStream fs = new FileStream(DIR + imageName, FileMode.Open, FileAccess.Read);
                        image = Image.FromStream(fs);
                        fs.Close();

                        if (X != 0 && Y != 0)
                        {
                            Pen pen = new Pen(Color.Red);
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(X, Y - 60, 150, 150);
                            Graphics gr = Graphics.FromImage(image);
                            gr.DrawRectangle(pen, rect);
                            gr.ResetClip();
                        }

                        pic_camera.Image = image;

                        extractAndMatchFace();
                        temp.Release();
                        temp.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine(e);
            }
            finally
            {
                if(cap != null)
                {
                    cap.Dispose();
                }
            }
        }

        private string extractAndMatchFace()
        {
            try
            {
                //Check for current image
                if (!File.Exists(DIR + imageName))
                {
                    Console.WriteLine("image file dosen't exist  ");
                    return null;
                }
                
                using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(DIR + imageName))
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
                            lblStatus("Error: No face found");
                        };
                        this.Invoke(inv);
                        return null;
                    }
                    if (faces.Count > 1)
                    {
                        MethodInvoker inv = delegate
                        {
                            lblStatus("Error: More than one face found");
                        };
                        this.Invoke(inv);
                        return null;
                    }
                    if (faces.Count == 1) {
                        MethodInvoker inv = delegate
                        {
                            lblStatus(null);
                        };
                        this.Invoke(inv);
                    }
                    
                    var faceDescriptors = net.Operator(faces);                    

                    descriptionList = new List<double>();
                    foreach (var face in faceDescriptors[0])
                    {
                        descriptionList.Add(face);
                    }
                                        
                    //save image
                    Dlib.SaveJpeg(faces[0], DIR+"recognition.jpg");

                    //Check the descriptionsList retrieved from DB
                    if (list != null)
                    {
                        double maxDistance = 5;
                        double calDistance = 0;
                        int minPosition = -1;
                        int i = 0;

                        foreach (Description desc in list)
                        {
                            //Distance b/w current image's face description and db face's description
                            calDistance = calculateDistance(descriptionList, desc.description);
                            if (calDistance < maxDistance)
                            {
                                maxDistance = calDistance;
                                minPosition = i;
                            }
                            i++;
                        }
                        //if distance b/w current image's description and db
                        if (maxDistance <= 0.58)
                        {
                            Console.WriteLine("275Roll: "+ list.ElementAt(minPosition).rollNo + "name: " + list.ElementAt(minPosition).studentName + " ïmg: " + list.ElementAt(minPosition).image + " dis: " + maxDistance);
                            queue.Enqueue(list.ElementAt(minPosition));
                            //Console.WriteLine("\nCount after if+ " + queue.Count);
                            if (queue.Count < 3)
                            {
                                return "";
                            }
                            if (queue.Count >= 3)
                            {
                                Description desc1 = queue.Dequeue();
                                Description desc2 = queue.Dequeue();
                                Description desc3 = queue.Dequeue();

                                Console.WriteLine("292: "+desc1.rollNo+" "+desc2.rollNo+" "+desc3.rollNo);
                                //if desc1==desc2==desc3
                                if (desc1.Equals(desc2) && desc2.Equals(desc3))
                                {
                                    MethodInvoker inv = delegate
                                    {
                                        lblStatus(desc1.studentName + " " + maxDistance,"SUCCESS");
                                    };
                                    this.Invoke(inv);
                                    if (DBHandler.insertOrUpdateAttandance(desc1.rollNo.ToString()))
                                    {
                                        Console.WriteLine("Attandance Marked+ " + desc1.rollNo.ToString());
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("309recog:   d: " + maxDistance + " po: " + minPosition);
                                    MethodInvoker inv = delegate
                                    {
                                        lblStatus("Unknown");
                                    };
                                    this.Invoke(inv);
                                    Console.WriteLine("Unknown");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("321recog:   d: " + maxDistance + " po: " + minPosition+" "+list.ElementAt(minPosition).rollNo);
                            MethodInvoker inv = delegate
                            {
                                lblStatus("Unknown");
                            };
                            this.Invoke(inv);
                            Console.WriteLine("Unknown");
                        }
                    }
                    
                    foreach (var descriptor in faceDescriptors)
                        descriptor.Dispose();

                    foreach (var face in faces)
                        face.Dispose();
                    //Delete previous face image
                    if (File.Exists(DIR + "recognition.jpg"))
                    {
                        File.Delete(DIR + "recognition.jpg");
                    }
                }
                return name;
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine("face ex: "+e.Message);
            }
            finally
            {
                if (descriptionList != null)
                {
                    descriptionList = null;
                }
            }
            return null;
        }
        
        #endregion recognitionButton

        #region enrollmentButton
        //Enrollemnet button function
        private void btn_enrollment_Click(object sender, EventArgs e)
        {            
            tab_1.Visible = true;
            tab_1.SelectedIndex = 0;
            ((Control)tab_enrollment).Enabled = true;
            pnl_viewList.Visible = true;
            pnl_viewList.Enabled = false;
        }
        
        private void captureImage(string id) {
            if (_canceller != null)
            {
                camStatus = true;
                _canceller.Dispose();
                _canceller = null;
            }
            if (_canceller == null)
            {
                string viewName = FaceExtraction(id+".jpg");
                Console.WriteLine("captureImage " + viewName);
                if (viewName != null && File.Exists(viewName))
                {
                    Image image = Image.FromFile(viewName);
                    imageList.Add(id, image);
                    pic_camera.Image = null;
                    showImageList(id,image);
                    counter++;
                }
            }
        }

        private string FaceExtraction(string id)
        {
            string faceImageName = DIR + id.ToString();
            try
            {
                if (!File.Exists(DIR + imageName))
                {
                    return null;
                }

                using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(DIR + imageName))
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
                        lblStatus("Error: No face found");
                        return null;
                    }
                    if (faces.Count > 1)
                    {
                        lblStatus("Error: More than one face found");
                        return null;
                    }

                    var faceDescriptors = net.Operator(faces);

                    string filePath = DIR + "description.text";
                    if (File.Exists(filePath) == true)
                    {
                        File.Delete(filePath);
                    }

                    descriptionList = new List<double>();
                    foreach (var face in faceDescriptors[0])
                    {
                        descriptionList.Add(face);
                    }

                    if (counter == 1)
                    {
                        DeleteFiles(faceImageName);
                    }
                    //save image
                    Dlib.SaveJpeg(faces[0], faceImageName);

                    bool res = DBHandler.InsertFaceDescription(descriptionList.ToArray(), id.ToString(), rollNo);
                    lblStatus(id + " image is captured successfully","SUCCESS");

                    foreach (var descriptor in faceDescriptors)
                        descriptor.Dispose();

                    foreach (var face in faces)
                        face.Dispose();
                }
                return faceImageName;
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                if (descriptionList != null)
                {
                    descriptionList = null;
                }
            }
        }
        
        //function to set respective image in the picture box
        private void showImageList(string key, Image image)
        {
            //Console.WriteLine("count+switch " + key);
            switch (key)
            {
                /*-----------3-feet-----------------*/
                case "pic_3_frontView":
                    pic_3_frontView.Image = image;
                    break;
                case "pic_3_leftView":
                    pic_3_leftView.Image = image;
                    break;
                case "pic_3_rightView":
                    pic_3_rightView.Image = image;
                    break;
                case "pic_3_topView":
                    pic_3_topView.Image = image;
                    break;
                case "pic_3_downView":
                    pic_3_downView.Image = image;
                    break;
                /*-----------5-feet-----------------*/
                case "pic_5_frontView":
                    pic_5_frontView.Image = image;
                    break;
                case "pic_5_leftView":
                    pic_5_leftView.Image = image;
                    break;
                case "pic_5_rightView":
                    pic_5_rightView.Image = image;
                    break;
                case "pic_5_topView":
                    pic_5_topView.Image = image;
                    break;
                case "pic_5_downView":
                    pic_5_downView.Image = image;
                    break;
                /*-----------7-feet-----------------*/
                case "pic_7_frontView":
                    pic_7_frontView.Image = image;
                    break;
                case "pic_7_leftView":
                    pic_7_leftView.Image = image;
                    break;
                case "pic_7_rightView":
                    pic_7_rightView.Image = image;
                    break;
                case "14":
                    pic_7_topView.Image = image;
                    break;
                case "pic_7_downView":
                    pic_7_downView.Image = image;
                    break;
            }
        }

        //function to reset image
        private void resetImage(string id)
        {
            try
            {
                if (File.Exists(id + ".jpg"))
                {
                    File.Delete(id + ".jpg");
                }            
                imageList.Remove(id);
                Console.WriteLine("remove map  " + imageList.ToString());
            }
            catch(Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine("resetImage:  " + e.Message);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            rollNo = txt_rollNo.Text.Trim();
            course = txt_course.Text.Trim();
            name = txt_name.Text.Trim();
            dob = txt_dob.Text.Trim();
            email = txt_email.Text.Trim();
            institute = txt_institute.Text.Trim();
            if (validateData() == true)
            {
                lblStatus("Data saved successfully","SUCCESS");
                DBHandler.InsertEnrollmenmtData(rollNo, name, course,dob,email,institute,"1");
                seq = "saved";

                tab_1.SelectedIndex = 1;
                pnl_viewList.Enabled = true;
            }
        }

        private void btn_reset_Click(object sender, EventArgs e) {
            txt_rollNo.Text = null;
            txt_name.Text = null;
            txt_dob.Text = null;
            txt_email.Text = null;
            txt_institute.Text = null;
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (seq != "saved")
            {
                lblStatus("Please fill the enrollment data");
                return;
            }
            Console.WriteLine("counter submit  " + counter);
            if (seq != "saved" || counter != 16 )
            {
                lblStatus("Please capture all images. Some images are missing");
                return;
            }
            bool res=DBHandler.InsertEnrollmenmtData(rollNo.Trim(), name.Trim(), course.Trim(), dob.Trim(), email.Trim(), institute.Trim(), "1");
            if (res == true)
            {
                lblStatus("Data submitted successfully","SUCCESS");

                seq = "";
                txt_rollNo.Text = null;
                txt_name.Text = null;
                txt_dob.Text = null;
                txt_email.Text = null;
                txt_institute.Text = null;
            }
        }

        private bool validateData()
        {
            Regex Pattern = new Regex("[0-9]$", RegexOptions.Compiled);
            if (string.IsNullOrEmpty(rollNo.Trim()))
            {
                lbl_status.Text = "Please fill rollNO";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (!Pattern.IsMatch(rollNo.Trim())) {
                lbl_status.Text = "Only digits are allowed as RollNo";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (DBHandler.checkRollnoExist(rollNo.Trim()))
            {
                lbl_status.Text = "This roll no already exist";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }

            Pattern = new Regex("[a-zA-z ]$", RegexOptions.Compiled);
            if ( string.IsNullOrEmpty(name.Trim())) {
                lbl_status.Text = "Please fill Name";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (!Pattern.IsMatch(name.Trim()))
            {
                lbl_status.Text = "Please fill valid name. Only Chars are allowed";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }

            if (string.IsNullOrEmpty(dob.Trim()))
            {
                lbl_status.Text = "Please fill dob";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }

            Pattern = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(email.Trim()))
            {
                lbl_status.Text = "Please fill email";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (!Pattern.IsMatch(email.Trim()))
            {
                lbl_status.Text = "Please fill valid email id";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            Pattern = new Regex(@"^([^0-9][A-Za-z]*( )?(\.)?)[0-9]*|^([^0-9][A-Za-z]*(\.)?( )?)[0-9]*$", RegexOptions.Compiled);
            if (string.IsNullOrEmpty(course.Trim()))
            {
                lbl_status.Text = "Please fill course";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (!Pattern.IsMatch(course.Trim()))
            {
                lbl_status.Text = "Invalid Course name";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }

            if (string.IsNullOrEmpty(institute.Trim()))
            {
                lbl_status.Text = "Please fill institution";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            else if (!Pattern.IsMatch(institute.Trim()))
            {
                lbl_status.Text = "invalid institute name";
                lbl_status.ForeColor = Color.DarkRed;
                return false;
            }
            return true;
        }

        private void txt_dob_focusOut(object sender, EventArgs e) {
            cal_dob.Visible=false;
        }

        private void txt_dob_click(object sender, EventArgs e)
        {
            cal_dob.Visible = true;
        }

        private void cal_dob_DateChanged(object sender, DateRangeEventArgs e)
        {
            txt_dob.Text = cal_dob.SelectionRange.Start.ToString("dd-MM-yyyy");
            cal_dob.Visible = false;
        }

        private void chooseImageFile(string viewName)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = viewName;
            fdlg.Filter = "All files (*.*)|*.*|All files (*.jpg*)|*.jpg*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                lblStatus(fdlg.FileName,"SUCCESS");
                pic_camera.Image = Image.FromFile(fdlg.FileName);
                if (File.Exists(DIR + imageName))
                {
                    File.Delete(DIR + imageName);
                }
                pic_camera.Image.Save(DIR + imageName);
            }
        }

        /**
         * Fetch image file from folders and find the name mapping form 
         */
        private void enrollFromFile()
        {
            try
            {
                string studentRollno;
                string studentName;

                string studentCourse = (string.IsNullOrEmpty(course)) ? "PG-DAC" : course;
                string studentEmail = (string.IsNullOrEmpty(email)) ? "mail@mail.com" : course;
                string studentInstitute = (string.IsNullOrEmpty(institute)) ? "CDAC,Juhu" : course;
                string studentCenterId = "1";
                string referesh = "................................................................................................";

                Dictionary<string, string> dict = new Dictionary<string, string>();
                /*Read mapping.csv file and store all the mapping data into dictionary
                 * to get name corresponding roll number
                 */
                string[] csvData = File.ReadAllLines(imageDir + @"mapping.csv");
                if (csvData.Length > 0)
                {
                    foreach (string line in csvData)
                    {
                        //Console.WriteLine(line);
                        string[] data = line.Split(',');
                        if (!dict.ContainsKey(data[1]))
                        {
                            dict.Add(data[1], data[0]);
                        }
                    }
                }

                foreach (string studentImageDir in Directory.GetDirectories(imageDir))
                {
                    studentRollno = studentImageDir.Substring(studentImageDir.LastIndexOf('\\') + 1);
                    int i = 1;
                    //Console.WriteLine(studentRollno + "\n");
                    foreach (string studentImage in Directory.GetFiles(studentImageDir))
                    {
                        lblStatus(studentRollno + " Data is capturing."+referesh.Substring(0,i++),"SUCCESS");
                        if (!DBHandler.checkRollnoExist(studentRollno))
                        {
                            if (dict.TryGetValue(studentRollno, out studentName))
                            {
                                bool status = DBHandler.InsertEnrollmenmtData(studentRollno, studentName, studentCourse, DateTime.Now.ToString(), studentEmail, studentInstitute, studentCenterId);
                                if (status)
                                {
                                    rollNo = studentRollno;
                                }
                            }
                        }
                        if (DBHandler.checkRollnoExist(studentRollno))
                        {
                            rollNo = studentRollno;
                            //Console.WriteLine(studentImage);
                            File.Copy(studentImage, DIR + imageName, true);
                            string viewName = FaceExtraction(studentRollno + ".jpg");
                            //Console.WriteLine("captureImage " + viewName);
                        }
                    }
                    lblStatus(null);
                }
            }
            catch (DirectoryNotFoundException dex) {
                lblStatus(imageDir +" Directory not found");
                Program.log(dex.ToString());
            }
            catch (FileNotFoundException fex)
            {
                lblStatus(imageDir + @"\mapping.csv"+" File doesn't exist");
                Program.log(fex.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(""+e, "Error ",MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.log(e.ToString());
                Console.WriteLine(e.Message);
            }
        }
        #endregion enrollmentButton

        #region viewButtons
        private void Webcam()
        {
            try
            {
                var cap = new OpenCvSharp.VideoCapture(0);
                if (!cap.IsOpened())
                {
                    MethodInvoker inv = delegate
                    {
                        lblStatus("Unable to connect to camera");
                    };
                    this.Invoke(inv);
                    return;
                }

                // Grab a frame
                var temp = new OpenCvSharp.Mat();                    
                if (!cap.Read(temp))
                {
                    return;
                }

                var array = new byte[temp.Width * temp.Height * temp.ElemSize()];
                Marshal.Copy(temp.Data, array, 0, array.Length);
                using (var cimg = Dlib.LoadImageData<BgrPixel>(array, (uint)temp.Height, (uint)temp.Width, (uint)(temp.Width * temp.ElemSize())))
                {
                    // Detect faces 
                    var faces = detector.Operator(cimg);
                    // Find the pose of each face.
                    var shapes = new List<FullObjectDetection>();
                    for (var i = 0; i < faces.Length; ++i)
                    {
                        var det = sp.Detect(cimg, faces[i]);
                        shapes.Add(det);
                    }

                    //delete previous image file
                    if (File.Exists(DIR + imageName))
                    {
                        File.Delete(DIR + imageName);
                    }

                    image = convertArray2DToImage(cimg);
                    image.Save(DIR + imageName, ImageFormat.Jpeg);
                    //Dlib.SaveJpeg(cimg, DIR + imageName);

                    //FileStream fs = new FileStream(DIR + imageName, FileMode.Open, FileAccess.Read);
                    //image = Image.FromStream(fs);
                    //fs.Close();

                    pic_camera.Image = image;
                    temp.Dispose();

                }
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine(e);
            }
        }

        #region 3_feet_buttons
        /*-----------------3feet-frontview Buttons------------------------------*/
        private void btn_3_capture1_ClickAsync(object sender, EventArgs e)
        {
            captureImage("pic_3_frontView");
        }

        private async void pic_3_frontView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_3_frontView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });            
        }

        private void btn_3_reset1_Click(object sender, EventArgs e)
        {
            pic_3_frontView.Image = null;
            resetImage("pic_3_frontView");
        }
        /*-----------------3feet-frontview Buttons------------------------------*/

        /*-----------------3feet-leftView Buttons------------------------------*/
        private void btn_3_capture2_Click(object sender, EventArgs e)
        {
            captureImage("pic_3_leftView");
        }

        private void btn_3_reset2_Click(object sender, EventArgs e)
        {
            pic_3_leftView.Image = null;
            resetImage("pic_3_leftView");
        }
        
        private async void pic_3_leftView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_3_leftView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        /*-----------------3feet-leftView Buttons------------------------------*/

        /*-----------------3feet-rightView Buttons------------------------------*/
        private async void pic_3_rightView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_3_rightView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_3_reset3_Click(object sender, EventArgs e)
        {
            pic_3_rightView.Image = null;
            resetImage("pic_3_rightView");
        }

        private void btn_3_capture3_Click(object sender, EventArgs e)
        {
            captureImage("pic_3_rightView");
        }

        /*-----------------3feet-rightView Buttons------------------------------*/

        /*-----------------3feet-topView Buttons------------------------------*/
        private async void pic_3_topView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_3_topView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_3_capture4_Click(object sender, EventArgs e)
        {
            captureImage("pic_3_topView");
        }

        private void btn_3_reset4_Click(object sender, EventArgs e)
        {
            pic_3_topView.Image = null;
            resetImage("pic_3_topView");
        }
        /*-----------------3feet-topView Buttons------------------------------*/

        /*-----------------3feet-downView Buttons------------------------------*/
        private async void pic_3_downView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_3_downView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_3_capture5_Click(object sender, EventArgs e)
        {
            captureImage("pic_3_downView");
        }

        private void btn_3_reset5_Click(object sender, EventArgs e)
        {
            pic_3_downView.Image = null;
            resetImage("pic_3_downView");
        }
        /*-----------------3feet-downView Buttons------------------------------*/
        #endregion 3_feet_buttons

        #region 5_feet_buttons
        //frontview
        private async void pic_5_frontView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_5_frontView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }
        private void btn_5_capture1_Click(object sender, EventArgs e)
        {
            captureImage("pic_5_frontView");
        }

        private void btn_5_reset1_Click(object sender, EventArgs e)
        {
            pic_5_frontView.Image = null;
            resetImage("pic_5_frontView");
        }
        //leftView
        private async void pic_5_leftView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_5_leftView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_5_capture2_Click(object sender, EventArgs e)
        {
            captureImage("pic_5_leftView");
        }

        private void btn_5_reset2_Click(object sender, EventArgs e)
        {
            pic_5_leftView.Image = null;
            resetImage("pic_5_leftView");
        }
        //rightView
        private async void pic_5_rightView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_5_rightView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_5_capture3_Click(object sender, EventArgs e)
        {
            captureImage("pic_5_rightView");
        }

        private void btn_5_reset3_Click(object sender, EventArgs e)
        {
            pic_5_rightView.Image = null;
            resetImage("pic_5_rightView");
        }
        //topView
        private async void pic_5_topView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_5_topView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_5_capture4_Click(object sender, EventArgs e)
        {
            captureImage("pic_5_topView");
        }

        private void btn_5_reset4_Click(object sender, EventArgs e)
        {
            pic_5_topView.Image = null;
            resetImage("pic_5_topView");
        }
        //downView
        private async void pic_5_downView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_5_downView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_5_capture5_Click(object sender, EventArgs e)
        {
            captureImage("pic_5_downView");
        }

        private void btn_5_reset5_Click(object sender, EventArgs e)
        {
            pic_5_downView.Image = null;
            resetImage("pic_5_downView");
        }
        #endregion 5_feet_buttons

        #region 7_feet_buttons
        //front view
        private async void pic_7_frontView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_7_frontView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }
        private void btn_7_capture1_Click(object sender, EventArgs e)
        {
            captureImage("pic_7_frontView");
        }

        private void btn_7_reset1_Click(object sender, EventArgs e)
        {
            pic_7_frontView.Image = null;
            resetImage("pic_7_frontView");
        }
        //left view
        private async void pic_7_leftView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_7_leftView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_7_capture2_Click(object sender, EventArgs e)
        {
            captureImage("pic_7_leftView");
        }

        private void btn_7_reset2_Click(object sender, EventArgs e)
        {
            pic_7_leftView.Image = null;
            resetImage("pic_7_leftView");
        }
        //right view
        private async void pic_7_rightView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_7_rightView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_7_capture3_Click(object sender, EventArgs e)
        {
            captureImage("pic_7_rightView");
        }

        private void btn_7_reset3_Click(object sender, EventArgs e)
        {
            pic_7_rightView.Image = null;
            resetImage("pic_7_rightView");
        }
        //top view
        private async void pic_7_topView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_7_topView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_7_capture4_Click(object sender, EventArgs e)
        {
            captureImage("pic_7_topView");
        }

        private void btn_7_reset4_Click(object sender, EventArgs e)
        {
            pic_7_topView.Image = null;
            resetImage("pic_7_topView");
        }
        //down view
        private async void pic_7_downView_Click(object sender, EventArgs e)
        {
            if (rdbtn_fileUpload.Checked == true)
            {
                chooseImageFile("pic_7_downView");
                return;
            }
            lbl_status.Text = "Camera started";
            lbl_status.ForeColor = Color.Blue;
            _canceller = new CancellationTokenSource();
            camStatus = false;
            await Task.Run(() =>
            {
                while (!camStatus)
                {
                    Webcam();
                }
            });
        }

        private void btn_7_capture5_Click(object sender, EventArgs e)
        {
            captureImage("pic_7_downView");
        }

        private void btn_7_reset5_Click(object sender, EventArgs e)
        {
            pic_7_downView.Image = null;
            resetImage("pic_7_downView");
        }
        #endregion 7_feet_buttons
        #endregion viewButtons
        
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

        //function to delete all previous files 
        private void DeleteFiles(string faceImageName)
        {
            //Delete faceimages
            var list = Directory.GetFiles(DIR, "*View.jpg", SearchOption.TopDirectoryOnly);
            //Console.WriteLine("deleteall  " + list.ToString()+" s: "+list.Length);
            foreach (var item in list)
            {
                File.Delete(item);
            }
        }

        private Bitmap convertArray2DToImage(Array2D<BgrPixel> cimg)
        {
            Bitmap pic = new Bitmap(cimg.Columns, cimg.Rows);
            RgbPixel[] pixelArray = new RgbPixel[cimg.Rows * cimg.Columns];
            for (int i = 0; i < 400; i++)
            {
                for (int j = 0; j < 600; j++)
                {
                    Color color = Color.FromArgb(cimg[i][j].Red, cimg[i][j].Green, cimg[i][j].Blue);
                    pic.SetPixel(j, i, color);
                }
            }
            return pic;
        }

        private void rdbtn_webcam_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbtn_fileUpload_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_enrollFromFile_Click(object sender, EventArgs e)
        {
            enrollFromFile();
        }

        private void btn_calDistance_Click(object sender, EventArgs e)
        {
            CalculateDistance f=new CalculateDistance();
            f.Show();
        }

        private void calculateDistancesBWMultiImages(string baseDir)
        {
            string path = Application.StartupPath;
            if (baseDir.Equals("lfw_eq_2_diff"))
            {
                path = path + @"\lfw_eq_2_diff";
            }
            else if (baseDir.Equals("lfw_eq_2_same"))
            {
                path = path + @"\lfw_eq_2_same";
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Path doesn't Exist: "+ path);
                return;
            }
            try
            {
                if (File.Exists(path + @"\distances.txt"))
                {
                    File.Delete(path + @"\distances.txt");
                }
                if(File.Exists(DIR + "image1.jpg"))
                {
                    File.Delete(DIR + "image1.jpg");
                }
                if(File.Exists(DIR + "image2.jpg"))
                {
                    File.Delete(DIR + "image2.jpg");
                }

                List<double> desc1;
                List<double> desc2;
                double distance;
                foreach (string directory in Directory.GetDirectories(path))
                {
                    string[] file = Directory.GetFiles(directory);
                    File.Copy(file[0], DIR+"image1.jpg",true);
                    File.Copy(file[1], DIR +"image2.jpg",true);
                    desc1=getDescription("image1.jpg");
                    desc2= getDescription("image2.jpg");
                    if (desc1 != null && desc2 != null)
                    {
                        distance = calculateDistance(desc1,desc2);
                        File.AppendAllText(path + @"\distances_"+ baseDir + ".csv", file[0].Substring(file[0].LastIndexOf('\\') + 1) + "," + file[1].Substring(file[1].LastIndexOf('\\') + 1) + "," + distance + "\n");
                        Console.WriteLine(path + " " + file[0].Substring(file[0].LastIndexOf('\\') + 1) + "," + file[1].Substring(file[1].LastIndexOf('\\') + 1) + "," + distance);
                    }
                }
            }catch(Exception e)
            {
                Program.log(e.ToString());
                Console.WriteLine("Error: calculateDistancesBWMultiImages\n" + e+"\n");
            }

        }

        private List<double> getDescription(string image)
        {
            try
            {
                if (!File.Exists(DIR + @"/" + image))
                {
                    MessageBox.Show(image + " doesn't exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(DIR + @"/" + image))
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
                            this.lbl_status.Text = "Error: No face found "+image;
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
                    //Dlib.SaveJpeg(faces[0], DIR + @"/face_" + image);
                    foreach (var descriptor in faceDescriptors)
                        descriptor.Dispose();

                    foreach (var face in faces)
                        face.Dispose();
                    return descriptionList;
                }
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            //calculateDistancesBWMultiImages("lfw_eq_2_same");
            //calculateDistancesBWMultiImages("lfw_eq_2_diff");
        }

        private void txt_rollNo_LostFocus(object sender, EventArgs e)
        {
            string applicationNo = txt_rollNo.Text.ToUpper().Trim();
            SeafarerApplication application = DBHandler.GetSeafarerApplication(applicationNo);
            if (application != null)
            {
                lblStatus("Valid SID.","SUCCESS");
                txt_name.Text = application.firstname + " " + application.middlename + " " + application.lastname;
                txt_dob.Text = application.dob.ToShortDateString();
                txt_email.Text = application.emailid;
            }
            else
            {
                resetForm();
                lblStatus("Ivalid INDOS/SID no.","ERROR");
            }
        }

        private void lblStatus(string message,string type="ERROR")
        {
            if (string.IsNullOrEmpty(message)) {
                lbl_status.ResetText();
                lbl_status.Refresh();
            }
            else if (type.ToUpper() == "ERROR")
            {
                lbl_status.Text = message;
                lbl_status.ForeColor = Color.DarkRed;
                lbl_status.Refresh();
            }
            else if (type.ToUpper() == "SUCCESS")
            {
                lbl_status.Text = message;
                lbl_status.ForeColor = Color.ForestGreen;
                lbl_status.Refresh();
            }
            else
            {
                lbl_status.Text = message;
                lbl_status.ForeColor = Color.Blue;
                lbl_status.Refresh();
            }
        }

        private void resetForm()
        {
            //txt_rollNo.ResetText();
            txt_name.ResetText();
            txt_dob.ResetText();
            txt_email.ResetText();
            txt_course.ResetText();
            txt_institute.ResetText();
        }
    }
}

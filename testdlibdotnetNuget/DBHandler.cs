using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Text.Json.Serialization;

namespace testdlibdotnetNuget
{
    class DBHandler
    {
        public static NpgsqlConnection con;
        public static NpgsqlConnection conSID;

        private DBHandler()
        { }

        public static NpgsqlConnection GetConnection()
        {
            if (con != null)
            {
                return con;
            }
            else
            {
                try
                {
                    string conStr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    con = new NpgsqlConnection(conStr);
                    return con;
                }
                catch (Exception e)
                {
                    Program.log("GetConnection: " + e.ToString());
                    return null;
                }
            }
        }

        public static NpgsqlConnection GetSIDConnection()
        {
            if (conSID != null)
            {
                return conSID;
            }
            else
            {
                try
                {
                    string conStr = ConfigurationManager.ConnectionStrings["conSID"].ConnectionString;
                    conSID = new NpgsqlConnection(conStr);
                    return conSID;
                }
                catch (Exception e)
                {
                    Program.log("GetSIDConnection: " + e.ToString());
                    return null;
                }
            }
        }

        public static bool CloseConnection()
        {
            try
            {
                bool status = false;
                if (con != null)
                {
                    con.Close();
                    status= true;
                }
                if(conSID != null)
                {
                    conSID.Close();
                    status= true;
                }
                return status;
            }
            catch(Exception e)
            {
                Program.log(e.ToString());
                return false;
            }
        }

        public static bool InsertFaceDescription(double[] description,string faceImageName,string rollNo)
        {
            if (description != null && faceImageName!=null)
            {
                try
                {
                    GetConnection().Open();
                    string str = "insert into description (description,image,roll_no) values (ARRAY[{0}],'{1}','{2}')";
                    string arr = string.Join(",", description);
                    string sqlstr= string.Format(str, arr.ToString(), faceImageName, rollNo);
                    //Console.WriteLine("count: " + sqlstr);
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    int res = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return true;
                }catch(Exception e)
                {
                    Program.log(e.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
            return false;
        } 

        public static bool InsertEnrollmenmtData(string rollNo, string name, string course,string dob,string email,string institute,string centerId)
        {
            try
            {
                //string sqlstr = "insert into students (roll_no,college,course,dob,mobile_no,student_name,centre_id) values";
                string str = "insert into students (roll_no,student_name,course,dob,email,college,centre_id) values(" +
                    "'{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                string sqlstr = string.Format(str,rollNo, name,course,dob,email,institute,centerId);
                //Console.WriteLine("connection  " + sqlstr);
                GetConnection().Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                int res = cmd.ExecuteNonQuery();
                Console.WriteLine("count: " + res);
                cmd.Dispose();
                if (res > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
            }
            finally
            {
                CloseConnection();
            }            
            return false;
        }

        public static bool checkRollnoExist(string rollNo)
        {
            try
            {
                string str = "select count(*) from students where roll_no='{0}'";
                string sqlstr = string.Format(str, rollNo);
                GetConnection().Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                int res = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                if (res == 1)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public static List<Description> getRecognitionDetails()
        {
            try
            {
                List<Description> list=new List<Description>();
                string str = "select s.roll_no,s.student_name,d.image,d.description from students s inner join description d" +
                    " on s.roll_no=d.roll_no ";
                string sqlstr = string.Format(str);
                //Console.WriteLine("recog: "+ sqlstr);
                GetConnection().Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                List<double> descList;
                double[] desc;
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        descList = new List<double>();
                        desc=reader.GetFieldValue<double[]>(3);
                        descList.AddRange(desc);

                        //Console.WriteLine(string.Join(",",descList));
                        list.Add( new Description(reader.GetInt64(0),reader.GetString(1), reader.GetString(2),descList));
                        //res =reader.GetString(0);
                        //string.Join(",", reader.GetFieldValue<double[]>(1));
                    }
                    return list;
                }
                cmd.Dispose();
                return null;                
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
            }
            finally
            {
                CloseConnection();               
            }
            return null;
        }

        public static bool checkDescriptionExist(string rollNo)
        {
            try
            {
                string str = "select count(*) from description where roll_no='{0}'";
                string sqlstr = string.Format(str, rollNo);
                GetConnection().Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                int res = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                if (res >=5)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Program.log(e.ToString());
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public static bool insertOrUpdateAttandance(string rollNo)
        {
            /* if entry time exist for the particular rollno then update exit time
             * else insert new entry for the current date
             */
            try
            {
                string str = "select count(*) from attendance where roll_no='{0}'";
                string sqlstr = string.Format(str, rollNo);
                GetConnection().Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                int res = Convert.ToInt32(cmd.ExecuteScalar());
                if (res > 0)
                {
                    str = "update attendance set exit_dateTime=now()::timestamp(0) where roll_no={0}";
                    sqlstr = string.Format(str, rollNo);
                    //Console.WriteLine(sqlstr);
                    cmd = new NpgsqlCommand(sqlstr);
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    res =cmd.ExecuteNonQuery();
                    if (res > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    str = "insert into attendance(roll_no,is_present,entry_dateTime) values({0},'true',now()::timestamp(0))";
                    sqlstr = string.Format(str, rollNo);
                    Console.WriteLine(sqlstr);
                    cmd = new NpgsqlCommand(sqlstr);
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    res = cmd.ExecuteNonQuery();
                    con.Close();
                    if (res > 0)
                    {
                        return true;
                    }
                }                
            }catch(Exception ex)
            {
                Program.log(ex.ToString());
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        /// <summary>
        /// Retrieve the seafarer details from sid server
        /// </summary>
        /// <param name="sidNo"></param>
        /// <returns></returns>
        public static SeafarerApplication GetSeafarerApplication(string sidNo)
        {
            SeafarerApplication seafarerApplication=null;
            if (!string.IsNullOrEmpty(sidNo))
            {
                try
                {
                    string str = "select sidno,applicationid,firstname,middlename,lastname,gender,dob,pob,nationality,identification_mark,sid_doi,sid_doe,cdcno,transactionid,sid_status,countrycode,sid_poi,indos_no,sid_counter,barcode,document_id,document,emailid" +
                        " from \"SIDDB\".sid_view where sidno='{0}' or indos_no='{0}'and document_id='102'";
                    string sqlstr = string.Format(str, sidNo);
                    Console.WriteLine(sqlstr);
                    GetSIDConnection().Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlstr);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSID;
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        return seafarerApplication;
                    }
                    seafarerApplication = new SeafarerApplication();
                    reader.Read();   
                    seafarerApplication.sidno = reader.GetString(0);
                    seafarerApplication.applicationid = reader.GetString(1);
                    seafarerApplication.firstname = reader.GetString(2);
                    seafarerApplication.middlename = reader.GetString(3);
                    seafarerApplication.lastname = reader.GetString(4);
                    seafarerApplication.gender = reader.GetString(5);
                    seafarerApplication.dob = reader.GetDateTime(6);
                    seafarerApplication.pob = reader.GetString(7);
                    seafarerApplication.nationality = reader.GetString(8);
                    seafarerApplication.identification_mark = reader.GetString(9);
                    seafarerApplication.sid_doi = reader.GetDateTime(10);
                    seafarerApplication.sid_doe = reader.GetDateTime(11);
                    seafarerApplication.cdcno = reader.GetString(12);
                    seafarerApplication.transactionid = reader.IsDBNull(13)? "": reader.GetString(13);
                    seafarerApplication.sid_status = reader.GetString(14);
                    seafarerApplication.countrycode = reader.GetInt32(15);
                    seafarerApplication.sid_poi = reader.GetString(16);
                    seafarerApplication.indos_no = reader.GetString(17);
                    seafarerApplication.sid_counter = reader.GetInt32(18);
                    seafarerApplication.barcode = (reader.IsDBNull(19) ? null:(byte[])reader.GetValue(19));
                    SeafarerDocument documemt = new SeafarerDocument();
                    documemt.documentId = reader.GetString(20);
                    documemt.document = reader.IsDBNull(21) ? null:(byte[])reader.GetValue(21);
                    seafarerApplication.document = documemt;
                    seafarerApplication.emailid = reader.GetString(22);


                    Console.WriteLine("GetSeafarerApplication: " + seafarerApplication);
                }
                catch (Exception ex)
                {                    
                    Program.log(ex.ToString());
                }
                finally
                {
                    CloseConnection();
                }
            }
            return seafarerApplication;
        }

    }
}

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
                    Console.WriteLine("connection  " + e.Message);
                    return null;
                }
            }
        }

        public static bool CloseConnection()
        {
            try
            {
                if (con != null)
                {
                    con.Close();
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine("close con  " + e.Message);
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
                    Console.WriteLine("InsertOrUpdateDescription  " + e);
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
                Console.WriteLine("InsertOrUpdateDescription  " + e);
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
                Console.WriteLine("InsertOrUpdateDescription  " + e);
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
                Console.WriteLine("getRecognitionDetails  " + e);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }                
            }
            return null;
        }

        internal static bool checkDescriptionExist(string rollNo)
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
                Console.WriteLine("InsertOrUpdateDescription  " + e);
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
                    Console.WriteLine(sqlstr);
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
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            return false;
        }
    }
}

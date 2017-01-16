using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ChargeCollection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int quantity;
        public double total;
        public string ttbEmployeeName;
        public bool ttbIsLunch;
        public bool ttbIsDinner;
        public bool ttbIsSupper;
        public bool ttbIsCoffee;
        public string ttbMasterRowId;
        public string unFinishRecordFileName;
        public string button10MealCode;
        public string button9MealCode;
        public string button8MealCode;
        public string mealCode;
        public string mealClass;
        private Thread threadSync;
        private bool threadFlag = true;
        private bool threadFinsh = false;
        public ManualResetEvent flagForUnfinshRecordFileName = new ManualResetEvent(true);
        public ManualResetEvent flagForThread = new ManualResetEvent(true);
        public ManualResetEvent flagForSyncMasterInfo = new ManualResetEvent(true);
        private List<EmployeeInfo> employeesCache = new List<EmployeeInfo>();
        private List<MealPriceInfo> mealPriceCache = new List<MealPriceInfo>();
        private List<Mealrecord> mealRecordCache = new List<Mealrecord>();
        private delegate void SetPos(int ipos, string vinfo);
        private Meal lastIndexCache = Meal.Lunch;
        private Meal lastIndex = Meal.Lunch;
        private string undeleteFile = "";

        private void SetTextMesssage(int ipos, string vinfo)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMesssage);
                this.Invoke(setpos, new object[] { ipos, vinfo });
            }
            else
            {
                this.label5.Text = vinfo;
                this.progressBar1.Value = Convert.ToInt32(ipos);
                if (ipos == this.progressBar1.Maximum)
                {
                    this.textBox1.Enabled = true;
                    this.label5.Visible = false;
                    this.progressBar1.Visible = false;
                    this.textBox1.Focus();
                }
            }
        }
        [Serializable]
        public class EmployeeInfo
        {
            public string name { get; set; }
            public double coffeeFee { get; set; }
            public string id { get; set; }
            public bool status { get; set; }
            public bool Lunch { get; set; }
            public bool Dinner { get; set; }
            public bool Supper { get; set; }
            public bool IsCoffee { get; set; }
            public string MasterRowId { get; set; }
            public string emp_id { get; set; }
            public DateTime lastMealTime { get; set; }
            public bool Breakfast { get; set; }
        }

        [Serializable]
        public class MealPriceInfo
        {
            public string code { get; set; }
            public double price { get; set; }
        }

        public class Mealrecord
        {
            public string code { get; set; }
            public string mealClass { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
            public double price { get; set; }
            public bool EXTRA_MEAL { get; set; }

        }

        public void readFileUpdateDB(string path, int index)
        {
            string conString = Properties.Settings.Default.ConnString;
            StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"/Test.txt");
            string str = sr.ReadToEnd();
            if (sr.EndOfStream)
            {
                sr.Close();
            }
            string[] strArray = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(string.Format("文本行数（不计空行）是{0}", strArray.Length));
        }

        public void insertMealRecord2DB(string record)
        {
            string[] arrary = record.Split(',');
            string import_id = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddhhmmss"), System.Guid.NewGuid().ToString().Split('-')[0]);
            string sql = string.Format(
                @"INSERT INTO [HU_MEAL_RECORD] 
([CREATED_PROGRAM],[GROUP_ID],[COMPANY_ID],[EMPLOYEE_ID],[MEAL_DATETIME],
[MEAL_TYPE_CODE],[MEAL_PRICE],[QUANTITY],[MEAL_CLASS],[MEAL_AMOUNT],
[IMPORT_DATA_ID],[CREATED_DATE],[TAG],[DELETE_FLAG],[CREATED_BY],[ROW_ID],[EXTRA_MEAL]) 
values({13},{14},{15},{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{16})"
                , arrary[0]
                , arrary[1]
                , arrary[2]
                , arrary[3]
                , arrary[4]
                , arrary[5]
                , arrary[6]
                , string.Format("'{0}'", import_id)
                , string.Format("'{0}'", DateTime.Now.ToString())
                , "'1'"
                , 0
                , string.Format("'{0}'", Properties.Settings.Default.CreatedBy.ToString())
                , string.Format("'{0}{1}'", DateTime.Now.ToString("yyyyMMddhhmmss"), System.Guid.NewGuid().ToString().Split('-')[0])
                , string.Format("'{0}'", Properties.Settings.Default.Created_Program.ToString())
                , string.Format("'{0}'", Properties.Settings.Default.GroupID.ToString())
                , string.Format("'{0}'", Properties.Settings.Default.CompanyID.ToString())
                , arrary[7] == "True" ? 1:0);

            string sql1 = "UPDATE A SET A.MEAL_TYPE_ID=B.ROW_ID,A.MEAL_TYPE_NAME=B.MEAL_TYPE_NAME FROM HU_MEAL_RECORD A JOIN HU_MEAL_TYPE B ON A.MEAL_TYPE_CODE=B.MEAL_TYPE_CODE AND B.DELETE_FLAG=0 WHERE A.IMPORT_DATA_ID='{0}' ";

            string sql3 = string.Format(@" UPDATE MSDS_Customer SET LastMealTime = {0} WHERE ROW_ID = {1} ", arrary[1], arrary[0]);
            
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.Connection = con;
                SqlTransaction Trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;
                cmd.CommandText = sql;

                try
                {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format(sql1, import_id);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = sql3;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    throw (ex);
                }
            }
        }

        private void insertMealRecord2File(string content)
        {
            flagForUnfinshRecordFileName.WaitOne();
            flagForUnfinshRecordFileName.Reset();
            FileStream fs = new FileStream(this.unFinishRecordFileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(content);
            sw.Close();
            fs.Close();
            flagForUnfinshRecordFileName.Set();
        }

        private List<string> insertMealRecord2File(bool flag)
        {
            flagForSyncMasterInfo.WaitOne();
            flagForSyncMasterInfo.Reset();
            List<string> result = new List<string>();
            flagForUnfinshRecordFileName.WaitOne();
            flagForUnfinshRecordFileName.Reset();
            FileStream fs = new FileStream(unFinishRecordFileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            DateTime dt = DateTime.Now;
            EmployeeInfo entity = employeesCache.SingleOrDefault(x => x.MasterRowId == ttbMasterRowId);
            if (entity != null)
            {
                foreach (var item in mealRecordCache)
                {
                    string text = string.Format("'{0}','{1}','{2}',{3},{4},'{5}',{6},{7}\r\n"
                        , this.ttbMasterRowId
                        , dt.ToString()
                        , item.code
                        , item.price
                        , item.quantity
                        , item.mealClass
                        , item.price * item.quantity
                        , item.EXTRA_MEAL);
                    if (item.code == "coffee")
                    {
                        entity.coffeeFee = item.price * item.quantity + entity.coffeeFee;
                    }
                    else
                    {
                        if (item.code != "rice soup")
                        {
                            //只要点了正餐就更新最后用餐时间 买咖啡和早餐不更新最后用餐时间
                            entity.lastMealTime = dt;
                        }

                    }
                    if (flag)
                    {
                        sw.Write(text);
                    }
                    else
                    {
                        result.Add(text.Replace("\r\n", ""));
                    }
                }

               
            }

            sw.Close();
            fs.Close();
            flagForUnfinshRecordFileName.Set();
            flagForSyncMasterInfo.Set();
            return result;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始加载方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.unableForm();
            this.listView1.View = View.List;
            this.button1.Enabled = false;
            this.textBox1.Enabled = false;
            progressBar1.Maximum = 200;
            Thread thread = new Thread(FormLoadThread);
            thread.Start();
        }

        private void FormLoadThread(object obj)
        {
            string employeeCachePath = Path.Combine(System.Environment.CurrentDirectory, "EmployeeCache.txt");
            string priceCachePath = Path.Combine(System.Environment.CurrentDirectory, "priceCache.txt");
            string undeletefilePath = Path.Combine(System.Environment.CurrentDirectory, "undeleteFile.txt");
            bool exceptionFlag = false;
            if (!Properties.Settings.Default.StartWithLoadLocalCache)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnString))
                    {
                        string sql = @"SELECT A.EMPLOYEE_CARD,A.ROW_ID,A.EMPLOYEE_NAME,A.EMPLOYEE_NAME_CN,A.EMPLOYEE_PY,A.IS_CHINESE_FOOD,A.IS_WEST_FOOD,A.IS_SPECIAL_FOOD,A.IS_COFFEE,A.CARD_STATUS
      , ISNULL(B.COFE_FEE, 0) AS COFE_FEE, A.EMPLOYEE_ID , A.LastMealTime ,A.IS_BREAKFAST
FROM [MSDS_Customer] A
 LEFT JOIN(
 SELECT SUM(ISNULL(A.MEAL_AMOUNT, 0)) AS COFE_FEE, EMPLOYEE_ID FROM HU_MEAL_RECORD A
 WHERE MEAL_TYPE_CODE = 'coffee'
 AND MEAL_DATETIME BETWEEN convert(char(10), dateadd(d, -day(getdate()) + 1, getdate()), 120) + ' 00:00:00' AND convert(char(10), dateadd(d, -day(getdate()), dateadd(m, 1, getdate())), 120) + ' 23:59:59'
 GROUP BY EMPLOYEE_ID
) B ON A.ROW_ID = B.EMPLOYEE_ID ";

                        SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        EmployeeInfo ei;
                        int count = dt.Rows.Count;
                        double feed = ((double)160) / ((double)count);
                        double startFeed = 0;
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            i++;
                            startFeed = startFeed + feed;
                            SetTextMesssage((int)startFeed, string.Format("加载员工信息...{0}/{1}", i, count));
                            ei = new EmployeeInfo();
                            ei.id =  dr["EMPLOYEE_CARD"].ToString();
                            ei.Lunch =  bool.Parse(dr["IS_CHINESE_FOOD"].ToString());
                            ei.Dinner = bool.Parse(dr["IS_WEST_FOOD"].ToString());
                            ei.Supper = bool.Parse(dr["IS_SPECIAL_FOOD"].ToString());
                            ei.IsCoffee = bool.Parse(dr["IS_COFFEE"].ToString());
                            ei.MasterRowId = dr["ROW_ID"].ToString();
                            ei.name = (dr["EMPLOYEE_NAME_CN"] is DBNull || String.IsNullOrEmpty(dr["EMPLOYEE_NAME_CN"].ToString()) ? dr["EMPLOYEE_NAME"].ToString() : dr["EMPLOYEE_NAME_CN"].ToString());
                            ei.status = bool.Parse(dr["CARD_STATUS"].ToString());
                            ei.coffeeFee = double.Parse(dr["COFE_FEE"].ToString());
                            ei.emp_id = dr["EMPLOYEE_ID"].ToString();
                            ei.lastMealTime = (dr["LastMealTime"] is DBNull || string.IsNullOrEmpty(dr["LastMealTime"].ToString()) ? DateTime.MinValue : DateTime.Parse(dr["LastMealTime"].ToString()));
                            ei.Breakfast = bool.Parse(dr["IS_BREAKFAST"].ToString());
                            employeesCache.Add(ei);
                        }

                        #region  注释

                        //SqlCommand cmd = con.CreateCommand();
                        //cmd.CommandText = sql;
                        //con.Open();
                        //SetTextMesssage(10 / 200, "加载员工信息...");
                        //SqlDataReader reader = cmd.ExecuteReader();
                        //EmployeeInfo ei;
                        //while (reader.Read())
                        //{

                        //    SetTextMesssage(10 / 200, "加载员工信息...");
                        //    ei = new EmployeeInfo();
                        //    ei.id = reader["EMPLOYEE_CARD"].ToString();
                        //    ei.IsChineseFood = reader["IS_CHINESE_FOOD"].ToString();
                        //    ei.IsWestFood = reader["IS_WEST_FOOD"].ToString();
                        //    ei.IsSpecialFood = reader["IS_SPECIAL_FOOD"].ToString();
                        //    ei.IsCoffee = reader["IS_COFFEE"].ToString();
                        //    ei.MasterRowId = reader["ROW_ID"].ToString();
                        //    ei.name = reader["EMPLOYEE_NAME"].ToString();
                        //    ei.status = reader["CARD_STATUS"].ToString();
                        //    employeesCache.Add(ei);
                        //}
                        //reader.Close();

                        #endregion

                        SqlCommand cmd = con.CreateCommand();
                        con.Open();

                        #region 注释

                        //i = 0;
                        //foreach (var item in employeesCache)
                        //{
                        //    try
                        //    {
                        //        i++;
                        //        SetTextMesssage((20 + i * 175 / (count)), string.Format("加载员工咖啡消费信息...{0}/{1}", i, count));
                        //        #region 统计当月咖啡消费金额
                        //        int year = DateTime.Now.Year;
                        //        int month = DateTime.Now.Month;
                        //        sql = "select SUM(ISNULL(A.MEAL_AMOUNT,0)) from HU_MEAL_RECORD A where A.EMPLOYEE_ID = '" + item.MasterRowId + "' AND A.MEAL_TYPE_CODE ='coffee' AND DATEPART(year,A.MEAL_DATETIME) =" + year + " AND DATEPART(mm,A.MEAL_DATETIME)=" + month;
                        //        cmd.CommandText = sql;
                        //        item.coffeeFee = 0;

                        //        string value = Convert.ToString(cmd.ExecuteScalar());
                        //        item.coffeeFee = double.Parse(string.IsNullOrEmpty(value) ? "0" : Convert.ToString(value));
                        //        #endregion
                        //    }
                        //    catch
                        //    { }

                        //}

                        #endregion
                        int feed2 = 0;
                        cmd.CommandText = "select MEAL_TYPE_CODE,MEAL_PRICE from HU_MEAL_TYPE";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            feed2 = feed2 + 8;
                            SetTextMesssage(feed2, string.Format("加载菜单信息...{0}/{1}", i, count));
                            string code = code = reader["MEAL_TYPE_CODE"].ToString();
                            //double price = code.Equals("coffee", StringComparison.OrdinalIgnoreCase) ? double.Parse(reader["MEAL_PRICE"].ToString()) : 0;
                            double price = double.Parse(reader["MEAL_PRICE"].ToString());
                            mealPriceCache.Add(new MealPriceInfo()
                            {
                                code = code,
                                price = price
                            });
                        }
                        SetTextMesssage(200, "加载餐费单价数据...");
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    MessageBox.Show("数据库连接失败，数据将缓存在本地");
                    exceptionFlag = true;
                }
            }
            //for  test
            //exceptionFlag = true;

            if (exceptionFlag || Properties.Settings.Default.StartWithLoadLocalCache)
            {
                if (File.Exists(employeeCachePath))
                {
                    using (FileStream fs = new FileStream(employeeCachePath, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            XmlSerializer xml = new XmlSerializer(typeof(List<EmployeeInfo>));
                            employeesCache = (List<EmployeeInfo>)xml.Deserialize(fs);
                        }
                        catch
                        { }
                    }
                }

                #region  注释
                //foreach (EmployeeInfo item in employeesCache)
                //{
                //    using (SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnString))
                //    {
                //        if (con.State != ConnectionState.Open)
                //        {
                //            con.Open();
                //        }

                //        SqlCommand cmd = con.CreateCommand();
                //        cmd.Connection = con;
                //        //SqlTransaction Trans = con.BeginTransaction(IsolationLevel.ReadCommitted);
                //        //cmd.Transaction = Trans;
                //        cmd.CommandText = string.Format(@"INSERT INTO [dbo].[HR_EMPLOYEE]
                //                        ([EMPLOYEE_CARD]
                //                        ,[ROW_ID]
                //                        ,[EMPLOYEE_NAME]
                //                        ,[EMPLOYEE_PY]
                //                        ,[IS_CHINESE_FOOD]
                //                        ,[IS_WEST_FOOD]
                //                        ,[IS_SPECIAL_FOOD]
                //                        ,[IS_COFFEE]
                //                        ,[CARD_STATUS])
                //                        VALUES
                //                        ('{0}'
                //                        ,'{1}'
                //                        ,'{2}'
                //                        ,'{3}'
                //                        ,{4}
                //                        ,{5}
                //                        ,{6}
                //                        ,{7}
                //                        ,{8})",item.id,
                //                        item.MasterRowId,
                //                        item.name,
                //                        "PY",
                //                        (item.IsChineseFood.Equals("Y")?1:0),
                //                        (item.IsWestFood.Equals("Y")?1:0),
                //                        (item.IsSpecialFood.Equals("Y")?1:0),
                //                        (item.IsCoffee.Equals("Y")?1:0),
                //                        (string.IsNullOrEmpty(item.status)? 1: 0));

                //        try
                //        {
                //            cmd.ExecuteNonQuery();
                //            //Trans.Commit();
                //        }
                //        catch (Exception ex)
                //        {
                //            //Trans.Rollback();
                //            throw (ex);
                //        }
                //    }
                //}
                #endregion

                if (File.Exists(priceCachePath))
                {
                    using (FileStream fs = new FileStream(priceCachePath, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            XmlSerializer xml = new XmlSerializer(typeof(List<MealPriceInfo>));
                            mealPriceCache = (List<MealPriceInfo>)xml.Deserialize(fs);
                        }
                        catch
                        { }
                    }
                }
                SetTextMesssage(200, "");
            }
            string unfinshFile = string.Empty;
            foreach (var item in Directory.GetFiles(System.Environment.CurrentDirectory))
            {
                if (item.Contains("unFinshRecords"))
                {
                    unfinshFile = item;
                    break;
                }
            }
            if (string.IsNullOrEmpty(unfinshFile))
            {
                unfinshFile = Path.Combine(System.Environment.CurrentDirectory, string.Format("unFinshRecords{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss")));
                FileStream fs;
                fs = File.Create(unfinshFile);
                fs.Close();
            }
            if (File.Exists(undeletefilePath))
            {
                try
                {
                    StreamReader sr = new StreamReader(undeletefilePath);
                    undeleteFile = sr.ReadToEnd();
                    sr.Close();
                }
                catch { }
            }

            flagForUnfinshRecordFileName.Reset();
            unFinishRecordFileName = unfinshFile;
            flagForUnfinshRecordFileName.Set();
            threadSync = new Thread(syncRecordFunction);
            threadSync.Start();
        }

        private void syncRecordFunction(object obj)
        {
            int k = 0;
            while (threadFlag)
            {
                

                flagForThread.Reset();
                string unfinshFile = Path.Combine(System.Environment.CurrentDirectory, string.Format("unFinshRecords{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss")));
                FileStream fs;
                fs = File.Create(unfinshFile);
                fs.Close();

                flagForUnfinshRecordFileName.WaitOne();
                flagForUnfinshRecordFileName.Reset();
                unFinishRecordFileName = unfinshFile;
                flagForUnfinshRecordFileName.Set();
                StreamReader sr;
                string content = "";
                string[] contentArray;
                foreach (var item in Directory.GetFiles(System.Environment.CurrentDirectory))
                {
                    if (!item.Contains("unFinshRecords") || item == unfinshFile)
                    {
                        continue;
                    }
                    if (!undeleteFile.Contains(item))
                    {
                        sr = new StreamReader(item);
                        content = sr.ReadToEnd();
                        if (sr.EndOfStream)
                        {
                            sr.Close();
                        }
                        string[] aa = new string[1] { "\r\n" };
                        //content = content.Replace("\r\n", "");
                        contentArray = content.Split(aa, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string record in contentArray)
                        {
                            if (string.IsNullOrEmpty(record.Trim()))
                            {
                                continue;
                            }
                            try
                            {
                                this.insertMealRecord2DB(record);
                            }
                            catch
                            {
                                var newRecord = record + "\r\n";
                                this.insertMealRecord2File(newRecord);
                            }
                        }
                    }
                    try
                    {
                        File.Delete(item);
                        undeleteFile.Replace(item, "");
                    }
                    catch
                    {
                        if (!undeleteFile.Contains(item))
                        {
                            undeleteFile += item;
                        }
                    }
                }

                if (k == 0)
                {
                    #region 从服务器抓取数据
                    try
                    {
                        List<EmployeeInfo> employeesCachetemp = new List<EmployeeInfo>();
                        using (SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnString))
                        {
                            //string sql = "select A.EMPLOYEE_CARD,A.ROW_ID,A.EMPLOYEE_NAME,A.EMPLOYEE_PY,A.IS_CHINESE_FOOD,A.IS_WEST_FOOD,A.IS_SPECIAL_FOOD,A.IS_COFFEE,A.CARD_STATUS from HR_EMPLOYEE A";
                            string sql = @"SELECT A.EMPLOYEE_CARD,A.ROW_ID,A.EMPLOYEE_NAME,A.EMPLOYEE_NAME_CN,A.EMPLOYEE_PY,A.IS_CHINESE_FOOD,A.IS_WEST_FOOD,A.IS_SPECIAL_FOOD,A.IS_COFFEE,A.CARD_STATUS
      , ISNULL(B.COFE_FEE, 0) AS COFE_FEE,A.EMPLOYEE_ID,A.LastMealTime,A.IS_BREAKFAST
FROM [MSDS_Customer] A
 LEFT JOIN(
 SELECT SUM(ISNULL(A.MEAL_AMOUNT, 0)) AS COFE_FEE, EMPLOYEE_ID FROM HU_MEAL_RECORD A
 WHERE MEAL_TYPE_CODE = 'coffee'
 AND MEAL_DATETIME BETWEEN convert(char(10), dateadd(d, -day(getdate()) + 1, getdate()), 120) + ' 00:00:00' AND convert(char(10), dateadd(d, -day(getdate()), dateadd(m, 1, getdate())), 120) + ' 23:59:59'
 GROUP BY EMPLOYEE_ID
) B ON A.ROW_ID = B.EMPLOYEE_ID ";

                            SqlCommand cmd = con.CreateCommand();
                            cmd.CommandText = sql;
                            con.Open();

                            SqlDataReader reader = cmd.ExecuteReader();
                            EmployeeInfo ei;
                            while (reader.Read())
                            {
                                ei = new EmployeeInfo();
                                ei.id = reader["EMPLOYEE_CARD"].ToString();
                                ei.Lunch = bool.Parse(reader["IS_CHINESE_FOOD"].ToString());
                                ei.Dinner = bool.Parse(reader["IS_WEST_FOOD"].ToString());
                                ei.Supper = bool.Parse(reader["IS_SPECIAL_FOOD"].ToString());
                                ei.IsCoffee = bool.Parse(reader["IS_COFFEE"].ToString());
                                ei.MasterRowId = reader["ROW_ID"].ToString();
                                ei.name = reader["EMPLOYEE_NAME"].ToString();
                                ei.status = bool.Parse(reader["CARD_STATUS"].ToString());
                                ei.coffeeFee = reader["COFE_FEE"] is DBNull ? 0 : double.Parse(reader["COFE_FEE"].ToString());
                                ei.emp_id = reader["EMPLOYEE_ID"].ToString();
                                ei.lastMealTime = (reader["LastMealTime"] is DBNull || string.IsNullOrEmpty(reader["LastMealTime"].ToString()) ? DateTime.MinValue : DateTime.Parse(reader["LastMealTime"].ToString()));
                                ei.Breakfast = bool.Parse(reader["IS_BREAKFAST"].ToString());
                                employeesCachetemp.Add(ei);
                            }
                            reader.Close();

                            #region 注释

                            //foreach (var item in employeesCachetemp)
                            //{
                            //    try
                            //    {
                            //        #region 统计当月咖啡消费金额
                            //        int year = DateTime.Now.Year;
                            //        int month = DateTime.Now.Month;
                            //        sql = "select SUM(ISNULL(A.MEAL_AMOUNT,0)) from HU_MEAL_RECORD A where A.EMPLOYEE_ID = '" + item.MasterRowId + "' AND A.MEAL_TYPE_CODE ='coffee' AND DATEPART(year,A.MEAL_DATETIME) =" + year + " AND DATEPART(mm,A.MEAL_DATETIME)=" + month;
                            //        cmd.CommandText = sql;
                            //        item.coffeeFee = 0;

                            //        string value = Convert.ToString(cmd.ExecuteScalar());
                            //        item.coffeeFee = double.Parse(string.IsNullOrEmpty(value) ? "0" : Convert.ToString(value));
                            //    }
                            //    catch
                            //    {
                            //    }
                            //        #endregion
                            //}

                            #endregion

                            cmd.CommandText = "select MEAL_TYPE_CODE,MEAL_PRICE from HU_MEAL_TYPE";
                            reader = cmd.ExecuteReader();
                            List<MealPriceInfo> mealPriceCachetemp = new List<MealPriceInfo>();
                            while (reader.Read())
                            {
                                string code = code = reader["MEAL_TYPE_CODE"].ToString();
                                //double price = code.Equals("coffee", StringComparison.OrdinalIgnoreCase) ? double.Parse(reader["MEAL_PRICE"].ToString()) : 0;
                                double price = double.Parse(reader["MEAL_PRICE"].ToString());
                                mealPriceCachetemp.Add(new MealPriceInfo()
                                {
                                    code = code,
                                    price = price
                                });
                            }
                            reader.Close();
                            flagForSyncMasterInfo.WaitOne();
                            flagForSyncMasterInfo.Reset();
                            employeesCache = null;
                            employeesCache = employeesCachetemp;
                            mealPriceCache = null;
                            mealPriceCache = mealPriceCachetemp;
                            flagForSyncMasterInfo.Set();
                        }
                    }
                    catch
                    {
                    }
                    #endregion
                }

                k++;

                flagForThread.Set();
                Thread.Sleep(Properties.Settings.Default.SyncIntervalSec * 1000);
            }
            this.threadFinsh = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> result = this.insertMealRecord2File(!Properties.Settings.Default.ImmediatelySync);
            try
            {
                foreach (var item in result)
                {
                    this.insertMealRecord2DB(item);
                }
            }
            catch
            {
                this.insertMealRecord2File(true);
            }
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox1.Focus();
            this.initForm();
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            this.button3.BackColor = System.Drawing.Color.White;
            this.button4.BackColor = System.Drawing.Color.White;
            this.button5.BackColor = System.Drawing.Color.White;
            this.button6.BackColor = System.Drawing.Color.White;
            this.button7.BackColor = System.Drawing.Color.White;
            this.lastIndex = this.lastIndexCache;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定要关闭吗！", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                e.Cancel = false;  //点击OK
                this.threadFlag = false;
                flagForThread.WaitOne();
                if (!this.threadFinsh && threadSync != null)
                {
                    threadSync.Abort();
                }
                if (File.Exists(Path.Combine(System.Environment.CurrentDirectory, "undeleteFile.txt")))
                {
                    File.Delete(Path.Combine(System.Environment.CurrentDirectory, "undeleteFile.txt"));
                }
                using (FileStream fs = new FileStream(Path.Combine(System.Environment.CurrentDirectory, "undeleteFile.txt"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(undeleteFile);
                    sw.Close();
                }

                File.Delete(Path.Combine(System.Environment.CurrentDirectory, "EmployeeCache.txt"));
                using (FileStream fs = new FileStream(Path.Combine(System.Environment.CurrentDirectory, "EmployeeCache.txt"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                    XmlSerializer xml = new XmlSerializer(typeof(List<EmployeeInfo>));
                    xml.Serialize(fs, employeesCache);
                }
                File.Delete(Path.Combine(System.Environment.CurrentDirectory, "priceCache.txt"));
                using (FileStream fs = new FileStream(Path.Combine(System.Environment.CurrentDirectory, "priceCache.txt"), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                    XmlSerializer xml = new XmlSerializer(typeof(List<MealPriceInfo>));
                    xml.Serialize(fs, mealPriceCache);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.lbErrorMsg.Text = "";
            button3Click();
            lastIndexCache = Meal.Lunch;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.lbErrorMsg.Text = "";
            button4Click();
            lastIndexCache = Meal.Dinner;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.lbErrorMsg.Text = "";
            button5Click();
            lastIndexCache = Meal.Supper;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.lbErrorMsg.Text = "";
            button6Click();
            lastIndexCache = Meal.Coffice;
        }


        private void button7_Click(object sender, EventArgs e)
        {
            this.lbErrorMsg.Text = "";
            button7Click();
            lastIndexCache = Meal.Breakfast;
        }

        /// <summary>
        /// 早餐
        /// </summary>
        private void button7Click()
        {
            if (MealPolicyCheck(Meal.Breakfast))
            {
                this.button10.Visible = true;
                this.button9.Visible = false;
                this.button8.Visible = false;

                this.button10.Text = "粥 rice soup";

                this.button10MealCode = "rice soup";
                this.mealClass = "Breakfast";
                this.button7.Focus();
                this.button3.BackColor = System.Drawing.Color.White;
                this.button4.BackColor = System.Drawing.Color.White;
                this.button5.BackColor = System.Drawing.Color.White;
                this.button6.BackColor = System.Drawing.Color.White;
                this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(155)))), ((int)(((byte)(198)))));
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.lbErrorMsg.Text = "不能享受该服务";
            }
        }

        /// <summary>
        /// 午餐
        /// </summary>
        private void button3Click()
        {
            if (MealPolicyCheck(Meal.Lunch))
            {
                this.button10.Visible = true;
                this.button9.Visible = true;
                this.button8.Visible = true;

                this.button10.Text = "中餐 Chinese food";
                this.button9.Text = "西餐 Western food";
                this.button8.Text = "特色餐点 Featured dishes";

                this.button10.Enabled = this.ttbIsLunch;
                this.button9.Enabled = this.ttbIsDinner;
                this.button8.Enabled = this.ttbIsSupper;

                this.button10MealCode = "chinese food";
                this.button9MealCode = "western food";
                this.button8MealCode = "special food";
                this.mealClass = "Lunch";
                this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(155)))), ((int)(((byte)(198)))));
                this.button4.BackColor = System.Drawing.Color.White;
                this.button5.BackColor = System.Drawing.Color.White;
                this.button6.BackColor = System.Drawing.Color.White;
                this.button7.BackColor = System.Drawing.Color.White;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
                this.button3.Focus();
            }
            else
            {
                this.lbErrorMsg.Text = "不能享受该服务";
            }
        }
        /// <summary>
        /// 晚餐
        /// </summary>
        private void button4Click()
        {
            if (MealPolicyCheck(Meal.Dinner))
            {
                this.button10.Visible = true;
                this.button9.Visible = true;
                this.button8.Visible = false;

                this.button10.Enabled = this.ttbIsLunch;
                this.button9.Enabled = this.ttbIsDinner;
                this.button8.Enabled = this.ttbIsSupper;

                this.button10.Text = "中餐 Chinese food";
                this.button9.Text = "西餐 Western food";
                this.button8.Text = "特色餐点 Featured dishes";

                this.button10MealCode = "chinese food";
                this.button9MealCode = "western food";
                this.button8MealCode = "special food";
                this.mealClass = "Dinner";
                this.button4.Focus();
                this.button3.BackColor = System.Drawing.Color.White;
                this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(155)))), ((int)(((byte)(198)))));
                this.button5.BackColor = System.Drawing.Color.White;
                this.button6.BackColor = System.Drawing.Color.White;
                this.button7.BackColor = System.Drawing.Color.White;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.lbErrorMsg.Text = "不能享受该服务";
            }
        }
        /// <summary>
        /// 夜宵
        /// </summary>
        private void button5Click()
        {
            if (MealPolicyCheck(Meal.Supper))
            {
                this.button10.Visible = true;
                this.button9.Visible = false;
                this.button8.Visible = false;

                this.button10.Text = "夜宵 Midnight snack";

                this.button10MealCode = "supper";
                this.mealClass = "Supper";
                this.button5.Focus();
                this.button3.BackColor = System.Drawing.Color.White;
                this.button4.BackColor = System.Drawing.Color.White;
                this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(155)))), ((int)(((byte)(198)))));
                this.button6.BackColor = System.Drawing.Color.White;
                this.button7.BackColor = System.Drawing.Color.White;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.lbErrorMsg.Text = "不能享受该服务";
            }
        }


        /// <summary>
        /// 咖啡
        /// </summary>
        private void button6Click()
        {
            if (MealPolicyCheck(Meal.Coffice))
            {
                this.button10.Visible = true;
                this.button9.Visible = false;
                this.button8.Visible = false;
                this.button10.Enabled = this.ttbIsCoffee;

                this.button10.Text = "咖啡 Coffee";

                this.button10MealCode = "coffee";
                this.mealClass = "Coffee";
                this.button6.Focus();
                this.button3.BackColor = System.Drawing.Color.White;
                this.button4.BackColor = System.Drawing.Color.White;
                this.button5.BackColor = System.Drawing.Color.White;
                this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(155)))), ((int)(((byte)(198)))));
                this.button7.BackColor = System.Drawing.Color.White;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.lbErrorMsg.Text = "不能享受该服务";
            }
        }

        /// <summary>
        /// 中餐、咖啡 、夜宵 、早餐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            int rel = RepeatedMealCheckWithExtra(lastIndexCache);
            if (rel > 0)
            {
                int index = mealRecordCache.FindIndex(x => x.code == this.button10MealCode);
                if (index == -1)
                {
                    flagForSyncMasterInfo.WaitOne();
                    flagForSyncMasterInfo.Reset();
                    mealRecordCache.Add(new Mealrecord()
                    {
                        code = this.button10MealCode,
                        name = this.button10.Text,
                        mealClass = this.mealClass,
                        price = mealPriceCache.Find(x => x.code == this.button10MealCode).price,
                        quantity = 1,
                        EXTRA_MEAL = (rel==2)
                    });
                    flagForSyncMasterInfo.Set();
                }
                else
                {
                    mealRecordCache[index].quantity++;
                    mealRecordCache[index].EXTRA_MEAL = (rel == 2);

                }
                this.listView1.Items.Clear();
                foreach (var item in mealRecordCache)
                {
                    this.listView1.Items.Add(string.Format("{0} * {1}", item.name, item.quantity));
                    if (this.button10MealCode == item.code && item.code == "coffee")
                    {
                        this.total += item.price;
                    }
                }
                this.label4.Text = string.Format("{0} 元", this.total);
                this.button1.Enabled = true;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            }

        }
        /// <summary>
        /// 西餐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            int rel = RepeatedMealCheckWithExtra(lastIndexCache);
            if (rel > 0)
            {
                this.mealCode = this.button9MealCode;
                int index = mealRecordCache.FindIndex(x => x.code == this.button9MealCode);
                if (index == -1)
                {
                    flagForSyncMasterInfo.WaitOne();
                    flagForSyncMasterInfo.Reset();
                    mealRecordCache.Add(new Mealrecord()
                    {
                        code = this.button9MealCode,
                        name = this.button9.Text,
                        mealClass = this.mealClass,
                        price = mealPriceCache.Find(x => x.code == this.button9MealCode).price,
                        quantity = 1,
                        EXTRA_MEAL = (rel == 2)
                    });
                    flagForSyncMasterInfo.Set();
                }
                else
                {
                    mealRecordCache[index].quantity++;
                    mealRecordCache[index].EXTRA_MEAL = (rel == 2);
                }
                this.listView1.Items.Clear();
                foreach (var item in mealRecordCache)
                {
                    this.listView1.Items.Add(string.Format("{0} * {1}", item.name, item.quantity));
                    if (this.button9MealCode == item.code && item.code == "coffee")
                    {
                        this.total += item.price;
                    }
                }
                this.label4.Text = string.Format("{0} 元", this.total);
                this.button1.Enabled = true;
                this.button8.BackColor = System.Drawing.Color.White;
                this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
                this.button10.BackColor = System.Drawing.Color.White;
            }
        }
        /// <summary>
        /// 特色餐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (RepeatedMealCheck(lastIndexCache))
            {
                this.mealCode = this.button8MealCode;
                int index = mealRecordCache.FindIndex(x => x.code == this.button8MealCode);
                if (index == -1)
                {
                    flagForSyncMasterInfo.WaitOne();
                    flagForSyncMasterInfo.Reset();
                    mealRecordCache.Add(new Mealrecord()
                    {
                        code = this.button8MealCode,
                        name = this.button8.Text,
                        mealClass = this.mealClass,
                        price = mealPriceCache.Find(x => x.code == this.button8MealCode).price,
                        quantity = 1
                    });
                    flagForSyncMasterInfo.Set();
                }
                else
                {
                    mealRecordCache[index].quantity++;
                }
                this.listView1.Items.Clear();
                foreach (var item in mealRecordCache)
                {
                    this.listView1.Items.Add(string.Format("{0} * {1}", item.name, item.quantity));
                    if (this.button8MealCode == item.code && item.code == "coffee")
                    {
                        this.total += item.price;
                    }
                }
                this.label4.Text = string.Format("{0} 元", this.total);
                this.button1.Enabled = true;
                this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
                this.button9.BackColor = System.Drawing.Color.White;
                this.button10.BackColor = System.Drawing.Color.White;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.initForm();
            this.unableForm();
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox1.Focus();
            this.button3.BackColor = System.Drawing.Color.White;
            this.button4.BackColor = System.Drawing.Color.White;
            this.button5.BackColor = System.Drawing.Color.White;
            this.button6.BackColor = System.Drawing.Color.White;
            this.button7.BackColor = System.Drawing.Color.White;
        }
        private void initForm()
        {
            this.button1.Enabled = false;
            this.button10.Enabled = true;
            this.button9.Enabled = true;
            this.button8.Enabled = true;
            this.button7.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
            this.button6.Enabled = true;

            this.button10.Visible = false;
            this.button9.Visible = false;
            this.button8.Visible = false;
            this.label4.Text = "";
            this.quantity = 0;
            this.total = 0;
            this.listView1.Items.Clear();
            this.mealRecordCache.Clear();
        }

        private void unableForm()
        {
            this.button1.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            this.button9.Enabled = false;
            this.button8.Enabled = false;
            this.button10.Enabled = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                flagForSyncMasterInfo.WaitOne();
                flagForSyncMasterInfo.Reset();
                this.unableForm();
                this.ttbEmployeeName = string.Empty;
                this.ttbIsLunch = false;
                this.ttbIsDinner = false;
                this.ttbIsSupper = false;
                this.ttbIsCoffee = false;
                this.ttbMasterRowId = string.Empty;
                //清空错误提示框
                this.lbErrorMsg.Text = string.Empty;


                if (string.IsNullOrEmpty(this.textBox1.Text))
                    return;

                this.initForm();
                bool flag = false;
                bool success = true;
                if (Properties.Settings.Default.ImmediatelySync)
                {
                    #region  立即读取数据
                    try
                    {
                        using (SqlConnection con = new SqlConnection(Properties.Settings.Default.ConnString))
                        {
                            string sql = "select A.ROW_ID,A.EMPLOYEE_NAME,A.EMPLOYEE_PY,A.IS_CHINESE_FOOD,A.IS_WEST_FOOD,A.IS_SPECIAL_FOOD,A.IS_COFFEE,A.CARD_STATUS from MSDS_Customer A where A.EMPLOYEE_CARD=" + this.textBox1.Text;

                            SqlCommand cmd = con.CreateCommand();
                            SqlCommand cmd1 = con.CreateCommand();
                            cmd.CommandText = sql;
                            con.Open();



                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                int index = employeesCache.FindIndex(x => x.id == this.textBox1.Text);

                                if (reader["CARD_STATUS"].ToString() == "N")
                                {
                                    MessageBox.Show("卡号已停用No access！");
                                    this.button3.Enabled = false;
                                    this.button4.Enabled = false;
                                    this.button5.Enabled = false;
                                    this.button6.Enabled = false;
                                    this.button7.Enabled = false;
                                    this.textBox1.Text = "";
                                    success = false;
                                }
                                else
                                {
                                    this.ttbEmployeeName = reader["EMPLOYEE_PY"].ToString();
                                    this.ttbIsLunch = bool.Parse(reader["IS_CHINESE_FOOD"].ToString());
                                    this.ttbIsDinner = bool.Parse(reader["IS_WEST_FOOD"].ToString());
                                    this.ttbIsSupper = bool.Parse(reader["IS_SPECIAL_FOOD"].ToString());
                                    this.ttbIsCoffee = bool.Parse(reader["IS_COFFEE"].ToString());
                                    this.ttbMasterRowId = reader["ROW_ID"].ToString();
                                    this.textBox2.Text = reader["EMPLOYEE_NAME"].ToString();
                                    reader.Close();
                                }
                                #region 统计当月咖啡消费金额
                                int year = DateTime.Now.Year;
                                int month = DateTime.Now.Month;
                                sql = "select SUM(ISNULL(A.MEAL_AMOUNT,0)) from HU_MEAL_RECORD A where A.EMPLOYEE_ID = '" + reader["ROW_ID"].ToString() + "' AND A.MEAL_TYPE_CODE ='coffee' AND DATEPART(year,A.MEAL_DATETIME) =" + year + " AND DATEPART(mm,A.MEAL_DATETIME)=" + month;
                                cmd1.CommandText = sql;
                                string value = Convert.ToString(cmd1.ExecuteScalar());
                                this.textBox3.Text = string.IsNullOrEmpty(Convert.ToString(value)) ? "0" : Convert.ToString(value);
                                #endregion

                                
                                if (index != -1)
                                {
                                    employeesCache[index].Lunch = bool.Parse(reader["IS_CHINESE_FOOD"].ToString());
                                    employeesCache[index].Dinner = bool.Parse(reader["IS_WEST_FOOD"].ToString());
                                    employeesCache[index].Supper = bool.Parse(reader["IS_SPECIAL_FOOD"].ToString());
                                    employeesCache[index].IsCoffee = bool.Parse(reader["IS_COFFEE"].ToString());
                                    employeesCache[index].MasterRowId = reader["ROW_ID"].ToString();
                                    employeesCache[index].name = reader["EMPLOYEE_NAME"].ToString();
                                    employeesCache[index].coffeeFee = double.Parse(this.textBox3.Text);
                                    employeesCache[index].status = bool.Parse(reader["CARD_STATUS"].ToString());
                                    
                                }
                                else
                                {
                                    employeesCache.Add(new EmployeeInfo()
                                    {
                                        id = this.textBox1.Text,
                                        name = reader["EMPLOYEE_NAME"].ToString(),
                                        MasterRowId = reader["ROW_ID"].ToString(),
                                        Lunch = bool.Parse(reader["IS_CHINESE_FOOD"].ToString()),
                                        Dinner = bool.Parse(reader["IS_WEST_FOOD"].ToString()),
                                        Supper = bool.Parse(reader["IS_SPECIAL_FOOD"].ToString()),
                                        IsCoffee = bool.Parse(reader["IS_COFFEE"].ToString()),
                                        status = bool.Parse(reader["CARD_STATUS"].ToString())
                                    });
                                }
                            }
                            else
                            {
                                MessageBox.Show("卡号不存在No access!");
                                this.textBox1.Text = "";
                                this.button3.Enabled = false;
                                this.button4.Enabled = false;
                                this.button5.Enabled = false;
                                this.button6.Enabled = false;
                                this.button7.Enabled = false;
                                success = false;
                            }
                            con.Close();
                        }
                    }
                    catch
                    {
                        flag = true;
                    }
                    #endregion
                }
                if (flag || !Properties.Settings.Default.ImmediatelySync)
                {

                    int i = employeesCache.FindIndex(x => x.id == this.textBox1.Text);
                    if (i != -1)
                    {
                        if (!employeesCache[i].status)
                        {
                            MessageBox.Show("卡号已停用No access！");
                            this.textBox1.Text = "";
                            this.button3.Enabled = false;
                            this.button4.Enabled = false;
                            this.button5.Enabled = false;
                            this.button6.Enabled = false;
                            this.button7.Enabled = false;
                            success = false;
                        }
                        else
                        {
                            this.ttbEmployeeName = employeesCache[i].name;
                            this.ttbIsLunch = employeesCache[i].Lunch;
                            this.ttbIsDinner = employeesCache[i].Dinner;
                            this.ttbIsSupper = employeesCache[i].Supper;
                            this.ttbIsCoffee = employeesCache[i].IsCoffee;
                            this.ttbMasterRowId = employeesCache[i].MasterRowId;
                            this.textBox2.Text = employeesCache[i].name;
                            this.textBox3.Text = employeesCache[i].coffeeFee.ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("卡号不存在No access!");
                        this.button3.Enabled = false;
                        this.button4.Enabled = false;
                        this.button5.Enabled = false;
                        this.button6.Enabled = false;
                        this.button7.Enabled = false;
                        this.textBox1.Text = "";
                        success = false;
                    }
                }
                flagForSyncMasterInfo.Set();
                if (success)
                {
                    if (lastIndex == Meal.Lunch)
                    {
                        button3Click();
                    }
                    if (lastIndex == Meal.Dinner)
                    {
                        button4Click();
                    }
                    if (lastIndex == Meal.Supper)
                    {
                        button5Click();
                    }
                    if (lastIndex == Meal.Coffice)
                    {
                        button6Click();
                    }
                    if (lastIndex == Meal.Breakfast)
                    {
                        button7Click();
                    }
                }
            }
        }


        /// <summary>
        /// 检查饭卡用餐权限
        /// </summary>
        private bool MealPolicyCheck(Meal meal)
        {
            bool canEat = false;
            EmployeeInfo currentEmp = employeesCache.SingleOrDefault(x => x.MasterRowId == this.ttbMasterRowId);

            switch (meal)
            {
                case Meal.Lunch:
                    canEat = currentEmp.Lunch;
                    break;
                case Meal.Dinner:
                    canEat = currentEmp.Dinner;
                    break;
                case Meal.Supper:
                    canEat = currentEmp.Supper;
                    break;
                case Meal.Coffice:
                    canEat = currentEmp.IsCoffee;
                    break;
                case Meal.Breakfast:
                    canEat = currentEmp.Breakfast;
                    break;
                    
            }

            return canEat;

        }


        private bool RepeatedMealCheck(Meal meal)
        {
            int startTime = 0;
            int endTime = 0;
            DateTime dt = DateTime.Now;
            DateTime dtDate = dt.Date;
            

            EmployeeInfo currentEmp = employeesCache.SingleOrDefault(x => x.MasterRowId == this.ttbMasterRowId);

            switch (meal)
            {
                case Meal.Breakfast:
                    startTime = Properties.Settings.Default.BreakfastStartTime;
                    endTime = Properties.Settings.Default.BreakfastEndTime;
                    break;
                case Meal.Lunch:
                    startTime = Properties.Settings.Default.LunchStartTime;
                    endTime = Properties.Settings.Default.LunchEndTime;
                    break;
                case Meal.Dinner:
                    startTime = Properties.Settings.Default.DinnerStartTime;
                    endTime = Properties.Settings.Default.DinnerEndTime;
                    break;
                case Meal.Supper:
                    startTime = Properties.Settings.Default.SupperStartTime;
                    endTime = Properties.Settings.Default.SupperEndTime;
                    break;
            }

            if (currentEmp != null)
            {
                if (meal != Meal.Coffice && (dt.Hour < startTime || dt.Hour > endTime))
                {
                    lbErrorMsg.Text = "未到用餐时间";
                    return false;
                }

                if (currentEmp.lastMealTime != null &&
                    meal != Meal.Coffice &&
                    currentEmp.lastMealTime.Date == dtDate &&
                    currentEmp.emp_id != "999999D" &&
                    currentEmp.lastMealTime.Hour >= startTime &&
                    currentEmp.lastMealTime.Hour < endTime)
                {
                    lbErrorMsg.Text = "同一用餐时段不能重复点餐";
                    return false;
                }

                //非部门卡只能点一份咖啡除外
                if (meal != Meal.Coffice && currentEmp.emp_id != "999999D" && this.mealRecordCache != null && this.mealRecordCache.Count > 0)
                {
                    this.mealRecordCache.Clear();
                }

                return true;
            }
            else
            {
                lbErrorMsg.Text = "未找到该卡号信息";
                return false;
            }
        }

        private int RepeatedMealCheckWithExtra(Meal meal)
        {
            int startTime = 0;
            int endTime = 0;

            DateTime dt = DateTime.Now;
            //DateTime dt = DateTime.Now.AddHours(-14);
            DateTime dtDate = dt.Date;


            EmployeeInfo currentEmp = employeesCache.SingleOrDefault(x => x.MasterRowId == this.ttbMasterRowId);
            //currentEmp.lastMealTime = new DateTime(2017,1,14,12,12,12);
            switch (meal)
            {
                case Meal.Breakfast:
                    startTime = Properties.Settings.Default.BreakfastStartTime;
                    endTime = Properties.Settings.Default.BreakfastEndTime;
                    break;
                case Meal.Lunch:
                    startTime = Properties.Settings.Default.LunchStartTime;
                    endTime = Properties.Settings.Default.LunchEndTime;
                    break;
                case Meal.Dinner:
                    startTime = Properties.Settings.Default.DinnerStartTime;
                    endTime = Properties.Settings.Default.DinnerEndTime;
                    break;
                case Meal.Supper:
                    startTime = Properties.Settings.Default.SupperStartTime;
                    endTime = Properties.Settings.Default.SupperEndTime;
                    break;
            }

            if (currentEmp != null)
            {
                if (meal != Meal.Coffice && (dt.Hour < startTime || dt.Hour > endTime))
                {
                    lbErrorMsg.Text = "未到用餐时间";
                    return 0;
                }

                if (currentEmp.lastMealTime != null &&
                    meal != Meal.Coffice &&
                    currentEmp.lastMealTime.Date == dtDate &&
                    currentEmp.emp_id != "999999D" &&
                    currentEmp.lastMealTime.Hour >= startTime &&
                    currentEmp.lastMealTime.Hour < endTime)
                {
                    lbErrorMsg.Text = "同一用餐时段不能重复点餐";
                    return 0;
                }



                //非部门卡只能点一份
                if (meal != Meal.Coffice && currentEmp.emp_id != "999999D" && this.mealRecordCache != null && this.mealRecordCache.Count > 0)
                {
                    this.mealRecordCache.Clear();
                }

                //判断是不是加班餐
                if (meal != Meal.Breakfast && !currentEmp.emp_id.StartsWith("999999"))
                {
                    if (currentEmp.lastMealTime.Date == dtDate && currentEmp.lastMealTime.Hour >= Properties.Settings.Default.LunchStartTime && currentEmp.lastMealTime.Hour <= Properties.Settings.Default.DinnerEndTime)
                    {
                        return 2;
                    }
                }

                return 1;
            }
            else
            {
                lbErrorMsg.Text = "未找到该卡号信息";
                return 0;
            }
            
        }

        public enum Meal
        {
            Lunch = 1, Dinner = 2, Supper = 3 , Coffice =4, Breakfast = 5
        }

    }
}

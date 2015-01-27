using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using MCIFramework.Models;
using MCIFramework.ViewModels;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Controls;
using Newtonsoft.Json.Converters;


namespace MCIFramework.Helper
{
  
    public interface IOService
    {
        string OpenFileDialog(string defaultPath);

        //Other similar untestable IO operations
        Stream OpenFile(string path);
    }

    //============================================================================================================================================

    public class ExcelData
    {
        public DataView Data
        {

            get
            {
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;
                Excel.Range range;
                //workbook = excelApp.Workbooks.Open(Environment.CurrentDirectory + "..\\Resources\\Assessments\\1\\xlsx\\Strategy Assessment.xlsx");
                workbook = excelApp.Workbooks.Open("F:\\Projects\\mci\\Resources\\Assessments\\1\\xlsx\\Strategy Assessment.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                
                
                worksheet = (Excel.Worksheet)workbook.Sheets["6 Data for upload"];

                int column = 0;
                int row = 0;

                range = worksheet.UsedRange;
                DataTable dt = new DataTable();

                //read column 1 for the header text and add
                for (column = 1; column <= range.Columns.Count; column++)
                {
                    //dr[column - 1] = (range.Cells[row, column] as Excel.Range).Value2.ToString();
                    dt.Columns.Add((range.Cells[1, column] as Excel.Range).Value2.ToString());
                }

                //read through data row
                for (row = 2; row <= range.Rows.Count; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (column = 1; column <= range.Columns.Count; column++)
                    {
                        //dr[column - 1] = (range.Cells[row, column] as Excel.Range).Value2.ToString();
                        dr[column - 1] = Convert.ToString((range.Cells[row, column] as Excel.Range).Value2);
                    }
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                }
                workbook.Close(true, Missing.Value, Missing.Value);
                excelApp.Quit();
                return dt.DefaultView;
            }
        }
    }


    //============================================================================================================================================
    public class OpenFileDialogVM : ViewModelBase
    {
        private string _selectedPath;
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; OnPropertyChanged("SelectedPath"); }
        }

        //private RelayCommand _openCommand;
        //public RelayCommand OpenCommand
        //{
        //    //You know the drill.
        //}

        private IOService _ioService;
        public OpenFileDialogVM(IOService ioService)
        {
            _ioService = ioService;
            //OpenCommand = new RelayCommand(OpenFile);
        }

        private void OpenFile()
        {
            SelectedPath = _ioService.OpenFileDialog(@"Resources\Assessments\1\xlsx\Strategy Assessment.xlsx");
            if (SelectedPath == null)
            {
                SelectedPath = string.Empty;
            }
        }
    }
    //============================================================================================================================================
    public class DialogHelper : DependencyObject
    {

        private string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        private static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(DialogHelper),
            new UIPropertyMetadata(new PropertyChangedCallback(FileNameProperty_Changed)));

        private static void FileNameProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("DialogHelper.FileName = {0}", e.NewValue);
        }

        public ICommand OpenFile { get; private set; }

        public DialogHelper()
        {
            OpenFile = new RelayCommand(OpenFileAction);
        }

        private void OpenFileAction(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {
                FileName = dlg.FileName;
            }
        }
    }

    //============================================================================================================================================

    namespace WPF_Excel_Reader_Writer
    {
        

        public class DataAccess
        {
            OleDbConnection Conn;
            OleDbCommand Cmd;

            public DataAccess()
            {
                //F:\\Projects\\mci\\Resources\\Assessments\\1\\xlsx\\Strategy Assessment.xlsx
                //F:\\Projects\\mci\\Resources\\Assessments\\1\\xlsx\\Social Media Assessment.xlsx
                //F:\\Projects\\mci\\Resources\\Assessments\\1\\xlsx\\Website Assessment.xlsx
                Conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\\Projects\\mci\\Resources\\Assessments\\1\\xlsx\\Strategy Assessment.xlsx;Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"");
            }

            /// <summary>
            /// Method to Get All the Records from Excel
            /// </summary>
            /// <returns></returns>
            public async Task<ObservableCollection<dataUploaded>> GetDataFormExcelAsync()
            {
                ObservableCollection<dataUploaded> dataUploaded = new ObservableCollection<dataUploaded>();
                await Conn.OpenAsync();
                Cmd = new OleDbCommand();
                Cmd.Connection = Conn;
                //
                //Strategy Assessment.xlsx      =  6 Data for upload$
                //Social Media Assessment.xlsx  = 10 Data for upload$
                //Website Assessment.xlsx       =  3 Data for upload$

                Cmd.CommandText = "Select * from [6 Data for upload$]";
                var Reader = await Cmd.ExecuteReaderAsync();

                //MCIFramework.Models.Assessment.Result result = new MCIFramework.Models.Assessment.Result();

                while (Reader.Read())
                {
                    //here to play around with data populating

                    dataUploaded.Add(new dataUploaded()
                    {
                        level = Convert.ToInt32(Reader["Level"]),
                        title = Reader["Title"].ToString(),
                        score = Reader["Score"].ToString(),
                        description = Reader["Description"].ToString(),
                        criteria = Reader["Criteria"].ToString(),
                        indicator = Reader["Indicator for"].ToString(),
                        recommendations = Reader["Recommendations"].ToString(),
                        assessor = Reader["Assessor comments"].ToString()

                    });

                    
                    //product.ExpiryDate = new DateTime(2008, 12, 28);

                    


                    //F:\Projects\mci\Resources\Assessments\1\Report\assets\MCI-Framework\js\strategy.js
//                    JObject o1 = JObject.Parse(@"{
//                      'FirstName': 'John',
//                      'LastName': 'Smith',
//                      'Enabled': false,
//                      'Roles': [ 'User' ]
//                    }");

//                   JObject o2 = JObject.Parse(@"{
//                      'Enabled': true,
//                      'Roles': [ 'User', 'Admin' ]
//                    }");

//                    o1.Merge(o2, new JsonMergeSettings
//                    {
//                        // union array values together to avoid duplicates
//                        MergeArrayHandling = MergeArrayHandling.Union
//                    });

//                    string json = o1.ToString();

                }
                Reader.Close();
                Conn.Close();

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                using (StreamWriter sw = new StreamWriter(@"c:\json.txt"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //serializer.Serialize(writer, product);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }


                return dataUploaded;
            }

            /// <summary>
            /// Method to Insert Record in the Excel
            /// S1. If the EmpNo =0, then the Operation is Skipped.
            /// S2. If the Employee is already exist, then it is taken for Update
            /// </summary>
            /// <param name="Emp"></param>
            /// 
            //============================================================================================================================================
            //add your custom read from dadtabase if needed
            //public async Task<bool> InsertOrUpdateRowInExcelAsync(dataUploaded Emp)
            //{
            //    bool IsSave = false;
            //    //S1
            //    if (Emp.EmpNo != 0)
            //    {
            //        await Conn.OpenAsync();
            //        Cmd = new OleDbCommand();
            //        Cmd.Connection = Conn;
            //        Cmd.Parameters.AddWithValue("@EmpNo", Emp.EmpNo);
            //        Cmd.Parameters.AddWithValue("@EmpName", Emp.EmpName);
            //        Cmd.Parameters.AddWithValue("@Salary", Emp.Salary);
            //        Cmd.Parameters.AddWithValue("@DeptName", Emp.DeptName);
            //        //S2
            //        if (!CheckIfRecordExistAsync(Emp).Result)
            //        {
            //            Cmd.CommandText = "Insert into [Sheet1$] values (@EmpNo,@EmpName,@Salary,@DeptName)";
            //        }
            //        else
            //        {
            //            if (Emp.EmpName != String.Empty || Emp.DeptName != String.Empty)
            //            {
            //                Cmd.CommandText = "Update [Sheet1$] set EmpNo=@EmpNo,EmpName=@EmpName,Salary=@Salary,DeptName=@DeptName where EmpNo=@EmpNo";
            //            }
            //        }
            //        int result = await Cmd.ExecuteNonQueryAsync();
            //        if (result > 0)
            //        {
            //            IsSave = true;
            //        }
            //        Conn.Close();
            //    }
            //    return IsSave;

            //}
            //============================================================================================================================================


            /// <summary>
            /// The method to check if the record is already available
            /// in the workgroup
            /// </summary>
            /// <param name="emp"></param>
            /// <returns></returns>
            //private async Task<bool> CheckIfRecordExistAsync(Employee emp)
            //{
            //    bool IsRecordExist = false;
            //    //select the sheet name
            //    Cmd.CommandText = "Select * from [Sheet1$] where EmpNo=@EmpNo";
            //    var Reader = await Cmd.ExecuteReaderAsync();
            //    if (Reader.HasRows)
            //    {
            //        IsRecordExist = true;
            //    }

            //    Reader.Close();
            //    return IsRecordExist;
            //}
        }

    }
    //============================================================================================================================================
 


    //============================================================================================================================================





    //============================================================================================================================================



    //============================================================================================================================================




    //============================================================================================================================================





    //============================================================================================================================================


}

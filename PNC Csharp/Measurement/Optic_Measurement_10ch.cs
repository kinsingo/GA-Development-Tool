﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;//Port 190530
using CASDK2;
using System.Security.AccessControl;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Xml.Serialization;

////using References
using SectionLib;
using System.IO.MemoryMappedFiles;
using System.IO;

using System.Threading.Tasks;
using System.Globalization;
using Microsoft.VisualBasic;

using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace PNC_Csharp
{
    public partial class Optic_Measurement_10ch : Form
    {
        XmlSerializer mySerializer = new XmlSerializer(typeof(Multi_CH_Measurement_Preferences));//Used For Saving and Loading Setting

        bool CA_connect = false;
        int ca_count = 0;

        // CA-410 관련 변수
        const int MAX_COUNT = 10;
        int err = 0;
        CASDK2Ca200 objCa200;
        CASDK2Cas objCas;
        CASDK2Ca[] objCa;
        CASDK2Probes[] objProbes;
        CASDK2OutputProbes[] objOutputProbes;
        CASDK2Probe[] objProbe;
        CASDK2Memory[] objMemory;
        bool[] connect;
        CASDK2DeviceData[] pDeviceData;

        // CA-310 관련 변수
        int probe_count = 10;
        public CA200SRVRLib.Ca200[] objCa200_310;
        public CA200SRVRLib.Ca[] objCa_310;
        public CA200SRVRLib.Cas[] objCas_310;
        public CA200SRVRLib.Probes[] objProbes_310;
        public CA200SRVRLib.Memory[] objMemory_310;

        // PNC 관련 변수
        bool[] check_PNC_CH = new bool[10];
        int[] PNC_CH_CA_NO = new int[10];
        int[] CA_NO_PNC_CH = new int[10];
        bool[] check_SET = new bool[6];
        int[] SET_sequence = new int[6];
        int PNC_CH_count = 0;
        DataGridView[] CH_Grid_View = new DataGridView[10];

        // GCS 관련 변수
        bool[] check_GCS_DBV = new bool[20];
        string[] value_GCS_DBV = new string[20];
        int GCS_check_count = 0;

        // BCS 관련 변수
        bool[] check_BCS_Gray = new bool[20];
        string[] value_BCS_Gray = new string[20];
        bool[] check_BCS_Range = new bool[3];
        string[] value_BCS_Range_min_DBV = new string[3];
        string[] value_BCS_Range_max_DBV = new string[3];
        string[] value_BCS_Range_DBV_step = new string[3];
        int BCS_check_count = 0;
        int BCS_range_check_count = 0;

        // AoD GCS 관련 변수
        bool[] check_AOD_GCS_DBV = new bool[3];
        string[] value_AOD_GCS_DBV = new string[3];
        int AOD_GCS_check_count = 0;

        // Gamma Crush 관련 변수
        bool[] check_Gamma_Crush = new bool[10];
        bool[] check_Gamma_Crush_color = new bool[4];
        string[] value_Gamma_Crush_DBV = new string[10];
        string[] value_Gamma_Crush_Gray = new string[10];
        int Gamma_Crush_check_count = 0;
        int Gamma_Crush_Color_check_count = 0;

        // IR Drop DeltaE 관련 변수
        Color[] IR_Drop_PTN = new Color[25];

        // deltaE 및 gamma 산출 변수
        XYLv[][][] data_list = new XYLv[][][] {
            new XYLv[10][], new XYLv[10][], new XYLv[10][], new XYLv[10][], new XYLv[10][], new XYLv[10][], };
        int[] index_list;
        int index_max;
        int count_measure;
        int data_index = 0;

        int meausre_count = 1;
        int row_count = 1;
        object[] first_line = new object[19];

        Color Color_Set1 = Color.FromArgb(255, 150, 150);
        Color Color_Set2 = Color.FromArgb(255, 200, 150);
        Color Color_Set3 = Color.FromArgb(175, 175, 255);
        Color Color_Set4 = Color.FromArgb(150, 200, 255);
        Color Color_Set5 = Color.FromArgb(40, 170, 160);
        Color Color_Set6 = Color.FromArgb(200, 255, 200);

        TextBox Textbox_Script_Set1_Final;
        TextBox Textbox_Script_Set2_Final;
        TextBox Textbox_Script_Set3_Final;
        TextBox Textbox_Script_Set4_Final;
        TextBox Textbox_Script_Set5_Final;
        TextBox Textbox_Script_Set6_Final;

        DateTime Start_Time;

        Form1 f1()
        {
            return (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
        }
            

        public void UpdateModelInfo()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            label_model.Text = "Model : " + f1.current_model.Get_Current_Model_Name().ToString();
            label_Max_DBV.Text = "Max DBV : " + f1.current_model.get_DBV_Max().ToString() + " (0x" + f1.current_model.get_DBV_Max().ToString("X3") + ")";
            label_Size.Text = "Size : " + f1.current_model.get_X().ToString() + " * " + f1.current_model.get_Y().ToString();
        }

        public struct MinMax
        {
            public int Min;
            public int Max;
        }

        public struct DeltaE
        {
            public double X;
            public double Y;
            public double Z;

            public double Xn;
            public double Yn;
            public double Zn;

            public double fx;
            public double fy;
            public double fz;

            public double L;
            public double a;
            public double b;

            public double delta_a;
            public double delta_b;
            public double delta_L;
            
            public double delta_C;
            public double delta_E;
        }

        public struct RGB
        {
            public double data_R;
            public double data_G;
            public double data_B;
        }

        public bool GCS_measure = false;
        public bool BCS_measure = false;
        public bool AOD_GCS_measure = false;
        public bool IR_Drop_DeltaE_measure = false;
        public bool Gamma_Crush_measure = false;
        public bool aging_flag = true;
        public bool stop_flag = false;

        private static Optic_Measurement_10ch Instance;
        public static Optic_Measurement_10ch getInstance()
        {
            if (Instance == null)
                Instance = new Optic_Measurement_10ch();

            return Instance;
        }


        public static bool IsIstanceNull()
        {
            if (Instance == null)
                return true;
            else
                return false;
        }

        public static void DeleteInstance()
        {
            Instance = null;
        }

        private Optic_Measurement_10ch()
        {
            InitializeComponent();
           

            try
            {
                this.CA_zero_cal_button.Hide();
                this.CA_Test_button.Hide();
                this.groupBox_Measuremode.Hide();
                this.groupBox_Syncmode.Hide();
                this.button_Measure.Hide();
                this.button_channel_change.Hide();

                for (int i = 0; i < 19; i++)
                {
                    if (i == 0) first_line[i] = "/ DBV";
                    else if (i % 3 == 0) first_line[i] = "Lv";
                    else if (i % 3 == 1) first_line[i] = "x";
                    else if (i % 3 == 2) first_line[i] = "y";
                }

                CH_Grid_View[0] = dataGridView_CH1;
                CH_Grid_View[1] = dataGridView_CH2;
                CH_Grid_View[2] = dataGridView_CH3;
                CH_Grid_View[3] = dataGridView_CH4;
                CH_Grid_View[4] = dataGridView_CH5;
                CH_Grid_View[5] = dataGridView_CH6;
                CH_Grid_View[6] = dataGridView_CH7;
                CH_Grid_View[7] = dataGridView_CH8;
                CH_Grid_View[8] = dataGridView_CH9;
                CH_Grid_View[9] = dataGridView_CH10;

                CH_Grid_View_all_initial_setting();

                dataGridView_CA1_5_initial_setting();
                dataGridView_CA6_10_initial_setting();

                Button_Click_Enable(false);
                btn_CA_Connect.Show();
            }
            catch (Exception er)
            {
                DisplayError(er);
                System.Windows.Forms.Application.Exit();
            }
        }

        private void Uri_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView_CA1_5_initial_setting()
        {
            dataGridView_CA1_5.EnableHeadersVisualStyles = false;
            dataGridView_CA1_5.ReadOnly = true;
            dataGridView_CA1_5.Columns.Add("CA1", "CA1");
            dataGridView_CA1_5.Columns.Add("CA2", "CA2");
            dataGridView_CA1_5.Columns.Add("CA3", "CA3");
            dataGridView_CA1_5.Columns.Add("CA4", "CA4");
            dataGridView_CA1_5.Columns.Add("CA5", "CA5");
            dataGridView_CA1_5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_CA1_5.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font, System.Drawing.FontStyle.Bold);
            dataGridView_CA1_5.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dataGridView_CA1_5.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;
            dataGridView_CA1_5.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            for (int i = 0; i < dataGridView_CA1_5.ColumnCount; i++)
            {
                dataGridView_CA1_5.Rows[0].Cells[i].Value = "-";
                dataGridView_CA1_5.Rows[0].Cells[i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void dataGridView_CA6_10_initial_setting()
        {
            dataGridView_CA6_10.EnableHeadersVisualStyles = false;
            dataGridView_CA6_10.ReadOnly = true;
            dataGridView_CA6_10.Columns.Add("CA6", "CA6");
            dataGridView_CA6_10.Columns.Add("CA7", "CA7");
            dataGridView_CA6_10.Columns.Add("CA8", "CA8");
            dataGridView_CA6_10.Columns.Add("CA9", "CA9");
            dataGridView_CA6_10.Columns.Add("CA10", "CA10");
            dataGridView_CA6_10.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_CA6_10.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font, System.Drawing.FontStyle.Bold);
            dataGridView_CA6_10.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dataGridView_CA6_10.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;
            dataGridView_CA6_10.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            for (int i = 0; i < dataGridView_CA6_10.ColumnCount; i++)
            {
                dataGridView_CA6_10.Rows[0].Cells[i].Value = "-";
                dataGridView_CA6_10.Rows[0].Cells[i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void CH_Grid_View_all_initial_setting()
        {
            for (int ch = 0; ch < 10; ch++)
            {
                CH_Grid_View[ch].EnableHeadersVisualStyles = false;
                CH_Grid_View[ch].ReadOnly = true;

                CH_Grid_View[ch].Columns.Add("Gray", "Gray");
                CH_Grid_View[ch].Columns.Add("SET1_x", "");
                CH_Grid_View[ch].Columns.Add("SET1_y", "SET1");
                CH_Grid_View[ch].Columns.Add("SET1_Lv", "");
                CH_Grid_View[ch].Columns.Add("SET2_x", "");
                CH_Grid_View[ch].Columns.Add("SET2_y", "SET2");
                CH_Grid_View[ch].Columns.Add("SET2_Lv", "");
                CH_Grid_View[ch].Columns.Add("SET3_x", "");
                CH_Grid_View[ch].Columns.Add("SET3_y", "SET3");
                CH_Grid_View[ch].Columns.Add("SET3_Lv", "");
                CH_Grid_View[ch].Columns.Add("SET4_x", "");
                CH_Grid_View[ch].Columns.Add("SET4_y", "SET4");
                CH_Grid_View[ch].Columns.Add("SET4_Lv", "");
                CH_Grid_View[ch].Columns.Add("SET5_x", "");
                CH_Grid_View[ch].Columns.Add("SET5_y", "SET5");
                CH_Grid_View[ch].Columns.Add("SET5_Lv", "");
                CH_Grid_View[ch].Columns.Add("SET6_x", "");
                CH_Grid_View[ch].Columns.Add("SET6_y", "SET6");
                CH_Grid_View[ch].Columns.Add("SET6_Lv", "");

                CH_Grid_View[ch].Rows.Add(first_line);

                CH_Grid_View[ch].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                for (int col = 0; col < CH_Grid_View[ch].ColumnCount; col++)
                {
                    CH_Grid_View[ch].Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    CH_Grid_View[ch].Columns[col].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (col == 0)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
                    }
                    else if (col < 4)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set1;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set1;
                    }
                    else if (col < 7)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set2;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set2;
                    }
                    else if (col < 10)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set3;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set3;
                    }
                    else if (col < 13)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set4;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set4;
                    }
                    else if (col < 16)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set5;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set5;
                    }
                    else if (col < 19)
                    {
                        CH_Grid_View[ch].Columns[col].DefaultCellStyle.BackColor = this.Color_Set6;
                        CH_Grid_View[ch].Columns[col].HeaderCell.Style.BackColor = this.Color_Set6;
                    }

                    CH_Grid_View[ch].Columns[col].Width = 50;
                }
                foreach (DataGridViewColumn column in this.CH_Grid_View[ch].Columns)
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void Button_Hide_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void Get_All_Serial_Port()
        {
            ca_count = 0;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                ca_count++;
            }

            pDeviceData = new CASDK2DeviceData[ca_count];
            CASDK2.CASDK2Discovery.SearchAllUSBDevices(ref pDeviceData);

            for (int ca = 0; ca < ca_count; ca++)
            {
                // CH1~5와 CH6~10 grid 분리 & CA serial number 끝 4자리 표시
                dataGridView_CA1_5.Rows[0].DefaultCellStyle.Font = new Font("굴림", 9);
                dataGridView_CA6_10.Rows[0].DefaultCellStyle.Font = new Font("굴림", 9);
                if (ca_count < 5) dataGridView_CA1_5.Rows[0].Cells[ca].Value = pDeviceData[ca].strSerialNo.Substring(4, pDeviceData[ca].strSerialNo.Length - 4);
                else dataGridView_CA6_10.Rows[0].Cells[ca - 5].Value = pDeviceData[ca].strSerialNo.Substring(4, pDeviceData[ca].strSerialNo.Length - 4);
            }
        }

        private string[] CA310_connect_check()
        {
            string[] ca_probe = new string[2];
            ca_count = 0;
            probe_count = 0;
            if (checkBox_CA310_1.Checked)
            {
                ca_count++;
                switch (Convert.ToInt32(textBox_CA310_no_of_probe_1.Text)-1)
                {
                    case 0:
                        ca_probe[0] = "1";
                        probe_count += 1;
                        break;
                    case 1:
                        ca_probe[0] = "12";
                        probe_count += 2;
                        break;
                    case 2:
                        ca_probe[0] = "123";
                        probe_count += 3;
                        break;
                    case 3:
                        ca_probe[0] = "1234";
                        probe_count += 4;
                        break;
                    case 4:
                        ca_probe[0] = "12345";
                        probe_count += 5;
                        break;
                    default:
                        ca_probe[0] = "1";
                        probe_count += 1;
                        break;
                }
            }
            if (checkBox_CA310_2.Checked)
            {
                ca_count++;
                switch (Convert.ToInt32(textBox_CA310_no_of_probe_2.Text) - 1)
                {
                    case 0:
                        ca_probe[1] = "1";
                        probe_count += 1;
                        break;
                    case 1:
                        ca_probe[1] = "12";
                        probe_count += 2;
                        break;
                    case 2:
                        ca_probe[1] = "123";
                        probe_count += 3;
                        break;
                    case 3:
                        ca_probe[1] = "1234";
                        probe_count += 4;
                        break;
                    case 4:
                        ca_probe[1] = "12345";
                        probe_count += 5;
                        break;
                    default:
                        ca_probe[1] = "1";
                        probe_count += 1;
                        break;
                }
            }

            return ca_probe;
        }
        private void radioButton_CA310_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_CA310_1.Show();
            checkBox_CA310_2.Show();
            textBox_CA310_no_of_probe_1.Show();
            textBox_CA310_no_of_probe_2.Show();
            label33.Show();
            label35.Show();

            button_Romote_on.Show();
            button_Romote_off.Show();
        }
        private void radioButton_CA410_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_CA310_1.Hide();
            checkBox_CA310_2.Hide();
            textBox_CA310_no_of_probe_1.Hide();
            textBox_CA310_no_of_probe_2.Hide();
            label33.Hide();
            label35.Hide();

            button_Romote_on.Hide();
            button_Romote_off.Hide();
        }

        private void btn_CA_Connect_Click(object sender, EventArgs e)
        {
            Button_Click_Enable(false);

            if (!CA_connect)
            {
                radioButton_CA310.Enabled = false;
                radioButton_CA410.Enabled = false;

                if (radioButton_CA410.Checked)
                {
                    CA_connect = CA410_connect();
                }
                else
                {
                    CA_connect = CA310_connect();
                }

                if (CA_connect)
                {
                    this.groupBox_Measuremode.Show();
                    this.groupBox_Syncmode.Show();
                    this.btn_CA_Connect.Text = "Disconnect";
                    this.btn_CA_Connect.ForeColor = System.Drawing.Color.White;

                    MultiSetting();
                    Button_Click_Enable(true);
                }
                else
                {
                    Discconect();
                    this.groupBox_Measuremode.Hide();
                    this.groupBox_Syncmode.Hide();
                    this.btn_CA_Connect.Text = "CA connect";
                    this.btn_CA_Connect.ForeColor = System.Drawing.Color.White;
                    Button_Click_Enable(false);
                    btn_CA_Connect.Show();
                }
            }
            else
            {
                Discconect();
                this.groupBox_Measuremode.Hide();
                this.groupBox_Syncmode.Hide();
                this.btn_CA_Connect.Text = "CA connect";
                this.btn_CA_Connect.ForeColor = System.Drawing.Color.White;
                Button_Click_Enable(false);
                btn_CA_Connect.Show();
            }
        }
        private bool CA410_connect()
        {
            Get_All_Serial_Port();

            //Create CA Object
            objCa = new CASDK2Ca[ca_count];
            objProbes = new CASDK2Probes[ca_count];
            objOutputProbes = new CASDK2OutputProbes[ca_count];
            objProbe = new CASDK2Probe[ca_count];
            objMemory = new CASDK2Memory[ca_count];
            connect = new bool[ca_count];

            objCa200 = new CASDK2Ca200();
            bool errorcheck = true;
            int errorcheck_count = 0;
                
            //int ca_port;
            for (int ca = 0; ca < ca_count; ca++)
            {
                errorcheck = GetErrorMessage(objCa200.SetConfiguration(ca + 1, "1", pDeviceData[ca].lPortNo, 38400, 0));
                if (errorcheck)
                {
                    connect[ca] = true;
                }
                else
                {
                    connect[ca] = false;
                    errorcheck_count++;
                }
                errorcheck = true;
            }

            err = objCa200.get_Cas(ref objCas);
            for (int ca = 0; ca < ca_count; ca++)
            {
                if (errorcheck_count != 0) break;
                else errorcheck_count = 0;

                errorcheck = GetErrorMessage(objCas.get_Item(ca + 1, ref objCa[ca]));
                errorcheck = GetErrorMessage(objCa[ca].get_Probes(ref objProbes[ca]));
                errorcheck = GetErrorMessage(objCa[ca].get_OutputProbes(ref objOutputProbes[ca]));
                errorcheck = GetErrorMessage(objCa[ca].get_Memory(ref objMemory[ca]));
                errorcheck = GetErrorMessage(objProbes[ca].get_Item(1, ref objProbe[ca]));
                errorcheck = GetErrorMessage(objOutputProbes[ca].AddAll());
                errorcheck = GetErrorMessage(objOutputProbes[ca].get_Item(1, ref objProbe[ca]));

                if (ca < 5 && connect[ca])
                {
                    dataGridView_CA1_5.Rows[0].Cells[ca].Style.ForeColor = Color.Green;
                }
                else if (ca < 5 && !connect[ca])
                {
                    dataGridView_CA1_5.Rows[0].Cells[ca].Style.ForeColor = Color.Red;
                }
                else if (ca > 4 && connect[ca])
                {
                    dataGridView_CA6_10.Rows[0].Cells[ca].Style.ForeColor = Color.Green;
                }
                else if (ca > 4 && !connect[ca])
                {
                    dataGridView_CA6_10.Rows[0].Cells[ca].Style.ForeColor = Color.Red;
                }

                if (!errorcheck) errorcheck_count++;
                errorcheck = true;
            }

            if (errorcheck_count == 0)
            {
                CA_connect = true;
            }
            else
            {
                CA_connect = false;
            }

            return CA_connect;
        }
        private bool CA310_connect()
        {
            string[] ca_probe = new string[2];
            ca_probe = CA310_connect_check();

            if (ca_count != 0)
            {
                objCa200_310 = new CA200SRVRLib.Ca200[ca_count];
                objCa_310 = new CA200SRVRLib.Ca[ca_count];
                objCas_310 = new CA200SRVRLib.Cas[ca_count];
                objProbes_310 = new CA200SRVRLib.Probes[ca_count];
                objMemory_310 = new CA200SRVRLib.Memory[ca_count];

                try
                {
                    for (int ca = 0; ca < ca_count; ca++)
                    {
                        objCa200_310[ca] = new CA200SRVRLib.Ca200();
                        objCa200_310[ca].SetConfiguration(1, ca_probe[ca], ca);

                        objCas_310[ca] = objCa200_310[ca].Cas;
                        objCa_310[ca] = objCas_310[ca].get_ItemOfNumber(1);
                        objMemory_310[ca] = objCa_310[ca].Memory;
                        objProbes_310[ca] = objCa_310[ca].Probes;

                        objCa_310[ca].OutputProbes.AddAll();
                    }
                    CA_connect = true;

                    button_Romote_on.Show();
                    button_Romote_off.Show();

                    CA310_probe_info_update();

                    checkBox_CA310_1.Enabled = false;
                    checkBox_CA310_2.Enabled = false;
                    textBox_CA310_no_of_probe_1.Enabled = false;
                    textBox_CA310_no_of_probe_2.Enabled = false;
                }
                catch
                {
                    CA_connect = false;

                    checkBox_CA310_1.Enabled = true;
                    checkBox_CA310_2.Enabled = true;
                    textBox_CA310_no_of_probe_1.Enabled = true;
                    textBox_CA310_no_of_probe_2.Enabled = true;
                }
            }
            else
            {
                CA_connect = false;

                checkBox_CA310_1.Enabled = true;
                checkBox_CA310_2.Enabled = true;
                textBox_CA310_no_of_probe_1.Enabled = true;
                textBox_CA310_no_of_probe_2.Enabled = true;

                System.Windows.Forms.MessageBox.Show("Please check the number of CA-310");
            }

            return CA_connect;
        }
        private void CA310_probe_info_update()
        {
            if (CA_connect)
            {
                if (ca_count > 1)
                {
                    dataGridView_CA1_5.Rows[0].DefaultCellStyle.Font = new Font("굴림", 7);
                    for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_1.Text); probe++)
                    {
                        dataGridView_CA1_5.Rows[0].Cells[probe].Value = objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Substring(2, objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Length - 2);
                    }
                    dataGridView_CA6_10.Rows[0].DefaultCellStyle.Font = new Font("굴림", 7);
                    for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_2.Text); probe++)
                    {
                        dataGridView_CA6_10.Rows[0].Cells[probe].Value = objCa_310[1].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Substring(2, objCa_310[1].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Length - 2);
                    }
                }
                else
                {
                    dataGridView_CA1_5.Rows[0].DefaultCellStyle.Font = new Font("굴림", 7);
                    for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_1.Text); probe++)
                    {
                        dataGridView_CA1_5.Rows[0].Cells[probe].Value = objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Substring(2, objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).SerialNO.Length - 2);
                    }
                }
            }
        }
        private void Discconect()
        {
            if (radioButton_CA410.Checked)
            {
                GetErrorMessage(objCa200.DisconnectAll());
                Get_All_Serial_Port();
            }
            else
            {
                if (ca_count != 0)
                {
                    for (int ca = 0; ca < ca_count; ca++)
                    {
                        objCa_310[ca].RemoteMode = 0;
                    }
                }
            }
            Button_Click_Enable(false);
            radioButton_CA310.Enabled = true;
            radioButton_CA410.Enabled = true;
        }
        private void button_Romote_on_Click(object sender, EventArgs e)
        {
            for (int ca = 0; ca < ca_count; ca++)
            {
                objCa_310[ca].RemoteMode = 1;
            }
            Button_Click_Enable(true);
        }
        private void button_Romote_off_Click(object sender, EventArgs e)
        {
            for (int ca = 0; ca < ca_count; ca++)
            {
                objCa_310[ca].RemoteMode = 0;
            }
            Button_Click_Enable(false);
            button_Romote_on.Show();
        }
        private void CA_zero_cal_button_Click(object sender, EventArgs e)
        {
            for (int ca = 0; ca < ca_count; ca++)
            {
                GetErrorMessage(objCa[ca].CalZero());
            }
        }
        private XYLv[] MultiMeasurement_CA410()
        {
            XYLv[] measurement = new XYLv[ca_count];
            XYLv[][] measurement_avg = new XYLv[ca_count][];
            XYLv[] sum_measurement = new XYLv[ca_count];
            int count_avg_measure=0;
            double[] Min_Lv_data = new double[ca_count];
            double[] Max_Lv_data = new double[ca_count];
            int[] Min_index = new int[ca_count];
            int[] Max_index = new int[ca_count];
            int[] count = new int[ca_count];
            
            double avg_measure_Lv_Limit = Convert.ToDouble(textBox_Avg_Lv_Limit.Text);

            for (int ca = 0; ca < ca_count; ca++)
            {
                GetErrorMessage(objCa[ca].put_DisplayMode(0));     //Set Lvxy mode
            }
            GetErrorMessage(objCas.SendMsr());         //Measure
            GetErrorMessage(objCas.ReceiveMsr());      //Get results
            
            for (int ca = 0; ca < ca_count; ca++)
            {
                // Get measurement data
                GetErrorMessage(objProbe[ca].get_Lv(ref measurement[ca].double_Lv));
                GetErrorMessage(objProbe[ca].get_sx(ref measurement[ca].double_X));
                GetErrorMessage(objProbe[ca].get_sy(ref measurement[ca].double_Y));
                measurement_avg[ca] = new XYLv[5];
                measurement_avg[ca][0] = measurement[ca];

                sum_measurement[ca].double_X = 0;
                sum_measurement[ca].double_Y = 0;
                sum_measurement[ca].double_Lv = 0;
                Min_Lv_data[ca] = measurement[ca].double_Lv;
                Max_Lv_data[ca] = measurement[ca].double_Lv;
                count[ca] = 0;

                if (check_Avg_Measure.Checked && (measurement[ca].double_Lv < avg_measure_Lv_Limit))
                {
                    count_avg_measure++;
                }
            }

            if (check_Avg_Measure.Checked && (count_avg_measure > 1))
            {
                for (int num = 0; num < 4; num++)
                {
                    GetErrorMessage(objCas.SendMsr());         //Measure
                    GetErrorMessage(objCas.ReceiveMsr());      //Get results
                    for (int ca = 0; ca < ca_count; ca++)
                    {
                        // Get measurement data
                        GetErrorMessage(objProbe[ca].get_Lv(ref measurement_avg[ca][num+1].double_Lv));
                        GetErrorMessage(objProbe[ca].get_sx(ref measurement_avg[ca][num + 1].double_X));
                        GetErrorMessage(objProbe[ca].get_sy(ref measurement_avg[ca][num + 1].double_Y));

                        if (measurement_avg[ca][num + 1].double_Lv < Min_Lv_data[ca])
                        {
                            Min_Lv_data[ca] = measurement_avg[ca][num + 1].double_Lv;
                            Min_index[ca] = num + 1;
                        }
                        if (measurement_avg[ca][num + 1].double_Lv > Max_Lv_data[ca])
                        {
                            Max_Lv_data[ca] = measurement_avg[ca][num + 1].double_Lv;
                            Max_index[ca] = num + 1;
                        }
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    for (int ca = 0; ca < ca_count; ca++)
                    {
                        if (i == Min_index[ca] || i == Max_index[ca])
                        {

                        }
                        else
                        {
                            sum_measurement[ca].double_X += measurement_avg[ca][i].double_X;
                            sum_measurement[ca].double_Y += measurement_avg[ca][i].double_Y;
                            sum_measurement[ca].double_Lv += measurement_avg[ca][i].double_Lv;
                            count[ca]++;
                        }
                    }
                }

                for (int ca = 0; ca < ca_count; ca++)
                {
                    measurement[ca].double_X = sum_measurement[ca].double_X / Convert.ToDouble(count[ca]);
                    measurement[ca].double_Y = sum_measurement[ca].double_Y / Convert.ToDouble(count[ca]);
                    measurement[ca].double_Lv = sum_measurement[ca].double_Lv / Convert.ToDouble(count[ca]);
                }
            }

            return measurement;
        }
        private XYLv[] Get_CA310_Measurement_data()
        {
            XYLv[] measurement = new XYLv[probe_count];
            if (ca_count > 1)
            {
                for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_1.Text); probe++)
                {
                    measurement[probe].double_X = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).sx);
                    measurement[probe].double_Y = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).sy);
                    measurement[probe].double_Lv = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).Lv);
                }

                for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_2.Text); probe++)
                {
                    measurement[probe + 5].double_X = Convert.ToDouble(objCa_310[1].OutputProbes.get_ItemOfNumber(probe + 1).sx);
                    measurement[probe + 5].double_Y = Convert.ToDouble(objCa_310[1].OutputProbes.get_ItemOfNumber(probe + 1).sy);
                    measurement[probe + 5].double_Lv = Convert.ToDouble(objCa_310[1].OutputProbes.get_ItemOfNumber(probe + 1).Lv);
                }
            }
            else
            {
                for (int probe = 0; probe < Convert.ToInt32(textBox_CA310_no_of_probe_1.Text); probe++)
                {
                    measurement[probe].double_X = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).sx);
                    measurement[probe].double_Y = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).sy);
                    measurement[probe].double_Lv = Convert.ToDouble(objCa_310[0].OutputProbes.get_ItemOfNumber(probe + 1).Lv);
                }
            }
            return measurement;
        }
        private XYLv[] MultiMeasurement_CA310()
        {
            XYLv[] measurement = new XYLv[probe_count];
            XYLv[][] measurement_avg = new XYLv[probe_count][];
            XYLv[] sum_measurement = new XYLv[probe_count];
            int count_avg_measure = 0;
            double[] Min_Lv_data = new double[probe_count];
            double[] Max_Lv_data = new double[probe_count];
            int[] Min_index = new int[probe_count];
            int[] Max_index = new int[probe_count];
            int[] count = new int[probe_count];

            double avg_measure_Lv_Limit = Convert.ToDouble(textBox_Avg_Lv_Limit.Text);

            for (int ca = 0; ca < ca_count; ca++)
            {
                objCa_310[ca].DisplayMode = 0;
                objCa_310[ca].Measure();
            }

            measurement = Get_CA310_Measurement_data();

            for (int probe = 0; probe < probe_count; probe++)
            {
                measurement_avg[probe] = new XYLv[5];
                measurement_avg[probe][0] = measurement[probe];

                sum_measurement[probe].double_X = 0;
                sum_measurement[probe].double_Y = 0;
                sum_measurement[probe].double_Lv = 0;
                Min_Lv_data[probe] = measurement[probe].double_Lv;
                Max_Lv_data[probe] = measurement[probe].double_Lv;
                count[probe] = 0;

                if (check_Avg_Measure.Checked && (measurement[probe].double_Lv < avg_measure_Lv_Limit))
                {
                    count_avg_measure++;
                }
            }

            if (check_Avg_Measure.Checked && (count_avg_measure > 1))
            {
                for (int num = 0; num < 4; num++)
                {
                    for (int probe = 0; probe < probe_count; probe++)
                    {
                        for (int ca = 0; ca < ca_count; ca++)
                        {
                            objCa_310[ca].DisplayMode = 0;
                            objCa_310[ca].Measure();
                        }

                        measurement = Get_CA310_Measurement_data();

                        if (measurement_avg[probe][num + 1].double_Lv < Min_Lv_data[probe])
                        {
                            Min_Lv_data[probe] = measurement_avg[probe][num + 1].double_Lv;
                            Min_index[probe] = num + 1;
                        }
                        if (measurement_avg[probe][num + 1].double_Lv > Max_Lv_data[probe])
                        {
                            Max_Lv_data[probe] = measurement_avg[probe][num + 1].double_Lv;
                            Max_index[probe] = num + 1;
                        }
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    for (int probe = 0; probe < probe_count; probe++)
                    {
                        if (i == Min_index[probe] || i == Max_index[probe])
                        {

                        }
                        else
                        {
                            sum_measurement[probe].double_X += measurement_avg[probe][i].double_X;
                            sum_measurement[probe].double_Y += measurement_avg[probe][i].double_Y;
                            sum_measurement[probe].double_Lv += measurement_avg[probe][i].double_Lv;
                            count[probe]++;
                        }
                    }
                }

                for (int probe = 0; probe < probe_count; probe++)
                {
                    measurement[probe].double_X = sum_measurement[probe].double_X / Convert.ToDouble(count[probe]);
                    measurement[probe].double_Y = sum_measurement[probe].double_Y / Convert.ToDouble(count[probe]);
                    measurement[probe].double_Lv = sum_measurement[probe].double_Lv / Convert.ToDouble(count[probe]);
                }
            }

            return measurement;
        }
        private void CA_Test_button_Click(object sender, EventArgs e)
        {
            int count = 0;
            if (radioButton_CA410.Checked) count = ca_count;
            else count = probe_count;
            XYLv[] measurement = new XYLv[count];

            if (radioButton_CA410.Checked)  measurement = MultiMeasurement_CA410();
            else measurement = MultiMeasurement_CA310();

            PNC_CH_check();

            for (int ca = 0; ca < count; ca++)
            {
                switch (CA_NO_PNC_CH[ca])
                {
                    case 0: dataGridView_CH1.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 1: dataGridView_CH2.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 2: dataGridView_CH3.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 3: dataGridView_CH4.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 4: dataGridView_CH5.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 5: dataGridView_CH6.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 6: dataGridView_CH7.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 7: dataGridView_CH8.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 8: dataGridView_CH9.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                    case 9: dataGridView_CH10.Rows.Add(meausre_count.ToString(), measurement[ca].double_X.ToString("0.0000"), measurement[ca].double_Y.ToString("0.0000"), measurement[ca].double_Lv.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""); break;
                }
            }
            meausre_count++;

            if (!radioButton_CA410.Checked)
            {
                button_Romote_on.Show();
                button_Romote_off.Show();
            }
        }

        private void MultiSetting()
        {
            int freqmode = 0;   // SyncMode : NTSC 0 / PAL 1 / EXT 2 / INT 4
            double freq = 60.0; //frequency = 60.0Hz
            int speed = 1;      //Measurement speed : FAST
            int Lvmode = 1;     //Lv : cd/m2

            if (radioButton_CA410.Checked)
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    GetErrorMessage(objCa[ca].CalZero());                      //Zero-Calibration
                    GetErrorMessage(objCa[ca].put_DisplayProbe("P1"));         //Set display probe to P1
                    GetErrorMessage(objCa[ca].put_SyncMode(freqmode, freq));   //Set sync mode and frequency
                    GetErrorMessage(objCa[ca].put_AveragingMode(speed));       //Set measurement speed
                    GetErrorMessage(objCa[ca].put_BrightnessUnit(Lvmode));     //Set Brightness unit
                    GetErrorMessage(objMemory[ca].put_ChannelNO(Convert.ToInt32(textBox_ch_W.Text)));
                }
            }
            else
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    objCa_310[ca].AveragingMode = speed;
                    objCa_310[ca].BrightnessUnit = Lvmode;
                    objCa_310[ca].SyncMode = freqmode;
                    objMemory_310[ca].ChannelNO = Convert.ToInt32(textBox_ch_W.Text);
                }
            }
        }
        private void MultiModeChange()
        {
            int freqmode = 0;   // SyncMode : NTSC 0 / PAL 1 / EXT 2 / INT 4
            double freq = 60.0; //frequency = 60.0Hz
            int speed = 1;      //Measurement speed : FAST

            if (this.radioButton_CA_Measure_Auto.Checked) speed = 2; // Auto
            else if (this.radioButton_CA_Measure_Fast.Checked) speed = 1; // Fast
            else if (this.radioButton_CA_Measure_Slow.Checked) speed = 0; // Slow

            if (radioButton_NTSC.Checked) freqmode = 0;
            else if (radioButton_PAL.Checked) freqmode = 1;
            else if (radioButton_EXT.Checked)
            {
                try
                {
                    freqmode = 2;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    System.Windows.Forms.MessageBox.Show("Please Input an external synchronization signal");
                    radioButton_NTSC.Checked = true;
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("EXT Mode Error");
                    radioButton_NTSC.Checked = true;
                }
            }

            if (radioButton_CA410.Checked)
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    GetErrorMessage(objCa[ca].put_SyncMode(freqmode, freq));   //Set sync mode and frequency
                    GetErrorMessage(objCa[ca].put_AveragingMode(speed));       //Set measurement speed
                }
            }
            else
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    objCa_310[ca].SyncMode = freqmode;
                    objCa_310[ca].AveragingMode = speed;
                }
            }
            
        }
        private void checkBox_Color_CH_fix_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Color_CH_fix.Checked)
            {
                textBox_ch_R.Text = textBox_ch_W.Text;
                textBox_ch_G.Text = textBox_ch_W.Text;
                textBox_ch_B.Text = textBox_ch_W.Text;
            }
            else
            {
                textBox_ch_R.Text = (Convert.ToInt32(textBox_ch_W.Text) + 1).ToString();
                textBox_ch_G.Text = (Convert.ToInt32(textBox_ch_W.Text) + 2).ToString();
                textBox_ch_B.Text = (Convert.ToInt32(textBox_ch_W.Text) + 3).ToString();
            }
        }
        private void CA_CH_Change(int color)
        {
            int ca_ch = 0;
            switch (color)
            {
                case 0: ca_ch = Convert.ToInt32(textBox_ch_W.Text); break;
                case 1: ca_ch = Convert.ToInt32(textBox_ch_R.Text); break;
                case 2: ca_ch = Convert.ToInt32(textBox_ch_G.Text); break;
                case 3: ca_ch = Convert.ToInt32(textBox_ch_B.Text); break;
            }
            
            if (radioButton_CA410.Checked)
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    GetErrorMessage(objMemory[ca].put_ChannelNO(ca_ch));
                }
            }
            else
            {
                for (int ca = 0; ca < ca_count; ca++)
                {
                    objMemory_310[ca].ChannelNO = ca_ch;
                }
            }
        }
        private void button_channel_change_Click_1(object sender, EventArgs e)
        {
            CA_CH_Change(0);
        }
        private void radioButton_CA_Measure_Auto_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }
        private void radioButton_CA_Measure_Fast_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }
        private void radioButton_CA_Measure_Slow_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }
        private void radioButton_NTSC_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }
        private void radioButton_PAL_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }
        private void radioButton_EXT_CheckedChanged(object sender, EventArgs e)
        {
            MultiModeChange();
        }

        private void PNC_CH_check()
        {
            PNC_CH_count = 0;

            //Textbox to String
            PNC_CH_CA_NO[0] = Convert.ToInt32(CA_NO_CH1.Text);
            PNC_CH_CA_NO[1] = Convert.ToInt32(CA_NO_CH2.Text);
            PNC_CH_CA_NO[2] = Convert.ToInt32(CA_NO_CH3.Text);
            PNC_CH_CA_NO[3] = Convert.ToInt32(CA_NO_CH4.Text);
            PNC_CH_CA_NO[4] = Convert.ToInt32(CA_NO_CH5.Text);
            PNC_CH_CA_NO[5] = Convert.ToInt32(CA_NO_CH6.Text);
            PNC_CH_CA_NO[6] = Convert.ToInt32(CA_NO_CH7.Text);
            PNC_CH_CA_NO[7] = Convert.ToInt32(CA_NO_CH8.Text);
            PNC_CH_CA_NO[8] = Convert.ToInt32(CA_NO_CH9.Text);
            PNC_CH_CA_NO[9] = Convert.ToInt32(CA_NO_CH10.Text);

            //CheckBox to Bool
            check_PNC_CH[0] = check_PNC_CH1.Checked;
            check_PNC_CH[1] = check_PNC_CH2.Checked;
            check_PNC_CH[2] = check_PNC_CH3.Checked;
            check_PNC_CH[3] = check_PNC_CH4.Checked;
            check_PNC_CH[4] = check_PNC_CH5.Checked;
            check_PNC_CH[5] = check_PNC_CH6.Checked;
            check_PNC_CH[6] = check_PNC_CH7.Checked;
            check_PNC_CH[7] = check_PNC_CH8.Checked;
            check_PNC_CH[8] = check_PNC_CH9.Checked;
            check_PNC_CH[9] = check_PNC_CH10.Checked;

            for(int ch=0;ch<10;ch++)
            {
                for (int ca = 0; ca < 10; ca++)
                {
                    if ((PNC_CH_CA_NO[ch]-1) == ca)
                    {
                        CA_NO_PNC_CH[ca] = ch;
                    }
                }
                if (check_PNC_CH[ch]) PNC_CH_count++;
            }

            if(PNC_CH_count>1)  PNC_CH_Setting_CMD();
            PNC_CH_Label_update();
        }
        private void PNC_CH_Label_update()
        {
            for (int ch = 0; ch < 10; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    switch (ch)
                    {
                        case 0:
                            label_PNC_CH1.Text = "PNC CH1" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH1.ForeColor = Color.YellowGreen;
                            break;
                        case 1:
                            label_PNC_CH2.Text = "PNC CH2" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH2.ForeColor = Color.YellowGreen;
                            break;
                        case 2:
                            label_PNC_CH3.Text = "PNC CH3" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH3.ForeColor = Color.YellowGreen;
                            break;
                        case 3:
                            label_PNC_CH4.Text = "PNC CH4" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH4.ForeColor = Color.YellowGreen;
                            break;
                        case 4:
                            label_PNC_CH5.Text = "PNC CH5" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH5.ForeColor = Color.YellowGreen;
                            break;
                        case 5:
                            label_PNC_CH6.Text = "PNC CH6" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH6.ForeColor = Color.YellowGreen;
                            break;
                        case 6:
                            label_PNC_CH7.Text = "PNC CH7" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH7.ForeColor = Color.YellowGreen;
                            break;
                        case 7:
                            label_PNC_CH8.Text = "PNC CH8" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH8.ForeColor = Color.YellowGreen;
                            break;
                        case 8:
                            label_PNC_CH9.Text = "PNC CH9" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH9.ForeColor = Color.YellowGreen;
                            break;
                        case 9:
                            label_PNC_CH10.Text = "PNC CH10" + " - CA #" + PNC_CH_CA_NO[ch];
                            label_PNC_CH10.ForeColor = Color.LimeGreen;
                            break;
                    }
                }
                else
                {
                    switch (ch)
                    {
                        case 0:
                            label_PNC_CH1.ForeColor = Color.DarkGray;
                            break;
                        case 1:
                            label_PNC_CH2.ForeColor = Color.DarkGray;
                            break;
                        case 2:
                            label_PNC_CH3.ForeColor = Color.DarkGray;
                            break;
                        case 3:
                            label_PNC_CH4.ForeColor = Color.DarkGray;
                            break;
                        case 4:
                            label_PNC_CH5.ForeColor = Color.DarkGray;
                            break;
                        case 5:
                            label_PNC_CH6.ForeColor = Color.DarkGray;
                            break;
                        case 6:
                            label_PNC_CH7.ForeColor = Color.DarkGray;
                            break;
                        case 7:
                            label_PNC_CH8.ForeColor = Color.DarkGray;
                            break;
                        case 8:
                            label_PNC_CH9.ForeColor = Color.DarkGray;
                            break;
                        case 9:
                            label_PNC_CH10.ForeColor = Color.DarkGray;
                            break;
                    }
                }
            }
        }
        private void PNC_CH_Setting_CMD()
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];

            string Binary_10ch = "";
            string Hex_10ch = "";

            for (int ch = 0; ch < 10; ch++)
            {

                Binary_10ch = Binary_10ch + Convert.ToInt32(check_PNC_CH[9-ch]).ToString();
            }

            Hex_10ch = Convert.ToInt32(Binary_10ch, 2).ToString("X");

            f1.IPC_Quick_Send("mipi.nvd.wchannel 0x0" + Hex_10ch);
        }
        private void PNC_SET_check()
        {
            int[] seq_temp = new int[6];
            
            //Textbox to String
            seq_temp[0] = Convert.ToInt32(SEQ_SET1.Text);
            seq_temp[1] = Convert.ToInt32(SEQ_SET2.Text);
            seq_temp[2] = Convert.ToInt32(SEQ_SET3.Text);
            seq_temp[3] = Convert.ToInt32(SEQ_SET4.Text);
            seq_temp[4] = Convert.ToInt32(SEQ_SET5.Text);
            seq_temp[5] = Convert.ToInt32(SEQ_SET6.Text);

            for (int seq = 0; seq < 6; seq++)
            {
                switch (seq_temp[seq]-1)
                {
                    case 0: SET_sequence[seq_temp[seq] - 1] = seq; break;
                    case 1: SET_sequence[seq_temp[seq] - 1] = seq; break;
                    case 2: SET_sequence[seq_temp[seq] - 1] = seq; break;
                    case 3: SET_sequence[seq_temp[seq] - 1] = seq; break;
                    case 4: SET_sequence[seq_temp[seq] - 1] = seq; break;
                    case 5: SET_sequence[seq_temp[seq] - 1] = seq; break;
                }
            }

            //CheckBox to Bool
            check_SET[0] = check_SET1.Checked;
            check_SET[1] = check_SET2.Checked;
            check_SET[2] = check_SET3.Checked;
            check_SET[3] = check_SET4.Checked;
            check_SET[4] = check_SET5.Checked;
            check_SET[5] = check_SET6.Checked;


        }
        private void PNC_ALL_CH_Status(bool Checked)
        {
            check_PNC_CH1.Checked = Checked;
            check_PNC_CH2.Checked = Checked;
            check_PNC_CH3.Checked = Checked;
            check_PNC_CH4.Checked = Checked;
            check_PNC_CH5.Checked = Checked;
            check_PNC_CH6.Checked = Checked;
            check_PNC_CH7.Checked = Checked;
            check_PNC_CH8.Checked = Checked;
            check_PNC_CH9.Checked = Checked;
            check_PNC_CH10.Checked = Checked;
        }
        private void button_PNC_CH_all_select_Click(object sender, EventArgs e)
        {
            PNC_ALL_CH_Status(true);
        }
        private void button_PNC_CH_all_deselect_Click(object sender, EventArgs e)
        {
            PNC_ALL_CH_Status(false);
        }
        
        private void Set_change_Measure()
        {
            int SET_delay = Convert.ToInt16(Set_Change_Delay.Text);
            int count=0;
            if (radioButton_CA410.Checked) count=ca_count;
            else count = probe_count;
            XYLv[] measurement = new XYLv[count];

            // Set 변환
            for (int seq = 0; seq < 6; seq++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_SET[SET_sequence[seq]])
                {
                    PNC_SET_Script_Send(SET_sequence[seq]);

                    Thread.Sleep(SET_delay);

                    if (radioButton_CA410.Checked) measurement = MultiMeasurement_CA410();
                    else measurement = MultiMeasurement_CA310();

                    // Data update

                    for (int ch = 0; ch < 10; ch++)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (check_PNC_CH[ch])
                        {
                            PNC_CH_Data_Update(ch, SET_sequence[seq], Convert.ToInt32(PNC_CH_CA_NO[ch]) - 1, measurement);
                        }
                    }
                }
            }
        }
        private void Set_non_change_Measure()
        {
            int SET_delay = Convert.ToInt16(Set_Change_Delay.Text);
            int count = 0;
            if (radioButton_CA410.Checked) count = ca_count;
            else count = probe_count;
            XYLv[] measurement = new XYLv[count];

            if (radioButton_CA410.Checked) measurement = MultiMeasurement_CA410();
            else measurement = MultiMeasurement_CA310();

            // Data update
            for (int ch = 0; ch < 10; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    PNC_CH_Data_Update(ch, 0, Convert.ToInt32(PNC_CH_CA_NO[ch]) - 1, measurement);
                }
            }
        }

        private void PNC_SET_Script_Send(int set)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            TextBox TextBox_script_data = null;

            PNC_Script_Transform(set);

            switch (set)
            {
                case 0:
                    TextBox_script_data = Textbox_Script_Set1_Final;
                    break;
                case 1:
                    TextBox_script_data = Textbox_Script_Set2_Final;
                    break;
                case 2:
                    TextBox_script_data = Textbox_Script_Set3_Final;
                    break;
                case 3:
                    TextBox_script_data = Textbox_Script_Set4_Final;
                    break;
                case 4:
                    TextBox_script_data = Textbox_Script_Set5_Final;
                    break;
                case 5:
                    TextBox_script_data = Textbox_Script_Set6_Final;
                    break;
            }

            for (int i = 0; i < TextBox_script_data.Lines.Length - 1; i++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (TextBox_script_data.Lines[i].Length >= 10
                    && TextBox_script_data.Lines[i].Substring(0, 10) == "mipi.write")
                {
                    f1.IPC_Quick_Send(TextBox_script_data.Lines[i]);
                }
                else if (TextBox_script_data.Lines[i].Length >= 5 && (
                    TextBox_script_data.Lines[i].Substring(0, 5) == "delay"
                    || TextBox_script_data.Lines[i].Substring(0, 5) == "image"))
                {
                    f1.IPC_Quick_Send(TextBox_script_data.Lines[i]);
                }
                else if (TextBox_script_data.Lines[i].Substring(0, 14) == "gpio.i2c.write")
                {
                    f1.IPC_Quick_Send(TextBox_script_data.Lines[i]);
                }
            }
        }
        private void PNC_Script_Transform(int set)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];

            string temp_Mipi_Data_String = string.Empty;
            int count_mipi_cmd = 0;
            int count_one_mipi_cmd_length = 0;
            bool Flag = false;
            TextBox Textbox_transform_before = textBox_script_copy_temp;
            TextBox Textbox_transform_after = textBox_script_copy_temp;

            Textbox_transform_before.Clear();
            Textbox_transform_after.Clear();

            switch (set)
            {
                case 0:
                    Textbox_transform_before = textBox_Script_SET1;
                    break;
                case 1:
                    Textbox_transform_before = textBox_Script_SET2;
                    break;
                case 2:
                    Textbox_transform_before = textBox_Script_SET3;
                    break;
                case 3:
                    Textbox_transform_before = textBox_Script_SET4;
                    break;
                case 4:
                    Textbox_transform_before = textBox_Script_SET5;
                    break;
                case 5:
                    Textbox_transform_before = textBox_Script_SET6;
                    break;
            }

            //Delete others except for Mipi CMDs and Write on the 2nd Textbox
            for (int i = 0; i < Textbox_transform_before.Lines.Length; i++)
            {
                if (Textbox_transform_before.Lines[i].Length >= 20) // mipi.write 0xXX 0xXX <-- 20ea Character
                {
                    if (Textbox_transform_before.Lines[i].Substring(0, 10) == "mipi.write")
                    {
                        count_mipi_cmd++;

                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 10; k < Textbox_transform_before.Lines[i].Length; k++)
                        {
                            if (Textbox_transform_before.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && Textbox_transform_before.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        Textbox_transform_after.Text += temp_Mipi_Data_String + "\r\n";
                    }
                    else if (Textbox_transform_before.Lines[i].Substring(0, 14) == "gpio.i2c.write")
                    {
                        count_mipi_cmd++;

                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 14; k < Textbox_transform_before.Lines[i].Length; k++)
                        {
                            if (Textbox_transform_before.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && Textbox_transform_before.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        Textbox_transform_after.Text += temp_Mipi_Data_String + "\r\n";
                    }

                    else
                    {
                        // It's not a "mipi.write" of "delay" command , do nothing 
                    }
                }

                //Delay
                else if (Textbox_transform_before.Lines[i].Length >= 5
                    && Textbox_transform_before.Lines[i].Substring(0, 5) != "     ")
                {
                    if (Textbox_transform_before.Lines[i].Substring(0, 5) == "delay")
                    {
                        count_mipi_cmd++;
                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 5; k < Textbox_transform_before.Lines[i].Length; k++)
                        {
                            if (Textbox_transform_before.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && Textbox_transform_before.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        Textbox_transform_after.Text += temp_Mipi_Data_String + "\r\n";
                    }

                    else if (Textbox_transform_before.Lines[i].Substring(0, 5) == "image")
                    {
                        count_mipi_cmd++;
                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 5; k < Textbox_transform_before.Lines[i].Length; k++)
                        {
                            if (Textbox_transform_before.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && Textbox_transform_before.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = Textbox_transform_before.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        Textbox_transform_after.Text += temp_Mipi_Data_String + "\r\n";
                    }
                }
            }

            switch (set)
            {
                case 0:
                    Textbox_Script_Set1_Final = Textbox_transform_after;
                    break;
                case 1:
                    Textbox_Script_Set2_Final = Textbox_transform_after;
                    break;
                case 2:
                    Textbox_Script_Set3_Final = Textbox_transform_after;
                    break;
                case 3:
                    Textbox_Script_Set4_Final = Textbox_transform_after;
                    break;
                case 4:
                    Textbox_Script_Set5_Final = Textbox_transform_after;
                    break;
                case 5:
                    Textbox_Script_Set6_Final = Textbox_transform_after;
                    break;
            }
        }
        private void PNC_CH_Data_Update(int PNC_ch, int set, int CA_ch, XYLv[] measurement)
        {
            CH_Grid_View[PNC_ch].Rows[row_count - 1].Cells[set * 3 + 1].Value = measurement[CA_ch].double_X.ToString("0.0000");
            CH_Grid_View[PNC_ch].Rows[row_count - 1].Cells[set * 3 + 2].Value = measurement[CA_ch].double_Y.ToString("0.0000");
            CH_Grid_View[PNC_ch].Rows[row_count - 1].Cells[set * 3 + 3].Value = measurement[CA_ch].double_Lv.ToString();
        }

        private void save_GCS_data(int number, string DBV, StreamWriter sw)
        {
            double gamma_data = 0;
            string log_data;

            DeltaE calc_data;
            DeltaE max_data;

            label_mornitoring.Text = "Calculate 'Gamma & DeltaE3' and save the data of GCS" + (number + 1).ToString() + "";
            label_mornitoring.Text = "Calculate 'Gamma & DeltaE3' and save the data of GCS" + (number + 1).ToString() + "";

            if (number != 0) sw.WriteLine("-");

            for (int index = 0; index < count_measure; index++)
            {
                log_data = "";
                log_data = log_data + index.ToString() + "\t" + DBV + "\t" + index_list[index] + "\t";
                for (int ch = 0; ch < 10; ch++)
                {
                    for (int set = 0; set < 6; set++)
                    {
                        if (check_PNC_CH[ch] && check_SET[set])
                        {
                            log_data = log_data + data_list[set][ch][index].double_X.ToString() + "\t"; // x
                            log_data = log_data + data_list[set][ch][index].double_Y.ToString() + "\t"; // y
                            log_data = log_data + data_list[set][ch][index].double_Lv.ToString() + "\t";    // Lv

                            max_data = calculate_DeltaE_max(set, ch);
                            calc_data = calculate_DeltaE(set, ch, index, max_data);

                            log_data = log_data + calc_data.delta_C.ToString() + "\t";  // △E3

                            gamma_data = Math.Log(data_list[set][ch][index].double_Lv / data_list[set][ch][index_max].double_Lv) / Math.Log(Convert.ToDouble(index_list[index]) / Convert.ToDouble(255));

                            log_data = log_data + gamma_data.ToString() + "\t";  // Gamma

                            //if ((index_list[index] > 240) || (index_list[index] < 17))
                            //{
                            //    log_data = log_data + "-" + "\t";  // Gamma
                            //}
                            //else
                            //{
                            //    log_data = log_data + gamma_data.ToString() + "\t";  // Gamma
                            //}
                        }
                        else
                        {
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                        }
                            
                    }
                }
                sw.WriteLine(log_data);
            }
        }
        private void save_BCS_data(int ragne, int number, int gray, StreamWriter sw)
        {
            double accuracy = 0;
            string log_data;

            DeltaE calc_data;
            DeltaE max_data;

            label_mornitoring.Text = "Calculate 'delta E & accuracy' and save the data of BCS" + (number + 1).ToString() + "";
            label_mornitoring.Text = "Calculate 'delta E & accuracy' and save the data of BCS" + (number + 1).ToString() + "";

            if(number!=0)  sw.WriteLine("-");

            for (int index = 0; index < count_measure; index++)
            {
                log_data = "";
                log_data = log_data + "range" + ragne.ToString() + "\t" +  index.ToString() + "\t" + gray.ToString() + "\t" + index_list[index] + "\t";
                for (int ch = 0; ch < 10; ch++)
                {
                    for (int set = 0; set < 6; set++)
                    {
                        if (check_PNC_CH[ch] && check_SET[set])
                        {
                            log_data = log_data + data_list[set][ch][index].double_X.ToString() + "\t"; // x
                            log_data = log_data + data_list[set][ch][index].double_Y.ToString() + "\t"; // y
                            log_data = log_data + data_list[set][ch][index].double_Lv.ToString() + "\t";    // Lv

                            max_data = calculate_DeltaE_max(set, ch);
                            calc_data = calculate_DeltaE(set, ch, index, max_data);

                            log_data = log_data + calc_data.delta_C.ToString() + "\t";  // △E2

                            if (index == index_max)
                            {
                                log_data = log_data + "-" + "\t";  // Accuracy
                            }
                            else
                            {
                                if (index_list[index] < index_list[index - 1]) accuracy = Math.Abs((data_list[set][ch][index].double_Lv - data_list[set][ch][index - 1].double_Lv) / data_list[set][ch][index].double_Lv) * 100;
                                else accuracy = Math.Abs((data_list[set][ch][index].double_Lv - data_list[set][ch][index - 1].double_Lv) / data_list[set][ch][index - 1].double_Lv) * 100;
                                log_data = log_data + accuracy.ToString() + "\t";  // Accuracy
                            }
                        }
                        else
                        {
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                            log_data += "\t";
                        }

                    }
                }
                sw.WriteLine(log_data);
            }
        }
        private void save_AOD_GCS_data(int number, string DBV, StreamWriter sw)
        {
            double gamma_data = 0;
            string log_data;

            DeltaE calc_data;
            DeltaE max_data;

            label_mornitoring.Text = "Calculate 'Gamma & DeltaE3' and save the data of AOD_GCS" + (number + 1).ToString() + "";
            label_mornitoring.Text = "Calculate 'Gamma & DeltaE3' and save the data of AOD_GCS" + (number + 1).ToString() + "";

            if (number != 0) sw.WriteLine("-");

            for (int index = 0; index < count_measure; index++)
            {
                log_data = "";
                log_data = log_data + index.ToString() + "\t" + DBV + "\t" + index_list[index] + "\t";
                for (int ch = 0; ch < 10; ch++)
                {
                    if (check_PNC_CH[ch])
                    {
                        log_data = log_data + data_list[0][ch][index].double_X.ToString() + "\t"; // x
                        log_data = log_data + data_list[0][ch][index].double_Y.ToString() + "\t"; // y
                        log_data = log_data + data_list[0][ch][index].double_Lv.ToString() + "\t";    // Lv

                        max_data = calculate_DeltaE_max(0, ch);
                        calc_data = calculate_DeltaE(0, ch, index, max_data);

                        log_data = log_data + calc_data.delta_C.ToString() + "\t";  // △E3

                        gamma_data = Math.Log(data_list[0][ch][index].double_Lv / data_list[0][ch][index_max].double_Lv) / Math.Log(Convert.ToDouble(index_list[index]) / Convert.ToDouble(255));

                        log_data = log_data + gamma_data.ToString() + "\t";  // Gamma

                        //if ((index_list[index] > 240) || (index_list[index] < 17))
                        //{
                        //    log_data = log_data + "-" + "\t";  // Gamma
                        //}
                        //else
                        //{
                        //    log_data = log_data + gamma_data.ToString() + "\t";  // Gamma
                        //}
                    }
                    else
                    {
                        log_data += "\t";
                        log_data += "\t";
                        log_data += "\t";
                        log_data += "\t";
                        log_data += "\t";
                    }
                }
                sw.WriteLine(log_data);
            }
        }
        private void save_Gamma_Crush(int number, string DBV, int gray, StreamWriter sw)
        {
            string log_data;

            RGB gamma_crush;

            label_mornitoring.Text = "Calculate 'Gamma Crush' and save the data of Setting" + (number + 1).ToString() + "";
            label_mornitoring.Text = "Calculate 'Gamma Crush' and save the data of Setting" + (number + 1).ToString() + "";

            for (int ch = 0; ch < 10; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    log_data = "";
                    log_data = log_data + "#" + (ch + 1).ToString() + "\t" + DBV + "\t" + gray.ToString() + "\t";
                    for (int set = 0; set < 6; set++)
                    {
                        if (check_SET[set])
                        {
                            for (int index = 0; index < count_measure; index++)
                            {
                                log_data = log_data + data_list[set][ch][index].double_X.ToString() + "\t"; // x
                                log_data = log_data + data_list[set][ch][index].double_Y.ToString() + "\t"; // y
                                log_data = log_data + data_list[set][ch][index].double_Lv.ToString() + "\t";    // Lv
                            }
                            gamma_crush = calcultae_Gamma_Crush(set, ch);

                            log_data = log_data + gamma_crush.data_R + "\t" + gamma_crush.data_G + "\t" + gamma_crush.data_B + "\t";
                        }
                        else
                        {
                            for (int index = 0; index < count_measure; index++)
                            {
                                log_data += "\t";
                                log_data += "\t";
                                log_data += "\t";
                            }
                            log_data += "\t";
                        }
                    }
                    sw.WriteLine(log_data);
                }
            }
        }
        private void save_IR_Drop_DeltaE(string set, string DBV, StreamWriter sw)
        {
            string log_data;

            DeltaE data_Ref;
            DeltaE data_full;
            DeltaE data_APL;

            double delta_L;
            double delta_a;
            double delta_b;
            double delta_E;
            double[] delta_E_sum = new double[10];
            double[] delta_E_avg = new double[10];

            label_mornitoring.Text = "Calculate 'IR Drop delta E' and save the data";
            label_mornitoring.Text = "Calculate 'IR Drop delta E' and save the data";

            for (int ch = 0; ch < 10; ch++)
            {
                delta_E_sum[ch] = 0;
            }

            for (int i = 0; i < 25; i++)
            {
                log_data = "";
                log_data = log_data + (i + 1).ToString() + "\t" + set.ToString() + "\t" + DBV + "\t" + IR_Drop_PTN[i].R.ToString() + "\t" + IR_Drop_PTN[i].G.ToString() + "\t" + IR_Drop_PTN[i].B.ToString() + "\t";
                for (int ch = 0; ch < 10; ch++)
                {
                    if (check_PNC_CH[ch])
                    {
                        log_data = log_data + data_list[0][ch][2 * i].double_X + "\t";  // full PTN x
                        log_data = log_data + data_list[0][ch][2 * i].double_Y + "\t";  // full PTN y
                        log_data = log_data + data_list[0][ch][2 * i].double_Lv + "\t";  // full PTN Lv

                        log_data = log_data + data_list[0][ch][2 * i + 1].double_X + "\t";  // 30% PTN x
                        log_data = log_data + data_list[0][ch][2 * i + 1].double_Y + "\t";  // 30% PTN y
                        log_data = log_data + data_list[0][ch][2 * i + 1].double_Lv + "\t";  // 30% PTN Lv

                        data_Ref = calculate_DeltaE_max(0, ch);
                        data_full = calculate_DeltaE(0, ch, 2 * i, data_Ref);
                        data_APL = calculate_DeltaE(0, ch, 2 * i + 1, data_Ref);

                        delta_L = data_full.L - data_APL.L;
                        delta_a = data_full.a - data_APL.a;
                        delta_b = data_full.b - data_APL.b;

                        delta_E = Math.Sqrt(Math.Pow(delta_L, 2) + Math.Pow(delta_a, 2) + Math.Pow(delta_b, 2));

                        log_data = log_data + delta_E.ToString() + "\t";  // 30% PTN Lv

                        delta_E_sum[ch] += delta_E;
                        delta_E_avg[ch] = delta_E_sum[ch] / (i + 1);
                    }
                    else
                    {
                        for (int skip = 0; skip < 7; skip++)
                        {
                            log_data += "\t";
                        }
                    }
                }

                sw.WriteLine(log_data);
            }

            log_data = "\t" + "\t" + "\t" + "\t" + "\t" + "\t";

            for (int ch = 0; ch < 10; ch++)
            {
                log_data += "\t";
                log_data += "\t";
                log_data += "\t";
                log_data += "\t";
                log_data += "\t";
                log_data += "\t";
                if (check_PNC_CH[ch]) log_data = log_data + delta_E_avg[ch] + "\t";
                else log_data += "\t";
            }
            sw.WriteLine(log_data);
        }

        private DeltaE calculate_DeltaE_max(int set, int ch)
        {
            DeltaE calc_data;

            calc_data.X = data_list[set][ch][index_max].double_X / data_list[set][ch][index_max].double_Y * data_list[set][ch][index_max].double_Lv;
            calc_data.Y = data_list[set][ch][index_max].double_Lv;
            calc_data.Z = (1 - data_list[set][ch][index_max].double_X - data_list[set][ch][index_max].double_Y) / data_list[set][ch][index_max].double_Y * data_list[set][ch][index_max].double_Lv;

            calc_data.Xn = calc_data.X / calc_data.X;
            calc_data.Yn = calc_data.Y / calc_data.Y;
            calc_data.Zn = calc_data.Z / calc_data.Z;

            if (calc_data.Xn > 0.008856) calc_data.fx = Math.Pow(calc_data.Xn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fx = 7.787 * calc_data.Xn + (Convert.ToDouble(16) / Convert.ToDouble(116));
            if (calc_data.Yn > 0.008856) calc_data.fy = Math.Pow(calc_data.Yn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fy = 7.787 * calc_data.Yn + (Convert.ToDouble(16) / Convert.ToDouble(116));
            if (calc_data.Zn > 0.008856) calc_data.fz = Math.Pow(calc_data.Zn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fz = 7.787 * calc_data.Zn + (Convert.ToDouble(16) / Convert.ToDouble(116));

            if (calc_data.Yn > 0.008856) calc_data.L = 116 * Math.Pow(calc_data.Yn, Convert.ToDouble(1) / Convert.ToDouble(3)) - 16;
            else calc_data.L = 903.3 * calc_data.Yn;
            calc_data.a = 500 * (calc_data.fx - calc_data.fy);
            calc_data.b = 200 * (calc_data.fy - calc_data.fz);

            calc_data.delta_a = calc_data.a - calc_data.a;
            calc_data.delta_b = calc_data.b - calc_data.b;
            calc_data.delta_L = calc_data.L - calc_data.L;

            calc_data.delta_C = Math.Sqrt(Math.Pow(calc_data.delta_a,2) + Math.Pow(calc_data.delta_b,2));
            calc_data.delta_E = Math.Sqrt(Math.Pow(calc_data.delta_L, 2) + Math.Pow(calc_data.delta_C, 2));

            return calc_data;
        }
        private DeltaE calculate_DeltaE(int set, int ch, int index, DeltaE ref_data)
        {
            DeltaE calc_data;

            calc_data.X = data_list[set][ch][index].double_X / data_list[set][ch][index].double_Y * data_list[set][ch][index].double_Lv;
            calc_data.Y = data_list[set][ch][index].double_Lv;
            calc_data.Z = (1 - data_list[set][ch][index].double_X - data_list[set][ch][index].double_Y) / data_list[set][ch][index].double_Y * data_list[set][ch][index].double_Lv;

            calc_data.Xn = calc_data.X / ref_data.X;
            calc_data.Yn = calc_data.Y / ref_data.Y;
            calc_data.Zn = calc_data.Z / ref_data.Z;

            if (calc_data.Xn > 0.008856) calc_data.fx = Math.Pow(calc_data.Xn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fx = 7.787 * calc_data.Xn + (Convert.ToDouble(16) / Convert.ToDouble(116));
            if (calc_data.Yn > 0.008856) calc_data.fy = Math.Pow(calc_data.Yn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fy = 7.787 * calc_data.Yn + (Convert.ToDouble(16) / Convert.ToDouble(116));
            if (calc_data.Zn > 0.008856) calc_data.fz = Math.Pow(calc_data.Zn, Convert.ToDouble(1) / Convert.ToDouble(3));
            else calc_data.fz = 7.787 * calc_data.Zn + (Convert.ToDouble(16) / Convert.ToDouble(116));

            if (calc_data.Yn > 0.008856) calc_data.L = 116 * Math.Pow(calc_data.Yn, Convert.ToDouble(1) / Convert.ToDouble(3)) - 16;
            else calc_data.L = 903.3 * calc_data.Yn;
            calc_data.a = 500 * (calc_data.fx - calc_data.fy);
            calc_data.b = 200 * (calc_data.fy - calc_data.fz);

            calc_data.delta_a = calc_data.a - ref_data.a;
            calc_data.delta_b = calc_data.b - ref_data.b;
            calc_data.delta_L = calc_data.L - ref_data.L;

            calc_data.delta_C = Math.Sqrt(Math.Pow(calc_data.delta_a, 2) + Math.Pow(calc_data.delta_b, 2));
            calc_data.delta_E = Math.Sqrt(Math.Pow(calc_data.delta_L, 2) + Math.Pow(calc_data.delta_C, 2));

            return calc_data;
        }
        private RGB calcultae_Gamma_Crush(int set, int ch)
        {
            RGB ratio;
            RGB target_Lv;
            RGB gamma_crush;

            double Wx = data_list[set][ch][0].double_X;
            double Wy = data_list[set][ch][0].double_Y;
            double WLV = data_list[set][ch][0].double_Lv;

            double Rx = data_list[set][ch][1].double_X;
            double Ry = data_list[set][ch][1].double_Y;
            double RLV = data_list[set][ch][1].double_Lv;

            double Gx = data_list[set][ch][2].double_X;
            double Gy = data_list[set][ch][2].double_Y;
            double GLV = data_list[set][ch][2].double_Lv;

            double Bx = data_list[set][ch][3].double_X;
            double By = data_list[set][ch][3].double_Y;
            double BLV = data_list[set][ch][3].double_Lv;

            ratio.data_G = 1;
            ratio.data_B = (By * (Wy * (Gx - Rx) + Ry * (Wx - Gx) + Gy * (Rx - Wx))) / (Gy * (Wy * (Rx - Bx) + By * (Wx - Rx) + Ry * (Bx - Wx)));
            ratio.data_R = ((Wx - Bx) * Ry * ratio.data_B) / (By * (Rx - Wx)) + (Ry * (Wx - Gx)) / (Gy * (Rx - Wx));

            target_Lv.data_R = WLV * (ratio.data_R / (ratio.data_R + ratio.data_G + ratio.data_B));
            target_Lv.data_G = WLV * (ratio.data_G / (ratio.data_R + ratio.data_G + ratio.data_B));
            target_Lv.data_B = WLV * (ratio.data_B / (ratio.data_R + ratio.data_G + ratio.data_B));

            gamma_crush.data_R = RLV / target_Lv.data_R * 100;
            gamma_crush.data_G = GLV / target_Lv.data_G * 100;
            gamma_crush.data_B = BLV / target_Lv.data_B * 100;

            return gamma_crush;
        }

        private void GCS_DBV_check()
        {
            GCS_check_count = 0;

            //Textbox to String
            value_GCS_DBV[0] = GCS_DBV1.Text;
            value_GCS_DBV[1] = GCS_DBV2.Text;
            value_GCS_DBV[2] = GCS_DBV3.Text;
            value_GCS_DBV[3] = GCS_DBV4.Text;
            value_GCS_DBV[4] = GCS_DBV5.Text;
            value_GCS_DBV[5] = GCS_DBV6.Text;
            value_GCS_DBV[6] = GCS_DBV7.Text;
            value_GCS_DBV[7] = GCS_DBV8.Text;
            value_GCS_DBV[8] = GCS_DBV9.Text;
            value_GCS_DBV[9] = GCS_DBV10.Text;
            value_GCS_DBV[10] = GCS_DBV11.Text;
            value_GCS_DBV[11] = GCS_DBV12.Text;
            value_GCS_DBV[12] = GCS_DBV13.Text;
            value_GCS_DBV[13] = GCS_DBV14.Text;
            value_GCS_DBV[14] = GCS_DBV15.Text;
            value_GCS_DBV[15] = GCS_DBV16.Text;
            value_GCS_DBV[16] = GCS_DBV17.Text;
            value_GCS_DBV[17] = GCS_DBV18.Text;
            value_GCS_DBV[18] = GCS_DBV19.Text;
            value_GCS_DBV[19] = GCS_DBV20.Text;

            //CheckBox to Bool
            check_GCS_DBV[0] = check_GCS_DBV1.Checked;
            check_GCS_DBV[1] = check_GCS_DBV2.Checked;
            check_GCS_DBV[2] = check_GCS_DBV3.Checked;
            check_GCS_DBV[3] = check_GCS_DBV4.Checked;
            check_GCS_DBV[4] = check_GCS_DBV5.Checked;
            check_GCS_DBV[5] = check_GCS_DBV6.Checked;
            check_GCS_DBV[6] = check_GCS_DBV7.Checked;
            check_GCS_DBV[7] = check_GCS_DBV8.Checked;
            check_GCS_DBV[8] = check_GCS_DBV9.Checked;
            check_GCS_DBV[9] = check_GCS_DBV10.Checked;
            check_GCS_DBV[10] = check_GCS_DBV11.Checked;
            check_GCS_DBV[11] = check_GCS_DBV12.Checked;
            check_GCS_DBV[12] = check_GCS_DBV13.Checked;
            check_GCS_DBV[13] = check_GCS_DBV14.Checked;
            check_GCS_DBV[14] = check_GCS_DBV15.Checked;
            check_GCS_DBV[15] = check_GCS_DBV16.Checked;
            check_GCS_DBV[16] = check_GCS_DBV17.Checked;
            check_GCS_DBV[17] = check_GCS_DBV18.Checked;
            check_GCS_DBV[18] = check_GCS_DBV19.Checked;
            check_GCS_DBV[19] = check_GCS_DBV20.Checked;

            for (int gcs = 0; gcs < 10; gcs++)
            {
                if (check_GCS_DBV[gcs]) GCS_check_count++;
            }

            if (GCS_check_count == 0) GCS_measure = false;
        }
        private void GCS_All_DBV_Status(bool Checked)
        {
            check_GCS_DBV1.Checked = Checked;
            check_GCS_DBV2.Checked = Checked;
            check_GCS_DBV3.Checked = Checked;
            check_GCS_DBV4.Checked = Checked;
            check_GCS_DBV5.Checked = Checked;
            check_GCS_DBV6.Checked = Checked;
            check_GCS_DBV7.Checked = Checked;
            check_GCS_DBV8.Checked = Checked;
            check_GCS_DBV9.Checked = Checked;
            check_GCS_DBV10.Checked = Checked;
            check_GCS_DBV11.Checked = Checked;
            check_GCS_DBV12.Checked = Checked;
            check_GCS_DBV13.Checked = Checked;
            check_GCS_DBV14.Checked = Checked;
            check_GCS_DBV15.Checked = Checked;
            check_GCS_DBV16.Checked = Checked;
            check_GCS_DBV17.Checked = Checked;
            check_GCS_DBV18.Checked = Checked;
            check_GCS_DBV19.Checked = Checked;
            check_GCS_DBV20.Checked = Checked;
        }
        private void button_GCS_DBV_all_select_Click(object sender, EventArgs e)
        {
            GCS_All_DBV_Status(true);
        }
        private void button_GCS_DBV_all_deselect_Click(object sender, EventArgs e)
        {
            GCS_All_DBV_Status(false);
        }
        private MinMax GCS_Min_Max_check(int max_gray, int min_gray)
        {
            MinMax GCS_MinMax = new MinMax();

            GCS_MinMax.Max = max_gray;
            GCS_MinMax.Min = min_gray;

            if (max_gray > 255)
            {
                GCS_max_gray.Text = "255";
                max_gray = 255;
                GCS_MinMax.Max = 255;
            }
            if (min_gray < 0)
            {
                GCS_min_gray.Text = "0";
                min_gray = 0;
                GCS_MinMax.Min = 0;
            }
            return GCS_MinMax;
        }
        private void GCS_Measure()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];

            label_mornitoring.Text = "Open file for saving GCS measurement data";
           
            ///////////////////// Log 저장을 위한 file setting
            string FileName_load = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\format\\GCS_format.csv";
            string FileName_save = Make_new_folder(Start_Time) + "\\GCS_" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US")) + ".csv";

            StreamReader sr = new StreamReader(FileName_load);
            FileStream stream = new FileStream(FileName_save, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(Regex.Replace(line, ",", "\t", RegexOptions.IgnoreCase));
            }

            sr.Close();
            ///////////////////// Log 저장을 위한 file setting

            MinMax GCS_MinMax = new MinMax();
            int max_gray = Convert.ToInt32(GCS_max_gray.Text);
            int min_gray = Convert.ToInt32(GCS_min_gray.Text);
            int gray_step = Convert.ToInt32(GCS_step.Text);
            int PTN_delay = Convert.ToInt16(GCS_Delay.Text);

            GCS_MinMax = GCS_Min_Max_check(max_gray, min_gray);
            progressBar_GCS.PerformStep();

            // GCS Measurement
            for (int i = 0; i < 20 & GCS_measure; i++)
            {
                data_index = 0;

                System.Windows.Forms.Application.DoEvents();
                if (check_GCS_DBV[i] && GCS_measure)
                {
                    string DBV = value_GCS_DBV[i].PadLeft(3, '0');//dex to hex (as a string form)
                    try
                    {
                        f1.DBV_Setting(DBV);
                        Thread.Sleep(PTN_delay);

                        GCS_header_update(i+1, DBV);
                        row_count++;

                        f1.GB_Status_AppendText_Nextline("multi CH measurement GCS" + (i+1).ToString() + ") DBV[" + DBV + "] was applied", Color.Blue);
                    }
                    catch
                    {
                        f1.GB_Status_AppendText_Nextline("multi CH measurement GCS" + (i + 1).ToString() + ") DBV[" + DBV + "] was failed", Color.Red);
                    }

                    try
                    {
                        GCS_Measure_step(i + 1, DBV, GCS_MinMax.Min, GCS_MinMax.Max, gray_step, PTN_delay);

                        save_GCS_data(i + 1, DBV, sw);
                    }
                    catch
                    {
                        sw.Close();
                        stream.Close();

                        stop_flag = true;
                        measure_flahg_change(false);
                    }
                }
                else
                {
                    f1.GB_Status_AppendText_Nextline("multi CH measurement GCS" + (i + 1).ToString() + ") DBV Point Skip", Color.Black);
                }
            }
            
            if (!stop_flag)
            {
                label_mornitoring.Text = "Save the GCS measurement data to file";

                sw.Close();
                stream.Close();
            }
        }
        private void GCS_Measure_step(int num, string DBV, int min_gray, int max_gray, int gray_step, int PTN_delay)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            int gray=0;
            int number_of_measure = (max_gray - min_gray) / gray_step + 1;
            double number_of_measure_remain = (max_gray - min_gray) % gray_step;
            if(number_of_measure_remain>=0.5) number_of_measure++;

            count_measure = number_of_measure;
            for (int set = 0; set < 6; set++)
            {
                index_list = new int[count_measure];
                for (int ch = 0; ch < 10; ch++)
                {
                    data_list[set][ch] = new XYLv[count_measure];
                }
            }

            if (GCS_Max_to_Min.Checked)
            {
                index_max = 0;
                gray = max_gray;
            }
            else
            {
                index_max = number_of_measure - 1;
                gray = min_gray;
            }

            for (int n = 0; n < number_of_measure & GCS_measure; n++)
            {
                System.Windows.Forms.Application.DoEvents();
                label_mornitoring.Text = "GCS Measure : GCS" + num.ToString() + ") DBV 0x" + DBV + " Gray " + gray.ToString();

                try
                {
                    GCS_row_update(gray);

                    row_count++;

                    f1.PTN_update(gray, gray, gray);

                    Thread.Sleep(PTN_delay);

                    Set_change_Measure();

                    GCS_measurement_data_Save(num, n, DBV, gray);

                    if (GCS_Max_to_Min.Checked)
                    {
                        gray = gray - gray_step;
                        if (gray < min_gray) gray = min_gray;
                    }
                    else
                    {
                        gray = gray + gray_step;
                        if (gray > max_gray) gray = max_gray;
                    }

                    progressBar_GCS.PerformStep();
                    progressBar_Measurement.PerformStep();
                }
                catch
                {
                    stop_flag = true;
                    measure_flahg_change(false);
                }
            }


        }
        private int GCS_Progress_Bar_Setting()
        {
            int max_gray = Convert.ToInt32(GCS_max_gray.Text);
            int min_gray = Convert.ToInt32(GCS_min_gray.Text);
            int gray_step = Convert.ToInt32(GCS_step.Text);
            int number_of_measure = (max_gray - min_gray) / gray_step + 1;
            double number_of_measure_remain = (max_gray - min_gray) % gray_step;
            if (number_of_measure_remain >= 0.5) number_of_measure++;

            progressBar_GCS.Value = 0;
            progressBar_GCS.Step = 1;
            progressBar_GCS.Minimum = 0;
            progressBar_GCS.Maximum = GCS_check_count * number_of_measure;

            return progressBar_GCS.Maximum;
        }
        private void GCS_header_update(int number, string DBV)
        {
            for (int ch = 0; ch < 10 & GCS_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add("GCS", number.ToString() + ")DBV", DBV, "", number.ToString() + ")DBV", DBV, "", number.ToString() + ")DBV", DBV, "", number.ToString() + ")DBV", DBV, "", number.ToString() + ")DBV", DBV, "", number.ToString() + ")DBV", DBV, "");
                }
            }
        }
        private void GCS_row_update(int gray)
        {
            for (int ch = 0; ch < 10 & GCS_measure; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add(gray.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void GCS_measurement_data_Save(int number, int measure_num, string DBV, int gray)
        {
            Application.DoEvents();

            for (int ch = 0; ch < 10; ch++)
            {
                for (int set = 0; set < 6; set++) // Set & 시료별 data write
                {
                    if (check_SET[set] && check_PNC_CH[ch])
                    {
                        index_list[data_index] = gray;

                        data_list[set][ch][data_index].double_X = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 1].Value);
                        data_list[set][ch][data_index].double_Y = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 2].Value);
                        data_list[set][ch][data_index].double_Lv = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 3].Value);
                    }
                }
            }
            data_index++;
        }

        private void BCS_Gray_check()
        {
            BCS_check_count = 0;
            BCS_range_check_count = 0;

            //Textbox to String
            value_BCS_Gray[0] = BCS_Gray1.Text;
            value_BCS_Gray[1] = BCS_Gray2.Text;
            value_BCS_Gray[2] = BCS_Gray3.Text;
            value_BCS_Gray[3] = BCS_Gray4.Text;
            value_BCS_Gray[4] = BCS_Gray5.Text;
            value_BCS_Gray[5] = BCS_Gray6.Text;
            value_BCS_Gray[6] = BCS_Gray7.Text;
            value_BCS_Gray[7] = BCS_Gray8.Text;
            value_BCS_Gray[8] = BCS_Gray9.Text;
            value_BCS_Gray[9] = BCS_Gray10.Text;
            value_BCS_Gray[10] = BCS_Gray11.Text;
            value_BCS_Gray[11] = BCS_Gray12.Text;
            value_BCS_Gray[12] = BCS_Gray13.Text;
            value_BCS_Gray[13] = BCS_Gray14.Text;
            value_BCS_Gray[14] = BCS_Gray15.Text;
            value_BCS_Gray[15] = BCS_Gray16.Text;
            value_BCS_Gray[16] = BCS_Gray17.Text;
            value_BCS_Gray[17] = BCS_Gray18.Text;
            value_BCS_Gray[18] = BCS_Gray19.Text;
            value_BCS_Gray[19] = BCS_Gray20.Text;

            //CheckBox to Bool
            check_BCS_Gray[0] = check_BCS_Gray1.Checked;
            check_BCS_Gray[1] = check_BCS_Gray2.Checked;
            check_BCS_Gray[2] = check_BCS_Gray3.Checked;
            check_BCS_Gray[3] = check_BCS_Gray4.Checked;
            check_BCS_Gray[4] = check_BCS_Gray5.Checked;
            check_BCS_Gray[5] = check_BCS_Gray6.Checked;
            check_BCS_Gray[6] = check_BCS_Gray7.Checked;
            check_BCS_Gray[7] = check_BCS_Gray8.Checked;
            check_BCS_Gray[8] = check_BCS_Gray9.Checked;
            check_BCS_Gray[9] = check_BCS_Gray10.Checked;
            check_BCS_Gray[10] = check_BCS_Gray11.Checked;
            check_BCS_Gray[11] = check_BCS_Gray12.Checked;
            check_BCS_Gray[12] = check_BCS_Gray13.Checked;
            check_BCS_Gray[13] = check_BCS_Gray14.Checked;
            check_BCS_Gray[14] = check_BCS_Gray15.Checked;
            check_BCS_Gray[15] = check_BCS_Gray16.Checked;
            check_BCS_Gray[16] = check_BCS_Gray17.Checked;
            check_BCS_Gray[17] = check_BCS_Gray18.Checked;
            check_BCS_Gray[18] = check_BCS_Gray19.Checked;
            check_BCS_Gray[19] = check_BCS_Gray20.Checked;

            check_BCS_Range[0] = BCS_Range1.Checked;
            check_BCS_Range[1] = BCS_Range2.Checked;
            check_BCS_Range[2] = BCS_Range3.Checked;

            value_BCS_Range_min_DBV[0] = BCS_Range1_min_DBV.Text;
            value_BCS_Range_min_DBV[1] = BCS_Range2_min_DBV.Text;
            value_BCS_Range_min_DBV[2] = BCS_Range3_min_DBV.Text;
            
            value_BCS_Range_max_DBV[0] = BCS_Range1_max_DBV.Text;
            value_BCS_Range_max_DBV[1] = BCS_Range2_max_DBV.Text;
            value_BCS_Range_max_DBV[2] = BCS_Range3_max_DBV.Text;

            value_BCS_Range_DBV_step[0] = BCS_Range1_step.Text;
            value_BCS_Range_DBV_step[1] = BCS_Range2_step.Text;
            value_BCS_Range_DBV_step[2] = BCS_Range3_step.Text;

            for (int bcs = 0; bcs < 10; bcs++)
            {
                if (check_BCS_Gray[bcs]) BCS_check_count++;
            }
            for (int range = 0; range < 3; range++)
            {
                if (check_BCS_Range[range]) BCS_range_check_count++;
            }

            if ((BCS_check_count == 0) || (BCS_range_check_count == 0)) BCS_measure = false;
        }
        private void BCS_All_Gray_Status(bool Checked)
        {
            check_BCS_Gray1.Checked = Checked;
            check_BCS_Gray2.Checked = Checked;
            check_BCS_Gray3.Checked = Checked;
            check_BCS_Gray4.Checked = Checked;
            check_BCS_Gray5.Checked = Checked;
            check_BCS_Gray6.Checked = Checked;
            check_BCS_Gray7.Checked = Checked;
            check_BCS_Gray8.Checked = Checked;
            check_BCS_Gray9.Checked = Checked;
            check_BCS_Gray10.Checked = Checked;
            check_BCS_Gray11.Checked = Checked;
            check_BCS_Gray12.Checked = Checked;
            check_BCS_Gray13.Checked = Checked;
            check_BCS_Gray14.Checked = Checked;
            check_BCS_Gray15.Checked = Checked;
            check_BCS_Gray16.Checked = Checked;
            check_BCS_Gray17.Checked = Checked;
            check_BCS_Gray18.Checked = Checked;
            check_BCS_Gray19.Checked = Checked;
            check_BCS_Gray20.Checked = Checked;
        }
        private void button_BCS_Gray_all_select_Click_1(object sender, EventArgs e)
        {
            BCS_All_Gray_Status(true);
        }
        private void button_BCS_Gray_all_deselect_Click_1(object sender, EventArgs e)
        {
            BCS_All_Gray_Status(false);
        }
        private MinMax[] BCS_Min_Max_check(int max_limit_DBV)
        {
            MinMax[] BCS_MinMax = new MinMax[3];
            
            for (int r = 0; r < 3 & BCS_measure; r++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_BCS_Range[r] && BCS_measure)
                {
                    BCS_MinMax[r].Max = Convert.ToInt32(value_BCS_Range_max_DBV[r]);
                    BCS_MinMax[r].Min = Convert.ToInt32(value_BCS_Range_min_DBV[r]);

                    if (BCS_MinMax[r].Max > max_limit_DBV)
                    {
                        switch (r)
                        {
                            case 0:
                                BCS_Range1_max_DBV.Text = max_limit_DBV.ToString();
                                value_BCS_Range_max_DBV[r] = max_limit_DBV.ToString();
                                break;
                            case 1:
                                BCS_Range2_max_DBV.Text = max_limit_DBV.ToString();
                                value_BCS_Range_max_DBV[r] = max_limit_DBV.ToString();
                                break;
                            case 2:
                                BCS_Range3_max_DBV.Text = max_limit_DBV.ToString();
                                value_BCS_Range_max_DBV[r] = max_limit_DBV.ToString();
                                break;
                        }
                        BCS_MinMax[r].Max = max_limit_DBV;
                    }
                    if (BCS_MinMax[r].Min < 1)
                    {
                        switch (r)
                        {
                            case 0:
                                BCS_Range1_min_DBV.Text = "1";
                                value_BCS_Range_min_DBV[r] = "1";
                                break;
                            case 1:
                                BCS_Range2_min_DBV.Text = "1";
                                value_BCS_Range_min_DBV[r] = "1";
                                break;
                            case 2:
                                BCS_Range3_min_DBV.Text = "1";
                                value_BCS_Range_min_DBV[r] = "1";
                                break;
                        }
                        BCS_MinMax[r].Min = 1;
                    }
                }
            }

            return BCS_MinMax;
        }
        private void BCS_Measure()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];

            label_mornitoring.Text = "Open file for saving BCS measurement data";
            ///////////////////// Log 저장을 위한 file setting
            string FileName_load = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\format\\BCS_format.csv";
            string FileName_save = Make_new_folder(Start_Time) + "\\BCS_" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US")) + ".csv";

            StreamReader sr = new StreamReader(FileName_load);
            FileStream stream = new FileStream(FileName_save, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(Regex.Replace(line, ",", "\t", RegexOptions.IgnoreCase));
            }

            sr.Close();
            ///////////////////// Log 저장을 위한 file setting

            int DBV_step=0;
            int DBV_delay = Convert.ToInt16(BCS_Delay.Text);
            MinMax[] BCS_MinMax = new MinMax[3];
            int max_limit_DBV = f1.current_model.get_DBV_Max();

            BCS_MinMax = BCS_Min_Max_check(max_limit_DBV);

            progressBar_BCS.PerformStep();
            
            for (int i = 0; i < 20 & BCS_measure; i++)
            {
                if (check_BCS_Gray[i] && BCS_measure)
                {
                    int gray = Convert.ToInt32(value_BCS_Gray[i]);
                    for (int r = 0; r < 3 & BCS_measure; r++)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        if (check_BCS_Range[r] && BCS_measure)
                        {
                            data_index = 0;
                            DBV_step = Convert.ToInt32(value_BCS_Range_DBV_step[r]);
                        
                            System.Windows.Forms.Application.DoEvents();

                            try
                            {
                                f1.PTN_update(gray, gray, gray);
                                BCS_header_update(i+1, gray, r+1);
                                
                                row_count++;

                                f1.GB_Status_AppendText_Nextline("multi CH measurement BCS" + (i+1).ToString() + ")Gray" + gray.ToString() + " was applied", Color.Blue);
                            }
                            catch
                            {
                                f1.GB_Status_AppendText_Nextline("multi CH measurement BCS" + (i + 1).ToString() + ")Gray point was skipped was failed", Color.Black);
                            }

                            try
                            {
                                BCS_Measure_step(i + 1, r, gray, BCS_MinMax[r].Min, BCS_MinMax[r].Max, DBV_step, DBV_delay);

                                save_BCS_data(r+1,i+1,gray,sw);
                            }
                            catch
                            {
                                sw.Close();
                                stream.Close();

                                stop_flag = true;
                                measure_flahg_change(false);
                            }
                        }
                        else
                        {
                            f1.GB_Status_AppendText_Nextline("multi CH measurement BCS" + (i + 1).ToString() + ")Gray Point Skip", Color.Black);
                        }
                    }
                }
            }

            if (!stop_flag)
            {
                label_mornitoring.Text = "Save the BCS measurement data to file";

                sw.Close();
                stream.Close();
            }
        }
        private void BCS_Measure_step(int num, int range, int gray, int min_DBV, int max_DBV, int DBV_step, int DBV_delay)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            int DBV = 0;
            int number_of_measure = (max_DBV - min_DBV) / DBV_step + 1;
            double number_of_measure_remain = (max_DBV - min_DBV) % DBV_step;
            if (number_of_measure_remain >= 0.5) number_of_measure++;

            count_measure = number_of_measure;
            for (int set = 0; set < 6; set++)
            {
                index_list = new int[count_measure];
                for (int ch = 0; ch < 10; ch++)
                {
                    data_list[set][ch] = new XYLv[count_measure];
                }
            }

            if (BCS_Max_to_Min.Checked)
            {
                index_max = 0;
                DBV = max_DBV;
            }
            else
            {
                index_max = number_of_measure - 1;
                DBV = min_DBV;
            }

            for (int n = 0; n < number_of_measure & BCS_measure; n++)
            {
                System.Windows.Forms.Application.DoEvents();
                label_mornitoring.Text = "BCS Measure : range" + (range + 1).ToString() + "-BCS" + num.ToString() + ") Gray" + gray.ToString() + " DBV 0x" + DBV.ToString("X3");
                BCS_row_update(DBV);
                row_count++;

                try
                {
                    f1.DBV_Setting(DBV.ToString("X3"));
                    Thread.Sleep(DBV_delay);

                    Set_change_Measure();

                    BCS_measurement_data_Save(range, n, DBV, gray);

                    // 측정 후 Gray 변경
                    if (BCS_Max_to_Min.Checked)
                    {
                        DBV = DBV - DBV_step;
                        if (DBV < min_DBV) DBV = min_DBV;
                    }
                    else
                    {
                        DBV = DBV + DBV_step;
                        if (DBV > max_DBV) DBV = max_DBV;
                    }
                    progressBar_BCS.PerformStep();
                    progressBar_Measurement.PerformStep();
                }
                catch
                {
                    stop_flag = true;
                    measure_flahg_change(false);
                }
            }
        }
        private int BCS_Progress_Bar_Setting()
        {
            //Set ProgressBar
            int multiple = 0;

            for (int r = 0; r < 3 & BCS_measure; r++)
            {
                int max_DBV = Convert.ToInt32(value_BCS_Range_max_DBV[r]);
                int min_DBV = Convert.ToInt32(value_BCS_Range_min_DBV[r]);
                int DBV_step = Convert.ToInt32(value_BCS_Range_DBV_step[r]);

                int number_of_measure = (max_DBV - min_DBV) / DBV_step + 1;
                double number_of_measure_remain = (max_DBV - min_DBV) % DBV_step;
                if (number_of_measure_remain >= 0.5) number_of_measure++;

                if (check_BCS_Range[r])
                {
                    multiple += number_of_measure;
                }
            }

            progressBar_BCS.Value = 0;
            progressBar_BCS.Step = 1;
            progressBar_BCS.Minimum = 0;
            progressBar_BCS.Maximum = BCS_check_count * multiple;

            return progressBar_BCS.Maximum;
        }
        private void BCS_header_update(int number, int gray, int range)
        {
            for (int ch = 0; ch < 10 & BCS_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add("BCS-" + range.ToString(), number.ToString() + ")Gray", gray, "", number.ToString() + ")Gray", gray, "", number.ToString() + ")Gray", gray, "", number.ToString() + ")Gray", gray, "", number.ToString() + ")Gray", gray, "", number.ToString() + ")Gray", gray, "");
                }
            }
        }
        private void BCS_row_update(int DBV)
        {
            for (int ch = 0; ch < 10 & BCS_measure; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add(DBV.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void BCS_measurement_data_Save(int range, int measure_num, int DBV, int gray)
        {
            Application.DoEvents();

            for (int ch = 0; ch < 10; ch++)
            {
                for (int set = 0; set < 6; set++) // Set & 시료별 data write
                {
                    if (check_SET[set] && check_PNC_CH[ch])
                    {
                        index_list[data_index] = DBV;

                        data_list[set][ch][data_index].double_X = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 1].Value);
                        data_list[set][ch][data_index].double_Y = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 2].Value);
                        data_list[set][ch][data_index].double_Lv = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 3].Value);
                    }
                }
            }

            data_index++;
        }

        private void Gamma_Crush_Setting_check()
        {
            Gamma_Crush_check_count = 0;
            Gamma_Crush_check_count = 0;

            //Textbox to String
            value_Gamma_Crush_DBV[0] = Gamma_Crush_DBV1.Text;
            value_Gamma_Crush_DBV[1] = Gamma_Crush_DBV2.Text;
            value_Gamma_Crush_DBV[2] = Gamma_Crush_DBV3.Text;
            value_Gamma_Crush_DBV[3] = Gamma_Crush_DBV4.Text;
            value_Gamma_Crush_DBV[4] = Gamma_Crush_DBV5.Text;
            value_Gamma_Crush_DBV[5] = Gamma_Crush_DBV6.Text;
            value_Gamma_Crush_DBV[6] = Gamma_Crush_DBV7.Text;
            value_Gamma_Crush_DBV[7] = Gamma_Crush_DBV8.Text;
            value_Gamma_Crush_DBV[8] = Gamma_Crush_DBV9.Text;
            value_Gamma_Crush_DBV[9] = Gamma_Crush_DBV10.Text;

            //Textbox to String
            value_Gamma_Crush_Gray[0] = Gamma_Crush_Gray1.Text;
            value_Gamma_Crush_Gray[1] = Gamma_Crush_Gray2.Text;
            value_Gamma_Crush_Gray[2] = Gamma_Crush_Gray3.Text;
            value_Gamma_Crush_Gray[3] = Gamma_Crush_Gray4.Text;
            value_Gamma_Crush_Gray[4] = Gamma_Crush_Gray5.Text;
            value_Gamma_Crush_Gray[5] = Gamma_Crush_Gray6.Text;
            value_Gamma_Crush_Gray[6] = Gamma_Crush_Gray7.Text;
            value_Gamma_Crush_Gray[7] = Gamma_Crush_Gray8.Text;
            value_Gamma_Crush_Gray[8] = Gamma_Crush_Gray9.Text;
            value_Gamma_Crush_Gray[9] = Gamma_Crush_Gray10.Text;

            //CheckBox to Bool
            check_Gamma_Crush[0] = check_Gamma_Crush1.Checked;
            check_Gamma_Crush[1] = check_Gamma_Crush2.Checked;
            check_Gamma_Crush[2] = check_Gamma_Crush3.Checked;
            check_Gamma_Crush[3] = check_Gamma_Crush4.Checked;
            check_Gamma_Crush[4] = check_Gamma_Crush5.Checked;
            check_Gamma_Crush[5] = check_Gamma_Crush6.Checked;
            check_Gamma_Crush[6] = check_Gamma_Crush7.Checked;
            check_Gamma_Crush[7] = check_Gamma_Crush8.Checked;
            check_Gamma_Crush[8] = check_Gamma_Crush9.Checked;
            check_Gamma_Crush[9] = check_Gamma_Crush10.Checked;

            //CheckBox to Bool
            check_Gamma_Crush_color[0] = checkBox_Gamma_Crush_W.Checked;
            check_Gamma_Crush_color[1] = checkBox_Gamma_Crush_R.Checked;
            check_Gamma_Crush_color[2] = checkBox_Gamma_Crush_G.Checked;
            check_Gamma_Crush_color[3] = checkBox_Gamma_Crush_B.Checked;

            for (int gc = 0; gc < 10; gc++)
            {
                if (check_Gamma_Crush[gc]) Gamma_Crush_check_count++;
            }
            for (int color = 0; color < 4; color++)
            {
                if (check_Gamma_Crush_color[color]) Gamma_Crush_Color_check_count++;
            }
            if ((Gamma_Crush_check_count == 0) || (Gamma_Crush_Color_check_count == 0)) Gamma_Crush_measure = false;
        }
        private void Gamma_Crush_Measure()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];

            label_mornitoring.Text = "Open file for saving Gamma_Crush measurement data";
            ///////////////////// Log 저장을 위한 file setting
            string FileName_load = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\format\\Gamma_Crush_format.csv";
            string FileName_save = Make_new_folder(Start_Time) + "\\Gamma_Crush" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US")) + ".csv";

            StreamReader sr = new StreamReader(FileName_load);
            FileStream stream = new FileStream(FileName_save, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(Regex.Replace(line, ",", "\t", RegexOptions.IgnoreCase));
            }

            sr.Close();
            ///////////////////// Log 저장을 위한 file setting

            int DBV_delay = Convert.ToInt16(Gamma_Crush_DBV_Delay.Text);
            int PTN_delay = Convert.ToInt16(Gamma_Crush_PTN_Delay.Text);

            progressBar_Gamma_Crush.PerformStep();

            for (int i = 0; i < 10 & Gamma_Crush_measure; i++)
            {
                data_index = 0;
                System.Windows.Forms.Application.DoEvents();
                if (check_Gamma_Crush[i] && Gamma_Crush_measure)
                {
                    int gray = Convert.ToInt16(value_Gamma_Crush_Gray[i]);
                    string DBV = value_Gamma_Crush_DBV[i].PadLeft(3, '0');

                    try
                    {
                        f1.DBV_Setting(DBV);
                        Thread.Sleep(DBV_delay);
                        Gamma_Crush_header_update(i+1);

                        row_count++;

                        f1.GB_Status_AppendText_Nextline("multi CH measurement Gamma Cursh" + (i + 1).ToString() + ") DBV[" + DBV + "] was applied", Color.Blue);
                    }
                    catch
                    {
                        f1.GB_Status_AppendText_Nextline("multi CH measurement Gamma Cursh" + (i + 1).ToString() + ") DBV[" + DBV + "] was failed", Color.Red);
                    }

                    try
                    {
                        Gamma_Crush_Measure_step(i + 1, gray, DBV, PTN_delay);

                        save_Gamma_Crush(i + 1, DBV, gray, sw);
                    }
                    catch
                    {
                        sw.Close();
                        stream.Close();

                        stop_flag = true;
                        measure_flahg_change(false);
                    }
                }
                else
                {
                    f1.GB_Status_AppendText_Nextline("multi CH measurement Gamma Cursh" + (i + 1).ToString() + ") Point Skip", Color.Black);
                }
            }

            if (!stop_flag)
            {
                label_mornitoring.Text = "Save the Gamma_Crush measurement data to file";

                sw.Close();
                stream.Close();
            }
        }
        private void Gamma_Crush_Measure_step(int num, int gray, string DBV, int PTN_delay)
        {
            count_measure = 4;
            
            index_list = new int[count_measure];

            for (int set = 0; set < 6; set++)
            {
                for (int ch = 0; ch < 10; ch++)
                {
                    data_list[set][ch] = new XYLv[count_measure];
                }
            }

            for (int color = 0; color < 4 & Gamma_Crush_measure; color++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_Gamma_Crush_color[color] && Gamma_Crush_measure)
                {
                    string PTN_Color = "";
                    switch (color)
                    {
                        case 0: PTN_Color = "W"; break;
                        case 1: PTN_Color = "R"; break;
                        case 2: PTN_Color = "G"; break;
                        case 3: PTN_Color = "B"; break;
                    }
                    label_mornitoring.Text = "Gamma Crush Measure : " + num.ToString() + ") DBV 0x" + DBV + " Gray " + gray.ToString() + PTN_Color;

                    Gamma_Crush_row_update(color);
                    row_count++;

                    Gamma_Crush_PTN_update(color, gray, PTN_delay);

                    Set_change_Measure();
                    
                    Gamma_Crush_measurement_data_Save(num-1, color, DBV, gray);
                }
                progressBar_Gamma_Crush.PerformStep();
                progressBar_Measurement.PerformStep();
            }
        }
        private void Gamma_Crush_header_update(int number)
        {
           for (int ch = 0; ch < 10 & GCS_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add("G.Crush", number.ToString() + ")", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private int Gamma_Crush_Progress_Bar_Setting()
        {
            ///Set ProgressBar
            progressBar_Gamma_Crush.Value = 0;
            progressBar_Gamma_Crush.Step = 1;
            progressBar_Gamma_Crush.Minimum = 0;
            progressBar_Gamma_Crush.Maximum = Gamma_Crush_check_count * Gamma_Crush_Color_check_count;

            return progressBar_Gamma_Crush.Maximum;
        }
        private void Gamma_Crush_row_update(int color)
        {
            string color_string = null;
            switch (color)
            {
                case 0:
                    color_string = "W";
                    break;
                case 1:
                    color_string = "R";
                    break;
                case 2:
                    color_string = "G";
                    break;
                case 3:
                    color_string = "B";
                    break;
            }
            for (int ch = 0; ch < 10 & GCS_measure; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add(color_string, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void Gamma_Crush_PTN_update(int color, int gray, int PTN_delay)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];

            CA_CH_Change(color);
            CA_CH_Change(color);

            switch (color)
            {
                case 0:
                    f1.PTN_update(gray, gray, gray);
                    Thread.Sleep(PTN_delay);
                    break;
                case 1:
                    f1.PTN_update(gray, 0, 0);
                    Thread.Sleep(PTN_delay);
                    break;
                case 2:
                    f1.PTN_update(0, gray, 0);
                    Thread.Sleep(PTN_delay);
                    break;
                case 3:
                    f1.PTN_update(0, 0, gray);
                    Thread.Sleep(PTN_delay);
                    break;
            }
        }
        private void Gamma_Crush_measurement_data_Save(int num, int color, string DBV, int gray)
        {
            Application.DoEvents();

            for (int ch = 0; ch < 10; ch++)
            {
                for (int set = 0; set < 6; set++) // Set & 시료별 data write
                {
                    if (check_SET[set] && check_PNC_CH[ch])
                    {
                        data_list[set][ch][data_index].double_X = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 1].Value);
                        data_list[set][ch][data_index].double_Y = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 2].Value);
                        data_list[set][ch][data_index].double_Lv = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3 * set + 3].Value);
                    }
                }
            }
            data_index++;
        }

        private void AOD_GCS_DBV_check()
        {
            AOD_GCS_check_count = 0;
            //Textbox to String
            value_AOD_GCS_DBV[0] = AOD_GCS_DBV1.Text;
            value_AOD_GCS_DBV[1] = AOD_GCS_DBV2.Text;
            value_AOD_GCS_DBV[2] = AOD_GCS_DBV3.Text;

            //CheckBox to Bool
            check_AOD_GCS_DBV[0] = check_AOD_GCS_DBV1.Checked;
            check_AOD_GCS_DBV[1] = check_AOD_GCS_DBV2.Checked;
            check_AOD_GCS_DBV[2] = check_AOD_GCS_DBV3.Checked;

            for (int gcs = 0; gcs < 3; gcs++)
            {
                if (check_AOD_GCS_DBV[gcs]) AOD_GCS_check_count++;
            }
            if (GCS_check_count == 0) GCS_measure = false;
        }
        private void AOD_GCS_Measure()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];

            label_mornitoring.Text = "Open file for saving GCS measurement data";
            ///////////////////// Log 저장을 위한 file setting
            string FileName_load = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\format\\AOD_GCS_format.csv";
            string FileName_save = Make_new_folder(Start_Time) + "\\AOD_GCS_" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US")) + ".csv";

            StreamReader sr = new StreamReader(FileName_load);
            FileStream stream = new FileStream(FileName_save, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(Regex.Replace(line, ",", "\t", RegexOptions.IgnoreCase));
            }

            sr.Close();
            ///////////////////// Log 저장을 위한 file setting
            
            int max_gray = Convert.ToInt32(AOD_GCS_max_gray.Text);
            int min_gray = Convert.ToInt32(AOD_GCS_min_gray.Text);
            int gray_step = Convert.ToInt32(AOD_GCS_step.Text);
            int PTN_delay = Convert.ToInt16(AOD_GCS_Delay.Text);
            int Code_delay = Convert.ToInt16(AOD_CODE_Delay.Text);

            if (max_gray > 255)
            {
                AOD_GCS_measure = false;
                System.Windows.Forms.MessageBox.Show("Maximun value of Gray Max is 255");
                GCS_max_gray.Text = "255";
            }
            if (min_gray < 0)
            {
                AOD_GCS_measure = false;
                System.Windows.Forms.MessageBox.Show("Minimum value of Gray Min is 0");
                GCS_min_gray.Text = "0";
            }

            progressBar_AOD_GCS.PerformStep();

            f1.PTN_update(0, 0, 0);
            Thread.Sleep(PTN_delay);
            if(AOD_GCS_measure) f1.AOD_On();
            else f1.AOD_Off();
            Thread.Sleep(Code_delay);

            // GCS Measurement
            for (int i = 0; i < 3 & AOD_GCS_measure; i++)
            {
                data_index = 0;
                System.Windows.Forms.Application.DoEvents();
                if (check_AOD_GCS_DBV[i])
                {
                    string DBV = value_AOD_GCS_DBV[i].PadLeft(3, '0');//dex to hex (as a string form)
                    try
                    {
                        f1.DBV_Setting(DBV);
                        Thread.Sleep(PTN_delay);

                        AOD_GCS_header_update(i, DBV);
                        row_count++;

                        f1.GB_Status_AppendText_Nextline("multi CH measurement AOD GCS" + (i + 1).ToString() + ") DBV[" + DBV + "] was applied", Color.Blue);
                    }
                    catch
                    {
                        f1.GB_Status_AppendText_Nextline("multi CH measurement AOD GCS" + (i + 1).ToString() + ") DBV[" + DBV + "] was failed", Color.Red);
                    }

                    try
                    {
                        AOD_GCS_Measure_step(i+1, DBV, min_gray, max_gray, gray_step, PTN_delay);

                        save_AOD_GCS_data(i + 1, DBV, sw);
                    }
                    catch
                    {
                        sw.Close();
                        stream.Close();

                        stop_flag = true;
                        measure_flahg_change(false);
                    }
                }
                else
                {
                    f1.GB_Status_AppendText_Nextline("multi CH measurement AOD GCS" + (i + 1).ToString() + ") DBV Point Skip", Color.Black);
                }
            }

            if (!stop_flag)
            {
                label_mornitoring.Text = "Save the AOD_GCS measurement data to file";
                sw.Close();
                stream.Close();
            }

            f1.PTN_update(0, 0, 0);
            Thread.Sleep(PTN_delay);
            f1.AOD_Off();
            Thread.Sleep(Code_delay);
            f1.AOD_Off();
            Thread.Sleep(Code_delay);
            f1.AOD_Off();
            Thread.Sleep(Code_delay);
        }

        private void AOD_Pattern_Setting(int Gray)
        {
                f1().IPC_Quick_Send("image.crosstalk " + f1().current_model.get_AOD_X().ToString() + " " + f1().current_model.get_AOD_Y().ToString() + " 0 0 0 " + Gray.ToString() + " " + Gray.ToString() + " " + Gray.ToString());
        }

        private void AOD_GCS_Measure_step(int num, string DBV, int min_gray, int max_gray, int gray_step, int PTN_delay)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            int gray = 0;
            int number_of_measure = (max_gray - min_gray) / gray_step + 1;
            double number_of_measure_remain = (max_gray - min_gray) % gray_step;
            if (number_of_measure_remain >= 0.5) number_of_measure++;

            count_measure = number_of_measure;
            index_list = new int[count_measure];
            for (int ch = 0; ch < 10; ch++)
            {
                data_list[0][ch] = new XYLv[count_measure];
            }

            if (AOD_GCS_Max_to_Min.Checked)
            {
                index_max = 0;
                gray = max_gray;
            }
            else
            {
                index_max = number_of_measure - 1;
                gray = min_gray;
            }

            for (int n = 0; n < number_of_measure & AOD_GCS_measure; n++)
            {
                System.Windows.Forms.Application.DoEvents();
                label_mornitoring.Text = "AoD GCS Measure : AOD GCS" + num.ToString() + ") DBV 0x" + DBV + " Gray " + gray.ToString();
                AOD_GCS_row_update(gray);
                row_count++;

                try
                {
                    AOD_Pattern_Setting(gray);

                    Thread.Sleep(PTN_delay);

                    Set_non_change_Measure();

                    AOD_GCS_measurement_data_Save(num, n, DBV, gray);

                    // 측정 후 Gray 변경
                    if (AOD_GCS_Max_to_Min.Checked)
                    {
                        gray = gray - gray_step;
                        if (gray < min_gray) gray = min_gray;
                    }
                    else
                    {
                        gray = gray + gray_step;
                        if (gray > max_gray) gray = max_gray;
                    }
                    progressBar_AOD_GCS.PerformStep();
                    progressBar_Measurement.PerformStep();
                }
                catch
                {
                    stop_flag = true;
                    measure_flahg_change(false);
                } 
            }
        }
        private int AOD_GCS_Progress_Bar_Setting()
        {
            ///Set ProgressBar
            int max_gray = Convert.ToInt32(AOD_GCS_max_gray.Text);
            int min_gray = Convert.ToInt32(AOD_GCS_min_gray.Text);
            int gray_step = Convert.ToInt32(AOD_GCS_step.Text);

            int number_of_measure = (max_gray - min_gray) / gray_step + 1;
            double number_of_measure_remain = (max_gray - min_gray) % gray_step;
            if (number_of_measure_remain >= 0.5) number_of_measure++;

            progressBar_AOD_GCS.Value = 0;
            progressBar_AOD_GCS.Step = 1;
            progressBar_AOD_GCS.Minimum = 0;
            progressBar_AOD_GCS.Maximum = AOD_GCS_check_count * number_of_measure;

            return progressBar_AOD_GCS.Maximum;
        }
        private void AOD_GCS_header_update(int number, string DBV)
        {
            for (int ch = 0; ch < 10 & GCS_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add("AOD GCS", number.ToString() + ")DBV", DBV, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void AOD_GCS_row_update(int gray)
        {
            for (int ch = 0; ch < 10 & AOD_GCS_measure; ch++)
            {
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add(gray.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void AOD_GCS_measurement_data_Save(int number, int measure_num, string DBV, int gray)
        {
            Application.DoEvents();
            
            for (int ch = 0; ch < 10; ch++)
            {
                for (int set = 0; set < 6; set++) // Set & 시료별 data write
                {
                    if ((set==0) && check_PNC_CH[ch])
                    {
                        index_list[data_index] = gray;

                        data_list[0][ch][data_index].double_X = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[1].Value);
                        data_list[0][ch][data_index].double_Y = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[2].Value);
                        data_list[0][ch][data_index].double_Lv = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3].Value);
                    }
                }
            }

            data_index++;
        }

        private void IR_Drop_DeltaE_Measure()
        {
            Make_IR_Drop_DeltaE_PTN_List();
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            double X = 0;
            double Y = 0;
            f1.Intialize_XY(ref X, ref Y);
            bool Full_PTN = false;
            int PTN_delay = Convert.ToInt16(IR_Drop_DeltaE_Delay.Text);

            label_mornitoring.Text = "Open file for saving IR_Drop_DeltaE measurement data";
            ///////////////////// Log 저장을 위한 file setting
            string FileName_load = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\format\\IR_Drop_DeltaE_format.csv";
            string FileName_save = Make_new_folder(Start_Time) + "\\IR_Drop_DeltaE_" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US")) + ".csv";

            StreamReader sr = new StreamReader(FileName_load);
            FileStream stream = new FileStream(FileName_save, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(Regex.Replace(line, ",", "\t", RegexOptions.IgnoreCase));
            }

            sr.Close();
            ///////////////////// Log 저장을 위한 file setting

            PNC_SET_Script_Send((Convert.ToInt32(IR_Drop_DeltaE_Set.Text)-1));  // Set change for IR Drop DeltaE
            string DBV = IR_Drop_DeltaE_DBV.Text.PadLeft(3, '0');//dex to hex (as a string form)
            f1.DBV_Setting(DBV);
            Thread.Sleep(PTN_delay);

            IR_Drop_DeltaE_header_update();
            row_count++;

            progressBar_IR_Drop_DeltaE.PerformStep();

            data_index = 0;
            count_measure = 50;
            index_list = new int[count_measure];
            for (int ch = 0; ch < 10; ch++)
            {
                data_list[0][ch] = new XYLv[count_measure];
            }

            index_max = 48;
            for (int i = 0; i < 50 & IR_Drop_DeltaE_measure; i++)
            {
                int Color_index =  i/2;
                int remain = i - Color_index*2;
                if(remain==1) Full_PTN = false;
                else Full_PTN = true;
                string PTN_Sizs = "";

                switch (Full_PTN)
                {
                    case true:
                        PTN_Sizs = "Full PTN";
                        break;
                    case false:
                        PTN_Sizs = "APL 30% PTN";
                        break;
                }
                label_mornitoring.Text = "IR Drop DeltaE Measure : Color" + (Color_index + 1).ToString() + ") (" + IR_Drop_PTN[Color_index].R.ToString() + "/" + IR_Drop_PTN[Color_index].G.ToString() + "/" + IR_Drop_PTN[Color_index].B.ToString() + ") " + PTN_Sizs;
                
                IR_Drop_DeltaE_row_update(i+1);
                row_count++;

                try
                {
                    System.Windows.Forms.Application.DoEvents();
                    IR_Drop_DeltaE_PTN_update(X, Y, IR_Drop_PTN[Color_index], Full_PTN);
                    Thread.Sleep(PTN_delay);

                    f1.GB_Status_AppendText_Nextline("multi CH measurement IR Drop DeltaE" + (i + 1).ToString() + ") PTN was applied", Color.Blue);
                }
                catch
                {
                    f1.GB_Status_AppendText_Nextline("multi CH measurement IR Drop DeltaE" + (i + 1).ToString() + ") PTN was failed", Color.Red);
                }


                try
                {
                    Set_non_change_Measure();
                    IR_Drop_DeltaE_measurement_data_Save(i);
                    progressBar_IR_Drop_DeltaE.PerformStep();
                    progressBar_Measurement.PerformStep();
                }
                catch
                {
                    stop_flag = true;
                    measure_flahg_change(false);
                }
            }

            try
            {
                save_IR_Drop_DeltaE(IR_Drop_DeltaE_Set.Text,DBV,sw);
            }
            catch
            {
                sw.Close();
                stream.Close();

                stop_flag = true;
                measure_flahg_change(false);
            }

            if (!stop_flag)
            {
                label_mornitoring.Text = "Save the IR_Drop_DeltaE measurement data to file";

                sw.Close();
                stream.Close();
            }
        }
        private void IR_Drop_DeltaE_header_update()
        {
            for (int ch = 0; ch < 10 & IR_Drop_DeltaE_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add("IR", "Drop", "DeltaE", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void IR_Drop_DeltaE_Progress_Bar_Setting()
        {
            ///Set ProgressBar
            progressBar_IR_Drop_DeltaE.Value = 0;
            progressBar_IR_Drop_DeltaE.Step = 1;
            progressBar_IR_Drop_DeltaE.Minimum = 0;
            progressBar_IR_Drop_DeltaE.Maximum = 50;
        }
        private void IR_Drop_DeltaE_row_update(int index)
        {
            for (int ch = 0; ch < 10 & IR_Drop_DeltaE_measure; ch++)
            {
                System.Windows.Forms.Application.DoEvents();
                if (check_PNC_CH[ch])
                {
                    CH_Grid_View[ch].Rows.Add(index.ToString(), "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
        }
        private void IR_Drop_DeltaE_PTN_update(double X, double Y, Color Inner_color, bool Full_PTN = false)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            Color Outer_color = Color.White;
            
            string one_side = Convert.ToInt16(Math.Sqrt(X * Y * 0.3)).ToString();

            if (Full_PTN)
            {
                f1.PTN_update(Inner_color.R, Inner_color.G, Inner_color.B);

                //Just Mornitoring
                f1.GB_Status_AppendText_Nextline("Full PTN : " + Inner_color.R.ToString() + "/" + Inner_color.G.ToString()
                    + "/" + Inner_color.B.ToString(), Color.Black);
            }
            else
            {
                f1.IPC_Quick_Send("image.crosstalk " + one_side + " " + one_side
                    + " " + Outer_color.R.ToString() + " " + Outer_color.G.ToString() + " " + Outer_color.B.ToString()
                    + " " + Inner_color.R.ToString() + " " + Inner_color.G.ToString() + " " + Inner_color.B.ToString());

                //Just Mornitoring
                f1.GB_Status_AppendText_Nextline("Small PTN : " + Inner_color.R.ToString() + "/" + Inner_color.G.ToString()
                    + "/" + Inner_color.B.ToString(), Color.Black);
            }
        }
        public void Make_IR_Drop_DeltaE_PTN_List()
        {
            for(int i=0;i<25;i++)
            {
                switch (i)
                {
                    case 0:
                        IR_Drop_PTN[i] = Color.FromArgb(255, 0, 0);
                        break;
                    case 1:
                        IR_Drop_PTN[i] = Color.FromArgb(0, 255, 0);
                        break;
                    case 2:
                        IR_Drop_PTN[i] = Color.FromArgb(0, 0, 255);
                        break;
                    case 3:
                        IR_Drop_PTN[i] = Color.FromArgb(255, 255, 0);
                        break;
                    case 4:
                        IR_Drop_PTN[i] = Color.FromArgb(0, 255, 255);
                        break;
                    case 5:
                        IR_Drop_PTN[i] = Color.FromArgb(255, 0, 255);
                        break;
                    case 6:
                        IR_Drop_PTN[i] = Color.FromArgb(115, 82, 66);
                        break;
                    case 7:
                        IR_Drop_PTN[i] = Color.FromArgb(194, 150, 130);
                        break;
                    case 8:
                        IR_Drop_PTN[i] = Color.FromArgb(94, 122, 156);
                        break;
                    case 9:
                        IR_Drop_PTN[i] = Color.FromArgb(89, 107, 66);
                        break;
                    case 10:
                        IR_Drop_PTN[i] = Color.FromArgb(130, 128, 176);
                        break;
                    case 11:
                        IR_Drop_PTN[i] = Color.FromArgb(99, 189, 168);
                        break;
                    case 12:
                        IR_Drop_PTN[i] = Color.FromArgb(217, 120, 41);
                        break;
                    case 13:
                        IR_Drop_PTN[i] = Color.FromArgb(74, 92, 163);
                        break;
                    case 14:
                        IR_Drop_PTN[i] = Color.FromArgb(194, 84, 97);
                        break;
                    case 15:
                        IR_Drop_PTN[i] = Color.FromArgb(92, 61, 107);
                        break;
                    case 16:
                        IR_Drop_PTN[i] = Color.FromArgb(158, 186, 64);
                        break;
                    case 17:
                        IR_Drop_PTN[i] = Color.FromArgb(230, 161, 46);
                        break;
                    case 18:
                        IR_Drop_PTN[i] = Color.FromArgb(51, 61, 150);
                        break;
                    case 19:
                        IR_Drop_PTN[i] = Color.FromArgb(71, 148, 71);
                        break;
                    case 20:
                        IR_Drop_PTN[i] = Color.FromArgb(176, 48, 59);
                        break;
                    case 21:
                        IR_Drop_PTN[i] = Color.FromArgb(237, 199, 33);
                        break;
                    case 22:
                        IR_Drop_PTN[i] = Color.FromArgb(186, 84, 145);
                        break;
                    case 23:
                        IR_Drop_PTN[i] = Color.FromArgb(0, 133, 163);
                        break;
                    case 24:
                        IR_Drop_PTN[i] = Color.FromArgb(255, 255, 255);
                        break;
                }
            }
        }
        private void IR_Drop_DeltaE_measurement_data_Save(int index)
        {
            Application.DoEvents();
            
            for (int ch = 0; ch < 10; ch++)
            {
                for (int set = 0; set < 6; set++) // Set & 시료별 data write
                {
                    if ((set == 0) && check_PNC_CH[ch])
                    {
                        data_list[0][ch][data_index].double_X = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[1].Value);
                        data_list[0][ch][data_index].double_Y = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[2].Value);
                        data_list[0][ch][data_index].double_Lv = Convert.ToDouble(CH_Grid_View[ch].Rows[row_count - 1].Cells[3].Value);
                    }
                }
            }

            data_index++;
        }
        
        private bool GetErrorMessage(int errornum)
        {
            string errormessage = "";
            if (errornum != 0)
            {
                //Get Error message from Error number
                err = GlobalFunctions.CASDK2_GetLocalizedErrorMsgFromErrorCode(0, errornum, ref errormessage);
                Console.WriteLine(errormessage);

                this.btn_CA_Connect.ForeColor = System.Drawing.Color.Red;
                this.CA_zero_cal_button.Hide();
                this.CA_Test_button.Hide();
                return false;
            }
            return true;
        }
        private void DisplayError(Exception er)
        {
            String msg;
            msg = "Error from" + er.Source + "\r\n";
            msg += er.Message + "\r\n";
            //msg += "HR:" + (er.HResult - vbObjectError).ToString();
            System.Windows.Forms.MessageBox.Show(msg);
        }

        private void Clear_all()
        {
            meausre_count = 1;
            row_count = 1;

            for (int ch = 0; ch < 10; ch++)
            {
                CH_Grid_View[ch].Rows.Clear();
                CH_Grid_View[ch].Rows.Add(first_line);
            }

            progressBar_GCS.Value = 0;
            progressBar_BCS.Value = 0;
            progressBar_Gamma_Crush.Value = 0;
            progressBar_AOD_GCS.Value = 0;
            progressBar_IR_Drop_DeltaE.Value = 0;
            progressBar_Measurement.Value = 0;
        }
        private void button_clear_Click(object sender, EventArgs e)
        {
            Clear_all();
        }

        private void button_Setting_Load_Click(object sender, EventArgs e)
        {
            Button_Click_Enable(false);

            Application.DoEvents();

            //------Set Setting Here------
            //FileStream myFileStream = new FileStream(Directory.GetCurrentDirectory() + "\\prefs.xml", FileMode.Open);//Used For Loading Setting

            FileStream myFileStream; 
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()+"\\Optic_Measurement(10ch)";
            openFileDialog1.Filter = "Default Extension (*.xml)|*.xml";
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.AddExtension = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myFileStream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                Multi_CH_Measurement_Preferences up = (Multi_CH_Measurement_Preferences)mySerializer.Deserialize(myFileStream);
                
                //---PNC Setting---
                check_PNC_CH1.Checked = up.check_PNC_CH[0];
                check_PNC_CH2.Checked = up.check_PNC_CH[1];
                check_PNC_CH3.Checked = up.check_PNC_CH[2];
                check_PNC_CH4.Checked = up.check_PNC_CH[3];
                check_PNC_CH5.Checked = up.check_PNC_CH[4];
                check_PNC_CH6.Checked = up.check_PNC_CH[5];
                check_PNC_CH7.Checked = up.check_PNC_CH[6];
                check_PNC_CH8.Checked = up.check_PNC_CH[7];
                check_PNC_CH9.Checked = up.check_PNC_CH[8];
                check_PNC_CH10.Checked = up.check_PNC_CH[9];

                CA_NO_CH1.Text = up.CA_NO_CH[0];
                CA_NO_CH2.Text = up.CA_NO_CH[1];
                CA_NO_CH3.Text = up.CA_NO_CH[2];
                CA_NO_CH4.Text = up.CA_NO_CH[3];
                CA_NO_CH5.Text = up.CA_NO_CH[4];
                CA_NO_CH6.Text = up.CA_NO_CH[5];
                CA_NO_CH7.Text = up.CA_NO_CH[6];
                CA_NO_CH8.Text = up.CA_NO_CH[7];
                CA_NO_CH9.Text = up.CA_NO_CH[8];
                CA_NO_CH10.Text = up.CA_NO_CH[9];

                Set_Change_Delay.Text = up.Set_Change_Delay;

                check_SET1.Checked = up.check_SET[0];
                check_SET2.Checked = up.check_SET[1];
                check_SET3.Checked = up.check_SET[2];
                check_SET4.Checked = up.check_SET[3];
                check_SET5.Checked = up.check_SET[4];
                check_SET6.Checked = up.check_SET[5];

                SEQ_SET1.Text = up.SEQ_SET[0];
                SEQ_SET2.Text = up.SEQ_SET[1];
                SEQ_SET3.Text = up.SEQ_SET[2];
                SEQ_SET4.Text = up.SEQ_SET[3];
                SEQ_SET5.Text = up.SEQ_SET[4];
                SEQ_SET6.Text = up.SEQ_SET[5];

                textBox_Script_SET1.Text = up.textBox_Script_SET[0];
                textBox_Script_SET2.Text = up.textBox_Script_SET[1];
                textBox_Script_SET3.Text = up.textBox_Script_SET[2];
                textBox_Script_SET4.Text = up.textBox_Script_SET[3];
                textBox_Script_SET5.Text = up.textBox_Script_SET[4];
                textBox_Script_SET6.Text = up.textBox_Script_SET[5];

                //---GCS Setting---
                check_GCS_DBV1.Checked = up.check_GCS_DBV[0];
                check_GCS_DBV2.Checked = up.check_GCS_DBV[1];
                check_GCS_DBV3.Checked = up.check_GCS_DBV[2];
                check_GCS_DBV4.Checked = up.check_GCS_DBV[3];
                check_GCS_DBV5.Checked = up.check_GCS_DBV[4];
                check_GCS_DBV6.Checked = up.check_GCS_DBV[5];
                check_GCS_DBV7.Checked = up.check_GCS_DBV[6];
                check_GCS_DBV8.Checked = up.check_GCS_DBV[7];
                check_GCS_DBV9.Checked = up.check_GCS_DBV[8];
                check_GCS_DBV10.Checked = up.check_GCS_DBV[9];
                check_GCS_DBV11.Checked = up.check_GCS_DBV[10];
                check_GCS_DBV12.Checked = up.check_GCS_DBV[11];
                check_GCS_DBV13.Checked = up.check_GCS_DBV[12];
                check_GCS_DBV14.Checked = up.check_GCS_DBV[13];
                check_GCS_DBV15.Checked = up.check_GCS_DBV[14];
                check_GCS_DBV16.Checked = up.check_GCS_DBV[15];
                check_GCS_DBV17.Checked = up.check_GCS_DBV[16];
                check_GCS_DBV18.Checked = up.check_GCS_DBV[17];
                check_GCS_DBV19.Checked = up.check_GCS_DBV[18];
                check_GCS_DBV20.Checked = up.check_GCS_DBV[19];

                GCS_DBV1.Text = up.GCS_DBV[0];
                GCS_DBV2.Text = up.GCS_DBV[1];
                GCS_DBV3.Text = up.GCS_DBV[2];
                GCS_DBV4.Text = up.GCS_DBV[3];
                GCS_DBV5.Text = up.GCS_DBV[4];
                GCS_DBV6.Text = up.GCS_DBV[5];
                GCS_DBV7.Text = up.GCS_DBV[6];
                GCS_DBV8.Text = up.GCS_DBV[7];
                GCS_DBV9.Text = up.GCS_DBV[8];
                GCS_DBV10.Text = up.GCS_DBV[9];
                GCS_DBV11.Text = up.GCS_DBV[10];
                GCS_DBV12.Text = up.GCS_DBV[11];
                GCS_DBV13.Text = up.GCS_DBV[12];
                GCS_DBV14.Text = up.GCS_DBV[13];
                GCS_DBV15.Text = up.GCS_DBV[14];
                GCS_DBV16.Text = up.GCS_DBV[15];
                GCS_DBV17.Text = up.GCS_DBV[16];
                GCS_DBV18.Text = up.GCS_DBV[17];
                GCS_DBV19.Text = up.GCS_DBV[18];
                GCS_DBV20.Text = up.GCS_DBV[19];

                GCS_Delay.Text = up.GCS_Delay;
                GCS_min_gray.Text = up.GCS_min_gray;
                GCS_max_gray.Text = up.GCS_max_gray;
                GCS_step.Text = up.GCS_step;

                GCS_Min_to_Max.Checked = up.GCS_Min_to_Max;
                GCS_Max_to_Min.Checked = up.GCS_Max_to_Min;

                //---BCS Setting---
                check_BCS_Gray1.Checked = up.check_BCS_Gray[0];
                check_BCS_Gray2.Checked = up.check_BCS_Gray[1];
                check_BCS_Gray3.Checked = up.check_BCS_Gray[2];
                check_BCS_Gray4.Checked = up.check_BCS_Gray[3];
                check_BCS_Gray5.Checked = up.check_BCS_Gray[4];
                check_BCS_Gray6.Checked = up.check_BCS_Gray[5];
                check_BCS_Gray7.Checked = up.check_BCS_Gray[6];
                check_BCS_Gray8.Checked = up.check_BCS_Gray[7];
                check_BCS_Gray9.Checked = up.check_BCS_Gray[8];
                check_BCS_Gray10.Checked = up.check_BCS_Gray[9];
                check_BCS_Gray11.Checked = up.check_BCS_Gray[10];
                check_BCS_Gray12.Checked = up.check_BCS_Gray[11];
                check_BCS_Gray13.Checked = up.check_BCS_Gray[12];
                check_BCS_Gray14.Checked = up.check_BCS_Gray[13];
                check_BCS_Gray15.Checked = up.check_BCS_Gray[14];
                check_BCS_Gray16.Checked = up.check_BCS_Gray[15];
                check_BCS_Gray17.Checked = up.check_BCS_Gray[16];
                check_BCS_Gray18.Checked = up.check_BCS_Gray[17];
                check_BCS_Gray19.Checked = up.check_BCS_Gray[18];
                check_BCS_Gray20.Checked = up.check_BCS_Gray[19];

                BCS_Gray1.Text = up.BCS_Gray[0];
                BCS_Gray2.Text = up.BCS_Gray[1];
                BCS_Gray3.Text = up.BCS_Gray[2];
                BCS_Gray4.Text = up.BCS_Gray[3];
                BCS_Gray5.Text = up.BCS_Gray[4];
                BCS_Gray6.Text = up.BCS_Gray[5];
                BCS_Gray7.Text = up.BCS_Gray[6];
                BCS_Gray8.Text = up.BCS_Gray[7];
                BCS_Gray9.Text = up.BCS_Gray[8];
                BCS_Gray10.Text = up.BCS_Gray[9];
                BCS_Gray11.Text = up.BCS_Gray[10];
                BCS_Gray12.Text = up.BCS_Gray[11];
                BCS_Gray13.Text = up.BCS_Gray[12];
                BCS_Gray14.Text = up.BCS_Gray[13];
                BCS_Gray15.Text = up.BCS_Gray[14];
                BCS_Gray16.Text = up.BCS_Gray[15];
                BCS_Gray17.Text = up.BCS_Gray[16];
                BCS_Gray18.Text = up.BCS_Gray[17];
                BCS_Gray19.Text = up.BCS_Gray[18];
                BCS_Gray20.Text = up.BCS_Gray[19];

                BCS_Range1.Checked = up.BCS_Range1;
                BCS_Range2.Checked = up.BCS_Range2;
                BCS_Range3.Checked = up.BCS_Range3;

                BCS_Delay.Text = up.BCS_Delay;
                BCS_Range1_min_DBV.Text = up.BCS_Range1_min_DBV;
                BCS_Range1_max_DBV.Text = up.BCS_Range1_max_DBV;
                BCS_Range1_step.Text = up.BCS_Range1_step;
                BCS_Range2_min_DBV.Text = up.BCS_Range2_min_DBV;
                BCS_Range2_max_DBV.Text = up.BCS_Range2_max_DBV;
                BCS_Range2_step.Text = up.BCS_Range2_step;
                BCS_Range3_min_DBV.Text = up.BCS_Range3_min_DBV;
                BCS_Range3_max_DBV.Text = up.BCS_Range3_max_DBV;
                BCS_Range3_step.Text = up.BCS_Range3_step;

                BCS_Min_to_Max.Checked = up.BCS_Min_to_Max;
                BCS_Max_to_Min.Checked = up.BCS_Max_to_Min;

                //---Gamma Crush Setting---
                check_Gamma_Crush1.Checked = up.check_Gamma_Crush[0];
                check_Gamma_Crush2.Checked = up.check_Gamma_Crush[1];
                check_Gamma_Crush3.Checked = up.check_Gamma_Crush[2];
                check_Gamma_Crush4.Checked = up.check_Gamma_Crush[3];
                check_Gamma_Crush5.Checked = up.check_Gamma_Crush[4];
                check_Gamma_Crush6.Checked = up.check_Gamma_Crush[5];
                check_Gamma_Crush7.Checked = up.check_Gamma_Crush[6];
                check_Gamma_Crush8.Checked = up.check_Gamma_Crush[7];
                check_Gamma_Crush9.Checked = up.check_Gamma_Crush[8];
                check_Gamma_Crush10.Checked = up.check_Gamma_Crush[9];

                Gamma_Crush_DBV1.Text = up.Gamma_Crush_DBV[0];
                Gamma_Crush_DBV2.Text = up.Gamma_Crush_DBV[1];
                Gamma_Crush_DBV3.Text = up.Gamma_Crush_DBV[2];
                Gamma_Crush_DBV4.Text = up.Gamma_Crush_DBV[3];
                Gamma_Crush_DBV5.Text = up.Gamma_Crush_DBV[4];
                Gamma_Crush_DBV6.Text = up.Gamma_Crush_DBV[5];
                Gamma_Crush_DBV7.Text = up.Gamma_Crush_DBV[6];
                Gamma_Crush_DBV8.Text = up.Gamma_Crush_DBV[7];
                Gamma_Crush_DBV9.Text = up.Gamma_Crush_DBV[8];
                Gamma_Crush_DBV10.Text = up.Gamma_Crush_DBV[9];

                Gamma_Crush_Gray1.Text = up.Gamma_Crush_Gray[0];
                Gamma_Crush_Gray2.Text = up.Gamma_Crush_Gray[1];
                Gamma_Crush_Gray3.Text = up.Gamma_Crush_Gray[2];
                Gamma_Crush_Gray4.Text = up.Gamma_Crush_Gray[3];
                Gamma_Crush_Gray5.Text = up.Gamma_Crush_Gray[4];
                Gamma_Crush_Gray6.Text = up.Gamma_Crush_Gray[5];
                Gamma_Crush_Gray7.Text = up.Gamma_Crush_Gray[6];
                Gamma_Crush_Gray8.Text = up.Gamma_Crush_Gray[7];
                Gamma_Crush_Gray9.Text = up.Gamma_Crush_Gray[8];
                Gamma_Crush_Gray10.Text = up.Gamma_Crush_Gray[9];

                Gamma_Crush_PTN_Delay.Text = up.Gamma_Crush_PTN_Delay;
                Gamma_Crush_DBV_Delay.Text = up.Gamma_Crush_DBV_Delay;

                checkBox_Gamma_Crush_W.Checked = up.checkBox_Gamma_Crush_W;
                checkBox_Gamma_Crush_R.Checked = up.checkBox_Gamma_Crush_R;
                checkBox_Gamma_Crush_G.Checked = up.checkBox_Gamma_Crush_G;
                checkBox_Gamma_Crush_B.Checked = up.checkBox_Gamma_Crush_B;

                //---AOD GCS Setting---
                check_AOD_GCS_DBV1.Checked = up.check_AOD_GCS_DBV[0];
                check_AOD_GCS_DBV2.Checked = up.check_AOD_GCS_DBV[1];
                check_AOD_GCS_DBV3.Checked = up.check_AOD_GCS_DBV[2];

                AOD_GCS_DBV1.Text = up.AOD_GCS_DBV[0];
                AOD_GCS_DBV2.Text = up.AOD_GCS_DBV[1];
                AOD_GCS_DBV3.Text = up.AOD_GCS_DBV[2];

                AOD_GCS_Delay.Text = up.AOD_GCS_Delay;
                AOD_CODE_Delay.Text = up.AOD_CODE_Delay;               
                AOD_GCS_min_gray.Text = up.AOD_GCS_min_gray;
                AOD_GCS_max_gray.Text = up.AOD_GCS_max_gray;
                AOD_GCS_step.Text = up.AOD_GCS_step;

                AOD_GCS_Min_to_Max.Checked = up.AOD_GCS_Min_to_Max;
                AOD_GCS_Max_to_Min.Checked = up.AOD_GCS_Max_to_Min;

                //---IR Drop DeltaE Setting---
                IR_Drop_DeltaE_DBV.Text = up.IR_Drop_DeltaE_DBV;
                IR_Drop_DeltaE_Delay.Text = up.IR_Drop_DeltaE_Delay;
                IR_Drop_DeltaE_Set.Text = up.IR_Drop_DeltaE_Set;

                //---Measurement Setting---
                textBox_Aging_Time.Text = up.textBox_Aging_Time;
                textBox_Aging_DBV.Text = up.textBox_Aging_DBV;

                check_GCS_Measure.Checked = up.check_GCS_Measure;
                check_BCS_Measure.Checked = up.check_BCS_Measure;
                check_AOD_GCS_Measure.Checked = up.check_AOD_GCS_Measure;
                check_IR_Drop_DeltaE_Measure.Checked = up.check_IR_Drop_DeltaE_Measure;
                check_Gamma_Crush_Measure.Checked = up.check_Gamma_Crush_Measure;

                check_Avg_Measure.Checked = up.check_Avg_Measure;
                textBox_Avg_Lv_Limit.Text = up.textBox_Avg_Lv_Limit;

                //---CA CH Setting---
                textBox_ch_W.Text = up.CA_CH_W;
                textBox_ch_R.Text = up.CA_CH_R;
                textBox_ch_G.Text = up.CA_CH_G;
                textBox_ch_B.Text = up.CA_CH_B;
                checkBox_Color_CH_fix.Checked = up.All_Color_CH_fix;

                button_channel_change_Click_1(button_channel_change, null); // CH setting 불러온걸로 적용

                myFileStream.Close();
                System.Windows.Forms.MessageBox.Show("Setting has been Loaded (File Date : " + up.Saved_Date + ")");
            }
            else
            {
                myFileStream = null;
                System.Windows.Forms.MessageBox.Show("Nothing has been Loaded");
            }
            Button_Click_Enable(true);
        }
        private void button_Setting_Save_Click(object sender, EventArgs e)
        {
            Button_Click_Enable(false);

            Application.DoEvents();

            //------Get Setting Here------
            Multi_CH_Measurement_Preferences up = new Multi_CH_Measurement_Preferences();

            //---Common---
            //Save Current Date
            DateTime localDate = DateTime.Now;
            up.Saved_Date = localDate.ToString(@"yyyy.MM.dd HH:mm:ss", new CultureInfo("en-US"));

            //---PNC Setting---
            up.check_PNC_CH[0] = check_PNC_CH1.Checked;
            up.check_PNC_CH[1] = check_PNC_CH2.Checked;
            up.check_PNC_CH[2] = check_PNC_CH3.Checked;
            up.check_PNC_CH[3] = check_PNC_CH4.Checked;
            up.check_PNC_CH[4] = check_PNC_CH5.Checked;
            up.check_PNC_CH[5] = check_PNC_CH6.Checked;
            up.check_PNC_CH[6] = check_PNC_CH7.Checked;
            up.check_PNC_CH[7] = check_PNC_CH8.Checked;
            up.check_PNC_CH[8] = check_PNC_CH9.Checked;
            up.check_PNC_CH[9] = check_PNC_CH10.Checked;

            up.CA_NO_CH[0] = CA_NO_CH1.Text;
            up.CA_NO_CH[1] = CA_NO_CH2.Text;
            up.CA_NO_CH[2] = CA_NO_CH3.Text;
            up.CA_NO_CH[3] = CA_NO_CH4.Text;
            up.CA_NO_CH[4] = CA_NO_CH5.Text;
            up.CA_NO_CH[5] = CA_NO_CH6.Text;
            up.CA_NO_CH[6] = CA_NO_CH7.Text;
            up.CA_NO_CH[7] = CA_NO_CH8.Text;
            up.CA_NO_CH[8] = CA_NO_CH9.Text;
            up.CA_NO_CH[9] = CA_NO_CH10.Text;

            up.Set_Change_Delay = Set_Change_Delay.Text;

            up.check_SET[0] = check_SET1.Checked;
            up.check_SET[1] = check_SET2.Checked;
            up.check_SET[2] = check_SET3.Checked;
            up.check_SET[3] = check_SET4.Checked;
            up.check_SET[4] = check_SET5.Checked;
            up.check_SET[5] = check_SET6.Checked;

            up.SEQ_SET[0] = SEQ_SET1.Text;
            up.SEQ_SET[1] = SEQ_SET2.Text;
            up.SEQ_SET[2] = SEQ_SET3.Text;
            up.SEQ_SET[3] = SEQ_SET4.Text;
            up.SEQ_SET[4] = SEQ_SET5.Text;
            up.SEQ_SET[5] = SEQ_SET6.Text;

            up.textBox_Script_SET[0] = textBox_Script_SET1.Text;
            up.textBox_Script_SET[1] = textBox_Script_SET2.Text;
            up.textBox_Script_SET[2] = textBox_Script_SET3.Text;
            up.textBox_Script_SET[3] = textBox_Script_SET4.Text;
            up.textBox_Script_SET[4] = textBox_Script_SET5.Text;
            up.textBox_Script_SET[5] = textBox_Script_SET6.Text;

            //---GCS Setting---
            up.check_GCS_DBV[0] = check_GCS_DBV1.Checked;
            up.check_GCS_DBV[1] = check_GCS_DBV2.Checked;
            up.check_GCS_DBV[2] = check_GCS_DBV3.Checked;
            up.check_GCS_DBV[3] = check_GCS_DBV4.Checked;
            up.check_GCS_DBV[4] = check_GCS_DBV5.Checked;
            up.check_GCS_DBV[5] = check_GCS_DBV6.Checked;
            up.check_GCS_DBV[6] = check_GCS_DBV7.Checked;
            up.check_GCS_DBV[7] = check_GCS_DBV8.Checked;
            up.check_GCS_DBV[8] = check_GCS_DBV9.Checked;
            up.check_GCS_DBV[9] = check_GCS_DBV10.Checked;
            up.check_GCS_DBV[10] = check_GCS_DBV11.Checked;
            up.check_GCS_DBV[11] = check_GCS_DBV12.Checked;
            up.check_GCS_DBV[12] = check_GCS_DBV13.Checked;
            up.check_GCS_DBV[13] = check_GCS_DBV14.Checked;
            up.check_GCS_DBV[14] = check_GCS_DBV15.Checked;
            up.check_GCS_DBV[15] = check_GCS_DBV16.Checked;
            up.check_GCS_DBV[16] = check_GCS_DBV17.Checked;
            up.check_GCS_DBV[17] = check_GCS_DBV18.Checked;
            up.check_GCS_DBV[18] = check_GCS_DBV19.Checked;
            up.check_GCS_DBV[19] = check_GCS_DBV20.Checked;

            up.GCS_DBV[0] = GCS_DBV1.Text;
            up.GCS_DBV[1] = GCS_DBV2.Text;
            up.GCS_DBV[2] = GCS_DBV3.Text;
            up.GCS_DBV[3] = GCS_DBV4.Text;
            up.GCS_DBV[4] = GCS_DBV5.Text;
            up.GCS_DBV[5] = GCS_DBV6.Text;
            up.GCS_DBV[6] = GCS_DBV7.Text;
            up.GCS_DBV[7] = GCS_DBV8.Text;
            up.GCS_DBV[8] = GCS_DBV9.Text;
            up.GCS_DBV[9] = GCS_DBV10.Text;
            up.GCS_DBV[10] = GCS_DBV11.Text;
            up.GCS_DBV[11] = GCS_DBV12.Text;
            up.GCS_DBV[12] = GCS_DBV13.Text;
            up.GCS_DBV[13] = GCS_DBV14.Text;
            up.GCS_DBV[14] = GCS_DBV15.Text;
            up.GCS_DBV[15] = GCS_DBV16.Text;
            up.GCS_DBV[16] = GCS_DBV17.Text;
            up.GCS_DBV[17] = GCS_DBV18.Text;
            up.GCS_DBV[18] = GCS_DBV19.Text;
            up.GCS_DBV[19] = GCS_DBV20.Text;

            up.GCS_Delay = GCS_Delay.Text;
            up.GCS_min_gray = GCS_min_gray.Text;
            up.GCS_max_gray = GCS_max_gray.Text;
            up.GCS_step = GCS_step.Text;

            up.GCS_Min_to_Max = GCS_Min_to_Max.Checked;
            up.GCS_Max_to_Min = GCS_Max_to_Min.Checked;

            //---BCS Setting---
            up.check_BCS_Gray[0] = check_BCS_Gray1.Checked;
            up.check_BCS_Gray[1] = check_BCS_Gray2.Checked;
            up.check_BCS_Gray[2] = check_BCS_Gray3.Checked;
            up.check_BCS_Gray[3] = check_BCS_Gray4.Checked;
            up.check_BCS_Gray[4] = check_BCS_Gray5.Checked;
            up.check_BCS_Gray[5] = check_BCS_Gray6.Checked;
            up.check_BCS_Gray[6] = check_BCS_Gray7.Checked;
            up.check_BCS_Gray[7] = check_BCS_Gray8.Checked;
            up.check_BCS_Gray[8] = check_BCS_Gray9.Checked;
            up.check_BCS_Gray[9] = check_BCS_Gray10.Checked;
            up.check_BCS_Gray[10] = check_BCS_Gray11.Checked;
            up.check_BCS_Gray[11] = check_BCS_Gray12.Checked;
            up.check_BCS_Gray[12] = check_BCS_Gray13.Checked;
            up.check_BCS_Gray[13] = check_BCS_Gray14.Checked;
            up.check_BCS_Gray[14] = check_BCS_Gray15.Checked;
            up.check_BCS_Gray[15] = check_BCS_Gray16.Checked;
            up.check_BCS_Gray[16] = check_BCS_Gray17.Checked;
            up.check_BCS_Gray[17] = check_BCS_Gray18.Checked;
            up.check_BCS_Gray[18] = check_BCS_Gray19.Checked;
            up.check_BCS_Gray[19] = check_BCS_Gray20.Checked;

            up.BCS_Gray[0] = BCS_Gray1.Text;
            up.BCS_Gray[1] = BCS_Gray2.Text;
            up.BCS_Gray[2] = BCS_Gray3.Text;
            up.BCS_Gray[3] = BCS_Gray4.Text;
            up.BCS_Gray[4] = BCS_Gray5.Text;
            up.BCS_Gray[5] = BCS_Gray6.Text;
            up.BCS_Gray[6] = BCS_Gray7.Text;
            up.BCS_Gray[7] = BCS_Gray8.Text;
            up.BCS_Gray[8] = BCS_Gray9.Text;
            up.BCS_Gray[9] = BCS_Gray10.Text;
            up.BCS_Gray[10] = BCS_Gray11.Text;
            up.BCS_Gray[11] = BCS_Gray12.Text;
            up.BCS_Gray[12] = BCS_Gray13.Text;
            up.BCS_Gray[13] = BCS_Gray14.Text;
            up.BCS_Gray[14] = BCS_Gray15.Text;
            up.BCS_Gray[15] = BCS_Gray16.Text;
            up.BCS_Gray[16] = BCS_Gray17.Text;
            up.BCS_Gray[17] = BCS_Gray18.Text;
            up.BCS_Gray[18] = BCS_Gray19.Text;
            up.BCS_Gray[19] = BCS_Gray20.Text;

            up.BCS_Range1 = BCS_Range1.Checked;
            up.BCS_Range2 = BCS_Range2.Checked;
            up.BCS_Range3 = BCS_Range3.Checked;

            up.BCS_Delay = BCS_Delay.Text;
            up.BCS_Range1_min_DBV = BCS_Range1_min_DBV.Text;
            up.BCS_Range1_max_DBV = BCS_Range1_max_DBV.Text;
            up.BCS_Range1_step = BCS_Range1_step.Text;
            up.BCS_Range2_min_DBV = BCS_Range2_min_DBV.Text;
            up.BCS_Range2_max_DBV = BCS_Range2_max_DBV.Text;
            up.BCS_Range2_step = BCS_Range2_step.Text;
            up.BCS_Range3_min_DBV = BCS_Range3_min_DBV.Text;
            up.BCS_Range3_max_DBV = BCS_Range3_max_DBV.Text;
            up.BCS_Range3_step = BCS_Range3_step.Text;

            up.BCS_Min_to_Max = BCS_Min_to_Max.Checked;
            up.BCS_Max_to_Min = BCS_Max_to_Min.Checked;

            //---Gamma Crush Setting---
            up.check_Gamma_Crush[0] = check_Gamma_Crush1.Checked;
            up.check_Gamma_Crush[1] = check_Gamma_Crush2.Checked;
            up.check_Gamma_Crush[2] = check_Gamma_Crush3.Checked;
            up.check_Gamma_Crush[3] = check_Gamma_Crush4.Checked;
            up.check_Gamma_Crush[4] = check_Gamma_Crush5.Checked;
            up.check_Gamma_Crush[5] = check_Gamma_Crush6.Checked;
            up.check_Gamma_Crush[6] = check_Gamma_Crush7.Checked;
            up.check_Gamma_Crush[7] = check_Gamma_Crush8.Checked;
            up.check_Gamma_Crush[8] = check_Gamma_Crush9.Checked;
            up.check_Gamma_Crush[9] = check_Gamma_Crush10.Checked;

            up.Gamma_Crush_DBV[0] = Gamma_Crush_DBV1.Text;
            up.Gamma_Crush_DBV[1] = Gamma_Crush_DBV2.Text;
            up.Gamma_Crush_DBV[2] = Gamma_Crush_DBV3.Text;
            up.Gamma_Crush_DBV[3] = Gamma_Crush_DBV4.Text;
            up.Gamma_Crush_DBV[4] = Gamma_Crush_DBV5.Text;
            up.Gamma_Crush_DBV[5] = Gamma_Crush_DBV6.Text;
            up.Gamma_Crush_DBV[6] = Gamma_Crush_DBV7.Text;
            up.Gamma_Crush_DBV[7] = Gamma_Crush_DBV8.Text;
            up.Gamma_Crush_DBV[8] = Gamma_Crush_DBV9.Text;
            up.Gamma_Crush_DBV[9] = Gamma_Crush_DBV10.Text;

            up.Gamma_Crush_Gray[0] = Gamma_Crush_Gray1.Text;
            up.Gamma_Crush_Gray[1] = Gamma_Crush_Gray2.Text;
            up.Gamma_Crush_Gray[2] = Gamma_Crush_Gray3.Text;
            up.Gamma_Crush_Gray[3] = Gamma_Crush_Gray4.Text;
            up.Gamma_Crush_Gray[4] = Gamma_Crush_Gray5.Text;
            up.Gamma_Crush_Gray[5] = Gamma_Crush_Gray6.Text;
            up.Gamma_Crush_Gray[6] = Gamma_Crush_Gray7.Text;
            up.Gamma_Crush_Gray[7] = Gamma_Crush_Gray8.Text;
            up.Gamma_Crush_Gray[8] = Gamma_Crush_Gray9.Text;
            up.Gamma_Crush_Gray[9] = Gamma_Crush_Gray10.Text;

            up.Gamma_Crush_PTN_Delay = Gamma_Crush_PTN_Delay.Text;
            up.Gamma_Crush_DBV_Delay = Gamma_Crush_DBV_Delay.Text;

            up.checkBox_Gamma_Crush_W = checkBox_Gamma_Crush_W.Checked;
            up.checkBox_Gamma_Crush_R = checkBox_Gamma_Crush_R.Checked;
            up.checkBox_Gamma_Crush_G = checkBox_Gamma_Crush_G.Checked;
            up.checkBox_Gamma_Crush_B = checkBox_Gamma_Crush_B.Checked;

            //---AOD GCS Setting---
            up.check_AOD_GCS_DBV[0] = check_AOD_GCS_DBV1.Checked;
            up.check_AOD_GCS_DBV[1] = check_AOD_GCS_DBV2.Checked;
            up.check_AOD_GCS_DBV[2] = check_AOD_GCS_DBV3.Checked;

            up.AOD_GCS_DBV[0] = AOD_GCS_DBV1.Text;
            up.AOD_GCS_DBV[1] = AOD_GCS_DBV2.Text;
            up.AOD_GCS_DBV[2] = AOD_GCS_DBV3.Text;

            up.AOD_GCS_Delay = AOD_GCS_Delay.Text;
            up.AOD_CODE_Delay = AOD_CODE_Delay.Text;
            up.AOD_GCS_min_gray = AOD_GCS_min_gray.Text;
            up.AOD_GCS_max_gray = AOD_GCS_max_gray.Text;
            up.AOD_GCS_step = AOD_GCS_step.Text;

            up.AOD_GCS_Min_to_Max = AOD_GCS_Min_to_Max.Checked;
            up.AOD_GCS_Max_to_Min = AOD_GCS_Max_to_Min.Checked;

            //---IR Drop DeltaE Setting---
            up.IR_Drop_DeltaE_DBV = IR_Drop_DeltaE_DBV.Text;
            up.IR_Drop_DeltaE_Delay = IR_Drop_DeltaE_Delay.Text;
            up.IR_Drop_DeltaE_Set = IR_Drop_DeltaE_Set.Text;

            //---Measurement Setting---
            up.textBox_Aging_Time = textBox_Aging_Time.Text;
            up.textBox_Aging_DBV = textBox_Aging_DBV.Text;

            up.check_GCS_Measure = check_GCS_Measure.Checked;
            up.check_BCS_Measure = check_BCS_Measure.Checked;
            up.check_AOD_GCS_Measure = check_AOD_GCS_Measure.Checked;
            up.check_IR_Drop_DeltaE_Measure = check_IR_Drop_DeltaE_Measure.Checked;
            up.check_Gamma_Crush_Measure = check_Gamma_Crush_Measure.Checked;

            up.check_Avg_Measure = check_Avg_Measure.Checked;
            up.textBox_Avg_Lv_Limit = textBox_Avg_Lv_Limit.Text;

            //---CA CH Setting---
            up.CA_CH_W = textBox_ch_W.Text;
            up.CA_CH_R = textBox_ch_R.Text;
            up.CA_CH_G = textBox_ch_G.Text;
            up.CA_CH_B = textBox_ch_B.Text;
            up.All_Color_CH_fix = checkBox_Color_CH_fix.Checked;

            //---------------------------------------------------------------------

            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Optic_Measurement(10ch)";
            saveFileDialog1.Filter = "Default Extension (*.xml)|*.xml";
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter myWriter = new StreamWriter(saveFileDialog1.FileName);
                mySerializer.Serialize(myWriter, up);
                myWriter.Close();
                System.Windows.Forms.MessageBox.Show("Setting has been saved (File Date : " + up.Saved_Date + ")");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nothing has benn Saved");
            }
            Button_Click_Enable(true);
        }
        private void Button_Click_Enable(bool Able)
        {
            if (Able)
            {
                btn_CA_Connect.Show();
                button_channel_change.Show();
                CA_zero_cal_button.Show();
                CA_Test_button.Show();
                button_clear.Show();
                button_Setting_Load.Show();
                button_Setting_Save.Show();
                button_Measure.Show();

                button_PNC_CH_all_select.Show();
                button_PNC_CH_all_deselect.Show();
                button_GCS_DBV_all_select.Show();
                button_GCS_DBV_all_deselect.Show();
                button_BCS_Gray_all_select.Show();
                button_BCS_Gray_all_deselect.Show();
                button_Stop.Show();
            }
            else
            {
                btn_CA_Connect.Hide();
                button_channel_change.Hide();
                CA_zero_cal_button.Hide();
                CA_Test_button.Hide();
                button_clear.Hide();
                button_Setting_Load.Hide();
                button_Setting_Save.Hide();
                button_Measure.Hide();

                button_PNC_CH_all_select.Hide();
                button_PNC_CH_all_deselect.Hide();
                button_GCS_DBV_all_select.Hide();
                button_GCS_DBV_all_deselect.Hide();
                button_BCS_Gray_all_select.Hide();
                button_BCS_Gray_all_deselect.Hide();
                button_Stop.Hide();

                button_Romote_on.Hide();
                button_Romote_off.Hide();
            }
        }

        private void All_Setting_Check()
        {
            PNC_CH_check();
            PNC_SET_check();

            if (check_GCS_Measure.Checked)
            {
                GCS_measure = true;
            }
            if(check_BCS_Measure.Checked)
            {
                BCS_measure = true;
            }
            if(check_AOD_GCS_Measure.Checked)
            {
                AOD_GCS_measure = true;
            }
            if(check_IR_Drop_DeltaE_Measure.Checked)
            {
                IR_Drop_DeltaE_measure = true;
            }
            if (check_Gamma_Crush_Measure.Checked)
            {
                Gamma_Crush_measure = true;
            }
        }
        private void button_Measure_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            control_Check(false);
            Clear_all();
            stop_flag = false;
            All_Setting_Check();
            button_channel_change.PerformClick();

            Start_Time = DateTime.Now;

            Aging();

            Measurement_Progress_Bar_Setting();

            int count = 0;
            if (radioButton_CA410.Checked) count = ca_count;
            else count = probe_count;

            if (PNC_CH_count > count)
            {
                System.Windows.Forms.MessageBox.Show("Number of CA can't be less than number of PNC channel");
                measure_flahg_change(false);
            }

            if (GCS_measure)
            {
                GCS_Measure();
            }

            if(BCS_measure)
            {
                BCS_Measure();
            }

            if (Gamma_Crush_measure)
            {
                Gamma_Crush_Measure();
            }

            if(AOD_GCS_measure)
            {
                AOD_GCS_Measure();
            }

            if(IR_Drop_DeltaE_measure)
            {
                IR_Drop_DeltaE_Measure();
            }

            if (stop_flag)
            {
                System.Windows.Forms.MessageBox.Show("Measurement Stop");
            }
            else
            {
                label_mornitoring.Text = "Measurement Finish!";
                System.Windows.Forms.MessageBox.Show("Measurement Finish!");
            }

            f1.PTN_update(127, 127, 127);
            control_Check(true);
        }
        private void button_Stop_Click(object sender, EventArgs e)
        {
            stop_flag = true;
            measure_flahg_change(false);
        }
        private void Measurement_Progress_Bar_Setting()
        {
            int maximum_value = 0;

            if (GCS_measure)
            {
                GCS_DBV_check();
                maximum_value += GCS_Progress_Bar_Setting();
            }

            if(BCS_measure)
            {
                BCS_Gray_check();
                maximum_value += BCS_Progress_Bar_Setting();
            }

            if(AOD_GCS_measure)
            {
                AOD_GCS_DBV_check();
                maximum_value += AOD_GCS_Progress_Bar_Setting();
            }

            if(IR_Drop_DeltaE_measure)
            {
                IR_Drop_DeltaE_Progress_Bar_Setting();
                maximum_value += 50;
            }

            if (Gamma_Crush_measure)
            {
                Gamma_Crush_Setting_check();
                maximum_value += Gamma_Crush_Progress_Bar_Setting();
            }
            ///Set ProgressBar
            progressBar_Measurement.Value = 0;
            progressBar_Measurement.Step = 1;
            progressBar_Measurement.Minimum = 0;
            progressBar_Measurement.Maximum = maximum_value;
            progressBar_Measurement.PerformStep();
        }
        private void measure_flahg_change(bool flag)
        {
            GCS_measure = flag;
            BCS_measure = flag;
            AOD_GCS_measure = flag;
            IR_Drop_DeltaE_measure = flag;
            Gamma_Crush_measure = flag;
            aging_flag = flag;
        }
        private void Aging()
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            int Sec = Convert.ToInt16(textBox_Aging_Time.Text);
            int Sec_temp = Sec;

            string DBV = textBox_Aging_DBV.Text.PadLeft(3, '0');//dex to hex (as a string form)
            f1.DBV_Setting(DBV);
            f1.PTN_update(255, 255, 255);

            ///Set ProgressBar
            progressBar_Measurement.Value = 0;
            progressBar_Measurement.Step = 1;
            progressBar_Measurement.Minimum = 0;
            progressBar_Measurement.Maximum = Sec;
            progressBar_Measurement.PerformStep();

            aging_flag = true;

            Application.DoEvents();
            label_mornitoring.Text = "Aging-remain time : " + (Sec_temp).ToString();
            while (aging_flag)
            {
                if (!aging_flag) break;
                if (Sec_temp > 0)
                {
                    Thread.Sleep(1000);
                    Sec_temp--;
                    textBox_Aging_Time.Text = (Sec_temp).ToString();
                    label_mornitoring.Text = "Aging-remain time : " + (Sec_temp).ToString();
                    Application.DoEvents();
                    progressBar_Measurement.PerformStep();
                }
                else
                {
                    textBox_Aging_Time.Text = Sec_temp.ToString();
                    Application.DoEvents();
                    progressBar_Measurement.PerformStep();
                    break;
                }
            }

            label_mornitoring.Text = "Aging is finished";
            textBox_Aging_Time.Text = Sec.ToString();
        }
        private void control_Check(bool able)
        {
            check_PNC_CH1.Enabled = able;
            check_PNC_CH2.Enabled = able;
            check_PNC_CH3.Enabled = able;
            check_PNC_CH4.Enabled = able;
            check_PNC_CH5.Enabled = able;
            check_PNC_CH6.Enabled = able;
            check_PNC_CH7.Enabled = able;
            check_PNC_CH8.Enabled = able;
            check_PNC_CH9.Enabled = able;
            check_PNC_CH10.Enabled = able;

            CA_NO_CH1.Enabled = able;
            CA_NO_CH2.Enabled = able;
            CA_NO_CH3.Enabled = able;
            CA_NO_CH4.Enabled = able;
            CA_NO_CH5.Enabled = able;
            CA_NO_CH6.Enabled = able;
            CA_NO_CH7.Enabled = able;
            CA_NO_CH8.Enabled = able;
            CA_NO_CH9.Enabled = able;
            CA_NO_CH10.Enabled = able;

            Set_Change_Delay.Enabled = able;

            check_SET1.Enabled = able;
            check_SET2.Enabled = able;
            check_SET3.Enabled = able;
            check_SET4.Enabled = able;
            check_SET5.Enabled = able;
            check_SET6.Enabled = able;

            SEQ_SET1.Enabled = able;
            SEQ_SET2.Enabled = able;
            SEQ_SET3.Enabled = able;
            SEQ_SET4.Enabled = able;
            SEQ_SET5.Enabled = able;
            SEQ_SET6.Enabled = able;

            textBox_Script_SET1.Enabled = able;
            textBox_Script_SET2.Enabled = able;
            textBox_Script_SET3.Enabled = able;
            textBox_Script_SET4.Enabled = able;
            textBox_Script_SET5.Enabled = able;
            textBox_Script_SET6.Enabled = able;

            //---GCS Setting---
            check_GCS_DBV1.Enabled = able;
            check_GCS_DBV2.Enabled = able;
            check_GCS_DBV3.Enabled = able;
            check_GCS_DBV4.Enabled = able;
            check_GCS_DBV5.Enabled = able;
            check_GCS_DBV6.Enabled = able;
            check_GCS_DBV7.Enabled = able;
            check_GCS_DBV8.Enabled = able;
            check_GCS_DBV9.Enabled = able;
            check_GCS_DBV10.Enabled = able;
            check_GCS_DBV11.Enabled = able;
            check_GCS_DBV12.Enabled = able;
            check_GCS_DBV13.Enabled = able;
            check_GCS_DBV14.Enabled = able;
            check_GCS_DBV15.Enabled = able;
            check_GCS_DBV16.Enabled = able;
            check_GCS_DBV17.Enabled = able;
            check_GCS_DBV18.Enabled = able;
            check_GCS_DBV19.Enabled = able;
            check_GCS_DBV20.Enabled = able;

            GCS_DBV1.Enabled = able;
            GCS_DBV2.Enabled = able;
            GCS_DBV3.Enabled = able;
            GCS_DBV4.Enabled = able;
            GCS_DBV5.Enabled = able;
            GCS_DBV6.Enabled = able;
            GCS_DBV7.Enabled = able;
            GCS_DBV8.Enabled = able;
            GCS_DBV9.Enabled = able;
            GCS_DBV10.Enabled = able;
            GCS_DBV11.Enabled = able;
            GCS_DBV12.Enabled = able;
            GCS_DBV13.Enabled = able;
            GCS_DBV14.Enabled = able;
            GCS_DBV15.Enabled = able;
            GCS_DBV16.Enabled = able;
            GCS_DBV17.Enabled = able;
            GCS_DBV18.Enabled = able;
            GCS_DBV19.Enabled = able;
            GCS_DBV20.Enabled = able;

            GCS_Delay.Enabled = able;
            GCS_min_gray.Enabled = able;
            GCS_max_gray.Enabled = able;
            GCS_step.Enabled = able;

            GCS_Min_to_Max.Enabled = able;
            GCS_Max_to_Min.Enabled = able;

            //---BCS Setting---
            check_BCS_Gray1.Enabled = able;
            check_BCS_Gray2.Enabled = able;
            check_BCS_Gray3.Enabled = able;
            check_BCS_Gray4.Enabled = able;
            check_BCS_Gray5.Enabled = able;
            check_BCS_Gray6.Enabled = able;
            check_BCS_Gray7.Enabled = able;
            check_BCS_Gray8.Enabled = able;
            check_BCS_Gray9.Enabled = able;
            check_BCS_Gray10.Enabled = able;
            check_BCS_Gray11.Enabled = able;
            check_BCS_Gray12.Enabled = able;
            check_BCS_Gray13.Enabled = able;
            check_BCS_Gray14.Enabled = able;
            check_BCS_Gray15.Enabled = able;
            check_BCS_Gray16.Enabled = able;
            check_BCS_Gray17.Enabled = able;
            check_BCS_Gray18.Enabled = able;
            check_BCS_Gray19.Enabled = able;
            check_BCS_Gray20.Enabled = able;

            BCS_Gray1.Enabled = able;
            BCS_Gray2.Enabled = able;
            BCS_Gray3.Enabled = able;
            BCS_Gray4.Enabled = able;
            BCS_Gray5.Enabled = able;
            BCS_Gray6.Enabled = able;
            BCS_Gray7.Enabled = able;
            BCS_Gray8.Enabled = able;
            BCS_Gray9.Enabled = able;
            BCS_Gray10.Enabled = able;
            BCS_Gray11.Enabled = able;
            BCS_Gray12.Enabled = able;
            BCS_Gray13.Enabled = able;
            BCS_Gray14.Enabled = able;
            BCS_Gray15.Enabled = able;
            BCS_Gray16.Enabled = able;
            BCS_Gray17.Enabled = able;
            BCS_Gray18.Enabled = able;
            BCS_Gray19.Enabled = able;
            BCS_Gray20.Enabled = able;

            BCS_Range1.Enabled = able;
            BCS_Range2.Enabled = able;
            BCS_Range3.Enabled = able;

            BCS_Delay.Enabled = able;
            BCS_Range1_min_DBV.Enabled = able;
            BCS_Range1_max_DBV.Enabled = able;
            BCS_Range1_step.Enabled = able;
            BCS_Range2_min_DBV.Enabled = able;
            BCS_Range2_max_DBV.Enabled = able;
            BCS_Range2_step.Enabled = able;
            BCS_Range3_min_DBV.Enabled = able;
            BCS_Range3_max_DBV.Enabled = able;
            BCS_Range3_step.Enabled = able;

            BCS_Min_to_Max.Enabled = able;
            BCS_Max_to_Min.Enabled = able;

            //---Gamma Crush Setting---
            check_Gamma_Crush1.Enabled = able;
            check_Gamma_Crush2.Enabled = able;
            check_Gamma_Crush3.Enabled = able;
            check_Gamma_Crush4.Enabled = able;
            check_Gamma_Crush5.Enabled = able;
            check_Gamma_Crush6.Enabled = able;
            check_Gamma_Crush7.Enabled = able;
            check_Gamma_Crush8.Enabled = able;
            check_Gamma_Crush9.Enabled = able;
            check_Gamma_Crush10.Enabled = able;

            Gamma_Crush_DBV1.Enabled = able;
            Gamma_Crush_DBV2.Enabled = able;
            Gamma_Crush_DBV3.Enabled = able;
            Gamma_Crush_DBV4.Enabled = able;
            Gamma_Crush_DBV5.Enabled = able;
            Gamma_Crush_DBV6.Enabled = able;
            Gamma_Crush_DBV7.Enabled = able;
            Gamma_Crush_DBV8.Enabled = able;
            Gamma_Crush_DBV9.Enabled = able;
            Gamma_Crush_DBV10.Enabled = able;

            Gamma_Crush_Gray1.Enabled = able;
            Gamma_Crush_Gray2.Enabled = able;
            Gamma_Crush_Gray3.Enabled = able;
            Gamma_Crush_Gray4.Enabled = able;
            Gamma_Crush_Gray5.Enabled = able;
            Gamma_Crush_Gray6.Enabled = able;
            Gamma_Crush_Gray7.Enabled = able;
            Gamma_Crush_Gray8.Enabled = able;
            Gamma_Crush_Gray9.Enabled = able;
            Gamma_Crush_Gray10.Enabled = able;

            Gamma_Crush_PTN_Delay.Enabled = able;
            Gamma_Crush_DBV_Delay.Enabled = able;

            checkBox_Gamma_Crush_W.Enabled = able;
            checkBox_Gamma_Crush_R.Enabled = able;
            checkBox_Gamma_Crush_G.Enabled = able;
            checkBox_Gamma_Crush_B.Enabled = able;

            //---AOD GCS Setting---
            check_AOD_GCS_DBV1.Enabled = able;
            check_AOD_GCS_DBV2.Enabled = able;
            check_AOD_GCS_DBV3.Enabled = able;

            AOD_GCS_DBV1.Enabled = able;
            AOD_GCS_DBV2.Enabled = able;
            AOD_GCS_DBV3.Enabled = able;

            AOD_GCS_Delay.Enabled = able;
            AOD_CODE_Delay.Enabled = able;
            AOD_GCS_min_gray.Enabled = able;
            AOD_GCS_max_gray.Enabled = able;
            AOD_GCS_step.Enabled = able;

            AOD_GCS_Min_to_Max.Enabled = able;
            AOD_GCS_Max_to_Min.Enabled = able;

            //---IR Drop DeltaE Setting---
            IR_Drop_DeltaE_DBV.Enabled = able;
            IR_Drop_DeltaE_Delay.Enabled = able;

            //---Measurement Setting---
            textBox_Aging_Time.Enabled = able;
            textBox_Aging_DBV.Enabled = able;

            check_GCS_Measure.Enabled = able;
            check_BCS_Measure.Enabled = able;
            check_AOD_GCS_Measure.Enabled = able;
            check_IR_Drop_DeltaE_Measure.Enabled = able;
            check_Gamma_Crush_Measure.Enabled = able;

            check_Avg_Measure.Enabled = able;
            textBox_Avg_Lv_Limit.Enabled = able;

            if (able)
            {
                btn_CA_Connect.Show();
                button_channel_change.Show();
                CA_zero_cal_button.Show();
                CA_Test_button.Show();
                button_clear.Show();
                button_Setting_Load.Show();
                button_Setting_Save.Show();
                button_Measure.Show();

                button_PNC_CH_all_select.Show();
                button_PNC_CH_all_deselect.Show();
                button_GCS_DBV_all_select.Show();
                button_GCS_DBV_all_deselect.Show();
                button_BCS_Gray_all_select.Show();
                button_BCS_Gray_all_deselect.Show();
            }
            else
            {
                btn_CA_Connect.Hide();
                button_channel_change.Hide();
                CA_zero_cal_button.Hide();
                CA_Test_button.Hide();
                button_clear.Hide();
                button_Setting_Load.Hide();
                button_Setting_Save.Hide();
                button_Measure.Hide();

                button_PNC_CH_all_select.Hide();
                button_PNC_CH_all_deselect.Hide();
                button_GCS_DBV_all_select.Hide();
                button_GCS_DBV_all_deselect.Hide();
                button_BCS_Gray_all_select.Hide();
                button_BCS_Gray_all_deselect.Hide();
            }
        }

        public string Make_new_folder(DateTime Start_Time)
        {
            string sDirPath;

            sDirPath = Directory.GetCurrentDirectory() + "\\Optic_Measurement_Data(10ch)\\" + Start_Time.ToString(@"yyyy_MM_dd_HH_mm", new CultureInfo("en-US"));

            DirectoryInfo di = new DirectoryInfo(sDirPath);

            if (di.Exists == false)
            {
                di.Create();
            }

            return sDirPath;
        }


    }

    public class Multi_CH_Measurement_Preferences
    {
        //Info
        public string Saved_Date;

        public bool[] check_PNC_CH = new bool[10];
        public string[] CA_NO_CH = new string[10];

        public string Set_Change_Delay;

        public bool[] check_SET = new bool[6];
        public string[] SEQ_SET = new string[6];
        public string[] textBox_Script_SET = new string[6];

        public bool[] check_GCS_DBV = new bool[20];
        public string[] GCS_DBV = new string[20];

        public string GCS_Delay;
        public string GCS_min_gray;
        public string GCS_max_gray;
        public string GCS_step;

        public bool GCS_Min_to_Max;
        public bool GCS_Max_to_Min;

        public bool[] check_BCS_Gray = new bool[20];
        public string[] BCS_Gray = new string[20];

        public bool BCS_Range1;
        public bool BCS_Range2;
        public bool BCS_Range3;

        public string BCS_Delay;
        public string BCS_Range1_min_DBV;
        public string BCS_Range1_max_DBV;
        public string BCS_Range1_step;
        public string BCS_Range2_min_DBV;
        public string BCS_Range2_max_DBV;
        public string BCS_Range2_step;
        public string BCS_Range3_min_DBV;
        public string BCS_Range3_max_DBV;
        public string BCS_Range3_step;

        public bool BCS_Min_to_Max;
        public bool BCS_Max_to_Min;

        public bool[] check_Gamma_Crush = new bool[10];
        public string[] Gamma_Crush_DBV = new string[10];
        public string[] Gamma_Crush_Gray = new string[10];

        public string Gamma_Crush_PTN_Delay;
        public string Gamma_Crush_DBV_Delay;

        public bool checkBox_Gamma_Crush_W;
        public bool checkBox_Gamma_Crush_R;
        public bool checkBox_Gamma_Crush_G;
        public bool checkBox_Gamma_Crush_B;

        public bool[] check_AOD_GCS_DBV = new bool[3];
        public string[] AOD_GCS_DBV = new string[3];

        public string AOD_GCS_Delay;
        public string AOD_CODE_Delay;
        public string AOD_GCS_min_gray;
        public string AOD_GCS_max_gray;
        public string AOD_GCS_step;

        public bool AOD_GCS_Min_to_Max;
        public bool AOD_GCS_Max_to_Min;

        public string IR_Drop_DeltaE_DBV;
        public string IR_Drop_DeltaE_Delay;
        public string IR_Drop_DeltaE_Set;

        public string textBox_Aging_Time;
        public string textBox_Aging_DBV;

        public bool check_GCS_Measure;
        public bool check_BCS_Measure;
        public bool check_AOD_GCS_Measure;
        public bool check_IR_Drop_DeltaE_Measure;
        public bool check_Gamma_Crush_Measure;

        public bool check_Avg_Measure;
        public string textBox_Avg_Lv_Limit;

        public string CA_CH_W;
        public string CA_CH_R;
        public string CA_CH_G;
        public string CA_CH_B;
        public bool All_Color_CH_fix;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Xml.Serialization;

//using References
using SectionLib;
using System.IO.MemoryMappedFiles;
using System.IO;

using System.Threading.Tasks;
using System.Globalization;
using Microsoft.VisualBasic;

namespace PNC_Csharp
{
    public partial class Optic_Measurement_Form : Form
    {
        XmlSerializer mySerializer = new XmlSerializer(typeof(UserPreferences));//Used For Saving and Loading Setting

        //E3
        bool[] checkBox_Condition1 = new bool[16];
        bool[] checkBox_Condition2 = new bool[16];
        bool[] checkBox_Condition3 = new bool[16];
        string[] textBox_Condition1 = new string[16];
        string[] textBox_Condition2 = new string[16];
        string[] textBox_Condition3 = new string[16];

        //GCS Difference
        bool[] checkBox_Diff_GCS_DBV = new bool[20];
        string[] textBox_Diff_GCS_DBV = new string[20];

        //BCS Difference
        bool[] checkBox_Diff_BCS_Gray = new bool[11];
        string[] textBox_Diff_BCS_DBV = new string[11];

        //Gamma Crush
        bool[] checkBox_Gamma_Crush = new bool[10];
        string[] textBox_Gamma_Crush_DBV= new string[10];
        string[] textBox_Gamma_Crush_Gray= new string[10];

        private static Optic_Measurement_Form Instance;
        public static Optic_Measurement_Form getInstance()
        {
            if (Instance == null)
                Instance = new Optic_Measurement_Form();

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

        private Optic_Measurement_Form()
        {
            InitializeComponent();
            dataGridView_Inialize(1, "Gray");
            dataGridView_Inialize(2, "Gray");
            dataGridView_Inialize(3, "Gray");
            dataGridView_Inialize(4, "DBV");
            dataGridView_Inialize(5, "DBV");
            dataGridView_Inialize(6, "DBV");
            dataGridView_Inialize(7, "Gray");
            dataGridView_Inialize(8, "Gray");
            dataGridView_Inialize(9, "Gray");
            dataGridView_Inialize(10, "DBV");
            dataGridView_Inialize(11, "DBV");
            dataGridView_Inialize(12, "DBV");
            dataGridView_Inialize(13, "Gray");

            button_Script_Transform.PerformClick();
            button_Script_Transform2.PerformClick();
            button_Script_Transform3.PerformClick();
        }


        private void DBV_Accuracy_Measure_Show_or_Hide()
        {
            if (f1().current_model.Get_Current_Model_Name() == Model_Name.DP150)
                button_DBV_Accuracy_20191217.Show();
            else
                button_DBV_Accuracy_20191217.Hide();
        }

        private void Optic_Measurement_Form_Load(object sender, EventArgs e)
        {
            label_Model_Indicate.ForeColor = f1().current_model.Get_Back_Ground_Color();
            label_Model_DBV_Max_Indicate.ForeColor = f1().current_model.Get_Back_Ground_Color();
            DBV_Accuracy_Measure_Show_or_Hide();
            label_Model_Indicate.Text = "Model:" + f1().current_model.Get_Current_Model_Name().ToString();
            label_Model_DBV_Max_Indicate.Text = "DBV Max:" + f1().current_model.get_DBV_Max().ToString();
            try
            {
                button_Load_Setting.PerformClick();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Loadindg Settings Failed('prefs.xml'needs to be updated)");
                Button_Click_Enable(true);
            }
            DBV_CheckBox_Status_Update(sender, e);
        }
        private void Gamma_Crush_Textbox_CheckBox_Radiobutton_Enable(bool Able)
        {
            checkBox_Gamma_Crush_Conditon_1.Enabled = Able;
            checkBox_Gamma_Crush_Conditon_2.Enabled = Able;
            checkBox_Gamma_Crush_Conditon_3.Enabled = Able;

            checkBox_Gamma_Crush_W.Enabled = Able;
            checkBox_Gamma_Crush_R.Enabled = Able;
            checkBox_Gamma_Crush_G.Enabled = Able;
            checkBox_Gamma_Crush_B.Enabled = Able;
            checkBox_Gamma_Crush_GB.Enabled = Able;
            checkBox_Gamma_Crush_RB.Enabled = Able;
            checkBox_Gamma_Crush_RG.Enabled = Able;

            textBox_Gamma_Crush_DBV1.Enabled = Able;
            textBox_Gamma_Crush_DBV2.Enabled = Able;
            textBox_Gamma_Crush_DBV3.Enabled = Able;
            textBox_Gamma_Crush_DBV4.Enabled = Able;
            textBox_Gamma_Crush_DBV5.Enabled = Able;
            textBox_Gamma_Crush_DBV6.Enabled = Able;
            textBox_Gamma_Crush_DBV7.Enabled = Able;
            textBox_Gamma_Crush_DBV8.Enabled = Able;
            textBox_Gamma_Crush_DBV9.Enabled = Able;
            textBox_Gamma_Crush_DBV10.Enabled = Able;

            textBox_Gamma_Crush_Gray_1.Enabled = Able;
            textBox_Gamma_Crush_Gray_2.Enabled = Able;
            textBox_Gamma_Crush_Gray_3.Enabled = Able;
            textBox_Gamma_Crush_Gray_4.Enabled = Able;
            textBox_Gamma_Crush_Gray_5.Enabled = Able;
            textBox_Gamma_Crush_Gray_6.Enabled = Able;
            textBox_Gamma_Crush_Gray_7.Enabled = Able;
            textBox_Gamma_Crush_Gray_8.Enabled = Able;
            textBox_Gamma_Crush_Gray_9.Enabled = Able;
            textBox_Gamma_Crush_Gray_10.Enabled = Able;

            checkBox_Gamma_Crush_P1.Enabled = Able;
            checkBox_Gamma_Crush_P2.Enabled = Able;
            checkBox_Gamma_Crush_P3.Enabled = Able;
            checkBox_Gamma_Crush_P4.Enabled = Able;
            checkBox_Gamma_Crush_P5.Enabled = Able;
            checkBox_Gamma_Crush_P6.Enabled = Able;
            checkBox_Gamma_Crush_P7.Enabled = Able;
            checkBox_Gamma_Crush_P8.Enabled = Able;
            checkBox_Gamma_Crush_P9.Enabled = Able;
            checkBox_Gamma_Crush_P10.Enabled = Able;
        }

        private void Update_Gamma_Crush_Checkbox_And_DBV_Gray_Textbox()
        {
            //Textbox to String(DBV)
            textBox_Gamma_Crush_DBV[0] = textBox_Gamma_Crush_DBV1.Text;
            textBox_Gamma_Crush_DBV[1] = textBox_Gamma_Crush_DBV2.Text;
            textBox_Gamma_Crush_DBV[2] = textBox_Gamma_Crush_DBV3.Text;
            textBox_Gamma_Crush_DBV[3] = textBox_Gamma_Crush_DBV4.Text;
            textBox_Gamma_Crush_DBV[4] = textBox_Gamma_Crush_DBV5.Text;
            textBox_Gamma_Crush_DBV[5] = textBox_Gamma_Crush_DBV6.Text;
            textBox_Gamma_Crush_DBV[6] = textBox_Gamma_Crush_DBV7.Text;
            textBox_Gamma_Crush_DBV[7] = textBox_Gamma_Crush_DBV8.Text;
            textBox_Gamma_Crush_DBV[8] = textBox_Gamma_Crush_DBV9.Text;
            textBox_Gamma_Crush_DBV[9] = textBox_Gamma_Crush_DBV10.Text;

            //Textbox to String(Gray)
            textBox_Gamma_Crush_Gray[0] = textBox_Gamma_Crush_Gray_1.Text;
            textBox_Gamma_Crush_Gray[1] = textBox_Gamma_Crush_Gray_2.Text;
            textBox_Gamma_Crush_Gray[2] = textBox_Gamma_Crush_Gray_3.Text;
            textBox_Gamma_Crush_Gray[3] = textBox_Gamma_Crush_Gray_4.Text;
            textBox_Gamma_Crush_Gray[4] = textBox_Gamma_Crush_Gray_5.Text;
            textBox_Gamma_Crush_Gray[5] = textBox_Gamma_Crush_Gray_6.Text;
            textBox_Gamma_Crush_Gray[6] = textBox_Gamma_Crush_Gray_7.Text;
            textBox_Gamma_Crush_Gray[7] = textBox_Gamma_Crush_Gray_8.Text;
            textBox_Gamma_Crush_Gray[8] = textBox_Gamma_Crush_Gray_9.Text;
            textBox_Gamma_Crush_Gray[9] = textBox_Gamma_Crush_Gray_10.Text;

            //CheckBox to Bool
            checkBox_Gamma_Crush[0] = checkBox_Gamma_Crush_P1.Checked;
            checkBox_Gamma_Crush[1] = checkBox_Gamma_Crush_P2.Checked;
            checkBox_Gamma_Crush[2] = checkBox_Gamma_Crush_P3.Checked;
            checkBox_Gamma_Crush[3] = checkBox_Gamma_Crush_P4.Checked;
            checkBox_Gamma_Crush[4] = checkBox_Gamma_Crush_P5.Checked;
            checkBox_Gamma_Crush[5] = checkBox_Gamma_Crush_P6.Checked;
            checkBox_Gamma_Crush[6] = checkBox_Gamma_Crush_P7.Checked;
            checkBox_Gamma_Crush[7] = checkBox_Gamma_Crush_P8.Checked;
            checkBox_Gamma_Crush[8] = checkBox_Gamma_Crush_P9.Checked;
            checkBox_Gamma_Crush[9] = checkBox_Gamma_Crush_P10.Checked;
        }






        private void Update_Diff_DBV_Checkbox_And_Textbox()
        {
            //Textbox to String
            textBox_Diff_GCS_DBV[0] = textBox1_Diff.Text;
            textBox_Diff_GCS_DBV[1] = textBox2_Diff.Text;
            textBox_Diff_GCS_DBV[2] = textBox3_Diff.Text;
            textBox_Diff_GCS_DBV[3] = textBox4_Diff.Text;
            textBox_Diff_GCS_DBV[4] = textBox5_Diff.Text;
            textBox_Diff_GCS_DBV[5] = textBox6_Diff.Text;
            textBox_Diff_GCS_DBV[6] = textBox7_Diff.Text;
            textBox_Diff_GCS_DBV[7] = textBox8_Diff.Text;
            textBox_Diff_GCS_DBV[8] = textBox9_Diff.Text;
            textBox_Diff_GCS_DBV[9] = textBox10_Diff.Text;
            textBox_Diff_GCS_DBV[10] = textBox11_Diff.Text;
            textBox_Diff_GCS_DBV[11] = textBox12_Diff.Text;
            textBox_Diff_GCS_DBV[12] = textBox13_Diff.Text;
            textBox_Diff_GCS_DBV[13] = textBox14_Diff.Text;
            textBox_Diff_GCS_DBV[14] = textBox15_Diff.Text;
            textBox_Diff_GCS_DBV[15] = textBox16_Diff.Text;
            textBox_Diff_GCS_DBV[16] = textBox17_Diff.Text;
            textBox_Diff_GCS_DBV[17] = textBox18_Diff.Text;
            textBox_Diff_GCS_DBV[18] = textBox19_Diff.Text;
            textBox_Diff_GCS_DBV[19] = textBox20_Diff.Text;
            

            //CheckBox to Bool
            checkBox_Diff_GCS_DBV[0] = checkBox1_Diff.Checked;
            checkBox_Diff_GCS_DBV[1] = checkBox2_Diff.Checked;
            checkBox_Diff_GCS_DBV[2] = checkBox3_Diff.Checked;
            checkBox_Diff_GCS_DBV[3] = checkBox4_Diff.Checked;
            checkBox_Diff_GCS_DBV[4] = checkBox5_Diff.Checked;
            checkBox_Diff_GCS_DBV[5] = checkBox6_Diff.Checked;
            checkBox_Diff_GCS_DBV[6] = checkBox7_Diff.Checked;
            checkBox_Diff_GCS_DBV[7] = checkBox8_Diff.Checked;
            checkBox_Diff_GCS_DBV[8] = checkBox9_Diff.Checked;
            checkBox_Diff_GCS_DBV[9] = checkBox10_Diff.Checked;
            checkBox_Diff_GCS_DBV[10] = checkBox11_Diff.Checked;
            checkBox_Diff_GCS_DBV[11] = checkBox12_Diff.Checked;
            checkBox_Diff_GCS_DBV[12] = checkBox13_Diff.Checked;
            checkBox_Diff_GCS_DBV[13] = checkBox14_Diff.Checked;
            checkBox_Diff_GCS_DBV[14] = checkBox15_Diff.Checked;
            checkBox_Diff_GCS_DBV[15] = checkBox16_Diff.Checked;
            checkBox_Diff_GCS_DBV[16] = checkBox17_Diff.Checked;
            checkBox_Diff_GCS_DBV[17] = checkBox18_Diff.Checked;
            checkBox_Diff_GCS_DBV[18] = checkBox19_Diff.Checked;
            checkBox_Diff_GCS_DBV[19] = checkBox20_Diff.Checked;
        }


        private void Update_Diff_BCS_Gray_Checkbox_And_Textbox()
        {
            //Textbox to String
            textBox_Diff_BCS_DBV[0] = textBox_BCS_Diff_Gray_P1.Text;
            textBox_Diff_BCS_DBV[1] = textBox_BCS_Diff_Gray_P2.Text;
            textBox_Diff_BCS_DBV[2] = textBox_BCS_Diff_Gray_P3.Text;
            textBox_Diff_BCS_DBV[3] = textBox_BCS_Diff_Gray_P4.Text;
            textBox_Diff_BCS_DBV[4] = textBox_BCS_Diff_Gray_P5.Text;
            textBox_Diff_BCS_DBV[5] = textBox_BCS_Diff_Gray_P6.Text;
            textBox_Diff_BCS_DBV[6] = textBox_BCS_Diff_Gray_P7.Text;
            textBox_Diff_BCS_DBV[7] = textBox_BCS_Diff_Gray_P8.Text;
            textBox_Diff_BCS_DBV[8] = textBox_BCS_Diff_Gray_P9.Text;
            textBox_Diff_BCS_DBV[9] = textBox_BCS_Diff_Gray_P10.Text;
            textBox_Diff_BCS_DBV[10] = textBox_BCS_Diff_Gray_P11.Text;

            //CheckBox to Bool
            checkBox_Diff_BCS_Gray[0] = checkBox1_BCS_Diff_Gray_P1.Checked;
            checkBox_Diff_BCS_Gray[1] = checkBox1_BCS_Diff_Gray_P2.Checked;
            checkBox_Diff_BCS_Gray[2] = checkBox1_BCS_Diff_Gray_P3.Checked;
            checkBox_Diff_BCS_Gray[3] = checkBox1_BCS_Diff_Gray_P4.Checked;
            checkBox_Diff_BCS_Gray[4] = checkBox1_BCS_Diff_Gray_P5.Checked;
            checkBox_Diff_BCS_Gray[5] = checkBox1_BCS_Diff_Gray_P6.Checked;
            checkBox_Diff_BCS_Gray[6] = checkBox1_BCS_Diff_Gray_P7.Checked;
            checkBox_Diff_BCS_Gray[7] = checkBox1_BCS_Diff_Gray_P8.Checked;
            checkBox_Diff_BCS_Gray[8] = checkBox1_BCS_Diff_Gray_P9.Checked;
            checkBox_Diff_BCS_Gray[9] = checkBox1_BCS_Diff_Gray_P10.Checked;
            checkBox_Diff_BCS_Gray[10] = checkBox1_BCS_Diff_Gray_P11.Checked;
        }

        private void Update_E3_DBV_Checkbox_And_Textbox()
        {
            //---Condition---
            //Textbox to String
            textBox_Condition1[0] = textBox1_1.Text;
            textBox_Condition1[1] = textBox2_1.Text;
            textBox_Condition1[2] = textBox3_1.Text;
            textBox_Condition1[3] = textBox4_1.Text;
            textBox_Condition1[4] = textBox5_1.Text;
            textBox_Condition1[5] = textBox6_1.Text;
            textBox_Condition1[6] = textBox7_1.Text;
            textBox_Condition1[7] = textBox8_1.Text;
            textBox_Condition1[8] = textBox9_1.Text;
            textBox_Condition1[9] = textBox10_1.Text;
            textBox_Condition1[10] = textBox11_1.Text;
            textBox_Condition1[11] = textBox12_1.Text;
            textBox_Condition1[12] = textBox13_1.Text;
            textBox_Condition1[13] = textBox14_1.Text;
            textBox_Condition1[14] = textBox15_1.Text;
            textBox_Condition1[15] = textBox16_1.Text;
            //CheckBox to Bool
            checkBox_Condition1[0] = checkBox1_1.Checked;
            checkBox_Condition1[1] = checkBox2_1.Checked;
            checkBox_Condition1[2] = checkBox3_1.Checked;
            checkBox_Condition1[3] = checkBox4_1.Checked;
            checkBox_Condition1[4] = checkBox5_1.Checked;
            checkBox_Condition1[5] = checkBox6_1.Checked;
            checkBox_Condition1[6] = checkBox7_1.Checked;
            checkBox_Condition1[7] = checkBox8_1.Checked;
            checkBox_Condition1[8] = checkBox9_1.Checked;
            checkBox_Condition1[9] = checkBox10_1.Checked;
            checkBox_Condition1[10] = checkBox11_1.Checked;
            checkBox_Condition1[11] = checkBox12_1.Checked;
            checkBox_Condition1[12] = checkBox13_1.Checked;
            checkBox_Condition1[13] = checkBox14_1.Checked;
            checkBox_Condition1[14] = checkBox15_1.Checked;
            checkBox_Condition1[15] = checkBox16_1.Checked;

            //---Condition2---
            //Textbox to String
            textBox_Condition2[0] = textBox1_2.Text;
            textBox_Condition2[1] = textBox2_2.Text;
            textBox_Condition2[2] = textBox3_2.Text;
            textBox_Condition2[3] = textBox4_2.Text;
            textBox_Condition2[4] = textBox5_2.Text;
            textBox_Condition2[5] = textBox6_2.Text;
            textBox_Condition2[6] = textBox7_2.Text;
            textBox_Condition2[7] = textBox8_2.Text;
            textBox_Condition2[8] = textBox9_2.Text;
            textBox_Condition2[9] = textBox10_2.Text;
            textBox_Condition2[10] = textBox11_2.Text;
            textBox_Condition2[11] = textBox12_2.Text;
            textBox_Condition2[12] = textBox13_2.Text;
            textBox_Condition2[13] = textBox14_2.Text;
            textBox_Condition2[14] = textBox15_2.Text;
            textBox_Condition2[15] = textBox16_2.Text;
            //CheckBox to Bool
            checkBox_Condition2[0] = checkBox1_2.Checked;
            checkBox_Condition2[1] = checkBox2_2.Checked;
            checkBox_Condition2[2] = checkBox3_2.Checked;
            checkBox_Condition2[3] = checkBox4_2.Checked;
            checkBox_Condition2[4] = checkBox5_2.Checked;
            checkBox_Condition2[5] = checkBox6_2.Checked;
            checkBox_Condition2[6] = checkBox7_2.Checked;
            checkBox_Condition2[7] = checkBox8_2.Checked;
            checkBox_Condition2[8] = checkBox9_2.Checked;
            checkBox_Condition2[9] = checkBox10_2.Checked;
            checkBox_Condition2[10] = checkBox11_2.Checked;
            checkBox_Condition2[11] = checkBox12_2.Checked;
            checkBox_Condition2[12] = checkBox13_2.Checked;
            checkBox_Condition2[13] = checkBox14_2.Checked;
            checkBox_Condition2[14] = checkBox15_2.Checked;
            checkBox_Condition2[15] = checkBox16_2.Checked;

            //---Condition3---
            //Textbox to String
            textBox_Condition3[0] = textBox1_3.Text;
            textBox_Condition3[1] = textBox2_3.Text;
            textBox_Condition3[2] = textBox3_3.Text;
            textBox_Condition3[3] = textBox4_3.Text;
            textBox_Condition3[4] = textBox5_3.Text;
            textBox_Condition3[5] = textBox6_3.Text;
            textBox_Condition3[6] = textBox7_3.Text;
            textBox_Condition3[7] = textBox8_3.Text;
            textBox_Condition3[8] = textBox9_3.Text;
            textBox_Condition3[9] = textBox10_3.Text;
            textBox_Condition3[10] = textBox11_3.Text;
            textBox_Condition3[11] = textBox12_3.Text;
            textBox_Condition3[12] = textBox13_3.Text;
            textBox_Condition3[13] = textBox14_3.Text;
            textBox_Condition3[14] = textBox15_3.Text;
            textBox_Condition3[15] = textBox16_3.Text;
            //CheckBox to Bool
            checkBox_Condition3[0] = checkBox1_3.Checked;
            checkBox_Condition3[1] = checkBox2_3.Checked;
            checkBox_Condition3[2] = checkBox3_3.Checked;
            checkBox_Condition3[3] = checkBox4_3.Checked;
            checkBox_Condition3[4] = checkBox5_3.Checked;
            checkBox_Condition3[5] = checkBox6_3.Checked;
            checkBox_Condition3[6] = checkBox7_3.Checked;
            checkBox_Condition3[7] = checkBox8_3.Checked;
            checkBox_Condition3[8] = checkBox9_3.Checked;
            checkBox_Condition3[9] = checkBox10_3.Checked;
            checkBox_Condition3[10] = checkBox11_3.Checked;
            checkBox_Condition3[11] = checkBox12_3.Checked;
            checkBox_Condition3[12] = checkBox13_3.Checked;
            checkBox_Condition3[13] = checkBox14_3.Checked;
            checkBox_Condition3[14] = checkBox15_3.Checked;
            checkBox_Condition3[15] = checkBox16_3.Checked;
        }

        //Form Parameter
        public bool Availability_Agine = false;
        public bool Availability_E3 = false;
        public bool Availability_E2 = false;
        public bool Availability_Diff = false;
        public bool Availability_Diff_BCS = false;
        public bool Availability_AOD_GCS = false;
        public bool Availability_Gamma_Crush = false;


        private void dataGridView_Inialize(int condition, string first)
        {
            DataGridView datagridview;
            if (condition == 1) datagridview = dataGridView1;
            else if (condition == 2) datagridview = dataGridView2;
            else if (condition == 3) datagridview = dataGridView3;
            else if (condition == 4) datagridview = dataGridView4;
            else if (condition == 5) datagridview = dataGridView5;
            else if (condition == 6) datagridview = dataGridView6;
            else if (condition == 7) datagridview = dataGridView7;
            else if (condition == 8) datagridview = dataGridView8;
            else if (condition == 9) datagridview = dataGridView9;
            else if (condition == 10) datagridview = dataGridView10;
            else if (condition == 11) datagridview = dataGridView11;
            else if (condition == 12) datagridview = dataGridView12;
            else if (condition == 13) datagridview = dataGridView13;
            else datagridview = null;

            //Set the datagridview's EnableHeadersVisualStyles to false to get the header cell to accept the color change
            datagridview.EnableHeadersVisualStyles = false;
            datagridview.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            datagridview.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;

            //Delta E Data Grid view initialize
            datagridview.Columns.Add(first, first);
            datagridview.Columns.Add("x", "x");
            datagridview.Columns.Add("y", "y");
            datagridview.Columns.Add("Lv", "Lv");
            if (condition == 1 || condition == 2 || condition == 3 || condition == 13) datagridview.Columns.Add("Delta E*", "E3");
            else if (condition == 4 || condition == 5 || condition == 6) datagridview.Columns.Add("Delta E*", "E2");

            //Auto size (columns)
            datagridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //change color for X/Y/Lv Measured area  
            datagridview.Columns[0].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            datagridview.Columns[0].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            datagridview.Columns[0].HeaderCell.Style.BackColor = System.Drawing.Color.Gray;
            datagridview.Columns[0].HeaderCell.Style.Font = new Font(this.Font, System.Drawing.FontStyle.Bold);
            datagridview.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;

            for (int i = 1; i <= 3; i++)
            {
                if (condition == 1 || condition == 2 || condition == 3)
                {
                    datagridview.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(255, 100, 100);
                    datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 200, 200);
                    //datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
                }
                else if (condition == 4 || condition == 5 || condition == 6)
                {
                    datagridview.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(100, 255, 100);
                    datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(200, 255, 200);
                }
                else if (condition == 7 || condition == 8 || condition == 9)
                {
                    datagridview.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(100, 100, 255);
                    datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(200, 200, 255);
                }
                else if (condition == 10 || condition == 11 || condition == 12)
                {
                    datagridview.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(255, 100, 255);
                    datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 200, 255);
                }
                else if (condition == 13)
                {
                    datagridview.Columns[i].HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(100, 255, 255);
                    datagridview.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(200, 255, 255);
                }
                datagridview.Columns[i].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                datagridview.Columns[i].HeaderCell.Style.Font = new Font(this.Font, System.Drawing.FontStyle.Bold);
                datagridview.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
            }

            if (condition == 1 || condition == 2 || condition == 3 || condition == 4 || condition == 5 || condition == 6 || condition == 13)
            {
                datagridview.Columns[4].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                datagridview.Columns[4].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                datagridview.Columns[4].HeaderCell.Style.BackColor = System.Drawing.Color.Coral;
                datagridview.Columns[4].HeaderCell.Style.Font = new Font(this.Font, System.Drawing.FontStyle.Regular);
                datagridview.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
            }
        }

        public void Set_CA_Channel(CA_Channel channel)
        {
            switch (channel)
            {
                case CA_Channel.ch1:
                    label_CA_Channel.Text = "CA : Channel1";
                    label_CA_Channel.ForeColor = Color.Magenta;
                    break;
                case CA_Channel.ch2:
                    label_CA_Channel.Text = "CA : Channel2";
                    label_CA_Channel.ForeColor = Color.Yellow;
                    break;
                case CA_Channel.ch3:
                    label_CA_Channel.Text = "CA : Channel3";
                    label_CA_Channel.ForeColor = Color.Cyan;
                    break;
                case CA_Channel.ch4:
                    label_CA_Channel.Text = "CA : Channel4";
                    label_CA_Channel.ForeColor = Color.Green;
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("cA Channel should be 1,2,3 or 4");
                    break;
            }
        }
        

        

        private void button_Clear_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();
            dataGridView6.Rows.Clear();
            dataGridView7.Rows.Clear();
            dataGridView8.Rows.Clear();
            dataGridView9.Rows.Clear();
            dataGridView10.Rows.Clear();
            dataGridView11.Rows.Clear();
            dataGridView12.Rows.Clear();
            dataGridView13.Rows.Clear();
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            Availability_Agine = false;
            Availability_E3 = false;
            Availability_E2 = false;
            Availability_Diff = false;
            Availability_Diff_BCS = false;
            Availability_AOD_GCS = false;
            Availability_Gamma_Crush = false;
        }

        private void button_Hide_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void Script_Transform(int condition = 1)
        {
            TextBox TextBox_Mipi_Script_Condition;
            TextBox TextBox_Show_Compared_Mipi_Data;
            if (condition == 1)
            {
                TextBox_Mipi_Script_Condition = textBox_Mipi_Script_Condition1;
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data;
            }
            else if (condition == 2)
            {
                TextBox_Mipi_Script_Condition = textBox_Mipi_Script_Condition2;
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data2;
            }
            else if (condition == 3)
            {
                TextBox_Mipi_Script_Condition = textBox_Mipi_Script_Condition3;
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data3;
            }
            else
            {
                TextBox_Mipi_Script_Condition = null;
                TextBox_Show_Compared_Mipi_Data = null;
            }

            Form1 f1 = (Form1)Application.OpenForms["Form1"];

            string temp_Mipi_Data_String = string.Empty;
            int count_mipi_cmd = 0;
            int count_one_mipi_cmd_length = 0;
            bool Flag = false;

            TextBox_Show_Compared_Mipi_Data.Clear();

            //Delete others except for Mipi CMDs and Write on the 2nd Textbox
            for (int i = 0; i < TextBox_Mipi_Script_Condition.Lines.Length; i++)
            {
                if (TextBox_Mipi_Script_Condition.Lines[i].Length >= 20) // mipi.write 0xXX 0xXX <-- 20ea Character
                {
                    if (TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 10) == "mipi.write")
                    {
                        count_mipi_cmd++;

                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 10; k < TextBox_Mipi_Script_Condition.Lines[i].Length; k++)
                        {
                            if (TextBox_Mipi_Script_Condition.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && TextBox_Mipi_Script_Condition.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 10 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        TextBox_Show_Compared_Mipi_Data.Text += temp_Mipi_Data_String + "\r\n";
                    }
                    else if (TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 14) == "gpio.i2c.write")
                    {
                        count_mipi_cmd++;

                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 14; k < TextBox_Mipi_Script_Condition.Lines[i].Length; k++)
                        {
                            if (TextBox_Mipi_Script_Condition.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && TextBox_Mipi_Script_Condition.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 14 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        TextBox_Show_Compared_Mipi_Data.Text += temp_Mipi_Data_String + "\r\n";
                    }

                    else
                    {
                        // It's not a "mipi.write" of "delay" command , do nothing 
                    }
                }

                //Delay
                else if (TextBox_Mipi_Script_Condition.Lines[i].Length >= 5
                    && TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5) != "     ")
                {
                    if (TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5) == "delay")
                    {
                        count_mipi_cmd++;
                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 5; k < TextBox_Mipi_Script_Condition.Lines[i].Length; k++)
                        {
                            if (TextBox_Mipi_Script_Condition.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && TextBox_Mipi_Script_Condition.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        TextBox_Show_Compared_Mipi_Data.Text += temp_Mipi_Data_String + "\r\n";
                    }

                    else if (TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5) == "image")
                    {
                        count_mipi_cmd++;
                        count_one_mipi_cmd_length = 0;
                        Flag = false;
                        for (int k = 5; k < TextBox_Mipi_Script_Condition.Lines[i].Length; k++)
                        {
                            if (TextBox_Mipi_Script_Condition.Lines[i][k] != '#') //주석이 없으면
                            {
                                count_one_mipi_cmd_length++;
                            }
                            else //<-- 주석이 나타나면 그만 Count
                            {
                                Flag = true;
                                break;
                            }
                        }
                        if (Flag && TextBox_Mipi_Script_Condition.Lines[i][count_one_mipi_cmd_length] != ' ')
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else if (Flag == false)
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length) + " ";
                        }
                        else
                        {
                            temp_Mipi_Data_String = TextBox_Mipi_Script_Condition.Lines[i].Substring(0, 5 + count_one_mipi_cmd_length);
                        }

                        temp_Mipi_Data_String = System.Text.RegularExpressions.Regex.Replace(temp_Mipi_Data_String, @"\s+", " ");
                        TextBox_Show_Compared_Mipi_Data.Text += temp_Mipi_Data_String + "\r\n";
                    }
                }
            }
        }






        private void button_Script_Transform_Click(object sender, EventArgs e)
        {
            Script_Transform(1);
        }

        private void button_Script_Transform2_Click(object sender, EventArgs e)
        {
            Script_Transform(2);
        }

        private void button_Script_Transform3_Click(object sender, EventArgs e)
        {
            Script_Transform(3);
        }

        private void Script_Apply(int condition)
        {
            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            TextBox TextBox_Show_Compared_Mipi_Data;
            if (condition == 1)
            {
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data;
                f1.GB_Status_AppendText_Nextline("1st Condition Script Applied", Color.Teal);
            }
            else if (condition == 2)
            {
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data2;
                f1.GB_Status_AppendText_Nextline("2nd Condition Script Applied", Color.Green);
            }
            else if (condition == 3)
            {
                TextBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data3;
                f1.GB_Status_AppendText_Nextline("3rd Condition Script Applied", Color.Olive);
            }
            else TextBox_Show_Compared_Mipi_Data = null;

            //Send "mipi.write" of "delay" command
            for (int i = 0; i < TextBox_Show_Compared_Mipi_Data.Lines.Length - 1; i++)
            {
                System.Windows.Forms.Application.DoEvents();

                if (TextBox_Show_Compared_Mipi_Data.Lines[i].Length >= 10
                    && TextBox_Show_Compared_Mipi_Data.Lines[i].Substring(0, 10) == "mipi.write")
                {
                    f1.IPC_Quick_Send(TextBox_Show_Compared_Mipi_Data.Lines[i]);
                }
                else if (TextBox_Show_Compared_Mipi_Data.Lines[i].Length >= 5 && (
                    TextBox_Show_Compared_Mipi_Data.Lines[i].Substring(0, 5) == "delay"
                    || TextBox_Show_Compared_Mipi_Data.Lines[i].Substring(0, 5) == "image"))
                {
                    f1.IPC_Quick_Send(TextBox_Show_Compared_Mipi_Data.Lines[i]);
                }
                else if (TextBox_Show_Compared_Mipi_Data.Lines[i].Substring(0, 14) == "gpio.i2c.write")
                {
                    f1.IPC_Quick_Send(TextBox_Show_Compared_Mipi_Data.Lines[i]);
                }
                else
                {
                    // It's not a "mipi.write" command , do nothing 
                }
            }
        }


        private void Script_Apply_For_Condition1()
        {
            Script_Apply(1);
            int delay = Convert.ToInt16(textBox_delay_After_Condition_1.Text);
            Thread.Sleep(delay);
            f1().GB_Status_AppendText_Nextline("Thread delay " + delay.ToString() + " was applied", Color.Teal);
        }

        private void Script_Apply_For_Condition2()
        {
            Script_Apply(2);
            int delay = Convert.ToInt16(textBox_delay_After_Condition_2.Text);
            Thread.Sleep(delay);
            f1().GB_Status_AppendText_Nextline("Thread delay " + delay.ToString() + " was applied", Color.Green);
        }

        private void Script_Apply_For_Condition3()
        {
            Script_Apply(3);
            int delay = Convert.ToInt16(textBox_delay_After_Condition_3.Text);
            Thread.Sleep(delay);
            f1().GB_Status_AppendText_Nextline("Thread delay " + delay.ToString() + " was applied", Color.Olive);
        }

        private void button_Script_Clear_Click(object sender, EventArgs e)
        {
            textBox_Mipi_Script_Condition1.Clear();
            textBox_Show_Compared_Mipi_Data.Clear();
        }

        private void button_Script_Clear2_Click(object sender, EventArgs e)
        {
            textBox_Mipi_Script_Condition2.Clear();
            textBox_Show_Compared_Mipi_Data2.Clear();
        }

        private void button_Script_Clear3_Click(object sender, EventArgs e)
        {
            textBox_Mipi_Script_Condition3.Clear();
            textBox_Show_Compared_Mipi_Data3.Clear();
        }

        private void Button_Click_Enable(bool Able)
        {
            //Button
            Delta_E_calculation_btn.Enabled = Able;
            Delta_E2_calculation_btn.Enabled = Able;
            button_Clear.Enabled = Able;
            button_Save_Setting.Enabled = Able;
            button_Load_Setting.Enabled = Able;
            button_SH_GCS_Difference_Measure.Enabled = Able;
            button_All_At_Once.Enabled = Able;
            button_AOD_GCS.Enabled = Able;
            button_Gamma_Crush.Enabled = Able;
        }

        private void Calculate_Delta_E_From_x_y_Lv(int gray_end_Point, int Addtional_DeltaE_Rows, int Condition = 1)
        {
            DataGridView datagridview;
            if (Condition == 1) datagridview = dataGridView1;
            else if (Condition == 2) datagridview = dataGridView2;
            else if (Condition == 3) datagridview = dataGridView3;
            else if (Condition == 4) datagridview = dataGridView13;//AOD
            else datagridview = null;

            if (Availability_E3 || Availability_AOD_GCS)
            {
                // x/y/Lv 계산 완료 후 -- > X Y Z 계산
                double x;
                double y;
                double Lv;

                double[] X_Array = new double[256];//0~255 (maximum 256ea) 
                double[] Y_Array = new double[256];//0~255 (maximum 256ea)
                double[] Z_Array = new double[256];//0~255 (maximum 256ea)

                Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
                int Offset = (Addtional_DeltaE_Rows) - (255 - gray_end_Point + 1);
                
                for (int gray = (gray_end_Point + Offset); gray <= (255 + Offset); gray++)
                {
                    f1.GB_Status_AppendText_Nextline("[gray - gray_end_Point] : " + (gray - gray_end_Point).ToString(), Color.Blue, true);
                    x = Convert.ToDouble(datagridview.Rows[gray - gray_end_Point].Cells[1].Value);
                    y = Convert.ToDouble(datagridview.Rows[gray - gray_end_Point].Cells[2].Value);
                    Lv = Convert.ToDouble(datagridview.Rows[gray - gray_end_Point].Cells[3].Value);

                    f1.GB_Status_AppendText_Nextline("[gray - (gray_end_Point+Offset)] : " + (gray - (gray_end_Point + Offset)).ToString(), Color.Blue, true);
                    X_Array[gray - (gray_end_Point + Offset)] = (x / y) * Lv;
                    Y_Array[gray - (gray_end_Point + Offset)] = Lv;
                    Z_Array[gray - (gray_end_Point + Offset)] = ((1 - x - y) / y) * Lv;
                }


                // X/Y/Z 계산 완료 후 --> L* / f(X/Xw) / f(Y/Yw) / f(Z/Zw) 
                double L, FX, FY, FZ;

                double X255 = 0;
                double Y255 = 0;
                double Z255 = 0;

                if (radioButton_Min_to_Max_E3.Checked)
                {
                    f1.GB_Status_AppendText_Nextline("[255 - gray_end_Point] : " + (255 - gray_end_Point).ToString(), Color.Red, true);
                    X255 = X_Array[255 - gray_end_Point];
                    Y255 = Y_Array[255 - gray_end_Point];
                    Z255 = Z_Array[255 - gray_end_Point];
                }
                else if (radioButton_Max_to_Min_E3.Checked)
                {
                    X255 = X_Array[0];
                    Y255 = Y_Array[0];
                    Z255 = Z_Array[0];
                }

                //a* , b* , Delta E
                double a, b, Delta_E;
                double Max_Delta_E = 0;
                double X;
                double Y;
                double Z;

                for (int gray = (gray_end_Point + Offset); gray <= (255 + Offset); gray++)
                {
                    f1.GB_Status_AppendText_Nextline("[gray - (gray_end_Point+Offset)] : " + (gray - (gray_end_Point + Offset)).ToString(), Color.Red, true);
                    X = X_Array[gray - (gray_end_Point + Offset)];
                    Y = Y_Array[gray - (gray_end_Point + Offset)];
                    Z = Z_Array[gray - (gray_end_Point + Offset)];

                    //Calculate L*
                    if (Y / Y255 > 0.008856) L = 116 * Math.Pow(Y / Y255, 0.33333333) - 16;
                    else L = 903.3 * (Y / Y255);

                    //Calculate F(X/Xw)
                    if (X / X255 > 0.008856) FX = Math.Pow((X / X255), 0.33333333);
                    else FX = 7.787 * (X / X255) + (16 / 116.0);

                    //Calculate F(Y/Yw)
                    if (Y / Y255 > 0.008856) FY = Math.Pow((Y / Y255), 0.33333333);
                    else FY = 7.787 * (Y / Y255) + (16 / 116.0);

                    //Calculate F(Z/Zw)
                    if (Z / Z255 > 0.008856) FZ = Math.Pow((Z / Z255), 0.33333333);
                    else FZ = 7.787 * (Z / Z255) + (16 / 116.0);

                    //Calculate a* , b* , Delta E
                    a = 500 * (FX - FY);
                    b = 200 * (FY - FZ);
                    Delta_E = Math.Pow((Math.Pow(a, 2) + Math.Pow(b, 2)), 0.5);

                    datagridview.Rows[gray - gray_end_Point].Cells[4].Value = Delta_E; //Delta E

                    if (Max_Delta_E <= Delta_E) Max_Delta_E = Delta_E;
                }
                //Excel 에 Data 남기기 위한 자료 추가.
                datagridview.Rows.Add("Delta E3"); // 한열은 띄어쓰기로
                datagridview.Rows.Add(Max_Delta_E.ToString());
                datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
            }
        }


        private void Measure_And_Calculate_Delta_E(int gray_end_Point, int delay_time_between_measurement, int Condition = 1)
        {
            Optic_SH_Delta_E3_Measure(gray_end_Point, delay_time_between_measurement, Condition);

            int Addtional_DeltaE_Rows = 0;
            if (Condition == 1) Addtional_DeltaE_Rows = (dataGridView1.Rows.Count - 1);
            else if (Condition == 2) Addtional_DeltaE_Rows = (dataGridView2.Rows.Count - 1);
            else if (Condition == 3) Addtional_DeltaE_Rows = (dataGridView3.Rows.Count - 1);
            Calculate_Delta_E_From_x_y_Lv(gray_end_Point, Addtional_DeltaE_Rows, Condition);
        }

        private void AOD_GCS_Textbox_CheckBox_Enable(bool Able)
        {
            textBox_AOD_DBV1.Enabled = Able;
            textBox_AOD_DBV2.Enabled = Able;
            textBox_AOD_DBV3.Enabled = Able;
            textBox_AOD_DBV4.Enabled = Able;
            textBox_AOD_DBV5.Enabled = Able;
            textBox_AOD_DBV6.Enabled = Able;

            checkBox_AOD_DBV1.Enabled = Able;
            checkBox_AOD_DBV2.Enabled = Able;
            checkBox_AOD_DBV3.Enabled = Able;
            checkBox_AOD_DBV4.Enabled = Able;
            checkBox_AOD_DBV5.Enabled = Able;
            checkBox_AOD_DBV6.Enabled = Able;
        }


        private void Delta_E3_Textbox_CheckBox_Radiobutton_Enable(bool Able)
        {
            //CheckBox
            checkBox_1st_Condition_Measure_E3.Enabled = Able;
            checkBox_2nd_Condition_Measure_E3.Enabled = Able;
            checkBox_3rd_Condition_Measure_E3.Enabled = Able;
            checkBox_Ave_Measure_E3.Enabled = Able;

            //TextBox
            textBox_delay_time.Enabled = Able;
            textBox_Delta_E_End_Point.Enabled = Able;
            textBox_Ave_Lv_Limit_E3.Enabled = Able;

            //Radiobutton
            radioButton_Min_to_Max_E3.Enabled = Able;
            radioButton_Max_to_Min_E3.Enabled = Able;

            //---Condition1---
            textBox1_1.Enabled = Able;
            textBox2_1.Enabled = Able;
            textBox3_1.Enabled = Able;
            textBox4_1.Enabled = Able;
            textBox5_1.Enabled = Able;
            textBox6_1.Enabled = Able;
            textBox7_1.Enabled = Able;
            textBox8_1.Enabled = Able;
            textBox9_1.Enabled = Able;
            textBox10_1.Enabled = Able;
            textBox11_1.Enabled = Able;
            textBox12_1.Enabled = Able;
            textBox13_1.Enabled = Able;
            textBox14_1.Enabled = Able;
            textBox15_1.Enabled = Able;
            textBox16_1.Enabled = Able;
            //CheckBox to Bool
            checkBox1_1.Enabled = Able;
            checkBox2_1.Enabled = Able;
            checkBox3_1.Enabled = Able;
            checkBox4_1.Enabled = Able;
            checkBox5_1.Enabled = Able;
            checkBox6_1.Enabled = Able;
            checkBox7_1.Enabled = Able;
            checkBox8_1.Enabled = Able;
            checkBox9_1.Enabled = Able;
            checkBox10_1.Enabled = Able;
            checkBox11_1.Enabled = Able;
            checkBox12_1.Enabled = Able;
            checkBox13_1.Enabled = Able;
            checkBox14_1.Enabled = Able;
            checkBox15_1.Enabled = Able;
            checkBox16_1.Enabled = Able;

            //---Condition2---
            //Textbox to String
            textBox1_2.Enabled = Able;
            textBox2_2.Enabled = Able;
            textBox3_2.Enabled = Able;
            textBox4_2.Enabled = Able;
            textBox5_2.Enabled = Able;
            textBox6_2.Enabled = Able;
            textBox7_2.Enabled = Able;
            textBox8_2.Enabled = Able;
            textBox9_2.Enabled = Able;
            textBox10_2.Enabled = Able;
            textBox11_2.Enabled = Able;
            textBox12_2.Enabled = Able;
            textBox13_2.Enabled = Able;
            textBox14_2.Enabled = Able;
            textBox15_2.Enabled = Able;
            textBox16_2.Enabled = Able;
            //CheckBox to Bool
            checkBox1_2.Enabled = Able;
            checkBox2_2.Enabled = Able;
            checkBox3_2.Enabled = Able;
            checkBox4_2.Enabled = Able;
            checkBox5_2.Enabled = Able;
            checkBox6_2.Enabled = Able;
            checkBox7_2.Enabled = Able;
            checkBox8_2.Enabled = Able;
            checkBox9_2.Enabled = Able;
            checkBox10_2.Enabled = Able;
            checkBox11_2.Enabled = Able;
            checkBox12_2.Enabled = Able;
            checkBox13_2.Enabled = Able;
            checkBox14_2.Enabled = Able;
            checkBox15_2.Enabled = Able;
            checkBox16_2.Enabled = Able;

            //---Condition3---
            //Textbox to String
            textBox1_3.Enabled = Able;
            textBox2_3.Enabled = Able;
            textBox3_3.Enabled = Able;
            textBox4_3.Enabled = Able;
            textBox5_3.Enabled = Able;
            textBox6_3.Enabled = Able;
            textBox7_3.Enabled = Able;
            textBox8_3.Enabled = Able;
            textBox9_3.Enabled = Able;
            textBox10_3.Enabled = Able;
            textBox11_3.Enabled = Able;
            textBox12_3.Enabled = Able;
            textBox13_3.Enabled = Able;
            textBox14_3.Enabled = Able;
            textBox15_3.Enabled = Able;
            textBox16_3.Enabled = Able;
            //CheckBox to Bool
            checkBox1_3.Enabled = Able;
            checkBox2_3.Enabled = Able;
            checkBox3_3.Enabled = Able;
            checkBox4_3.Enabled = Able;
            checkBox5_3.Enabled = Able;
            checkBox6_3.Enabled = Able;
            checkBox7_3.Enabled = Able;
            checkBox8_3.Enabled = Able;
            checkBox9_3.Enabled = Able;
            checkBox10_3.Enabled = Able;
            checkBox11_3.Enabled = Able;
            checkBox12_3.Enabled = Able;
            checkBox13_3.Enabled = Able;
            checkBox14_3.Enabled = Able;
            checkBox15_3.Enabled = Able;
            checkBox16_3.Enabled = Able;
        }

        private void Delta_E_calculation_btn_Click(object sender, EventArgs e)
        {
            button_Stop.PerformClick();

            if (f1().label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                Button_Click_Enable(false);
                Delta_E3_Textbox_CheckBox_Radiobutton_Enable(false);
                Update_E3_DBV_Checkbox_And_Textbox();

                //Set ProgressBar_E3 Max and Step and Value
                int Progress_Bar_Condion1_Max = 0;
                int Progress_Bar_Condion2_Max = 0;
                int Progress_Bar_Condion3_Max = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (checkBox_Condition1[i]) Progress_Bar_Condion1_Max++;
                    if (checkBox_Condition2[i]) Progress_Bar_Condion2_Max++;
                    if (checkBox_Condition3[i]) Progress_Bar_Condion3_Max++;
                }
                progressBar_E3.Value = 0;
                progressBar_E3.Step = 1;
                progressBar_E3.Maximum = 1;
                if (checkBox_1st_Condition_Measure_E3.Checked) progressBar_E3.Maximum += Progress_Bar_Condion1_Max;
                if (checkBox_2nd_Condition_Measure_E3.Checked) progressBar_E3.Maximum += Progress_Bar_Condion2_Max;
                if (checkBox_3rd_Condition_Measure_E3.Checked) progressBar_E3.Maximum += Progress_Bar_Condion3_Max;
                progressBar_E3.PerformStep();

                //Set gray_end_Point     
                int gray_end_Point = Get_AOD_or_DeltaE3_gray_end_Point();
        
                //Set Availability
                Availability_E3 = true;

                //measure delay time setting
                int delay_time_between_measurement = Convert.ToInt16(textBox_delay_time.Text);

                if (checkBox_1st_Condition_Measure_E3.Checked) dataGridView1.Rows.Clear();
                if (checkBox_2nd_Condition_Measure_E3.Checked) dataGridView2.Rows.Clear();
                if (checkBox_3rd_Condition_Measure_E3.Checked) dataGridView3.Rows.Clear();

                if (checkBox_1st_Condition_Measure_E3.Checked)
                {
                    dataGridView1.Columns[0].HeaderText = "Gray";
                    Script_Apply_For_Condition1();

                    for (int i = 0; i < 16; i++)
                    {
                        if (checkBox_Condition1[i])
                        {
                            string DBV = textBox_Condition1[i].PadLeft(3, '0');//dex to hex (as a string form)
                            try
                            {
                                f1().DBV_Setting(DBV);

                                dataGridView1.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");
                                f1().GB_Status_AppendText_Nextline(i.ToString() + ")1st Condition DBV[" + DBV + "] was applied", Color.Teal);
                            }
                            catch
                            {
                                f1().GB_Status_AppendText_Nextline("Sending 1st Condition DBV[" + DBV + "] was failed", Color.Red);
                            }
                            Measure_And_Calculate_Delta_E(gray_end_Point, delay_time_between_measurement, 1);
                            progressBar_E3.PerformStep();
                        }
                        else
                        {
                            f1().GB_Status_AppendText_Nextline(i.ToString() + ")DBV Point Skip(1st Condition)", Color.Black);
                        }
                    }

                }
                if (checkBox_2nd_Condition_Measure_E3.Checked)
                {
                    dataGridView2.Columns[0].HeaderText = "Gray";
                    Script_Apply_For_Condition2();
                    for (int i = 0; i < 16; i++)
                    {
                        if (checkBox_Condition2[i])
                        {
                            string DBV = textBox_Condition2[i].PadLeft(3, '0');//dex to hex (as a string form)
                            try
                            {
                                f1().DBV_Setting(DBV);

                                dataGridView2.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");
                                f1().GB_Status_AppendText_Nextline(i.ToString() + ")2nd Condition DBV[" + DBV + "] was applied", Color.Green);
                            }
                            catch
                            {
                                f1().GB_Status_AppendText_Nextline("Sending 2nd Condition DBV[" + DBV + "] was failed", Color.Red);
                            }
                            Measure_And_Calculate_Delta_E(gray_end_Point, delay_time_between_measurement, 2);
                            progressBar_E3.PerformStep();
                        }
                        else
                        {
                            f1().GB_Status_AppendText_Nextline(i.ToString() + ")DBV Point Skip(2nd Condition)", Color.Black);
                        }
                    }
                }
                if (checkBox_3rd_Condition_Measure_E3.Checked)
                {
                    dataGridView3.Columns[0].HeaderText = "Gray";
                    Script_Apply_For_Condition3();
                    for (int i = 0; i < 16; i++)
                    {
                        if (checkBox_Condition3[i])
                        {
                            string DBV = textBox_Condition3[i].PadLeft(3, '0');//dex to hex (as a string form)
                            try
                            {
                                f1().DBV_Setting(DBV);

                                dataGridView3.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");
                                f1().GB_Status_AppendText_Nextline(i.ToString() + ")3rd Condition DBV [" + DBV + "] was applied", Color.Olive);
                            }
                            catch
                            {
                                f1().GB_Status_AppendText_Nextline("Sending 3rd Condition DBV[" + DBV + "] was failed", Color.Red);
                            }
                            Measure_And_Calculate_Delta_E(gray_end_Point, delay_time_between_measurement, 3);
                            progressBar_E3.PerformStep();
                        }
                        else
                        {
                            f1().GB_Status_AppendText_Nextline(i.ToString() + ")DBV Point Skip(3rd Condition)", Color.Black);
                        }
                    }
                }
                Button_Click_Enable(true);
                Delta_E3_Textbox_CheckBox_Radiobutton_Enable(true);
            }
        }

        private void button_Save_Setting_Click(object sender, EventArgs e)
        {
            Button_Click_Enable(false);

            Application.DoEvents();

            //------Get Setting Here------
            UserPreferences up = new UserPreferences();

            //---Common---
            //Save Current Date
            DateTime localDate = DateTime.Now;
            up.Saved_Date = localDate.ToString(@"yyyy.MM.dd HH:mm:ss", new CultureInfo("en-US"));

            //Textbox to String
            up.textBox_Mipi_Script_Condition1 = textBox_Mipi_Script_Condition1.Text;
            up.textBox_Mipi_Script_Condition2 = textBox_Mipi_Script_Condition2.Text;
            up.textBox_Mipi_Script_Condition3 = textBox_Mipi_Script_Condition3.Text;

            up.textBox_Show_Compared_Mipi_Data = textBox_Show_Compared_Mipi_Data.Text;
            up.textBox_Show_Compared_Mipi_Data2 = textBox_Show_Compared_Mipi_Data2.Text;
            up.textBox_Show_Compared_Mipi_Data3 = textBox_Show_Compared_Mipi_Data3.Text;

            up.textBox_Ave_Lv_Limit = textBox_Ave_Lv_Limit_E3.Text;
            up.textBox_Delta_E_End_Point = textBox_Delta_E_End_Point.Text;
            up.textBox_delay_time = textBox_delay_time.Text;

            up.textBox_Delta_E2_End_Point = textBox_Delta_E2_End_Point.Text;
            up.textBox_Delta_E2_Max_Point = textBox_Delta_E2_Max_Point.Text;
            up.textBox_Ave_Lv_Limit_E2 = textBox_Ave_Lv_Limit_E2.Text;
            up.textBox_delay_time_E2 = textBox_delay_time_E2.Text;


            up.textBox_delay_After_Condition_1 = textBox_delay_After_Condition_1.Text;
            up.textBox_delay_After_Condition_2 = textBox_delay_After_Condition_2.Text;
            up.textBox_delay_After_Condition_3 = textBox_delay_After_Condition_3.Text;

            up.textBox_Aging_Sec = textBox_Aging_Sec.Text;

            //CheckBox to Bool
            up.checkBox_3rd_Condition_Measure = checkBox_3rd_Condition_Measure_E3.Checked;
            up.checkBox_2nd_Condition_Measure = checkBox_2nd_Condition_Measure_E3.Checked;
            up.checkBox_1st_Condition_Measure = checkBox_1st_Condition_Measure_E3.Checked;
            up.checkBox_Ave_Measure = checkBox_Ave_Measure_E3.Checked;
            up.checkBox_1st_Condition_Measure_E2 = checkBox_1st_Condition_Measure_E2.Checked;
            up.checkBox_2nd_Condition_Measure_E2 = checkBox_2nd_Condition_Measure_E2.Checked;
            up.checkBox_3rd_Condition_Measure_E2 = checkBox_3rd_Condition_Measure_E2.Checked;
            up.checkBox_Ave_Measure_E2 = checkBox_Ave_Measure_E2.Checked;
            up.checkBox_White_PTN_Apply_E2 = checkBox_White_PTN_Apply_E2.Checked;

            //RadioButton to Bool
            up.radioButton_Max_to_Min_E3 = radioButton_Max_to_Min_E3.Checked;
            up.radioButton_Min_to_Max_E3 = radioButton_Min_to_Max_E3.Checked;
            up.step_value_1 = step_value_1.Checked;
            up.step_value_4 = step_value_4.Checked;
            up.step_value_8 = step_value_8.Checked;
            up.step_value_16 = step_value_16.Checked;
            up.radioButton_Min_to_Max_E2 = radioButton_Min_to_Max_E2.Checked;
            up.radioButton_Max_to_Min_E2 = radioButton_Max_to_Min_E2.Checked;

            //---Condition---
            //Textbox to String
            up.textBox_Condition1[0] = textBox1_1.Text;
            up.textBox_Condition1[1] = textBox2_1.Text;
            up.textBox_Condition1[2] = textBox3_1.Text;
            up.textBox_Condition1[3] = textBox4_1.Text;
            up.textBox_Condition1[4] = textBox5_1.Text;
            up.textBox_Condition1[5] = textBox6_1.Text;
            up.textBox_Condition1[6] = textBox7_1.Text;
            up.textBox_Condition1[7] = textBox8_1.Text;
            up.textBox_Condition1[8] = textBox9_1.Text;
            up.textBox_Condition1[9] = textBox10_1.Text;
            up.textBox_Condition1[10] = textBox11_1.Text;
            up.textBox_Condition1[11] = textBox12_1.Text;
            up.textBox_Condition1[12] = textBox13_1.Text;
            up.textBox_Condition1[13] = textBox14_1.Text;
            up.textBox_Condition1[14] = textBox15_1.Text;
            up.textBox_Condition1[15] = textBox16_1.Text;
            //CheckBox to Bool
            up.checkBox_Condition1[0] = checkBox1_1.Checked;
            up.checkBox_Condition1[1] = checkBox2_1.Checked;
            up.checkBox_Condition1[2] = checkBox3_1.Checked;
            up.checkBox_Condition1[3] = checkBox4_1.Checked;
            up.checkBox_Condition1[4] = checkBox5_1.Checked;
            up.checkBox_Condition1[5] = checkBox6_1.Checked;
            up.checkBox_Condition1[6] = checkBox7_1.Checked;
            up.checkBox_Condition1[7] = checkBox8_1.Checked;
            up.checkBox_Condition1[8] = checkBox9_1.Checked;
            up.checkBox_Condition1[9] = checkBox10_1.Checked;
            up.checkBox_Condition1[10] = checkBox11_1.Checked;
            up.checkBox_Condition1[11] = checkBox12_1.Checked;
            up.checkBox_Condition1[12] = checkBox13_1.Checked;
            up.checkBox_Condition1[13] = checkBox14_1.Checked;
            up.checkBox_Condition1[14] = checkBox15_1.Checked;
            up.checkBox_Condition1[15] = checkBox16_1.Checked;

            //---Condition2---
            //Textbox to String
            up.textBox_Condition2[0] = textBox1_2.Text;
            up.textBox_Condition2[1] = textBox2_2.Text;
            up.textBox_Condition2[2] = textBox3_2.Text;
            up.textBox_Condition2[3] = textBox4_2.Text;
            up.textBox_Condition2[4] = textBox5_2.Text;
            up.textBox_Condition2[5] = textBox6_2.Text;
            up.textBox_Condition2[6] = textBox7_2.Text;
            up.textBox_Condition2[7] = textBox8_2.Text;
            up.textBox_Condition2[8] = textBox9_2.Text;
            up.textBox_Condition2[9] = textBox10_2.Text;
            up.textBox_Condition2[10] = textBox11_2.Text;
            up.textBox_Condition2[11] = textBox12_2.Text;
            up.textBox_Condition2[12] = textBox13_2.Text;
            up.textBox_Condition2[13] = textBox14_2.Text;
            up.textBox_Condition2[14] = textBox15_2.Text;
            up.textBox_Condition2[15] = textBox16_2.Text;
            //CheckBox to Bool
            up.checkBox_Condition2[0] = checkBox1_2.Checked;
            up.checkBox_Condition2[1] = checkBox2_2.Checked;
            up.checkBox_Condition2[2] = checkBox3_2.Checked;
            up.checkBox_Condition2[3] = checkBox4_2.Checked;
            up.checkBox_Condition2[4] = checkBox5_2.Checked;
            up.checkBox_Condition2[5] = checkBox6_2.Checked;
            up.checkBox_Condition2[6] = checkBox7_2.Checked;
            up.checkBox_Condition2[7] = checkBox8_2.Checked;
            up.checkBox_Condition2[8] = checkBox9_2.Checked;
            up.checkBox_Condition2[9] = checkBox10_2.Checked;
            up.checkBox_Condition2[10] = checkBox11_2.Checked;
            up.checkBox_Condition2[11] = checkBox12_2.Checked;
            up.checkBox_Condition2[12] = checkBox13_2.Checked;
            up.checkBox_Condition2[13] = checkBox14_2.Checked;
            up.checkBox_Condition2[14] = checkBox15_2.Checked;
            up.checkBox_Condition2[15] = checkBox16_2.Checked;

            //---Condition3---
            //Textbox to String
            up.textBox_Condition3[0] = textBox1_3.Text;
            up.textBox_Condition3[1] = textBox2_3.Text;
            up.textBox_Condition3[2] = textBox3_3.Text;
            up.textBox_Condition3[3] = textBox4_3.Text;
            up.textBox_Condition3[4] = textBox5_3.Text;
            up.textBox_Condition3[5] = textBox6_3.Text;
            up.textBox_Condition3[6] = textBox7_3.Text;
            up.textBox_Condition3[7] = textBox8_3.Text;
            up.textBox_Condition3[8] = textBox9_3.Text;
            up.textBox_Condition3[9] = textBox10_3.Text;
            up.textBox_Condition3[10] = textBox11_3.Text;
            up.textBox_Condition3[11] = textBox12_3.Text;
            up.textBox_Condition3[12] = textBox13_3.Text;
            up.textBox_Condition3[13] = textBox14_3.Text;
            up.textBox_Condition3[14] = textBox15_3.Text;
            up.textBox_Condition3[15] = textBox16_3.Text;
            //CheckBox to Bool
            up.checkBox_Condition3[0] = checkBox1_3.Checked;
            up.checkBox_Condition3[1] = checkBox2_3.Checked;
            up.checkBox_Condition3[2] = checkBox3_3.Checked;
            up.checkBox_Condition3[3] = checkBox4_3.Checked;
            up.checkBox_Condition3[4] = checkBox5_3.Checked;
            up.checkBox_Condition3[5] = checkBox6_3.Checked;
            up.checkBox_Condition3[6] = checkBox7_3.Checked;
            up.checkBox_Condition3[7] = checkBox8_3.Checked;
            up.checkBox_Condition3[8] = checkBox9_3.Checked;
            up.checkBox_Condition3[9] = checkBox10_3.Checked;
            up.checkBox_Condition3[10] = checkBox11_3.Checked;
            up.checkBox_Condition3[11] = checkBox12_3.Checked;
            up.checkBox_Condition3[12] = checkBox13_3.Checked;
            up.checkBox_Condition3[13] = checkBox14_3.Checked;
            up.checkBox_Condition3[14] = checkBox15_3.Checked;
            up.checkBox_Condition3[15] = checkBox16_3.Checked;
            //------------------------

            //-------GCS Diff Measure Related---------
            up.checkBox_Diff[0] = checkBox1_Diff.Checked;
            up.checkBox_Diff[1] = checkBox2_Diff.Checked;
            up.checkBox_Diff[2] = checkBox3_Diff.Checked;
            up.checkBox_Diff[3] = checkBox4_Diff.Checked;
            up.checkBox_Diff[4] = checkBox5_Diff.Checked;
            up.checkBox_Diff[5] = checkBox6_Diff.Checked;
            up.checkBox_Diff[6] = checkBox7_Diff.Checked;
            up.checkBox_Diff[7] = checkBox8_Diff.Checked;
            up.checkBox_Diff[8] = checkBox9_Diff.Checked;
            up.checkBox_Diff[9] = checkBox10_Diff.Checked;
            up.checkBox_Diff[10] = checkBox11_Diff.Checked;
            up.checkBox_Diff[11] = checkBox12_Diff.Checked;
            up.checkBox_Diff[12] = checkBox13_Diff.Checked;
            up.checkBox_Diff[13] = checkBox14_Diff.Checked;
            up.checkBox_Diff[14] = checkBox15_Diff.Checked;
            up.checkBox_Diff[15] = checkBox16_Diff.Checked;
            up.checkBox_Diff[16] = checkBox17_Diff.Checked;
            up.checkBox_Diff[17] = checkBox18_Diff.Checked;
            up.checkBox_Diff[18] = checkBox19_Diff.Checked;
            up.checkBox_Diff[19] = checkBox20_Diff.Checked;


            up.checkBox_Ave_Measure_Diff = checkBox_Ave_Measure_Diff.Checked;


            up.textBox_Diff[0] = textBox1_Diff.Text;
            up.textBox_Diff[1] = textBox2_Diff.Text;
            up.textBox_Diff[2] = textBox3_Diff.Text;
            up.textBox_Diff[3] = textBox4_Diff.Text;
            up.textBox_Diff[4] = textBox5_Diff.Text;
            up.textBox_Diff[5] = textBox6_Diff.Text;
            up.textBox_Diff[6] = textBox7_Diff.Text;
            up.textBox_Diff[7] = textBox8_Diff.Text;
            up.textBox_Diff[8] = textBox9_Diff.Text;
            up.textBox_Diff[9] = textBox10_Diff.Text;
            up.textBox_Diff[10] = textBox11_Diff.Text;
            up.textBox_Diff[11] = textBox12_Diff.Text;
            up.textBox_Diff[12] = textBox13_Diff.Text;
            up.textBox_Diff[13] = textBox14_Diff.Text;
            up.textBox_Diff[14] = textBox15_Diff.Text;
            up.textBox_Diff[15] = textBox16_Diff.Text;
            up.textBox_Diff[16] = textBox17_Diff.Text;
            up.textBox_Diff[17] = textBox18_Diff.Text;
            up.textBox_Diff[18] = textBox19_Diff.Text;
            up.textBox_Diff[19] = textBox20_Diff.Text;


            up.textBox_delay_time_Diff = textBox_delay_time_Diff.Text;
            up.textBox_Ave_Lv_Limit_Diff = textBox_Ave_Lv_Limit_Diff.Text;

            up.textBox_GCS_Diff_Min_Point = textBox_GCS_Diff_Min_Point.Text;
            up.textBox_GCS_Diff_Max_Point = textBox_GCS_Diff_Max_Point.Text;

            up.radioButton_Diff_1st_and_3rd = radioButton_Diff_1st_and_3rd.Checked;
            up.radioButton_Diff_2nd_and_3rd = radioButton_Diff_2nd_and_3rd.Checked;
            up.radioButton_Diff_1st_and_2nd = radioButton_Diff_1st_and_2nd.Checked;
            up.radioButton_Diff_1st_2nd_3rd = radioButton_Diff_1st_2nd_3rd.Checked;

            up.radioButton_SH_Diff_Step_16 = radioButton_SH_Diff_Step_16.Checked;
            up.radioButton_SH_Diff_Step_8 = radioButton_SH_Diff_Step_8.Checked;
            up.radioButton_SH_Diff_Step_4 = radioButton_SH_Diff_Step_4.Checked;
            //------------------------------------

            //-------BCS Diff Measure Related--------
            up.textBox_delay_time_Diff_BCS = textBox_delay_time_Diff_BCS.Text;

            up.BCS_Diff_step_value_1_Range1 = BCS_Diff_step_value_1_Range1.Checked;
            up.BCS_Diff_step_value_4_Range1 = BCS_Diff_step_value_4_Range1.Checked;
            up.BCS_Diff_step_value_8_Range1 = BCS_Diff_step_value_8_Range1.Checked;
            up.BCS_Diff_step_value_16_Range1 = BCS_Diff_step_value_16_Range1.Checked;

            up.BCS_Diff_step_value_1_Range2 = BCS_Diff_step_value_1_Range2.Checked;
            up.BCS_Diff_step_value_4_Range2 = BCS_Diff_step_value_4_Range2.Checked;
            up.BCS_Diff_step_value_8_Range2 = BCS_Diff_step_value_8_Range2.Checked;
            up.BCS_Diff_step_value_16_Range2 = BCS_Diff_step_value_16_Range2.Checked;

            up.BCS_Diff_step_value_1_Range3 = BCS_Diff_step_value_1_Range3.Checked;
            up.BCS_Diff_step_value_4_Range3 = BCS_Diff_step_value_4_Range3.Checked;
            up.BCS_Diff_step_value_8_Range3 = BCS_Diff_step_value_8_Range3.Checked;
            up.BCS_Diff_step_value_16_Range3 = BCS_Diff_step_value_16_Range3.Checked;

            up.CheckBox_BCS_Diff_Gray_Points[0] = checkBox1_BCS_Diff_Gray_P1.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[1] = checkBox1_BCS_Diff_Gray_P2.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[2] = checkBox1_BCS_Diff_Gray_P3.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[3] = checkBox1_BCS_Diff_Gray_P4.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[4] = checkBox1_BCS_Diff_Gray_P5.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[5] = checkBox1_BCS_Diff_Gray_P6.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[6] = checkBox1_BCS_Diff_Gray_P7.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[7] = checkBox1_BCS_Diff_Gray_P8.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[8] = checkBox1_BCS_Diff_Gray_P9.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[9] = checkBox1_BCS_Diff_Gray_P10.Checked;
            up.CheckBox_BCS_Diff_Gray_Points[10] = checkBox1_BCS_Diff_Gray_P11.Checked;

            up.textBox_BCS_Diff_Gray_Points[0] = textBox_BCS_Diff_Gray_P1.Text;
            up.textBox_BCS_Diff_Gray_Points[1] = textBox_BCS_Diff_Gray_P2.Text;
            up.textBox_BCS_Diff_Gray_Points[2] = textBox_BCS_Diff_Gray_P3.Text;
            up.textBox_BCS_Diff_Gray_Points[3] = textBox_BCS_Diff_Gray_P4.Text;
            up.textBox_BCS_Diff_Gray_Points[4] = textBox_BCS_Diff_Gray_P5.Text;
            up.textBox_BCS_Diff_Gray_Points[5] = textBox_BCS_Diff_Gray_P6.Text;
            up.textBox_BCS_Diff_Gray_Points[6] = textBox_BCS_Diff_Gray_P7.Text;
            up.textBox_BCS_Diff_Gray_Points[7] = textBox_BCS_Diff_Gray_P8.Text;
            up.textBox_BCS_Diff_Gray_Points[8] = textBox_BCS_Diff_Gray_P9.Text;
            up.textBox_BCS_Diff_Gray_Points[9] = textBox_BCS_Diff_Gray_P10.Text;
            up.textBox_BCS_Diff_Gray_Points[10] = textBox_BCS_Diff_Gray_P11.Text;

            up.radioButton_BCS_Diff_Dual = radioButton_BCS_Diff_Dual.Checked;
            up.radioButton_BCS_Diff_Triple = radioButton_BCS_Diff_Triple.Checked;

            up.radioButton_BCS_Diff_Single = radioButton_BCS_Diff_Single.Checked;

            up.textBox_BCS_Diff_Sub_1_Min_Point = textBox_BCS_Diff_Sub_1_Min_Point.Text;
            up.textBox_BCS_Diff_Sub_2_Min_Point = textBox_BCS_Diff_Sub_2_Min_Point.Text;
            up.textBox_BCS_Diff_Sub_3_Min_Point = textBox_BCS_Diff_Sub_3_Min_Point.Text;
            up.textBox_BCS_Diff_Sub_1_Max_Point = textBox_BCS_Diff_Sub_1_Max_Point.Text;
            up.textBox_BCS_Diff_Sub_2_Max_Point = textBox_BCS_Diff_Sub_2_Max_Point.Text;
            up.textBox_BCS_Diff_Sub_3_Max_Point = textBox_BCS_Diff_Sub_3_Max_Point.Text;

            up.checkBox_BCS_Diif_Range_1 = checkBox_BCS_Diif_Range_1.Checked;
            up.checkBox_BCS_Diif_Range_2 = checkBox_BCS_Diif_Range_2.Checked;
            up.checkBox_BCS_Diif_Range_3 = checkBox_BCS_Diif_Range_3.Checked;
            //---------------------------------------

            //-------All_At_Once Measure Related---------
            up.checkBox_All_At_Once_E3 = checkBox_All_At_Once_E3.Checked;
            up.checkBox_All_At_Once_E2 = checkBox_All_At_Once_E2.Checked;
            up.checkBox_All_At_Once_Diff = checkBox_All_At_Once_Diff_GCS.Checked;
            //-------------------------------------------


            up.radioButton_GCS_White_PTN = radioButton_GCS_White_PTN.Checked;
            up.radioButton_GCS_Red_PTN = radioButton_GCS_Red_PTN.Checked;
            up.radioButton_GCS_Green_PTN = radioButton_GCS_Green_PTN.Checked;
            up.radioButton_GCS_Blue_PTN = radioButton_GCS_Blue_PTN.Checked;

            //-----AOD GCS Measure Related------
            //Textbox to String
            up.textBox_AOD_DBV1 = textBox_AOD_DBV1.Text;
            up.textBox_AOD_DBV2 = textBox_AOD_DBV2.Text;
            up.textBox_AOD_DBV3 = textBox_AOD_DBV3.Text;
            up.textBox_AOD_DBV4 = textBox_AOD_DBV4.Text;
            up.textBox_AOD_DBV5 = textBox_AOD_DBV5.Text;
            up.textBox_AOD_DBV6 = textBox_AOD_DBV6.Text;


            //CheckBox to Bool
            up.checkBox_AOD_DBV1 = checkBox_AOD_DBV1.Checked;
            up.checkBox_AOD_DBV2 = checkBox_AOD_DBV2.Checked;
            up.checkBox_AOD_DBV3 = checkBox_AOD_DBV3.Checked;
            up.checkBox_AOD_DBV4 = checkBox_AOD_DBV4.Checked;
            up.checkBox_AOD_DBV5 = checkBox_AOD_DBV5.Checked;
            up.checkBox_AOD_DBV6 = checkBox_AOD_DBV6.Checked;
            up.checkBox_All_At_Once_AOD_GCS = checkBox_All_At_Once_AOD_GCS.Checked;
            //-------------------------------

            //----- Gamma Crush ------
            up.checkBox_Gamma_Crush_Conditon_1 = checkBox_Gamma_Crush_Conditon_1.Checked;
            up.checkBox_Gamma_Crush_Conditon_2 = checkBox_Gamma_Crush_Conditon_2.Checked;
            up.checkBox_Gamma_Crush_Conditon_3 = checkBox_Gamma_Crush_Conditon_3.Checked;

            up.checkBox_Gamma_Crush_W = checkBox_Gamma_Crush_W.Checked;
            up.checkBox_Gamma_Crush_R = checkBox_Gamma_Crush_R.Checked;
            up.checkBox_Gamma_Crush_G = checkBox_Gamma_Crush_G.Checked;
            up.checkBox_Gamma_Crush_B = checkBox_Gamma_Crush_B.Checked;
            up.checkBox_Gamma_Crush_GB = checkBox_Gamma_Crush_GB.Checked;
            up.checkBox_Gamma_Crush_RB = checkBox_Gamma_Crush_RB.Checked;
            up.checkBox_Gamma_Crush_RG = checkBox_Gamma_Crush_RG.Checked;

            up.checkBox_Gamma_Crush[0] = checkBox_Gamma_Crush_P1.Checked;
            up.checkBox_Gamma_Crush[1] = checkBox_Gamma_Crush_P2.Checked;
            up.checkBox_Gamma_Crush[2] = checkBox_Gamma_Crush_P3.Checked;
            up.checkBox_Gamma_Crush[3] = checkBox_Gamma_Crush_P4.Checked;
            up.checkBox_Gamma_Crush[4] = checkBox_Gamma_Crush_P5.Checked;
            up.checkBox_Gamma_Crush[5] = checkBox_Gamma_Crush_P6.Checked;
            up.checkBox_Gamma_Crush[6] = checkBox_Gamma_Crush_P7.Checked;
            up.checkBox_Gamma_Crush[7] = checkBox_Gamma_Crush_P8.Checked;
            up.checkBox_Gamma_Crush[8] = checkBox_Gamma_Crush_P9.Checked;
            up.checkBox_Gamma_Crush[9] = checkBox_Gamma_Crush_P10.Checked;

            up.textBox_Gamma_Crush_DBV[0] = textBox_Gamma_Crush_DBV1.Text;
            up.textBox_Gamma_Crush_DBV[1] = textBox_Gamma_Crush_DBV2.Text;
            up.textBox_Gamma_Crush_DBV[2] = textBox_Gamma_Crush_DBV3.Text;
            up.textBox_Gamma_Crush_DBV[3] = textBox_Gamma_Crush_DBV4.Text;
            up.textBox_Gamma_Crush_DBV[4] = textBox_Gamma_Crush_DBV5.Text;
            up.textBox_Gamma_Crush_DBV[5] = textBox_Gamma_Crush_DBV6.Text;
            up.textBox_Gamma_Crush_DBV[6] = textBox_Gamma_Crush_DBV7.Text;
            up.textBox_Gamma_Crush_DBV[7] = textBox_Gamma_Crush_DBV8.Text;
            up.textBox_Gamma_Crush_DBV[8] = textBox_Gamma_Crush_DBV9.Text;
            up.textBox_Gamma_Crush_DBV[9] = textBox_Gamma_Crush_DBV10.Text;
            
            up.textBox_Gamma_Crush_Gray[0] = textBox_Gamma_Crush_Gray_1.Text;
            up.textBox_Gamma_Crush_Gray[1] = textBox_Gamma_Crush_Gray_2.Text;
            up.textBox_Gamma_Crush_Gray[2] = textBox_Gamma_Crush_Gray_3.Text;
            up.textBox_Gamma_Crush_Gray[3] = textBox_Gamma_Crush_Gray_4.Text;
            up.textBox_Gamma_Crush_Gray[4] = textBox_Gamma_Crush_Gray_5.Text;
            up.textBox_Gamma_Crush_Gray[5] = textBox_Gamma_Crush_Gray_6.Text;
            up.textBox_Gamma_Crush_Gray[6] = textBox_Gamma_Crush_Gray_7.Text;
            up.textBox_Gamma_Crush_Gray[7] = textBox_Gamma_Crush_Gray_8.Text;
            up.textBox_Gamma_Crush_Gray[8] = textBox_Gamma_Crush_Gray_9.Text;
            up.textBox_Gamma_Crush_Gray[9] = textBox_Gamma_Crush_Gray_10.Text;
            //-----------------------   

            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Optic_Measurement";
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

        private void button_Load_Setting_Click(object sender, EventArgs e)
        {
            Button_Click_Enable(false);
            Application.DoEvents();

            //------Set Setting Here------
            //FileStream myFileStream = new FileStream(Directory.GetCurrentDirectory() + "\\prefs.xml", FileMode.Open);//Used For Loading Setting

            FileStream myFileStream; 
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()+"\\Optic_Measurement";
            openFileDialog1.Filter = "Default Extension (*.xml)|*.xml";
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.AddExtension = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myFileStream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                UserPreferences up = (UserPreferences)mySerializer.Deserialize(myFileStream);

                //---Common---
                //Textbox to String
                textBox_Mipi_Script_Condition1.Text = up.textBox_Mipi_Script_Condition1;
                textBox_Mipi_Script_Condition2.Text = up.textBox_Mipi_Script_Condition2;
                textBox_Mipi_Script_Condition3.Text = up.textBox_Mipi_Script_Condition3;

                textBox_Delta_E2_End_Point.Text = up.textBox_Delta_E2_End_Point;
                textBox_Delta_E2_Max_Point.Text = up.textBox_Delta_E2_Max_Point;
                textBox_Ave_Lv_Limit_E2.Text = up.textBox_Ave_Lv_Limit_E2;
                textBox_delay_time_E2.Text = up.textBox_delay_time_E2;

                textBox_Show_Compared_Mipi_Data.Text = up.textBox_Show_Compared_Mipi_Data;
                textBox_Show_Compared_Mipi_Data2.Text = up.textBox_Show_Compared_Mipi_Data2;
                textBox_Show_Compared_Mipi_Data3.Text = up.textBox_Show_Compared_Mipi_Data3;

                textBox_Ave_Lv_Limit_E3.Text = up.textBox_Ave_Lv_Limit;
                textBox_Delta_E_End_Point.Text = up.textBox_Delta_E_End_Point;
                textBox_delay_time.Text = up.textBox_delay_time;

                textBox_delay_After_Condition_1.Text = up.textBox_delay_After_Condition_1;
                textBox_delay_After_Condition_2.Text = up.textBox_delay_After_Condition_2;
                textBox_delay_After_Condition_3.Text = up.textBox_delay_After_Condition_3;

                textBox_Aging_Sec.Text = up.textBox_Aging_Sec;

                //CheckBox to Bool
                checkBox_3rd_Condition_Measure_E3.Checked = up.checkBox_3rd_Condition_Measure;
                checkBox_2nd_Condition_Measure_E3.Checked = up.checkBox_2nd_Condition_Measure;
                checkBox_1st_Condition_Measure_E3.Checked = up.checkBox_1st_Condition_Measure;
                checkBox_Ave_Measure_E3.Checked = up.checkBox_Ave_Measure;

                checkBox_1st_Condition_Measure_E2.Checked = up.checkBox_1st_Condition_Measure_E2;
                checkBox_2nd_Condition_Measure_E2.Checked = up.checkBox_2nd_Condition_Measure_E2;
                checkBox_3rd_Condition_Measure_E2.Checked = up.checkBox_3rd_Condition_Measure_E2;
                checkBox_Ave_Measure_E2.Checked = up.checkBox_Ave_Measure_E2;
                checkBox_White_PTN_Apply_E2.Checked = up.checkBox_White_PTN_Apply_E2;

                //RadioButton to Bool
                radioButton_Max_to_Min_E3.Checked = up.radioButton_Max_to_Min_E3;
                radioButton_Min_to_Max_E3.Checked = up.radioButton_Min_to_Max_E3;

                step_value_1.Checked = up.step_value_1;
                step_value_4.Checked = up.step_value_4;
                step_value_8.Checked = up.step_value_8;
                step_value_16.Checked = up.step_value_16;
                radioButton_Min_to_Max_E2.Checked = up.radioButton_Min_to_Max_E2;
                radioButton_Max_to_Min_E2.Checked = up.radioButton_Max_to_Min_E2;

                //---Condition---
                //Textbox to String
                textBox1_1.Text = up.textBox_Condition1[0];
                textBox2_1.Text = up.textBox_Condition1[1];
                textBox3_1.Text = up.textBox_Condition1[2];
                textBox4_1.Text = up.textBox_Condition1[3];
                textBox5_1.Text = up.textBox_Condition1[4];
                textBox6_1.Text = up.textBox_Condition1[5];
                textBox7_1.Text = up.textBox_Condition1[6];
                textBox8_1.Text = up.textBox_Condition1[7];
                textBox9_1.Text = up.textBox_Condition1[8];
                textBox10_1.Text = up.textBox_Condition1[9];
                textBox11_1.Text = up.textBox_Condition1[10];
                textBox12_1.Text = up.textBox_Condition1[11];
                textBox13_1.Text = up.textBox_Condition1[12];
                textBox14_1.Text = up.textBox_Condition1[13];
                textBox15_1.Text = up.textBox_Condition1[14];
                textBox16_1.Text = up.textBox_Condition1[15];
                //CheckBox to Bool
                checkBox1_1.Checked = up.checkBox_Condition1[0];
                checkBox2_1.Checked = up.checkBox_Condition1[1];
                checkBox3_1.Checked = up.checkBox_Condition1[2];
                checkBox4_1.Checked = up.checkBox_Condition1[3];
                checkBox5_1.Checked = up.checkBox_Condition1[4];
                checkBox6_1.Checked = up.checkBox_Condition1[5];
                checkBox7_1.Checked = up.checkBox_Condition1[6];
                checkBox8_1.Checked = up.checkBox_Condition1[7];
                checkBox9_1.Checked = up.checkBox_Condition1[8];
                checkBox10_1.Checked = up.checkBox_Condition1[9];
                checkBox11_1.Checked = up.checkBox_Condition1[10];
                checkBox12_1.Checked = up.checkBox_Condition1[11];
                checkBox13_1.Checked = up.checkBox_Condition1[12];
                checkBox14_1.Checked = up.checkBox_Condition1[13];
                checkBox15_1.Checked = up.checkBox_Condition1[14];
                checkBox16_1.Checked = up.checkBox_Condition1[15];

                //---Condition2---
                //Textbox to String
                textBox1_2.Text = up.textBox_Condition2[0];
                textBox2_2.Text = up.textBox_Condition2[1];
                textBox3_2.Text = up.textBox_Condition2[2];
                textBox4_2.Text = up.textBox_Condition2[3];
                textBox5_2.Text = up.textBox_Condition2[4];
                textBox6_2.Text = up.textBox_Condition2[5];
                textBox7_2.Text = up.textBox_Condition2[6];
                textBox8_2.Text = up.textBox_Condition2[7];
                textBox9_2.Text = up.textBox_Condition2[8];
                textBox10_2.Text = up.textBox_Condition2[9];
                textBox11_2.Text = up.textBox_Condition2[10];
                textBox12_2.Text = up.textBox_Condition2[11];
                textBox13_2.Text = up.textBox_Condition2[12];
                textBox14_2.Text = up.textBox_Condition2[13];
                textBox15_2.Text = up.textBox_Condition2[14];
                textBox16_2.Text = up.textBox_Condition2[15];
                //CheckBox to Bool
                checkBox1_2.Checked = up.checkBox_Condition2[0];
                checkBox2_2.Checked = up.checkBox_Condition2[1];
                checkBox3_2.Checked = up.checkBox_Condition2[2];
                checkBox4_2.Checked = up.checkBox_Condition2[3];
                checkBox5_2.Checked = up.checkBox_Condition2[4];
                checkBox6_2.Checked = up.checkBox_Condition2[5];
                checkBox7_2.Checked = up.checkBox_Condition2[6];
                checkBox8_2.Checked = up.checkBox_Condition2[7];
                checkBox9_2.Checked = up.checkBox_Condition2[8];
                checkBox10_2.Checked = up.checkBox_Condition2[9];
                checkBox11_2.Checked = up.checkBox_Condition2[10];
                checkBox12_2.Checked = up.checkBox_Condition2[11];
                checkBox13_2.Checked = up.checkBox_Condition2[12];
                checkBox14_2.Checked = up.checkBox_Condition2[13];
                checkBox15_2.Checked = up.checkBox_Condition2[14];
                checkBox16_2.Checked = up.checkBox_Condition2[15];

                //---Condition3---
                //Textbox to String
                textBox1_3.Text = up.textBox_Condition3[0];
                textBox2_3.Text = up.textBox_Condition3[1];
                textBox3_3.Text = up.textBox_Condition3[2];
                textBox4_3.Text = up.textBox_Condition3[3];
                textBox5_3.Text = up.textBox_Condition3[4];
                textBox6_3.Text = up.textBox_Condition3[5];
                textBox7_3.Text = up.textBox_Condition3[6];
                textBox8_3.Text = up.textBox_Condition3[7];
                textBox9_3.Text = up.textBox_Condition3[8];
                textBox10_3.Text = up.textBox_Condition3[9];
                textBox11_3.Text = up.textBox_Condition3[10];
                textBox12_3.Text = up.textBox_Condition3[11];
                textBox13_3.Text = up.textBox_Condition3[12];
                textBox14_3.Text = up.textBox_Condition3[13];
                textBox15_3.Text = up.textBox_Condition3[14];
                textBox16_3.Text = up.textBox_Condition3[15];
                //CheckBox to Bool
                checkBox1_3.Checked = up.checkBox_Condition3[0];
                checkBox2_3.Checked = up.checkBox_Condition3[1];
                checkBox3_3.Checked = up.checkBox_Condition3[2];
                checkBox4_3.Checked = up.checkBox_Condition3[3];
                checkBox5_3.Checked = up.checkBox_Condition3[4];
                checkBox6_3.Checked = up.checkBox_Condition3[5];
                checkBox7_3.Checked = up.checkBox_Condition3[6];
                checkBox8_3.Checked = up.checkBox_Condition3[7];
                checkBox9_3.Checked = up.checkBox_Condition3[8];
                checkBox10_3.Checked = up.checkBox_Condition3[9];
                checkBox11_3.Checked = up.checkBox_Condition3[10];
                checkBox12_3.Checked = up.checkBox_Condition3[11];
                checkBox13_3.Checked = up.checkBox_Condition3[12];
                checkBox14_3.Checked = up.checkBox_Condition3[13];
                checkBox15_3.Checked = up.checkBox_Condition3[14];
                checkBox16_3.Checked = up.checkBox_Condition3[15];

                //-------GCS Diff Measure Related---------
                checkBox1_Diff.Checked = up.checkBox_Diff[0];
                checkBox2_Diff.Checked = up.checkBox_Diff[1];
                checkBox3_Diff.Checked = up.checkBox_Diff[2];
                checkBox4_Diff.Checked = up.checkBox_Diff[3];
                checkBox5_Diff.Checked = up.checkBox_Diff[4];
                checkBox6_Diff.Checked = up.checkBox_Diff[5];
                checkBox7_Diff.Checked = up.checkBox_Diff[6];
                checkBox8_Diff.Checked = up.checkBox_Diff[7];
                checkBox9_Diff.Checked = up.checkBox_Diff[8];
                checkBox10_Diff.Checked = up.checkBox_Diff[9];
                checkBox11_Diff.Checked = up.checkBox_Diff[10];
                checkBox12_Diff.Checked = up.checkBox_Diff[11];
                checkBox13_Diff.Checked = up.checkBox_Diff[12];
                checkBox14_Diff.Checked = up.checkBox_Diff[13];
                checkBox15_Diff.Checked = up.checkBox_Diff[14];
                checkBox16_Diff.Checked = up.checkBox_Diff[15];
                checkBox17_Diff.Checked = up.checkBox_Diff[16];
                checkBox18_Diff.Checked = up.checkBox_Diff[17];
                checkBox19_Diff.Checked = up.checkBox_Diff[18];
                checkBox20_Diff.Checked = up.checkBox_Diff[19];

                checkBox_Ave_Measure_Diff.Checked = up.checkBox_Ave_Measure_Diff;

                textBox1_Diff.Text = up.textBox_Diff[0];
                textBox2_Diff.Text = up.textBox_Diff[1];
                textBox3_Diff.Text = up.textBox_Diff[2];
                textBox4_Diff.Text = up.textBox_Diff[3];
                textBox5_Diff.Text = up.textBox_Diff[4];
                textBox6_Diff.Text = up.textBox_Diff[5];
                textBox7_Diff.Text = up.textBox_Diff[6];
                textBox8_Diff.Text = up.textBox_Diff[7];
                textBox9_Diff.Text = up.textBox_Diff[8];
                textBox10_Diff.Text = up.textBox_Diff[9];
                textBox11_Diff.Text = up.textBox_Diff[10];
                textBox12_Diff.Text = up.textBox_Diff[11];
                textBox13_Diff.Text = up.textBox_Diff[12];
                textBox14_Diff.Text = up.textBox_Diff[13];
                textBox15_Diff.Text = up.textBox_Diff[14];
                textBox16_Diff.Text = up.textBox_Diff[15];
                textBox17_Diff.Text = up.textBox_Diff[16];
                textBox18_Diff.Text = up.textBox_Diff[17];
                textBox19_Diff.Text = up.textBox_Diff[18];
                textBox20_Diff.Text = up.textBox_Diff[19];

                textBox_delay_time_Diff.Text = up.textBox_delay_time_Diff;
                textBox_Ave_Lv_Limit_Diff.Text = up.textBox_Ave_Lv_Limit_Diff;

                textBox_GCS_Diff_Min_Point.Text = up.textBox_GCS_Diff_Min_Point;
                textBox_GCS_Diff_Max_Point.Text = up.textBox_GCS_Diff_Max_Point;

                radioButton_Diff_1st_and_3rd.Checked = up.radioButton_Diff_1st_and_3rd;
                radioButton_Diff_2nd_and_3rd.Checked = up.radioButton_Diff_2nd_and_3rd;
                radioButton_Diff_1st_and_2nd.Checked = up.radioButton_Diff_1st_and_2nd;
                radioButton_Diff_1st_2nd_3rd.Checked = up.radioButton_Diff_1st_2nd_3rd;

                radioButton_SH_Diff_Step_16.Checked = up.radioButton_SH_Diff_Step_16;
                radioButton_SH_Diff_Step_8.Checked = up.radioButton_SH_Diff_Step_8;
                radioButton_SH_Diff_Step_4.Checked = up.radioButton_SH_Diff_Step_4;
                //------------------------------------

                //-------BCS Diff Measure Related--------
                textBox_delay_time_Diff_BCS.Text = up.textBox_delay_time_Diff_BCS;

                BCS_Diff_step_value_1_Range1.Checked = up.BCS_Diff_step_value_1_Range1;
                BCS_Diff_step_value_4_Range1.Checked = up.BCS_Diff_step_value_4_Range1;
                BCS_Diff_step_value_8_Range1.Checked = up.BCS_Diff_step_value_8_Range1;
                BCS_Diff_step_value_16_Range1.Checked = up.BCS_Diff_step_value_16_Range1;

                BCS_Diff_step_value_1_Range2.Checked = up.BCS_Diff_step_value_1_Range2;
                BCS_Diff_step_value_4_Range2.Checked = up.BCS_Diff_step_value_4_Range2;
                BCS_Diff_step_value_8_Range2.Checked = up.BCS_Diff_step_value_8_Range2;
                BCS_Diff_step_value_16_Range2.Checked = up.BCS_Diff_step_value_16_Range2;

                BCS_Diff_step_value_1_Range3.Checked = up.BCS_Diff_step_value_1_Range3;
                BCS_Diff_step_value_4_Range3.Checked = up.BCS_Diff_step_value_4_Range3;
                BCS_Diff_step_value_8_Range3.Checked = up.BCS_Diff_step_value_8_Range3;
                BCS_Diff_step_value_16_Range3.Checked = up.BCS_Diff_step_value_16_Range3;

                checkBox1_BCS_Diff_Gray_P1.Checked = up.CheckBox_BCS_Diff_Gray_Points[0];
                checkBox1_BCS_Diff_Gray_P2.Checked = up.CheckBox_BCS_Diff_Gray_Points[1];
                checkBox1_BCS_Diff_Gray_P3.Checked = up.CheckBox_BCS_Diff_Gray_Points[2];
                checkBox1_BCS_Diff_Gray_P4.Checked = up.CheckBox_BCS_Diff_Gray_Points[3];
                checkBox1_BCS_Diff_Gray_P5.Checked = up.CheckBox_BCS_Diff_Gray_Points[4];
                checkBox1_BCS_Diff_Gray_P6.Checked = up.CheckBox_BCS_Diff_Gray_Points[5];
                checkBox1_BCS_Diff_Gray_P7.Checked = up.CheckBox_BCS_Diff_Gray_Points[6];
                checkBox1_BCS_Diff_Gray_P8.Checked = up.CheckBox_BCS_Diff_Gray_Points[7];
                checkBox1_BCS_Diff_Gray_P9.Checked = up.CheckBox_BCS_Diff_Gray_Points[8];
                checkBox1_BCS_Diff_Gray_P10.Checked = up.CheckBox_BCS_Diff_Gray_Points[9];
                checkBox1_BCS_Diff_Gray_P11.Checked = up.CheckBox_BCS_Diff_Gray_Points[10];

                textBox_BCS_Diff_Gray_P1.Text = up.textBox_BCS_Diff_Gray_Points[0];
                textBox_BCS_Diff_Gray_P2.Text = up.textBox_BCS_Diff_Gray_Points[1];
                textBox_BCS_Diff_Gray_P3.Text = up.textBox_BCS_Diff_Gray_Points[2];
                textBox_BCS_Diff_Gray_P4.Text = up.textBox_BCS_Diff_Gray_Points[3];
                textBox_BCS_Diff_Gray_P5.Text = up.textBox_BCS_Diff_Gray_Points[4];
                textBox_BCS_Diff_Gray_P6.Text = up.textBox_BCS_Diff_Gray_Points[5];
                textBox_BCS_Diff_Gray_P7.Text = up.textBox_BCS_Diff_Gray_Points[6];
                textBox_BCS_Diff_Gray_P8.Text = up.textBox_BCS_Diff_Gray_Points[7];
                textBox_BCS_Diff_Gray_P9.Text = up.textBox_BCS_Diff_Gray_Points[8];
                textBox_BCS_Diff_Gray_P10.Text = up.textBox_BCS_Diff_Gray_Points[9];
                textBox_BCS_Diff_Gray_P11.Text = up.textBox_BCS_Diff_Gray_Points[10];

                radioButton_BCS_Diff_Dual.Checked = up.radioButton_BCS_Diff_Dual;
                radioButton_BCS_Diff_Triple.Checked = up.radioButton_BCS_Diff_Triple;

                radioButton_BCS_Diff_Single.Checked = up.radioButton_BCS_Diff_Single;

                textBox_BCS_Diff_Sub_1_Min_Point.Text = up.textBox_BCS_Diff_Sub_1_Min_Point;
                textBox_BCS_Diff_Sub_2_Min_Point.Text = up.textBox_BCS_Diff_Sub_2_Min_Point;
                textBox_BCS_Diff_Sub_3_Min_Point.Text = up.textBox_BCS_Diff_Sub_3_Min_Point;
                textBox_BCS_Diff_Sub_1_Max_Point.Text = up.textBox_BCS_Diff_Sub_1_Max_Point;
                textBox_BCS_Diff_Sub_2_Max_Point.Text = up.textBox_BCS_Diff_Sub_2_Max_Point;
                textBox_BCS_Diff_Sub_3_Max_Point.Text = up.textBox_BCS_Diff_Sub_3_Max_Point;

                checkBox_BCS_Diif_Range_1.Checked = up.checkBox_BCS_Diif_Range_1;
                checkBox_BCS_Diif_Range_2.Checked = up.checkBox_BCS_Diif_Range_2;
                checkBox_BCS_Diif_Range_3.Checked = up.checkBox_BCS_Diif_Range_3;
                //---------------------------------------

                //-------All_At_Once Measure Related---------
                checkBox_All_At_Once_E3.Checked = up.checkBox_All_At_Once_E3;
                checkBox_All_At_Once_E2.Checked = up.checkBox_All_At_Once_E2;
                checkBox_All_At_Once_Diff_GCS.Checked = up.checkBox_All_At_Once_Diff;
                //-------------------------------------------

                //-----AOD GCS Measure Related------
                //Textbox to String
                textBox_AOD_DBV1.Text = up.textBox_AOD_DBV1;
                textBox_AOD_DBV2.Text = up.textBox_AOD_DBV2;
                textBox_AOD_DBV3.Text = up.textBox_AOD_DBV3;
                textBox_AOD_DBV4.Text = up.textBox_AOD_DBV4;
                textBox_AOD_DBV5.Text = up.textBox_AOD_DBV5;
                textBox_AOD_DBV6.Text = up.textBox_AOD_DBV6;

                //CheckBox to Bool
                checkBox_AOD_DBV1.Checked = up.checkBox_AOD_DBV1;
                checkBox_AOD_DBV2.Checked = up.checkBox_AOD_DBV2;
                checkBox_AOD_DBV3.Checked = up.checkBox_AOD_DBV3;
                checkBox_AOD_DBV4.Checked = up.checkBox_AOD_DBV4;
                checkBox_AOD_DBV5.Checked = up.checkBox_AOD_DBV5;
                checkBox_AOD_DBV6.Checked = up.checkBox_AOD_DBV6;
                checkBox_All_At_Once_AOD_GCS.Checked = up.checkBox_All_At_Once_AOD_GCS;
                //-------------------------------

                //----- Gamma Crush ------
                checkBox_Gamma_Crush_Conditon_1.Checked = up.checkBox_Gamma_Crush_Conditon_1;
                checkBox_Gamma_Crush_Conditon_2.Checked = up.checkBox_Gamma_Crush_Conditon_2;
                checkBox_Gamma_Crush_Conditon_3.Checked = up.checkBox_Gamma_Crush_Conditon_3;

                checkBox_Gamma_Crush_W.Checked = up.checkBox_Gamma_Crush_W;
                checkBox_Gamma_Crush_R.Checked = up.checkBox_Gamma_Crush_R;
                checkBox_Gamma_Crush_G.Checked = up.checkBox_Gamma_Crush_G;
                checkBox_Gamma_Crush_B.Checked = up.checkBox_Gamma_Crush_B;
                checkBox_Gamma_Crush_GB.Checked = up.checkBox_Gamma_Crush_GB;
                checkBox_Gamma_Crush_RB.Checked = up.checkBox_Gamma_Crush_RB;
                checkBox_Gamma_Crush_RG.Checked = up.checkBox_Gamma_Crush_RG;

                checkBox_Gamma_Crush_P1.Checked = up.checkBox_Gamma_Crush[0];
                checkBox_Gamma_Crush_P2.Checked = up.checkBox_Gamma_Crush[1];
                checkBox_Gamma_Crush_P3.Checked = up.checkBox_Gamma_Crush[2];
                checkBox_Gamma_Crush_P4.Checked = up.checkBox_Gamma_Crush[3];
                checkBox_Gamma_Crush_P5.Checked = up.checkBox_Gamma_Crush[4];
                checkBox_Gamma_Crush_P6.Checked = up.checkBox_Gamma_Crush[5];
                checkBox_Gamma_Crush_P7.Checked = up.checkBox_Gamma_Crush[6];
                checkBox_Gamma_Crush_P8.Checked = up.checkBox_Gamma_Crush[7];
                checkBox_Gamma_Crush_P9.Checked = up.checkBox_Gamma_Crush[8];
                checkBox_Gamma_Crush_P10.Checked = up.checkBox_Gamma_Crush[9];

                textBox_Gamma_Crush_DBV1.Text = up.textBox_Gamma_Crush_DBV[0];
                textBox_Gamma_Crush_DBV2.Text = up.textBox_Gamma_Crush_DBV[1];
                textBox_Gamma_Crush_DBV3.Text = up.textBox_Gamma_Crush_DBV[2];
                textBox_Gamma_Crush_DBV4.Text = up.textBox_Gamma_Crush_DBV[3];
                textBox_Gamma_Crush_DBV5.Text = up.textBox_Gamma_Crush_DBV[4];
                textBox_Gamma_Crush_DBV6.Text = up.textBox_Gamma_Crush_DBV[5];
                textBox_Gamma_Crush_DBV7.Text = up.textBox_Gamma_Crush_DBV[6];
                textBox_Gamma_Crush_DBV8.Text = up.textBox_Gamma_Crush_DBV[7];
                textBox_Gamma_Crush_DBV9.Text = up.textBox_Gamma_Crush_DBV[8];
                textBox_Gamma_Crush_DBV10.Text = up.textBox_Gamma_Crush_DBV[9];

                textBox_Gamma_Crush_Gray_1.Text = up.textBox_Gamma_Crush_Gray[0];
                textBox_Gamma_Crush_Gray_2.Text = up.textBox_Gamma_Crush_Gray[1];
                textBox_Gamma_Crush_Gray_3.Text = up.textBox_Gamma_Crush_Gray[2];
                textBox_Gamma_Crush_Gray_4.Text = up.textBox_Gamma_Crush_Gray[3];
                textBox_Gamma_Crush_Gray_5.Text = up.textBox_Gamma_Crush_Gray[4];
                textBox_Gamma_Crush_Gray_6.Text = up.textBox_Gamma_Crush_Gray[5];
                textBox_Gamma_Crush_Gray_7.Text = up.textBox_Gamma_Crush_Gray[6];
                textBox_Gamma_Crush_Gray_8.Text = up.textBox_Gamma_Crush_Gray[7];
                textBox_Gamma_Crush_Gray_9.Text = up.textBox_Gamma_Crush_Gray[8];
                textBox_Gamma_Crush_Gray_10.Text = up.textBox_Gamma_Crush_Gray[9];
                //-----------------------

                radioButton_GCS_White_PTN.Checked = up.radioButton_GCS_White_PTN;
                radioButton_GCS_Red_PTN.Checked = up.radioButton_GCS_Red_PTN;
                radioButton_GCS_Green_PTN.Checked = up.radioButton_GCS_Green_PTN;
                radioButton_GCS_Blue_PTN.Checked = up.radioButton_GCS_Blue_PTN;

                myFileStream.Close();
                System.Windows.Forms.MessageBox.Show("Setting has been Loaded (File Date : " + up.Saved_Date + ")");
            }
            else
            {
                myFileStream = null;
                System.Windows.Forms.MessageBox.Show("Nothing has been Loaded");
            }
            //------------------------
            Button_Click_Enable(true);
        }

        private void DBV_CheckBox_Status_Update(object sender, EventArgs e)
        {
            //Condition1
            checkBox1_1_CheckedChanged(sender, e);
            checkBox2_1_CheckedChanged(sender, e);
            checkBox3_1_CheckedChanged(sender, e);
            checkBox4_1_CheckedChanged(sender, e);
            checkBox5_1_CheckedChanged(sender, e);
            checkBox6_1_CheckedChanged(sender, e);
            checkBox7_1_CheckedChanged(sender, e);
            checkBox8_1_CheckedChanged(sender, e);
            checkBox9_1_CheckedChanged(sender, e);
            checkBox10_1_CheckedChanged(sender, e);
            checkBox11_1_CheckedChanged(sender, e);
            checkBox12_1_CheckedChanged(sender, e);
            checkBox13_1_CheckedChanged(sender, e);
            checkBox14_1_CheckedChanged(sender, e);
            checkBox15_1_CheckedChanged(sender, e);
            checkBox16_1_CheckedChanged(sender, e);

            //Condition2
            checkBox1_2_CheckedChanged(sender, e);
            checkBox2_2_CheckedChanged(sender, e);
            checkBox3_2_CheckedChanged(sender, e);
            checkBox4_2_CheckedChanged(sender, e);
            checkBox5_2_CheckedChanged(sender, e);
            checkBox6_2_CheckedChanged(sender, e);
            checkBox7_2_CheckedChanged(sender, e);
            checkBox8_2_CheckedChanged(sender, e);
            checkBox9_2_CheckedChanged(sender, e);
            checkBox10_2_CheckedChanged(sender, e);
            checkBox11_2_CheckedChanged(sender, e);
            checkBox12_2_CheckedChanged(sender, e);
            checkBox13_2_CheckedChanged(sender, e);
            checkBox14_2_CheckedChanged(sender, e);
            checkBox15_2_CheckedChanged(sender, e);
            checkBox16_2_CheckedChanged(sender, e);

            //Condition3
            checkBox1_3_CheckedChanged(sender, e);
            checkBox2_3_CheckedChanged(sender, e);
            checkBox3_3_CheckedChanged(sender, e);
            checkBox4_3_CheckedChanged(sender, e);
            checkBox5_3_CheckedChanged(sender, e);
            checkBox6_3_CheckedChanged(sender, e);
            checkBox7_3_CheckedChanged(sender, e);
            checkBox8_3_CheckedChanged(sender, e);
            checkBox9_3_CheckedChanged(sender, e);
            checkBox10_3_CheckedChanged(sender, e);
            checkBox11_3_CheckedChanged(sender, e);
            checkBox12_3_CheckedChanged(sender, e);
            checkBox13_3_CheckedChanged(sender, e);
            checkBox14_3_CheckedChanged(sender, e);
            checkBox15_3_CheckedChanged(sender, e);
            checkBox16_3_CheckedChanged(sender, e);

            //Diff GCS
            checkBox1_Diff_CheckedChanged(sender, e);
            checkBox2_Diff_CheckedChanged(sender, e);
            checkBox3_Diff_CheckedChanged(sender, e);
            checkBox4_Diff_CheckedChanged(sender, e);
            checkBox5_Diff_CheckedChanged(sender, e);
            checkBox6_Diff_CheckedChanged(sender, e);
            checkBox7_Diff_CheckedChanged(sender, e);
            checkBox8_Diff_CheckedChanged(sender, e);
            checkBox9_Diff_CheckedChanged(sender, e);
            checkBox10_Diff_CheckedChanged(sender, e);
            checkBox11_Diff_CheckedChanged(sender, e);
            checkBox12_Diff_CheckedChanged(sender, e);
            checkBox13_Diff_CheckedChanged(sender, e);
            checkBox14_Diff_CheckedChanged(sender, e);
            checkBox15_Diff_CheckedChanged(sender, e);
            checkBox16_Diff_CheckedChanged(sender, e);
            checkBox17_Diff_CheckedChanged(sender, e);
            checkBox18_Diff_CheckedChanged(sender, e);
            checkBox19_Diff_CheckedChanged(sender, e);
            checkBox20_Diff_CheckedChanged(sender, e);

            //Diff BCS
            checkBox1_BCS_Diff_Gray_P1_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P2_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P3_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P4_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P5_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P6_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P7_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P8_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P9_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P10_CheckedChanged(sender, e);
            checkBox1_BCS_Diff_Gray_P11_CheckedChanged(sender, e);
            
            //AOD GCS
            checkBox_AOD_DBV1_CheckedChanged(sender, e);
            checkBox_AOD_DBV2_CheckedChanged(sender, e);
            checkBox_AOD_DBV3_CheckedChanged(sender, e);
            checkBox_AOD_DBV4_CheckedChanged(sender, e);
            checkBox_AOD_DBV5_CheckedChanged(sender, e);
            checkBox_AOD_DBV6_CheckedChanged(sender, e);

            //GammaCrush
            checkBox_Gamma_Crush_P1_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P2_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P3_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P4_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P5_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P6_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P7_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P8_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P9_CheckedChanged(sender, e);
            checkBox_Gamma_Crush_P10_CheckedChanged(sender, e);
        }

        private void CheckBox_CheckedChanged(CheckBox checkbox, TextBox textbox)
        {
            if (checkbox.Checked == false) textbox.BackColor = Color.Black;
            else textbox.BackColor = Color.White;
        }

        private void CheckBox_CheckedChanged(CheckBox checkbox, TextBox textbox1, TextBox textbox2)
        {
            if (checkbox.Checked == false)
            {
                textbox1.BackColor = Color.Black;
                textbox2.BackColor = Color.Black;
            }
            else
            {
                textbox1.BackColor = Color.White;
                textbox2.BackColor = Color.White;
            }
        }

        private void checkBox1_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_1, textBox1_1);
        }



        private void checkBox2_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox2_1, textBox2_1);
        }

        private void checkBox3_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox3_1, textBox3_1);
        }

        private void checkBox4_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox4_1, textBox4_1);
        }

        private void checkBox5_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox5_1, textBox5_1);
        }

        private void checkBox6_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox6_1, textBox6_1);
        }

        private void checkBox7_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox7_1, textBox7_1);
        }

        private void checkBox8_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox8_1, textBox8_1);
        }

        private void checkBox9_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox9_1, textBox9_1);
        }

        private void checkBox10_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox10_1, textBox10_1);
        }

        private void checkBox11_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox11_1, textBox11_1);
        }

        private void checkBox12_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox12_1, textBox12_1);
        }

        private void checkBox13_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox13_1, textBox13_1);
        }

        private void checkBox14_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox14_1, textBox14_1);
        }

        private void checkBox15_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox15_1, textBox15_1);
        }

        private void checkBox16_1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox16_1, textBox16_1);
        }

        private void checkBox1_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_2, textBox1_2);
        }

        private void checkBox2_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox2_2, textBox2_2);
        }

        private void checkBox3_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox3_2, textBox3_2);
        }

        private void checkBox4_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox4_2, textBox4_2);
        }

        private void checkBox5_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox5_2, textBox5_2);
        }

        private void checkBox6_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox6_2, textBox6_2);
        }

        private void checkBox7_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox7_2, textBox7_2);
        }

        private void checkBox8_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox8_2, textBox8_2);
        }

        private void checkBox9_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox9_2, textBox9_2);
        }

        private void checkBox10_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox10_2, textBox10_2);
        }

        private void checkBox11_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox11_2, textBox11_2);
        }

        private void checkBox12_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox12_2, textBox12_2);
        }

        private void checkBox13_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox13_2, textBox13_2);
        }

        private void checkBox14_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox14_2, textBox14_2);
        }

        private void checkBox15_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox15_2, textBox15_2);
        }

        private void checkBox16_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox16_2, textBox16_2);
        }

        private void checkBox1_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_3, textBox1_3);
        }

        private void checkBox2_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox2_3, textBox2_3);
        }

        private void checkBox3_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox3_3, textBox3_3);
        }

        private void checkBox4_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox4_3, textBox4_3);
        }

        private void checkBox5_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox5_3, textBox5_3);
        }

        private void checkBox6_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox6_3, textBox6_3);
        }

        private void checkBox7_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox7_3, textBox7_3);
        }

        private void checkBox8_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox8_3, textBox8_3);
        }

        private void checkBox9_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox9_3, textBox9_3);
        }

        private void checkBox10_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox10_3, textBox10_3);
        }

        private void checkBox11_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox11_3, textBox11_3);
        }

        private void checkBox12_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox12_3, textBox12_3);
        }

        private void checkBox13_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox13_3, textBox13_3);
        }

        private void checkBox14_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox14_3, textBox14_3);
        }

        private void checkBox15_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox15_3, textBox15_3);
        }

        private void checkBox16_3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox16_3, textBox16_3);
        }

        private void Calculate_Delta_E2_From_x_y_Lv(int dbv_end_Point, int dbv_max_point, int Step_Value, int Condition = 1)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            DataGridView datagridview;
            if (Condition == 1) datagridview = dataGridView4;
            else if (Condition == 2) datagridview = dataGridView5;
            else if (Condition == 3) datagridview = dataGridView6;
            else datagridview = null;

            if (Availability_E2)
            {
                // x/y/Lv 계산 완료 후 -- > X Y Z 계산
                double x;
                double y;
                double Lv;

                double[] X_Array = new double[dbv_max_point + 1];//Maximum DBV_MAX !
                double[] Y_Array = new double[dbv_max_point + 1];//Maximum DBV_MAX !
                double[] Z_Array = new double[dbv_max_point + 1];//Maximum DBV_MAX !

                int count = 0;
                for (int dbv = dbv_end_Point; dbv < (dbv_max_point + Step_Value); )
                {


                    x = Convert.ToDouble(datagridview.Rows[count].Cells[1].Value);
                    y = Convert.ToDouble(datagridview.Rows[count].Cells[2].Value);
                    Lv = Convert.ToDouble(datagridview.Rows[count].Cells[3].Value);

                    X_Array[count] = (x / y) * Lv;
                    Y_Array[count] = Lv;
                    Z_Array[count] = ((1 - x - y) / y) * Lv;

                    f1.GB_Status_AppendText_Nextline("[dbv_end_Point]/dbv_max_point : " + (dbv_end_Point).ToString() + "/" + dbv_max_point.ToString(), Color.Blue, true);
                    f1.GB_Status_AppendText_Nextline("[count]X_Array/Y_Array/Z_Array : [" + (count).ToString() + "]"
                        + X_Array[count].ToString() + "/" + Y_Array[count].ToString() + "/" + Z_Array[count].ToString(), Color.Blue, true);

                    count++;

                    dbv += Step_Value;
                    if (dbv > dbv_max_point) break;
                }


                // X/Y/Z 계산 완료 후 --> L* / f(X/Xw) / f(Y/Yw) / f(Z/Zw) 
                double L, FX, FY, FZ;
                double X4095 = 0;
                double Y4095 = 0;
                double Z4095 = 0;


                if (radioButton_Min_to_Max_E2.Checked)
                {
                    X4095 = X_Array[count - 1];
                    Y4095 = Y_Array[count - 1];
                    Z4095 = Z_Array[count - 1];
                }
                else if (radioButton_Max_to_Min_E2.Checked)
                {
                    X4095 = X_Array[0];
                    Y4095 = Y_Array[0];
                    Z4095 = Z_Array[0];
                }
                else { }

                f1.GB_Status_AppendText_Nextline("X4095/Y4095/Z4095 : " + X4095.ToString() + "/" + Y4095.ToString() + "/" + Z4095.ToString(), Color.Red, true);

                //a* , b* , Delta E
                double a, b, Delta_E;
                double Max_Delta_E = 0;
                double X;
                double Y;
                double Z;

                count = 0;
                for (int dbv = dbv_end_Point; dbv < (dbv_max_point + Step_Value); )
                {
                    X = X_Array[count];
                    Y = Y_Array[count];
                    Z = Z_Array[count];

                    //Calculate L*
                    if (Y / Y4095 > 0.008856)
                    {
                        L = 116 * Math.Pow(Y / Y4095, 0.33333333) - 16;
                    }
                    else
                    {
                        L = 903.3 * (Y / Y4095);
                    }

                    //Calculate F(X/Xw)
                    if (X / X4095 > 0.008856)
                    {
                        FX = Math.Pow((X / X4095), 0.33333333);
                    }
                    else
                    {
                        FX = 7.787 * (X / X4095) + (16 / 116.0);
                    }

                    //Calculate F(Y/Yw)
                    if (Y / Y4095 > 0.008856)
                    {
                        FY = Math.Pow((Y / Y4095), 0.33333333);
                    }
                    else
                    {
                        FY = 7.787 * (Y / Y4095) + (16 / 116.0);
                    }

                    //Calculate F(Z/Zw)
                    if (Z / Z4095 > 0.008856)
                    {
                        FZ = Math.Pow((Z / Z4095), 0.33333333);
                    }
                    else
                    {
                        FZ = 7.787 * (Z / Z4095) + (16 / 116.0);
                    }
                    //Calculate a* , b* , Delta E
                    a = 500 * (FX - FY);
                    b = 200 * (FY - FZ);
                    Delta_E = Math.Pow((Math.Pow(a, 2) + Math.Pow(b, 2)), 0.5);

                    datagridview.Rows[count].Cells[4].Value = Delta_E; //Delta E
                    if (Max_Delta_E <= Delta_E) Max_Delta_E = Delta_E;


                    count++;

                    dbv += Step_Value;
                    if (dbv > dbv_max_point) break;
                }
                //Excel 에 Data 남기기 위한 자료 추가.

                datagridview.Rows.Add("Delta E2"); // 한열은 띄어쓰기로
                datagridview.Rows.Add(Max_Delta_E.ToString());
            }
        }

        private void Delta_E2_Textbox_CheckBox_Radiobutton_Enable(bool Able)
        {
            checkBox_1st_Condition_Measure_E2.Enabled = Able;
            checkBox_2nd_Condition_Measure_E2.Enabled = Able;
            checkBox_3rd_Condition_Measure_E2.Enabled = Able;
            textBox_delay_time_E2.Enabled = Able;
            textBox_Delta_E2_End_Point.Enabled = Able;
            textBox_Delta_E2_Max_Point.Enabled = Able;
            step_value_1.Enabled = Able;
            step_value_4.Enabled = Able;
            step_value_8.Enabled = Able;
            step_value_16.Enabled = Able;
            radioButton_Min_to_Max_E2.Enabled = Able;
            radioButton_Max_to_Min_E2.Enabled = Able;
            checkBox_Ave_Measure_E2.Enabled = Able;
            textBox_Ave_Lv_Limit_E2.Enabled = Able;
        }


        private void Delta_E2_calculation_btn_Click(object sender, EventArgs e)
        {
            button_Stop.PerformClick();
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            if (f1.label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                Button_Click_Enable(false);
                Delta_E2_Textbox_CheckBox_Radiobutton_Enable(false);

                //Set ProgressBar_E2 Max and Step and Value
                progressBar_E2.Value = 0;
                progressBar_E2.Step = 1;
                progressBar_E2.Maximum = 1;
                if (checkBox_1st_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                if (checkBox_2nd_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                if (checkBox_3rd_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                progressBar_E2.PerformStep();

                int Step_Value = 0;
                if (step_value_1.Checked) Step_Value = 1;
                else if (step_value_4.Checked) Step_Value = 4;
                else if (step_value_8.Checked) Step_Value = 8;
                else if (step_value_16.Checked) Step_Value = 16;
                else System.Windows.Forms.MessageBox.Show("It's impossible(Delta E2)");

                //Set DBV_End_Point
                int dbv_max_point = Convert.ToInt16(textBox_Delta_E2_Max_Point.Text);
                if (dbv_max_point >= f1.Get_DBV_TrackBar_Maximum()) dbv_max_point = f1.Get_DBV_TrackBar_Maximum();
                else if (dbv_max_point <= 0) dbv_max_point = 0;
                textBox_Delta_E2_Max_Point.Text = dbv_max_point.ToString();

                int dbv_end_Point = Convert.ToInt16(textBox_Delta_E2_End_Point.Text);
                if (dbv_end_Point >= (dbv_max_point - 1)) dbv_end_Point = (dbv_max_point - 1);
                else if (dbv_end_Point <= 0) dbv_end_Point = 0;
                textBox_Delta_E2_End_Point.Text = dbv_end_Point.ToString();

                //Set Availability and delay_time_between_measurement
                Availability_E2 = true;
                int delay_time_between_measurement = Convert.ToInt16(textBox_delay_time_E2.Text);

                if (checkBox_1st_Condition_Measure_E2.Checked) dataGridView4.Rows.Clear();
                if (checkBox_2nd_Condition_Measure_E2.Checked) dataGridView5.Rows.Clear();
                if (checkBox_3rd_Condition_Measure_E2.Checked) dataGridView6.Rows.Clear();

                if (checkBox_1st_Condition_Measure_E2.Checked)
                {
                    dataGridView4.Columns[0].HeaderText = "DBV";
                    Script_Apply_For_Condition1();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1.PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    Optic_SH_Delta_E2_Measure(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, 1); //Condition1
                    Calculate_Delta_E2_From_x_y_Lv(dbv_end_Point, dbv_max_point, Step_Value, 1);
                    progressBar_E2.PerformStep();
                }

                if (checkBox_2nd_Condition_Measure_E2.Checked)
                {
                    dataGridView5.Columns[0].HeaderText = "DBV";
                    Script_Apply_For_Condition2();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1.PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    Optic_SH_Delta_E2_Measure(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, 2); //Condition2
                    Calculate_Delta_E2_From_x_y_Lv(dbv_end_Point, dbv_max_point, Step_Value, 2);
                    progressBar_E2.PerformStep();
                }

                if (checkBox_3rd_Condition_Measure_E2.Checked)
                {
                    dataGridView6.Columns[0].HeaderText = "DBV";
                    Script_Apply_For_Condition3();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1.PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    Optic_SH_Delta_E2_Measure(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, 3); //Condition3
                    Calculate_Delta_E2_From_x_y_Lv(dbv_end_Point, dbv_max_point, Step_Value, 3);
                    progressBar_E2.PerformStep();
                }
                Button_Click_Enable(true);
                Delta_E2_Textbox_CheckBox_Radiobutton_Enable(true);
            }
        }

        private void checkBox1_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_Diff, textBox1_Diff);
        }

        private void checkBox2_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox2_Diff, textBox2_Diff);
        }

        private void checkBox3_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox3_Diff, textBox3_Diff);
        }

        private void checkBox4_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox4_Diff, textBox4_Diff);
        }

        private void checkBox5_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox5_Diff, textBox5_Diff);
        }

        private void checkBox6_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox6_Diff, textBox6_Diff);
        }

        private void checkBox7_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox7_Diff, textBox7_Diff);
        }

        private void checkBox8_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox8_Diff, textBox8_Diff);
        }

        private void checkBox9_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox9_Diff, textBox9_Diff);
        }

        private void checkBox10_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox10_Diff, textBox10_Diff);
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void textBox_delay_After_Condition_1_TextChanged(object sender, EventArgs e)
        {

        }




        private void Diff_Textbox_CheckBox_Radiobutton_Enable(bool Able)
        {
            radioButton_Diff_1st_and_2nd.Enabled = Able;
            radioButton_Diff_2nd_and_3rd.Enabled = Able;
            radioButton_Diff_1st_and_3rd.Enabled = Able;
            radioButton_Diff_1st_2nd_3rd.Enabled = Able;
            checkBox_Ave_Measure_Diff.Enabled = Able;
            textBox_Ave_Lv_Limit_Diff.Enabled = Able;
            textBox_delay_time_Diff.Enabled = Able;

            checkBox1_Diff.Enabled = Able;
            checkBox2_Diff.Enabled = Able;
            checkBox3_Diff.Enabled = Able;
            checkBox4_Diff.Enabled = Able;
            checkBox5_Diff.Enabled = Able;
            checkBox6_Diff.Enabled = Able;
            checkBox7_Diff.Enabled = Able;
            checkBox8_Diff.Enabled = Able;
            checkBox9_Diff.Enabled = Able;
            checkBox10_Diff.Enabled = Able;

            textBox1_Diff.Enabled = Able;
            textBox2_Diff.Enabled = Able;
            textBox3_Diff.Enabled = Able;
            textBox4_Diff.Enabled = Able;
            textBox5_Diff.Enabled = Able;
            textBox6_Diff.Enabled = Able;
            textBox7_Diff.Enabled = Able;
            textBox8_Diff.Enabled = Able;
            textBox9_Diff.Enabled = Able;
            textBox10_Diff.Enabled = Able;

            radioButton_SH_Diff_Step_4.Enabled = Able;
            radioButton_SH_Diff_Step_8.Enabled = Able;
            radioButton_SH_Diff_Step_16.Enabled = Able;
        }

        private int Get_Updated_Gray(int gray)
        {
            if (gray > 255)
                return 255;
            if (gray < 0)
                return 0;

            return gray;
        }

        private void button_SH_Difference_Measure_Click(object sender, EventArgs e)
        {
            button_Stop.PerformClick();
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            if (f1.label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                int Gray_Max = Get_Updated_Gray(Convert.ToInt32(textBox_GCS_Diff_Max_Point.Text));
                textBox_GCS_Diff_Max_Point.Text = Gray_Max.ToString();

                int Gray_Min = Get_Updated_Gray(Convert.ToInt32(textBox_GCS_Diff_Min_Point.Text));
                textBox_GCS_Diff_Min_Point.Text = Gray_Min.ToString();

                Button_Click_Enable(false);
                Diff_Textbox_CheckBox_Radiobutton_Enable(false);
                Update_Diff_DBV_Checkbox_And_Textbox();


                dataGridView7.Rows.Clear();
                dataGridView8.Rows.Clear();
                dataGridView9.Rows.Clear();

                //step
                int step = 1;
                if (radioButton_SH_Diff_Step_4.Checked) step = 4;
                else if (radioButton_SH_Diff_Step_8.Checked) step = 8;
                else if (radioButton_SH_Diff_Step_16.Checked) step = 16;


                //delay time
                int delay_time_after_pattern = Convert.ToInt16(textBox_delay_time_Diff.Text);

                //Availability
                Availability_Diff = true;


                bool First_skip = false; if (radioButton_Diff_2nd_and_3rd.Checked) First_skip = true;
                bool Second_skip = false; if (radioButton_Diff_1st_and_3rd.Checked) Second_skip = true;
                bool Third_skip = false; if (radioButton_Diff_1st_and_2nd.Checked) Third_skip = true;

                //Set ProgressBar_E3 Max and Step and Value
                int Progress_Bar_Diff_Max = 0;
                for (int i = 0; i < 20; i++) if (checkBox_Diff_GCS_DBV[i]) Progress_Bar_Diff_Max++;
                progressBar_GCS_Diff.Value = 0;
                progressBar_GCS_Diff.Step = 1;
                progressBar_GCS_Diff.Maximum = 1;
                progressBar_GCS_Diff.Maximum += Progress_Bar_Diff_Max;
                progressBar_GCS_Diff.PerformStep();

                for (int i = 0; i < 20; i++)
                {
                    if (checkBox_Diff_GCS_DBV[i])
                    {
                        string DBV = textBox_Diff_GCS_DBV[i].PadLeft(3, '0');//dex to hex (as a string form)
                        try
                        {
                            f1.DBV_Setting(DBV);

                            if (First_skip == false) dataGridView7.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");
                            if (Second_skip == false) dataGridView8.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");
                            if (Third_skip == false) dataGridView9.Rows.Add(i.ToString() + ")DBV", DBV, "-", "-");

                            f1.GB_Status_AppendText_Nextline(i.ToString() + ")Diff DBV[" + DBV + "] was applied", Color.Blue);
                        }
                        catch
                        {
                            f1.GB_Status_AppendText_Nextline(i.ToString() + ")Diff DBV[" + DBV + "] was failed", Color.Red);
                        }
                        Optic_Dual_SH_Difference_Measure_By_Step(Gray_Max, Gray_Min,delay_time_after_pattern, step, First_skip, Second_skip, Third_skip);
                        progressBar_GCS_Diff.PerformStep();
                    }
                    else
                    {
                        f1.GB_Status_AppendText_Nextline(i.ToString() + ")Diff DBV Point Skip", Color.Black);
                    }
                }
                Button_Click_Enable(true);
                Diff_Textbox_CheckBox_Radiobutton_Enable(true);
            }
        }


        private void Optic_Dual_SH_Difference_Measure_By_Step(int Gray_Max, int Gray_Min, int delay_time_after_pattern, int step, bool First_skip, bool Second_skip, bool Third_skip)
        {
            XYLv[] Measure = new XYLv[5];
            int Max_Index = 0; double Max_Value = 0;
            int Min_Index = 0; double Min_Value = 2000;

            bool First_Step = true;
            for (int gray = Gray_Max; gray >= Gray_Min;)
            {
                if (Availability_Diff == false) break;
                Optic_Measurement_Pattern_Update(gray);
                f1().PTN_update(gray, gray, gray);

                Thread.Sleep(delay_time_after_pattern);
                try
                {
                    {//Condition 1
                        if (First_skip == false)
                        {
                            Script_Apply_For_Condition1();
                            Max_Index = 0; Min_Index = 0;
                            Max_Value = 0; Min_Value = 2000;
                            f1().isMsr = true;

                            f1().CA_Measure_button.Enabled = false;
                            f1().objCa.Measure();

                            if (checkBox_Ave_Measure_Diff.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_Diff.Text))
                            {
                                f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_Diff.Text, Color.Blue);

                                for (int a = 0; a < 5; a++)
                                {
                                    f1().objCa.Measure();
                                    Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                    Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                    Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");

                                    Measure[a].Double_Update_From_String();

                                    if (Measure[a].double_Lv < Min_Value)
                                    {
                                        Min_Value = Measure[a].double_Lv;
                                        Min_Index = a;
                                    }

                                    if (Measure[a].double_Lv > Max_Value)
                                    {
                                        Max_Value = Measure[a].double_Lv;
                                        Max_Index = a;
                                    }
                                }

                                int count = 0;
                                XYLv Sum_Measure = new XYLv();
                                XYLv Ave_Measure = new XYLv();
                                Sum_Measure.Set_Value(0, 0, 0);
                                Ave_Measure.Set_Value(0, 0, 0);

                                for (int a = 0; a < 5; a++)
                                {
                                    if (a == Max_Index || a == Min_Index)
                                    {
                                    }
                                    else
                                    {
                                        Sum_Measure.double_X += Measure[a].double_X;
                                        Sum_Measure.double_Y += Measure[a].double_Y;
                                        Sum_Measure.double_Lv += Measure[a].double_Lv;
                                        count++;
                                    }
                                }
                                Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                                Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                                Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                                Ave_Measure.String_Update_From_Double();
                                f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                                f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                                f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                                f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                                f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                            }

                            else
                            {
                                f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                            }

                            System.Windows.Forms.Application.DoEvents();

                            if (f1().isMsr == false)
                                return;

                            f1().CA_Measure_button.Enabled = true;

                            //Data Grid setting//////////////////////
                            dataGridView7.DataSource = null; // reset (unbind the datasource)
                            dataGridView7.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                            dataGridView7.FirstDisplayedScrollingRowIndex = dataGridView7.RowCount - 1;
                        }
                    }

                    { //Condition 2
                        if (Second_skip == false)
                        {
                            Script_Apply_For_Condition2();
                            Max_Index = 0; Min_Index = 0;
                            Max_Value = 0; Min_Value = 2000;
                            f1().isMsr = true;

                            f1().CA_Measure_button.Enabled = false;
                            f1().objCa.Measure();

                            if (checkBox_Ave_Measure_Diff.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_Diff.Text))
                            {
                                f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_Diff.Text, Color.Blue);

                                for (int a = 0; a < 5; a++)
                                {
                                    f1().objCa.Measure();
                                    Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                    Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                    Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");

                                    Measure[a].Double_Update_From_String();

                                    if (Measure[a].double_Lv < Min_Value)
                                    {
                                        Min_Value = Measure[a].double_Lv;
                                        Min_Index = a;
                                    }

                                    if (Measure[a].double_Lv > Max_Value)
                                    {
                                        Max_Value = Measure[a].double_Lv;
                                        Max_Index = a;
                                    }
                                }

                                int count = 0;
                                XYLv Sum_Measure = new XYLv();
                                XYLv Ave_Measure = new XYLv();
                                Sum_Measure.Set_Value(0, 0, 0);
                                Ave_Measure.Set_Value(0, 0, 0);

                                for (int a = 0; a < 5; a++)
                                {
                                    if (a == Max_Index || a == Min_Index)
                                    {
                                    }
                                    else
                                    {
                                        Sum_Measure.double_X += Measure[a].double_X;
                                        Sum_Measure.double_Y += Measure[a].double_Y;
                                        Sum_Measure.double_Lv += Measure[a].double_Lv;
                                        count++;
                                    }
                                }
                                Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                                Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                                Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                                Ave_Measure.String_Update_From_Double();
                                f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                                f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                                f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                                f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                                f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');

                            }

                            else
                            {
                                f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                            }

                            System.Windows.Forms.Application.DoEvents();

                            if (f1().isMsr == false)
                                return;

                            f1().CA_Measure_button.Enabled = true;

                            //Data Grid setting//////////////////////
                            dataGridView8.DataSource = null; // reset (unbind the datasource)
                            dataGridView8.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                            dataGridView8.FirstDisplayedScrollingRowIndex = dataGridView8.RowCount - 1;
                        }
                    }

                    { //Condition 3
                        if (Third_skip == false)
                        {
                            Script_Apply_For_Condition3();

                            Max_Index = 0; Min_Index = 0;
                            Max_Value = 0; Min_Value = 2000;
                            f1().isMsr = true;

                            f1().CA_Measure_button.Enabled = false;
                            f1().objCa.Measure();

                            if (checkBox_Ave_Measure_Diff.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_Diff.Text))
                            {
                                f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_Diff.Text, Color.Blue);

                                for (int a = 0; a < 5; a++)
                                {
                                    f1().objCa.Measure();
                                    Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                    Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                    Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");

                                    Measure[a].Double_Update_From_String();

                                    if (Measure[a].double_Lv < Min_Value)
                                    {
                                        Min_Value = Measure[a].double_Lv;
                                        Min_Index = a;
                                    }

                                    if (Measure[a].double_Lv > Max_Value)
                                    {
                                        Max_Value = Measure[a].double_Lv;
                                        Max_Index = a;
                                    }
                                }

                                int count = 0;
                                XYLv Sum_Measure = new XYLv();
                                XYLv Ave_Measure = new XYLv();
                                Sum_Measure.Set_Value(0, 0, 0);
                                Ave_Measure.Set_Value(0, 0, 0);

                                for (int a = 0; a < 5; a++)
                                {
                                    if (a == Max_Index || a == Min_Index)
                                    {
                                    }
                                    else
                                    {
                                        Sum_Measure.double_X += Measure[a].double_X;
                                        Sum_Measure.double_Y += Measure[a].double_Y;
                                        Sum_Measure.double_Lv += Measure[a].double_Lv;
                                        count++;
                                    }
                                }
                                Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                                Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                                Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                                Ave_Measure.String_Update_From_Double();
                                f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                                f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                                f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                                f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                                f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                                f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');

                            }

                            else
                            {
                                f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                            }

                            System.Windows.Forms.Application.DoEvents();

                            if (f1().isMsr == false)
                                return;

                            f1().CA_Measure_button.Enabled = true;

                            //Data Grid setting//////////////////////
                            dataGridView9.DataSource = null; // reset (unbind the datasource)
                            dataGridView9.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                            dataGridView9.FirstDisplayedScrollingRowIndex = dataGridView9.RowCount - 1;
                        }
                    }

                }
                catch (Exception er)
                {
                    f1().DisplayError(er);
                    System.Windows.Forms.Application.Exit();
                }

                if ((First_Step) && (step != 1))
                {
                    gray -= (step - 1);
                    First_Step = false;
                }
                else
                {
                    gray -= step;
                }
            }
        }

        private void All_At_Once_CheckBox_Enable(bool Able)
        {
            checkBox_All_At_Once_E3.Enabled = Able;
            checkBox_All_At_Once_E2.Enabled = Able;
            checkBox_All_At_Once_Diff_GCS.Enabled = Able;
            checkBox_All_At_Once_Diff_BCS.Enabled = Able;
        }
        private void button_All_At_Once_Click(object sender, EventArgs e)
        {
            All_At_Once_CheckBox_Enable(false);
            button_Clear.PerformClick();

            //Set ProgressBar_E3 Max and Step and Value
            int Progress_Bar_All_At_Once_Max = 0;
            if (checkBox_All_At_Once_E3.Checked) Progress_Bar_All_At_Once_Max++;
            if (checkBox_All_At_Once_E2.Checked) Progress_Bar_All_At_Once_Max++;
            if (checkBox_All_At_Once_Diff_GCS.Checked) Progress_Bar_All_At_Once_Max++;
            if (checkBox_All_At_Once_Diff_BCS.Checked) Progress_Bar_All_At_Once_Max++;
            if (checkBox_All_At_Once_AOD_GCS.Checked) Progress_Bar_All_At_Once_Max++;

            progressBar_All_At_Once.Value = 0;
            progressBar_All_At_Once.Step = 1;
            progressBar_All_At_Once.Maximum = 1;
            progressBar_All_At_Once.Maximum += Progress_Bar_All_At_Once_Max;
            progressBar_All_At_Once.PerformStep();

            //Initailize E3/E2/Diff ProgressBar
            progressBar_E3.Value = 0;
            progressBar_E2.Value = 0;
            progressBar_GCS_Diff.Value = 0;

            //....Aging....
            f1().PTN_update(255, 255, 255);
            progressBar_All_At_Once.Maximum++;
            Availability_Agine = true;
            int Sec = Convert.ToInt16(textBox_Aging_Sec.Text);
            textBox_Aging_Sec_Read.Text = Sec.ToString();
            Application.DoEvents();
            while (true)
            {
                if (Availability_Agine == false) break;

                if (Sec > 0)
                {
                    Thread.Sleep(1000);
                    textBox_Aging_Sec_Read.Text = (Sec--).ToString();
                    Application.DoEvents();

                }
                else
                {
                    textBox_Aging_Sec_Read.Text = Sec.ToString();
                    Application.DoEvents();
                    break;
                }
            }
            progressBar_All_At_Once.PerformStep();
            //.............


            //Perform Button E3/E2/Diff
            if (checkBox_All_At_Once_E3.Checked)
            {
                Delta_E_calculation_btn.PerformClick();
                progressBar_All_At_Once.PerformStep();
            }
            if (checkBox_All_At_Once_E2.Checked)
            {
                Delta_E2_calculation_btn.PerformClick();
                progressBar_All_At_Once.PerformStep();
            }
            if (checkBox_All_At_Once_Diff_GCS.Checked)
            {
                button_SH_GCS_Difference_Measure.PerformClick();
                progressBar_All_At_Once.PerformStep();
            }
            if (checkBox_All_At_Once_Diff_BCS.Checked)
            {
                button_SH_BCS_Difference_Measure.PerformClick();
                progressBar_All_At_Once.PerformStep();
            }
            if(checkBox_All_At_Once_AOD_GCS.Checked)
            {
                button_AOD_GCS.PerformClick();
                progressBar_All_At_Once.PerformStep();
            }

            All_At_Once_CheckBox_Enable(true);

            MessageBox.Show("All-AtOnce Measure was finished !");
        }

        private void checkBox11_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox11_Diff, textBox11_Diff);
        }

        int Get_DBV_Max_Point()
        {
            
            int dbv_max_point = Convert.ToInt16(textBox_Delta_E2_Max_Point.Text);
            if (dbv_max_point >= f1().Get_DBV_TrackBar_Maximum()) dbv_max_point = f1().Get_DBV_TrackBar_Maximum();
            else if (dbv_max_point <= 0) dbv_max_point = 0;
            textBox_Delta_E2_Max_Point.Text = dbv_max_point.ToString();

            return dbv_max_point;
        }

        int Get_DBV_End_Point(int dbv_max_point)
        {
         
            int dbv_end_Point = Convert.ToInt16(textBox_Delta_E2_End_Point.Text);
            if (dbv_end_Point >= (dbv_max_point - 1)) dbv_end_Point = (dbv_max_point - 1);
            else if (dbv_end_Point <= 0) dbv_end_Point = 0;
            textBox_Delta_E2_End_Point.Text = dbv_end_Point.ToString();

            return dbv_end_Point;
        }

        private void button_DBV_Accuracy_20191217_Click(object sender, EventArgs e)
        {
            if (f1().label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                //-----------------------//
                checkBox_White_PTN_Apply_E2.Checked = true;
                step_value_1.Checked = true; //1step
                radioButton_Max_to_Min_E2.Checked = true; //max to min DBV
                textBox_delay_time_E2.Text = "16";//delay 16ms
                //-----------------------//

                Button_Click_Enable(false);
                Delta_E2_Textbox_CheckBox_Radiobutton_Enable(false);

                //Set ProgressBar_E2 Max and Step and Value
                progressBar_E2.Value = 0;
                progressBar_E2.Step = 1;
                progressBar_E2.Maximum = 1;
                if (checkBox_1st_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                if (checkBox_2nd_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                if (checkBox_3rd_Condition_Measure_E2.Checked) progressBar_E2.Maximum++;
                progressBar_E2.PerformStep();

                int Step_Value = 0;
                if (step_value_1.Checked) Step_Value = 1;
                else if (step_value_4.Checked) Step_Value = 4;
                else if (step_value_8.Checked) Step_Value = 8;
                else if (step_value_16.Checked) Step_Value = 16;
                else System.Windows.Forms.MessageBox.Show("It's impossible(Delta E2)");

                //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                int dbv_max_point = Get_DBV_Max_Point();
                int dbv_end_Point = Get_DBV_End_Point(dbv_max_point);

                //Set Availability and delay_time_between_measurement
                Availability_E2 = true;
                int delay_time_between_measurement = Convert.ToInt16(textBox_delay_time_E2.Text);

                if (checkBox_1st_Condition_Measure_E2.Checked) dataGridView4.Rows.Clear();
                if (checkBox_2nd_Condition_Measure_E2.Checked) dataGridView5.Rows.Clear();
                if (checkBox_3rd_Condition_Measure_E2.Checked) dataGridView6.Rows.Clear();

                if (checkBox_1st_Condition_Measure_E2.Checked)
                {
                    dataGridView4.Columns[0].HeaderText = "DBV";
                    int Condition = 1;
                    Script_Apply_For_Condition1();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1().PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    //---------------------------------------------//
                    //First 2nit ~ 600nit
                    dataGridView4.Rows.Add("1st", "-", "-", "-");
                    int Average_Count = 1;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "2046";
                    textBox_Delta_E2_End_Point.Text = "1";
                    dbv_max_point = Get_DBV_Max_Point();
                    dbv_end_Point = Get_DBV_End_Point(dbv_max_point);
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1


                    //Second
                    dataGridView4.Rows.Add("2nd", "-", "-", "-");
                    Average_Count = 5;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "444";
                    dbv_max_point = Get_DBV_Max_Point();
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1

                    //Third (=Second);
                    dataGridView4.Rows.Add("3rd", "-", "-", "-");
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1
                    //---------------------------------------------//

                    progressBar_E2.PerformStep();
                }

                if (checkBox_2nd_Condition_Measure_E2.Checked)
                {
                    dataGridView5.Columns[0].HeaderText = "DBV";
                    int Condition = 2;
                    Script_Apply_For_Condition2();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1().PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    //---------------------------------------------//
                    //First 2nit ~ 600nit
                    dataGridView5.Rows.Add("2nd", "-", "-", "-");
                    int Average_Count = 1;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "2046";
                    textBox_Delta_E2_End_Point.Text = "1";
                    dbv_max_point = Get_DBV_Max_Point();
                    dbv_end_Point = Get_DBV_End_Point(dbv_max_point);
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1

                    //Second
                    dataGridView5.Rows.Add("2nd", "-", "-", "-");
                    Average_Count = 5;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "444";
                    dbv_max_point = Get_DBV_Max_Point();
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1

                    //Third (=Second);
                    dataGridView5.Rows.Add("3rd", "-", "-", "-");
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1
                    //---------------------------------------------//

                    //---------------------------------------------//

                    progressBar_E2.PerformStep();
                }

                if (checkBox_3rd_Condition_Measure_E2.Checked)
                {
                    dataGridView6.Columns[0].HeaderText = "DBV";
                    int Condition = 3;
                    Script_Apply_For_Condition3();
                    if (checkBox_White_PTN_Apply_E2.Checked)
                    {
                        f1().PTN_update(255, 255, 255);
                        Thread.Sleep(300);
                    }
                    //---------------------------------------------//
                    //First 2nit ~ 600nit
                    dataGridView6.Rows.Add("3rd", "-", "-", "-");
                    int Average_Count = 1;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "2046";
                    textBox_Delta_E2_End_Point.Text = "1";
                    dbv_max_point = Get_DBV_Max_Point();
                    dbv_end_Point = Get_DBV_End_Point(dbv_max_point);
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1


                    //Second
                    dataGridView6.Rows.Add("2nd", "-", "-", "-");
                    Average_Count = 5;
                    //Set DBV_Max_End_Point (From textBox_Delta_E2_Max_Point  / textBox_Delta_E2_End_Point)
                    textBox_Delta_E2_Max_Point.Text = "444";
                    dbv_max_point = Get_DBV_Max_Point();
                    //Measure
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1

                    //Third (=Second);
                    dataGridView6.Rows.Add("3rd", "-", "-", "-");
                    Optic_SH_Delta_E2_Measure_Average_Mode(dbv_end_Point, dbv_max_point, delay_time_between_measurement, Step_Value, Condition, Average_Count); //Condition1
                    //---------------------------------------------//


                    //---------------------------------------------//

                    progressBar_E2.PerformStep();
                }
                Button_Click_Enable(true);
                Delta_E2_Textbox_CheckBox_Radiobutton_Enable(true);
            }
        }

        private void groupBox21_Enter(object sender, EventArgs e)
        {

        }

        private int DBV_Max_Boundary_Processing(int dbv_max_point,int Max_DBV)
        {
            if (dbv_max_point >= Max_DBV) dbv_max_point = Max_DBV;
            else if (dbv_max_point <= 0) dbv_max_point = 0;
            return dbv_max_point;
        }

        private int DBV_End_Boundary_Processing(int dbv_end_Point, int dbv_max_point)
        {
            if (dbv_end_Point >= (dbv_max_point - 1)) dbv_end_Point = (dbv_max_point - 1);
            else if (dbv_end_Point <= 0) dbv_end_Point = 0;
            return dbv_end_Point;
        }

        private void Sub_BCS_Diff_Measure(int gray, int dbv_delay_time, int Step_Value, int dbv_end_Point, int dbv_max_point)
        {
            if (radioButton_BCS_Diff_Single.Checked) Optic_SH_Diff_BCS_Measure(gray, dbv_end_Point, dbv_max_point, dbv_delay_time, Step_Value, OC_Single_Dual_Triple.Single);
            else if (radioButton_BCS_Diff_Dual.Checked) Optic_SH_Diff_BCS_Measure(gray, dbv_end_Point, dbv_max_point, dbv_delay_time, Step_Value, OC_Single_Dual_Triple.Dual);
            else if (radioButton_BCS_Diff_Triple.Checked) Optic_SH_Diff_BCS_Measure(gray, dbv_end_Point, dbv_max_point, dbv_delay_time, Step_Value, OC_Single_Dual_Triple.Triple);
            
        }

        private int Get_DBV_Max_Point_1(int Max_DBV)
        {
            int dbv_max_point_1 = Convert.ToInt32(textBox_BCS_Diff_Sub_1_Max_Point.Text);
            dbv_max_point_1 = DBV_Max_Boundary_Processing(dbv_max_point_1, Max_DBV);
            textBox_BCS_Diff_Sub_1_Max_Point.Text = dbv_max_point_1.ToString();

            return dbv_max_point_1;
        }

        private int Get_DBV_Max_Point_2(int Max_DBV)
        {
            int dbv_max_point_2 = Convert.ToInt32(textBox_BCS_Diff_Sub_2_Max_Point.Text);
            dbv_max_point_2 = DBV_Max_Boundary_Processing(dbv_max_point_2, Max_DBV);
            textBox_BCS_Diff_Sub_2_Max_Point.Text = dbv_max_point_2.ToString();

            return dbv_max_point_2;
        }

        private int Get_DBV_Max_Point_3(int Max_DBV)
        {
            int dbv_max_point_3 = Convert.ToInt32(textBox_BCS_Diff_Sub_3_Max_Point.Text);
            dbv_max_point_3 = DBV_Max_Boundary_Processing(dbv_max_point_3, Max_DBV);
            textBox_BCS_Diff_Sub_3_Max_Point.Text = dbv_max_point_3.ToString();

            return dbv_max_point_3;
        }

        private int Get_DBV_Min_Point_1(int dbv_max_point_1)
        {
            int dbv_end_Point_1 = Convert.ToInt32(textBox_BCS_Diff_Sub_1_Min_Point.Text);
            dbv_end_Point_1 = DBV_End_Boundary_Processing(dbv_end_Point_1, dbv_max_point_1);
            textBox_BCS_Diff_Sub_1_Min_Point.Text = dbv_end_Point_1.ToString();

            return dbv_end_Point_1;
        }

        private int Get_DBV_Min_Point_2(int dbv_max_point_2)
        {
            int dbv_end_Point_2 = Convert.ToInt32(textBox_BCS_Diff_Sub_2_Min_Point.Text);
            dbv_end_Point_2 = DBV_End_Boundary_Processing(dbv_end_Point_2, dbv_max_point_2);
            textBox_BCS_Diff_Sub_2_Min_Point.Text = dbv_end_Point_2.ToString();

            return dbv_end_Point_2;
        }

        private int Get_DBV_Min_Point_3(int dbv_max_point_3)
        {
            int dbv_end_Point_3 = Convert.ToInt32(textBox_BCS_Diff_Sub_3_Min_Point.Text);
            dbv_end_Point_3 = DBV_End_Boundary_Processing(dbv_end_Point_3, dbv_max_point_3);
            textBox_BCS_Diff_Sub_3_Min_Point.Text = dbv_end_Point_3.ToString();

            return dbv_end_Point_3;
        }

        private void BCS_Diff_Measure(int gray, int dbv_delay_time, int Step_Value_Range1,int Step_Value_Range2,int Step_Value_Range3)
        {
            int Max_DBV = f1().Get_DBV_TrackBar_Maximum();

            if (checkBox_BCS_Diif_Range_1.Checked && Availability_Diff_BCS)
            {
                //Set DBV_End_Point (Range1)
                int dbv_max_point_1 = Get_DBV_Max_Point_1(Max_DBV);
                int dbv_end_Point_1 = Get_DBV_Min_Point_1(dbv_max_point_1);
                Sub_BCS_Diff_Measure(gray, dbv_delay_time, Step_Value_Range1, dbv_end_Point_1, dbv_max_point_1);
            }

            if (checkBox_BCS_Diif_Range_2.Checked && Availability_Diff_BCS)
            {
                //Set DBV_End_Point (Range2)
                int dbv_max_point_2 = Get_DBV_Max_Point_2(Max_DBV);
                int dbv_end_Point_2 = Get_DBV_Min_Point_2(dbv_max_point_2);
                Sub_BCS_Diff_Measure(gray, dbv_delay_time, Step_Value_Range2, dbv_end_Point_2, dbv_max_point_2);
            }

            if (checkBox_BCS_Diif_Range_3.Checked && Availability_Diff_BCS)
            {
                //Set DBV_End_Point (Range3)
                int dbv_max_point_3 = Get_DBV_Max_Point_3(Max_DBV);
                int dbv_end_Point_3 = Get_DBV_Min_Point_3(dbv_max_point_3);
                Sub_BCS_Diff_Measure(gray, dbv_delay_time, Step_Value_Range3, dbv_end_Point_3, dbv_max_point_3);
            }
        }

        private void button_SH_BCS_Difference_Measure_Click(object sender, EventArgs e)
        {
            button_Stop.PerformClick(); 
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            if (f1.label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                groupBox_BCS_Diff.Enabled = false;

                dataGridView10.Rows.Clear();
                dataGridView11.Rows.Clear();
                dataGridView12.Rows.Clear();
                dataGridView10.Columns[0].HeaderText = "DBV";
                dataGridView11.Columns[0].HeaderText = "DBV";
                dataGridView12.Columns[0].HeaderText = "DBV";

                int Step_Value_Range1 = 0;
                if (BCS_Diff_step_value_1_Range1.Checked) Step_Value_Range1 = 1;
                else if (BCS_Diff_step_value_4_Range1.Checked) Step_Value_Range1 = 4;
                else if (BCS_Diff_step_value_8_Range1.Checked) Step_Value_Range1 = 8;
                else if (BCS_Diff_step_value_16_Range1.Checked) Step_Value_Range1 = 16;
                
                int Step_Value_Range2 = 0;
                if (BCS_Diff_step_value_1_Range2.Checked) Step_Value_Range2 = 1;
                else if (BCS_Diff_step_value_4_Range2.Checked) Step_Value_Range2 = 4;
                else if (BCS_Diff_step_value_8_Range2.Checked) Step_Value_Range2 = 8;
                else if (BCS_Diff_step_value_16_Range2.Checked) Step_Value_Range2 = 16;

                int Step_Value_Range3 = 0;
                if (BCS_Diff_step_value_1_Range3.Checked) Step_Value_Range3 = 1;
                else if (BCS_Diff_step_value_4_Range3.Checked) Step_Value_Range3 = 4;
                else if (BCS_Diff_step_value_8_Range3.Checked) Step_Value_Range3 = 8;
                else if (BCS_Diff_step_value_16_Range3.Checked) Step_Value_Range3 = 16;

                Update_Diff_BCS_Gray_Checkbox_And_Textbox();

                //Set Availability and delay_time_between_measurement
                Availability_Diff_BCS = true;
                int dbv_delay_time = Convert.ToInt16(textBox_delay_time_Diff_BCS.Text);

                //Set ProgressBar_E3 Max and Step and Value
                int Progress_Bar_Diff_Max = 0;
                for (int i = 0; i < 11; i++) if (checkBox_Diff_BCS_Gray[i]) Progress_Bar_Diff_Max++;
                progressBar_BCS_Diff.Value = 0;
                progressBar_BCS_Diff.Step = 1;
                progressBar_BCS_Diff.Maximum = 1;
                progressBar_BCS_Diff.Maximum += Progress_Bar_Diff_Max;
                progressBar_BCS_Diff.PerformStep();

                for (int i = 0; i < 11; i++)
                {
                    if (checkBox_Diff_BCS_Gray[i])
                    {
                        int gray = Convert.ToInt32(textBox_Diff_BCS_DBV[i]);
                        f1.PTN_update(gray, gray, gray); 
                        Thread.Sleep(300);
                        BCS_Diff_Measure(gray, dbv_delay_time, Step_Value_Range1, Step_Value_Range2, Step_Value_Range3);
                        progressBar_BCS_Diff.PerformStep();
                        f1.GB_Status_AppendText_Nextline(i.ToString() + ")Gray" + gray.ToString() + " was applied", Color.Blue);
                    }
                    else
                    {
                        f1.GB_Status_AppendText_Nextline(i.ToString() + ")Gray point was skipped was Skipped", Color.Black);
                    }
                }

                groupBox_BCS_Diff.Enabled = true;
            }
        }

        private void textBox4_1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_AOD_DBV1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV1, textBox_AOD_DBV1);
        }

        private void checkBox_AOD_DBV2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV2, textBox_AOD_DBV2);
        }

        private void checkBox_AOD_DBV3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV3, textBox_AOD_DBV3);
        }

        private void button_AOD_GCS_Click(object sender, EventArgs e)
        {
            if (f1().label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                Button_Click_Enable(false);
                AOD_GCS_Textbox_CheckBox_Enable(false);

                Initalize_For_AOD_GCS_Measure();
                AOD_GCS_Measure();

                Button_Click_Enable(true);
                AOD_GCS_Textbox_CheckBox_Enable(true);
            }
        }

        private void Initalize_For_AOD_GCS_Measure()
        {
            button_Stop.PerformClick(); 
            Availability_AOD_GCS = true;

            progressBar_E3.Value = 0;
            progressBar_E3.Step = 1;
            progressBar_E3.Maximum = 1;
            if (checkBox_AOD_DBV1.Checked) progressBar_E3.Maximum++;
            if (checkBox_AOD_DBV2.Checked) progressBar_E3.Maximum++;
            if (checkBox_AOD_DBV3.Checked) progressBar_E3.Maximum++;
            if (checkBox_AOD_DBV4.Checked) progressBar_E3.Maximum++;
            if (checkBox_AOD_DBV5.Checked) progressBar_E3.Maximum++;
            if (checkBox_AOD_DBV6.Checked) progressBar_E3.Maximum++;

            progressBar_E3.PerformStep();
            dataGridView13.Rows.Clear();
        }


        private void AOD_GCS_Measure()
        {
            f1().AOD_On(); Thread.Sleep(50);
            f1().AOD_On(); Thread.Sleep(50);

            if (checkBox_AOD_DBV1.Checked) AOD_GCS_Measure(textBox_AOD_DBV1.Text.PadLeft(3, '0'));
            if (checkBox_AOD_DBV2.Checked) AOD_GCS_Measure(textBox_AOD_DBV2.Text.PadLeft(3, '0'));
            if (checkBox_AOD_DBV3.Checked) AOD_GCS_Measure(textBox_AOD_DBV3.Text.PadLeft(3, '0'));
            if (checkBox_AOD_DBV4.Checked) AOD_GCS_Measure(textBox_AOD_DBV4.Text.PadLeft(3, '0'));
            if (checkBox_AOD_DBV5.Checked) AOD_GCS_Measure(textBox_AOD_DBV5.Text.PadLeft(3, '0'));
            if (checkBox_AOD_DBV6.Checked) AOD_GCS_Measure(textBox_AOD_DBV6.Text.PadLeft(3, '0'));

            f1().AOD_Off(); Thread.Sleep(50);
            f1().AOD_Off(); Thread.Sleep(50);
        }


        private void AOD_GCS_Measure(string DBV)
        {
            f1().DBV_Setting(DBV);
            dataGridView13.Rows.Add("DBV", DBV, "-", "-");
            AOD_GCS_Measure(Get_AOD_or_DeltaE3_gray_end_Point(), Convert.ToInt16(textBox_delay_time.Text));
            Calculate_Delta_E_From_x_y_Lv(Get_AOD_or_DeltaE3_gray_end_Point(), (dataGridView13.Rows.Count - 1), 4);
            progressBar_E3.PerformStep();
        }

        private int Get_AOD_or_DeltaE3_gray_end_Point()
        {
            int gray_end_Point = Convert.ToInt16(textBox_Delta_E_End_Point.Text);
            if (gray_end_Point >= 254) gray_end_Point = 254;
            else if (gray_end_Point <= 0) gray_end_Point = 0;
            else { }
            textBox_Delta_E_End_Point.Text = gray_end_Point.ToString();

            return gray_end_Point;
        }

        private void checkBox_Ave_Measure_E3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox12_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox12_Diff, textBox12_Diff);
        }

        private void checkBox13_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox13_Diff, textBox13_Diff);
        }

        private void checkBox14_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox14_Diff, textBox14_Diff);
        }

        private void checkBox15_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox15_Diff, textBox15_Diff);
        }

        private void checkBox16_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox16_Diff, textBox16_Diff);
        }

        private void checkBox17_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox17_Diff, textBox17_Diff);
        }

        private void checkBox18_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox18_Diff, textBox18_Diff);
        }

        private void checkBox19_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox19_Diff, textBox19_Diff);
        }

        private void checkBox20_Diff_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox20_Diff, textBox20_Diff);
        }

        private void checkBox1_BCS_Diff_Gray_P1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P1, textBox_BCS_Diff_Gray_P1);
        }

        private void checkBox1_BCS_Diff_Gray_P2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P2, textBox_BCS_Diff_Gray_P2);
        }

        private void checkBox1_BCS_Diff_Gray_P3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P3, textBox_BCS_Diff_Gray_P3);
        }

        private void checkBox1_BCS_Diff_Gray_P4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P4, textBox_BCS_Diff_Gray_P4);
        }

        private void checkBox1_BCS_Diff_Gray_P5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P5, textBox_BCS_Diff_Gray_P5);
        }

        private void checkBox1_BCS_Diff_Gray_P6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P6, textBox_BCS_Diff_Gray_P6);
        }

        private void checkBox1_BCS_Diff_Gray_P7_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P7, textBox_BCS_Diff_Gray_P7);
        }

        private void checkBox1_BCS_Diff_Gray_P8_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P8, textBox_BCS_Diff_Gray_P8);
        }

        private void checkBox1_BCS_Diff_Gray_P9_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P9, textBox_BCS_Diff_Gray_P9);
        }

        private void checkBox1_BCS_Diff_Gray_P10_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P10, textBox_BCS_Diff_Gray_P10);
        }

        private void checkBox1_BCS_Diff_Gray_P11_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox1_BCS_Diff_Gray_P11, textBox_BCS_Diff_Gray_P11);
        }

        private void textBox_Aging_Sec_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void textBox_Aging_Sec_Read_TextChanged(object sender, EventArgs e)
        {

        }

        private int Get_progressBar_Gamma_Crush_Max()
        {
            int max = 0;
            foreach (bool e in checkBox_Gamma_Crush) if (e) max++;
            return max;
        }

        private void Intialize_Gamma_Crush_ProgressBar()
        {
            progressBar_Gamma_Crush.Maximum = Get_progressBar_Gamma_Crush_Max();
            progressBar_Gamma_Crush.Value = 0;
            progressBar_Gamma_Crush.Step = 1;
        }

        private void Clear_Gamma_Crush_dataGridView()
        {
            dataGridView10.Rows.Clear();
            dataGridView11.Rows.Clear();
            dataGridView12.Rows.Clear();
            Application.DoEvents();
        }
        private Form1 f1()
        {
            return (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
        }

        private void Update_DBV_For_Gamma_Crush_and_Show_Gray(string DBV,int Gray)
        {
            f1().DBV_Setting(DBV);
            if (checkBox_Gamma_Crush_Conditon_1.Checked) dataGridView10.Rows.Add("DBV", DBV, "Gray", Gray.ToString());
            if (checkBox_Gamma_Crush_Conditon_2.Checked) dataGridView11.Rows.Add("DBV", DBV, "Gray", Gray.ToString());
            if (checkBox_Gamma_Crush_Conditon_3.Checked) dataGridView12.Rows.Add("DBV", DBV, "Gray", Gray.ToString());
            f1().GB_Status_AppendText_Nextline("DBV[" + DBV + "]/Gray[" + Gray.ToString() + "] was applied", Color.Blue);
        }

        private void Initializing_Condition_Setting_For_Gamma_Crush_Measure()
        {
            button_Stop.PerformClick();
            Button_Click_Enable(false);
            Gamma_Crush_Textbox_CheckBox_Radiobutton_Enable(false);
            Update_Gamma_Crush_Checkbox_And_DBV_Gray_Textbox();
            Intialize_Gamma_Crush_ProgressBar();
            Clear_Gamma_Crush_dataGridView();
            Availability_Gamma_Crush = true;
            f1().GB_Status_AppendText_Nextline("Gamma Crush Measure Start", Color.Blue);
        }

        private void Measure_and_update_datagridview(DataGridView datagridview, string Comment)
        {
            XYLv Meas = new XYLv();
            Meas = f1().Measure();
            datagridview.DataSource = null; // reset (unbind the datasource)
            datagridview.Rows.Add(Comment, Meas.X, Meas.Y, Meas.Lv);
            datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
        }


        private void Sub_Sub_Measure_Gamma_Crush(DataGridView datagridview,int Gray)
        {

            if (checkBox_Gamma_Crush_W.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(Gray, Gray, Gray);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "W");
                f1().GB_Status_AppendText_Nextline("White Pattern", Color.Black);
            }

            if (checkBox_Gamma_Crush_R.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(Gray, 0, 0);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "R");
                f1().GB_Status_AppendText_Nextline("Red Pattern", Color.Red);
            }

            if (checkBox_Gamma_Crush_G.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(0, Gray, 0);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "G");
                f1().GB_Status_AppendText_Nextline("Green Pattern", Color.Green);
            }

            if (checkBox_Gamma_Crush_B.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(0, 0, Gray);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "B");
                f1().GB_Status_AppendText_Nextline("Blue Pattern", Color.Blue);
            }

            if (checkBox_Gamma_Crush_GB.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(0, Gray, Gray);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "GB");
                f1().GB_Status_AppendText_Nextline("Cyan Pattern", Color.Cyan);
            }

            if (checkBox_Gamma_Crush_RB.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(Gray, 0, Gray);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "RB");
                f1().GB_Status_AppendText_Nextline("Magenta Pattern", Color.Magenta);
            }

            if (checkBox_Gamma_Crush_RG.Checked && Availability_Gamma_Crush)
            {
                f1().PTN_update(Gray, Gray, 0);
                Thread.Sleep(300);
                Measure_and_update_datagridview(datagridview, "RG");
                f1().GB_Status_AppendText_Nextline("Yellow Pattern", Color.DarkGoldenrod);
            }
        }

        private void Sub_Measure_Gamma_Crush(int Gray)
        {
            if (checkBox_Gamma_Crush_Conditon_1.Checked && Availability_Gamma_Crush)
            {
                Script_Apply_For_Condition1();
                Sub_Sub_Measure_Gamma_Crush(dataGridView10, Gray);
            }
            if (checkBox_Gamma_Crush_Conditon_2.Checked && Availability_Gamma_Crush)
            {
                Script_Apply_For_Condition2();
                Sub_Sub_Measure_Gamma_Crush(dataGridView11, Gray);
            }
            if (checkBox_Gamma_Crush_Conditon_3.Checked && Availability_Gamma_Crush)
            {
                Script_Apply_For_Condition3();
                Sub_Sub_Measure_Gamma_Crush(dataGridView12, Gray);
            }
        }

        private void Main_Measure_Gamma_Crush()
        {
            for (int i = 0; i < 10; i++)
            {
                if (checkBox_Gamma_Crush[i] && Availability_Gamma_Crush)
                {
                    int Gray = Convert.ToInt16(textBox_Gamma_Crush_Gray[i]);
                    string DBV = textBox_Gamma_Crush_DBV[i].PadLeft(3, '0');// hex (as a string form)
                    Update_DBV_For_Gamma_Crush_and_Show_Gray(DBV, Gray);

                    Sub_Measure_Gamma_Crush(Gray);

                    progressBar_Gamma_Crush.PerformStep();
                }
            }
        }

        private void Finishing_Condition_Setting_For_Gamma_Crush_Measure()
        {
            Button_Click_Enable(true);
            Gamma_Crush_Textbox_CheckBox_Radiobutton_Enable(true);
            f1().GB_Status_AppendText_Nextline("Gamma Crush Measure Finished", Color.Blue);
        }




        private void button_Gamma_Crush_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)System.Windows.Forms.Application.OpenForms["Form1"];
            if (f1.label_CA_remote_status.Text != "CA Remote : On")
            {
                System.Windows.Forms.MessageBox.Show("Please Connect CA310 or CA410 First");
            }
            else
            {
                Initializing_Condition_Setting_For_Gamma_Crush_Measure();
                Main_Measure_Gamma_Crush();
                Finishing_Condition_Setting_For_Gamma_Crush_Measure();
            }
        }


        private void Optic_SH_Diff_BCS_Measure(int gray, int dbv_end_Point, int dbv_max_point, int dbv_delay_time, int Step_Value, OC_Single_Dual_Triple oc_mode)
        {

            DataGridView datagridview1 = dataGridView10;
            DataGridView datagridview2 = dataGridView11;
            DataGridView datagridview3 = dataGridView12;
            int dbv = dbv_max_point;

            if (oc_mode == OC_Single_Dual_Triple.Triple)
            {
                datagridview1.Rows.Add("G" + gray.ToString(), "-", "-", "-");
                datagridview2.Rows.Add("G" + gray.ToString(), "-", "-", "-");
                datagridview3.Rows.Add("G" + gray.ToString(), "-", "-", "-");
            }
            else if (oc_mode == OC_Single_Dual_Triple.Dual)
            {
                datagridview1.Rows.Add("G" + gray.ToString(), "-", "-", "-");
                datagridview2.Rows.Add("G" + gray.ToString(), "-", "-", "-");
            }
            else if (oc_mode == OC_Single_Dual_Triple.Single)
            {
                datagridview1.Rows.Add("G" + gray.ToString(), "-", "-", "-");
            }


            for (int i = (dbv_max_point); i > dbv_end_Point - Step_Value & Availability_Diff_BCS;)
            {
                i = i - Step_Value;
                dbv = i + Step_Value;
                if (dbv < dbv_end_Point)
                    break;

                f1().Set_BCS(dbv);
                Thread.Sleep(dbv_delay_time);

                //Condition1   
                Script_Apply_For_Condition1();
                try
                {
                    //Measure
                    f1().isMsr = true;
                    f1().CA_Measure_button.Enabled = false;
                    f1().objCa.Measure();
                    f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                    f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                    f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                    System.Windows.Forms.Application.DoEvents();
                    if (f1().isMsr == false) break;
                    f1().CA_Measure_button.Enabled = true;

                    //Data Grid setting
                    datagridview1.DataSource = null; // reset (unbind the datasource
                    datagridview1.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                    datagridview1.FirstDisplayedScrollingRowIndex = datagridview1.RowCount - 1;
                }
                catch (Exception er)
                {
                    f1().DisplayError(er);
                    System.Windows.Forms.Application.Exit();
                }

                if (oc_mode == OC_Single_Dual_Triple.Triple || oc_mode == OC_Single_Dual_Triple.Dual)
                {
                    //Condition2
                    Script_Apply_For_Condition2();
                    try
                    {
                        //Measure
                        f1().isMsr = true;
                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();
                        f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                        f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                        f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        System.Windows.Forms.Application.DoEvents();
                        if (f1().isMsr == false) break;
                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting
                        datagridview2.DataSource = null; // reset (unbind the datasource
                        datagridview2.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview2.FirstDisplayedScrollingRowIndex = datagridview1.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }

                if (oc_mode == OC_Single_Dual_Triple.Triple)
                {
                    //Condition3
                    Script_Apply_For_Condition3();
                    try
                    {
                        //Measure
                        f1().isMsr = true;
                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();
                        f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                        f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                        f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        System.Windows.Forms.Application.DoEvents();
                        if (f1().isMsr == false) break;
                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting
                        datagridview3.DataSource = null; // reset (unbind the datasource
                        datagridview3.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview3.FirstDisplayedScrollingRowIndex = datagridview1.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
        }



        private void Optic_SH_Delta_E2_Measure(int dbv_end_Point, int dbv_max_point, int delay_time_between_measurement, int Step_Value, int Condition = 1)
        {
            DataGridView datagridview;
            if (Condition == 1) datagridview = dataGridView4;
            else if (Condition == 2) datagridview = dataGridView5;
            else if (Condition == 3) datagridview = dataGridView6;
            else datagridview = null;

            XYLv[] Measure = new XYLv[5];
            int Max_Index = 0; double Max_Value = 0;
            int Min_Index = 0; double Min_Value = 2000;

            int dbv = dbv_end_Point;

            //Min to Max
            if (radioButton_Min_to_Max_E2.Checked)
            {
                for (int i = dbv_end_Point; i < (dbv_max_point + Step_Value) & Availability_E2;)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    i = i + Step_Value;
                    dbv = i - Step_Value;
                    if (dbv > dbv_max_point)
                        break;

                    f1().Set_BCS(dbv);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E2.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E2.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E2.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");

                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {

                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }

                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)

                        //Add data to Datagridview

                        datagridview.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            //Max to Min
            else if (radioButton_Max_to_Min_E2.Checked)
            {
                for (int i = (dbv_max_point); i > dbv_end_Point - Step_Value & Availability_E2;)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    i = i - Step_Value;
                    dbv = i + Step_Value;
                    if (dbv < dbv_end_Point)
                        break;


                    f1().Set_BCS(dbv);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E2.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E2.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E2.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");

                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {

                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }

                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)

                        //Add data to Datagridview
                        datagridview.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else
            {
                //It Cannot Happen 
            }
        }

        public void Optic_SH_Delta_E2_Measure_Average_Mode(int dbv_end_Point, int dbv_max_point, int delay_time_between_measurement, int Step_Value, int Condition = 1, int Average_Count = 1)
        {
            DataGridView datagridview;
            if (Condition == 1) datagridview = dataGridView4;
            else if (Condition == 2) datagridview = dataGridView5;
            else if (Condition == 3) datagridview = dataGridView6;
            else datagridview = null;

            XYLv[] Measure = new XYLv[5];
            int dbv = dbv_end_Point;

            //Min to Max
            if (radioButton_Min_to_Max_E2.Checked)
            {
                for (int i = dbv_end_Point; i < (dbv_max_point + Step_Value) & Availability_E2;)
                {
                    i = i + Step_Value;
                    dbv = i - Step_Value;
                    if (dbv > dbv_max_point)
                        break;

                    f1().Set_BCS(dbv);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (Average_Count > 1)
                        {
                            f1().GB_Status_AppendText_Nextline("Average_Count : " + Average_Count.ToString(), Color.Blue);
                            XYLv Sum_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            for (int a = 0; a < Average_Count; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();
                                Sum_Measure.double_X += Measure[a].double_X;
                                Sum_Measure.double_Y += Measure[a].double_Y;
                                Sum_Measure.double_Lv += Measure[a].double_Lv;
                                //GB_Status_AppendText_Nextline(a.ToString() + ")Measure X/Y/Lv: " + Measure[a].X + "/" + Measure[a].Y + "/" + Measure[a].Lv, Color.Green);

                            }
                            XYLv Ave_Measure = new XYLv();
                            Ave_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / Average_Count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / Average_Count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / Average_Count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Ave X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);
                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }
                        else
                        {
                            f1().GB_Status_AppendText_Nextline("Average_Count : " + Average_Count.ToString(), Color.Green);
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                            //GB_Status_AppendText_Nextline("Measure X/Y/Lv: " + X_Value_display.Text + "/" + Y_Value_display.Text + "/" + Lv_Value_display.Text, Color.Green);
                        }
                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false) break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            //Max to Min
            else if (radioButton_Max_to_Min_E2.Checked)
            {
                for (int i = (dbv_max_point); i > dbv_end_Point - Step_Value & Availability_E2;)
                {
                    i = i - Step_Value;
                    dbv = i + Step_Value;
                    if (dbv < dbv_end_Point)
                        break;


                    f1().Set_BCS(dbv);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (Average_Count > 1)
                        {
                            f1().GB_Status_AppendText_Nextline("Average_Count : " + Average_Count.ToString(), Color.Blue);
                            XYLv Sum_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            for (int a = 0; a < Average_Count; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();
                                Sum_Measure.double_X += Measure[a].double_X;
                                Sum_Measure.double_Y += Measure[a].double_Y;
                                Sum_Measure.double_Lv += Measure[a].double_Lv;
                                //GB_Status_AppendText_Nextline(a.ToString() + ")Measure X/Y/Lv: " + Measure[a].X + "/" + Measure[a].Y + "/" + Measure[a].Lv, Color.Green);
                            }
                            XYLv Ave_Measure = new XYLv();
                            Ave_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / Average_Count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / Average_Count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / Average_Count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Ave X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);
                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }
                        else
                        {
                            f1().GB_Status_AppendText_Nextline("Average_Count : " + Average_Count.ToString(), Color.Green);
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                            //GB_Status_AppendText_Nextline("Measure X/Y/Lv: " + X_Value_display.Text + "/" + Y_Value_display.Text + "/" + Lv_Value_display.Text, Color.Green);
                        }
                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false) break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(dbv.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else
            {
                //It Cannot Happen 
            }
        }


        private void AOD_Pattern_Setting(int Gray)
        {
            if (radioButton_GCS_White_PTN.Checked)
                f1().IPC_Quick_Send("image.crosstalk " + f1().current_model.get_AOD_X().ToString() + " " + f1().current_model.get_AOD_Y().ToString() + " 0 0 0 " + Gray.ToString() + " " + Gray.ToString() + " " + Gray.ToString());
            else if (radioButton_GCS_Red_PTN.Checked)
                f1().IPC_Quick_Send("image.crosstalk " + f1().current_model.get_AOD_X().ToString() + " " + f1().current_model.get_AOD_Y().ToString() + " 0 0 0 " + Gray.ToString() + " " + (0).ToString() + " " + (0).ToString());
            else if (radioButton_GCS_Green_PTN.Checked)
                f1().IPC_Quick_Send("image.crosstalk " + f1().current_model.get_AOD_X().ToString() + " " + f1().current_model.get_AOD_Y().ToString() + " 0 0 0 " + (0).ToString() + " " + Gray.ToString() + " " + (0).ToString());
            else if (radioButton_GCS_Blue_PTN.Checked)
                f1().IPC_Quick_Send("image.crosstalk " + f1().current_model.get_AOD_X().ToString() + " " + f1().current_model.get_AOD_Y().ToString() + " 0 0 0 " + (0).ToString() + " " + (0).ToString() + " " + Gray.ToString());
        }

        private void AOD_GCS_Measure(int gray_end_Point, int delay_time_between_measurement, int Condition = 1)
        {
            DataGridView datagridview = dataGridView13;

            XYLv[] Measure = new XYLv[5];
            int Max_Index = 0; double Max_Value = 0;
            int Min_Index = 0; double Min_Value = 2000;

            //Gray 48~255 에서 X/Y/Lv 먼저 찍음
            if (radioButton_Min_to_Max_E3.Checked)
            {
                for (int gray = gray_end_Point; gray <= 255 & Availability_AOD_GCS; gray++)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    AOD_Pattern_Setting(gray);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E3.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E3.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E3.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {
                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else if (radioButton_Max_to_Min_E3.Checked)
            {
                for (int gray = 255; gray >= gray_end_Point & Availability_AOD_GCS; gray--)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    AOD_Pattern_Setting(gray);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E3.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E3.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E3.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {
                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else { }
        }


        private void Optic_Measurement_Pattern_Update(int gray)
        {
            if (radioButton_GCS_White_PTN.Checked)
                f1().PTN_update(gray, gray, gray);
            else if (radioButton_GCS_Red_PTN.Checked)
                f1().PTN_update(gray, 0, 0);
            else if (radioButton_GCS_Green_PTN.Checked)
                f1().PTN_update(0, gray, 0);
            else if (radioButton_GCS_Blue_PTN.Checked)
                f1().PTN_update(0, 0, gray);
        }


    

        private void Optic_SH_Delta_E3_Measure(int gray_end_Point, int delay_time_between_measurement, int Condition = 1)
        {
            DataGridView datagridview;
            if (Condition == 1)
            {
                datagridview = dataGridView1;
            }
            else if (Condition == 2)
            {
                datagridview = dataGridView2;
            }
            else if (Condition == 3)
            {
                datagridview = dataGridView3;
            }
            else
            {
                datagridview = null;
            }

            XYLv[] Measure = new XYLv[5];
            int Max_Index = 0; double Max_Value = 0;
            int Min_Index = 0; double Min_Value = 2000;

            //Gray 48~255 에서 X/Y/Lv 먼저 찍음
            if (radioButton_Min_to_Max_E3.Checked)
            {
                for (int gray = gray_end_Point; gray <= 255 & Availability_E3; gray++)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    Optic_Measurement_Pattern_Update(gray);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E3.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E3.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E3.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {
                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else if (radioButton_Max_to_Min_E3.Checked)
            {
                for (int gray = 255; gray >= gray_end_Point & Availability_E3; gray--)
                {
                    Max_Index = 0; Min_Index = 0;
                    Max_Value = 0; Min_Value = 2000;

                    Optic_Measurement_Pattern_Update(gray);
                    Thread.Sleep(delay_time_between_measurement);

                    try
                    {
                        f1().isMsr = true;

                        f1().CA_Measure_button.Enabled = false;
                        f1().objCa.Measure();

                        if (checkBox_Ave_Measure_E3.Checked && f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv < Convert.ToDouble(textBox_Ave_Lv_Limit_E3.Text))
                        {
                            f1().GB_Status_AppendText_Nextline(f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString() + "<" + textBox_Ave_Lv_Limit_E3.Text, Color.Blue);

                            for (int a = 0; a < 5; a++)
                            {
                                f1().objCa.Measure();
                                Measure[a].X = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                                Measure[a].Y = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                                Measure[a].Lv = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                                Measure[a].Double_Update_From_String();

                                if (Measure[a].double_Lv < Min_Value)
                                {
                                    Min_Value = Measure[a].double_Lv;
                                    Min_Index = a;
                                }

                                if (Measure[a].double_Lv > Max_Value)
                                {
                                    Max_Value = Measure[a].double_Lv;
                                    Max_Index = a;
                                }
                            }

                            int count = 0;
                            XYLv Sum_Measure = new XYLv();
                            XYLv Ave_Measure = new XYLv();
                            Sum_Measure.Set_Value(0, 0, 0);
                            Ave_Measure.Set_Value(0, 0, 0);

                            for (int a = 0; a < 5; a++)
                            {
                                if (a == Max_Index || a == Min_Index)
                                {
                                }
                                else
                                {
                                    Sum_Measure.double_X += Measure[a].double_X;
                                    Sum_Measure.double_Y += Measure[a].double_Y;
                                    Sum_Measure.double_Lv += Measure[a].double_Lv;
                                    count++;
                                }
                            }
                            Ave_Measure.double_X = Math.Round((Sum_Measure.double_X / count), 4);
                            Ave_Measure.double_Y = Math.Round((Sum_Measure.double_Y / count), 4);
                            Ave_Measure.double_Lv = Math.Round((Sum_Measure.double_Lv / count), 4);
                            Ave_Measure.String_Update_From_Double();
                            f1().GB_Status_AppendText_Nextline("Max_Lv/Min_Lv Index = " + Max_Index.ToString() + "/" + Min_Index.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Min Lv = " + Measure[Min_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("Ave Lv = " + Ave_Measure.Lv, Color.Black);
                            f1().GB_Status_AppendText_Nextline("Max Lv = " + Measure[Max_Index].double_Lv.ToString(), Color.Black);
                            f1().GB_Status_AppendText_Nextline("count = " + count.ToString() + ",Ave_X/Y/Lv = " + Ave_Measure.X + "/" + Ave_Measure.Y + "/" + Ave_Measure.Lv, Color.Black);

                            f1().X_Value_display.Text = Ave_Measure.X.PadRight(6, '0');
                            f1().Y_Value_display.Text = Ave_Measure.Y.PadRight(6, '0');
                            f1().Lv_Value_display.Text = Ave_Measure.Lv.PadRight(6, '0');
                        }

                        else
                        {
                            f1().X_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sx.ToString("0.0000");
                            f1().Y_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).sy.ToString("0.0000");
                            f1().Lv_Value_display.Text = f1().objCa.OutputProbes.get_ItemOfNumber(1).Lv.ToString("0.0000");
                        }

                        System.Windows.Forms.Application.DoEvents();

                        if (f1().isMsr == false)
                            break;

                        f1().CA_Measure_button.Enabled = true;

                        //Data Grid setting//////////////////////
                        datagridview.DataSource = null; // reset (unbind the datasource)
                        datagridview.Rows.Add(gray.ToString(), f1().X_Value_display.Text, f1().Y_Value_display.Text, f1().Lv_Value_display.Text);
                        datagridview.FirstDisplayedScrollingRowIndex = datagridview.RowCount - 1;
                    }
                    catch (Exception er)
                    {
                        f1().DisplayError(er);
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
            else { }
        }

        private void checkBox_AOD_DBV4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV4, textBox_AOD_DBV4);
        }

        private void checkBox_AOD_DBV5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV5, textBox_AOD_DBV5);
        }

        private void checkBox_AOD_DBV6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_AOD_DBV6, textBox_AOD_DBV6);
        }

        private void checkBox_Gamma_Crush_P1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P1, textBox_Gamma_Crush_DBV1, textBox_Gamma_Crush_Gray_1);
        }

        private void checkBox_Gamma_Crush_P2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P2, textBox_Gamma_Crush_DBV2, textBox_Gamma_Crush_Gray_2);
        }

        private void checkBox_Gamma_Crush_P3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P3, textBox_Gamma_Crush_DBV3, textBox_Gamma_Crush_Gray_3);
        }

        private void checkBox_Gamma_Crush_P4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P4, textBox_Gamma_Crush_DBV4, textBox_Gamma_Crush_Gray_4);
        }

        private void checkBox_Gamma_Crush_P5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P5, textBox_Gamma_Crush_DBV5, textBox_Gamma_Crush_Gray_5);
        }

        private void checkBox_Gamma_Crush_P6_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P6, textBox_Gamma_Crush_DBV6, textBox_Gamma_Crush_Gray_6);
        }

        private void checkBox_Gamma_Crush_P7_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P7, textBox_Gamma_Crush_DBV7, textBox_Gamma_Crush_Gray_7);
        }

        private void checkBox_Gamma_Crush_P8_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P8, textBox_Gamma_Crush_DBV8, textBox_Gamma_Crush_Gray_8);
        }

        private void checkBox_Gamma_Crush_P9_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P9, textBox_Gamma_Crush_DBV9, textBox_Gamma_Crush_Gray_9);
        }

        private void checkBox_Gamma_Crush_P10_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_CheckedChanged(checkBox_Gamma_Crush_P10, textBox_Gamma_Crush_DBV10, textBox_Gamma_Crush_Gray_10);
        }
    }
}

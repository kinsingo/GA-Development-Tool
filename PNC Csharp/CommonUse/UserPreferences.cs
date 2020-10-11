using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSQH_Csharp_Library;

namespace PNC_Csharp
{
    public class EA9155_Preferences
    {
        //GB_Ctrl
        public bool[] checkBox_Band = new bool[DP213_Static.Max_HBM_and_Normal_Band_Amount];
        public bool[] checkBox_AOD = new bool[DP213_Static.Max_AOD_Band_Amount];
        public bool checkBox_Read_DBV_Values;
        public bool checkBox_Special_Gray_Compensation;
        public bool checkBox_Only_255G;
        public bool radioButton_G2G_On;
        public bool radioButton_G2G_Off;
        public string textBox_Max_Loop;
        public string textBox_Subcompensation_GB_skip_Lv;
        public bool radioButton_Mode23456_Gray255_RGB_OC;
        public bool radioButton_Mode23456_Gray255_RVreg1B_OC;

        //Limit Apply Ratio
        public bool radioButton_Limit_Apply_Ratio1;
        public bool radioButton_Limit_Apply_Ratio2;
        public bool radioButton_Limit_Apply_Ratio3;

        //Set_Mode_Selection
        public decimal numericUpDown_Set_Mode_1;
        public decimal numericUpDown_Set_Mode_2;
        public decimal numericUpDown_Set_Mode_3;
        public decimal numericUpDown_Set_Mode_4;
        public decimal numericUpDown_Set_Mode_5;
        public decimal numericUpDown_Set_Mode_6;
        public bool checkBox_Mode_2_Skip;
        public bool checkBox_Mode_3_Skip;
        public bool checkBox_Mode_4_Skip;
        public bool checkBox_Mode_5_Skip;
        public bool checkBox_Mode_6_Skip;

        //ELVSS & Vinit2 Offset
        public string[] ELVSS_Offset = new string[DP213_Static.Max_Set_Amount * DP213_Static.Max_HBM_and_Normal_Band_Amount];
        public string[] Vinit2_Offset = new string[DP213_Static.Max_Set_Amount * DP213_Static.Max_HBM_and_Normal_Band_Amount];

        //ELVSS OC
        public bool checkBox_ELVSS_and_Vinit2_Comp;
        public string textBox_ELVSS_Margin;
        public string textBox_ELVSS_CMD_Delay;
        public string textBox_Slope_Margin;
        public string textBox_ELVSS_Min_Before_Add_Offset;
        public string textBox_ELVSS_Max_Before_Add_Offset;

        //Vreg1 OC
        public bool checkBox_Mode1_Vreg1_Compensation;

        //AM1 OC
        public bool radioButton_AM1_0x00;
        public bool radioButton_AM1_Original_Value;
        public bool radioButton_AM1_Comp;
        public string textBox_AM1_Margin_R;
        public string textBox_AM1_Margin_G;
        public string textBox_AM1_Margin_B;

        //Triple Mode Related
        public bool checkBox_Set23_OC_Skip_If_UV_and_deltaL_Are_within_Specs;
        public bool checkBox_Copy_Mode1_Vreg1_to_Mode23456;
        public bool Checkbox_Copy_Mode1_Measure_to_Mode23456_Target;
        public bool checkBox_Copy_Mode12_Ave_M_to_Mode3_T;
        public bool checkBox_Copy_Mode1_Gamma_to_Mode23;
        public bool checkBox_Copy_Mode1_Gamma_to_Mode4;
        public bool checkBox_Copy_Mode2_Gamma_to_Mode5;
        public bool checkBox_Copy_Mode3_Gamma_to_Mode6;
        public string textBox_OC_Mode1_Green_Offset_Max_Lv;
        public string textBox_OC_Mode1_to_Mode2_Green_Offset;
        public string textBox_OC_Mode1_to_Mode3_Green_Offset;
        public string textBox_OC_Mode1_Green_Offset_Min_Lv;


        //OD ERA DGGM CRC 
        public string textBox_OD_CRC_Hex_1;
        public string textBox_OD_CRC_Hex_2;
        public string textBox_ERA_CRC_Hex_1;
        public string textBox_ERA_CRC_Hex_2;
        public string textBox_DGGM_CRC_Hex_1;
        public string textBox_DGGM_CRC_Hex_2;

        //Black OC
        public bool radioButton_AM0_0x00;
        public bool radioButton_AM0_Original_Value;
        public bool radioButton_Black_Compensation;
        public string textBox_AM0_R_Margin;
        public string textBox_AM0_G_Margin;
        public string textBox_AM0_B_Margin;
        public string textBox_Black_Limit_Lv;
       

        //REF0
        public bool checkBox_VREF0_Comp;
        public string textBox_REF0_Margin;

        public decimal numericUpDown_Set456_Skip_Max_Band;

        //Initial RGBVreg1 Finding Algorithm
        public bool checkBox_Initial_RVreg1B_or_RGB_Algorithm_Apply;
        public string textBox_Initial_OC_RGB_Algorithm_Skip_Target_Lv_Upper_Limit;
        public string textBox_Initial_OC_RGB_Algorithm_Skip_Target_Lv_Lower_Limit;
        public string textBox_Initial_RGB_Algorithm_Lv_Combine_Ratio;
        public bool radioButton_MCI_and_3Points;
        public bool radioButton_LUT_MCI;

        //UVL
        public bool checkBox_OC_Mode23_UVL_Check;
    }





    public class EA9154_Preferences
    {
        public string[] ELVSS_Offset_Set1 = new string[11];//band0 ~ band10
        public string[] Vinit_Offset_Set1 = new string[11];//band0 ~ band10;

        public string[] ELVSS_Offset_Set2 = new string[11];//band0 ~ band10;
        public string[] Vinit_Offset_Set2 = new string[11];//band0 ~ band10;

        public string[] ELVSS_Offset_Set3 = new string[11];//band0 ~ band10;
        public string[] Vinit_Offset_Set3 = new string[11];//band0 ~ band10;

        public string[] ELVSS_Offset_Set4 = new string[11];//band0 ~ band10;
        public string[] Vinit_Offset_Set4 = new string[11];//band0 ~ band10;

        public string[] ELVSS_Offset_Set5 = new string[11];//band0 ~ band10;
        public string[] Vinit_Offset_Set5 = new string[11];//band0 ~ band10;

        public string[] ELVSS_Offset_Set6 = new string[11];//band0 ~ band10;
        public string[] Vinit_Offset_Set6 = new string[11];//band0 ~ band10;

        public bool checkBox_ELVSS_Comp;
        public bool checkBox_ELVSS_VINIT2_Low_Temperature;
        public bool checkBox_ELVSS_Vinit2_Copy_From_Set1_to_Set5;
        public bool checkBox_ELVSS_Vinit2_Copy_From_Set2_to_Set6;

        public string textBox_ELVSS_Margin;
        public string textBox_ELVSS_CMD_Delay;
        public string textBox_Slope_Margin;
        public string textBox_ELVSS_Min;
        public string textBox_ELVSS_Max;

        public bool radioButton_ELVSS_Start_From_Band0_First_ELVSS_60;
        public bool radioButton_ELVSS_Start_From_Band1_First_ELVSS_45;
    }



    public class UserPreferences
    {
        //------Common--------
        //Info
        public string Saved_Date;

        //Textbox to String
        public string textBox_Mipi_Script_Condition1;
        public string textBox_Mipi_Script_Condition2;
        public string textBox_Mipi_Script_Condition3;
        public string textBox_Show_Compared_Mipi_Data;
        public string textBox_Show_Compared_Mipi_Data2;
        public string textBox_Show_Compared_Mipi_Data3;
        public string textBox_delay_After_Condition_1;
        public string textBox_delay_After_Condition_2;
        public string textBox_delay_After_Condition_3;
        public string textBox_Aging_Sec;
        //CheckBox to Bool
        //RadioButton to Bool
        //--------------------

        //-----Delta E2 Measure Related------
        //Textbox to String
        public string textBox_Delta_E2_End_Point;
        public string textBox_Delta_E2_Max_Point;
        public string textBox_Ave_Lv_Limit_E2;
        public string textBox_delay_time_E2;

        //CheckBox to Bool
        public bool checkBox_1st_Condition_Measure_E2;
        public bool checkBox_2nd_Condition_Measure_E2;
        public bool checkBox_3rd_Condition_Measure_E2;
        public bool checkBox_Ave_Measure_E2;
        public bool checkBox_White_PTN_Apply_E2;

        //RadioButton to Bool
        public bool step_value_1;
        public bool step_value_4;
        public bool step_value_8;
        public bool step_value_16;
        public bool radioButton_Min_to_Max_E2;
        public bool radioButton_Max_to_Min_E2;
        //-------------------------------


        //-----Delta E3 Measure Related------
        //Textbox to String
        public string[] textBox_Condition1 = new string[16];//DBV Condition 1
        public string[] textBox_Condition2 = new string[16];//DBV Condition 2
        public string[] textBox_Condition3 = new string[16];//DBV Condition 3
        public string textBox_Ave_Lv_Limit;
        public string textBox_Delta_E_End_Point;
        public string textBox_delay_time;


        //CheckBox to Bool
        public bool checkBox_3rd_Condition_Measure;
        public bool checkBox_2nd_Condition_Measure;
        public bool checkBox_1st_Condition_Measure;
        public bool[] checkBox_Condition1 = new bool[16];//DBV Condition 1
        public bool[] checkBox_Condition2 = new bool[16];//DBV Condition 2
        public bool[] checkBox_Condition3 = new bool[16];//DBV Condition 3
        public bool checkBox_Ave_Measure; //Delta E3
        //RadioButton to Bool
        public bool radioButton_Max_to_Min_E3;
        public bool radioButton_Min_to_Max_E3;

        //-------------------------------


        //-----GCS Diff Measure Related------
        //Textbox to String
        public string textBox_GCS_Diff_Min_Point;
        public string textBox_GCS_Diff_Max_Point;

        public string textBox_delay_time_Diff;
        public string textBox_Ave_Lv_Limit_Diff;
        public string[] textBox_Diff = new string[20];

        //CheckBox to Bool
        public bool[] checkBox_Diff = new bool[20];
        public bool checkBox_Ave_Measure_Diff;

        //RadioButton to Bool
        public bool radioButton_Diff_1st_and_3rd;
        public bool radioButton_Diff_2nd_and_3rd;
        public bool radioButton_Diff_1st_and_2nd;
        public bool radioButton_Diff_1st_2nd_3rd;
        public bool radioButton_SH_Diff_Step_16;
        public bool radioButton_SH_Diff_Step_8;
        public bool radioButton_SH_Diff_Step_4;
        //-------------------------------

        //-----BCS Diff Measure Related------
        //Textbox to String
        public string textBox_delay_time_Diff_BCS;
        public string[] textBox_BCS_Diff_Gray_Points = new string[11];

        public string textBox_BCS_Diff_Sub_1_Min_Point;
        public string textBox_BCS_Diff_Sub_2_Min_Point;
        public string textBox_BCS_Diff_Sub_3_Min_Point;
        public string textBox_BCS_Diff_Sub_1_Max_Point;
        public string textBox_BCS_Diff_Sub_2_Max_Point;
        public string textBox_BCS_Diff_Sub_3_Max_Point;

        //CheckBox to Bool
        public bool[] CheckBox_BCS_Diff_Gray_Points = new bool[11];

        //RadioButton to Bool
        public bool BCS_Diff_step_value_1_Range1;
        public bool BCS_Diff_step_value_4_Range1;
        public bool BCS_Diff_step_value_8_Range1;
        public bool BCS_Diff_step_value_16_Range1;

        public bool BCS_Diff_step_value_1_Range2;
        public bool BCS_Diff_step_value_4_Range2;
        public bool BCS_Diff_step_value_8_Range2;
        public bool BCS_Diff_step_value_16_Range2;

        public bool BCS_Diff_step_value_1_Range3;
        public bool BCS_Diff_step_value_4_Range3;
        public bool BCS_Diff_step_value_8_Range3;
        public bool BCS_Diff_step_value_16_Range3;

        public bool radioButton_BCS_Diff_Single;
        public bool radioButton_BCS_Diff_Dual;
        public bool radioButton_BCS_Diff_Triple;

        public bool checkBox_BCS_Diif_Range_1;
        public bool checkBox_BCS_Diif_Range_2;
        public bool checkBox_BCS_Diif_Range_3;
        //-------------------------------

        //-----All At Once Measure Related------
        //CheckBox to Bool
        public bool checkBox_All_At_Once_E3;
        public bool checkBox_All_At_Once_E2;
        public bool checkBox_All_At_Once_Diff;
        //--------------------------------------

        //-----AOD GCS Measure Related------
        //Textbox to String
        public string textBox_AOD_DBV1;
        public string textBox_AOD_DBV2;
        public string textBox_AOD_DBV3;
        public string textBox_AOD_DBV4;
        public string textBox_AOD_DBV5;
        public string textBox_AOD_DBV6;

        //CheckBox to Bool
        public bool checkBox_AOD_DBV1;
        public bool checkBox_AOD_DBV2;
        public bool checkBox_AOD_DBV3;
        public bool checkBox_AOD_DBV4;
        public bool checkBox_AOD_DBV5;
        public bool checkBox_AOD_DBV6;
        public bool checkBox_All_At_Once_AOD_GCS;
        //----------------------------------

        public bool radioButton_GCS_White_PTN;
        public bool radioButton_GCS_Red_PTN;
        public bool radioButton_GCS_Green_PTN;
        public bool radioButton_GCS_Blue_PTN;

        //----- Gamma Crush ------
        public bool[] checkBox_Gamma_Crush = new bool[10];
        public string[] textBox_Gamma_Crush_DBV = new string[10];
        public string[] textBox_Gamma_Crush_Gray = new string[10];
        public bool checkBox_Gamma_Crush_Conditon_1;
        public bool checkBox_Gamma_Crush_Conditon_2;
        public bool checkBox_Gamma_Crush_Conditon_3;

        public bool checkBox_Gamma_Crush_W;
        public bool checkBox_Gamma_Crush_R;
        public bool checkBox_Gamma_Crush_G;
        public bool checkBox_Gamma_Crush_B;
        public bool checkBox_Gamma_Crush_GB;
        public bool checkBox_Gamma_Crush_RB;
        public bool checkBox_Gamma_Crush_RG;
        //------------------------
    }
}

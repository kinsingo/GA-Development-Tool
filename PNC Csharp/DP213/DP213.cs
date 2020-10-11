﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices; // (Dll 사용위해 필요)
using BSQH_Csharp_Library;
using PNC_Csharp;
using System.Windows.Ink;

namespace PNC_Csharp
{
    public interface DP213_Optic_Compensation
    {
        void ELVSS_and_Vinit2_Compensation();
        void VREF0_Compensation();
        void Black_Compensation();
        void AM1_Compensation();
        void AOD_Compensation();
    }

    public class DP213_Color_Static
    {
        static readonly public Color Color_Set1 = Color.FromArgb(255, 150, 150);
        static readonly public Color Color_Set2 = Color.FromArgb(255, 200, 150);
        static readonly public Color Color_Set3 = Color.FromArgb(175, 175, 255);
        static readonly public Color Color_Set4 = Color.FromArgb(150, 200, 255);
        static readonly public Color Color_Set5 = Color.FromArgb(200, 255, 200);
        static readonly public Color Color_Set6 = Color.FromArgb(50, 255, 200);
    }

    public class Sub_RGB_Vreg1_OC : DP213_forms_accessor
    {
        protected DP213_CMDS_Write_Read_Update_Variables cmds = DP213_CMDS_Write_Read_Update_Variables.getInstance();
        protected DP213_OC_Values_Storage storage = DP213_OC_Values_Storage.getInstance();
        protected DP213_OC_Current_Variables_Structure vars = DP213_OC_Current_Variables_Structure.getInstance();
        protected DP213_OC_Variables_Update_Algorithm_Interface vars_update = DP213_OC_Variables_Update_Algorithm.getInstance();

        protected DP213 OC_Set_Mode1;
        protected DP213 OC_Set_Mode2;
        protected DP213 OC_Set_Mode3;
        protected DP213 OC_Set_Mode4;
        protected DP213 OC_Set_Mode5;
        protected DP213 OC_Set_Mode6;

        public Sub_RGB_Vreg1_OC(DP213_OC_Modes DP213_Objects)
        {
            OC_Set_Mode1 = DP213_Objects.Get_DP213_object(OC_Mode.Mode1);
            OC_Set_Mode2 = DP213_Objects.Get_DP213_object(OC_Mode.Mode2);
            OC_Set_Mode3 = DP213_Objects.Get_DP213_object(OC_Mode.Mode3);
            OC_Set_Mode4 = DP213_Objects.Get_DP213_object(OC_Mode.Mode4);
            OC_Set_Mode5 = DP213_Objects.Get_DP213_object(OC_Mode.Mode5);
            OC_Set_Mode6 = DP213_Objects.Get_DP213_object(OC_Mode.Mode6);
        }

        protected void Set_and_Send_Prev_Band_Gamma_to_Current_Band_and_Update_Gridviews(DP213 To_OC_Set_Mode, int band, int gray)
        {
            RGB AM0 = To_OC_Set_Mode.Get_AM0(band);
            RGB AM1 = To_OC_Set_Mode.Get_AM1(band);
            vars.Gamma = storage.Get_All_band_gray_Gamma(To_OC_Set_Mode.Get_Current_OC_Set(), (band - 1), gray);
            cmds.Set_and_Send_RGB_CMD(To_OC_Set_Mode.Get_Current_OC_Set(), band, gray, vars.Gamma, AM0, AM1);
            dp213_mornitoring().Set_OC_Param_and_OC_Viewer_Gamma(gray, band, To_OC_Set_Mode.Get_Current_OC_Mode());
        }

        protected void Set_and_Send_Current_Band_Gamma_and_Update_Gridviews(DP213 To_OC_Set_Mode, int band, int gray)
        {
            RGB AM0 = To_OC_Set_Mode.Get_AM0(band);
            RGB AM1 = To_OC_Set_Mode.Get_AM1(band);
            vars.Gamma = storage.Get_All_band_gray_Gamma(To_OC_Set_Mode.Get_Current_OC_Set(), band, gray);
            cmds.Set_and_Send_RGB_CMD(To_OC_Set_Mode.Get_Current_OC_Set(), band, gray, vars.Gamma, AM0, AM1);
            dp213_mornitoring().Set_OC_Param_and_OC_Viewer_Gamma(gray, band, To_OC_Set_Mode.Get_Current_OC_Mode());
        }

        protected void Main_OC_Sub_RGB_OC(DP213 OC_Set_Mode, int band, int gray)
        {
            Gamma_Set Set = OC_Set_Mode.Get_Current_OC_Set();
            OC_Mode Mode = OC_Set_Mode.Get_Current_OC_Mode();

            f1().GB_Status_AppendText_Nextline("R/G/B)Band/Gray : " + band.ToString() + "/" + gray.ToString(), Color.Blue);

            OC_Set_Mode.Band_Radiobutton_Selection(band);

            RGB AM0 = OC_Set_Mode.Get_AM0(band);
            RGB AM1 = OC_Set_Mode.Get_AM1(band);

            vars_update.Initailize();
            vars_update.Update_Gamma_Vreg1_Target_Limit_Extension_And_All_Band_Gray_Gamma(Set, Mode, band, gray);

            bool Is_Init_Algorithm_Applied = false;

            if (Is_OC_Mode1456_and_Initial_RGBVreg1_Algorithm_Apply_Checked(OC_Set_Mode) && band > 0)
                Is_Init_Algorithm_Applied =  Apply_Initial_RGB_Algorithm(OC_Set_Mode, band, gray);

            cmds.Set_and_Send_RGB_CMD(Set, band, gray, vars.Gamma, AM0, AM1);
            OC_Set_Mode.Measure_and_Update_All_Gridviews_OC_Params(band, gray);

            if (Is_Sub_OC_Skip(OC_Set_Mode, band, gray) == false)
            {
                while (vars_update.Is_Sub_OC_Should_Be_Conducted())
                {
                    vars_update.Check_InfiniteLoop_and_Update_ExtensionApplied();

                    OC_Set_Mode.Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, gray, AM0, AM1, Is_Init_Algorithm_Applied);

                    OC_Set_Mode.Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                    if (OC_Set_Mode.Is_OC_Infished()) break;
                }
            }
        }

        private bool Is_OC_Mode1456_and_Initial_RGBVreg1_Algorithm_Apply_Checked(DP213 OC_Set_Mode)
        {
            return (dp213_form().checkBox_Initial_RVreg1B_or_RGB_Algorithm_Apply.Checked &&
                (OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode1
                || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode4
                || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode5
                || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode6));
        }

        private bool Apply_Initial_RGB_Algorithm(DP213 OC_Set_Mode, int band, int gray)
        {
            if(dp213_form().radioButton_MCI_and_3Points.Checked)
            {
                if (gray == 0)
                   return OC_Set_Mode.Gray255_Get_Intial_R_G_B_Using_Previous_Band_Method(band);
                else
                   return OC_Set_Mode.Get_Intial_R_G_B_Using_3Points_Method(band, gray);
            }
            else if(dp213_form().radioButton_LUT_MCI.Checked)
            {
                return OC_Set_Mode.Get_Initial_RGB_Using_LUT_MCI(band, gray);
            }
            else
            {
                return false;
            }
        }

        private bool Is_Sub_OC_Skip(DP213 OC_Set_Mode, int band, int gray)
        {
            if (dp213_form().checkBox_Set23_OC_Skip_If_UV_and_deltaL_Are_within_Specs.Checked)
            {
                if (dp213_form().checkBox_OC_Mode23_UVL_Check.Checked)
                    f1().SB_Append("\n" + "Mode / band / gray / " + OC_Set_Mode.Get_Current_OC_Mode().ToString() + "/" + band.ToString() + "/" + gray.ToString());

                Is_Mode23_OC_Skip_Within_UVL Within_UVL = new Is_Mode23_OC_Skip_Within_UVL(band, gray);

                if (OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode2 && Within_UVL.Is_Mode2_OC_Skip_Within_UVL())
                    return true;
                else if (OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode3 && Within_UVL.Is_Mode3_OC_Skip_Within_UVL())
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }



        protected void Main_Sub_RVreg1B_OC(DP213 OC_Set_Mode, int band)
        {
            int gray = 0;
            Gamma_Set Set = OC_Set_Mode.Get_Current_OC_Set();
            OC_Mode Mode = OC_Set_Mode.Get_Current_OC_Mode();

            f1().GB_Status_AppendText_Nextline("R/Vreg1/B)Band/Gray : " + band.ToString() + "/" + gray.ToString(), Color.Blue);      
            OC_Set_Mode.Band_Radiobutton_Selection(band);

            RGB AM0 = OC_Set_Mode.Get_AM0(band);
            RGB AM1 = OC_Set_Mode.Get_AM1(band);

            vars_update.Initailize();
            vars_update.Update_Gamma_Vreg1_Target_Limit_Extension_And_All_Band_Gray_Gamma(Set, Mode, band, gray);

            bool Is_Init_Algorithm_Applied = false;

            if (OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode1 || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode4
            || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode5 || OC_Set_Mode.Get_Current_OC_Mode() == OC_Mode.Mode6)
            {
                vars.Gamma = storage.Get_All_band_gray_Gamma(Set, (band - 1), gray);

                if (dp213_form().checkBox_Initial_RVreg1B_or_RGB_Algorithm_Apply.Checked && band > 0)
                {
                    if (dp213_form().radioButton_MCI_and_3Points.Checked)
                        Is_Init_Algorithm_Applied = OC_Set_Mode.Gray255_Get_Intial_R_Vreg1_B_Using_Previous_Band_Method(band);
                    else if (dp213_form().radioButton_LUT_MCI.Checked)
                        Is_Init_Algorithm_Applied = OC_Set_Mode.Get_Initial_RVreg1B_Using_LUT_MCI(band);
                }
                      
            }

            cmds.Set_and_Send_Vreg1_and_update_Textbox(Set, band, vars.Vreg1);
            cmds.Set_and_Send_RGB_CMD(Set, band, gray, vars.Gamma, AM0, AM1);
            OC_Set_Mode.Measure_and_Update_All_Gridviews_OC_Params(band, gray);

            if (Is_Sub_OC_Skip(OC_Set_Mode, band, gray) == false)
            {
                while (vars_update.Is_Sub_OC_Should_Be_Conducted())
                {
                    vars_update.Check_Vreg1_InfiniteLoop_and_Update_ExtensionApplied();

                    OC_Set_Mode.Vreg1_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, AM0, AM1, Is_Init_Algorithm_Applied);

                    OC_Set_Mode.Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                    if (OC_Set_Mode.Is_OC_Infished()) break;
                }
            }
        }
    }

    public class DP213_Main_Compensation
    {
        DP213_Mode123_Main_Compensation Mode123_OC;
        DP213_Mode456_Main_Compensation Mode456_OC;

        public DP213_Main_Compensation(DP213_OC_Modes DP213_Objects)
        {
            Mode123_OC = new DP213_Mode123_Main_Compensation(DP213_Objects);
            Mode456_OC = new DP213_Mode456_Main_Compensation(DP213_Objects);
        }

        public void Mode123_Compensation()
        {
            Mode123_OC.Mode123_Compensation();
        }

        public void Mode4_Compensation()
        {
            Mode456_OC.Mode4_Compensation();
        }

        public void Mode5_Compensation()
        {
            Mode456_OC.Mode5_Compensation();
        }

        public void Mode6_Compensation()
        {
            Mode456_OC.Mode6_Compensation();
        }
    }


    public class DP213_Mode456_Main_Compensation : Sub_RGB_Vreg1_OC
    {
        public DP213_Mode456_Main_Compensation(DP213_OC_Modes DP213_Objects)
            : base(DP213_Objects) { }
        
        public void Mode4_Compensation()
        {
            CheckBox checkbox_Is_OC_Skip = dp213_form().checkBox_Mode_4_Skip;
            Set_and_Send_Mode_to_Mode_or_OC(OC_Set_Mode1, OC_Set_Mode4, checkbox_Is_OC_Skip);
        }

        public void Mode5_Compensation()
        {
            CheckBox checkbox_Is_OC_Skip = dp213_form().checkBox_Mode_5_Skip;
            Set_and_Send_Mode_to_Mode_or_OC(OC_Set_Mode2, OC_Set_Mode5, checkbox_Is_OC_Skip);
        }

        public void Mode6_Compensation()
        {
            CheckBox checkbox_Is_OC_Skip = dp213_form().checkBox_Mode_6_Skip;
            Set_and_Send_Mode_to_Mode_or_OC(OC_Set_Mode3, OC_Set_Mode6, checkbox_Is_OC_Skip);
        }


        private void Set_and_Send_Mode_to_Mode_or_OC(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, CheckBox checkbox_Is_OC_Skip)
        {
            To_OC_Set_Mode.Apply_Current_Gamma_Set();

            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount && vars.Optic_Compensation_Stop == false; band++)
            {
                if (Is_Band_OC_Selected(band))
                    Band_OC_Compensation(From_OC_Set_Mode, To_OC_Set_Mode, checkbox_Is_OC_Skip, band);
                else
                    f1().GB_Status_AppendText_Nextline("Band : " + band.ToString() + " OC Skip", Color.Blue);
            }
        }

        private bool Is_Band_OC_Selected(int band)
        {
            return dp213_form().Band_BSQH_Selection(band);
        }

        private void Band_OC_Compensation(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, CheckBox checkbox_Is_OC_Skip, int band)
        {
            dp213_form().DP213_DBV_Setting(band);//DBV Select
            To_OC_Set_Mode.Band_Radiobutton_Selection(band);

            for (int gray = 0; gray < DP213_Static.Max_Gray_Amount && vars.Optic_Compensation_Stop == false; gray++)
            {
                if (Is_OC_Skip_Band(checkbox_Is_OC_Skip, band))
                    Procss_OC_Skip_Band(From_OC_Set_Mode, To_OC_Set_Mode, band, gray);
                else
                {
                    dp213_form().DP213_Pattern_Setting(To_OC_Set_Mode.Get_Current_OC_Mode(), gray, band);
                    Thread.Sleep(300); //Pattern 안정화 Time
                    Process_OC_Band(To_OC_Set_Mode, band, gray);
                }

                if (Is_Only_OC_Gray255(gray))
                    break;
            }

            if (vars.Optic_Compensation_Stop == false)
            vars.Optic_Compensation_Stop = IsOCModeBandGammaReversed(band, To_OC_Set_Mode);

            double[] Grays = dp213_form().DP213_Get_Pattern_Grays(To_OC_Set_Mode.Get_Current_OC_Mode(), band);
            RGB_Double[] RGBvoltages = storage.Get_Band_Set_Gamma_Voltages(To_OC_Set_Mode.Get_Current_OC_Set(), band);

            if (band > 0) DP213_Main_OC_Flow.init_gray_lut.Update_Matched_Grays(band, RGBvoltages, To_OC_Set_Mode.Get_Current_OC_Mode());

            InterpolationFomulaFactory.CreatePrevBand_G2V_Fomula(Grays, RGBvoltages);
            InterpolationFomulaFactory.CreatePrevBand_V2G_Fomula(RGBvoltages, Grays);
            
        }

        private bool IsOCModeBandGammaReversed(int band, DP213 OC_Mode)
        {
            if (ModelFactory.Get_DP213_Instance().IsBandGammaReversed(storage.Get_Band_Set_Gamma_Voltages(OC_Mode.Get_Current_OC_Set(), band)))
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("OC_Mode" + OC_Mode.Get_Current_OC_Mode() +  " band" + band.ToString() + " GammaReversed");
                return true;
            }
            return false;
        }

        private void Process_OC_Band(DP213 To_OC_Set_Mode, int band, int gray)
        {
            if (Is_Within_Skip_Target_Lv(band, gray))
            {
                Set_and_Send_Gamma_AM1_AM0_and_Update_Gridviews(To_OC_Set_Mode, band, gray);
                Measure_For_OC_Skiped_Band_Gray(To_OC_Set_Mode, band, gray);
            }
            else
            {
                if (Is_Mode23456_Vreg1_OC(band, gray))
                    Main_Sub_RVreg1B_OC(To_OC_Set_Mode, band);
                else
                    Main_OC_Sub_RGB_OC(To_OC_Set_Mode, band, gray);
            }
        }

        private bool Is_Mode23456_Vreg1_OC(int band, int gray)
        {
            return (dp213_form().checkBox_Mode1_Vreg1_Compensation.Checked && gray == 0 && band >= 1
                && dp213_form().radioButton_Mode23456_Gray255_RVreg1B_OC.Checked);
        }


        private void Procss_OC_Skip_Band(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, int band, int gray)
        {
            if (Is_Only_OC_Gray255(gray))
                Copy_and_Set_and_Send_Gamma_AM1_AM0_Vreg1(From_OC_Set_Mode, To_OC_Set_Mode, band, gray);
            else
                Copy_and_Set_and_Send_Gamma_AM1_AM0(From_OC_Set_Mode, To_OC_Set_Mode, band, gray);
                
            //Measure_For_OC_Skiped_Band_Gray(To_OC_Set_Mode, band, gray);
        }

        private void Measure_For_OC_Skiped_Band_Gray(DP213 To_OC_Set_Mode, int band, int gray)
        {
            vars.loop_count = 0;
            Thread.Sleep(20);
            f1().CA_Measure_button_Perform_Click(ref vars.Measure.double_X, ref vars.Measure.double_Y, ref vars.Measure.double_Lv);
            dp213_mornitoring().Set_OC_Param_and_OC_Viewer_Measured_Values(gray, band, To_OC_Set_Mode.Get_Current_OC_Mode());
        }

        private bool Is_Within_Skip_Target_Lv(int band,int gray)
        {
            return (Get_OC_Mode1_Target_Lv(band, gray) < dp213_form().Get_OC_Skip_Lv());   
        }

        private double Get_OC_Mode1_Target_Lv(int band, int gray)
        {
            dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, OC_Set_Mode1.Get_Current_OC_Set(), OC_Set_Mode1.Get_Current_OC_Mode());
            return vars.Target.double_Lv;
        }

        private bool Is_OC_Skip_Band(CheckBox checkbox_Is_OC_Skip,int band)
        {
            return (checkbox_Is_OC_Skip.Checked || (band <= dp213_form().Get_Set456_Skip_Max_Band()));
        }

        private void Set_and_Send_Gamma_AM1_AM0_and_Update_Gridviews(DP213 To_OC_Set_Mode,int band,int gray)
        {
            f1().GB_Status_AppendText_Nextline("Band/Gray(" + band.ToString() + "/" + gray.ToString() + ") OC Skip", Color.Blue);
            f1().GB_Status_AppendText_Nextline("Mode1_Target_Lv < OC_Skip_Lv : " + vars.Target.double_Lv.ToString() + "<" + dp213_form().Get_OC_Skip_Lv().ToString(), Color.Blue);

            if (DP213_Static.Is_Not_AOD0_or_HBM_Band(band))
                Set_and_Send_Prev_Band_Gamma_to_Current_Band_and_Update_Gridviews(To_OC_Set_Mode, band, gray);
            else
                Set_and_Send_Current_Band_Gamma_and_Update_Gridviews(To_OC_Set_Mode, band, gray);

        }

        private bool Is_Only_OC_Gray255(int gray)
        {
            return (dp213_form().checkBox_Only_255G.Checked && gray == 0);
        }

        private void Copy_and_Set_and_Send_Vreg1(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, int band)
        {
            int Copied_vreg1 = storage.Get_Normal_Dec_Vreg1(From_OC_Set_Mode.Get_Current_OC_Set(), band);
            cmds.Set_and_Send_Vreg1_and_update_Textbox(To_OC_Set_Mode.Get_Current_OC_Set(), band, Copied_vreg1);
        }

        private void Copy_and_Set_and_Send_Gamma_AM1_AM0(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode,int band,int gray)
        {
            RGB Copied_Gamma = storage.Get_All_band_gray_Gamma(From_OC_Set_Mode.Get_Current_OC_Set(), band, gray);
            RGB Copied_AM0 = From_OC_Set_Mode.Get_AM0(band);
            RGB Copied_AM1 = From_OC_Set_Mode.Get_AM1(band);
            cmds.Set_and_Send_RGB_CMD(To_OC_Set_Mode.Get_Current_OC_Set(), band, gray, Copied_Gamma, Copied_AM0, Copied_AM1);
        }

        private void Copy_and_Set_and_Send_Gamma_AM1_AM0_Vreg1(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, int band, int gray)
        {
            Copy_and_Set_and_Send_Vreg1(From_OC_Set_Mode, To_OC_Set_Mode, band);
            Copy_and_Set_and_Send_Gamma_AM1_AM0(From_OC_Set_Mode, To_OC_Set_Mode, band, gray);
        }


        private void Copy_and_Set_All_band_gray_Gamma(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, int band, int gray)
        {
            RGB Copied_Gamma = storage.Get_All_band_gray_Gamma(From_OC_Set_Mode.Get_Current_OC_Set(), band, gray);
            storage.Set_All_band_gray_Gamma(To_OC_Set_Mode.Get_Current_OC_Set(), band, gray, Copied_Gamma);
        }

        private void Copy_and_Set_AM0_AM1(DP213 From_OC_Set_Mode, DP213 To_OC_Set_Mode, int band)
        {
            RGB Copied_AM0 = From_OC_Set_Mode.Get_AM0(band);
            RGB Copied_AM1 = From_OC_Set_Mode.Get_AM1(band);
            storage.Set_Band_Set_AM0(To_OC_Set_Mode.Get_Current_OC_Set(), band, Copied_AM0);
            storage.Set_Band_Set_AM1(To_OC_Set_Mode.Get_Current_OC_Set(), band, Copied_AM1);
        }
    }

   
    public class DP213_Mode123_Main_Compensation : Sub_RGB_Vreg1_OC
    {
        int mode456_max_skip_band;

        public DP213_Mode123_Main_Compensation(DP213_OC_Modes DP213_Objects)
            : base(DP213_Objects) 
        {
           mode456_max_skip_band = Convert.ToInt16(dp213_form().numericUpDown_Set456_Skip_Max_Band.Value);
        }
        
        public void Mode123_Compensation()
        {
            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount && vars.Optic_Compensation_Stop == false; band++)
            {
                if (dp213_form().Band_BSQH_Selection(band))
                    Mode123_Gray_OC(band);
                else
                    f1().GB_Status_AppendText_Nextline("Band : " + band.ToString() + " OC Skip", Color.Blue);

               
            }
        }

        private void Mode123_Gray_OC(int band)
        {
            dp213_form().DP213_DBV_Setting(band);
            for (int gray = 0; gray < DP213_Static.Max_Gray_Amount && vars.Optic_Compensation_Stop == false; gray++)
            {
                dp213_form().DP213_Pattern_Setting(OC_Mode.Mode1, gray, band);
                Thread.Sleep(300); //Pattern 안정화 Time

                if (Is_Mode123_OC_Gray255_Only_and_Gray_Is_Not_255(gray))
                    break;

                if (Is_Mode123_OC_Skip(band, gray))
                {
                    Set_and_Send_Gamma_and_Copy_Gamma_To_Other_Modes(band, gray);
                    OC_Mode123_Measure_and_Update_Gridviews(band, gray);
                }
                else
                    Mode123_Optic_Compensation_and_Copy_Measure_For_Init_Algorithm(band, gray);
            }

            if(vars.Optic_Compensation_Stop == false)
                 vars.Optic_Compensation_Stop = IsOCMode123BandGammaReversed(band);

            //OC_Mode23 don't need to get init R/G/B/Vreg1 (it just copy from final OC_Mode1)
            double[] Grays = dp213_form().DP213_Get_Pattern_Grays(OC_Mode.Mode1, band);
            RGB_Double[] RGBvoltages = storage.Get_Band_Set_Gamma_Voltages(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode1), band);

           if (band > 0) DP213_Main_OC_Flow.init_gray_lut.Update_Matched_Grays(band, RGBvoltages, OC_Mode.Mode1);

            InterpolationFomulaFactory.CreatePrevBand_G2V_Fomula(Grays, RGBvoltages);
            InterpolationFomulaFactory.CreatePrevBand_V2G_Fomula(RGBvoltages, Grays);


        }


        private bool IsOCMode123BandGammaReversed(int band)
        {
            Model dp213dll = ModelFactory.Get_DP213_Instance();
            if (dp213dll.IsBandGammaReversed(storage.Get_Band_Set_Gamma_Voltages(OC_Set_Mode1.Get_Current_OC_Set(), band)))
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("OC_Mode1 band" + band.ToString() + " GammaReversed");
                return true;
            }

            if (dp213dll.IsBandGammaReversed(storage.Get_Band_Set_Gamma_Voltages(OC_Set_Mode2.Get_Current_OC_Set(), band)))
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("OC_Mode2 band" + band.ToString() + " GammaReversed");
                return true;
            }

            if (dp213dll.IsBandGammaReversed(storage.Get_Band_Set_Gamma_Voltages(OC_Set_Mode3.Get_Current_OC_Set(), band)))
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("OC_Mode3 band" + band.ToString() + " GammaReversed");
                return true;
            }
            return false;
        }

        private void OC_Mode123_Measure_and_Update_Gridviews(int band, int gray)
        {
            Measure_and_Update_Gridviews(OC_Set_Mode1, band, gray);
            Measure_and_Update_Gridviews(OC_Set_Mode2, band, gray);
            Measure_and_Update_Gridviews(OC_Set_Mode3, band, gray);
        }

        private void Measure_and_Update_Gridviews(DP213 OC_Set_Mode,int band,int gray)
        {
            OC_Set_Mode.Band_Radiobutton_Selection(band);
            vars.loop_count = 0;
            OC_Set_Mode.Apply_Current_Gamma_Set();
            Thread.Sleep(20);
            f1().CA_Measure_button_Perform_Click(ref vars.Measure.double_X, ref vars.Measure.double_Y, ref vars.Measure.double_Lv);
            dp213_mornitoring().Update_Gamma_Measure_Loopcount_ExtensionApplied_to_OC_Param_and_OC_Viewer(gray, band, OC_Set_Mode.Get_Current_OC_Mode());
            Application.DoEvents();
        }


        private void Mode123_Optic_Compensation_and_Copy_Measure_For_Init_Algorithm(int band, int gray)
        {
            Mode123_Optic_Compensation(band,gray);

            if (band <= mode456_max_skip_band && dp213_form().checkBox_Initial_RVreg1B_or_RGB_Algorithm_Apply.Checked)
               Copy_Measure_From_Mode123_to_Mode456(band, gray);
        }


        private void Mode123_Optic_Compensation(int band, int gray)
        {
            //OC_Set_Mode1
            OC_Set_Mode1_Compensation(band, gray);
            
            //OC_Set_Mode2
            if (dp213_form().checkBox_Mode_2_Skip.Checked == false)
                OC_Set_Mode2_Compensation(band, gray);

            //OC_Set_Mode3
            if (dp213_form().checkBox_Mode_3_Skip.Checked == false)
                OC_Set_Mode3_Compensation(band, gray);
        }

        private void Copy_Measure_From_Mode123_to_Mode456(int band,int gray)
        {
            dp213_mornitoring().Dual_Copy_Mode1_Measure_To_Mode4_Measure(band,gray);

            if (dp213_form().checkBox_Mode_2_Skip.Checked == false)
                 dp213_mornitoring().Dual_Copy_Mode2_Measure_To_Mode5_Measure(band,gray);

            if (dp213_form().checkBox_Mode_3_Skip.Checked == false)
                dp213_mornitoring().Dual_Copy_Mode3_Measure_To_Mode6_Measure(band, gray);
        }


        private bool Is_Mode123_OC_Gray255_Only_and_Gray_Is_Not_255(int gray)
        {
            return (dp213_form().checkBox_Only_255G.Checked && gray > 0);
        }

        private bool Is_Mode123_OC_Skip(int band,int gray)
        {
            dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, OC_Set_Mode1.Get_Current_OC_Set(), OC_Set_Mode1.Get_Current_OC_Mode());
            return (vars.Target.double_Lv < dp213_form().Get_OC_Skip_Lv());
        }

        private void Set_and_Send_Gamma_and_Copy_Gamma_To_Other_Modes(int band,int gray)
        {
            f1().GB_Status_AppendText_Nextline("Band/Gray(" + band.ToString() + "/" + gray.ToString() + ") OC Skip", Color.Blue);
            f1().GB_Status_AppendText_Nextline("Mode1_Target_Lv < OC_Skip_Lv : " + vars.Target.double_Lv.ToString() + "<" + dp213_form().Get_OC_Skip_Lv().ToString(), Color.Blue);

            if (DP213_Static.Is_Not_AOD0_or_HBM_Band(band)) //Normal(1~11),AOD(1~2)
            {
                Set_and_Send_Prev_Band_Gamma_to_Current_Band_and_Update_Gridviews(OC_Set_Mode1, band, gray);
                Set_and_Send_Prev_Band_Gamma_to_Current_Band_and_Update_Gridviews(OC_Set_Mode2, band, gray);
                Set_and_Send_Prev_Band_Gamma_to_Current_Band_and_Update_Gridviews(OC_Set_Mode3, band, gray);
            }
            else //HBM or AOD0
            {
                Set_and_Send_Current_Band_Gamma_and_Update_Gridviews(OC_Set_Mode1, band, gray);
                Set_and_Send_Current_Band_Gamma_and_Update_Gridviews(OC_Set_Mode2, band, gray);
                Set_and_Send_Current_Band_Gamma_and_Update_Gridviews(OC_Set_Mode3, band, gray);
            }
            OC_Set_Mode123_Copy_Gamma_To_Mode456_If_OC_Skip(band, gray);
        }

        private void OC_Set_Mode1_Compensation(int band, int gray)
        {
            OC_Set_Mode1.Apply_Current_Gamma_Set();

            if (Is_Mode1_Vreg1_OC(band,gray))
                Main_Sub_RVreg1B_OC(OC_Set_Mode1, band);
            else
                Main_OC_Sub_RGB_OC(OC_Set_Mode1, band, gray);
            
            OC_Set_Mode1_Copy_and_Send_Vreg1_to_Mode23456(band, gray);
            OC_Set_Mode1_Copy_Measure_To_Mode23_Target(band, gray);
            OC_Set_Mode1_Copy_Gamma_To_Other_Mode(band, gray);
        }

        private bool Is_Mode1_Vreg1_OC(int band, int gray)
        {
            return (dp213_form().checkBox_Mode1_Vreg1_Compensation.Checked && gray == 0 && band >= 1);
        }

        private bool Is_Mode23456_Vreg1_OC(int band, int gray)
        {
            return (dp213_form().checkBox_Mode1_Vreg1_Compensation.Checked && gray == 0 && band >= 1 
                && dp213_form().radioButton_Mode23456_Gray255_RVreg1B_OC.Checked);
        }

        private void OC_Set_Mode1_Copy_and_Send_Vreg1_to_Mode23456(int band,int gray)
        {
            int Mode1_vreg1 = storage.Get_Normal_Dec_Vreg1(OC_Set_Mode1.Get_Current_OC_Set(), band);
            int[,] Vreg1_Offset = DP213_OC_Offset.getInstance().getVreg1Offset();

            if (dp213_form().checkBox_Copy_Mode1_Vreg1_to_Mode23456.Checked && gray == 0)
            {   
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set1, band, Mode1_vreg1 + Vreg1_Offset[band, 0]);
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set2, band, Mode1_vreg1 + Vreg1_Offset[band, 1]);
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set3, band, Mode1_vreg1 + Vreg1_Offset[band, 2]);
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set4, band, Mode1_vreg1 + Vreg1_Offset[band, 3]);
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set5, band, Mode1_vreg1 + Vreg1_Offset[band, 4]);
                cmds.Set_and_Send_Vreg1_and_update_Textbox(Gamma_Set.Set6, band, Mode1_vreg1 + Vreg1_Offset[band, 5]);
            }
        }

        private void OC_Set_Mode1_Copy_Measure_To_Mode23_Target(int band, int gray)
        {
            if (dp213_form().Checkbox_Copy_Mode1_Measure_to_Mode23_Target.Checked)
            {
                dp213_mornitoring().Dual_Copy_Mode1_Measure_To_Mode23Target(band, gray);
            }
        }

        private void OC_Set_Mode1_Copy_Gamma_To_Other_Mode(int band, int gray)
        {
            if (dp213_form().checkBox_Copy_Mode1_Gamma_to_Mode23.Checked)
            {
                RGB Offset_Mode1_to_Mode2 = DP213_OC_Offset.getInstance().Get_RGB_Offset_From_OCMode1_to_OCMode2()[band, gray];
                RGB Offset_Mode1_to_Mode3 = DP213_OC_Offset.getInstance().Get_RGB_Offset_From_OCMode1_to_OCMode3()[band, gray];

                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode1_to_Mode2_and_Apply_Offset(band, gray, Offset_Mode1_to_Mode2);
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode1_to_Mode3_and_Apply_Offset(band, gray, Offset_Mode1_to_Mode3);
            }

            if (dp213_form().checkBox_Copy_Mode1_Gamma_to_Mode4.Checked)
            {
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode1_to_Mode4(band, gray);
            }
        }

        private bool Is_OC_Mode1_Target_Lv_Within_Max_Min(double Max_Lv,double Min_Lv)
        {
            bool Is_OC_Mode1_Target_Lv_Within_Max_Min = ((vars.Target.double_Lv < Max_Lv) && (vars.Target.double_Lv > Min_Lv));

            if (Is_OC_Mode1_Target_Lv_Within_Max_Min)
                f1().GB_Status_AppendText_Nextline("Green Offset is applied)Max_Lv > OC_Mode1_Target_LV > Min_Lv : " + Max_Lv.ToString() + " > " + vars.Target.double_Lv.ToString() + " > " + Min_Lv.ToString(), Color.Green);
            else
                f1().GB_Status_AppendText_Nextline("Green Offset isn't applied)Max_Lv / OC_Mode1_Target_LV / Min_Lv : " + Max_Lv.ToString() + " / " + vars.Target.double_Lv.ToString() + " / " + Min_Lv.ToString(), Color.Red);
   
            return Is_OC_Mode1_Target_Lv_Within_Max_Min;
        }

        private void OC_Set_Mode123_Copy_Gamma_To_Mode456_If_OC_Skip(int band, int gray)
        {
            if (dp213_form().checkBox_Copy_Mode1_Gamma_to_Mode4.Checked)
            {
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode1_to_Mode4(band, gray);
            }

            if (dp213_form().checkBox_Copy_Mode2_Gamma_to_Mode5.Checked)
            {
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode2_to_Mode5(band, gray);
            }

            if (dp213_form().checkBox_Copy_Mode3_Gamma_to_Mode6.Checked)
            {
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode3_to_Mode6(band, gray);
            }
        }

        private void OC_Set_Mode2_Compensation(int band, int gray)
        {
            OC_Set_Mode2.Apply_Current_Gamma_Set();

            if (Is_Mode23456_Vreg1_OC(band, gray))
                Main_Sub_RVreg1B_OC(OC_Set_Mode2, band);
            else
                Main_OC_Sub_RGB_OC(OC_Set_Mode2, band, gray);

            if (dp213_form().checkBox_Copy_Mode2_Gamma_to_Mode5.Checked)
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode2_to_Mode5(band, gray);
        }

        private void OC_Set_Mode3_Compensation(int band, int gray)
        {
            OC_Set_Mode3.Apply_Current_Gamma_Set();

            if (dp213_form().checkBox_Copy_Mode12_Ave_M_to_Mode3_T.Checked)
                dp213_mornitoring().Copy_Mode1_Mode2_AveMeasure_To_Mode3_Target(band, gray);

            if (Is_Mode23456_Vreg1_OC(band, gray))
                Main_Sub_RVreg1B_OC(OC_Set_Mode3, band);
            else
                Main_OC_Sub_RGB_OC(OC_Set_Mode3, band, gray);

            if (dp213_form().checkBox_Copy_Mode3_Gamma_to_Mode6.Checked)
                dp213_mornitoring().Dual_Mode_Gamma_Copy_Mode3_to_Mode6(band, gray);
        }
    }


    public class Is_Mode23_OC_Skip_Within_UVL : DP213_forms_accessor
    {
        private int band;
        private int gray;

        public Is_Mode23_OC_Skip_Within_UVL(int band, int gray)
        {
            this.band = band;
            this.gray = gray;
        }

        public bool Is_Mode2_OC_Skip_Within_UVL()
        {
            XYLv Mode1_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode1);
            XYLv Mode2_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode2);
            f1().GB_Status_AppendText_Nextline("OC_Mode1_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode1_Measured.X + "/" + Mode1_Measured.Y + "/" + Mode1_Measured.Lv, Color.Blue);
            f1().GB_Status_AppendText_Nextline("OC_Mode2_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode2_Measured.X + "/" + Mode2_Measured.Y + "/" + Mode2_Measured.Lv, Color.Blue);

            return Is_Two_Measured_Are_Within_UVL_Spec(Mode1_Measured, Mode2_Measured);
        }

        public bool Is_Mode3_OC_Skip_Within_UVL()
        {
            if(dp213_form().checkBox_Copy_Mode12_Ave_M_to_Mode3_T.Checked)
            {
                XYLv Mode1_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode1);
                XYLv Mode2_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode2);

                XYLv Mode12_Ave_Measured = new XYLv();
                Mode12_Ave_Measured.double_X = (Mode1_Measured.double_X + Mode2_Measured.double_X) / 2;
                Mode12_Ave_Measured.double_Y = (Mode1_Measured.double_Y + Mode2_Measured.double_Y) / 2;
                Mode12_Ave_Measured.double_Lv = (Mode1_Measured.double_Lv + Mode2_Measured.double_Lv) / 2;
                XYLv Mode3_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode3);

                f1().GB_Status_AppendText_Nextline("OC_Mode12_Ave_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode12_Ave_Measured.X + "/" + Mode12_Ave_Measured.Y + "/" + Mode12_Ave_Measured.Lv, Color.Blue);
                f1().GB_Status_AppendText_Nextline("OC_Mode3_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode3_Measured.X + "/" + Mode3_Measured.Y + "/" + Mode3_Measured.Lv, Color.Blue);
                return Is_Two_Measured_Are_Within_UVL_Spec(Mode12_Ave_Measured, Mode3_Measured);
            }
            else
            {
                XYLv Mode1_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode1);
                XYLv Mode3_Measured = dp213_mornitoring().Get_Mode_Measured_Values(band, gray, OC_Mode.Mode3);
                f1().GB_Status_AppendText_Nextline("OC_Mode1_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode1_Measured.X + "/" + Mode1_Measured.Y + "/" + Mode1_Measured.Lv, Color.Blue);
                f1().GB_Status_AppendText_Nextline("OC_Mode3_Measured B" + band.ToString() + "/" + gray.ToString() + " X/Y/LV : " + Mode3_Measured.X + "/" + Mode3_Measured.Y + "/" + Mode3_Measured.Lv, Color.Blue);
                return Is_Two_Measured_Are_Within_UVL_Spec(Mode1_Measured, Mode3_Measured);
            }
        }

        private bool Is_Two_Measured_Are_Within_UVL_Spec(XYLv Mode1_Measured, XYLv Mode2_or_Mode3_Measured)
        {
            double Diff_Delta_L_Spec = dp213_form().OC_Mode23_Diff_Delta_L_Spec[band, gray];
            double UV_Distance_Limit = dp213_form().OC_Mode23_Diff_Delta_UV_Spec[band, gray];

            bool Delta_L_Spec_In = Compare_Delta_L(Mode1_Measured, Mode2_or_Mode3_Measured, Diff_Delta_L_Spec);
            bool Delta_UV_Spec_In = Compare_Delta_UV(Mode1_Measured, Mode2_or_Mode3_Measured, UV_Distance_Limit);

            if (Delta_L_Spec_In && Delta_UV_Spec_In)
                return true;
            else
                return false;
        }

        private bool Compare_Delta_UV(XYLv Mode1_Measured, XYLv Mode2_or_Mode3_Measured, double UV_Distance_Limit)
        {
            double UV_Distance = Color_Coordinate.Get_UV_Distance_From_XY(Mode1_Measured.double_X, Mode1_Measured.double_Y, Mode2_or_Mode3_Measured.double_X, Mode2_or_Mode3_Measured.double_Y);
            bool UV_Spec_In;

            if (dp213_form().checkBox_OC_Mode23_UVL_Check.Checked)
                f1().SB_Append("/ UV_Distance / " + UV_Distance.ToString());

            if (UV_Distance < UV_Distance_Limit)
            {
                UV_Spec_In = true;
                f1().GB_Status_AppendText_Nextline("UV_Distance(" + UV_Distance.ToString() + ") < UV_Distance_Limit(" + UV_Distance_Limit.ToString() + "), UV_Spec_In = true)", Color.Green);
            }
            else
            {
                UV_Spec_In = false;
                f1().GB_Status_AppendText_Nextline("UV_Distance(" + UV_Distance.ToString() + ") >= UV_Distance_Limit(" + UV_Distance_Limit.ToString() + "), UV_Spec_In = false)", Color.Red);
            }

            return UV_Spec_In;
        }

        private bool Compare_Delta_L(XYLv Mode1_Measured, XYLv Mode2_or_Mode3_Measured, double Diff_Delta_L_Spec)
        {
            double Diff_Measured_Lv = Mode1_Measured.double_Lv - Mode2_or_Mode3_Measured.double_Lv;
            double Delta_L = Math.Abs(Diff_Measured_Lv / (Mode1_Measured.double_Lv));

            if (dp213_form().checkBox_OC_Mode23_UVL_Check.Checked)
                f1().SB_Append("/ L / " + Delta_L.ToString());

            bool Delta_L_Spec_In;

            if (Delta_L < Diff_Delta_L_Spec)
            {
                Delta_L_Spec_In = true;
                f1().GB_Status_AppendText_Nextline("Delta_L(" + Delta_L.ToString() + ") < Diff_Delta_L_Spec(" + Diff_Delta_L_Spec.ToString() + "), Delta_L_Spec_In = true)", Color.Green);
            }
            else
            {
                Delta_L_Spec_In = false;
                f1().GB_Status_AppendText_Nextline("Delta_L(" + Delta_L.ToString() + ") >= Diff_Delta_L_Spec(" + Diff_Delta_L_Spec.ToString() + "), Delta_L_Spec_In = false)", Color.Red);
            }

            return Delta_L_Spec_In;
        }
    }



    public class DP213_OC_Modes : DP213_forms_accessor
    {
        DP213_OC_Values_Storage storage = DP213_OC_Values_Storage.getInstance();
        DP213_Intial_Algorithm_RGBVreg1_CPP init_algorithm_storage_cpp = DP213_Intial_Algorithm_RGBVreg1_CPP.getInstance();
        
        /// <summary>
        /// publics
        /// </summary>
        public DP213_OC_Modes()
        {
            storage.Data_Initializing_As_Zeros();
            init_algorithm_storage_cpp.Data_Initializing_As_Zeros();
            
            Setting_OC_Set_Mode();
            DP213_Dual_Engineering_Mornitoring.getInstance().Show();
            Set_Mornitoring_Mode_Background_Color();
            Set_All_Band_Gray_Gammas_by_reading_from_gridviews();
            dp213_mornitoring().Dual_Mode_GridView_Measure_Loopcount_ExtentionApplied_Clear();
        }

        private void Setting_OC_Set_Mode()
        {
            OC_Set_Mode1 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode1), OC_Mode.Mode1);
            OC_Set_Mode2 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode2), OC_Mode.Mode2);
            OC_Set_Mode3 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode3), OC_Mode.Mode3);
            OC_Set_Mode4 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode4), OC_Mode.Mode4);
            OC_Set_Mode5 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode5), OC_Mode.Mode5);
            OC_Set_Mode6 = new DP213(dp213_form().Get_OC_Mode_Set(OC_Mode.Mode6), OC_Mode.Mode6);
        }


        public DP213 Get_DP213_object(OC_Mode Mode)
        {
            if (Mode == OC_Mode.Mode1) return OC_Set_Mode1;
            else if (Mode == OC_Mode.Mode2) return OC_Set_Mode2;
            else if (Mode == OC_Mode.Mode3) return OC_Set_Mode3;
            else if (Mode == OC_Mode.Mode4) return OC_Set_Mode4;
            else if (Mode == OC_Mode.Mode5) return OC_Set_Mode5;
            else if (Mode == OC_Mode.Mode6) return OC_Set_Mode6;
            else return null;
        }

        /// <summary>
        // Privates
        /// </summary>
        private DP213 OC_Set_Mode1;
        private DP213 OC_Set_Mode2;
        private DP213 OC_Set_Mode3;
        private DP213 OC_Set_Mode4;
        private DP213 OC_Set_Mode5;
        private DP213 OC_Set_Mode6;

        private void Set_All_Band_Gray_Gammas_by_reading_from_gridviews()
        {
            dp213_mornitoring().DP213_Update_All_Band_Gray_Gamma();
        }


        private void Set_Mornitoring_Mode_Background_Color()
        {
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode1, Get_Color_Set(OC_Set_Mode1.Get_Current_OC_Set()));
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode2, Get_Color_Set(OC_Set_Mode2.Get_Current_OC_Set()));
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode3, Get_Color_Set(OC_Set_Mode3.Get_Current_OC_Set()));
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode4, Get_Color_Set(OC_Set_Mode4.Get_Current_OC_Set()));
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode5, Get_Color_Set(OC_Set_Mode5.Get_Current_OC_Set()));
            dp213_mornitoring().Set_Mode_Background_Color(OC_Mode.Mode6, Get_Color_Set(OC_Set_Mode6.Get_Current_OC_Set()));
        }

        

        private Color Get_Color_Set(Gamma_Set Set)
        {
            switch (Set)
            {
                case Gamma_Set.Set1:
                    return DP213_Color_Static.Color_Set1;
                case Gamma_Set.Set2:
                    return DP213_Color_Static.Color_Set2;
                case Gamma_Set.Set3:
                    return DP213_Color_Static.Color_Set3;
                case Gamma_Set.Set4:
                    return DP213_Color_Static.Color_Set4;
                case Gamma_Set.Set5:
                    return DP213_Color_Static.Color_Set5;
                case Gamma_Set.Set6:
                    return DP213_Color_Static.Color_Set6;
                default:
                    return Color.Black;
            }
        }
    }

    public class DP213_forms_accessor 
    {
        protected Form1 f1(){return (Form1)Application.OpenForms["Form1"];}
        protected DP213_Model_Option_Form dp213_form() { return (DP213_Model_Option_Form)Application.OpenForms["DP213_Model_Option_Form"]; }
        protected DP213_Dual_Engineering_Mornitoring dp213_mornitoring() { return (DP213_Dual_Engineering_Mornitoring)Application.OpenForms["DP213_Dual_Engineering_Mornitoring"]; }    
    }

    public class Base_Sub_Option_Compensation : DP213_forms_accessor
    {
        //Commonly used
        protected DP213_CMDS_Write_Read_Update_Variables cmds = DP213_CMDS_Write_Read_Update_Variables.getInstance();
        protected DP213_OC_Values_Storage storage = DP213_OC_Values_Storage.getInstance();
        protected DP213_OC_Current_Variables_Structure vars = DP213_OC_Current_Variables_Structure.getInstance();
        protected DP213_OC_Variables_Update_Algorithm_Interface vars_update = DP213_OC_Variables_Update_Algorithm.getInstance();

        protected void Vreg1_Compensation(bool Is_Init_Algorithm_Applied)
        {
            double Limit_X = vars.Limit.double_X;
            double Limit_Y = vars.Limit.double_Y;
            double Limit_Lv = vars.Limit.double_Lv;

            if (dp213_form().radioButton_Limit_Apply_Ratio2.Checked)
            {
                Limit_X *= 2;
                Limit_Y *= 2;
                Limit_Lv *= 2;
            }
            else if(dp213_form().radioButton_Limit_Apply_Ratio3.Checked)
            {
                Limit_X *= 3;
                Limit_Y *= 3;
                Limit_Lv *= 3;
            }


            if (Is_Init_Algorithm_Applied)
            {
                f1().GB_Status_AppendText_Nextline("Init_Algorithm_Applied_Vreg1_Compensation Applied", Color.Purple);
                Imported_my_cpp_dll.Init_Algorithm_Applied_Vreg1_Compensation(vars.loop_count, vars.Vreg1_Infinite, vars.Vreg1_Infinite_Count, ref vars.Gamma.int_R, ref vars.Vreg1, ref vars.Gamma.int_B, vars.Measure.double_X, vars.Measure.double_Y, vars.Measure.double_Lv, vars.Target.double_X, vars.Target.double_Y, vars.Target.double_Lv
                                                , Limit_X, Limit_Y, Limit_Lv, vars.Extension.double_X, vars.Extension.double_Y, DP213_Static.Gamma_Register_Max, DP213_Static.Vreg1_Register_Max, ref vars.Gamma_Out_Of_Register_Limit, ref vars.Within_Spec_Limit);
            }
            else
                Imported_my_cpp_dll.Vreg1_Compensation(vars.loop_count, vars.Vreg1_Infinite, vars.Vreg1_Infinite_Count, ref vars.Gamma.int_R, ref vars.Vreg1, ref vars.Gamma.int_B, vars.Measure.double_X, vars.Measure.double_Y, vars.Measure.double_Lv, vars.Target.double_X, vars.Target.double_Y, vars.Target.double_Lv
                                                , Limit_X, Limit_Y, Limit_Lv, vars.Extension.double_X, vars.Extension.double_Y, DP213_Static.Gamma_Register_Max, DP213_Static.Vreg1_Register_Max, ref vars.Gamma_Out_Of_Register_Limit, ref vars.Within_Spec_Limit);
        }

        protected void Sub_Compensation(bool Is_Init_Algorithm_Applied)
        {
            double Limit_X = vars.Limit.double_X;
            double Limit_Y = vars.Limit.double_Y;
            double Limit_Lv = vars.Limit.double_Lv;

            if (dp213_form().radioButton_Limit_Apply_Ratio2.Checked)
            {
                Limit_X *= 2;
                Limit_Y *= 2;
                Limit_Lv *= 2;
            }
            else if (dp213_form().radioButton_Limit_Apply_Ratio3.Checked)
            {
                Limit_X *= 3;
                Limit_Y *= 3;
                Limit_Lv *= 3;
            }

            if (Is_Init_Algorithm_Applied)
            {
                f1().GB_Status_AppendText_Nextline("Init_Algorithm_Applied_Sub_Compensation Applied", Color.DarkGoldenrod);
                Imported_my_cpp_dll.Init_Algorithm_Applied_Sub_Compensation(vars.loop_count, vars.Infinite, ref vars.Infinite_Count, ref vars.Gamma.int_R, ref vars.Gamma.int_G, ref vars.Gamma.int_B, vars.Measure.double_X, vars.Measure.double_Y, vars.Measure.double_Lv, vars.Target.double_X, vars.Target.double_Y, vars.Target.double_Lv
                                                    , Limit_X, Limit_Y, Limit_Lv, vars.Extension.double_X, vars.Extension.double_Y, DP213_Static.Gamma_Register_Max, ref vars.Gamma_Out_Of_Register_Limit, ref vars.Within_Spec_Limit);
            }
            else
                Imported_my_cpp_dll.Sub_Compensation(vars.loop_count, vars.Infinite, ref vars.Infinite_Count, ref vars.Gamma.int_R, ref vars.Gamma.int_G, ref vars.Gamma.int_B, vars.Measure.double_X, vars.Measure.double_Y, vars.Measure.double_Lv, vars.Target.double_X, vars.Target.double_Y, vars.Target.double_Lv
                                                        , Limit_X, Limit_Y, Limit_Lv, vars.Extension.double_X, vars.Extension.double_Y, DP213_Static.Gamma_Register_Max, ref vars.Gamma_Out_Of_Register_Limit, ref vars.Within_Spec_Limit);
        }

        protected void Measure_and_Update_Measured_Values()
        {
            f1().CA_Measure_button_Perform_Click(ref vars.Measure.double_X, ref vars.Measure.double_Y, ref vars.Measure.double_Lv);
            f1().Set_And_2D_Drawing_Target_Measure_Limit_XYLv(vars.Target, vars.Measure, vars.Limit, vars.Extension);
            Application.DoEvents();
        }

        protected void Measure_and_Update_Firstly_Measured_Values()
        {
            f1().CA_Measure_button_Perform_Click(ref vars.First_Measure.double_X, ref vars.First_Measure.double_Y, ref vars.First_Measure.double_Lv);
            f1().Set_And_2D_Drawing_Target_Measure_Limit_XYLv(vars.Target, vars.First_Measure, vars.Limit, vars.Extension);
            Application.DoEvents();
        }
    }

    public class DP213 : DP213_Calculating_Init_RGBVreg1_Algorithm, DP213_Optic_Compensation 
    {
        private Single_Gamma_Set_Band_RGB_OC OC;

        public void Band_Radiobutton_Selection(int band)
        {
            OC.Band_Radiobutton_Selection(band);
        }
        public RGB Get_AM0(int band)
        {
            return OC.Get_AM0(band);
        }

        public RGB Get_AM1(int band)
        {
            return OC.Get_AM1(band);
        }


        public void Measure_and_Update_All_Gridviews_OC_Params(int band, int gray)
        {
            OC.Measure_and_Update_All_Gridviews_OC_Params(band, gray);
        }

        public void Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(int band, int gray, RGB AM0, RGB AM1,bool Is_Init_Algorithm_Applied)
        {
            OC.Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, gray, AM0, AM1, Is_Init_Algorithm_Applied);
        }

        public void Vreg1_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(int band, RGB AM0, RGB AM1,bool Is_Init_Algorithm_Applied)
        {
            OC.Vreg1_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, AM0, AM1, Is_Init_Algorithm_Applied);
        }

        public bool Is_OC_Infished()
        {
            return OC.Is_OC_Infished();
        }


        //Current Set related
        private Gamma_Set current_OC_set;
        private OC_Mode current_OC_mode;

        public void Show_Gradation_Pattern_At_Selected_Band(int band)
        {
            Apply_Current_Gamma_Set();
            f1().Display_Gradation_Pattern();
            dp213_form().DP213_DBV_Setting(band);//DBV Select
        }

        public DP213(Gamma_Set Set,OC_Mode Mode)
        {
            current_OC_set = Set;
            current_OC_mode = Mode;
            OC = new Single_Gamma_Set_Band_RGB_OC(Set, Mode);
        }
        public Gamma_Set Get_Current_OC_Set()
        {
            return current_OC_set;
        }
        public OC_Mode Get_Current_OC_Mode()
        {
            return current_OC_mode;
        }

        public void Apply_Current_Gamma_Set()
        {
            dp213_form().Set_Condition_Mipi_Script_Send(current_OC_set);
        }

        //OC Public & privates (specific)
        public void AOD_Compensation()
        {
            DP213_AOD_OC AOD_OC = new DP213_AOD_OC(current_OC_set, current_OC_mode);
            AOD_OC.AOD_Compensation();
        }

        public void ELVSS_and_Vinit2_Compensation()
        {
            Apply_Current_Gamma_Set();
            DP213_ELVSS_and_Vinit2_OC ELVSS_and_Vinit2_OC = new DP213_ELVSS_and_Vinit2_OC(current_OC_set, current_OC_mode);
            ELVSS_and_Vinit2_OC.ELVSS_and_Vinit2_Compensation();

            
        }

        public void VREF0_Compensation()
        {
            Apply_Current_Gamma_Set();
            DP213_VREF0_OC VREF0_OC = new DP213_VREF0_OC(current_OC_set, current_OC_mode);
            VREF0_OC.VREF0_Compensation();
        }

        public void Black_Compensation()
        {
            Apply_Current_Gamma_Set();
            DP213_Black_OC Black_OC = new DP213_Black_OC(current_OC_set);
            Black_OC.Black_Compensation();
        }

        public void AM1_Compensation()
        {
            Apply_Current_Gamma_Set();
            DP213_AM1_OC AM1_OC = new DP213_AM1_OC(current_OC_set, current_OC_mode);
            AM1_OC.AM1_Compensation();
        }

        public bool Gray255_Get_Intial_R_Vreg1_B_Using_Previous_Band_Method(int band)
        {
           return DP213_Gray255_Get_Intial_R_Vreg1_B_Using_Previous_Band_Method(current_OC_set, band, current_OC_mode);
        }

        public bool Gray255_Get_Intial_R_G_B_Using_Previous_Band_Method(int band)
        {
           return DP213_Gray255_Get_Intial_R_G_B_Using_Previous_Band_Method(current_OC_set, band, current_OC_mode);
        }

        public bool Get_Intial_R_G_B_Using_3Points_Method(int band, int gray)
        {
            return DP213_Get_Intial_R_G_B_Using_3Points_Method(current_OC_set, band, gray, current_OC_mode);
        }

        public bool Get_Initial_RGB_Using_LUT_MCI(int band, int gray)
        {
            return Get_Initial_RGB_Using_LUT_MCI(current_OC_set, band, gray, current_OC_mode);
        }

        public bool Get_Initial_RVreg1B_Using_LUT_MCI(int band)
        {
           return  Get_Initial_RVreg1B_Using_LUT_MCI(current_OC_set, band, current_OC_mode);
        }
    }

    public class DP213_AOD_OC : Single_Gamma_Set_Band_RGB_OC
    {
        public DP213_AOD_OC(Gamma_Set Set, OC_Mode Mode) : base(Set, Mode) { }

        public void AOD_Compensation()
        {
            f1().AOD_On(); 
            Thread.Sleep(100);
            
            Sub_AOD_Compensation();
            
            f1().AOD_Off();
            Thread.Sleep(100);
        }

        private void Sub_AOD_Compensation()
        {
            for (int band = 12; band < DP213_Static.Max_Band_Amount; band++)
            {
                if (dp213_form().Band_BSQH_Selection(band))
                {
                    AOD_GammaSet_Band_RGB_OC(band);
                }
                else
                    f1().GB_Status_AppendText_Nextline("Band : " + band.ToString() + " OC Skip", Color.Blue);
            }
        }
    }

    public class DP213_AM1_OC : Single_Gamma_Set_Band_RGB_OC
    {
        public DP213_AM1_OC(Gamma_Set Set, OC_Mode Mode) : base(Set, Mode) { }

        public void AM1_Compensation()
        {
            int band = 0;
            GammaSet_Band_RGB_OC_For_AM1_OC(band);
            Sub_AM1_Compensation(band);
        }

        private void Sub_AM1_Compensation(int band)
        {
            RGB_Double HBM_GR1_Voltage = Get_HBM_GR1_Voltage();
            RGB_Double AM1_Margin = Get_AM1_Margin();
            
            RGB_Double New_AM1_Voltage = Get_New_AM1_Voltage(HBM_GR1_Voltage, AM1_Margin);
            RGB_Double AM0_Voltage = storage.Get_Band_Set_Voltage_AM0(Get_Current_OC_Set(),band);
           
            if (Is_All_AM1_Voltages_Lower_Than_AM0_Voltages(New_AM1_Voltage,AM0_Voltage))
            {
                RGB New_AM1 = storage.AM1_Convert_Voltgae_to_Dec(Get_Current_OC_Set(), band, New_AM1_Voltage);
                storage.Set_All_Band_Set_AM1_By_Applying_Offset(New_AM1);//storage.Set_All_Band_Set_AM1_As_Same_Values(New_AM1);
                Show_AM1_OC_Result();
            }
            else
            {
                vars.Optic_Compensation_Stop = true;
                vars.Optic_Compensation_Succeed = false;
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("(Out of Range)At lease One AM1 > AM0, AM1 Compensation NG");
            }
        }

        private RGB_Double Get_AM1_Margin()
        {
            RGB_Double AM1_Margin = new RGB_Double();
            AM1_Margin.double_R = Convert.ToDouble(dp213_form().textBox_AM1_Margin_R.Text);
            AM1_Margin.double_G = Convert.ToDouble(dp213_form().textBox_AM1_Margin_G.Text);
            AM1_Margin.double_B = Convert.ToDouble(dp213_form().textBox_AM1_Margin_B.Text);
            return AM1_Margin;
        }


        private void Show_AM1_OC_Result()
        {
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.R : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).int_R.ToString(), Color.Red);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.R Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).double_R.ToString(), Color.Red);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.R : " + storage.Get_Band_Set_AM1(Get_Current_OC_Set(), 0).int_R.ToString(), Color.Red);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.R Voltage " + storage.Get_Band_Set_Voltage_AM1(Get_Current_OC_Set(), 0).double_R.ToString(), Color.Red);

            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.G : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).int_G.ToString(), Color.Green);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.G Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).double_G.ToString(), Color.Green);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.G : " + storage.Get_Band_Set_AM1(Get_Current_OC_Set(), 0).int_G.ToString(), Color.Green);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.G Voltage " + storage.Get_Band_Set_Voltage_AM1(Get_Current_OC_Set(), 0).double_G.ToString(), Color.Green);

            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.B : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).int_B.ToString(), Color.Blue);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM GR1.B Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 10).double_B.ToString(), Color.Blue);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.B : " + storage.Get_Band_Set_AM1(Get_Current_OC_Set(), 0).int_B.ToString(), Color.Blue);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM1.B Voltage " + storage.Get_Band_Set_Voltage_AM1(Get_Current_OC_Set(), 0).double_B.ToString(), Color.Blue);
        }



        private void Show_AM1_AM0_Voltages(RGB_Double New_AM1_Voltage,RGB_Double AM0_Voltage)
        {
            f1().GB_Status_AppendText_Nextline("New_AM1_Voltage_R/G/B : " + New_AM1_Voltage.double_R.ToString()
                + "/" + New_AM1_Voltage.double_G.ToString()
                + "/" + New_AM1_Voltage.double_B.ToString(), Color.Blue);
            f1().GB_Status_AppendText_Nextline("AM0_Voltage/G/B : " + AM0_Voltage.double_R.ToString()
                + "/" + AM0_Voltage.double_G.ToString()
                + "/" + AM0_Voltage.double_B.ToString(), Color.Blue);
        }

        private RGB_Double Get_HBM_GR1_Voltage()
        {
            int band = 0;
            int gray = 10;

            return storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), band, gray);
        }

        private RGB_Double Get_New_AM1_Voltage(RGB_Double HBM_GR1_Voltage, RGB_Double AM1_Margin)
        {
            RGB_Double New_AM1_Voltage = new RGB_Double();

            New_AM1_Voltage.double_R = (HBM_GR1_Voltage.double_R + AM1_Margin.double_R);
            New_AM1_Voltage.double_G = (HBM_GR1_Voltage.double_G + AM1_Margin.double_G);
            New_AM1_Voltage.double_B = (HBM_GR1_Voltage.double_B + AM1_Margin.double_B);

            return New_AM1_Voltage;
        }

        bool Is_All_AM1_Voltages_Lower_Than_AM0_Voltages(RGB_Double AM1_Voltage, RGB_Double AM0_Voltage)
        {
            Show_AM1_AM0_Voltages(AM1_Voltage, AM0_Voltage);

            if ((AM1_Voltage.double_R < AM0_Voltage.double_R)
                && (AM1_Voltage.double_G < AM0_Voltage.double_G)
                && (AM1_Voltage.double_B < AM0_Voltage.double_B))
                return true;
            else
            {
                return false;
            }
        }
    }

    public class Single_Gamma_Set_Band_RGB_OC : Band_Gray255_RGB_OC
    {
        public Single_Gamma_Set_Band_RGB_OC(Gamma_Set Set, OC_Mode Mode) : base(Set, Mode) { }
        protected void AOD_GammaSet_Band_RGB_OC(int band)
        {
            Band_Radiobutton_Selection(band);
            dp213_form().DP213_DBV_Setting(band);//DBV Select

            RGB AM0 = Get_AM0(band);
            RGB AM1 = Get_AM1(band);

            for (int gray = 0; gray < DP213_Static.Max_Gray_Amount && vars.Optic_Compensation_Stop == false; gray++)
            {
                dp213_form().DP213_Pattern_Setting(Get_Current_OC_Mode(), gray, band);
                Thread.Sleep(300); //Pattern 안정화 Time

                if (Is_OC_Gray255_Only(gray))
                    break;

                if (Is_OC_Skip(band,gray))
                {
                    Set_and_Send_Gamma_If_OC_Skip(band, gray, AM0, AM1);
                    continue;
                }

                vars_update.Initailize();

                dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, Get_Current_OC_Set(), Get_Current_OC_Mode());
                cmds.Set_and_Send_RGB_CMD(Get_Current_OC_Set(), band, gray, vars.Gamma, AM0, AM1);

                Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                while (vars_update.Is_Sub_OC_Should_Be_Conducted())
                {
                    vars_update.Check_InfiniteLoop_and_Update_ExtensionApplied();

                    Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, gray, AM0, AM1,false);

                    Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                    if (Is_OC_Infished()) break;
                }
            }
        }

        private bool Is_OC_Gray255_Only(int gray)
        {
            return (dp213_form().checkBox_Only_255G.Checked && gray > 0);
        }


        private bool Is_OC_Skip(int band, int gray)
        {
            dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, Get_Current_OC_Set(), Get_Current_OC_Mode());
            return (vars.Target.double_Lv < dp213_form().Get_OC_Skip_Lv());
        }


        private void Set_and_Send_Gamma_If_OC_Skip(int band, int gray, RGB AM0, RGB AM1)
        {
            f1().GB_Status_AppendText_Nextline("AOD)Band/Gray(" + band.ToString() + "/" + gray.ToString() + ") OC Skip", Color.Blue);
            f1().GB_Status_AppendText_Nextline("AOD)Mode1_Target_Lv < OC_Skip_Lv : " + vars.Target.double_Lv.ToString() + "<" + dp213_form().Get_OC_Skip_Lv().ToString(), Color.Blue);

            if (!(DP213_Static.Is_AOD0_or_HBM_Band(band))) //AOD1 or AOD2
                vars.Gamma = storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), (band - 1), gray);

            cmds.Set_and_Send_RGB_CMD(Get_Current_OC_Set(), band, gray, vars.Gamma, AM0, AM1);
            dp213_mornitoring().Set_OC_Param_and_OC_Viewer_Gamma(gray, band, Get_Current_OC_Mode());
        }

        protected void GammaSet_Band_RGB_OC_For_AM1_OC(int band)
        {
            Band_Radiobutton_Selection(band);
            dp213_form().DP213_DBV_Setting(band);//DBV Select

            RGB AM0 = Get_AM0(band);
            RGB AM1 = Get_AM1(band);

            for (int gray = 0; gray < DP213_Static.Max_Gray_Amount && vars.Optic_Compensation_Stop == false; gray++)
            {
                dp213_form().DP213_Pattern_Setting(Get_Current_OC_Mode(), gray, band);
                Thread.Sleep(300); //Pattern 안정화 Time
                vars_update.Initailize();

                dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, Get_Current_OC_Set(), Get_Current_OC_Mode());
                cmds.Set_and_Send_RGB_CMD(Get_Current_OC_Set(), band, gray, vars.Gamma, AM0, AM1);

                Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                while (vars_update.Is_Sub_OC_Should_Be_Conducted())
                {
                    vars_update.Check_InfiniteLoop_and_Update_ExtensionApplied();

                    Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, gray, AM0, AM1,false);

                    Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                    if (Is_OC_Infished()) break;
                }
            }
        }
    }

    public class DP213_VREF0_OC : Band_Gray255_RGB_OC
    {
        public DP213_VREF0_OC(Gamma_Set Set, OC_Mode Mode) : base(Set, Mode) { }

        public void VREF0_Compensation()
        {
            int band = 0;//HBM
            Band_Gray255_RGB_Compensation(band);

            double HBM_RGB_Min_AM2_Voltage = Get_HBM_RGB_Min_AM2_Voltage();
            Sub_VREF0_Compensation(HBM_RGB_Min_AM2_Voltage);
        }

        private double Get_HBM_RGB_Min_AM2_Voltage()
        {
            //Dll Verify
            int band = 0;
            int gray = 0;
            RGB_Double HBM_RGB_AM2_Voltage = storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), band, gray);

            return Get_Min_RGB_Voltage(HBM_RGB_AM2_Voltage);
        }

        private double Get_Min_RGB_Voltage(RGB_Double HBM_RGB_AM2_Voltage)
        {
            double HBM_RGB_Min_AM2_Voltage = HBM_RGB_AM2_Voltage.double_R;
            if (HBM_RGB_Min_AM2_Voltage > HBM_RGB_AM2_Voltage.double_G) HBM_RGB_Min_AM2_Voltage = HBM_RGB_AM2_Voltage.double_G;
            if (HBM_RGB_Min_AM2_Voltage > HBM_RGB_AM2_Voltage.double_B) HBM_RGB_Min_AM2_Voltage = HBM_RGB_AM2_Voltage.double_B;

            return HBM_RGB_Min_AM2_Voltage;
        }

        private void Sub_VREF0_Compensation(double HBM_RGB_Min_AM2_Voltage)
        {
            double VREF0_Margin = Convert.ToDouble(dp213_form().textBox_REF0_Margin.Text);
            double New_VREF0_Voltage = (HBM_RGB_Min_AM2_Voltage - VREF0_Margin);
            f1().GB_Status_AppendText_Nextline("New_VREF0_Voltage : " + New_VREF0_Voltage.ToString(), Color.Blue);

            if (Is_New_VREF0_Overflow(New_VREF0_Voltage))
            {
                vars.Optic_Compensation_Succeed = false;
                vars.Optic_Compensation_Stop = true;
            }
            else
            {
                byte Byte_VREG1_REF0 = Convert.ToByte(Imported_my_cpp_dll.DP213_VREF0_Voltage_to_Dec(New_VREF0_Voltage));
                cmds.Set_and_Send_VREF0_and_update_Textbox(Byte_VREG1_REF0);
                Show_VREF0_OC_Result();
           }
        }

        private void Show_VREF0_OC_Result()
        {
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)Get_Dec_VREF0 : " + storage.Get_Dec_VREF0().ToString(), Color.Black);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)Get_Voltage_VREF0() " + storage.Get_Voltage_VREF0(), Color.Black);

            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.R : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).int_R.ToString(), Color.Red);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.R Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).double_R.ToString(), Color.Red);

            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.G : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).int_G.ToString(), Color.Green);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.G Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).double_G.ToString(), Color.Red);

            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.B : " + storage.Get_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).int_B.ToString(), Color.Blue);
            f1().GB_Status_AppendText_Nextline("After VREF0_OC)HBM AM2.B Voltage " + storage.Get_Voltage_All_band_gray_Gamma(Get_Current_OC_Set(), 0, 0).double_B.ToString(), Color.Red);
        }

        private bool Is_New_VREF0_Overflow(double New_VREF0_Voltage)
        {
            if (New_VREF0_Voltage > 5.25)
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("VREF0 Upper Overflow NG(>5.25)");
                return true;
            }
            else if (New_VREF0_Voltage < 0.25)
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("VREF0 Lower Overflow NG(<0.25)");
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class DP213_ELVSS_and_Vinit2_OC : Band_Gray255_RGB_OC
    {
        int delay;
        int band;
        double ELVSS_Margin;
        double Slope_Margin;
        double ELVSS_Voltage_Max;
        double ELVSS_Voltage_Min;
        double First_ELVSS = -4.5;
        double Last_ELVSS = -2.0;
        bool ELVSS_Find_Finish;
        double ELVSS_Voltage = 0; //Input & Output (ref)
        double First_Slope = 0; //Input & Output (ref)
        double[,] ELVSS_Offset;
        double[,] Vinit2_Offset;

        public DP213_ELVSS_and_Vinit2_OC(Gamma_Set Set, OC_Mode Mode) : base(Set, Mode) 
        {
            ELVSS_Margin = Convert.ToDouble(dp213_form().textBox_ELVSS_Margin.Text);
            Slope_Margin = Convert.ToDouble(dp213_form().textBox_Slope_Margin.Text);
            ELVSS_Voltage_Max = Convert.ToDouble(dp213_form().textBox_ELVSS_Max_Before_Add_Offset.Text);//-4.0v
            ELVSS_Voltage_Min = Convert.ToDouble(dp213_form().textBox_ELVSS_Min_Before_Add_Offset.Text);//-4.5v
            delay = Convert.ToInt32(dp213_form().textBox_ELVSS_CMD_Delay.Text);
            
            ELVSS_Offset = new double[DP213_Static.Max_Set_Amount, DP213_Static.Max_HBM_and_Normal_Band_Amount];
            Update_ELVSS_Offset();
           
            Vinit2_Offset = new double[DP213_Static.Max_Set_Amount, DP213_Static.Max_HBM_and_Normal_Band_Amount];
            Update_Vinit2_Offset();

            int Set_Index = Convert.ToInt16(Set);
            band = 1;

            First_ELVSS = -4.5;//First ELVSS Should be a fixed value(because the slope is fixed value)
            Last_ELVSS = ELVSS_Voltage_Max + (ELVSS_Offset[Set_Index, band] + 0.1); 
        }
        
        private void Update_ELVSS_Offset()
        {
            //Set1
            ELVSS_Offset[0, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set1.Text);
            ELVSS_Offset[0, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set1.Text);
            ELVSS_Offset[0, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set1.Text);
            ELVSS_Offset[0, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set1.Text);
            ELVSS_Offset[0, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set1.Text);
            ELVSS_Offset[0, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set1.Text);
            ELVSS_Offset[0, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set1.Text);
            ELVSS_Offset[0, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set1.Text);
            ELVSS_Offset[0, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set1.Text);
            ELVSS_Offset[0, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set1.Text);
            ELVSS_Offset[0, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set1.Text);
            ELVSS_Offset[0, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set1.Text);

            //Set2
            ELVSS_Offset[1, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set2.Text);
            ELVSS_Offset[1, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set2.Text);
            ELVSS_Offset[1, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set2.Text);
            ELVSS_Offset[1, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set2.Text);
            ELVSS_Offset[1, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set2.Text);
            ELVSS_Offset[1, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set2.Text);
            ELVSS_Offset[1, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set2.Text);
            ELVSS_Offset[1, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set2.Text);
            ELVSS_Offset[1, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set2.Text);
            ELVSS_Offset[1, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set2.Text);
            ELVSS_Offset[1, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set2.Text);
            ELVSS_Offset[1, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set2.Text);

            //Set3
            ELVSS_Offset[2, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set3.Text);
            ELVSS_Offset[2, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set3.Text);
            ELVSS_Offset[2, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set3.Text);
            ELVSS_Offset[2, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set3.Text);
            ELVSS_Offset[2, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set3.Text);
            ELVSS_Offset[2, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set3.Text);
            ELVSS_Offset[2, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set3.Text);
            ELVSS_Offset[2, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set3.Text);
            ELVSS_Offset[2, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set3.Text);
            ELVSS_Offset[2, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set3.Text);
            ELVSS_Offset[2, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set3.Text);
            ELVSS_Offset[2, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set3.Text);

            //Set4
            ELVSS_Offset[3, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set4.Text);
            ELVSS_Offset[3, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set4.Text);
            ELVSS_Offset[3, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set4.Text);
            ELVSS_Offset[3, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set4.Text);
            ELVSS_Offset[3, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set4.Text);
            ELVSS_Offset[3, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set4.Text);
            ELVSS_Offset[3, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set4.Text);
            ELVSS_Offset[3, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set4.Text);
            ELVSS_Offset[3, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set4.Text);
            ELVSS_Offset[3, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set4.Text);
            ELVSS_Offset[3, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set4.Text);
            ELVSS_Offset[3, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set4.Text);

            //Set5
            ELVSS_Offset[4, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set5.Text);
            ELVSS_Offset[4, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set5.Text);
            ELVSS_Offset[4, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set5.Text);
            ELVSS_Offset[4, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set5.Text);
            ELVSS_Offset[4, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set5.Text);
            ELVSS_Offset[4, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set5.Text);
            ELVSS_Offset[4, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set5.Text);
            ELVSS_Offset[4, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set5.Text);
            ELVSS_Offset[4, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set5.Text);
            ELVSS_Offset[4, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set5.Text);
            ELVSS_Offset[4, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set5.Text);
            ELVSS_Offset[4, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set5.Text);

            //Set6
            ELVSS_Offset[5, 0] = Convert.ToDouble(dp213_form().ELVSS_B0_Offset_Set6.Text);
            ELVSS_Offset[5, 1] = Convert.ToDouble(dp213_form().ELVSS_B1_Offset_Set6.Text);
            ELVSS_Offset[5, 2] = Convert.ToDouble(dp213_form().ELVSS_B2_Offset_Set6.Text);
            ELVSS_Offset[5, 3] = Convert.ToDouble(dp213_form().ELVSS_B3_Offset_Set6.Text);
            ELVSS_Offset[5, 4] = Convert.ToDouble(dp213_form().ELVSS_B4_Offset_Set6.Text);
            ELVSS_Offset[5, 5] = Convert.ToDouble(dp213_form().ELVSS_B5_Offset_Set6.Text);
            ELVSS_Offset[5, 6] = Convert.ToDouble(dp213_form().ELVSS_B6_Offset_Set6.Text);
            ELVSS_Offset[5, 7] = Convert.ToDouble(dp213_form().ELVSS_B7_Offset_Set6.Text);
            ELVSS_Offset[5, 8] = Convert.ToDouble(dp213_form().ELVSS_B8_Offset_Set6.Text);
            ELVSS_Offset[5, 9] = Convert.ToDouble(dp213_form().ELVSS_B9_Offset_Set6.Text);
            ELVSS_Offset[5, 10] = Convert.ToDouble(dp213_form().ELVSS_B10_Offset_Set6.Text);
            ELVSS_Offset[5, 11] = Convert.ToDouble(dp213_form().ELVSS_B11_Offset_Set6.Text);
        }

        private void Update_Vinit2_Offset()
        {
            //Set1
            Vinit2_Offset[0, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set1.Text);
            Vinit2_Offset[0, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set1.Text);
            Vinit2_Offset[0, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set1.Text);
            Vinit2_Offset[0, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set1.Text);
            Vinit2_Offset[0, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set1.Text);
            Vinit2_Offset[0, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set1.Text);
            Vinit2_Offset[0, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set1.Text);
            Vinit2_Offset[0, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set1.Text);
            Vinit2_Offset[0, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set1.Text);
            Vinit2_Offset[0, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set1.Text);
            Vinit2_Offset[0, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set1.Text);
            Vinit2_Offset[0, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set1.Text);

            //Set2
            Vinit2_Offset[1, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set2.Text);
            Vinit2_Offset[1, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set2.Text);
            Vinit2_Offset[1, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set2.Text);
            Vinit2_Offset[1, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set2.Text);
            Vinit2_Offset[1, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set2.Text);
            Vinit2_Offset[1, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set2.Text);
            Vinit2_Offset[1, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set2.Text);
            Vinit2_Offset[1, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set2.Text);
            Vinit2_Offset[1, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set2.Text);
            Vinit2_Offset[1, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set2.Text);
            Vinit2_Offset[1, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set2.Text);
            Vinit2_Offset[1, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set2.Text);

            //Set3
            Vinit2_Offset[2, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set3.Text);
            Vinit2_Offset[2, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set3.Text);
            Vinit2_Offset[2, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set3.Text);
            Vinit2_Offset[2, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set3.Text);
            Vinit2_Offset[2, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set3.Text);
            Vinit2_Offset[2, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set3.Text);
            Vinit2_Offset[2, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set3.Text);
            Vinit2_Offset[2, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set3.Text);
            Vinit2_Offset[2, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set3.Text);
            Vinit2_Offset[2, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set3.Text);
            Vinit2_Offset[2, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set3.Text);
            Vinit2_Offset[2, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set3.Text);

            //Set4
            Vinit2_Offset[3, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set4.Text);
            Vinit2_Offset[3, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set4.Text);
            Vinit2_Offset[3, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set4.Text);
            Vinit2_Offset[3, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set4.Text);
            Vinit2_Offset[3, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set4.Text);
            Vinit2_Offset[3, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set4.Text);
            Vinit2_Offset[3, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set4.Text);
            Vinit2_Offset[3, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set4.Text);
            Vinit2_Offset[3, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set4.Text);
            Vinit2_Offset[3, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set4.Text);
            Vinit2_Offset[3, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set4.Text);
            Vinit2_Offset[3, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set4.Text);

            //Set5
            Vinit2_Offset[4, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set5.Text);
            Vinit2_Offset[4, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set5.Text);
            Vinit2_Offset[4, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set5.Text);
            Vinit2_Offset[4, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set5.Text);
            Vinit2_Offset[4, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set5.Text);
            Vinit2_Offset[4, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set5.Text);
            Vinit2_Offset[4, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set5.Text);
            Vinit2_Offset[4, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set5.Text);
            Vinit2_Offset[4, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set5.Text);
            Vinit2_Offset[4, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set5.Text);
            Vinit2_Offset[4, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set5.Text);
            Vinit2_Offset[4, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set5.Text);

            //Set6
            Vinit2_Offset[5, 0] = Convert.ToDouble(dp213_form().Vinit_Offset_B0_Set6.Text);
            Vinit2_Offset[5, 1] = Convert.ToDouble(dp213_form().Vinit_Offset_B1_Set6.Text);
            Vinit2_Offset[5, 2] = Convert.ToDouble(dp213_form().Vinit_Offset_B2_Set6.Text);
            Vinit2_Offset[5, 3] = Convert.ToDouble(dp213_form().Vinit_Offset_B3_Set6.Text);
            Vinit2_Offset[5, 4] = Convert.ToDouble(dp213_form().Vinit_Offset_B4_Set6.Text);
            Vinit2_Offset[5, 5] = Convert.ToDouble(dp213_form().Vinit_Offset_B5_Set6.Text);
            Vinit2_Offset[5, 6] = Convert.ToDouble(dp213_form().Vinit_Offset_B6_Set6.Text);
            Vinit2_Offset[5, 7] = Convert.ToDouble(dp213_form().Vinit_Offset_B7_Set6.Text);
            Vinit2_Offset[5, 8] = Convert.ToDouble(dp213_form().Vinit_Offset_B8_Set6.Text);
            Vinit2_Offset[5, 9] = Convert.ToDouble(dp213_form().Vinit_Offset_B9_Set6.Text);
            Vinit2_Offset[5, 10] = Convert.ToDouble(dp213_form().Vinit_Offset_B10_Set6.Text);
            Vinit2_Offset[5, 11] = Convert.ToDouble(dp213_form().Vinit_Offset_B11_Set6.Text);
        }

        
        public void ELVSS_and_Vinit2_Compensation()
        {
            Band_Gray255_RGB_Compensation(band);
            Sub_ELVSS_and_Vinit2_Compensation();
        }


        private void Sub_ELVSS_and_Vinit2_Compensation()
        {
            ELVSS_Voltage = 0; //Input & Output (ref)
            First_Slope = 0; //Input & Output (ref)
            ELVSS_Find_Finish = false;

            for (ELVSS_Voltage = First_ELVSS; ELVSS_Voltage < Last_ELVSS; ELVSS_Voltage += 0.1)
            {
                cmds.Set_and_Send_ELVSS_CMD_and_update_Textbox(Get_Current_OC_Set(), band, ELVSS_Voltage);
                Thread.Sleep(delay);

                if (ELVSS_Voltage == First_ELVSS)
                {
                    Measure_and_Update_Firstly_Measured_Values();
                }
                else
                {
                    Measure_and_Update_Measured_Values();
                    if (Conduct_ELVSS_OC_and_Check_Is_Finished()) break;
                }
            }

            Set_and_Send_ELVSS_and_Vinit2();
            Set_and_Send_Cold_ELVSS_and_Vinit2();
        }

        private void Set_and_Send_ELVSS_and_Vinit2()
        {
            //Set and send ELVSS 
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode1);
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode2);
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode3);
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode4);
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode5);
            Set_ELVSS_By_Adding_Offset(OC_Mode.Mode6);

            //Set and send Vinit2
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode1);
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode2);
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode3);
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode4);
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode5);
            Set_Vinit2_By_Adding_Offset(OC_Mode.Mode6);
        }

        private void Set_and_Send_Cold_ELVSS_and_Vinit2()
        {
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode1);
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode2);
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode3);
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode4);
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode5);
            Set_Cold_ELVSS_By_Adding_Offset(OC_Mode.Mode6);

            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode1);
            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode2);
            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode3);
            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode4);
            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode5);
            Set_Cold_Vinit2_By_Adding_Offset(OC_Mode.Mode6);
        }

        private void Set_ELVSS_By_Adding_Offset(OC_Mode Mode)
        {
            Gamma_Set Set = dp213_form().Get_OC_Mode_Set(Mode);
            int Set_Index = Convert.ToInt16(Set);

            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount; band++)
                cmds.Set_Voltage_ELVSS_and_and_Update_Textboxes(Set, band, ELVSS_Voltage + ELVSS_Offset[Set_Index, band]);

            cmds.Send_ELVSS_CMD(Set);
            f1().GB_Status_AppendText_Nextline("(Set and Send and update_textboxes) ELVSS_Voltage, OC_Mode : " + Mode.ToString(), Color.Blue);
        }

        private void Set_Cold_ELVSS_By_Adding_Offset(OC_Mode Mode)
        {
            Gamma_Set Set = dp213_form().Get_OC_Mode_Set(Mode);
            
            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount; band++)
            {
                double Cold_ELVSS = (storage.Get_Normal_Voltage_ELVSS(Set, band) - 0.5);
                cmds.Set_Cold_ELVSS(Set, band, Cold_ELVSS);
            }
            cmds.Send_Cold_ELVSS_CMD(Set);
            f1().GB_Status_AppendText_Nextline("(Set and Send) Cold_ELVSS_Voltage, OC_Mode : " + Mode.ToString(), Color.Red);
        }

        private void Set_Vinit2_By_Adding_Offset(OC_Mode Mode)
        {
            Gamma_Set Set = dp213_form().Get_OC_Mode_Set(Mode);
            int Set_Index = Convert.ToInt16(Set);

            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount; band++)
                cmds.Set_Voltage_Vinit2_and_and_Update_Textboxes(Set, band, ELVSS_Voltage + ELVSS_Offset[Set_Index, band] + Vinit2_Offset[Set_Index, band]);
            
            cmds.Send_Vinit2_CMD(Set);
            f1().GB_Status_AppendText_Nextline("(Set and Send and update_textboxes) Vinit2_Voltage, OC_Mode : " + Mode.ToString(), Color.Blue);
        }

        private void Set_Cold_Vinit2_By_Adding_Offset(OC_Mode Mode)
        {
            Gamma_Set Set = dp213_form().Get_OC_Mode_Set(Mode);
            
            for (int band = 0; band < DP213_Static.Max_HBM_and_Normal_Band_Amount; band++)
            {
                double Cold_Vinit2 = (storage.Get_Normal_Voltage_Vinit2(Set, band) - 0.5);
                cmds.Set_Cold_Vinit2(Set, band, Cold_Vinit2);
            }
            cmds.Send_Cold_Vinit2_CMD(Set);
            f1().GB_Status_AppendText_Nextline("(Set and Send) Cold_Vinit2_Voltage, OC_Mode : " + Mode.ToString(), Color.Red);
        }

        private bool Conduct_ELVSS_OC_and_Check_Is_Finished()
        {
            Imported_my_cpp_dll.ELVSS_Compensation_For_DP173(ELVSS_Voltage_Min, ELVSS_Voltage_Max, ref ELVSS_Find_Finish, First_ELVSS, ref ELVSS_Voltage, ref First_Slope, ELVSS_Margin, Slope_Margin, vars.First_Measure.double_X, vars.First_Measure.double_Y
                            , vars.First_Measure.double_Lv, vars.Measure.double_X, vars.Measure.double_Y, vars.Measure.double_Lv);

            if (ELVSS_Find_Finish) return true;
            else return false;
        }

    }




    public class Band_Gray255_RGB_OC : Base_Sub_Option_Compensation
    {
        private Gamma_Set current_OC_set;
        private OC_Mode current_OC_mode;
        public Band_Gray255_RGB_OC(Gamma_Set Set, OC_Mode Mode)
        {
            current_OC_set = Set;
            current_OC_mode = Mode;
        }
        protected Gamma_Set Get_Current_OC_Set()
        {
            return current_OC_set;
        }

        protected OC_Mode Get_Current_OC_Mode()
        {
            return current_OC_mode;
        }

        public RGB Get_AM0(int band)
        {
            return storage.Get_Band_Set_AM0(current_OC_set, band);
        }

        public RGB Get_AM1(int band)
        {
            return storage.Get_Band_Set_AM1(current_OC_set, band);
        }

        public void Band_Radiobutton_Selection(int band)
        {
            dp213_mornitoring().Band_Radiobuttion_Select(band, current_OC_mode);
        }

        protected void Band_Gray255_RGB_Compensation(int band)
        {
            Band_Radiobutton_Selection(band);
            
            int gray = 0;
            RGB AM0 = Get_AM0(band);
            RGB AM1 = Get_AM1(band);

            Send_DBV_and_Display_White_Pattern(band);
            vars_update.Initailize();
           
            dp213_mornitoring().Get_Gamma_Target_Limit_Extention_and_Update_Set_All_band_gray_Gamma(band, gray, current_OC_set, current_OC_mode);
            cmds.Set_and_Send_RGB_CMD(current_OC_set, band, gray, vars.Gamma, AM0, AM1);

            Measure_and_Update_All_Gridviews_OC_Params(band, gray);

            while (vars_update.Is_Sub_OC_Should_Be_Conducted())
            {
                vars_update.Check_InfiniteLoop_and_Update_ExtensionApplied();
                
                Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(band, gray, AM0, AM1,false);

                Measure_and_Update_All_Gridviews_OC_Params(band, gray);

                if (Is_OC_Infished()) break;
            }
        }

        public void Sub_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(int band, int gray, RGB AM0, RGB AM1,bool Is_Init_Algorithm_Applied)
        {
            vars.Prev_Gamma.Equal_Value(vars.Gamma);

            

            Sub_Compensation(Is_Init_Algorithm_Applied);
            
            vars_update.Showing_Diff_and_Current_Gamma();
            
            cmds.Set_and_Send_RGB_CMD(current_OC_set, band, gray, vars.Gamma, AM0, AM1);
        }

        public void Vreg1_Compensation_and_Show_DiffGamma_and_Send_CurrentGamma(int band, RGB AM0, RGB AM1,bool Is_Init_Algorithm_Applied)
        {
            int gray = 0;

            vars.Prev_Gamma.Equal_Value(vars.Gamma);
            vars.Prev_Vreg1 = vars.Vreg1;
            
            Vreg1_Compensation(Is_Init_Algorithm_Applied);
            
            vars_update.Showing_Diff_and_Current_Vreg1_and_Gamma();
            
            cmds.Set_and_Send_RGB_CMD(current_OC_set, band, gray, vars.Gamma, AM0, AM1);
            cmds.Set_and_Send_Vreg1_and_update_Textbox(current_OC_set, band, vars.Vreg1);
        }


        public void Update_Gridviews_and_Loopcount_Textbox(int gray, int band)
        {
            dp213_mornitoring().Update_Gamma_Measure_Loopcount_ExtensionApplied_to_OC_Param_and_OC_Viewer(gray, band, current_OC_mode);
            dp213_form().textBox_loop_count.Text = (++vars.loop_count).ToString();
        }

        private void Send_DBV_and_Display_White_Pattern(int band)
        {
            dp213_form().DP213_DBV_Setting(band);//DBV Select
            f1().PTN_update(255, 255, 255);//White Pattern
            Thread.Sleep(300); //Pattern 안정화 Time
        }



        public void Measure_and_Update_All_Gridviews_OC_Params(int band, int gray)
        {
            Measure_and_Update_Measured_Values();
            Update_Gridviews_and_Loopcount_Textbox(gray, band);
        }

        public bool Is_OC_Infished()
        {
            bool Is_OC_Infished;

            if (vars.Within_Spec_Limit)
            {
                vars.Optic_Compensation_Succeed = true;
                
                Is_OC_Infished = true;
            }

            else if (vars.Gamma_Out_Of_Register_Limit)
            {
                System.Windows.Forms.MessageBox.Show("Gamma or Vreg1 is out of Limit");
                OC_Fail();
                Is_OC_Infished = true;
            }

            else if (vars.loop_count == Convert.ToInt32(dp213_form().textBox_Max_Loop.Text))
            {
                System.Windows.Forms.MessageBox.Show("Loop Count Over");
                OC_Fail();
                Is_OC_Infished = true;
            }

            else
            {
                Is_OC_Infished = false;
            }

            return Is_OC_Infished;
        }

        private void OC_Fail()
        {
            vars.Optic_Compensation_Succeed = false;
            if (dp213_form().checkBox_Continue_After_Fail.Checked == false) vars.Optic_Compensation_Stop = true;
        }
    }



    public class DP213_Black_OC : DP213_forms_accessor
    {
        //Commonly used
        protected DP213_CMDS_Write_Read_Update_Variables cmds = DP213_CMDS_Write_Read_Update_Variables.getInstance();
        protected DP213_OC_Values_Storage storage = DP213_OC_Values_Storage.getInstance();
        protected DP213_OC_Current_Variables_Structure vars = DP213_OC_Current_Variables_Structure.getInstance();
        protected DP213_OC_Variables_Update_Algorithm_Interface vars_update = DP213_OC_Variables_Update_Algorithm.getInstance();

        private Gamma_Set Current_Gamma_Set;
        public DP213_Black_OC(Gamma_Set Set)
        {
            Current_Gamma_Set = Set;
        }

        public void Black_Compensation()
        {
            f1().GB_Status_AppendText_Nextline("Black OC Start", Color.Blue);

            if (Sub_Black_Compensation())
            {
                
            }
            else
            {
                vars.Optic_Compensation_Stop = true;
                vars.Optic_Compensation_Succeed = false;
            }

            f1().GB_Status_AppendText_Nextline("Black OC Finished", Color.Blue);
        }

        

          

        private bool Sub_Black_Compensation()
        {
            Black_OC_Initalize();
            return AM0_Compensation();
        }

        private void Black_OC_Initalize()
        {
            int band = 0;//HBM
            dp213_form().DP213_DBV_Setting(band);//DBV Select
            f1().PTN_update(0, 0, 0);//Black Pattern
            Thread.Sleep(300);
        }

        private bool AM0_Compensation()
        {
            bool AM0_OC_OK = false;

            int band = 0;
            RGB AM0 = new RGB();
            double Black_Limit_Lv = Convert.ToDouble(dp213_form().textBox_Black_Limit_Lv.Text);
            
            int Dec_AM0 = 0;
            AM0.Set_Value(Dec_AM0,Dec_AM0,Dec_AM0);
            cmds.Set_and_Send_AM0(Current_Gamma_Set, band, AM0);

            while (Dec_AM0 <= DP213_Static.AM1_AM0_Max && vars.Optic_Compensation_Stop == false)
            {
                f1().GB_Status_AppendText_Nextline("Dec_AM0 : " + Dec_AM0.ToString(), Color.Blue);

                if (Dec_AM0 == DP213_Static.AM1_AM0_Max)
                {
                    AM0.Set_Value(Dec_AM0,Dec_AM0,Dec_AM0);
                    AM0 = Get_New_AM0(AM0);
                    cmds.Set_and_Send_AM0(Current_Gamma_Set, band, AM0);

                    AM0_OC_OK = true;
                    break;
                }
                else
                {
                    Black_Measure_With_delay20(Black_Limit_Lv);

                    if (vars.Measure.double_Lv < Black_Limit_Lv)
                    {
                        Dec_AM0++;
                        AM0.Set_Value(Dec_AM0, Dec_AM0, Dec_AM0);
                        cmds.Set_and_Send_AM0(Current_Gamma_Set, band, AM0);                        
                        continue;
                    }
                    else
                    {
                        AM0.Set_Value(Dec_AM0, Dec_AM0, Dec_AM0);
                        AM0 = Get_New_AM0(AM0);
                        
                        if (Is_AM0_Out_of_Limit(AM0))
                        {
                            AM0_OC_OK = false;
                            break;
                        }
                        else
                        {
                            cmds.Set_and_Send_AM0(Current_Gamma_Set, band, AM0);
                            AM0_OC_OK = true;
                            storage.Set_All_Band_Set_AM0_As_Same_Values(AM0);
                            break;
                        }
                    }
                }
            }

            Show_Black_OC_Result(AM0_OC_OK, AM0);
            return AM0_OC_OK;
        }



        private bool Is_AM0_Out_of_Limit(RGB AM0)
        {
            return ((AM0.int_R < 0)
                || (AM0.int_G < 0)
                || (AM0.int_B < 0));
        }

        private void Black_Measure_With_delay20(double Black_Limit_Lv)
        {
            Thread.Sleep(20);
            f1().CA_Measure_button_Perform_Click(ref vars.Measure.double_X, ref vars.Measure.double_Y, ref vars.Measure.double_Lv);
            f1().GB_Status_AppendText_Nextline("Measure.double_Lv / Black_Limit_Lv : " + vars.Measure.double_Lv.ToString() + "/" + Black_Limit_Lv.ToString(), Color.Black);

        }

        private RGB Get_New_AM0(RGB AM0)
        {
            double Margin_R = Convert.ToDouble(dp213_form().textBox_AM0_R_Margin.Text);
            double Margin_G = Convert.ToDouble(dp213_form().textBox_AM0_G_Margin.Text);
            double Margin_B = Convert.ToDouble(dp213_form().textBox_AM0_B_Margin.Text);

            double AM0_Resolution = Imported_my_cpp_dll.Get_DP213_EA9155_AM0_Resolution(storage.Get_Dec_VREF0(), storage.Get_Dec_VREF4095());

            AM0.int_R -= Convert.ToInt32(Margin_R / AM0_Resolution);
            AM0.int_G -= Convert.ToInt32(Margin_G / AM0_Resolution);
            AM0.int_B -= Convert.ToInt32(Margin_B / AM0_Resolution);

            return AM0;
        }

        private void Show_Black_OC_Result(bool AM0_OC_OK, RGB AM0)
        {
            if (AM0_OC_OK)
            {
                f1().Show_OK_Message("Black(AM0) Compensation OK");
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_R : " + AM0.int_R.ToString() + " (> 0)", Color.Red);
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_G : " + AM0.int_G.ToString() + " (> 0)", Color.Green);
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_B : " + AM0.int_B.ToString() + " (> 0)", Color.Blue);
            }

            else
            {
                f1().Show_NG_Message_and_pop_up_Messagebox_With_NG_Sound("Black(AM0) Compensation NG");
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_R : " + AM0.int_R.ToString() + " (OK Range : 0 ~ 127)", Color.Red);
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_G : " + AM0.int_G.ToString() + " (OK Range : 0 ~ 127)", Color.Green);
                f1().GB_Status_AppendText_Nextline("Final(Black Margin Applied) AM0_B : " + AM0.int_B.ToString() + " (OK Range : 0 ~ 127)", Color.Blue);
            }
        }
    }
}

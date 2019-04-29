//#define Bob//#define DEVELOPE_HOME////#define DEVELOPE_LAPTOP// 
/******************************************************************************
 * CHANGE LOG
 * ****************************************************************************
 * 
 *    Author:        Bob Cummings
 *    Date:          Wednesday, September 27, 2006 10:40:20 AM
 *    Description:   Removed writting out the end simulation date from
 *                   private void writeXML(string fileName)
 * ****************************************************************************
 *    Description:   Found flaw in calculating the end date for the 
 *                   simulation.  Made change in 
 *                   private void calcSimulationEndDate(DateTime dt)
 * ***************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using txtBoxArray;
using ESRI.ArcGIS.esriSystem;
using bobResize;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using SEARCH;
using log4net;
//using GISHACK;

namespace SEARCH
{
   /// <summary>
   /// Summary description for Form1.
   /// </summary>
   public class frmInput : System.Windows.Forms.Form
   {
		#region Public Members (1) 

		#region Constructors (1) 

       private log4net.ILog mLog = LogManager.GetLogger("formLog");
       private log4net.ILog eLog = LogManager.GetLogger("Error");

      public frmInput()
      {
          try
          {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         ////trash out any former error files
         //if (System.IO.File.Exists(@"C:\DispersalError.log"))
         //   System.IO.File.Delete(@"C:\DispersalError.log");

     //    if(! IsLicensed())
    //     {
    //        MessageBox.Show("This application is not licensed with ARC GIS." +
    //           System.Environment.NewLine + "It will now close");
    //        this.Close();
   //      }
        
         this.tabControl.Width = this.Width;
         myMapManager = MapManager.GetUniqueInstance();
         mySimManager = new SimulatonManager();
         mySimManager.MapManager = myMapManager;
         HideTabPage(this.tabSpecies);
         HideTabPage(this.tabModify);
         HideTabPage(this.tabSim);
         HideTabPage(this.tabHomeRange);
         HideTabPage(this.tabMap);
         this.speciesText = new TextBoxArray();
         this.simulationText = new TextBoxArray();
         this.homeRangeText = new TextBoxArray();
         this.timeText = new TextBoxArray();
         buildTextBoxes(this.tabSpecies, ref this.speciesText);
         buildTextBoxes(this.tabSim, ref this.simulationText);
         buildTextBoxes(this.tabHomeRange, ref this.homeRangeText);
         buildTextBoxes(this.tabTime,ref this.timeText);

         Trace.WriteLine("Just created frmInput");
          }
          catch (SystemException ex)
          {
              eLog.Debug(ex);
          }

         
         
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Methods (1) 

      private void btnParameterFile_Click(object sender, System.EventArgs e)
      {
         try
         {	//this.sfdCommon = new SaveFileDialog();
            //mLog.Debug("inside frmInput.loadMap called from button " + mapType + " map click event");
            this.sfdCommon.Filter = "XML Files *.xml|*.xml";
            this.sfdCommon.Title = "Browse for Parameter File";
				
            if (this.sfdCommon.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {  
               this.parameterFilePath = this.sfdCommon.FileName;
            }
         }
         catch (InvalidMapException ime)
         {
             eLog.Debug(ime.Message);
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

		#endregion Methods 

		#endregion Non-Public Members 


      #region private members
      private string socialDir = "";
      private string foodDir = "";
      private string riskDir = "";
      private string moveDir = "";
      private string disDir = "";
      private string parameterFilePath = "SimData.xml";

      private MapManager myMapManager;
      private TextBoxArray speciesText;
      private TextBoxArray simulationText;
      private TextBoxArray homeRangeText;
      private TextBoxArray timeText;
      private SimulatonManager mySimManager;

      public SimulatonManager MySimManager
      {
          get { return mySimManager; }
           
      }

       

      private System.Windows.Forms.TabControl tabControl;
      private System.Windows.Forms.TabPage tabMap;
      private System.Windows.Forms.TabPage tabSpecies;
      private System.Windows.Forms.TabPage tabModify;
      private System.Windows.Forms.TabPage tabSim;
      private System.Windows.Forms.TabPage tabHomeRange;
      private System.Windows.Forms.TabPage tabTime;

      private System.Windows.Forms.FolderBrowserDialog fdbCommon;
      private System.Windows.Forms.ToolTip myToolTip;

      private System.Windows.Forms.Button btnSocialMaps;
      private System.Windows.Forms.Button btnFoodMaps;
      private System.Windows.Forms.Button btnPredationMap;
      private System.Windows.Forms.Button btnRelease;
      private System.Windows.Forms.Button btnMove;
      private System.Windows.Forms.Button btnNext;
      private System.Windows.Forms.Button btnMaleGender;
      private System.Windows.Forms.Button btnMoveOK;
      private System.Windows.Forms.Button btnFemaleGender;
      private System.Windows.Forms.Button btnOutMapDir;
      private System.Windows.Forms.Button btnSpecies;
      private System.Windows.Forms.Button btnAddHourModifier;
      private System.Windows.Forms.Button btnMapNext;
 
      
      
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.Label lblHourMods;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Label label14;


      
      private System.Windows.Forms.TextBox txtInitialEnergy;
      private System.Windows.Forms.TextBox txtMaxEnergy;
      private System.Windows.Forms.TextBox txtRiskySafeThreshold;
      private System.Windows.Forms.TextBox txtMinEnergy;
      private System.Windows.Forms.TextBox txtVision;
      
      private System.Windows.Forms.TextBox txtSearchForageTrigger;
      private System.Windows.Forms.TextBox txtRiskyToSafe;
      private System.Windows.Forms.TextBox txtSafeToRisky;
      private System.Windows.Forms.TextBox txtResDieTimeStep;
      private System.Windows.Forms.TextBox txtResDieBetweenSeason;

      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.GroupBox groupBox7;


      private System.Windows.Forms.CheckBox chkTextOutPut;
      private System.Windows.Forms.DateTimePicker dateTimePicker1;
      private System.Windows.Forms.Label lblModifiers;
      private System.Windows.Forms.Button btnAddHourlyModifer;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label20;
      private System.Windows.Forms.Label label21;
      private System.Windows.Forms.Button btnDailyModifier;
      private System.Windows.Forms.Button btnSafeSearchModifier;
      private System.Windows.Forms.Button btnRiskyForageModifer;
      private System.Windows.Forms.Button btnSafeForageModifer;
      private System.Windows.Forms.Button btnRiskySearchModifier;
      private System.Windows.Forms.Label label25;
		
      private System.Windows.Forms.GroupBox groupBox8;
      private System.Windows.Forms.TextBox txtMaleHomeRangeArea;
      private System.Windows.Forms.TextBox txtFemaleHomeRangeArea;
      private System.Windows.Forms.Label label26;
      private System.Windows.Forms.Label label27;
      private System.Windows.Forms.Button btnHomeRange;
      private System.Windows.Forms.SaveFileDialog sfdCommon;
      private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
      private System.Windows.Forms.GroupBox groupBox10;
      private System.Windows.Forms.TextBox txtPerception;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.GroupBox groupBox12;
      private System.Windows.Forms.TextBox txtTriggerNum;
      private System.Windows.Forms.Label lblTriggerNum;
      private System.Windows.Forms.RadioButton rdoNumSteps;
      private System.Windows.Forms.RadioButton rdoNumSites;
      private System.Windows.Forms.Button btnAddActiveDuration;
      private System.Windows.Forms.Button btnAddRestDuration;
      private System.Windows.Forms.Label lblDuration;
      private System.Windows.Forms.TextBox txtDurMean;
      private System.Windows.Forms.TextBox txtDurSD;
      private System.Windows.Forms.Label label23;
      private System.Windows.Forms.Label label24;
      private System.Windows.Forms.GroupBox groupBox11;
      private System.Windows.Forms.RadioButton rdoClosest;
      private System.Windows.Forms.RadioButton rdoBestFood;
      private System.Windows.Forms.RadioButton rdoCombo;
      private System.Windows.Forms.RadioButton rdoRisk;
     
      private System.Windows.Forms.GroupBox groupBox13;
      private System.Windows.Forms.Label label33;
      private System.Windows.Forms.TextBox txtEndDaySim;
      private System.Windows.Forms.Label label34;
      private System.Windows.Forms.DateTimePicker SimStartDate;
      private System.Windows.Forms.Label label35;
      private System.Windows.Forms.Label label36;
      private System.Windows.Forms.Label label37;
      private System.Windows.Forms.Label label38;
      private System.Windows.Forms.Label label39;
      private System.Windows.Forms.Button btnTabSimOK;
      private System.Windows.Forms.TextBox txtEndDaySeason;
      private System.Windows.Forms.TextBox txtNumSeasonDays;
      private System.Windows.Forms.TextBox txtTimeBetweenDailyTimeStep;
      private System.Windows.Forms.TextBox txtNumYears;
      private System.Windows.Forms.Button btnTime;
      private System.Windows.Forms.DateTimePicker dtpSeasonStartDate;
      private System.Windows.Forms.GroupBox groupBox6;
      private System.Windows.Forms.GroupBox grpTime;
      private System.Windows.Forms.Button btnSetMove;
      private System.Windows.Forms.Button btnSetRisk;
      private System.Windows.Forms.Button btnSetRelease;
      private System.Windows.Forms.Button btnSetSocial;
      private System.Windows.Forms.Button btnSetFood;
      private System.Windows.Forms.Button btnLoadXML;
      private System.Windows.Forms.ProgressBar pbarSim;
      private System.Windows.Forms.Label label15;
      private System.Windows.Forms.Label label16;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.TextBox txtResOffspringMean;
      private System.Windows.Forms.TextBox txtResFemalePercent;
      private System.Windows.Forms.TextBox txtResBreedPercent;
      private System.Windows.Forms.TextBox txtResOffspringSD;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.GroupBox groupBox14;
      private System.Windows.Forms.Button btnParameterFile;
      private System.Windows.Forms.SaveFileDialog saveFileDialog1;
      private System.Windows.Forms.Label label19;
      private System.Windows.Forms.NumericUpDown nudWakeTime;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.TextBox txtFemaleDistMod;
      private System.Windows.Forms.TextBox txtMaleDistMod;
      private System.ComponentModel.IContainer components;
      #endregion

      #region WindowsCode
      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      //protected override void Dispose(bool disposing)
      //{
      //   if (disposing)
      //   {

      //      if (components != null) 
      //      {
      //         components.Dispose();
      //      }
      //   }
      //   base.Dispose(disposing);
      //}
   
      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
          this.components = new System.ComponentModel.Container();
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInput));
          this.tabControl = new System.Windows.Forms.TabControl();
          this.tabTime = new System.Windows.Forms.TabPage();
          this.btnLoadXML = new System.Windows.Forms.Button();
          this.btnTime = new System.Windows.Forms.Button();
          this.groupBox13 = new System.Windows.Forms.GroupBox();
          this.nudWakeTime = new System.Windows.Forms.NumericUpDown();
          this.label19 = new System.Windows.Forms.Label();
          this.label33 = new System.Windows.Forms.Label();
          this.txtEndDaySim = new System.Windows.Forms.TextBox();
          this.label34 = new System.Windows.Forms.Label();
          this.SimStartDate = new System.Windows.Forms.DateTimePicker();
          this.label35 = new System.Windows.Forms.Label();
          this.txtNumYears = new System.Windows.Forms.TextBox();
          this.label36 = new System.Windows.Forms.Label();
          this.txtTimeBetweenDailyTimeStep = new System.Windows.Forms.TextBox();
          this.label37 = new System.Windows.Forms.Label();
          this.txtEndDaySeason = new System.Windows.Forms.TextBox();
          this.label38 = new System.Windows.Forms.Label();
          this.txtNumSeasonDays = new System.Windows.Forms.TextBox();
          this.label39 = new System.Windows.Forms.Label();
          this.dtpSeasonStartDate = new System.Windows.Forms.DateTimePicker();
          this.tabSpecies = new System.Windows.Forms.TabPage();
          this.lblDuration = new System.Windows.Forms.Label();
          this.groupBox10 = new System.Windows.Forms.GroupBox();
          this.label3 = new System.Windows.Forms.Label();
          this.txtPerception = new System.Windows.Forms.TextBox();
          this.groupBox4 = new System.Windows.Forms.GroupBox();
          this.label20 = new System.Windows.Forms.Label();
          this.txtSafeToRisky = new System.Windows.Forms.TextBox();
          this.label12 = new System.Windows.Forms.Label();
          this.txtRiskyToSafe = new System.Windows.Forms.TextBox();
          this.txtSearchForageTrigger = new System.Windows.Forms.TextBox();
          this.label21 = new System.Windows.Forms.Label();
          this.groupBox3 = new System.Windows.Forms.GroupBox();
          this.label24 = new System.Windows.Forms.Label();
          this.label23 = new System.Windows.Forms.Label();
          this.txtDurSD = new System.Windows.Forms.TextBox();
          this.txtDurMean = new System.Windows.Forms.TextBox();
          this.btnAddRestDuration = new System.Windows.Forms.Button();
          this.btnAddActiveDuration = new System.Windows.Forms.Button();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.label8 = new System.Windows.Forms.Label();
          this.label7 = new System.Windows.Forms.Label();
          this.txtMinEnergy = new System.Windows.Forms.TextBox();
          this.label2 = new System.Windows.Forms.Label();
          this.txtMaxEnergy = new System.Windows.Forms.TextBox();
          this.txtInitialEnergy = new System.Windows.Forms.TextBox();
          this.btnSpecies = new System.Windows.Forms.Button();
          this.tabHomeRange = new System.Windows.Forms.TabPage();
          this.groupBox12 = new System.Windows.Forms.GroupBox();
          this.rdoNumSites = new System.Windows.Forms.RadioButton();
          this.rdoNumSteps = new System.Windows.Forms.RadioButton();
          this.lblTriggerNum = new System.Windows.Forms.Label();
          this.txtTriggerNum = new System.Windows.Forms.TextBox();
          this.btnHomeRange = new System.Windows.Forms.Button();
          this.groupBox8 = new System.Windows.Forms.GroupBox();
          this.label27 = new System.Windows.Forms.Label();
          this.label26 = new System.Windows.Forms.Label();
          this.txtFemaleHomeRangeArea = new System.Windows.Forms.TextBox();
          this.txtMaleHomeRangeArea = new System.Windows.Forms.TextBox();
          this.label6 = new System.Windows.Forms.Label();
          this.txtFemaleDistMod = new System.Windows.Forms.TextBox();
          this.txtMaleDistMod = new System.Windows.Forms.TextBox();
          this.label10 = new System.Windows.Forms.Label();
          this.groupBox11 = new System.Windows.Forms.GroupBox();
          this.rdoCombo = new System.Windows.Forms.RadioButton();
          this.rdoBestFood = new System.Windows.Forms.RadioButton();
          this.rdoRisk = new System.Windows.Forms.RadioButton();
          this.rdoClosest = new System.Windows.Forms.RadioButton();
          this.tabMap = new System.Windows.Forms.TabPage();
          this.grpTime = new System.Windows.Forms.GroupBox();
          this.btnSetMove = new System.Windows.Forms.Button();
          this.btnSetRisk = new System.Windows.Forms.Button();
          this.btnSetFood = new System.Windows.Forms.Button();
          this.btnSetRelease = new System.Windows.Forms.Button();
          this.btnSetSocial = new System.Windows.Forms.Button();
          this.btnMapNext = new System.Windows.Forms.Button();
          this.btnMove = new System.Windows.Forms.Button();
          this.btnRelease = new System.Windows.Forms.Button();
          this.btnPredationMap = new System.Windows.Forms.Button();
          this.btnFoodMaps = new System.Windows.Forms.Button();
          this.btnSocialMaps = new System.Windows.Forms.Button();
          this.groupBox6 = new System.Windows.Forms.GroupBox();
          this.tabModify = new System.Windows.Forms.TabPage();
          this.btnSafeForageModifer = new System.Windows.Forms.Button();
          this.btnSafeSearchModifier = new System.Windows.Forms.Button();
          this.btnRiskyForageModifer = new System.Windows.Forms.Button();
          this.btnRiskySearchModifier = new System.Windows.Forms.Button();
          this.btnDailyModifier = new System.Windows.Forms.Button();
          this.btnAddHourlyModifer = new System.Windows.Forms.Button();
          this.lblModifiers = new System.Windows.Forms.Label();
          this.btnFemaleGender = new System.Windows.Forms.Button();
          this.btnMoveOK = new System.Windows.Forms.Button();
          this.btnMaleGender = new System.Windows.Forms.Button();
          this.tabSim = new System.Windows.Forms.TabPage();
          this.groupBox14 = new System.Windows.Forms.GroupBox();
          this.btnParameterFile = new System.Windows.Forms.Button();
          this.pbarSim = new System.Windows.Forms.ProgressBar();
          this.btnTabSimOK = new System.Windows.Forms.Button();
          this.groupBox7 = new System.Windows.Forms.GroupBox();
          this.btnOutMapDir = new System.Windows.Forms.Button();
          this.chkTextOutPut = new System.Windows.Forms.CheckBox();
          this.groupBox5 = new System.Windows.Forms.GroupBox();
          this.label18 = new System.Windows.Forms.Label();
          this.txtResOffspringSD = new System.Windows.Forms.TextBox();
          this.txtResOffspringMean = new System.Windows.Forms.TextBox();
          this.txtResFemalePercent = new System.Windows.Forms.TextBox();
          this.txtResBreedPercent = new System.Windows.Forms.TextBox();
          this.label25 = new System.Windows.Forms.Label();
          this.label14 = new System.Windows.Forms.Label();
          this.txtResDieBetweenSeason = new System.Windows.Forms.TextBox();
          this.txtResDieTimeStep = new System.Windows.Forms.TextBox();
          this.label15 = new System.Windows.Forms.Label();
          this.label16 = new System.Windows.Forms.Label();
          this.label17 = new System.Windows.Forms.Label();
          this.label11 = new System.Windows.Forms.Label();
          this.label9 = new System.Windows.Forms.Label();
          this.label5 = new System.Windows.Forms.Label();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.label1 = new System.Windows.Forms.Label();
          this.label13 = new System.Windows.Forms.Label();
          this.btnAddHourModifier = new System.Windows.Forms.Button();
          this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
          this.btnNext = new System.Windows.Forms.Button();
          this.txtRiskySafeThreshold = new System.Windows.Forms.TextBox();
          this.label4 = new System.Windows.Forms.Label();
          this.txtVision = new System.Windows.Forms.TextBox();
          this.fdbCommon = new System.Windows.Forms.FolderBrowserDialog();
          this.myToolTip = new System.Windows.Forms.ToolTip(this.components);
          this.lblHourMods = new System.Windows.Forms.Label();
          this.sfdCommon = new System.Windows.Forms.SaveFileDialog();
          this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
          this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
          this.tabControl.SuspendLayout();
          this.tabTime.SuspendLayout();
          this.groupBox13.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.nudWakeTime)).BeginInit();
          this.tabSpecies.SuspendLayout();
          this.groupBox10.SuspendLayout();
          this.groupBox4.SuspendLayout();
          this.groupBox3.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.tabHomeRange.SuspendLayout();
          this.groupBox12.SuspendLayout();
          this.groupBox8.SuspendLayout();
          this.groupBox11.SuspendLayout();
          this.tabMap.SuspendLayout();
          this.grpTime.SuspendLayout();
          this.tabModify.SuspendLayout();
          this.tabSim.SuspendLayout();
          this.groupBox14.SuspendLayout();
          this.groupBox7.SuspendLayout();
          this.groupBox5.SuspendLayout();
          this.SuspendLayout();
          // 
          // tabControl
          // 
          this.tabControl.Controls.Add(this.tabTime);
          this.tabControl.Controls.Add(this.tabSpecies);
          this.tabControl.Controls.Add(this.tabHomeRange);
          this.tabControl.Controls.Add(this.tabMap);
          this.tabControl.Controls.Add(this.tabModify);
          this.tabControl.Controls.Add(this.tabSim);
          this.tabControl.Location = new System.Drawing.Point(-8, 24);
          this.tabControl.Name = "tabControl";
          this.tabControl.SelectedIndex = 0;
          this.tabControl.Size = new System.Drawing.Size(760, 456);
          this.tabControl.TabIndex = 0;
          // 
          // tabTime
          // 
          this.tabTime.Controls.Add(this.btnLoadXML);
          this.tabTime.Controls.Add(this.btnTime);
          this.tabTime.Controls.Add(this.groupBox13);
          this.tabTime.Location = new System.Drawing.Point(4, 22);
          this.tabTime.Name = "tabTime";
          this.tabTime.Size = new System.Drawing.Size(752, 430);
          this.tabTime.TabIndex = 5;
          this.tabTime.Text = "Time Parameters";
          this.tabTime.UseVisualStyleBackColor = true;
          // 
          // btnLoadXML
          // 
          this.btnLoadXML.Location = new System.Drawing.Point(540, 25);
          this.btnLoadXML.Name = "btnLoadXML";
          this.btnLoadXML.Size = new System.Drawing.Size(178, 23);
          this.btnLoadXML.TabIndex = 0;
          this.btnLoadXML.Text = "Load all parameters from XML file";
          this.btnLoadXML.Click += new System.EventHandler(this.btnLoadXML_Click);
          // 
          // btnTime
          // 
          this.btnTime.Location = new System.Drawing.Point(512, 376);
          this.btnTime.Name = "btnTime";
          this.btnTime.Size = new System.Drawing.Size(152, 32);
          this.btnTime.TabIndex = 1;
          this.btnTime.Text = "OK";
          this.btnTime.Click += new System.EventHandler(this.btnTime_Click);
          // 
          // groupBox13
          // 
          this.groupBox13.Controls.Add(this.nudWakeTime);
          this.groupBox13.Controls.Add(this.label19);
          this.groupBox13.Controls.Add(this.label33);
          this.groupBox13.Controls.Add(this.txtEndDaySim);
          this.groupBox13.Controls.Add(this.label34);
          this.groupBox13.Controls.Add(this.SimStartDate);
          this.groupBox13.Controls.Add(this.label35);
          this.groupBox13.Controls.Add(this.txtNumYears);
          this.groupBox13.Controls.Add(this.label36);
          this.groupBox13.Controls.Add(this.txtTimeBetweenDailyTimeStep);
          this.groupBox13.Controls.Add(this.label37);
          this.groupBox13.Controls.Add(this.txtEndDaySeason);
          this.groupBox13.Controls.Add(this.label38);
          this.groupBox13.Controls.Add(this.txtNumSeasonDays);
          this.groupBox13.Controls.Add(this.label39);
          this.groupBox13.Controls.Add(this.dtpSeasonStartDate);
          this.groupBox13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
          this.groupBox13.Location = new System.Drawing.Point(68, 75);
          this.groupBox13.Name = "groupBox13";
          this.groupBox13.Size = new System.Drawing.Size(616, 280);
          this.groupBox13.TabIndex = 3;
          this.groupBox13.TabStop = false;
          this.groupBox13.Text = "Time Parameters";
          // 
          // nudWakeTime
          // 
          this.nudWakeTime.Location = new System.Drawing.Point(19, 234);
          this.nudWakeTime.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
          this.nudWakeTime.Name = "nudWakeTime";
          this.nudWakeTime.Size = new System.Drawing.Size(120, 20);
          this.nudWakeTime.TabIndex = 5;
          // 
          // label19
          // 
          this.label19.AutoSize = true;
          this.label19.Location = new System.Drawing.Point(13, 216);
          this.label19.Name = "label19";
          this.label19.Size = new System.Drawing.Size(136, 13);
          this.label19.TabIndex = 16;
          this.label19.Text = "Initial Wake Up Time (0-23)";
          // 
          // label33
          // 
          this.label33.AutoSize = true;
          this.label33.Enabled = false;
          this.label33.Location = new System.Drawing.Point(447, 216);
          this.label33.Name = "label33";
          this.label33.Size = new System.Drawing.Size(111, 13);
          this.label33.TabIndex = 15;
          this.label33.Text = "End date of simulation";
          // 
          // txtEndDaySim
          // 
          this.txtEndDaySim.BackColor = System.Drawing.Color.White;
          this.txtEndDaySim.Enabled = false;
          this.txtEndDaySim.Location = new System.Drawing.Point(444, 234);
          this.txtEndDaySim.Name = "txtEndDaySim";
          this.txtEndDaySim.ReadOnly = true;
          this.txtEndDaySim.Size = new System.Drawing.Size(152, 20);
          this.txtEndDaySim.TabIndex = 14;
          this.txtEndDaySim.Tag = "Date";
          // 
          // label34
          // 
          this.label34.AutoSize = true;
          this.label34.Location = new System.Drawing.Point(16, 88);
          this.label34.Name = "label34";
          this.label34.Size = new System.Drawing.Size(114, 13);
          this.label34.TabIndex = 13;
          this.label34.Text = "Start date of simulation";
          // 
          // SimStartDate
          // 
          this.SimStartDate.Location = new System.Drawing.Point(16, 104);
          this.SimStartDate.Name = "SimStartDate";
          this.SimStartDate.Size = new System.Drawing.Size(200, 20);
          this.SimStartDate.TabIndex = 1;
          // 
          // label35
          // 
          this.label35.AutoSize = true;
          this.label35.Location = new System.Drawing.Point(232, 88);
          this.label35.Name = "label35";
          this.label35.Size = new System.Drawing.Size(144, 13);
          this.label35.TabIndex = 11;
          this.label35.Text = "Number of years in simulation";
          // 
          // txtNumYears
          // 
          this.txtNumYears.BackColor = System.Drawing.Color.White;
          this.txtNumYears.Location = new System.Drawing.Point(232, 107);
          this.txtNumYears.Name = "txtNumYears";
          this.txtNumYears.Size = new System.Drawing.Size(152, 20);
          this.txtNumYears.TabIndex = 2;
          // 
          // label36
          // 
          this.label36.AutoSize = true;
          this.label36.Location = new System.Drawing.Point(229, 203);
          this.label36.Name = "label36";
          this.label36.Size = new System.Drawing.Size(101, 26);
          this.label36.TabIndex = 9;
          this.label36.Text = "Number of minutes \r\nbetween time steps.";
          // 
          // txtTimeBetweenDailyTimeStep
          // 
          this.txtTimeBetweenDailyTimeStep.Location = new System.Drawing.Point(232, 233);
          this.txtTimeBetweenDailyTimeStep.Name = "txtTimeBetweenDailyTimeStep";
          this.txtTimeBetweenDailyTimeStep.Size = new System.Drawing.Size(152, 20);
          this.txtTimeBetweenDailyTimeStep.TabIndex = 6;
          // 
          // label37
          // 
          this.label37.AutoSize = true;
          this.label37.Enabled = false;
          this.label37.Location = new System.Drawing.Point(447, 152);
          this.label37.Name = "label37";
          this.label37.Size = new System.Drawing.Size(143, 13);
          this.label37.TabIndex = 7;
          this.label37.Text = "End date of dispersal season";
          // 
          // txtEndDaySeason
          // 
          this.txtEndDaySeason.BackColor = System.Drawing.Color.White;
          this.txtEndDaySeason.Enabled = false;
          this.txtEndDaySeason.Location = new System.Drawing.Point(444, 171);
          this.txtEndDaySeason.Name = "txtEndDaySeason";
          this.txtEndDaySeason.ReadOnly = true;
          this.txtEndDaySeason.Size = new System.Drawing.Size(152, 20);
          this.txtEndDaySeason.TabIndex = 6;
          this.txtEndDaySeason.TabStop = false;
          this.txtEndDaySeason.Tag = "Date";
          this.myToolTip.SetToolTip(this.txtEndDaySeason, "True");
          // 
          // label38
          // 
          this.label38.AutoSize = true;
          this.label38.Location = new System.Drawing.Point(229, 139);
          this.label38.Name = "label38";
          this.label38.Size = new System.Drawing.Size(121, 26);
          this.label38.TabIndex = 5;
          this.label38.Text = "Number of days in \r\ndispersal season (1-365)";
          // 
          // txtNumSeasonDays
          // 
          this.txtNumSeasonDays.Location = new System.Drawing.Point(232, 168);
          this.txtNumSeasonDays.Name = "txtNumSeasonDays";
          this.txtNumSeasonDays.Size = new System.Drawing.Size(152, 20);
          this.txtNumSeasonDays.TabIndex = 4;
          this.txtNumSeasonDays.LostFocus += new System.EventHandler(this.txtNumSeasonDays_LostFocus);
          // 
          // label39
          // 
          this.label39.AutoSize = true;
          this.label39.Location = new System.Drawing.Point(16, 152);
          this.label39.Name = "label39";
          this.label39.Size = new System.Drawing.Size(146, 13);
          this.label39.TabIndex = 2;
          this.label39.Text = "Start date of dispersal season";
          // 
          // dtpSeasonStartDate
          // 
          this.dtpSeasonStartDate.CustomFormat = "MMMM dd";
          this.dtpSeasonStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
          this.dtpSeasonStartDate.Location = new System.Drawing.Point(16, 168);
          this.dtpSeasonStartDate.Name = "dtpSeasonStartDate";
          this.dtpSeasonStartDate.Size = new System.Drawing.Size(200, 20);
          this.dtpSeasonStartDate.TabIndex = 3;
          this.myToolTip.SetToolTip(this.dtpSeasonStartDate, "Some Stuff");
          // 
          // tabSpecies
          // 
          this.tabSpecies.Controls.Add(this.lblDuration);
          this.tabSpecies.Controls.Add(this.groupBox10);
          this.tabSpecies.Controls.Add(this.groupBox4);
          this.tabSpecies.Controls.Add(this.groupBox3);
          this.tabSpecies.Controls.Add(this.groupBox1);
          this.tabSpecies.Controls.Add(this.btnSpecies);
          this.tabSpecies.Location = new System.Drawing.Point(4, 22);
          this.tabSpecies.Name = "tabSpecies";
          this.tabSpecies.Size = new System.Drawing.Size(752, 430);
          this.tabSpecies.TabIndex = 1;
          this.tabSpecies.Text = "Species Parameters";
          this.tabSpecies.UseVisualStyleBackColor = true;
          // 
          // lblDuration
          // 
          this.lblDuration.Location = new System.Drawing.Point(520, 144);
          this.lblDuration.Name = "lblDuration";
          this.lblDuration.Size = new System.Drawing.Size(184, 72);
          this.lblDuration.TabIndex = 5;
          // 
          // groupBox10
          // 
          this.groupBox10.Controls.Add(this.label3);
          this.groupBox10.Controls.Add(this.txtPerception);
          this.groupBox10.Location = new System.Drawing.Point(520, 16);
          this.groupBox10.Name = "groupBox10";
          this.groupBox10.Size = new System.Drawing.Size(176, 104);
          this.groupBox10.TabIndex = 1;
          this.groupBox10.TabStop = false;
          this.groupBox10.Text = "Perceptual Window";
          // 
          // label3
          // 
          this.label3.AutoSize = true;
          this.label3.Location = new System.Drawing.Point(16, 40);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(107, 13);
          this.label3.TabIndex = 6;
          this.label3.Text = "Baseline distance (m)";
          // 
          // txtPerception
          // 
          this.txtPerception.Location = new System.Drawing.Point(16, 56);
          this.txtPerception.Name = "txtPerception";
          this.txtPerception.Size = new System.Drawing.Size(128, 20);
          this.txtPerception.TabIndex = 3;
          // 
          // groupBox4
          // 
          this.groupBox4.Controls.Add(this.label20);
          this.groupBox4.Controls.Add(this.txtSafeToRisky);
          this.groupBox4.Controls.Add(this.label12);
          this.groupBox4.Controls.Add(this.txtRiskyToSafe);
          this.groupBox4.Controls.Add(this.txtSearchForageTrigger);
          this.groupBox4.Controls.Add(this.label21);
          this.groupBox4.Location = new System.Drawing.Point(24, 288);
          this.groupBox4.Name = "groupBox4";
          this.groupBox4.Size = new System.Drawing.Size(480, 112);
          this.groupBox4.TabIndex = 3;
          this.groupBox4.TabStop = false;
          this.groupBox4.Text = "Behavioral Triggers";
          // 
          // label20
          // 
          this.label20.AutoSize = true;
          this.label20.Location = new System.Drawing.Point(305, 27);
          this.label20.Name = "label20";
          this.label20.Size = new System.Drawing.Size(90, 26);
          this.label20.TabIndex = 5;
          this.label20.Text = "Probability of safe\r\nbehavior";
          // 
          // txtSafeToRisky
          // 
          this.txtSafeToRisky.Location = new System.Drawing.Point(167, 56);
          this.txtSafeToRisky.Name = "txtSafeToRisky";
          this.txtSafeToRisky.Size = new System.Drawing.Size(100, 20);
          this.txtSafeToRisky.TabIndex = 10;
          this.myToolTip.SetToolTip(this.txtSafeToRisky, "example: (Roll of Dice – Observed Risk) < 0.000001");
          // 
          // label12
          // 
          this.label12.AutoSize = true;
          this.label12.Location = new System.Drawing.Point(164, 27);
          this.label12.Name = "label12";
          this.label12.Size = new System.Drawing.Size(94, 26);
          this.label12.TabIndex = 4;
          this.label12.Text = "Probability of risky \r\nbehavior";
          // 
          // txtRiskyToSafe
          // 
          this.txtRiskyToSafe.Location = new System.Drawing.Point(308, 56);
          this.txtRiskyToSafe.Name = "txtRiskyToSafe";
          this.txtRiskyToSafe.Size = new System.Drawing.Size(100, 20);
          this.txtRiskyToSafe.TabIndex = 11;
          this.myToolTip.SetToolTip(this.txtRiskyToSafe, "Example: (Roll of Dice – Observed Risk) > 0.0001");
          // 
          // txtSearchForageTrigger
          // 
          this.txtSearchForageTrigger.Location = new System.Drawing.Point(24, 56);
          this.txtSearchForageTrigger.Name = "txtSearchForageTrigger";
          this.txtSearchForageTrigger.Size = new System.Drawing.Size(100, 20);
          this.txtSearchForageTrigger.TabIndex = 9;
          // 
          // label21
          // 
          this.label21.AutoSize = true;
          this.label21.Location = new System.Drawing.Point(24, 27);
          this.label21.Name = "label21";
          this.label21.Size = new System.Drawing.Size(85, 26);
          this.label21.TabIndex = 0;
          this.label21.Text = "Search/forage \r\nenergy threshold";
          // 
          // groupBox3
          // 
          this.groupBox3.Controls.Add(this.label24);
          this.groupBox3.Controls.Add(this.label23);
          this.groupBox3.Controls.Add(this.txtDurSD);
          this.groupBox3.Controls.Add(this.txtDurMean);
          this.groupBox3.Controls.Add(this.btnAddRestDuration);
          this.groupBox3.Controls.Add(this.btnAddActiveDuration);
          this.groupBox3.Location = new System.Drawing.Point(24, 136);
          this.groupBox3.Name = "groupBox3";
          this.groupBox3.Size = new System.Drawing.Size(456, 136);
          this.groupBox3.TabIndex = 2;
          this.groupBox3.TabStop = false;
          this.groupBox3.Text = "Activity Periods ";
          // 
          // label24
          // 
          this.label24.AutoSize = true;
          this.label24.Location = new System.Drawing.Point(88, 32);
          this.label24.Name = "label24";
          this.label24.Size = new System.Drawing.Size(89, 13);
          this.label24.TabIndex = 19;
          this.label24.Text = "Mean length (hrs)";
          // 
          // label23
          // 
          this.label23.AutoSize = true;
          this.label23.Location = new System.Drawing.Point(275, 32);
          this.label23.Name = "label23";
          this.label23.Size = new System.Drawing.Size(119, 13);
          this.label23.TabIndex = 18;
          this.label23.Text = "Standard deviation (hrs)";
          // 
          // txtDurSD
          // 
          this.txtDurSD.Location = new System.Drawing.Point(280, 56);
          this.txtDurSD.Name = "txtDurSD";
          this.txtDurSD.Size = new System.Drawing.Size(114, 20);
          this.txtDurSD.TabIndex = 6;
          // 
          // txtDurMean
          // 
          this.txtDurMean.Location = new System.Drawing.Point(80, 56);
          this.txtDurMean.Name = "txtDurMean";
          this.txtDurMean.Size = new System.Drawing.Size(104, 20);
          this.txtDurMean.TabIndex = 5;
          // 
          // btnAddRestDuration
          // 
          this.btnAddRestDuration.Enabled = false;
          this.btnAddRestDuration.Location = new System.Drawing.Point(280, 80);
          this.btnAddRestDuration.Name = "btnAddRestDuration";
          this.btnAddRestDuration.Size = new System.Drawing.Size(114, 32);
          this.btnAddRestDuration.TabIndex = 8;
          this.btnAddRestDuration.Text = "Add Resting Period";
          this.btnAddRestDuration.Click += new System.EventHandler(this.btnAddRestDuration_Click);
          // 
          // btnAddActiveDuration
          // 
          this.btnAddActiveDuration.Location = new System.Drawing.Point(80, 80);
          this.btnAddActiveDuration.Name = "btnAddActiveDuration";
          this.btnAddActiveDuration.Size = new System.Drawing.Size(104, 32);
          this.btnAddActiveDuration.TabIndex = 7;
          this.btnAddActiveDuration.Text = "Add Active Period";
          this.btnAddActiveDuration.Click += new System.EventHandler(this.btnAddActiveDuration_Click);
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this.label8);
          this.groupBox1.Controls.Add(this.label7);
          this.groupBox1.Controls.Add(this.txtMinEnergy);
          this.groupBox1.Controls.Add(this.label2);
          this.groupBox1.Controls.Add(this.txtMaxEnergy);
          this.groupBox1.Controls.Add(this.txtInitialEnergy);
          this.groupBox1.Location = new System.Drawing.Point(24, 16);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(464, 104);
          this.groupBox1.TabIndex = 0;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Energy Parameters";
          // 
          // label8
          // 
          this.label8.AutoSize = true;
          this.label8.Location = new System.Drawing.Point(325, 40);
          this.label8.Name = "label8";
          this.label8.Size = new System.Drawing.Size(83, 13);
          this.label8.TabIndex = 5;
          this.label8.Text = "Minimum energy";
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(24, 40);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(66, 13);
          this.label7.TabIndex = 4;
          this.label7.Text = "Initial energy";
          // 
          // txtMinEnergy
          // 
          this.txtMinEnergy.Location = new System.Drawing.Point(328, 56);
          this.txtMinEnergy.Name = "txtMinEnergy";
          this.txtMinEnergy.Size = new System.Drawing.Size(100, 20);
          this.txtMinEnergy.TabIndex = 2;
          this.myToolTip.SetToolTip(this.txtMinEnergy, "The min amount of energy before starvation");
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(181, 40);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(86, 13);
          this.label2.TabIndex = 3;
          this.label2.Text = "Maximum energy";
          // 
          // txtMaxEnergy
          // 
          this.txtMaxEnergy.Location = new System.Drawing.Point(184, 56);
          this.txtMaxEnergy.Name = "txtMaxEnergy";
          this.txtMaxEnergy.Size = new System.Drawing.Size(100, 20);
          this.txtMaxEnergy.TabIndex = 1;
          this.myToolTip.SetToolTip(this.txtMaxEnergy, "Max amount of energy the species can hold");
          // 
          // txtInitialEnergy
          // 
          this.txtInitialEnergy.Location = new System.Drawing.Point(24, 56);
          this.txtInitialEnergy.Name = "txtInitialEnergy";
          this.txtInitialEnergy.Size = new System.Drawing.Size(100, 20);
          this.txtInitialEnergy.TabIndex = 0;
          this.myToolTip.SetToolTip(this.txtInitialEnergy, "Iniatial amount of energy for day one");
          // 
          // btnSpecies
          // 
          this.btnSpecies.Enabled = false;
          this.btnSpecies.Location = new System.Drawing.Point(544, 360);
          this.btnSpecies.Name = "btnSpecies";
          this.btnSpecies.Size = new System.Drawing.Size(128, 40);
          this.btnSpecies.TabIndex = 12;
          this.btnSpecies.Text = "OK";
          this.btnSpecies.Click += new System.EventHandler(this.btnSpecies_Click);
          // 
          // tabHomeRange
          // 
          this.tabHomeRange.Controls.Add(this.groupBox12);
          this.tabHomeRange.Controls.Add(this.btnHomeRange);
          this.tabHomeRange.Controls.Add(this.groupBox8);
          this.tabHomeRange.Controls.Add(this.groupBox11);
          this.tabHomeRange.Location = new System.Drawing.Point(4, 22);
          this.tabHomeRange.Name = "tabHomeRange";
          this.tabHomeRange.Size = new System.Drawing.Size(752, 430);
          this.tabHomeRange.TabIndex = 4;
          this.tabHomeRange.Text = "Home Range Parameters";
          this.tabHomeRange.UseVisualStyleBackColor = true;
          // 
          // groupBox12
          // 
          this.groupBox12.Controls.Add(this.rdoNumSites);
          this.groupBox12.Controls.Add(this.rdoNumSteps);
          this.groupBox12.Controls.Add(this.lblTriggerNum);
          this.groupBox12.Controls.Add(this.txtTriggerNum);
          this.groupBox12.Location = new System.Drawing.Point(376, 32);
          this.groupBox12.Name = "groupBox12";
          this.groupBox12.Size = new System.Drawing.Size(352, 112);
          this.groupBox12.TabIndex = 3;
          this.groupBox12.TabStop = false;
          this.groupBox12.Text = "Home Range Selection Trigger";
          // 
          // rdoNumSites
          // 
          this.rdoNumSites.Location = new System.Drawing.Point(24, 56);
          this.rdoNumSites.Name = "rdoNumSites";
          this.rdoNumSites.Size = new System.Drawing.Size(178, 24);
          this.rdoNumSites.TabIndex = 5;
          this.rdoNumSites.Text = "Number of suitable sites visited";
          // 
          // rdoNumSteps
          // 
          this.rdoNumSteps.Checked = true;
          this.rdoNumSteps.Location = new System.Drawing.Point(24, 24);
          this.rdoNumSteps.Name = "rdoNumSteps";
          this.rdoNumSteps.Size = new System.Drawing.Size(136, 24);
          this.rdoNumSteps.TabIndex = 4;
          this.rdoNumSteps.TabStop = true;
          this.rdoNumSteps.Text = "Number of active steps taken";
          this.rdoNumSteps.CheckedChanged += new System.EventHandler(this.rdoNumSteps_CheckedChanged);
          // 
          // lblTriggerNum
          // 
          this.lblTriggerNum.Location = new System.Drawing.Point(208, 32);
          this.lblTriggerNum.Name = "lblTriggerNum";
          this.lblTriggerNum.Size = new System.Drawing.Size(104, 24);
          this.lblTriggerNum.TabIndex = 3;
          this.lblTriggerNum.Text = "Number of Steps";
          // 
          // txtTriggerNum
          // 
          this.txtTriggerNum.Location = new System.Drawing.Point(208, 64);
          this.txtTriggerNum.Name = "txtTriggerNum";
          this.txtTriggerNum.Size = new System.Drawing.Size(80, 20);
          this.txtTriggerNum.TabIndex = 2;
          // 
          // btnHomeRange
          // 
          this.btnHomeRange.Location = new System.Drawing.Point(496, 344);
          this.btnHomeRange.Name = "btnHomeRange";
          this.btnHomeRange.Size = new System.Drawing.Size(136, 40);
          this.btnHomeRange.TabIndex = 2;
          this.btnHomeRange.Text = "OK";
          this.btnHomeRange.Click += new System.EventHandler(this.btnHomeRange_Click);
          // 
          // groupBox8
          // 
          this.groupBox8.Controls.Add(this.label27);
          this.groupBox8.Controls.Add(this.label26);
          this.groupBox8.Controls.Add(this.txtFemaleHomeRangeArea);
          this.groupBox8.Controls.Add(this.txtMaleHomeRangeArea);
          this.groupBox8.Controls.Add(this.label6);
          this.groupBox8.Controls.Add(this.txtFemaleDistMod);
          this.groupBox8.Controls.Add(this.txtMaleDistMod);
          this.groupBox8.Controls.Add(this.label10);
          this.groupBox8.Location = new System.Drawing.Point(40, 32);
          this.groupBox8.Name = "groupBox8";
          this.groupBox8.Size = new System.Drawing.Size(296, 360);
          this.groupBox8.TabIndex = 0;
          this.groupBox8.TabStop = false;
          this.groupBox8.Text = "Home Range Parameters";
          // 
          // label27
          // 
          this.label27.Location = new System.Drawing.Point(152, 32);
          this.label27.Name = "label27";
          this.label27.Size = new System.Drawing.Size(112, 16);
          this.label27.TabIndex = 3;
          this.label27.Text = "Females";
          this.label27.TextAlign = System.Drawing.ContentAlignment.TopCenter;
          // 
          // label26
          // 
          this.label26.Location = new System.Drawing.Point(24, 32);
          this.label26.Name = "label26";
          this.label26.Size = new System.Drawing.Size(112, 16);
          this.label26.TabIndex = 2;
          this.label26.Text = "Males";
          this.label26.TextAlign = System.Drawing.ContentAlignment.TopCenter;
          // 
          // txtFemaleHomeRangeArea
          // 
          this.txtFemaleHomeRangeArea.Location = new System.Drawing.Point(152, 72);
          this.txtFemaleHomeRangeArea.Name = "txtFemaleHomeRangeArea";
          this.txtFemaleHomeRangeArea.Size = new System.Drawing.Size(112, 20);
          this.txtFemaleHomeRangeArea.TabIndex = 1;
          this.txtFemaleHomeRangeArea.Validating += new System.ComponentModel.CancelEventHandler(this.txtFemaleHomeRangeArea_Validating);
          // 
          // txtMaleHomeRangeArea
          // 
          this.txtMaleHomeRangeArea.Location = new System.Drawing.Point(24, 72);
          this.txtMaleHomeRangeArea.Name = "txtMaleHomeRangeArea";
          this.txtMaleHomeRangeArea.Size = new System.Drawing.Size(112, 20);
          this.txtMaleHomeRangeArea.TabIndex = 0;
          this.txtMaleHomeRangeArea.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaleHomeRangeArea_Validating);
          // 
          // label6
          // 
          this.label6.Location = new System.Drawing.Point(21, 56);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(243, 16);
          this.label6.TabIndex = 2;
          this.label6.Text = "Minimum home range area (sq. km)";
          this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
          // 
          // txtFemaleDistMod
          // 
          this.txtFemaleDistMod.Location = new System.Drawing.Point(152, 157);
          this.txtFemaleDistMod.Name = "txtFemaleDistMod";
          this.txtFemaleDistMod.Size = new System.Drawing.Size(112, 20);
          this.txtFemaleDistMod.TabIndex = 1;
          // 
          // txtMaleDistMod
          // 
          this.txtMaleDistMod.Location = new System.Drawing.Point(24, 157);
          this.txtMaleDistMod.Name = "txtMaleDistMod";
          this.txtMaleDistMod.Size = new System.Drawing.Size(112, 20);
          this.txtMaleDistMod.TabIndex = 0;
          // 
          // label10
          // 
          this.label10.Location = new System.Drawing.Point(24, 125);
          this.label10.Name = "label10";
          this.label10.Size = new System.Drawing.Size(240, 32);
          this.label10.TabIndex = 2;
          this.label10.Text = "Distance weighting factor\r\n(Used in conjunction with selection criteria)";
          this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
          // 
          // groupBox11
          // 
          this.groupBox11.Controls.Add(this.rdoCombo);
          this.groupBox11.Controls.Add(this.rdoBestFood);
          this.groupBox11.Controls.Add(this.rdoRisk);
          this.groupBox11.Controls.Add(this.rdoClosest);
          this.groupBox11.Location = new System.Drawing.Point(376, 168);
          this.groupBox11.Name = "groupBox11";
          this.groupBox11.Size = new System.Drawing.Size(352, 104);
          this.groupBox11.TabIndex = 8;
          this.groupBox11.TabStop = false;
          this.groupBox11.Text = "Home Range Selection Criteria";
          // 
          // rdoCombo
          // 
          this.rdoCombo.Location = new System.Drawing.Point(208, 56);
          this.rdoCombo.Name = "rdoCombo";
          this.rdoCombo.Size = new System.Drawing.Size(104, 24);
          this.rdoCombo.TabIndex = 10;
          this.rdoCombo.Text = "Combo";
          // 
          // rdoBestFood
          // 
          this.rdoBestFood.Location = new System.Drawing.Point(208, 24);
          this.rdoBestFood.Name = "rdoBestFood";
          this.rdoBestFood.Size = new System.Drawing.Size(104, 24);
          this.rdoBestFood.TabIndex = 9;
          this.rdoBestFood.Text = "Best food";
          // 
          // rdoRisk
          // 
          this.rdoRisk.Location = new System.Drawing.Point(24, 56);
          this.rdoRisk.Name = "rdoRisk";
          this.rdoRisk.Size = new System.Drawing.Size(104, 24);
          this.rdoRisk.TabIndex = 8;
          this.rdoRisk.Text = "Least risk";
          // 
          // rdoClosest
          // 
          this.rdoClosest.Checked = true;
          this.rdoClosest.Location = new System.Drawing.Point(24, 24);
          this.rdoClosest.Name = "rdoClosest";
          this.rdoClosest.Size = new System.Drawing.Size(127, 24);
          this.rdoClosest.TabIndex = 5;
          this.rdoClosest.TabStop = true;
          this.rdoClosest.Text = "Shortest distance";
          // 
          // tabMap
          // 
          this.tabMap.Controls.Add(this.grpTime);
          this.tabMap.Controls.Add(this.btnMapNext);
          this.tabMap.Controls.Add(this.btnMove);
          this.tabMap.Controls.Add(this.btnRelease);
          this.tabMap.Controls.Add(this.btnPredationMap);
          this.tabMap.Controls.Add(this.btnFoodMaps);
          this.tabMap.Controls.Add(this.btnSocialMaps);
          this.tabMap.Controls.Add(this.groupBox6);
          this.tabMap.Location = new System.Drawing.Point(4, 22);
          this.tabMap.Name = "tabMap";
          this.tabMap.Size = new System.Drawing.Size(752, 430);
          this.tabMap.TabIndex = 0;
          this.tabMap.Text = "Simulation Maps";
          this.tabMap.UseVisualStyleBackColor = true;
          // 
          // grpTime
          // 
          this.grpTime.Controls.Add(this.btnSetMove);
          this.grpTime.Controls.Add(this.btnSetRisk);
          this.grpTime.Controls.Add(this.btnSetFood);
          this.grpTime.Controls.Add(this.btnSetRelease);
          this.grpTime.Controls.Add(this.btnSetSocial);
          this.grpTime.Location = new System.Drawing.Point(276, 19);
          this.grpTime.Name = "grpTime";
          this.grpTime.Size = new System.Drawing.Size(200, 389);
          this.grpTime.TabIndex = 13;
          this.grpTime.TabStop = false;
          this.grpTime.Text = "Set Map Start Times";
          this.grpTime.Visible = false;
          // 
          // btnSetMove
          // 
          this.btnSetMove.Enabled = false;
          this.btnSetMove.Location = new System.Drawing.Point(24, 232);
          this.btnSetMove.Name = "btnSetMove";
          this.btnSetMove.Size = new System.Drawing.Size(152, 48);
          this.btnSetMove.TabIndex = 23;
          this.btnSetMove.Text = "Set Movement Maps";
          this.btnSetMove.Click += new System.EventHandler(this.btnSetMove_Click);
          // 
          // btnSetRisk
          // 
          this.btnSetRisk.Enabled = false;
          this.btnSetRisk.Location = new System.Drawing.Point(24, 160);
          this.btnSetRisk.Name = "btnSetRisk";
          this.btnSetRisk.Size = new System.Drawing.Size(152, 48);
          this.btnSetRisk.TabIndex = 21;
          this.btnSetRisk.Text = "Set Risk Maps";
          this.btnSetRisk.Click += new System.EventHandler(this.btnSetRisk_Click);
          // 
          // btnSetFood
          // 
          this.btnSetFood.Enabled = false;
          this.btnSetFood.Location = new System.Drawing.Point(24, 96);
          this.btnSetFood.Name = "btnSetFood";
          this.btnSetFood.Size = new System.Drawing.Size(152, 40);
          this.btnSetFood.TabIndex = 24;
          this.btnSetFood.Text = "Set Food Maps";
          this.btnSetFood.Click += new System.EventHandler(this.btnSetFood_Click);
          // 
          // btnSetRelease
          // 
          this.btnSetRelease.Location = new System.Drawing.Point(24, 304);
          this.btnSetRelease.Name = "btnSetRelease";
          this.btnSetRelease.Size = new System.Drawing.Size(152, 48);
          this.btnSetRelease.TabIndex = 22;
          this.btnSetRelease.Text = "Set Release Maps";
          this.btnSetRelease.Click += new System.EventHandler(this.btnSetRelease_Click);
          // 
          // btnSetSocial
          // 
          this.btnSetSocial.Location = new System.Drawing.Point(24, 32);
          this.btnSetSocial.Name = "btnSetSocial";
          this.btnSetSocial.Size = new System.Drawing.Size(152, 40);
          this.btnSetSocial.TabIndex = 20;
          this.btnSetSocial.Text = "Set Social Maps ";
          this.btnSetSocial.Click += new System.EventHandler(this.btnSetSocial_Click);
          // 
          // btnMapNext
          // 
          this.btnMapNext.Location = new System.Drawing.Point(616, 480);
          this.btnMapNext.Name = "btnMapNext";
          this.btnMapNext.Size = new System.Drawing.Size(144, 48);
          this.btnMapNext.TabIndex = 5;
          this.btnMapNext.Text = "Next";
          // 
          // btnMove
          // 
          this.btnMove.Enabled = false;
          this.btnMove.Location = new System.Drawing.Point(48, 254);
          this.btnMove.Name = "btnMove";
          this.btnMove.Size = new System.Drawing.Size(152, 48);
          this.btnMove.TabIndex = 4;
          this.btnMove.Text = "Load Movement Maps";
          this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
          // 
          // btnRelease
          // 
          this.btnRelease.Enabled = false;
          this.btnRelease.Location = new System.Drawing.Point(48, 328);
          this.btnRelease.Name = "btnRelease";
          this.btnRelease.Size = new System.Drawing.Size(152, 48);
          this.btnRelease.TabIndex = 3;
          this.btnRelease.Text = "Load Release Maps";
          this.btnRelease.Click += new System.EventHandler(this.btnDispersal_Click);
          // 
          // btnPredationMap
          // 
          this.btnPredationMap.Enabled = false;
          this.btnPredationMap.Location = new System.Drawing.Point(48, 180);
          this.btnPredationMap.Name = "btnPredationMap";
          this.btnPredationMap.Size = new System.Drawing.Size(152, 48);
          this.btnPredationMap.TabIndex = 2;
          this.btnPredationMap.Text = "Load Risk Maps";
          this.btnPredationMap.Click += new System.EventHandler(this.btnPredationMap_Click);
          // 
          // btnFoodMaps
          // 
          this.btnFoodMaps.Enabled = false;
          this.btnFoodMaps.Location = new System.Drawing.Point(48, 114);
          this.btnFoodMaps.Name = "btnFoodMaps";
          this.btnFoodMaps.Size = new System.Drawing.Size(152, 40);
          this.btnFoodMaps.TabIndex = 1;
          this.btnFoodMaps.Text = "Load Food Maps";
          this.btnFoodMaps.Click += new System.EventHandler(this.btnFoodMaps_Click);
          // 
          // btnSocialMaps
          // 
          this.btnSocialMaps.Location = new System.Drawing.Point(48, 48);
          this.btnSocialMaps.Name = "btnSocialMaps";
          this.btnSocialMaps.Size = new System.Drawing.Size(152, 40);
          this.btnSocialMaps.TabIndex = 0;
          this.btnSocialMaps.Text = "Load Social Maps";
          this.btnSocialMaps.Click += new System.EventHandler(this.btnSocialMaps_Click);
          // 
          // groupBox6
          // 
          this.groupBox6.Location = new System.Drawing.Point(24, 16);
          this.groupBox6.Name = "groupBox6";
          this.groupBox6.Size = new System.Drawing.Size(200, 392);
          this.groupBox6.TabIndex = 7;
          this.groupBox6.TabStop = false;
          this.groupBox6.Text = "Load Map Folders";
          // 
          // tabModify
          // 
          this.tabModify.Controls.Add(this.btnSafeForageModifer);
          this.tabModify.Controls.Add(this.btnSafeSearchModifier);
          this.tabModify.Controls.Add(this.btnRiskyForageModifer);
          this.tabModify.Controls.Add(this.btnRiskySearchModifier);
          this.tabModify.Controls.Add(this.btnDailyModifier);
          this.tabModify.Controls.Add(this.btnAddHourlyModifer);
          this.tabModify.Controls.Add(this.lblModifiers);
          this.tabModify.Controls.Add(this.btnFemaleGender);
          this.tabModify.Controls.Add(this.btnMoveOK);
          this.tabModify.Controls.Add(this.btnMaleGender);
          this.tabModify.Location = new System.Drawing.Point(4, 22);
          this.tabModify.Name = "tabModify";
          this.tabModify.Size = new System.Drawing.Size(752, 430);
          this.tabModify.TabIndex = 2;
          this.tabModify.Text = "Modifiers";
          this.tabModify.UseVisualStyleBackColor = true;
          // 
          // btnSafeForageModifer
          // 
          this.btnSafeForageModifer.Location = new System.Drawing.Point(32, 368);
          this.btnSafeForageModifer.Name = "btnSafeForageModifer";
          this.btnSafeForageModifer.Size = new System.Drawing.Size(208, 32);
          this.btnSafeForageModifer.TabIndex = 10;
          this.btnSafeForageModifer.Text = "Add Safe Forage Modifier";
          this.btnSafeForageModifer.Click += new System.EventHandler(this.btnSafeForageModifer_Click);
          // 
          // btnSafeSearchModifier
          // 
          this.btnSafeSearchModifier.Location = new System.Drawing.Point(32, 320);
          this.btnSafeSearchModifier.Name = "btnSafeSearchModifier";
          this.btnSafeSearchModifier.Size = new System.Drawing.Size(208, 32);
          this.btnSafeSearchModifier.TabIndex = 9;
          this.btnSafeSearchModifier.Text = "Add Safe Search Modifier";
          this.btnSafeSearchModifier.Click += new System.EventHandler(this.btnSafeSearchModifier_Click);
          // 
          // btnRiskyForageModifer
          // 
          this.btnRiskyForageModifer.Location = new System.Drawing.Point(32, 272);
          this.btnRiskyForageModifer.Name = "btnRiskyForageModifer";
          this.btnRiskyForageModifer.Size = new System.Drawing.Size(208, 32);
          this.btnRiskyForageModifer.TabIndex = 7;
          this.btnRiskyForageModifer.Text = "Add Risky Forage Modifier";
          this.btnRiskyForageModifer.Click += new System.EventHandler(this.btnRiskyForageModifer_Click);
          // 
          // btnRiskySearchModifier
          // 
          this.btnRiskySearchModifier.Location = new System.Drawing.Point(32, 224);
          this.btnRiskySearchModifier.Name = "btnRiskySearchModifier";
          this.btnRiskySearchModifier.Size = new System.Drawing.Size(208, 32);
          this.btnRiskySearchModifier.TabIndex = 8;
          this.btnRiskySearchModifier.Text = "Add Risky Search Modifer";
          this.btnRiskySearchModifier.Click += new System.EventHandler(this.btnRiskySearchModifier_Click);
          // 
          // btnDailyModifier
          // 
          this.btnDailyModifier.Location = new System.Drawing.Point(32, 184);
          this.btnDailyModifier.Name = "btnDailyModifier";
          this.btnDailyModifier.Size = new System.Drawing.Size(208, 32);
          this.btnDailyModifier.TabIndex = 2;
          this.btnDailyModifier.Text = "Add Daily Modifier";
          this.btnDailyModifier.Click += new System.EventHandler(this.btnDailyModifier_Click);
          // 
          // btnAddHourlyModifer
          // 
          this.btnAddHourlyModifer.Location = new System.Drawing.Point(32, 136);
          this.btnAddHourlyModifer.Name = "btnAddHourlyModifer";
          this.btnAddHourlyModifer.Size = new System.Drawing.Size(208, 32);
          this.btnAddHourlyModifer.TabIndex = 1;
          this.btnAddHourlyModifer.Text = "Add Hourly Modifier";
          this.btnAddHourlyModifer.Click += new System.EventHandler(this.btnAddHourlyModifer_Click_1);
          // 
          // lblModifiers
          // 
          this.lblModifiers.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.lblModifiers.Location = new System.Drawing.Point(320, 24);
          this.lblModifiers.Name = "lblModifiers";
          this.lblModifiers.Size = new System.Drawing.Size(400, 328);
          this.lblModifiers.TabIndex = 4;
          // 
          // btnFemaleGender
          // 
          this.btnFemaleGender.Location = new System.Drawing.Point(32, 88);
          this.btnFemaleGender.Name = "btnFemaleGender";
          this.btnFemaleGender.Size = new System.Drawing.Size(208, 32);
          this.btnFemaleGender.TabIndex = 0;
          this.btnFemaleGender.Text = "Add Female Modifier";
          this.btnFemaleGender.Click += new System.EventHandler(this.btnFemaleGender_Click);
          // 
          // btnMoveOK
          // 
          this.btnMoveOK.Location = new System.Drawing.Point(552, 384);
          this.btnMoveOK.Name = "btnMoveOK";
          this.btnMoveOK.Size = new System.Drawing.Size(80, 24);
          this.btnMoveOK.TabIndex = 3;
          this.btnMoveOK.Text = "OK";
          this.btnMoveOK.Click += new System.EventHandler(this.btnMoveOK_Click);
          // 
          // btnMaleGender
          // 
          this.btnMaleGender.Location = new System.Drawing.Point(32, 40);
          this.btnMaleGender.Name = "btnMaleGender";
          this.btnMaleGender.Size = new System.Drawing.Size(208, 32);
          this.btnMaleGender.TabIndex = 0;
          this.btnMaleGender.Text = "Add Male Modifer";
          this.btnMaleGender.Click += new System.EventHandler(this.btnMaleGender_Click);
          // 
          // tabSim
          // 
          this.tabSim.Controls.Add(this.groupBox14);
          this.tabSim.Controls.Add(this.pbarSim);
          this.tabSim.Controls.Add(this.btnTabSimOK);
          this.tabSim.Controls.Add(this.groupBox7);
          this.tabSim.Controls.Add(this.groupBox5);
          this.tabSim.Location = new System.Drawing.Point(4, 22);
          this.tabSim.Name = "tabSim";
          this.tabSim.Size = new System.Drawing.Size(752, 430);
          this.tabSim.TabIndex = 3;
          this.tabSim.Text = "Simulation Parameters";
          this.tabSim.UseVisualStyleBackColor = true;
          this.tabSim.Enter += new System.EventHandler(this.tabSim_Enter);
          // 
          // groupBox14
          // 
          this.groupBox14.Controls.Add(this.btnParameterFile);
          this.groupBox14.Location = new System.Drawing.Point(32, 368);
          this.groupBox14.Name = "groupBox14";
          this.groupBox14.Size = new System.Drawing.Size(424, 48);
          this.groupBox14.TabIndex = 4;
          this.groupBox14.TabStop = false;
          this.groupBox14.Text = "Save All Parameters";
          // 
          // btnParameterFile
          // 
          this.btnParameterFile.Location = new System.Drawing.Point(232, 16);
          this.btnParameterFile.Name = "btnParameterFile";
          this.btnParameterFile.Size = new System.Drawing.Size(152, 23);
          this.btnParameterFile.TabIndex = 0;
          this.btnParameterFile.Text = "Save Parameters to XML file";
          this.btnParameterFile.Click += new System.EventHandler(this.btnParameterFile_Click);
          // 
          // pbarSim
          // 
          this.pbarSim.Location = new System.Drawing.Point(48, 248);
          this.pbarSim.Name = "pbarSim";
          this.pbarSim.Size = new System.Drawing.Size(544, 32);
          this.pbarSim.Step = 1;
          this.pbarSim.TabIndex = 3;
          // 
          // btnTabSimOK
          // 
          this.btnTabSimOK.Enabled = false;
          this.btnTabSimOK.Location = new System.Drawing.Point(535, 347);
          this.btnTabSimOK.Name = "btnTabSimOK";
          this.btnTabSimOK.Size = new System.Drawing.Size(152, 32);
          this.btnTabSimOK.TabIndex = 0;
          this.btnTabSimOK.Text = "Run Simulation";
          this.btnTabSimOK.Click += new System.EventHandler(this.btnTabSimOK_Click);
          // 
          // groupBox7
          // 
          this.groupBox7.Controls.Add(this.btnOutMapDir);
          this.groupBox7.Controls.Add(this.chkTextOutPut);
          this.groupBox7.Location = new System.Drawing.Point(32, 304);
          this.groupBox7.Name = "groupBox7";
          this.groupBox7.Size = new System.Drawing.Size(424, 56);
          this.groupBox7.TabIndex = 2;
          this.groupBox7.TabStop = false;
          this.groupBox7.Text = "Output Parameters";
          // 
          // btnOutMapDir
          // 
          this.btnOutMapDir.Location = new System.Drawing.Point(232, 16);
          this.btnOutMapDir.Name = "btnOutMapDir";
          this.btnOutMapDir.Size = new System.Drawing.Size(152, 32);
          this.btnOutMapDir.TabIndex = 0;
          this.btnOutMapDir.Text = "Set Map Output";
          this.btnOutMapDir.Click += new System.EventHandler(this.btnOutMapDir_Click);
          // 
          // chkTextOutPut
          // 
          this.chkTextOutPut.Location = new System.Drawing.Point(16, 24);
          this.chkTextOutPut.Name = "chkTextOutPut";
          this.chkTextOutPut.Size = new System.Drawing.Size(120, 24);
          this.chkTextOutPut.TabIndex = 0;
          this.chkTextOutPut.Text = "Create Text Files";
          this.chkTextOutPut.CheckedChanged += new System.EventHandler(this.chkTextOutPut_CheckedChanged);
          // 
          // groupBox5
          // 
          this.groupBox5.Controls.Add(this.label18);
          this.groupBox5.Controls.Add(this.txtResOffspringSD);
          this.groupBox5.Controls.Add(this.txtResOffspringMean);
          this.groupBox5.Controls.Add(this.txtResFemalePercent);
          this.groupBox5.Controls.Add(this.txtResBreedPercent);
          this.groupBox5.Controls.Add(this.label25);
          this.groupBox5.Controls.Add(this.label14);
          this.groupBox5.Controls.Add(this.txtResDieBetweenSeason);
          this.groupBox5.Controls.Add(this.txtResDieTimeStep);
          this.groupBox5.Controls.Add(this.label15);
          this.groupBox5.Controls.Add(this.label16);
          this.groupBox5.Controls.Add(this.label17);
          this.groupBox5.Location = new System.Drawing.Point(24, 16);
          this.groupBox5.Name = "groupBox5";
          this.groupBox5.Size = new System.Drawing.Size(544, 208);
          this.groupBox5.TabIndex = 0;
          this.groupBox5.TabStop = false;
          this.groupBox5.Text = "Resident Parameters";
          // 
          // label18
          // 
          this.label18.AutoSize = true;
          this.label18.Location = new System.Drawing.Point(248, 144);
          this.label18.Name = "label18";
          this.label18.Size = new System.Drawing.Size(151, 13);
          this.label18.TabIndex = 9;
          this.label18.Text = "Standard deviation of litter size";
          // 
          // txtResOffspringSD
          // 
          this.txtResOffspringSD.Location = new System.Drawing.Point(248, 160);
          this.txtResOffspringSD.Name = "txtResOffspringSD";
          this.txtResOffspringSD.Size = new System.Drawing.Size(152, 20);
          this.txtResOffspringSD.TabIndex = 8;
          // 
          // txtResOffspringMean
          // 
          this.txtResOffspringMean.Location = new System.Drawing.Point(24, 160);
          this.txtResOffspringMean.Name = "txtResOffspringMean";
          this.txtResOffspringMean.Size = new System.Drawing.Size(136, 20);
          this.txtResOffspringMean.TabIndex = 7;
          this.myToolTip.SetToolTip(this.txtResOffspringMean, "Number of offspring who live old enough to become dispersers.");
          // 
          // txtResFemalePercent
          // 
          this.txtResFemalePercent.Location = new System.Drawing.Point(248, 112);
          this.txtResFemalePercent.Name = "txtResFemalePercent";
          this.txtResFemalePercent.Size = new System.Drawing.Size(152, 20);
          this.txtResFemalePercent.TabIndex = 6;
          // 
          // txtResBreedPercent
          // 
          this.txtResBreedPercent.Location = new System.Drawing.Point(24, 112);
          this.txtResBreedPercent.Name = "txtResBreedPercent";
          this.txtResBreedPercent.Size = new System.Drawing.Size(152, 20);
          this.txtResBreedPercent.TabIndex = 5;
          // 
          // label25
          // 
          this.label25.AutoSize = true;
          this.label25.Location = new System.Drawing.Point(24, 35);
          this.label25.Name = "label25";
          this.label25.Size = new System.Drawing.Size(138, 26);
          this.label25.TabIndex = 4;
          this.label25.Text = "Probability of resident dying \r\nin one time step";
          // 
          // label14
          // 
          this.label14.AutoSize = true;
          this.label14.Location = new System.Drawing.Point(245, 35);
          this.label14.Name = "label14";
          this.label14.Size = new System.Drawing.Size(138, 26);
          this.label14.TabIndex = 3;
          this.label14.Text = "Probability of resident dying \r\nbetween dispersal seasons";
          // 
          // txtResDieBetweenSeason
          // 
          this.txtResDieBetweenSeason.Location = new System.Drawing.Point(248, 64);
          this.txtResDieBetweenSeason.Name = "txtResDieBetweenSeason";
          this.txtResDieBetweenSeason.Size = new System.Drawing.Size(152, 20);
          this.txtResDieBetweenSeason.TabIndex = 2;
          this.myToolTip.SetToolTip(this.txtResDieBetweenSeason, "Since we skip the time steps between dispersal seasons we need to estimate the ch" +
                  "ance of survival on another level");
          // 
          // txtResDieTimeStep
          // 
          this.txtResDieTimeStep.Location = new System.Drawing.Point(24, 64);
          this.txtResDieTimeStep.Name = "txtResDieTimeStep";
          this.txtResDieTimeStep.Size = new System.Drawing.Size(152, 20);
          this.txtResDieTimeStep.TabIndex = 0;
          this.myToolTip.SetToolTip(this.txtResDieTimeStep, "What is the chance any single resident will die during a time step ");
          // 
          // label15
          // 
          this.label15.AutoSize = true;
          this.label15.Location = new System.Drawing.Point(24, 96);
          this.label15.Name = "label15";
          this.label15.Size = new System.Drawing.Size(213, 13);
          this.label15.TabIndex = 4;
          this.label15.Text = "Proportion of females that become pregnant";
          // 
          // label16
          // 
          this.label16.AutoSize = true;
          this.label16.Location = new System.Drawing.Point(248, 96);
          this.label16.Name = "label16";
          this.label16.Size = new System.Drawing.Size(183, 13);
          this.label16.TabIndex = 3;
          this.label16.Text = "Proportion of offspring that are female";
          // 
          // label17
          // 
          this.label17.AutoSize = true;
          this.label17.Location = new System.Drawing.Point(24, 144);
          this.label17.Name = "label17";
          this.label17.Size = new System.Drawing.Size(77, 13);
          this.label17.TabIndex = 3;
          this.label17.Text = "Mean litter size";
          // 
          // label11
          // 
          this.label11.Location = new System.Drawing.Point(0, 0);
          this.label11.Name = "label11";
          this.label11.Size = new System.Drawing.Size(100, 23);
          this.label11.TabIndex = 0;
          // 
          // label9
          // 
          this.label9.Location = new System.Drawing.Point(0, 0);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(100, 23);
          this.label9.TabIndex = 0;
          // 
          // label5
          // 
          this.label5.Location = new System.Drawing.Point(0, 0);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(100, 23);
          this.label5.TabIndex = 0;
          // 
          // groupBox2
          // 
          this.groupBox2.Location = new System.Drawing.Point(0, 0);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(200, 100);
          this.groupBox2.TabIndex = 0;
          this.groupBox2.TabStop = false;
          // 
          // label1
          // 
          this.label1.Location = new System.Drawing.Point(0, 0);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(100, 23);
          this.label1.TabIndex = 0;
          // 
          // label13
          // 
          this.label13.Location = new System.Drawing.Point(0, 0);
          this.label13.Name = "label13";
          this.label13.Size = new System.Drawing.Size(100, 23);
          this.label13.TabIndex = 0;
          // 
          // btnAddHourModifier
          // 
          this.btnAddHourModifier.Location = new System.Drawing.Point(0, 0);
          this.btnAddHourModifier.Name = "btnAddHourModifier";
          this.btnAddHourModifier.Size = new System.Drawing.Size(75, 23);
          this.btnAddHourModifier.TabIndex = 0;
          // 
          // dateTimePicker1
          // 
          this.dateTimePicker1.Location = new System.Drawing.Point(0, 0);
          this.dateTimePicker1.Name = "dateTimePicker1";
          this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
          this.dateTimePicker1.TabIndex = 0;
          // 
          // btnNext
          // 
          this.btnNext.Location = new System.Drawing.Point(0, 0);
          this.btnNext.Name = "btnNext";
          this.btnNext.Size = new System.Drawing.Size(75, 23);
          this.btnNext.TabIndex = 0;
          // 
          // txtRiskySafeThreshold
          // 
          this.txtRiskySafeThreshold.Location = new System.Drawing.Point(0, 0);
          this.txtRiskySafeThreshold.Name = "txtRiskySafeThreshold";
          this.txtRiskySafeThreshold.Size = new System.Drawing.Size(100, 20);
          this.txtRiskySafeThreshold.TabIndex = 0;
          // 
          // label4
          // 
          this.label4.Location = new System.Drawing.Point(50, 24);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(96, 24);
          this.label4.TabIndex = 7;
          this.label4.Text = "Max Vision Distance";
          // 
          // txtVision
          // 
          this.txtVision.Location = new System.Drawing.Point(50, 56);
          this.txtVision.Name = "txtVision";
          this.txtVision.Size = new System.Drawing.Size(100, 20);
          this.txtVision.TabIndex = 0;
          this.myToolTip.SetToolTip(this.txtVision, "The farthest the animal can see in meters");
          // 
          // lblHourMods
          // 
          this.lblHourMods.Location = new System.Drawing.Point(0, 0);
          this.lblHourMods.Name = "lblHourMods";
          this.lblHourMods.Size = new System.Drawing.Size(100, 23);
          this.lblHourMods.TabIndex = 0;
          // 
          // frmInput
          // 
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.ClientSize = new System.Drawing.Size(744, 477);
          this.Controls.Add(this.tabControl);
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.Name = "frmInput";
          this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
          this.Text = "S.E.A.R.C.H.";
          this.tabControl.ResumeLayout(false);
          this.tabTime.ResumeLayout(false);
          this.groupBox13.ResumeLayout(false);
          this.groupBox13.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.nudWakeTime)).EndInit();
          this.tabSpecies.ResumeLayout(false);
          this.groupBox10.ResumeLayout(false);
          this.groupBox10.PerformLayout();
          this.groupBox4.ResumeLayout(false);
          this.groupBox4.PerformLayout();
          this.groupBox3.ResumeLayout(false);
          this.groupBox3.PerformLayout();
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.tabHomeRange.ResumeLayout(false);
          this.groupBox12.ResumeLayout(false);
          this.groupBox12.PerformLayout();
          this.groupBox8.ResumeLayout(false);
          this.groupBox8.PerformLayout();
          this.groupBox11.ResumeLayout(false);
          this.tabMap.ResumeLayout(false);
          this.grpTime.ResumeLayout(false);
          this.tabModify.ResumeLayout(false);
          this.tabSim.ResumeLayout(false);
          this.groupBox14.ResumeLayout(false);
          this.groupBox7.ResumeLayout(false);
          this.groupBox5.ResumeLayout(false);
          this.groupBox5.PerformLayout();
          this.ResumeLayout(false);

      }
      
      #endregion
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		#endregion

		#region tabcocde
		private void HideTabPage(TabPage tp)
		{
			if (this.tabControl.TabPages.Contains(tp))
				tabControl.TabPages.Remove(tp);
		}


		private void ShowTabPage(TabPage tp)
		{
			ShowTabPage(tp, tabControl.TabPages.Count);
		}
		private void ShowTabPage(TabPage tp, int index)
		{
			if (tabControl.TabPages.Contains(tp)) return;
			InsertTabPage(tp, index);
		}


		private void InsertTabPage(TabPage tabpage, int index)
		{
			if (index < 0 || index > tabControl.TabCount)
				throw new ArgumentException("Index out of Range.");
			tabControl.TabPages.Add(tabpage);
			if (index < tabControl.TabCount - 1)
				do 
				{
					SwapTabPages(tabpage, (tabControl.TabPages[tabControl.TabPages.IndexOf(tabpage) - 1]));
				}
				while (tabControl.TabPages.IndexOf(tabpage) != index);
			tabControl.SelectedTab = tabpage;
		}
		private void SwapTabPages(TabPage tp1, TabPage tp2)
		{
			if (tabControl.TabPages.Contains(tp1) == false || tabControl.TabPages.Contains(tp2) == false)
				throw new ArgumentException("TabPages must be in the TabControls TabPageCollection.");
           
         int Index1 = tabControl.TabPages.IndexOf(tp1);
         int Index2 = tabControl.TabPages.IndexOf(tp2);
         tabControl.TabPages[Index1] = tp2;
         tabControl.TabPages[Index2] = tp1;

         //Uncomment the following section to overcome bugs in the Compact Framework
         //tabControl.SelectedIndex = tabControl.SelectedIndex; 
         //string tp1Text, tp2Text;
         //tp1Text = tp1.Text;
         //tp2Text = tp2.Text;
         //tp1.Text=tp2Text;
         //tp2.Text=tp1Text;

      }
   
      #endregion

       #region textBoxCode
      private void buildTextBoxes(Control inControl, ref TextBoxArray myTextBoxes)
      {  
         try
         {
            foreach (Control c in inControl.Controls)
            {
               if (c.Controls.Count > 0)
               {
                  buildTextBoxes(c, ref myTextBoxes);
               }
               else
               {
                  if (c.GetType().ToString() == "System.Windows.Forms.TextBox" && c.Name.IndexOf("End",0)<0)
                  {  
                     myTextBoxes.add((System.Windows.Forms.TextBox) c);
                  }
               }
            }

         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if DEVELOP
            System.Windows.Forms.MessageBox.Show("An error occured please find C:\\DispersalError.log " 
               + System.Environment.NewLine + 
               "and email to the developer  at rancho@mwwb.net");
#endif
         }
			
      }//end make text boxes
      
      private bool areTextBoxesFilled(TextBoxArray inTextBoxes)
      {
         string   strText;
         bool     success = true;

         for (int j = 0; j < inTextBoxes.Count; j++)
         {
            strText = inTextBoxes.getTextBoxText(j);
            if (strText.Length == 0)
            {
               success = false;
               MessageBox.Show("You left out an entry you must enter something even a zero","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
               inTextBoxes.setFocus(j);
               break;
            }
         }
         return success;
      }
      private bool areEnteriesNumeric(TextBoxArray inTextBoxes)
      {
         string strText;
         bool success = true;
         
         for (int j = 0; j < inTextBoxes.Count && success; j++)
         {
            strText = inTextBoxes.getTextBoxText(j);
            if (!isEntryNumeric(strText))
            {
               MessageBox.Show(strText + " is not a valid entry");
               inTextBoxes.setFocus(j);
               success = false;
            }
         } //end looping through all the text boxes
         return success;
      }// end areEnteriesNumeric

      private bool isEntryNumeric(string strText)
      {
         bool success = true;
         for (int i = 0; i < strText.Length; i++)
         {
            if ((strText[i] > 57 || strText[i] < 48) && strText[i] != 46)
            {
               success = false;
               break;
            }//end checking if numeric or '.'
         }//end looping through the string
         return success;
         
      }

      #endregion

      #region tab time code
      private void btnTime_Click(object sender, System.EventArgs e)
      {
         mLog.Debug("inside the btnSpecies_Click event calling to see if the entries are filled in and numeric");
         if (this.areTextBoxesFilled(this.timeText))
         {
            if (this.areEnteriesNumeric(this.timeText))
            {
        
               bool success = true;
               this.mySimManager.StartSeasonDate = this.dtpSeasonStartDate.Value.Date;
               this.mySimManager.EndSeasonDate = System.Convert.ToDateTime(this.txtEndDaySeason.Text);
               this.mySimManager.StartSimulationDate = this.SimStartDate.Value.Date;
               this.mySimManager.EndSimulatonDate = System.Convert.ToDateTime(this.txtEndDaySim.Text);
               this.mySimManager.NumDaysSeason = System.Convert.ToInt32(this.txtNumSeasonDays.Text);
               this.mySimManager.ElapsedTimeBetweenTimeStep = System.Convert.ToDouble(this.txtTimeBetweenDailyTimeStep.Text);
               this.mySimManager.StartSimulationDate = this.mySimManager.StartSimulationDate.AddHours(System.Convert.ToDouble(this.nudWakeTime.Value));
               if (60 % this.mySimManager.ElapsedTimeBetweenTimeStep != 0 && this.mySimManager.ElapsedTimeBetweenTimeStep % 60 != 0)
               {
                  MessageBox.Show("The time between timesteps a day has to be a mulitple of 60", 
                     "Input Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                  this.txtTimeBetweenDailyTimeStep.SelectAll();
                  this.txtTimeBetweenDailyTimeStep.Focus();
                  success = false;
               }
               this.mySimManager.NumSeasons = System.Convert.ToInt32(this.txtNumYears.Text);
               if (success)
               {
                //  MessageBox.Show("You have successfully set the time parameters"
                 //    + System.Environment.NewLine + 
                 //    "You may now set the various maps");
                  this.ShowTabPage(this.tabMap);
                  this.tabMap.Focus();
               }
            }
         }
      }

      private void txtNumSeasonDays_LostFocus(object sender, EventArgs e)
      {
         
         bool success;
         
         try
         {
            success = calcSeasonEndDate();
            if (success)
            {
               calcSimulationEndDate();
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         
      }
      
      
      /********************************************************************************
       *   Function name   : calcEndDate
       *   Description     : makes sure there are valid entries and then calculates the 
       *                     and displays the end date for a dispersal season 
       *   Return type     : void 
       * ********************************************************************************/
      private bool calcSeasonEndDate()
      {
         bool success  = false;
         try
         {
            if (this.txtNumSeasonDays.Text.Length > 0)
            {
               if (this.isEntryNumeric(this.txtNumSeasonDays.Text))
               {
                  double numDays = System.Convert.ToDouble(this.txtNumSeasonDays.Text);
                  if (numDays >= 1 && numDays <= 365)
                  {
                     this.txtEndDaySeason.Text = this.SimStartDate.Value.Date.AddDays(numDays).ToShortDateString();
                     success = true;
                  }
                  else
                  {
                     MessageBox.Show(numDays.ToString() + " is not between 1 and 365 days.  Not a valid season lenght", "Error");
                     this.txtNumSeasonDays.Focus();
                     this.txtNumSeasonDays.SelectAll();
                  }
                 
               }
               else
               {
                  MessageBox.Show("That is not a numeric entry");
                  this.txtNumSeasonDays.SelectAll();
               }
            }
            else
            {
               this.txtEndDaySeason.Text = "";
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return success;
         
      }

     
      private void calcSimulationEndDate()
      {
         if (this.txtNumYears.Text.Length > 0)
         {
            if(this.isEntryNumeric(this.txtNumYears.Text))
            {
               this.txtEndDaySim.Text = this.SimStartDate.Value.Date.AddYears(System.Convert.ToInt32(this.txtNumYears.Text)).ToShortDateString();
            }
         }
      }
              
      
     
      #endregion

      #region tabmapcode
      private void btnSocialMaps_Click(object sender, System.EventArgs e)
      {
         validateMaps("Social",ref socialDir,"social","energy/food",btnFoodMaps);
         this.mySimManager.MapManager.mySocialMaps.MyPath = socialDir;
      }// end btnSocialMaps_Click
   
      private void btnFoodMaps_Click(object sender, System.EventArgs e)
      {
         validateMaps("Food",ref foodDir,"food/energy","risk",btnPredationMap);
         this.mySimManager.MapManager.myFoodMaps.MyPath = foodDir;
      }// end btnFoodMaps_Click
      private void btnPredationMap_Click(object sender, System.EventArgs e)
      {
         validateMaps("Predation",ref riskDir,"risk","movement",btnMove);
         this.mySimManager.MapManager.myPredationMaps.MyPath = riskDir;
      }// end btnPredationMap_Click
      private void btnMove_Click(object sender, System.EventArgs e)
      {

         validateMaps("Move",ref moveDir,"movement","release",btnRelease);
         this.mySimManager.MapManager.myMoveMaps.MyPath = moveDir;
      }// end btnMove_Click
      private void btnDispersal_Click(object sender, System.EventArgs e)
      {
#if (DEVELOPE_HOME)
            
         if (this.myMapManager.validateMap("Dispersal",@"E:\Pat'sStuff\Dispersal\input files\small_example\Release"))
         {
            this.mySimManager.Maps.InitializeMaps();
            this.mySimManager.buildAnimals();
            MessageBox.Show("done building animals");
            this.ShowTabPage(this.tabSpecies);
            this.tabSpecies.Focus();
         }
         
#elif (DEVELOPE_LAPTOP)         
         if  (this.myMapManager.validateMap("Dispersal",@"D:\Dispersal\input files\small_example\Release"))
         {
            this.mySimManager.Maps.InitializeMaps();
            this.mySimManager.buildAnimals();
            MessageBox.Show("done building animals");
            this.ShowTabPage(this.tabSpecies);
            this.tabSpecies.Focus();
         }
         
#else 

         try
         {
            mLog.Debug("inside btnDispersal_Click");
            //If disDir is empty then it being called from the form.  If disDir has a value
            //then it is being loaded by file and no need to show the dialog box.
            if(disDir == "")
            {
               this.fdbCommon.Description = "Browse for release maps";
               if (this.fdbCommon.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                  disDir = this.fdbCommon.SelectedPath;
                  mLog.Debug("the dispersal Map dir is " + disDir);
               }
            }
            if (!this.myMapManager.validateMap("Dispersal",disDir))
            {
               mLog.Debug("error validating release maps");
               MessageBox.Show("Error for predation maps" + System.Environment.NewLine + this.myMapManager.getErrMessage());
            } // end if validated
            else
            {
               //MessageBox.Show("All the maps have been validated.  Now set the start times");
               this.mySimManager.MapManager.myDispersalMaps.MyPath = disDir;
               this.grpTime.Visible = true;
               this.btnSetSocial.Enabled = true;
            } 
					
				
            mLog.Debug("leaving btnDispersal_Click");
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
#endif 
      }// end btnDispersal_Click
      private void validateMaps(string mapType,ref string path, string description, string nextType, System.Windows.Forms.Button nextButton)
      {
         try
         {
            mLog.Debug("inside frmInput.validateMaps called from button " + mapType + " map click event");
            //if the path is empty then we are loading manually.  If the path has a value we are loading from a file.
            if (path == "")
            {
               this.fdbCommon.Description = "Browse for " + description + " maps";
               if (this.fdbCommon.ShowDialog() == System.Windows.Forms.DialogResult.OK)
               {
                  path = this.fdbCommon.SelectedPath;
                  mLog.Debug("the move dir is " + path);
                  mLog.Debug("calling process map dir");
                  processMapDir(mapType,path);
                  MessageBox.Show("The " + mapType + " maps loaded successfully" + System.Environment.NewLine + 
                     "You may now load the " + nextType + " maps");
                  nextButton.Enabled = true;
                  nextButton.PerformClick();
               }//end if show dialog is good
            }
            else
            {
               processMapDir(mapType,path);
            }
            
            mLog.Debug("leaving " + mapType + " map click event");
         }
         catch (InvalidMapException e)
         {
            MessageBox.Show(e.Message);
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }
      private void processMapDir(string mapType, string path)
      {
         mLog.Debug("in frmInput process map dir, calling validate map");
         if (!this.myMapManager.validateMap(mapType,path))
         {
            mLog.Debug("error validating " + mapType + " map at " + path);
            throw new InvalidMapException ("Error for " + mapType + " maps" + System.Environment.NewLine + this.myMapManager.getErrMessage());
         }// end if validated
         else
         {
            mLog.Debug("success validating " + mapType + " map at " + path);
         }
      }

      private void btnSetSocial_Click(object sender, System.EventArgs e)
      {
         YearlyForm yf = new YearlyForm("Social", ref mySimManager, "Social Landscape");
         yf.Show();
         this.btnSetFood.Enabled = true;
      
      }

      private void btnSetFood_Click(object sender, System.EventArgs e)
      {
         YearlyForm yf = new YearlyForm("Food", ref mySimManager, "Food Source");
         yf.Show();  
         this.btnSetRisk.Enabled = true;
      }

      private void btnSetRisk_Click(object sender, System.EventArgs e)
      {
         YearlyForm yf = new YearlyForm("Predation", ref mySimManager, "Predation Risk");
         yf.Show();
         this.btnSetMove.Enabled = true;
      }

      private void btnSetMove_Click(object sender, System.EventArgs e)
      {
         YearlyForm yf = new YearlyForm("Move", ref mySimManager, "Movement Modifier");
         yf.ShowDialog();
         this.btnSetRelease.Enabled = true;
      
      }

      private void btnSetRelease_Click(object sender, System.EventArgs e)
      {
         YearlyForm yf = new YearlyForm("Dispersal", ref mySimManager, "Release Area");
         yf.ShowDialog();
         //add one day because right now the current start date is at midnite.
         this.myMapManager.changeMaps(this.mySimManager.StartSimulationDate);
         this.mySimManager.buildAnimals();
         this.mySimManager.buildResidents();
         //MessageBox.Show("Map times have been successfully set");
         this.ShowTabPage(this.tabSpecies);
         this.tabSpecies.Focus();
         mLog.Debug("leaving btnDispersal_Click");
      
      }
      #endregion

      #region tab species code
      private void btnSpecies_Click(object sender, System.EventArgs e)
      {
         mLog.Debug("inside the btnSpecies_Click event calling to see if the entries are filled in and numeric");
         if (this.areTextBoxesFilled(this.speciesText))
         {
            if (this.areEnteriesNumeric(this.speciesText))
            {
               try
               {
                  mLog.Debug("they were now setting the values for the animal attributes");
                  this.mySimManager.AnimalManager.AnimalAttributes.ForageSearchTrigger = System.Convert.ToDouble(this.txtSearchForageTrigger.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.InitialEnergy = System.Convert.ToDouble(this.txtInitialEnergy.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.MaxEnergy = System.Convert.ToDouble(this.txtMaxEnergy.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.MinEnergy_Survive = System.Convert.ToDouble(this.txtMinEnergy.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime = System.Convert.ToDouble(this.nudWakeTime.Value);
                  this.mySimManager.AnimalManager.AnimalAttributes.SafeRiskyTrigger = System.Convert.ToDouble(this.txtSafeToRisky.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.RiskySafeTrigger = System.Convert.ToDouble(this.txtRiskyToSafe.Text);
                  this.mySimManager.AnimalManager.AnimalAttributes.PerceptionDistance = System.Convert.ToDouble(this.txtPerception.Text);         
                 

                  if (this.mySimManager.AnimalManager.setAttributes())
                  {
                     //MessageBox.Show("You have successfully loaded the species parameters." + 
                     //   System.Environment.NewLine + 
                     //   "Now load the home range stuff");
                     this.ShowTabPage(this.tabHomeRange);
                     this.tabHomeRange.Focus();
                  }
                  else
                  {
                     MessageBox.Show(this.mySimManager.AnimalManager.ErrMessage, "Error setting attributes");
                  }
               }
               catch (System.Exception ex)
               {
                  MessageBox.Show(ex.StackTrace);
               }
            }
         }
         mLog.Debug("leaving the btnSpecies_Click event");
      }
      private void btnInitialWakeUp_Click(object sender, System.EventArgs e)
      {
         this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime = System.Convert.ToDouble(this.nudWakeTime.Value);
         this.lblDuration.Text = "Start Time of yearly simulation is: " + this.nudWakeTime.Value.ToString();
      }

      private void btnAddActiveDuration_Click(object sender, System.EventArgs e)
      {
          //Console.WriteLine("inside actual click of add active");
         double mean = 0;
         double sd = 0;
         double numLeft=0;
         try
         {
            if (validateDuration())
            {
               mean=System.Convert.ToDouble(this.txtDurMean.Text);
               sd = System.Convert.ToDouble(this.txtDurSD.Text);
               Duration d = new Duration("Active",mean,sd);
               numLeft=this.mySimManager.AnimalManager.AnimalAttributes.addDuration(d);
               this.btnAddActiveDuration.Enabled = false;
               this.btnAddRestDuration.Enabled = true;
               this.lblDuration.Text += System.Environment.NewLine + this.txtDurMean.Text + " hour active period";
               finishAddingDuration(numLeft);
            }
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

      private void btnAddRestDuration_Click(object sender, System.EventArgs e)
      {
         double mean = 0;
         double sd = 0;
         double numLeft=0;
         try
         {
            if (validateDuration())
            {
               mean=System.Convert.ToDouble(this.txtDurMean.Text);
               sd = System.Convert.ToDouble(this.txtDurSD.Text);
               Duration d = new Duration("Sleeping",mean,sd);
               numLeft=this.mySimManager.AnimalManager.AnimalAttributes.addDuration(d);
               this.btnAddActiveDuration.Enabled = true;
               this.btnAddRestDuration.Enabled = false;
               this.lblDuration.Text += System.Environment.NewLine + this.txtDurMean.Text + " hour resting period";
               finishAddingDuration(numLeft);
            }
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
      
      }
      private void finishAddingDuration(double inNumHours)
      {
         if (inNumHours > 0)
         {
            //MessageBox.Show("You have " + inNumHours.ToString() + " hours in the day yet");
            this.txtDurMean.Text="";
            this.txtDurSD.Text="";
         }
         else
         {
            //MessageBox.Show("OK all the time in the day is accounted for");
            this.btnAddRestDuration.Enabled = false;
            this.btnAddActiveDuration.Enabled = false;
            this.btnSpecies.Enabled = true;
         }
         
         
      }
      private bool validateDuration()
      {
         bool success = false;
         if(this.txtDurMean.Text.Length > 0 && this.txtDurSD.Text.Length > 0)
         {
            if(this.txtDurMean.Text != "24")
            {
               if(isEntryNumeric(this.txtDurMean.Text)&&isEntryNumeric(this.txtDurSD.Text))
               {
                  success = true;
               }
               else
               {
                  MessageBox.Show("One of your entries is not numeric","Error");
               }
            }
            else
            {
               MessageBox.Show("You can not have a 24 hour activity period.","Error");
               this.txtDurMean.SelectAll();
               this.txtDurMean.Focus();
            }
         }
         else
         {
            MessageBox.Show("You need to fill in both the mean and sd text boxes","Error");
         }
         return success;

      }
      #endregion

      #region tab move code


      private void btnMaleGender_Click(object sender, System.EventArgs e)
      {  
         
#if DEBUG

         this.lblModifiers.Text = this.lblModifiers.Text + "Male Modifers Created" + System.Environment.NewLine;
         this.lblModifiers.Text = this.lblModifiers.Text + "Female Modifers Created" + System.Environment.NewLine;
         this.lblModifiers.Text = this.lblModifiers.Text + "Risky Search Modifer Created" + System.Environment.NewLine;
         this.lblModifiers.Text = this.lblModifiers.Text + "Risky Forage Modifer Created" + 
            System.Environment.NewLine;
         this.lblModifiers.Text = this.lblModifiers.Text + "Safe Search Modifer Created" + 
            System.Environment.NewLine;
         this.lblModifiers.Text = this.lblModifiers.Text + "Safe Forage Modifer Created" + 
            System.Environment.NewLine;

#else
         try
         {
            frmModifierInput fm = new frmModifierInput();
            mLog.Debug("inside btnMaleGender_Click on form input getting ready to show formModifierInput");
            fm.TempMod = this.mySimManager.AnimalManager.MaleModifier; ;
            fm.setGender("Male");
            fm.ShowDialog();
            mLog.Debug("done showing the form now check if it was success");
            if (fm.Value)
            {
               mLog.Debug("it was a success");
               fm.Close();
               this.lblModifiers.Text = this.lblModifiers.Text + "Male Modifers Created" + System.Environment.NewLine;
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
#endif
      }

      private void btnFemaleGender_Click(object sender, System.EventArgs e)
      {
#if DEBUG

         this.lblModifiers.Text = this.lblModifiers.Text + "Female Modifers Created" + System.Environment.NewLine;
#else

         try
         {
            frmModifierInput fm = new frmModifierInput();
            mLog.Debug("inside btnFemaleGender_Click on form input getting ready to show formModifierInput");
            fm.TempMod = this.mySimManager.AnimalManager.FemaleModifier; ;
            fm.setGender("Female");
            fm.ShowDialog();
            mLog.Debug("done showing the form now check if it was success");
            if (fm.Value)
            {
               mLog.Debug("it was a success");
               fm.Close();
               this.lblModifiers.Text = this.lblModifiers.Text + "Female Modifers Created" + System.Environment.NewLine;
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         
#endif
      
      }

      private void btnAddHourlyModifer_Click_1(object sender, System.EventArgs e)
      {
         //#if DEBUG
         //         System.IO.StreamReader sr = new System.IO.StreamReader("hmod.txt");
         //         string s;
         //         string[] sa;
         //         
         //         HourlyModifier hm = new HourlyModifier();
         //
         //         while (sr.Peek() >= 0)
         //         {
         //            s = sr.ReadLine();
         //            sa = s.Split(',');
         //            hm.CaptureFood = System.Convert.ToDouble(sa[0]);
         //            hm.EnergyUsed = System.Convert.ToDouble(sa[1]);
         //            hm.MoveSpeed = System.Convert.ToDouble(sa[2]);
         //            hm.MoveTurtosity = System.Convert.ToDouble(sa[3]);
         //            hm.PerceptonModifier = System.Convert.ToDouble(sa[4]);
         //            hm.PredationRisk = System.Convert.ToDouble(sa[5]);
         //            hm.StartTime = System.Convert.ToInt32(sa[6]);
         //            this.mySimManager.addHourlyModifier(hm);
         //            this.lblModifiers.Text = this.lblModifiers.Text + "Hourly Modifer Created" + 
         //               " Start Time: " + hm.StartTime.ToString() + ":00" + System.Environment.NewLine; ;
         //
         //
         //         }
         //         
         //#else
         Modifier m = new Modifier();
         HourlyModifier hm = new HourlyModifier();
         frmModifierInput fm = new frmModifierInput();
         mLog.Debug("inside btnAddHourModifier_Click on form input getting ready to show formModifierInput");
         fm.TempMod = m;
         fm.setHour();
         fm.ShowDialog();
         mLog.Debug("done showing the form now check if it was success");
         if (fm.Value)
         {
            mLog.Debug("it was a success");
            hm.CaptureFood = m.CaptureFood;
            hm.EnergyUsed = m.EnergyUsed;
            hm.MoveSpeed = m.MoveSpeed;
            hm.MoveTurtosity = m.MoveTurtosity;
            hm.PerceptonModifier = m.PerceptonModifier;
            hm.PredationRisk = m.PredationRisk;
            hm.StartTime = fm.Hour;
            mLog.Debug("now add the hourly modifier to the sim manager");
            this.mySimManager.addHourlyModifier(hm);
            this.lblModifiers.Text = this.lblModifiers.Text + "Hourly Modifer Created" + 
               " Start Time: " + fm.Hour.ToString() + ":00" + System.Environment.NewLine; ;
            fm.Close();
      
         }
         //#endif
      
      }

      private void btnDailyModifier_Click(object sender, System.EventArgs e)
      {
         //#if DEBUG
         //
         //         System.IO.StreamReader sr = new System.IO.StreamReader("dmod.txt");
         //         string s;
         //         string[] sa;
         //         double d = 0;
         //         
         //         DailyModifier dm = new DailyModifier();
         //
         //         while (sr.Peek() >= 0)
         //         {
         //            s = sr.ReadLine();
         //            sa = s.Split(',');
         //            dm.CaptureFood = System.Convert.ToDouble(sa[0]);
         //            dm.EnergyUsed = System.Convert.ToDouble(sa[1]);
         //            dm.MoveSpeed = System.Convert.ToDouble(sa[2]);
         //            dm.MoveTurtosity = System.Convert.ToDouble(sa[3]);
         //            dm.PerceptonModifier = System.Convert.ToDouble(sa[4]);
         //            dm.PredationRisk = System.Convert.ToDouble(sa[5]);
         //            dm.StartDate = System.DateTime.Today.AddDays(d);
         //            this.mySimManager.addDailyModifier(dm);
         //            ++d;
         //            this.lblModifiers.Text = this.lblModifiers.Text + "Daily Modifer Created" + 
         //               " Start date: " + dm.StartDate.ToShortDateString() + System.Environment.NewLine; ;
         //
         //
         //         }
         //         
         //#else
         
         DailyModifier dm = new DailyModifier();
         dm.StartDate = this.mySimManager.StartSimulationDate; 
         frmModifierInput fm = new frmModifierInput();
         mLog.Debug("inside btnDailyModifier_Click on form input getting ready to show formModifierInput");
         fm.TempMod = dm;
         fm.setText("Daily Modifier");
         fm.ShowDialog();
         mLog.Debug("done showing the form now check if it was success");
         if (fm.Value)
         {
            mLog.Debug("it was a success");
            mLog.Debug("now add the daily modifier to the sim manager");
            this.mySimManager.addDailyModifier(dm);
            this.lblModifiers.Text = this.lblModifiers.Text + "Daily Modifer Created" + 
               " Start Date is : " + dm.StartDate.ToString() + ":00" + System.Environment.NewLine; ;
            fm.Close();
         }
         //#endif
      
      
      }

      private void loadModifier (Modifier modIn, string textIn)
      {
         //create form to capture user input
         frmModifierInput fm = new frmModifierInput();
         fm.TempMod = modIn;
         fm.setText(textIn);
         fm.ShowDialog();
         mLog.Debug("done showing the form now check if it was success");
         if (fm.Value)
         {
            mLog.Debug("it was a success");
            this.lblModifiers.Text = this.lblModifiers.Text + textIn +" Modifer Created" + 
               System.Environment.NewLine;
            fm.Close();
         }


      }//end of loadModifier
      private void btnRiskySearchModifier_Click(object sender, System.EventArgs e)
      {
#if DEBUG
         this.lblModifiers.Text = this.lblModifiers.Text + "Risky Search Modifer Created" + System.Environment.NewLine;

#else
			mLog.Debug("inside btnRiskySearchModifier_Click on form input getting ready to show formModifierInput");
			loadModifier(this.mySimManager.AnimalManager.RiskySearchMod,"Risky Search");
#endif
      }

      private void btnRiskyForageModifer_Click(object sender, System.EventArgs e)
      {
#if DEBUG
         this.lblModifiers.Text = this.lblModifiers.Text + "Risky Forage Modifer Created" + 
            System.Environment.NewLine;

#else

         mLog.Debug("inside btnRiskyForageModifer_Click on form input getting ready to show formModifierInput");
         loadModifier(this.mySimManager.AnimalManager.RiskyForageMod,"Risky Forage");
#endif 
      }

      private void btnSafeSearchModifier_Click(object sender, System.EventArgs e)
      {
#if DEBUG
         this.lblModifiers.Text = this.lblModifiers.Text + "Safe Search Modifer Created" + 
            System.Environment.NewLine;

#else

         mLog.Debug("inside btnSafeSearchModifier_Click on form input getting ready to show formModifierInput");
         loadModifier(this.mySimManager.AnimalManager.SafeSearchMod,"Safe Search");
#endif
      
      }

      private void btnSafeForageModifer_Click(object sender, System.EventArgs e)
      {
#if DEBUG
         this.lblModifiers.Text = this.lblModifiers.Text + "Safe Forage Modifer Created" + 
            System.Environment.NewLine;
#else


  
         frmModifierInput fm = new frmModifierInput();
         mLog.Debug("inside btnSafeForageModifer_Click on form input getting ready to show formModifierInput");
         fm.TempMod = this.mySimManager.AnimalManager.SafeForageMod;
         fm.setText("Safe Forage");
         fm.ShowDialog();
         mLog.Debug("done showing the form now check if it was success");
         if (fm.Value)
         {
            mLog.Debug("it was a success");
            this.lblModifiers.Text = this.lblModifiers.Text + "Safe Forage Modifer Created" + 
               System.Environment.NewLine;
            fm.Close();
         }
#endif
      }

      private void btnMoveOK_Click(object sender, System.EventArgs e)
      {
         if(validateModifers())
         {
            this.mySimManager.AnimalManager.setModifiers();
            //MessageBox.Show("Modifiers successfuly set.  Now moving to Simulation parameters.");
            this.ShowTabPage(this.tabSim);
            this.tabSim.Focus();
         }
      }
      private bool validateModifers()
      {
         bool success = true;
         string [] myModifers = new string[8];
         myModifers[0] = "Male";
         myModifers[1] = "Female";
         myModifers[2] = "Hourly";
         myModifers[3] = "Daily";
         myModifers[4] = "Risky Search";
         myModifers[5] = "Risky Forage";
         myModifers[6] = "Safe Search";
         myModifers[7] = "Safe Forage";

         for (int i=0;i<myModifers.GetLength(0);i++)
         {
            if (this.lblModifiers.Text.IndexOf(myModifers[i])<0)
            {
               success = false;
               MessageBox.Show("You need to set the " + myModifers[i] + " modifiers before you can advance","Error",
                  MessageBoxButtons.OK,MessageBoxIcon.Information);
               break;
            }
         }
         return success;
      }
      #endregion

       #region tab home range code
      private void rdoNumSteps_CheckedChanged(object sender, System.EventArgs e)
      {
         if (this.rdoNumSteps.Checked)
            this.lblTriggerNum.Text = "Number of steps";
         else
            this.lblTriggerNum.Text = "Number of sites";
      
      }
      private void btnHomeRange_Click(object sender, System.EventArgs e)
      {
         bool success = false;

         try
         {
            if (this.areTextBoxesFilled(this.homeRangeText))
            {
               if (this.areEnteriesNumeric(this.homeRangeText))
               {
                     this.mySimManager.AnimalManager.setHomeRange("MALE",
                     System.Convert.ToDouble(this.txtMaleHomeRangeArea.Text),1,1,System.Convert.ToDouble(this.txtMaleDistMod.Text));

                  this.mySimManager.AnimalManager.setHomeRange("FEMALE",
                     System.Convert.ToDouble(this.txtFemaleHomeRangeArea.Text),1,1,System.Convert.ToDouble(this.txtFemaleDistMod.Text));

                  if (this.rdoNumSteps.Checked)
                     this.mySimManager.AnimalManager.setHomeRangeTrigger("STEPS",System.Convert.ToInt32(this.txtTriggerNum.Text));
                  else
                     this.mySimManager.AnimalManager.setHomeRangeTrigger("SITES",System.Convert.ToInt32(this.txtTriggerNum.Text));

                  if (this.rdoBestFood.Checked)
                  {  success = this.mySimManager.AnimalManager.setHomeRangeCriteria("Food");}
                  else if(this.rdoClosest.Checked)
                  {  success=this.mySimManager.AnimalManager.setHomeRangeCriteria("Closest");}
                  else if(this.rdoCombo.Checked)
                  {  success=this.mySimManager.AnimalManager.setHomeRangeCriteria("Combo");}
                  else
                  {  success=this.mySimManager.AnimalManager.setHomeRangeCriteria("Risk");}
               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         
         if (success)
         {
            //MessageBox.Show("You have successfully set the home range parameters"
            //   + System.Environment.NewLine + 
            //   "You may now set the various movement modifiers");
            this.ShowTabPage(this.tabModify);
            this.tabModify.Focus();
         }
         else
         {
            MessageBox.Show("Error occured setting home range parmeters. Find logs and send");
         }
      }
      private void txtMaleHomeRangeArea_Validating(object sender, CancelEventArgs e)
      { 
         //checkMinHomeRange(ref sender,ref e);
      }
      private void txtFemaleHomeRangeArea_Validating(object sender, CancelEventArgs e)
      {
          //checkMinHomeRange(ref sender,ref e);
      }
      private void checkMinHomeRange(ref object sender, ref CancelEventArgs e)
      {
         try
         {
            System.Windows.Forms.TextBox t = sender as TextBox;
            double temp = System.Convert.ToDouble(t.Text);
            if (temp < .1)
            {
               MessageBox.Show("Min Home Range must be at least .1");
               t.Text = "";
               t.Focus();
               e.Cancel = true;
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            MessageBox.Show(ex.Message);
            eLog.Debug(ex);
#endif
         }
      }
      #endregion

      #region tab simulation code
      /********************************************************************************
       *   Function name   : chkTextOutPut_CheckedChanged
       *   Description     : if it is checked then we have to make text out put files
       *                     so this will pop up the save file dialog to get the file name
       *                     and set the flag in the sim manager
       *   Return type     : void 
       *   Argument        : object sender
       *   Argument        : System.EventArgs e
       * ********************************************************************************/
      private void chkTextOutPut_CheckedChanged(object sender, System.EventArgs e)
      {
         if (this.chkTextOutPut.Checked)
         {
#if DEBUG
             this.fdbCommon.SelectedPath = @"C:\output";
#endif
            if (this.fdbCommon.ShowDialog() == DialogResult.OK)
            {
#if DEBUG
                 
               string [] files = Directory.GetFiles(this.fdbCommon.SelectedPath);
               for (int i=0;i<files.Length;i++)
                  File.Delete(files[i]);
#endif 

               if (System.IO.Directory.GetFiles(this.fdbCommon.SelectedPath).Length > 0)
               {

                  //MessageBox.Show("That directory already has files. Please choose an empty directory");
                  this.chkTextOutPut.Checked = false;
               }
               else
               {
                  this.mySimManager.DoTextOutPut = true;
                  this.mySimManager.AnimalManager.AnimalAttributes.OutPutDir = this.fdbCommon.SelectedPath;
                  this.mySimManager.setResidentsTextOutPut(this.fdbCommon.SelectedPath);
               }
            }
            else
            {
               this.chkTextOutPut.Checked = false;
            }
         }
      }

      private void btnOutMapDir_Click(object sender, System.EventArgs e)
      {
         string startYear;
         try
         {
            this.Cursor = Cursors.WaitCursor;
#if DEBUG
            this.fdbCommon.SelectedPath = @"C:\map";
            string [] files;
            string [] Dirs;
            Dirs = Directory.GetDirectories(@"C:\map");
            for(int j=0;j<Dirs.Length;j++)
               Directory.Delete(Dirs[j],true);
            
            files = Directory.GetFiles(@"C:\map");
            for (int i=0;i<files.Length;i++)
               File.Delete(files[i]);
#endif
            this.fdbCommon.ShowNewFolderButton=true;
            if (this.fdbCommon.ShowDialog() == DialogResult.OK)
            {
               
               //this will help use group the maps by year run 
               startYear = this.mySimManager.StartSeasonDate.Year.ToString();
               this.mySimManager.MapManager.OutMapPath = this.fdbCommon.SelectedPath + '\\' + startYear;
               if(! this.mySimManager.makeInitialAnimalMaps())
               {
                  System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage,"Error");
                  this.mySimManager.ErrMessage = "";
               }
               if (!this.mySimManager.makeResidentMaps())
               {
                  System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage,"Error");
                  this.mySimManager.ErrMessage = "";
               }
               if (!mySimManager.MapManager.MakeCurrStepMap(this.fdbCommon.SelectedPath))
               {
                  System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage, "Error");
                  this.mySimManager.ErrMessage = "";
               }

            }

         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         this.btnTabSimOK.Enabled = true;
         this.Cursor = Cursors.Default;
         
      }


      public void doRun(string inputfile,string outputdir,string txtoutpudir, string backupxml)
      {
          //Console.WriteLine("In doRun");
          if (!IsLicensed())
          {
              Console.WriteLine("This application is not licensed with ARC GIS.");
              Environment.Exit(-1);
          }

          // check for exceptions
          loadXML(inputfile);

          // set output path to map files.
          try
          {
                  //this will help use group the maps by year run 
          string newStartYear = this.mySimManager.StartSeasonDate.Year.ToString();
          this.mySimManager.MapManager.OutMapPath = outputdir + '\\' + newStartYear;
                  if (!this.mySimManager.makeInitialAnimalMaps())
                  {
                      Console.WriteLine("makeInitialAnimalMaps is the problem");
                      //System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage, "Error");
                      //this.mySimManager.ErrMessage = "";
                  }
                  if (!this.mySimManager.makeResidentMaps())
                  {
                      Console.WriteLine("makeResidentMaps is the problem");
                      //System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage, "Error");
                      //this.mySimManager.ErrMessage = "";
                  }
                  if (!mySimManager.MapManager.MakeCurrStepMap(outputdir))
                  {
                      Console.WriteLine("MakeCurrStepMap is the problem");
                      //System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage, "Error");
                      //this.mySimManager.ErrMessage = "";
                  }

              }

          
          catch (System.Exception ex)
          {
              eLog.Debug(ex);
          }

          
          //
          this.mySimManager.setResidentAttributes(System.Convert.ToDouble(this.txtResDieTimeStep.Text),
                  System.Convert.ToDouble(this.txtResDieBetweenSeason.Text),
                  System.Convert.ToDouble(this.txtResBreedPercent.Text),
                  System.Convert.ToDouble(this.txtResFemalePercent.Text),
                  System.Convert.ToDouble(this.txtResOffspringMean.Text),
                  System.Convert.ToDouble(this.txtResOffspringSD.Text));
          this.mySimManager.NumSeasons = System.Convert.ToInt32(this.txtNumYears.Text);

          this.mySimManager.DoTextOutPut = true;
          this.mySimManager.AnimalManager.AnimalAttributes.OutPutDir = txtoutpudir;
          this.mySimManager.setResidentsTextOutPut(txtoutpudir);


          bool success = this.mySimManager.AnimalManager.setSleepTime(this.mySimManager.StartSeasonDate.AddHours(this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime));
          if (!success)
          {
              Console.WriteLine("Something wrong with animal sleep time");
              Environment.Exit(-1);
          }

          // check for exceptions
          writeXML(backupxml);


          //Console.WriteLine("Going to doSimulation");
          // ready to run.
          this.mySimManager.doSimulation(this); //dave broke it here.
          //Console.WriteLine("Finished doSimulation");
          //this.mySimManager.doTestCheckPoint(this);
          //Console.WriteLine("Setting managers to null");
          this.myMapManager = null;
          this.mySimManager = null;



      }


      /********************************************************************************
       *   Function name   : btnTabSimOK_Click
       *   Description     : Checks the enteries for numeric entries and then sets the
       *                     value for the sim manager
       *   Return type     : void 
       *   Argument        : object sender
       *   Argument        : System.EventArgs e
       * ********************************************************************************/
      private void btnTabSimOK_Click(object sender, System.EventArgs e)
      {
         bool success = true;
         if (this.areTextBoxesFilled(this.simulationText))
         {
            if(this.mySimManager.MapManager.OutMapPath != null)
            {
               this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
               this.mySimManager.setResidentAttributes(System.Convert.ToDouble(this.txtResDieTimeStep.Text),
                  System.Convert.ToDouble(this.txtResDieBetweenSeason.Text),
                  System.Convert.ToDouble(this.txtResBreedPercent.Text),
                  System.Convert.ToDouble(this.txtResFemalePercent.Text),
                  System.Convert.ToDouble(this.txtResOffspringMean.Text),
                  System.Convert.ToDouble(this.txtResOffspringSD.Text));

              


               this.mySimManager.NumSeasons = System.Convert.ToInt32(this.txtNumYears.Text);
               if (success)
               {
                  success = this.mySimManager.AnimalManager.setSleepTime(this.mySimManager.StartSeasonDate.AddHours(this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime));
                  if (!success)
                     MessageBox.Show(this.mySimManager.AnimalManager.ErrMessage);
               }
            }
            else
            {
               success = false;
               MessageBox.Show("You need to set the out put directory first using the 'Set Output For MapManager' button");
               this.btnOutMapDir.Focus();
            }
         }
         else
         { 
            success = false;
         }
         this.Cursor = System.Windows.Forms.Cursors.Arrow;

          //this.mySimManager.initializeYearlySimulation();
          mLog.Debug("writing parameter space to file:" + parameterFilePath);
          writeXML(this.parameterFilePath);
          mLog.Debug("inside the button click calling the sim manager do simulation");
          this.mySimManager.doSimulation(this);
          this.Close();
      }

      private void tabSim_Enter(object sender, EventArgs e)
      {
#if DEBUG
         this.txtResDieTimeStep.Text = ".00001";
         this.txtResDieBetweenSeason.Text = ".0001";
         this.txtResBreedPercent.Text = ".8";
         this.txtResFemalePercent.Text = ".45";
         this.txtResOffspringMean.Text = "3";
         this.txtResOffspringSD.Text = "1";
#endif

      }

		
      #endregion

      #region private methods


      public bool IsLicensed()
      {

          
         bool hasLicense = true;
         //try
         //{
         //   if (CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeEngine) != esriLicenseStatus.esriLicenseCheckedOut)
         //   {
         //      if (CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcView) != esriLicenseStatus.esriLicenseCheckedOut)
         //      {
         //         if (CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcEditor) != esriLicenseStatus.esriLicenseCheckedOut)
         //         {
         //            if (CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcInfo) != esriLicenseStatus.esriLicenseCheckedOut)
         //            {
         //               hasLicense = false;
         //            }
         //         }
         //      }
         //   }
         //}
         //catch(System.Exception ex)
         //{
         //   hasLicense = false;
         //   eLog.Debug(ex);
         //}
         return hasLicense;
      }
      private esriLicenseStatus CheckOutLicenses(esriLicenseProductCode productCode) 
      {
         esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
      
         AoInitializeClass m_AoInitialize = new AoInitializeClass();
         if (m_AoInitialize == null) return licenseStatus;


         //Determine if the product is available
         licenseStatus = m_AoInitialize.IsProductCodeAvailable(productCode);
         if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
         {
            licenseStatus = m_AoInitialize.Initialize(productCode);
         }
         return licenseStatus;
      }

      #endregion

      #region public methods
      public void updateProgressBar(double inPercent)
      {
         //this.pbarSim.Value = System.Convert.ToInt32(inPercent);

      }
      #endregion

      #region xmlcode
		
      private void writeXML(string fileName)
      {			
         XmlTextWriter xw = new XmlTextWriter(@fileName,null);
         xw.Formatting = Formatting.Indented;
         xw.Indentation = 2;
         xw.WriteStartDocument();
         xw.WriteStartElement("Data");
         #region Time Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","Time");
         xw.WriteElementString("StartSimDate",this.SimStartDate.Value.Date.ToShortDateString());
         xw.WriteElementString("EndDaySeason",this.txtEndDaySeason.Text);
         xw.WriteElementString("NumSeasonDays",this.txtNumSeasonDays.Text);
         //Author: Bob Cummings Wednesday, September 27, 2006
         //Commented out writing the end date.
         // We calculate it anyhow, so if they enter a bad date by modifying
         // an existing XML file the one written out for this run would not match.
       //  xw.WriteElementString("EndSimDate",this.txtEndDaySim.Text);
         xw.WriteElementString("NumYears",this.txtNumYears.Text);
         xw.WriteElementString("TimeBetweenDailyTimeStep",this.txtTimeBetweenDailyTimeStep.Text);
         xw.WriteElementString("StartTime", this.nudWakeTime.Value.ToString());
         xw.WriteEndElement();//end time tab
         #endregion
         #region Map Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","Map");
         xw.WriteStartElement("Directory");
         xw.WriteAttributeString("type","Social");
         xw.WriteString(socialDir);
         xw.WriteEndElement();//end of Directory element
         xw.WriteStartElement("Directory");
         xw.WriteAttributeString("type","Food");
         xw.WriteString(foodDir);
         xw.WriteEndElement();//end of Directory element
         xw.WriteStartElement("Directory");
         xw.WriteAttributeString("type","Predation");
         xw.WriteString(riskDir);
         xw.WriteEndElement();//end of Directory element
         xw.WriteStartElement("Directory");
         xw.WriteAttributeString("type","Move");
         xw.WriteString(moveDir);
         xw.WriteEndElement();//end of Directory element
         xw.WriteStartElement("Directory");
         xw.WriteAttributeString("type","Dispersal");
         xw.WriteString(disDir);
         xw.WriteEndElement();//end of Directory element
         this.myMapManager.writeXMLTriggers(ref xw);
         xw.WriteEndElement();//end of map tab element
         #endregion
         #region Species Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","Species");
         xw.WriteStartElement("EnergyParameters");
         xw.WriteElementString("InitialEnergy",this.txtInitialEnergy.Text);
         xw.WriteElementString("MaxEnergy",this.txtMaxEnergy.Text);
         xw.WriteElementString("MinEnergy",this.txtMinEnergy.Text);
         xw.WriteEndElement();//end of energyParameter element
         xw.WriteStartElement("TemporalParameters");
         xw.WriteElementString("WakeUpTime",this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime.ToString());
         xw.WriteStartElement("ActivityDurations");
         Duration [] ad = this.mySimManager.AnimalManager.AnimalAttributes.ActivityDurations;
         IEnumerator  enumerator = ad.GetEnumerator();
         Duration tempDur;
         while (enumerator.MoveNext())
         {
            tempDur = (Duration)enumerator.Current;
            xw.WriteStartElement("Duration");
            xw.WriteAttributeString("Type",tempDur.Type);
            xw.WriteElementString("MeanAmt",tempDur.MeanAmt.ToString());
            xw.WriteElementString("StandardDeviation",tempDur.StandardDeviation.ToString());
            xw.WriteEndElement();//end of Duration
         }
         xw.WriteEndElement();//end of ActivityDurations
         xw.WriteEndElement();//end of TemporalParameters
         xw.WriteStartElement("BehavioralTriggers");
         xw.WriteElementString("txtRiskyToSafe",this.txtRiskyToSafe.Text);
         xw.WriteElementString("txtSafeToRisky",this.txtSafeToRisky.Text);
         xw.WriteElementString("txtSearchForageTrigger",this.txtSearchForageTrigger.Text);
         xw.WriteEndElement();//end of behavioral triggers
         xw.WriteElementString("txtPerception",this.txtPerception.Text);
         xw.WriteEndElement();//end of species tab element
         #endregion
         #region Homerange Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","HomeRange");
         xw.WriteStartElement("HomeRangeArea");
         xw.WriteAttributeString("gender","male");
         xw.WriteString(this.txtMaleHomeRangeArea.Text);
         xw.WriteEndElement();
         xw.WriteStartElement("HomeRangeArea");
         xw.WriteAttributeString("gender","female");
         xw.WriteString(this.txtFemaleHomeRangeArea.Text);
         xw.WriteEndElement();//enf of female area  element
         
         xw.WriteStartElement("MaleDistanceModifierWeight");
         xw.WriteAttributeString("gender","male");
         xw.WriteString(this.txtMaleDistMod.Text);
         xw.WriteEndElement();

         xw.WriteStartElement("FemaleDistanceModifierWeight");
         xw.WriteAttributeString("gender","female");
         xw.WriteString(this.txtFemaleDistMod.Text);
         xw.WriteEndElement();





         xw.WriteStartElement("HomeRangeTrigger");
         xw.WriteAttributeString("type","steps");
         xw.WriteString(this.rdoNumSteps.Checked.ToString());
         xw.WriteEndElement();//end of number of steps trigger element
         xw.WriteStartElement("HomeRangeTrigger");
         xw.WriteAttributeString("type","sites");
         xw.WriteString(this.rdoNumSites.Checked.ToString());
         xw.WriteEndElement();//end of number of sites trigger element
         xw.WriteElementString("txtTriggerNum",txtTriggerNum.Text);
         xw.WriteStartElement("HomeRangeCriteria");
         xw.WriteAttributeString("type","Closest");
         xw.WriteString(this.rdoClosest.Checked.ToString());
         xw.WriteEndElement();//end of closest criteria element
         xw.WriteStartElement("HomeRangeCriteria");
         xw.WriteAttributeString("type","food");
         xw.WriteString(this.rdoBestFood.Checked.ToString());
         xw.WriteEndElement();//end of food criteria element
         xw.WriteStartElement("HomeRangeCriteria");
         xw.WriteAttributeString("type","risk");
         xw.WriteString(this.rdoRisk.Checked.ToString());
         xw.WriteEndElement();//end of risk criteria element
         xw.WriteStartElement("HomeRangeCriteria");
         xw.WriteAttributeString("type","combo");
         xw.WriteString(this.rdoCombo.Checked.ToString());
         xw.WriteEndElement();//end of combo criteria element
         xw.WriteEndElement();//end of home range tab element
         #endregion
         #region Movement Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","Movement");
         xw.WriteStartElement("AnimalModifiers");
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","MaleModifier");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.MaleModifier.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.MaleModifier.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.MaleModifier.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.MaleModifier.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.MaleModifier.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.MaleModifier.PerceptonModifier.ToString());
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.MaleModifier.PredationRisk.ToString());
         xw.WriteEndElement();//end of MaleModifiers
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","FemaleModifier");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.FemaleModifier.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.FemaleModifier.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.FemaleModifier.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.FemaleModifier.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.FemaleModifier.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.FemaleModifier.PerceptonModifier.ToString());
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.FemaleModifier.PredationRisk.ToString());
         xw.WriteEndElement();//end of FemaleModifier
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","RiskyForageMod");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.RiskyForageMod.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.RiskyForageMod.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.RiskyForageMod.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.RiskyForageMod.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.RiskyForageMod.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.RiskyForageMod.PerceptonModifier.ToString());
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.RiskyForageMod.PredationRisk.ToString());
         xw.WriteEndElement();//end of RiskyForageMod
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","RiskySearchMod");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.RiskySearchMod.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.RiskySearchMod.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.RiskySearchMod.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.RiskySearchMod.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.RiskySearchMod.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.RiskySearchMod.PerceptonModifier.ToString());						
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.RiskySearchMod.PredationRisk.ToString());
         xw.WriteEndElement();//end of RiskySearchMod
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","SafeForageMod");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.SafeForageMod.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.SafeForageMod.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.SafeForageMod.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.SafeForageMod.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.SafeForageMod.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.SafeForageMod.PerceptonModifier.ToString());
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.SafeForageMod.PredationRisk.ToString());
         xw.WriteEndElement();//end of SafeForageMod
         xw.WriteStartElement("Modifier");
         xw.WriteAttributeString("type","SafeSearchMod");
         xw.WriteElementString("CaptureFood",this.mySimManager.AnimalManager.SafeSearchMod.CaptureFood.ToString());
         xw.WriteElementString("EnergyUsed",this.mySimManager.AnimalManager.SafeSearchMod.EnergyUsed.ToString());
         xw.WriteElementString("MoveSpeed",this.mySimManager.AnimalManager.SafeSearchMod.MoveSpeed.ToString());
         xw.WriteElementString("MoveTurtosity",this.mySimManager.AnimalManager.SafeSearchMod.MoveTurtosity.ToString());
         xw.WriteElementString("Name",this.mySimManager.AnimalManager.SafeSearchMod.Name);
         xw.WriteElementString("PerceptonModifier",this.mySimManager.AnimalManager.SafeSearchMod.PerceptonModifier.ToString());
         xw.WriteElementString("PredationRisk",this.mySimManager.AnimalManager.SafeSearchMod.PredationRisk.ToString());
         xw.WriteEndElement();//end of SafeSearchMod
         xw.WriteEndElement();//end of AnimalModifiers
         xw.WriteStartElement("TemporalModifiers");
         xw.WriteStartElement("DailyModifiers");
         DailyModiferCollection dmc = this.mySimManager.GetDailyModiferCollection();
         int cnt = 0;
         DailyModifier dm;
         while (cnt++ < dmc.Count)
         {
            dm = dmc.getNext();
            xw.WriteStartElement("Modifier");
            xw.WriteAttributeString("type","DailyModifier");
            xw.WriteElementString("StartDate",dm.StartDate.ToString());
            xw.WriteElementString("CaptureFood",dm.CaptureFood.ToString());
            xw.WriteElementString("EnergyUsed",dm.EnergyUsed.ToString());
            xw.WriteElementString("MoveSpeed",dm.MoveSpeed.ToString());
            xw.WriteElementString("MoveTurtosity",dm.MoveTurtosity.ToString());
            xw.WriteElementString("Name",dm.Name);
            xw.WriteElementString("PerceptonModifier",dm.PerceptonModifier.ToString());
            xw.WriteElementString("PredationRisk",dm.PredationRisk.ToString());
            xw.WriteEndElement();//end of DailyModifier
         }
         xw.WriteEndElement();//end of Daily modifiers
         xw.WriteStartElement("HourlyModifiers");
         HourlyModifierCollection hmc = this.mySimManager.GetHourlyModifierCollection();
         HourlyModifier hm;
         cnt = 0;
         while (cnt++ < hmc.Count)
         {
            hm = hmc.getNext();
            xw.WriteStartElement("Modifier");
            xw.WriteAttributeString("type","HourlyModifier");
            xw.WriteElementString("StartTime",hm.StartTime.ToString());
            xw.WriteElementString("CaptureFood",hm.CaptureFood.ToString());
            xw.WriteElementString("EnergyUsed",hm.EnergyUsed.ToString());
            xw.WriteElementString("MoveSpeed",hm.MoveSpeed.ToString());
            xw.WriteElementString("MoveTurtosity",hm.MoveTurtosity.ToString());
            xw.WriteElementString("Name",hm.Name);
            xw.WriteElementString("PerceptonModifier",hm.PerceptonModifier.ToString());
            xw.WriteElementString("PredationRisk",hm.PredationRisk.ToString());
            xw.WriteEndElement();//end of HourlyModifier
         }
         xw.WriteEndElement();//end of hourly modifiers
         xw.WriteEndElement();//end of TemporalModifiers
         xw.WriteEndElement();//end of movement tab element
         #endregion
         #region Simulation Tab
         xw.WriteStartElement("Tab");
         xw.WriteAttributeString("name","Simulation");
         xw.WriteStartElement("Resident");
         xw.WriteElementString("txtResDieBetweenSeason",this.txtResDieBetweenSeason.Text);
         xw.WriteElementString("txtResDieTimeStep",this.txtResDieTimeStep.Text);
         xw.WriteElementString("txtResBreedPercent",this.txtResBreedPercent.Text);
         xw.WriteElementString("txtResFemalePercent",this.txtResFemalePercent.Text);
         xw.WriteElementString("txtResOffspringMean",this.txtResOffspringMean.Text);
         xw.WriteElementString("txtResOffspringSD",this.txtResOffspringSD.Text);
         xw.WriteEndElement();//end of resident element
			
         xw.WriteStartElement("Output");
         xw.WriteElementString("OutputPath",this.mySimManager.MapManager.OutMapPath);
         xw.WriteElementString("TextOutput",this.mySimManager.DoTextOutPut.ToString());
         xw.WriteEndElement();//end of output element
         xw.WriteEndElement();//end of Simulation tab element
         #endregion
         xw.WriteEndElement();//end of Data
         xw.WriteEndDocument();
         xw.Flush();
         xw.Close();
      }//end of writeXML
      private void btnLoadXML_Click(object sender, System.EventArgs e)
      {
         string path;
         System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
         ofd.Filter = "XML files .xml|*.xml";
         try
         {
            mLog.Debug("inside btnLoadXML_Click");
            ofd.Title = "Browse for file to load parameters from:";
            //	ofd.Filter = "*.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               path = ofd.FileName;
               mLog.Debug("the xml parameter file to be loaded is " + path);
               mLog.Debug("calling load xml");
               loadXML(path);
            }//end if show dialog is good
            mLog.Debug("leaving btnLoadXML_Click");
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         ofd.Filter = "";
      }//end of btnLoadXML_Click
      public void loadXML(string file)
      {
         try
         {
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(file);
            System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
            System.Xml.XPath.XPathNodeIterator result;
            result = nav.Select("//Tab[@name=\"Time\"]");
            loadTimeTab(result);
            result = nav.Select("//Tab[@name=\"Map\"]");
            loadMapTab(result);

            result = nav.Select("//MapSwitchTriggers[@name=\"Social\"]");
            this.myMapManager.loadXMLTriggers("Social",result);

            result = nav.Select("//MapSwitchTriggers[@name=\"Food\"]");
            this.myMapManager.loadXMLTriggers("Food",result);

            result = nav.Select("//MapSwitchTriggers[@name=\"Predation\"]");
            this.myMapManager.loadXMLTriggers("Predation",result);

            result = nav.Select("//MapSwitchTriggers[@name=\"Move\"]");
            this.myMapManager.loadXMLTriggers("Move",result);

            result = nav.Select("//MapSwitchTriggers[@name=\"Dispersal\"]");
            this.myMapManager.loadXMLTriggers("Dispersal",result);
            
            finishMapTab();

            result = nav.Select("//Tab[@name=\"Species\"]");
            loadSpeciesTab(result);
            result = nav.Select("//Tab[@name=\"HomeRange\"]");
            loadHomeRangeTab(result);
            result = nav.Select("//Tab[@name=\"Movement\"]");
            loadMovementTab(result);
            result = nav.Select("//Tab[@name=\"Simulation\"]");
#if (! DEBUG)

            loadSimulationTab(result);

#endif

         }//end of try
         catch (CraptasticXmlException e)
         {
            MessageBox.Show("The parameter file is not well formed." + System.Environment.NewLine 
               + e.Message + System.Environment.NewLine + " Aborting load of parameters." );
         }//end of catch xml
         catch (InvalidMapException e)
         {
            MessageBox.Show(e.Message + System.Environment.NewLine + " Aborting load of parameters." );
         }
         catch (Exception e)
         {
            MessageBox.Show("Error of type " + e.ToString() + " encountered. " + System.Environment.NewLine + e.Message + " \nAborting load of parameters.");
         }
      }
      private void loadTimeTab(XPathNodeIterator inIterator)
      {
         DateTime dt;
         string [] tempString;
         try
         {
           
            XPathNodeIterator temp = inIterator.Current.Select("//StartDate");
            temp.MoveNext();
            tempString = temp.Current.Value.Split('/');
            dt = new DateTime(System.Convert.ToInt32(tempString[2]),System.Convert.ToInt32(tempString[0]),System.Convert.ToInt32(tempString[1]));
            this.SimStartDate.Value = dt;
            this.dtpSeasonStartDate.Value = dt;
            temp = inIterator.Current.Select("//EndDaySeason");
            temp.MoveNext();
            txtEndDaySeason.Text = temp.Current.Value;
            temp = inIterator.Current.Select("//NumSeasonDays");
            temp.MoveNext();
            txtNumSeasonDays.Text = temp.Current.Value;
            temp = inIterator.Current.Select("//NumYears");
            temp.MoveNext();
            txtNumYears.Text = temp.Current.Value;
            temp = inIterator.Current.Select("//TimeBetweenDailyTimeStep");
            temp.MoveNext();
            this.txtTimeBetweenDailyTimeStep.Text = temp.Current.Value;
            temp = inIterator.Current.Select("//StartTime");
            temp.MoveNext();
            this.nudWakeTime.Value=System.Convert.ToDecimal(temp.Current.Value);
            
        /*    temp = inIterator.Current.Select("//EndSimDate");
            temp.MoveNext();
            this.txtEndDaySim.Text = temp.Current.Value;*/
            this.txtNumSeasonDays_LostFocus(null,null);
            btnTime_Click(null, null);

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      private void loadMapTab(XPathNodeIterator nit)
      {
         XPathNodeIterator kids = nit.Current.Select("//Directory");
         kids.MoveNext();
         socialDir = kids.Current.Value;
         this.btnSocialMaps_Click(null,null);
         kids.MoveNext();
         foodDir = kids.Current.Value;
         this.btnFoodMaps_Click(null,null);
         kids.MoveNext();
         riskDir = kids.Current.Value;
         this.btnPredationMap_Click(null,null);
         kids.MoveNext();
         moveDir = kids.Current.Value;
         this.btnMove_Click(null,null);
         kids.MoveNext();
         disDir = kids.Current.Value;
         this.btnDispersal_Click(null,null);
      }
      private void finishMapTab()
      {
         this.myMapManager.changeMaps(this.mySimManager.StartSimulationDate);
         bool success = this.mySimManager.buildAnimals();
         this.mySimManager.buildResidents();
         if(success)
         {
            //MessageBox.Show("Map times have been successfully set");
            this.ShowTabPage(this.tabSpecies);
            this.tabSpecies.Focus();
            mLog.Debug("leaving btnDispersal_Click");
         }
         else
         {
            MessageBox.Show(this.mySimManager.ErrMessage);
         }
      }
      private void loadSpeciesTab (XPathNodeIterator nit)
      {
         XPathNodeIterator temp = nit.Current.Select("//EnergyParameters/InitialEnergy");
         temp.MoveNext();
         this.txtInitialEnergy.Text = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//EnergyParameters/MaxEnergy");
         temp.MoveNext();
         this.txtMaxEnergy.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//EnergyParameters/MinEnergy");
         temp.MoveNext();
         this.txtMinEnergy.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//TemporalParameters/WakeUpTime");
         temp.MoveNext();
         //		this.mySimManager.AnimalManager.AnimalAttributes.WakeUpTime  = System.Double.Parse(temp.Current.Value);
         //this.dtpWakeUp.Text = temp.Current.Value + ":00";
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//ActivityDurations/*");
         loadActivityDurations(temp);
         temp = nit.Current.Select("//BehavioralTriggers/txtRiskyToSafe");
         temp.MoveNext();
         this.txtRiskyToSafe.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//BehavioralTriggers/txtSafeToRisky");
         temp.MoveNext();
         this.txtSafeToRisky.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//BehavioralTriggers/txtSearchForageTrigger");
         temp.MoveNext();
         this.txtSearchForageTrigger.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//txtPerception");
         temp.MoveNext();
         this.txtPerception.Text  = temp.Current.Value;
         //		MessageBox.Show("Name = " + temp.Current.Name + "\nValue = " + temp.Current.Value);
         btnSpecies_Click(null, null);
         //this.btnSpecies.PerformClick();
      }
      private void loadActivityDurations(XPathNodeIterator nit)
      {
         string type;
         string mean;
         string stdDev;
         while (nit.MoveNext())
         {
            type = nit.Current.GetAttribute("Type","");
            nit.Current.MoveToFirstChild();
            mean = nit.Current.Value;
            nit.Current.MoveToNext();
            stdDev = nit.Current.Value;
            //Console.WriteLine("Type = " + type + "\nMean = " + mean + "\nStd Dev = " + stdDev);
            nit.Current.MoveToParent();
            this.txtDurMean.Text = mean;
            this.txtDurSD.Text = stdDev;
            if (type.Equals("Active"))
            {
                //Console.WriteLine("inside add active");
                btnAddActiveDuration_Click(null, null);
               //this.btnAddActiveDuration.PerformClick();
            }
            else if ( type.Equals("Sleeping"))
            {
                //Console.WriteLine("inside add sleeping");
                btnAddRestDuration_Click(null, null);
               //this.btnAddRestDuration.PerformClick();
            }
            else
            {
               throw new CraptasticXmlException("Expected Activity Duration element of type Active or Sleeping!  Instead I got type " + type);
            }

         }
      }
      private void loadHomeRangeTab(XPathNodeIterator nit)
      {
         XPathNodeIterator temp = nit.Current.Select("//HomeRangeArea[@gender=\"male\"]");
         temp.MoveNext();
         this.txtMaleHomeRangeArea.Text = temp.Current.Value;
         //	MessageBox.Show("Name = " + temp.Current.Name + "\nGender = " + temp.Current.GetAttribute("gender","") + "\nValue = " + temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeArea[@gender=\"female\"]");
         temp.MoveNext();
         this.txtFemaleHomeRangeArea.Text = temp.Current.Value;
         temp = nit.Current.Select("//HomeRangeTrigger[@type=\"steps\"]");
         temp.MoveNext();
         this.rdoNumSteps.Checked =  System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeTrigger[@type=\"sites\"]");
         temp.MoveNext();
         this.rdoNumSites.Checked =  System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeCriteria[@type=\"Closest\"]");
         temp.MoveNext();
         this.rdoClosest.Checked =  System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeCriteria[@type=\"food\"]");
         temp.MoveNext();
         this.rdoBestFood.Checked = System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeCriteria[@type=\"risk\"]");
         temp.MoveNext();
         this.rdoRisk.Checked = System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//HomeRangeCriteria[@type=\"combo\"]");
         temp.MoveNext();
         this.rdoCombo.Checked = System.Boolean.Parse(temp.Current.Value);
         temp = nit.Current.Select("//txtTriggerNum");
         temp.MoveNext();
         this.txtTriggerNum.Text = temp.Current.Value;
         temp = nit.Current.Select("//MaleDistanceModifierWeight[@gender=\"male\"]");
         if(temp.MoveNext())
            this.txtMaleDistMod.Text = temp.Current.Value;
         else
            MessageBox.Show("Error looking for tag MaleDistanceModifierWeight" );
         temp = nit.Current.Select("//FemaleDistanceModifierWeight[@gender=\"female\"]");
         if(temp.MoveNext())
            this.txtFemaleDistMod.Text = temp.Current.Value;
         else
            MessageBox.Show("Error looking for tag FemaleDistanceModifierWeight" );
         btnHomeRange_Click(null, null);
         //this.btnHomeRange.PerformClick();
      }
      private void loadMovementTab(XPathNodeIterator nit)
      {
         Modifier tm;
         String type;
         XPathNodeIterator temp = nit.Current.Select("//AnimalModifiers/*");
         while (temp.MoveNext())
         {
            type = temp.Current.GetAttribute("type","");
            //		MessageBox.Show("Type = " + type);
            switch (type)
            {
               case "MaleModifier":
                  tm = MaleModifier.GetUniqueInstance();
                  break;
               case "FemaleModifier":
                  tm = FemaleModifier.GetUniqueInstance();
                  break;
               case "RiskyForageMod":
                  tm = new RiskyForageModifier();
                  break;
               case "RiskySearchMod":
                  tm = new RiskySearchModifier();
                  break;
               case "SafeForageMod":
                  tm = new SafeForageModifier();
                  break;
               case "SafeSearchMod":
                  tm = new SafeSearchModifier();
                  break;
               default:
                  throw new CraptasticXmlException("Unknown Modifier element type!  Received: " + type);
            }//end of switch
            temp.Current.MoveToFirstChild();
            tm.CaptureFood = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            tm.EnergyUsed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            tm.MoveSpeed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            tm.MoveTurtosity = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            tm.Name = temp.Current.Value;
            temp.Current.MoveToNext();
            tm.PerceptonModifier = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            tm.PredationRisk = System.Convert.ToDouble(temp.Current.Value);

            switch (type)
            {
               case "MaleModifier":
                  this.mySimManager.AnimalManager.MaleModifier = (MaleModifier)tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Male Modifer Created" + System.Environment.NewLine;
                  break;
               case "FemaleModifier":
                  this.mySimManager.AnimalManager.FemaleModifier = (FemaleModifier)tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Female Modifer Created" + System.Environment.NewLine;
                  break;
               case "RiskyForageMod":
                  this.mySimManager.AnimalManager.RiskyForageMod = tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Risky Forage Modifer Created" + System.Environment.NewLine;
                  break;
               case "RiskySearchMod":
                  this.mySimManager.AnimalManager.RiskySearchMod = tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Risky Search Modifer Created" + System.Environment.NewLine;
                  break;
               case "SafeForageMod":
                  this.mySimManager.AnimalManager.SafeForageMod = tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Safe Forage Modifer Created" + System.Environment.NewLine;
                  break;
               case "SafeSearchMod":
                  this.mySimManager.AnimalManager.SafeSearchMod = tm;
                  this.lblModifiers.Text = this.lblModifiers.Text + "Safe Search Modifer Created" + System.Environment.NewLine;
                  break;
               default:
                  throw new CraptasticXmlException("Unknown Modifier element type!  Received: " + type);
            }//end of switch
         }//end of while
         temp = nit.Current.Select("//DailyModifiers/*");
         while (temp.MoveNext())
         {
            type = temp.Current.GetAttribute("type","");
            //		MessageBox.Show("Type = " + type);
            DailyModifier dm = new DailyModifier();
            temp.Current.MoveToFirstChild();
            dm.StartDate = System.DateTime.Parse(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.CaptureFood = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.EnergyUsed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.MoveSpeed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.MoveTurtosity = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.Name = temp.Current.Value;
            temp.Current.MoveToNext();
            dm.PerceptonModifier = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            dm.PredationRisk = System.Convert.ToDouble(temp.Current.Value);

            this.mySimManager.addDailyModifier(dm);
            this.lblModifiers.Text = this.lblModifiers.Text + "Daily Modifer Created" + System.Environment.NewLine;

         }//end of while
         temp = nit.Current.Select("//HourlyModifiers/*");
         while (temp.MoveNext())
         {
            type = temp.Current.GetAttribute("type","");
            //		MessageBox.Show("Type = " + type);
            HourlyModifier hm = new HourlyModifier();
            temp.Current.MoveToFirstChild();
            hm.StartTime = System.Convert.ToInt32(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.CaptureFood = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.EnergyUsed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.MoveSpeed = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.MoveTurtosity = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.Name = temp.Current.Value;
            temp.Current.MoveToNext();
            hm.PerceptonModifier = System.Convert.ToDouble(temp.Current.Value);
            temp.Current.MoveToNext();
            hm.PredationRisk = System.Convert.ToDouble(temp.Current.Value);
            this.mySimManager.addHourlyModifier(hm);
            this.lblModifiers.Text = this.lblModifiers.Text + "Hourly Modifer Created" + System.Environment.NewLine;
         }//end of while
         btnMoveOK_Click(null, null);
         //this.btnMoveOK.PerformClick();
      }
      private void loadSimulationTab(XPathNodeIterator nit)
      {
         XPathNodeIterator temp = nit.Current.Select("//txtResDieBetweenSeason");
         temp.MoveNext();
         txtResDieBetweenSeason.Text = temp.Current.Value;
         temp = nit.Current.Select("//txtResDieTimeStep");
         temp.MoveNext();
         txtResDieTimeStep.Text = temp.Current.Value;
         temp = nit.Current.Select("//txtResBreedPercent");
         temp.MoveNext();
         txtResBreedPercent.Text = temp.Current.Value;
         temp = nit.Current.Select("//txtResFemalePercent");
         temp.MoveNext();
         txtResFemalePercent.Text = temp.Current.Value;
         temp = nit.Current.Select("//txtResOffspringMean");
         temp.MoveNext();
         txtResOffspringMean.Text = temp.Current.Value;
         temp = nit.Current.Select("//txtResOffspringSD");
         temp.MoveNext();
         txtResOffspringSD.Text = temp.Current.Value;
			/*
         temp.MoveNext();
         temp = nit.Current.Select("//OutputPath");
         temp.MoveNext();
         this.mySimManager.MapManager.OutMapPath =  temp.Current.Value;
         temp = nit.Current.Select("//TextOutput");
         temp.MoveNext();
         this.chkTextOutPut.Checked = System.Boolean.Parse(temp.Current.Value);
         if(! this.mySimManager.makeInitialAnimalMaps())
         {
            System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage,"Error");
            this.mySimManager.ErrMessage = "";
         }
         if(! mySimManager.makeTempMap())
         {
            System.Windows.Forms.MessageBox.Show(this.mySimManager.ErrMessage,"Error");
            this.mySimManager.ErrMessage = "";
         }
         //btnTabSimOK.PerformClick();*/
      }
      #endregion
   }
			
}



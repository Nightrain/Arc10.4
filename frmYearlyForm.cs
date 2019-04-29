using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SEARCH;

using inputBox;
using log4net;

namespace SEARCH
{
   /// <summary>
   /// Summary description for YearlyForm.
   /// </summary>
   public class YearlyForm : System.Windows.Forms.Form
   {
		#region Public Members (2) 

		#region Constructors (2) 

       private ILog eLog = LogManager.GetLogger("Error");

      public YearlyForm(string mapType, ref SimulatonManager sm, string description):this()
      {
         this.mySimManager = sm;
         mMapType = mapType;
         mapDescription = description;
         switch (mapType)
         {
            case "Social":
               myMaps = myMapManager.mySocialMaps;
               break;
            case "Food":
               myMaps = myMapManager.myFoodMaps;
               break;
            case "Risk":
               myMaps = myMapManager.myPredationMaps;
               break;
            case "Predation":
               myMaps = myMapManager.myPredationMaps;
               break;
            case "Move":
               myMaps = myMapManager.myMoveMaps;
               break;
            case "Dispersal":
               myMaps = myMapManager.myDispersalMaps;
               break;
            default:
               break;
         }
         mapPath = myMaps.MyPath;
         this.Text += " for " + mapType + " maps";
         
      }

      public YearlyForm()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         this.myMapManager = MapManager.GetUniqueInstance();

      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (25) 

		#region Fields (21) 

      private System.Windows.Forms.Button btnChooseMaps;
      private System.Windows.Forms.Button btnOK;
	      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;
      private System.Windows.Forms.DataGrid dataGrid1;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label lblDisplay;
      private string mapDescription;
      private string mapPath;
      private string mMapType;
      private MapManager myMapManager;
      private Maps myMaps;
      private SimulatonManager mySimManager;
      private System.Windows.Forms.TextBox txtNumDays;
      private System.Windows.Forms.TextBox txtNumHours;
      private System.Windows.Forms.TextBox txtNumYears;

		#endregion Fields 
		#region Methods (4) 

      private void btnChooseMaps_Click(object sender, System.EventArgs e)
      {
         bool keepLooping = true;
         int counter = 0;
         int numYears = System.Convert.ToInt32(this.txtNumYears.Text);
         int numDays = System.Convert.ToInt32(this.txtNumDays.Text);
         int numHours = System.Convert.ToInt32(this.txtNumHours.Text);
         frmMapSelectForm f = new frmMapSelectForm(mMapType,mapPath,ref mySimManager);
         this.myMaps.MyTriggers = new MapSwapTrigger[numYears*numDays*numHours];
         
         
         f.setCalRange(this.mySimManager.StartSeasonDate,this.mySimManager.EndSimulatonDate);
         for(int i=0;i<numYears && keepLooping;i++)
            for(int j=0;j<numDays && keepLooping;j++)
               for(int h=0;h<numHours && keepLooping;h++)
               {
                  

                  //If this is not the first time through then bump the mindate up 
                  if(counter>0)
                     f.resetCalRange(this.myMaps.MyTriggers[counter-1].StartDate);

                  f.fillDisplay(swapYears[i],swapDays[j],swapHours[h]);

                  f.ShowDialog();
                  if (!f.Cancel)
                  {
                    
                     f.Mst.setTriggerType(numYears,numDays,numHours);
                     this.myMaps.MyTriggers[counter++]=f.Mst;
                    // this.myMaps.dumpTriggersHere();
                  }
                  else
                  {
                     //the user wanted to stop loading maps for some reason so stop the loop and empty out the que
                     keepLooping = false;
                     this.myMaps.MyTriggers = null;
                  }
               }
         if (!f.Cancel)
          this.myMaps.dumpTriggersHere();
         this.Close();
      }

      private void btnOK_Click(object sender, System.EventArgs e)
      {

         this.Close();
      }


      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

		#endregion Methods 

		#endregion Non-Public Members 
      private string [] swapYears;
      private string [] swapDays;
      private string [] swapHours;


      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YearlyForm));
          this.lblDisplay = new System.Windows.Forms.Label();
          this.label2 = new System.Windows.Forms.Label();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.txtNumYears = new System.Windows.Forms.TextBox();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.txtNumDays = new System.Windows.Forms.TextBox();
          this.label4 = new System.Windows.Forms.Label();
          this.btnChooseMaps = new System.Windows.Forms.Button();
          this.btnOK = new System.Windows.Forms.Button();
          this.groupBox3 = new System.Windows.Forms.GroupBox();
          this.txtNumHours = new System.Windows.Forms.TextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.dataGrid1 = new System.Windows.Forms.DataGrid();
          this.groupBox1.SuspendLayout();
          this.groupBox2.SuspendLayout();
          this.groupBox3.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
          this.SuspendLayout();
          // 
          // lblDisplay
          // 
          this.lblDisplay.Location = new System.Drawing.Point(264, 16);
          this.lblDisplay.Name = "lblDisplay";
          this.lblDisplay.Size = new System.Drawing.Size(256, 304);
          this.lblDisplay.TabIndex = 0;
          // 
          // label2
          // 
          this.label2.Location = new System.Drawing.Point(8, 24);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(168, 16);
          this.label2.TabIndex = 1;
          this.label2.Text = "Number of yearly maps";
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this.txtNumYears);
          this.groupBox1.Controls.Add(this.label2);
          this.groupBox1.Location = new System.Drawing.Point(16, 16);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(232, 88);
          this.groupBox1.TabIndex = 0;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Yearly map sets";
          // 
          // txtNumYears
          // 
          this.txtNumYears.Location = new System.Drawing.Point(168, 24);
          this.txtNumYears.Name = "txtNumYears";
          this.txtNumYears.Size = new System.Drawing.Size(48, 20);
          this.txtNumYears.TabIndex = 0;
          this.txtNumYears.Text = "1";
          this.txtNumYears.LostFocus += new System.EventHandler(this.txtNumYears_LostFocus);
          // 
          // groupBox2
          // 
          this.groupBox2.Controls.Add(this.txtNumDays);
          this.groupBox2.Controls.Add(this.label4);
          this.groupBox2.Location = new System.Drawing.Point(16, 120);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(232, 112);
          this.groupBox2.TabIndex = 1;
          this.groupBox2.TabStop = false;
          this.groupBox2.Text = "Seasonal map sets";
          // 
          // txtNumDays
          // 
          this.txtNumDays.Enabled = false;
          this.txtNumDays.Location = new System.Drawing.Point(168, 24);
          this.txtNumDays.Name = "txtNumDays";
          this.txtNumDays.Size = new System.Drawing.Size(48, 20);
          this.txtNumDays.TabIndex = 0;
          this.txtNumDays.Text = "1";
          this.txtNumDays.LostFocus += new System.EventHandler(this.txtNumDays_LostFocus);
          // 
          // label4
          // 
          this.label4.Location = new System.Drawing.Point(8, 24);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(152, 16);
          this.label4.TabIndex = 6;
          this.label4.Text = "Number of seasons per year";
          // 
          // btnChooseMaps
          // 
          this.btnChooseMaps.Enabled = false;
          this.btnChooseMaps.Location = new System.Drawing.Point(256, 328);
          this.btnChooseMaps.Name = "btnChooseMaps";
          this.btnChooseMaps.Size = new System.Drawing.Size(211, 23);
          this.btnChooseMaps.TabIndex = 3;
          this.btnChooseMaps.Text = "Select maps and specify start times";
          this.btnChooseMaps.Click += new System.EventHandler(this.btnChooseMaps_Click);
          // 
          // btnOK
          // 
          this.btnOK.Location = new System.Drawing.Point(491, 328);
          this.btnOK.Name = "btnOK";
          this.btnOK.Size = new System.Drawing.Size(75, 23);
          this.btnOK.TabIndex = 4;
          this.btnOK.Text = "OK";
          this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
          // 
          // groupBox3
          // 
          this.groupBox3.Controls.Add(this.txtNumHours);
          this.groupBox3.Controls.Add(this.label1);
          this.groupBox3.Location = new System.Drawing.Point(16, 248);
          this.groupBox3.Name = "groupBox3";
          this.groupBox3.Size = new System.Drawing.Size(232, 104);
          this.groupBox3.TabIndex = 2;
          this.groupBox3.TabStop = false;
          this.groupBox3.Text = "Daily map sets";
          // 
          // txtNumHours
          // 
          this.txtNumHours.Enabled = false;
          this.txtNumHours.Location = new System.Drawing.Point(168, 24);
          this.txtNumHours.Name = "txtNumHours";
          this.txtNumHours.Size = new System.Drawing.Size(48, 20);
          this.txtNumHours.TabIndex = 0;
          this.txtNumHours.Text = "1";
          this.txtNumHours.LostFocus += new System.EventHandler(this.txtNumHours_LostFocus);
          // 
          // label1
          // 
          this.label1.Location = new System.Drawing.Point(8, 24);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(152, 23);
          this.label1.TabIndex = 1;
          this.label1.Text = "Number of maps per day";
          // 
          // dataGrid1
          // 
          this.dataGrid1.DataMember = "";
          this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
          this.dataGrid1.Location = new System.Drawing.Point(256, 16);
          this.dataGrid1.Name = "dataGrid1";
          this.dataGrid1.ReadOnly = true;
          this.dataGrid1.Size = new System.Drawing.Size(616, 296);
          this.dataGrid1.TabIndex = 11;
          // 
          // YearlyForm
          // 
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.ClientSize = new System.Drawing.Size(920, 365);
          this.Controls.Add(this.dataGrid1);
          this.Controls.Add(this.groupBox3);
          this.Controls.Add(this.btnOK);
          this.Controls.Add(this.groupBox2);
          this.Controls.Add(this.groupBox1);
          this.Controls.Add(this.lblDisplay);
          this.Controls.Add(this.btnChooseMaps);
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.Name = "YearlyForm";
          this.Text = "Set map swap times";
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.groupBox2.ResumeLayout(false);
          this.groupBox2.PerformLayout();
          this.groupBox3.ResumeLayout(false);
          this.groupBox3.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
          this.ResumeLayout(false);

      }
      #endregion

      #region Years
      private void txtNumYears_LostFocus(object sender, EventArgs e)
      {
         bool success = false;
         try
         {
            int numYears = System.Convert.ToInt32(this.txtNumYears.Text);
            if(numYears > this.mySimManager.NumSeasons || numYears <= 0)
            {
               MessageBox.Show("You entered " + this.txtNumYears.Text + " years and the simulation is going to run for " + this.mySimManager.NumSeasons.ToString() + " years"  
                  + System.Environment.NewLine + "Please the correct amount of years.");
               this.txtNumYears.SelectAll();
               this.txtNumYears.Focus();
               
            }
            else if (numYears > 1)
            {  
               string tempYear;
               swapYears = new string[numYears];
               swapYears[0] = this.mySimManager.StartSeasonDate.Year.ToString();
               MessageBox.Show("The first year is set by default. Now set the swap times for the other years.");
               for (int i=1;i<numYears;i++)
               {
                  tempYear = InputBox.ShowInputBox("Please enter year " + i.ToString() + " to swap out (XXXX).","Yearly Map Sets");
                  while( tempYear.CompareTo(swapYears[i-1]) <= 0 || tempYear.CompareTo(this.mySimManager.EndSimulatonDate.Year.ToString()) > 0)
                  {
                     MessageBox.Show(tempYear + " is invalid. It must be between " + swapYears[i-1] + " and " + this.mySimManager.EndSimulatonDate.Year.ToString());
                     tempYear = InputBox.ShowInputBox("Please enter the " + i.ToString() + " year to swap out.","Enter 4 digit year");
                  }
                  swapYears[i] = tempYear;
               }
               success = true;
            }
            else
            {
               //only one map for the whole run
               swapYears = new string[1];
               swapYears[0] = this.mySimManager.StartSeasonDate.Year.ToString();
               success = true;
            }
            

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
           
#endif
            eLog.Debug(ex);
         }
         if (success)
            {
               this.txtNumDays.Enabled = true;
               this.txtNumDays.Focus();
            }
         
      }
      


      #endregion

      #region Seasons
      private void txtNumDays_LostFocus(object sender, EventArgs e)
      {
         bool validNumDays = true;
         try
         {
            int numSeasons = System.Convert.ToInt32(this.txtNumDays.Text);
            if(numSeasons > this.mySimManager.NumDaysSeason || numSeasons <= 0)
            {
               MessageBox.Show("You entered " + this.txtNumDays.Text + " seasons and the simulation is going to run for " + this.mySimManager.NumDaysSeason.ToString() + " days/year.");
               this.txtNumDays.Focus();
               this.txtNumDays.SelectAll();
               validNumDays = false;
            }
            else if (numSeasons > 1)
            {
            
               swapDays = new string[numSeasons];
               MessageBox.Show("Please enter a description of each season" + System.Environment.NewLine
                  + "(e.g. Spring,Summer,Leaf Off, etc.)");
               for (int i=0;i<numSeasons;i++)
               {
                   swapDays[i] = InputBox.ShowInputBox("Please enter a name for season " + (i + 1).ToString(),"Enter Season Name");
               }
            }
            else
            {
               //only one map for the whole run
               swapDays = new string[1];
               swapDays[0] = "default";
            }
            if(validNumDays)
            {
               this.txtNumHours.Enabled = true;
               this.txtNumHours.Focus();
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
      private void btnSeason_Click(object sender, System.EventArgs e)
      {
         //         int years = System.Convert.ToInt32(this.txtNumYears.Text);
         //         int seasons = System.Convert.ToInt32(this.txtNumDays.Text);
         //         int curYear = 0;
         //         int curSeason = 0;
         //         while (curYear++ < years)
         //         {
         //            curSeason = 0;
         //            while (curSeason++ < seasons)
         //            {
         //               frmSeason fs = new frmSeason(curYear, curSeason, ref mapSwapDatesList,ref this.mySimManager);
         //               fs.ShowDialog(this);
         //            }
         //         }
         //         dataGrid1.Refresh();
         //         currencyManager.Position = 0;
      }

    

      #endregion

      #region Hours
     
      private void txtNumHours_LostFocus(object sender, EventArgs e)
      {
         try
         {
            int numHours = System.Convert.ToInt32(this.txtNumHours.Text);
            if(numHours > 24 || numHours <= 0)
            {
               MessageBox.Show("You entered " + this.txtNumHours.Text + " hours and there are 24 hours in a day.");
               this.txtNumHours.Focus();
               this.txtNumHours.SelectAll();
            }
            else if (numHours > 1)
            {
            
               swapHours = new string[numHours];
               MessageBox.Show("Please enter a description for each part of a day" + System.Environment.NewLine
                  + "(e.g. early morning, daylight, night, sundown, etc.)");
               for (int i=0;i<numHours;i++)
               {
                  
                  swapHours[i] = InputBox.ShowInputBox("Please enter a name for time of day " + (i + 1).ToString(),"Enter Hour Group Name");
                  if (swapHours[i] == "-1")
                  {
                     MessageBox.Show("you need to enter something");
                     break;
                  }

               }
            }
            else
            {
               //evidently not going to swap out at the hour level
               swapHours = new string[1];
               swapHours[0] = "Default Hour";
            }
            
            this.btnChooseMaps.Enabled = true;
            this.btnChooseMaps.Focus();
            if(numHours==1 && swapYears.Length== 1 && swapDays.Length ==1)
               this.btnChooseMaps_Click(null,null);

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
        }

      }
     

    
      #endregion
   }
}

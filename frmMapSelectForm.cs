/*****************************************************************  
 * CHANGE LOG
 *  DATE          Sunday, September 09, 2007 11:57:39 AM
 *  Author:       Bob Cummings
 *  Description   Made change to set orginal time in MapSwapTrigger
 ******************************************************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using log4net;

namespace SEARCH
{
	/// <summary>
	/// Summary description for MapSelectForm.
	/// </summary>
	public class frmMapSelectForm : System.Windows.Forms.Form
	{
		#region Non-Public Members (24) 

		#region Fields (21) 

		private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.MonthCalendar calStartDate;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
      private System.Windows.Forms.DateTimePicker dtpStartTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label lblDay;
      private System.Windows.Forms.Label lblDiscription;
      private System.Windows.Forms.Label lblHour;
		private System.Windows.Forms.Label lblMapType;
		private System.Windows.Forms.Label lblYear;
      private string mapType;
      private bool mCancel;
      private MapSwapTrigger mMst;
      private MapManager myMapManager;
      private SimulatonManager mySimManager;
      private System.Windows.Forms.OpenFileDialog ofdCommon;

		#endregion Fields 
		#region Methods (3) 

      private ILog eLog = LogManager.GetLogger("Error");

		private void btnBrowse_Click(object sender, System.EventArgs e)
		{
			
			try
			{
				//fw.writeLine("inside frmInput.loadMap called from button " + mapType + " map click event");
            this.ofdCommon.Filter = "Shape Files *.shp|*.shp";
            this.ofdCommon.Title = "Browse for " + mapType + " MapManager";
				
				if (this.ofdCommon.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{  
               mMst = new MapSwapTrigger();
					mMst.Path = System.IO.Path.GetDirectoryName(this.ofdCommon.FileName);
					mMst.Filename = System.IO.Path.GetFileNameWithoutExtension(this.ofdCommon.FileName);
               mMst.StartDate = this.createDate();
               mMst.OriginalStartDate = mMst.StartDate;
               this.Hide();
				}
			}
			catch (InvalidMapException ime)
			{
				MessageBox.Show(ime.Message);
			}
			catch (System.Exception ex)
			{
				eLog.Debug(ex);
			}


		}

      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         mCancel = true;
         this.Hide();
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#endregion Methods 

		#endregion Non-Public Members 


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMapSelectForm));
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblHour = new System.Windows.Forms.Label();
            this.lblDay = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMapType = new System.Windows.Forms.Label();
            this.ofdCommon = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDiscription = new System.Windows.Forms.Label();
            this.calStartDate = new System.Windows.Forms.MonthCalendar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(328, 360);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(128, 25);
            this.btnBrowse.TabIndex = 26;
            this.btnBrowse.Text = "Browse for Map";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblHour
            // 
            this.lblHour.Location = new System.Drawing.Point(520, 72);
            this.lblHour.Name = "lblHour";
            this.lblHour.Size = new System.Drawing.Size(100, 16);
            this.lblHour.TabIndex = 25;
            this.lblHour.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblDay
            // 
            this.lblDay.Location = new System.Drawing.Point(312, 72);
            this.lblDay.Name = "lblDay";
            this.lblDay.Size = new System.Drawing.Size(100, 16);
            this.lblDay.TabIndex = 24;
            this.lblDay.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblYear
            // 
            this.lblYear.Location = new System.Drawing.Point(112, 72);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(100, 16);
            this.lblYear.TabIndex = 23;
            this.lblYear.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(432, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 22;
            this.label3.Text = "Part of Day";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(224, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 21;
            this.label2.Text = "Season";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 20;
            this.label1.Text = "Year";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // lblMapType
            // 
            this.lblMapType.AutoSize = true;
            this.lblMapType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapType.Location = new System.Drawing.Point(112, 136);
            this.lblMapType.Name = "lblMapType";
            this.lblMapType.Size = new System.Drawing.Size(0, 16);
            this.lblMapType.TabIndex = 16;
            this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(48, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 24);
            this.label4.TabIndex = 29;
            this.label4.Text = "Choose a Starting Date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpStartTime.Location = new System.Drawing.Point(48, 360);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(128, 20);
            this.dtpStartTime.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(40, 336);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(280, 24);
            this.label5.TabIndex = 31;
            this.label5.Text = "Choose a Starting Hour (We ignore the minutes)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblDiscription
            // 
            this.lblDiscription.Location = new System.Drawing.Point(40, 16);
            this.lblDiscription.Name = "lblDiscription";
            this.lblDiscription.Size = new System.Drawing.Size(712, 40);
            this.lblDiscription.TabIndex = 32;
            // 
            // calStartDate
            // 
            this.calStartDate.Location = new System.Drawing.Point(40, 168);
            this.calStartDate.MaxSelectionCount = 1;
            this.calStartDate.Name = "calStartDate";
            this.calStartDate.TabIndex = 33;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(565, 356);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 32);
            this.btnCancel.TabIndex = 34;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmMapSelectForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(784, 405);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.calStartDate);
            this.Controls.Add(this.lblDiscription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblMapType);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblHour);
            this.Controls.Add(this.lblDay);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMapSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

      #region constructors
      public frmMapSelectForm()
      {
         InitializeComponent();
         fillDescriptionLabel();
         mMst = new MapSwapTrigger();
         mCancel = false;
      }

      public frmMapSelectForm(string mapType, string mapPath, ref SimulatonManager inSimManager) : this()
      {
         this.ofdCommon.Title = "Browse for " + mapType + " MapManager";
         this.ofdCommon.InitialDirectory = mapPath;
         mySimManager = inSimManager;
      }
      public frmMapSelectForm(ref SimulatonManager inSimManager, ref MapSwapTrigger mst, String mMapType, string mapPath) : this()
      {
         lblMapType.Text = mMapType;
         mapType = mMapType;
         mMst = mst;
         mySimManager = inSimManager;
         myMapManager = MapManager.GetUniqueInstance();
         this.ofdCommon.InitialDirectory = mapPath;
      }
      public frmMapSelectForm(string mapType, string mapPath) : this()
      {
         this.ofdCommon.Title = "Browse for " + mapType + " MapManager";
         this.ofdCommon.InitialDirectory = mapPath;
      }
      #endregion

      #region public methods
      public void fillDisplay(string year, string day, string hour)
      {
         if (this.lblYear.Text.Length > 0 && this.lblYear.Text != year)
         {
            resetCalYear(year);
         }
         this.lblYear.Text = year;
         this.lblDay.Text = day;
         this.lblHour.Text = hour;
         mCancel = false;
      }
      public void setCalRange(DateTime startDate, DateTime endDate)
      {
         try
         {
            this.calStartDate.MinDate = startDate;
            this.calStartDate.MaxDate = endDate;
            this.calStartDate.MaxSelectionCount = 1;
            //this should stop the maps from being loaded to soon
            this.dtpStartTime.Value.AddHours(this.dtpStartTime.Value.Hour * -1);
            this.dtpStartTime.Value.AddHours(startDate.Hour);

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      public void resetCalRange(DateTime newStartDate)
      {
         try
         {
            this.calStartDate.MinDate = newStartDate;

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      #endregion

      #region privateMethods
      private void fillDescriptionLabel()
      {
         this.lblDiscription.Text = "You use this form to choose the maps for each year,day,hour combination " + 
            "you made.  The name of the time period is just below.  Please select the start time for this " + 
            "time period and then choose the map you want to load at the point in the simulation.";
      }

      private DateTime createDate()
      {
         DateTime d = new DateTime();
         int numYears = 0;
         
         try
         {
            d = this.calStartDate.SelectionStart.AddHours(this.calStartDate.SelectionStart.Hour * -1);
            numYears = System.Convert.ToInt32(this.lblYear.Text) - this.calStartDate.SelectionStart.Year;
            d = d.AddYears(numYears);
            d = d.AddHours(this.dtpStartTime.Value.Hour);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return d;
      }
      private void resetCalYear(string newYear)
      {
         
         try
         {
            int oYear = this.mySimManager.StartSimulationDate.Year;
            int nYear = System.Convert.ToInt32(newYear);
            int diff = nYear - oYear;
            this.calStartDate.MinDate = this.mySimManager.StartSimulationDate.AddYears(diff);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      #endregion

      #region getters and setters
      public MapSwapTrigger Mst
		{
			get { return mMst; }
			set { mMst = value; }
		}

      public bool Cancel
		{
			get { return mCancel; }
			set { mCancel = value; }
		}
      #endregion
	}
}

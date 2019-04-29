using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using SEARCH;
using log4net;

namespace SEARCH
{
   /// <summary>
   /// Summary description for frmMapTimes.
   /// </summary>
   public class frmMapTimes : System.Windows.Forms.Form
   {
		#region Public Members (3) 

		#region Constructors (2) 

       private ILog eLog = LogManager.GetLogger("Error");

      public frmMapTimes(ref SimulatonManager inSimManager, ref MapSwapTrigger tmpMst, String mMapType)
         : this(ref inSimManager)
      {
         myMST = tmpMst;
         lblTimeInstr.Text = "Set time for " + mMapType + " map.";
         lblYear.Text = myMST.YearGrp.ToString();
         lblSeason.Text = myMST.SeasonGrp.ToString();
         lblDay.Text = myMST.DayGrp.ToString();


      }

      public frmMapTimes(ref SimulatonManager sm)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         this.StartPosition = FormStartPosition.CenterParent;
         //         this.dateTimePicker1.Visible = false;

         this.mySimManager = sm;
      }

		#endregion Constructors 
		#region Properties (1) 

      public string[] OutFileNamesAndStartTimes
      {
         get { return mOutFileNamesAndStartTimes; }
         set { mOutFileNamesAndStartTimes = value; }
      }

		#endregion Properties 

		#endregion Public Members 

		#region Non-Public Members (15) 

		#region Fields (14) 

      private System.Windows.Forms.Button btnDone;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;
      private System.Windows.Forms.DateTimePicker dateTimePicker1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label lblDay;
      private System.Windows.Forms.Label lblMapType;
      private System.Windows.Forms.Label lblSeason;
      private System.Windows.Forms.Label lblTimeInstr;
      private System.Windows.Forms.Label lblYear;
      private string[] mOutFileNamesAndStartTimes;
      private MapSwapTrigger myMST;
      private SimulatonManager mySimManager;

		#endregion Fields 
		#region Methods (1) 

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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMapTimes));
         this.lblMapType = new System.Windows.Forms.Label();
         this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
         this.btnDone = new System.Windows.Forms.Button();
         this.lblTimeInstr = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.lblYear = new System.Windows.Forms.Label();
         this.lblSeason = new System.Windows.Forms.Label();
         this.lblDay = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // lblMapType
         // 
         this.lblMapType.AutoSize = true;
         this.lblMapType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
         this.lblMapType.Location = new System.Drawing.Point(16, 24);
         this.lblMapType.Name = "lblMapType";
         this.lblMapType.Size = new System.Drawing.Size(0, 18);
         this.lblMapType.TabIndex = 0;
         this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // dateTimePicker1
         // 
         this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
         this.dateTimePicker1.Location = new System.Drawing.Point(8, 152);
         this.dateTimePicker1.Name = "dateTimePicker1";
         this.dateTimePicker1.ShowUpDown = true;
         this.dateTimePicker1.Size = new System.Drawing.Size(112, 20);
         this.dateTimePicker1.TabIndex = 3;
         // 
         // btnDone
         // 
         this.btnDone.Location = new System.Drawing.Point(8, 184);
         this.btnDone.Name = "btnDone";
         this.btnDone.Size = new System.Drawing.Size(128, 24);
         this.btnDone.TabIndex = 5;
         this.btnDone.Text = "Done";
         this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
         // 
         // lblTimeInstr
         // 
         this.lblTimeInstr.Location = new System.Drawing.Point(8, 120);
         this.lblTimeInstr.Name = "lblTimeInstr";
         this.lblTimeInstr.Size = new System.Drawing.Size(136, 23);
         this.lblTimeInstr.TabIndex = 6;
         this.lblTimeInstr.Text = "Set Start Time for map";
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(16, 8);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(72, 16);
         this.label1.TabIndex = 8;
         this.label1.Text = "Year Group";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(32, 32);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(56, 16);
         this.label2.TabIndex = 9;
         this.label2.Text = "Season";
         this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(24, 56);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(64, 16);
         this.label3.TabIndex = 10;
         this.label3.Text = "Part of Day";
         this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // lblYear
         // 
         this.lblYear.Location = new System.Drawing.Point(104, 8);
         this.lblYear.Name = "lblYear";
         this.lblYear.Size = new System.Drawing.Size(100, 16);
         this.lblYear.TabIndex = 11;
         this.lblYear.Text = "label4";
         // 
         // lblSeason
         // 
         this.lblSeason.Location = new System.Drawing.Point(104, 32);
         this.lblSeason.Name = "lblSeason";
         this.lblSeason.Size = new System.Drawing.Size(100, 16);
         this.lblSeason.TabIndex = 12;
         this.lblSeason.Text = "label4";
         // 
         // lblDay
         // 
         this.lblDay.Location = new System.Drawing.Point(104, 56);
         this.lblDay.Name = "lblDay";
         this.lblDay.Size = new System.Drawing.Size(100, 16);
         this.lblDay.TabIndex = 13;
         this.lblDay.Text = "label4";
         // 
         // frmMapTimes
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(224, 229);
         this.Controls.Add(this.lblDay);
         this.Controls.Add(this.lblSeason);
         this.Controls.Add(this.lblYear);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.lblTimeInstr);
         this.Controls.Add(this.btnDone);
         this.Controls.Add(this.dateTimePicker1);
         this.Controls.Add(this.lblMapType);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmMapTimes";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Set Map Start Times";
         this.ResumeLayout(false);

      }
      #endregion

      #region publicMethods
      public void setLable(string text)
      {
         this.lblMapType.Text = "Setting start times for " + text + " maps";
      }

      #endregion

      #region privateMethods
      private void btnDone_Click(object sender, System.EventArgs e)
      {
         myMST.StartHour = dateTimePicker1.Value.Hour.ToString();
         myMST.StartMinute = dateTimePicker1.Value.Minute.ToString();
         myMST.StartDate.AddHours(dateTimePicker1.Value.Hour);
         myMST.StartDate.AddMinutes(dateTimePicker1.Value.Minute);
         this.Close();

      }
      private void setDTPicker()
      {
      }

      private bool validSettings()
      {
         bool success = true;
         try
         {
         }
         catch (System.Exception ex)
         {
            success = false;
            eLog.Debug(ex);
         }
         return success;
      }



      #endregion
   }
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SEARCH;
using log4net;

namespace SEARCH.Forms
{
	/// <summary>
	/// Summary description for frmSeason.
	/// </summary>
	public class frmSeason : System.Windows.Forms.Form
	{
		#region Public Members (2) 

		#region Constructors (2) 

        private ILog eLog = LogManager.GetLogger("Error");

		public frmSeason(int year, int season, ref ArrayList al, ref SimulatonManager sm,string swapYear):this()
		{
         try
         {
            this.mySimManager = sm;
            this.lblYear.Text = year.ToString() + " (" + swapYear + ")";
            this.lblSeason.Text = season.ToString();
            mMapTimeList = al;
            if (mMapTimeList.Count > 0)
            {
               mst = (MapSwapTrigger)mMapTimeList[mMapTimeList.Count -1];
               //if season is > 1 then we are in the same year.
               if(season>1)
               {
                  this.monthCalendar1.MinDate = mst.StartDate.AddDays(1);
               }
               else
               {
                  // the start date would be 
                  this.monthCalendar1.MinDate = sm.StartSeasonDate.AddYears(System.Convert.ToInt32(swapYear) - sm.StartSeasonDate.Year);
               }
            }
            else
            {
               //first map swap so it has to start at the begining of the simulation.
               mst = new MapSwapTrigger();
               mst.StartDate = this.mySimManager.StartSeasonDate;
               mst.SeasonGrp = season;
               mst.YearGrp = year;
               
            }
            mst.SeasonGrp = season;
            mst.YearGrp = year;
            this.monthCalendar1.MaxDate =  this.mySimManager.EndSeasonDate;

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
			

		}

		public frmSeason()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (14) 

		#region Fields (12) 

		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblMapType;
		private System.Windows.Forms.Label lblSeason;
		private System.Windows.Forms.Label lblYear;
		private ArrayList mMapTimeList;
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		private MapSwapTrigger mst;
		private SimulatonManager mySimManager;

		#endregion Fields 
		#region Methods (2) 

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			mst.StartDate = monthCalendar1.SelectionStart;
			this.mMapTimeList.Add(mst);
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


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSeason));
			this.lblSeason = new System.Windows.Forms.Label();
			this.lblYear = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblMapType = new System.Windows.Forms.Label();
			this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblSeason
			// 
			this.lblSeason.Location = new System.Drawing.Point(128, 40);
			this.lblSeason.Name = "lblSeason";
			this.lblSeason.Size = new System.Drawing.Size(104, 20);
			this.lblSeason.TabIndex = 17;
			// 
			// lblYear
			// 
			this.lblYear.Location = new System.Drawing.Point(128, 16);
			this.lblYear.Name = "lblYear";
			this.lblYear.Size = new System.Drawing.Size(104, 20);
			this.lblYear.TabIndex = 16;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 20);
			this.label2.TabIndex = 15;
			this.label2.Text = "Season";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 20);
			this.label1.TabIndex = 14;
			this.label1.Text = "Year Group";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblMapType
			// 
			this.lblMapType.AutoSize = true;
			this.lblMapType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblMapType.Location = new System.Drawing.Point(40, 32);
			this.lblMapType.Name = "lblMapType";
			this.lblMapType.Size = new System.Drawing.Size(0, 18);
			this.lblMapType.TabIndex = 13;
			this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// monthCalendar1
			// 
			this.monthCalendar1.Location = new System.Drawing.Point(48, 104);
			this.monthCalendar1.MaxSelectionCount = 1;
			this.monthCalendar1.Name = "monthCalendar1";
			this.monthCalendar1.ShowToday = false;
			this.monthCalendar1.TabIndex = 18;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(200, 23);
			this.label3.TabIndex = 19;
			this.label3.Text = "Select start date for season";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(104, 272);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 20;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// frmSeason
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 309);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.monthCalendar1);
			this.Controls.Add(this.lblSeason);
			this.Controls.Add(this.lblYear);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblMapType);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSeason";
			this.Text = "Set the season date";
			this.ResumeLayout(false);

		}
		#endregion
	}
}

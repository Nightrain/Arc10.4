using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

using SEARCH;

namespace DataCentric
{
    static class Program
    {
		#region Non-Public Members (1) 

		#region Methods (1) 

        static void usage()
        {
            Console.Error.WriteLine("program xmlInputFile mapOutputDirectory textOutputDirectory xmlBackup");
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run( new frm Input());
            //Console.WriteLine("I'm in is licensed");
            if (ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop))
            {
                //Console.WriteLine("Inside binding");
                ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
                //Console.WriteLine("Bound and checked out license");
            }

            if (args.Length <4 )
            {
                usage();
                Environment.Exit(-1);
            }


            frmInput f = new frmInput();


            string xmlInputFile = args[0];
            string mapOutputDirectory = args[1];
            string textOutputDir = args[2];
            string xmlBackup = args[3];

            f.doRun(xmlInputFile, mapOutputDirectory, textOutputDir,xmlBackup);

            //Console.WriteLine("Finished doRun");
            
            //Tried disposing form
            //f.Dispose();
            //if (f.IsDisposed)
            //{
            //    Console.WriteLine("Disposed form!");
            //}

            
            //Tried multiple types of exits
            //Do we need to force an exit here?
            //Environment.FailFast(null);
            //Console.WriteLine("Trying return");
            //return;
            //Console.WriteLine("Trying Application.Exit()");
            //Application.Exit();
            //Console.WriteLine("Trying Environment.Exit(0)");
            //Environment.Exit(0);


            //Tried garbage collection
            //GC.Collect();
            //Console.WriteLine("Waiting for finalizers");
            //GC.WaitForPendingFinalizers();

            //Tried sleeping
            //System.Threading.Thread.Sleep(5000);

            //Process thisProc = Process.GetCurrentProcess();
            //ProcessThreadCollection myThreads = thisProc.Threads;
            //foreach (ProcessThread pt in myThreads)
            //{
            //    DateTime startTime = pt.StartTime;
            //    TimeSpan cpuTime = pt.TotalProcessorTime;
                //int priority = pt.BasePriority;
            //    ThreadState ts = pt.ThreadState;

            //    Console.WriteLine("thread:  {0}", pt.Id);
            //    Console.WriteLine("    started: {0}", startTime.ToString());
            //    Console.WriteLine("    CPU time: {0}", cpuTime);
            //    Console.WriteLine("    priority: {0}", priority);
            //    Console.WriteLine("    thread state: {0}", ts.ToString());

            //    if (priority == 10)
            //    {
            //        Console.WriteLine("Disposing Thread!!!!");
            //        pt.Dispose();
            //    }
            //}

            Application.Exit();
            Environment.Exit(0);

            //thisProc.Kill();
 
            

            //Console.WriteLine("End of program");

        }
		#endregion Methods 

		#endregion Non-Public Members 
    }
}
/******************************************************************************
 * Author:        Bob Cummings
 * Name:          AnimalManager
 * Description:   Manages the animals as they work through the simulation. Everything
 *                from taking a step, to breeding, just about all of it.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Modify Date    Sunday, November 08, 2009
 * Description    2 years into changing thought time to start.  For issue
 *                59 the issue was during the current time step the current
 *                social map reference was not being updated when a new 
 *                resident was added.  Therefore when trying to remove a resident
 *                in the same time step there was not a correct refernce to 
 *                social map.
 * ***************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics;
using System.IO;
using DesignByContract;
using ESRI.ArcGIS.Geometry;
using log4net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace SEARCH
{
    [Serializable()]
    public class AnimalManager  
   {
		#region Constructors (1) 

      public AnimalManager()
      {
         mLog.Debug("getting the modifiers");
         mMaleModifier = MaleModifier.GetUniqueInstance();
         mFemaleModifier = FemaleModifier.GetUniqueInstance();
         mAnimalAttributes = new AnimalAtributes();
         mRiskySearchMod = new RiskySearchModifier();
         mRiskyForageMod = new RiskyForageModifier();
         mSafeForageMod = new SafeForageModifier();
         mSafeSearchMod = new SafeSearchModifier();
         mLog.Debug("now making the mover object");
         mMover = RandomWCMover.getRandomWCMover();
         this.myAnimals = new List<Animal>();
         currNumAnimals = 0;
   

      }

		#endregion Constructors 

		#region Fields (17) 

      private IEnumerator currAnimal;
      private AnimalAtributes mAnimalAttributes;
      private HomeRangeCriteria mFemaleHomeRangeCriteria;
      private FemaleModifier mFemaleModifier;
      private IHomeRangeFinder mHomeRangeFinder;
      private IHomeRangeTrigger mHomeRangeTrigger;
      private HomeRangeCriteria mMaleHomeRangeCriteria;
      private MaleModifier mMaleModifier;
      private Dictionary<IPoint, MapValue> mMapValues; // ersi may need custom serializer. mark as transient.

      private Mover mMover; // containes ref to map manager // mark as transient.
      
        private ResidentAttributes mResidentAttributes;
      private Modifier mRiskyForageMod;
      private Modifier mRiskySearchMod;
      private Modifier mSafeForageMod;
      private Modifier mSafeSearchMod;
      private string myErrMessage;
      private List<Animal> myAnimals;
      private int currNumAnimals;
      

      public int getNumDispersers()
      {
         return getDispersers().Count;
      }

      public int getNumResidents()
      {
         return getResidents().Count;
      }

		#endregion Fields 

		#region Properties (9) 

      public AnimalAtributes AnimalAttributes
      {
         get { return mAnimalAttributes; }
         set { mAnimalAttributes = value; }
      }

      public string ErrMessage
      {
         get { return myErrMessage; }
                                 
         set { myErrMessage = value; }
      }

      public FemaleModifier FemaleModifier
      {
         get { return mFemaleModifier; }
         set 
         {
            mFemaleModifier = value;
            mLog.Debug("inside animal manager setting the mFemaleModifierMod"); }
      }

      public MaleModifier MaleModifier
      {
         get { return mMaleModifier; }
         set 
         {
            mMaleModifier = value;
            mLog.Debug("inside animal manager setting the mMaleModifierMod"); }
      }

      public ResidentAttributes ResidentAttributes
      {
         get { return mResidentAttributes; }
         set { mResidentAttributes = value; }
      }

      public Modifier RiskyForageMod
      {
         get { return mRiskyForageMod; }
         set 
         {
            mRiskyForageMod = value;
            mLog.Debug("inside animal manager setting the mRiskyForageMod"); }
      }

      public Modifier RiskySearchMod
      {
         get { return mRiskySearchMod; }
         set { mRiskySearchMod = value; }
      }

      public Modifier SafeForageMod
      {
         get { return mSafeForageMod; }
         set 
         {
            mSafeForageMod = value;
            mLog.Debug("inside animal manager setting the mSafeForageMod"); }

      }

      public Modifier SafeSearchMod
      {
         get { return mSafeSearchMod; }
         set 
         {
            mSafeSearchMod = value;
            mLog.Debug("inside animal manager setting the mSafeSearchMod"); }
      }

		#endregion Properties 

		#region Methods (32) 

		#region Public Methods (20) 

      private ILog mLog = LogManager.GetLogger("animalManager");
      private ILog eLog = LogManager.GetLogger("Error");

      public void addNewDispersers(InitialAnimalAttributes[] inIAA, DateTime currTime)
      {
         try
         {
            mLog.Debug("inside addNewDispersers starting loop");
            for (int i = 0; i <= inIAA.Length - 1; i++)
            {
               mLog.Debug("calling makeNextGenAnimal");
               this.makeNextGenAnimal(inIAA[i],currTime);
            }
            mLog.Debug("done with loop in addNewDispersers calling mHomeRangeTrigger.reset");
            this.mHomeRangeTrigger.reset(this.myAnimals.Count + 1);
            mLog.Debug("back in addNewDispersers calling setNextGenHomeRange");
            setNextGenHomeRange();
            mLog.Debug("done with adding new dispersers leaving addNewDispersers");

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void adjustNewSocialMap(Map newSocialMap)
      {
         this.resetResidentsHomeRange(newSocialMap);
         this.resetDisperserSocialIndex(newSocialMap);
         
      }

      public int breedFemales(DateTime currTime)
      {
         mLog.Debug("inside breed females");
         int numMales=0;
         int numFemales=0;
         int totalNewAnimals = 0;
         InitialAnimalAttributes iaa;
         mLog.Debug("starting the loop");
         List<Resident> myBreeders = this.getResidents("female");

         foreach (Resident r in myBreeders)
         {

            r.breed(out numMales, out numFemales);
            totalNewAnimals += numFemales + numMales;
            if (numMales > 0)
            {
               iaa = new InitialAnimalAttributes();
               iaa.Location = r.HomeRangeCenter;
               iaa.Sex = 'M';
               iaa.NumToMake = numMales;
               this.makeNextGenAnimal(iaa, currTime);
            }
            if (numFemales > 0)
            {
               iaa = new InitialAnimalAttributes();
               iaa.Location = r.HomeRangeCenter;
               iaa.Sex = 'F';
               iaa.NumToMake = numFemales;
               this.makeNextGenAnimal(iaa, currTime);
            }
         }
         

         this.mHomeRangeTrigger.reset(this.myAnimals.Count + 1);
         setNextGenHomeRange();
         return totalNewAnimals;

      }

      public void changeToDeadAnimal(Animal inA)
      {
         try
         {
            mLog.Debug("inside changeToDeadAnimal for animal num " + inA.IdNum);
            DeadAnimal dd = new DeadAnimal(inA);
            mLog.Debug("removing at position " + dd.IdNum.ToString());
            this.myAnimals.RemoveAt(dd.IdNum);
            this.myAnimals.Insert(dd.IdNum, dd);
                  
            mLog.Debug("new animal type is " + dd.GetType());
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
             System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool DoTextOutPut, Map currSocialMap)
      {
         
         //List<Animal> liveAnimals = this.getAllLiveAnimals();
         Animal a;
         string status = "";
         try
         {
            mLog.Debug("inside animal manager do time step with modifiers");
           // foreach (Animal a in myAnimals)
            for(int i=0;i<myAnimals.Count; i++)
            {
               a = this.myAnimals[i];
               mLog.Debug("");
               mLog.Debug("animal id = " + a.IdNum.ToString());
               mLog.Debug("animal is type " + a.GetType().ToString());

//               if (a.GetType().ToString().Equals("SEARCH.Resident", StringComparison.CurrentCultureIgnoreCase))
//               {
//                  int bob = 0;
//               }

                a.doTimeStep(inHM, inDM, currTime, DoTextOutPut, ref status);
               //check to see if they died if they did remove them from the list
               if (a.IsDead)
               {
                   if (a.GetType().ToString().Equals("SEARCH.Resident", StringComparison.CurrentCultureIgnoreCase))
                   { this.AdjustMapForDeadAnimal(currSocialMap, a); }
                 
                 this.changeToDeadAnimal(a);
               }
               //check to see if they are changing from disperser to resident
               
               if (status == "resident")
               {
                  mLog.Debug("switching " + a.IdNum.ToString() + " to a resident");
                  Resident r = new Resident();
                  r.Sex = a.Sex;
                  r.IdNum = a.IdNum;
                  r.TextFileWriter = a.TextFileWriter;
                  r.HomeRangeCenter = a.HomeRangeCenter;
                  r.HomeRangeCriteria = a.HomeRangeCriteria;
                  r.MyAttributes = this.ResidentAttributes;
                  r.IdNumOrig = a.IdNum.ToString();
                  this.myAnimals.RemoveAt(i);
                  this.myAnimals.Insert(i, r);
                  status = "";
                  //Sunday, November 08, 2009
                  //update reference to current social map.
                  currSocialMap = MapManager.GetUniqueInstance().SocialMap;
               }
              
            }
            }
         
        
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
        
      }

      public void dump()
      {
         foreach (Animal a in myAnimals)
         {
            a.dump();
         }
      }

     

      

      public StringCollection getResidentIDs()
      {  
         System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
         try
         {
            foreach (Animal a in this.myAnimals)
            {
               if (a.GetType().Name == "Resident")
               {
                  sc.Add(a.IdNum.ToString());
               }
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return sc;
      }

      public bool makeInitialAnimals(InitialAnimalAttributes[] inIAA)
      {
         bool success = true;
         
         Animal tmpAnimal;
               
         //fw.writeLine("inside make initial animals");
         try
         {
            //this is an array of attributes one of which is how many to make of this type
            for (int i = 0; i <= inIAA.Length - 1; i++)
            {
               //so for each entity in the array we need to make j amount of animals
               for (int j = 0; j < inIAA[i].NumToMake; j++)
               {
                  if (inIAA[i].Sex == 'M')
                  {
                     //fw.writeLine("makeing a new male");
                     tmpAnimal = new Male();
                  }
                  else
                  {
                     mLog.Debug("makeing a new female");
                     tmpAnimal = new Female();
                  }

                  tmpAnimal.IdNum = this.currNumAnimals++;
                  mLog.Debug("just made " + tmpAnimal.IdNum.ToString());
                  tmpAnimal.Location = inIAA[i].Location;
                  
                  tmpAnimal.AnimalAtributes = this.AnimalAttributes;
                  mLog.Debug("now setting the mover");
                  tmpAnimal.myMover = this.mMover;
                  tmpAnimal.AnimalManager = this;
                  mLog.Debug("now adding to the list of my animals;");
                  this.myAnimals.Add(tmpAnimal);
               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
            mLog.Debug("");
            success = false;
         }
         return success;
      }

      public bool makeResidents(InitialAnimalAttributes[]inResAttributes,string currYear)
      {
         bool success = false;
         Resident r;
         mLog.Debug("inside makeResidents going to make " + inResAttributes.Length.ToString());
         mLog.Debug("total animals now is " + this.myAnimals.Count.ToString());
         try
         {
            for (int i = 0; i < inResAttributes.Length; i++)
            {
               r = new Resident();
               r.MyAttributes = new ResidentAttributes();
               if (inResAttributes[i].Sex.ToString().ToUpper() == "M")
               {
                  r.Sex = "Male";
                 
               }
               else
               {
                  r.Sex = "Female";
                 
               }
               //r.IsDead = false;
               r.IdNum = this.currNumAnimals++;
               r.Location = inResAttributes[i].Location;
               r.HomeRangeCenter = r.Location as PointClass; ;
               r.IdNumOrig = inResAttributes[i].OrginalID;
                
               this.myAnimals.Add(r);
            }
            
            mLog.Debug("after adding residents total animals now is " + this.myAnimals.Count.ToString());
            success = true;

         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         
         return success;

      }

      public void removeRemaingDispersers()
      {
         try
         {
            List<Animal> poorSuckers = this.getDispersers();
            mLog.Debug("inside remove remaining dispersers");
            mLog.Debug("going to loop through " + this.myAnimals.Count.ToString() + " in the collection.");
            foreach (Animal a in poorSuckers)
            {
               {
                  mLog.Debug("so we are setting isDead to true");
                  if (a.TextFileWriter != null)
                     a.TextFileWriter.addLine("Did not establish a home range so died during winter kill");
                  a.IsDead = true;
               }
            }
            mLog.Debug("there are " + this.getNumDispersers().ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         
      }

      public bool setAttributes()
      {
         bool success = true;
         try
         {
            List<Animal> a = this.getDispersers();
            foreach (Animal temp in a)
            {
               temp.AnimalAtributes = this.AnimalAttributes;
               temp.CurrEnergy = this.AnimalAttributes.InitialEnergy;
            }
         }
         catch (System.Exception ex)
         {
            success = false;
            eLog.Debug(ex);
         }
         return success;
      }

      /********************************************************************************
       *   Function name   : setHomeRange
       *   Description     : 
       *   Return type     : void 
       *   Argument        : string sex
       *   Argument        : double area
       *   Argument        : double distanceMean
       *   Argument        : double distanceSD
       * ********************************************************************************/
      public void setHomeRange(string sex, double area, double distanceMean, double distanceSD, double distanceWeight)
      {
         if (sex.ToUpper() == "MALE")
         {
            this.mMaleHomeRangeCriteria = new HomeRangeCriteria(area, distanceMean, distanceSD, distanceWeight);
            setMaleHomeRange();
         }
         else
         {
            this.mFemaleHomeRangeCriteria = new HomeRangeCriteria(area, distanceMean, distanceSD, distanceWeight);
            setFemaleHomeRange();
         }
      }

      public bool setHomeRangeCriteria(string type)
      {
         bool success = false;
       
         try
         {
            mLog.Debug("inside AnimalManger setHomeRangeCriteria " + "for " + type);
            switch (type)
            {
               case("Food") : 
                  mLog.Debug("making a foodie");
                  mHomeRangeFinder = BestFoodHomeRangeFinder.getInstance();
                  break;
               case("Risk") : 
                  mLog.Debug("making a risky");
                  mHomeRangeFinder = BestRiskHomeRangeFinder.getInstance();
                  break;
               case("Closest") : 
                  mLog.Debug("making a closest");
                  mHomeRangeFinder = ClosestHomeRangeFinder.getInstance();
                  break;
               case("Combo") : 
                  mLog.Debug("making a combo");
                  mHomeRangeFinder = BestComboHomeRangeFinder.getInstance();
                  break;
               default : 
                  throw new ArgumentException("Unexpected Home Range Criteria sent in type = " + type);
            }
            mLog.Debug("now looping and setting the animals");
            foreach (Animal a in this.myAnimals)
            {
               
               a.HomeRangeFinder = mHomeRangeFinder;
            }
            //if we get this far everything worked out fine
            success = true;

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return success;
      }

      public void setHomeRangeTrigger(string type, int num)
      {
         try
         {
            
            
            mLog.Debug("inside setHomeRangeTrigger want to create " + type + " for " + num.ToString());
            switch (type)
            {
               case "SITES" : 
                  mHomeRangeTrigger = new SiteHomeRangeTrigger(num, this.myAnimals.Count);
                  mLog.Debug("making a new site home ranger");
                  break;
               case "STEPS" :
                  mHomeRangeTrigger = new TimeHomeRangeTrigger(num, this.myAnimals.Count);
                  mLog.Debug("making a new step home ranger");
                  break;
               default : 
                  throw new System.Exception("Not a valid home range trigger");
            }
            //now set the trigger to the animals
            mLog.Debug("now set the triggers to the animals");
            
            foreach(Animal a in myAnimals)
            {
               a.HomeRangeTrigger = this.mHomeRangeTrigger;
            }
            mLog.Debug("done leaving setHomeRangeTrigger");
         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public bool setModifiers()
      {
         bool success = false;
         try
         {
            mLog.Debug("inside set gender modifier in Animal Modifier");
            foreach (Animal a in myAnimals)
            {
               if (a.GetType().Name == "Male")
                  a.GenderModifier = this.MaleModifier;
               else if (a.GetType().Name == "Female")
                  a.GenderModifier = this.FemaleModifier;

               a.StateModifer = this.SafeSearchMod;
            }
            mLog.Debug("done setting all the modifiers");
            success = true;
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return success;
      }

      public void setResidentModifierValues(double inTimeStepRisk, double inYearlyRisk, 
         double inPercentBreed, double inPercentFemale, double inMeanLitterSize, double inSDLitterSize)
      {
         try
         {
            mLog.Debug("inside animal Manager setResidentModifierValues inSDLitterSize is" + inSDLitterSize.ToString());
            Check.Require(inTimeStepRisk >= 0, "Resident Time Step Risk less then zero");
            Check.Require(inYearlyRisk >= 0, "Resident Yearly Risk less then zero");
            Check.Require(inPercentBreed >= 0, "Resident chance of breeding less then zero");
            Check.Require(inPercentFemale >= 0, "Resident chance of having female offspring less then zero");
            Check.Require(inMeanLitterSize >= 0, "Resident mean litter size less then zero");
            Check.Require(inSDLitterSize >= 0, "Resident sd litter size less then zero");
            mResidentAttributes = new ResidentAttributes(inTimeStepRisk, inYearlyRisk, inPercentBreed, inPercentFemale, inMeanLitterSize, inSDLitterSize);
            this.setResidentAttributes();

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public bool setSleepTime(DateTime currTime)
      {
         string CurrYear = currTime.Year.ToString();
         bool success = true;
         try
         {
            List<Animal> myDispersers = this.getDispersers();
            foreach (Animal a in myDispersers)
            {
               a.setInitialSleepTime(currTime);
               a.BuildTextWriter(CurrYear, this.AnimalAttributes.OutPutDir);
               SetMapValues(a, currTime);
            }
            
         }
         catch (System.Exception ex)
         {
            success = false;
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
            Process.GetCurrentProcess().Kill();
         }
         mLog.Debug("leaving animal manager set sleep time with a value of " + success.ToString());
         return success;

      }

      public void winterKillResidents(Map currSocialMap, int currentTime)
      {
         try
         {
            
            mLog.Debug("inside winterKillResidents");
            mLog.Debug("going to loop through " + this.myAnimals.Count.ToString() + " in the collection.");

            List<Resident> res = this.getResidents();
            foreach (Resident r in res)
            {
               mLog.Debug("so we are going to call the resident winter kill method");
               r.winterKill(currentTime);
               mLog.Debug("after calling resident winter kill the animal is dead = " + r.IsDead.ToString());
               if (r.IsDead)
               {
                  mLog.Debug("Well he died a glorious death but now he is just a dead animal ");
                  this.changeToDeadAnimal(r);
                  this.AdjustMapForDeadAnimal(currSocialMap, r);

               } 
            }
            
            mLog.Debug("there are " + this.myAnimals.Count.ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

		#endregion Public Methods 
		#region Private Methods (12) 

      private void AdjustMapForDeadAnimal(Map inSocialMap, Animal a)
      {

         string fieldName;
         try
         {
            if (a.Sex.ToLower() == "male")
               fieldName = "OCCUP_MALE";
            else
               fieldName = "OCCUP_FEMA";
            mLog.Debug("calling resetFields"); 
            inSocialMap.resetFields(fieldName, a.IdNumOrig, "none");
             //Changed above so that we use the original id for removing animals. For non-initial residents this
             //is equal to their id number
         }
         catch (Exception ex)
         {

            eLog.Debug(ex);
            eLog.Debug(inSocialMap.mySelf.AliasName);
            eLog.Debug(a.IdNum.ToString());

         }
      }


      private List<Animal> getAllLiveAnimals()
      {
         var temp = from a in myAnimals where a.IsDead == false select a;
         return temp.ToList<Animal>();
      }
 
      private List<Animal> getDispersers()
      {
         List<Animal> dispersers = new List<Animal>();
         foreach (Animal a in this.myAnimals)
         {
            if (a.GetType().Name != "Resident" && a.IsDead == false)
            {
               dispersers.Add(a);
            }
         }
         return dispersers;

      }
       private List<Animal> getDispersers(string inSex)
      {
         var temp = from a in myAnimals where a.GetType().Name.Equals(inSex, StringComparison.CurrentCultureIgnoreCase)  && !a.IsDead select a;
         return temp.ToList<Animal>();
      }
      private List<Resident> getResidents(string inSex)
      {

         var temp = from a in myAnimals where a.GetType().Name.Equals("resident", StringComparison.CurrentCultureIgnoreCase) && !a.IsDead && a.Sex.Equals(inSex, StringComparison.CurrentCultureIgnoreCase) select a as Resident;
         return temp.ToList<Resident>();
      }
      private List<Resident> getResidents()
      {

         List<Resident> r = new List<Resident>();
         int count = 0;
         try
         {
            foreach (Animal a in this.myAnimals)
            {
               if (a.GetType().Name == "Resident" && a.IsDead == false)
               {
                  r.Add(a as Resident);
               }
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return r;
      }

      private void makeNextGenAnimal(InitialAnimalAttributes inIAA, DateTime currTime)
      {
         Animal tmpAnimal = null;
         mLog.Debug("inside make next gen animal");
         try
         {
            for (int i = 0; i < inIAA.NumToMake; i++)
            {
               if (inIAA.Sex == 'M')
               {
                  mLog.Debug("makeing a new male");
                  tmpAnimal = new Male();
                  tmpAnimal.GenderModifier = this.mMaleModifier;
               }
               else
               {
                  mLog.Debug("makeing a new female");
                  tmpAnimal = new Female();
                  tmpAnimal.GenderModifier = this.mFemaleModifier;
               }
                  
               tmpAnimal.IdNum = this.currNumAnimals++;
               mLog.Debug("just made " + tmpAnimal.IdNum.ToString());
               tmpAnimal.Location = inIAA.Location;
               tmpAnimal.AnimalAtributes = this.AnimalAttributes;
               tmpAnimal.CurrEnergy = this.AnimalAttributes.InitialEnergy;
               mLog.Debug("now setting the mover");
               tmpAnimal.myMover = this.mMover;
               tmpAnimal.StateModifer = this.SafeSearchMod;
               tmpAnimal.AnimalManager = this;
               tmpAnimal.HomeRangeTrigger = this.mHomeRangeTrigger;
               tmpAnimal.HomeRangeFinder = this.mHomeRangeFinder;
               tmpAnimal.setInitialSleepTime(currTime);
               this.SetMapValues(tmpAnimal,currTime);
               tmpAnimal.BuildTextWriter(currTime.Year.ToString());
               tmpAnimal.dump();
               this.myAnimals.Add(tmpAnimal);
            }
            
         }

        
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      /// <summary>
      /// After changing the social map, the animals location will stay the same, 
      /// but the social index will need to be reset.
      /// </summary>
      /// <param name="inSocialMap">The new social map to pull the new index from</param>
      private void resetDisperserSocialIndex(Map inSocialMap)
      {
         List<Animal> dispersers = this.getDispersers();

         foreach (Animal d in dispersers)
         {
            d.SocialIndex = inSocialMap.getCurrentPolygon(d.Location);
         }

      }

      /// <summary>
      /// after changing out the social map we need to see if the current residents
      /// still have enough area to survive.
      /// </summary>
      /// <param name="newSocialMap">The new map to check area against</param>
      private void resetResidentsHomeRange(Map newSocialMap)
      {
         try
         {
            MapManager mm = MapManager.GetUniqueInstance();
            List<Resident> r = this.getResidents();
            mLog.Debug("inside resetResidentsHomeRange we are going to loop through " + r.Count.ToString() + " residents");
            foreach (Resident a in r)
            {
               mLog.Debug("my animals location is " + a.HomeRangeCenter.X.ToString() + " " + a.HomeRangeCenter.Y.ToString());
               mLog.Debug("now going to try and build a home range with the map manager");
               if (mm.BuildHomeRange(a))
               {
                  mLog.Debug("built it ");
               }
               else
               {
                  mLog.Debug("could not create one");
                  a.TextFileWriter.addLine("Not enough suitable habitat after map switch.");
                  mLog.Debug("calling changeToDeadAnimal");
                  this.changeToDeadAnimal(a);
                  mLog.Debug("new animal type is " + a.GetType());
                  //now reset the social map to not occupied for that resident.
                  AdjustMapForDeadAnimal(newSocialMap, a);
               }
            }
            mLog.Debug("leaving resetResidentsHomeRange");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      private void setFemaleHomeRange()
      {
         List<Animal> females = this.getDispersers("female");
         foreach (Animal female in females)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }
         List<Resident> femaleResidents = this.getResidents("female");
         foreach (Animal female in femaleResidents)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }


      }

      private void setMaleHomeRange()
      {
         List<Animal> males = this.getDispersers("male");
         foreach (Animal male in males)
         {
            male.HomeRangeCriteria = this.mMaleHomeRangeCriteria;
         }
           List<Resident> maleResidents = this.getResidents("male");
         foreach (Resident male in maleResidents)
         {
            male.HomeRangeCriteria = this.mMaleHomeRangeCriteria;
         }
      }

      private void SetMapValues(Animal a, DateTime currTime)
      {
         mLog.Debug("inside SetMapValues for " + a.IdNum.ToString());
         this.mMapValues = new Dictionary<IPoint, MapValue>();
         mLog.Debug("Now check to see if we have the value for this location or not");
         if (this.mMapValues.ContainsKey(a.Location))
         {
            mLog.Debug("had it so set the values");

            a.CaptureFood = this.mMapValues[a.Location].CaptureFood;
            a.FoodMeanSize = this.mMapValues[a.Location].FoodMeanSize;
            a.MoveSpeed = this.mMapValues[a.Location].MoveSpeed;
            a.MoveTurtosity = this.mMapValues[a.Location].MoveTurtosity;
            a.PerceptonModifier = this.mMapValues[a.Location].PerceptonModifier;
            a.PredationRisk = this.mMapValues[a.Location].PredationRisk;
            a.FoodIndex = this.mMapValues[a.Location].FoodIndex;
            a.MoveIndex = this.mMapValues[a.Location].MoveIndex;
            a.RiskIndex = this.mMapValues[a.Location].RiskIndex;
            a.SocialIndex = this.mMapValues[a.Location].SocialIndex;
         }
         else
         {
            mLog.Debug("did not have it, so go get them from the animal");
            a.setInitialValues(currTime);
            mLog.Debug("now store them off");
            MapValue mv = new MapValue();
            mv.CaptureFood = a.CaptureFood;
            mv.CurrLocation = a.Location;
            mv.FoodMeanSize = a.FoodMeanSize;
            mv.FoodSD_Size = a.FoodSD_Size;
            mv.MoveSpeed = a.MoveSpeed;
            mv.MoveTurtosity = a.MoveTurtosity;
            mv.PerceptonModifier = a.PerceptonModifier;
            mv.PredationRisk = a.PredationRisk;
            mv.FoodIndex = a.FoodIndex;
            mv.MoveIndex = a.MoveIndex;
            mv.RiskIndex = a.RiskIndex;
            mv.SocialIndex = a.SocialIndex;
            mLog.Debug("add to the list");
            this.mMapValues.Add(a.Location, mv);
         }
         return;
      }

      private void setNextGenHomeRange()
      {
         setMaleHomeRange();
         setFemaleHomeRange();

      }

      public void setResidentsTextOutput(string path, string year)
      {
         List<Resident> res = this.getResidents();
         foreach(Resident r in res)
            r.BuildTextWriter(year,path + "\\Resident");
      }

      private void setResidentAttributes()
      {
         List<Resident> rs = getResidents();
         foreach (Resident r in rs)
            r.MyAttributes = this.ResidentAttributes;
      }

		#endregion Private Methods 

		#endregion Methods 
      
       
       // toto
       public static bool save(string filename,AnimalManager animalManager)
      {
          FileStream fos = File.Create(filename);
          try
          {
              BinaryFormatter bf = new BinaryFormatter();
              bf.Serialize(fos, animalManager);
              fos.Close();
              return true;
          }catch(Exception e){
              fos.Close();
              return false; // something bad happened
          } 
      }

       //todo
       public static AnimalManager load(string filename)
       {
           if (!File.Exists(filename))
           {
               return null;
           }
           FileStream fis = File.OpenRead(filename);
           try
           {

              
               BinaryFormatter bf = new BinaryFormatter();
               AnimalManager am = (SEARCH.AnimalManager)bf.Deserialize(fis);
               fis.Close();
               return am;
           }
           catch (Exception e)
           {

               Console.WriteLine(e.ToString());
               // something bad happened.
               fis.Close();
               return null;
           }
       }

   }



   



}

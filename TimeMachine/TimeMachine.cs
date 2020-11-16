using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using View = Autodesk.Revit.DB.View;
using System.Windows.Forms;
using BH_TimeMachineForm;

namespace Github_TimeMachine
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        #region filter management

        public ParameterValueProvider findFilterParameterId(Document doc, string paramName)
        // Search for filter parameters ids by name
        {
            // A wall element is used to find the filter parameters
            Element wall = new FilteredElementCollector(doc).OfClass(typeof(Wall)).FirstOrDefault();
            ElementId paramId = new ElementId(wall.GetParameters(paramName).FirstOrDefault().Id.IntegerValue);
            return new ParameterValueProvider(paramId);
        }

        public ParameterFilterElement f_EnConstruction(Document doc, int debutPhase, int finPhase,
            ParameterValueProvider c0, ParameterValueProvider c1,
            ParameterValueProvider d0, ParameterValueProvider d1,
            string name, List<ElementId> catids)
        // Filter rules for elements under construction
        {
            //Filter Definition
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(c1, new FilterNumericGreater(), (debutPhase))));
            //lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericGreater(), (finPhase))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            //Creating the final filter (requires transaction)
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            //view.AddFilter(finalFilter.Id);
            return finalFilter;
        }

        public ParameterFilterElement f_DejaConstruit(Document doc, int debutPhase, int finPhase,
                    ParameterValueProvider c0, ParameterValueProvider c1,
                    ParameterValueProvider d0, ParameterValueProvider d1,
                    string name, List<ElementId> catids)
        // Filter rules for elements already built
        {
            List<ElementFilter> lev1Filter = new List<ElementFilter>();
            lev1Filter.Add(new ElementParameterFilter(
                new FilterInverseRule(
                    new FilterIntegerRule(
                        c0, new FilterNumericEquals(), (-1)))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev1Filter);

            List<ElementFilter> lev2Filter = new List<ElementFilter>();
            lev2Filter.Add(new ElementParameterFilter(
                    new FilterIntegerRule(
                        c1, new FilterNumericLessOrEqual(), (debutPhase))));

            lev2Filter.Add(new ElementParameterFilter(
                    new FilterIntegerRule(
                        d0, new FilterNumericGreaterOrEqual(), (finPhase))));
            LogicalAndFilter fRule2 = new LogicalAndFilter(lev2Filter);

            LogicalOrFilter fFilter = new LogicalOrFilter(fRule, fRule2);

            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fFilter);
            return finalFilter;
        }

        public ParameterFilterElement f_DejaDemoli(Document doc, int debutPhase, int finPhase,
                    ParameterValueProvider c0, ParameterValueProvider c1,
                    ParameterValueProvider d0, ParameterValueProvider d1,
                    string name, List<ElementId> catids)
        // Filter rules for elements already demolished
        {
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(
                    new FilterIntegerRule(
                        d1, new FilterNumericLessOrEqual(), (debutPhase))));

            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            return finalFilter;
        }

        public ParameterFilterElement f_EnDemolition(Document doc, int debutPhase, int finPhase,
            ParameterValueProvider c0, ParameterValueProvider c1,
            ParameterValueProvider d0, ParameterValueProvider d1,
            string name, List<ElementId> catids)
        // Filter rules for elements under demolition
        {
            List<ElementFilter> lev1AFilter = new List<ElementFilter>();
            lev1AFilter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericLess(), (finPhase))));

            LogicalAndFilter fRule = new LogicalAndFilter(lev1AFilter);
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);

            return finalFilter;
        }

        public ParameterFilterElement f_PasConstruit(Document doc, int debutPhase, int finPhase,
            ParameterValueProvider c0, ParameterValueProvider c1,
            ParameterValueProvider d0, ParameterValueProvider d1,
            string name, List<ElementId> catids)
        // Filter rules for elements not built yet
        {
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(c0, new FilterNumericGreaterOrEqual(), (finPhase))));

            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);

            return finalFilter;
        }

        public ParameterFilterElement overrideFilterGraphics(ParameterFilterElement filter, View view,
    ElementId surfaceForegroundPatternId, Color projectionFillColor, Color projectionLineColor,
    Color cutLineColor, int projectionLineWeight, int cutLineWeight, ElementId surfaceBackgroundPatternId,
    Color surfaceBackgroundPatternColor, int surfaceTransparency, ElementId cutBackgroundPatternId,
    Color cutBackgroundPatternColor, ElementId cutForegroundPatternId, Color cutForegroundPatternColor,
    bool cutBackgroundPatternVisible, bool cutForegroundPatternVisible,
    bool surfaceBackgroundPatternVisible, bool surfaceForegroundPatternVisible)
        // Set filter graphics for a given view
        {
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();

            if (projectionFillColor != null)
            {
                ogs.SetSurfaceForegroundPatternColor(projectionFillColor);
            }
            if (projectionLineColor != null)
            {
                ogs.SetProjectionLineColor(projectionLineColor);
            }
            if (projectionLineWeight != -1)
            {
                ogs.SetProjectionLineWeight(projectionLineWeight);
            }
            if (surfaceForegroundPatternId != null)
            {
                ogs.SetSurfaceForegroundPatternId(surfaceForegroundPatternId);
            }
            if (surfaceBackgroundPatternId != null)
            {
                ogs.SetSurfaceBackgroundPatternId(surfaceBackgroundPatternId);
            }
            if (surfaceBackgroundPatternColor != null)
            {
                ogs.SetSurfaceBackgroundPatternColor(surfaceBackgroundPatternColor);
            }
            if (surfaceTransparency != -1)
            {
                ogs.SetSurfaceTransparency(surfaceTransparency);
            }
            if (cutLineColor != null)
            {
                ogs.SetCutLineColor(cutLineColor);
            }
            if (cutLineWeight != -1)
            {
                ogs.SetCutLineWeight(cutLineWeight);
            }
            if (cutBackgroundPatternId != null)
            {
                ogs.SetCutBackgroundPatternId(cutBackgroundPatternId);
            }
            if (cutBackgroundPatternColor != null)
            {
                ogs.SetCutBackgroundPatternColor(cutBackgroundPatternColor);
            }
            if (cutForegroundPatternId != null)
            {
                ogs.SetCutForegroundPatternId(cutForegroundPatternId);
            }
            if (cutForegroundPatternColor != null)
            {
                ogs.SetCutForegroundPatternColor(cutForegroundPatternColor);
            }

            ogs.SetCutBackgroundPatternVisible(cutBackgroundPatternVisible);
            ogs.SetCutForegroundPatternVisible(cutForegroundPatternVisible);
            ogs.SetSurfaceBackgroundPatternVisible(surfaceBackgroundPatternVisible);
            ogs.SetSurfaceForegroundPatternVisible(surfaceForegroundPatternVisible);



            view.SetFilterOverrides(filter.Id, ogs);

            return filter;
        }


        #region Add / remove filters from view
        public void addFiltersToView(Document doc, View v, List<ElementId> catids, int debutPhase, int finPhase)
        //Adds the parameter filters to a given view
        {
            #region Filters definitions and graphics
            #region Parameter value providers
            ParameterValueProvider c0 = findFilterParameterId(doc, "GLS-PHA_Construction-début");
            ParameterValueProvider c1 = findFilterParameterId(doc, "GLS-PHA_Construction-fin");
            ParameterValueProvider d0 = findFilterParameterId(doc, "GLS-PHA_Démolition-début");
            ParameterValueProvider d1 = findFilterParameterId(doc, "GLS-PHA_Démolition-fin");
            #endregion
            #region Filters graphics
            // Deja construit
            Color c_DejaCons_Fill = new Color(180, 180, 180);
            Color l_DejaCons_Fill = new Color(160, 160, 160);
            Color c_DejaCons_Cut = new Color(140, 140, 140);
            Color l_DejaCons_Cut = new Color(100, 100, 100);

            // Existant
            Color c_Existant_Proj = new Color(180, 180, 180);// #FG
            Color c_Existant_Cut = new Color(140, 140, 140);// #BG
            int l_Existant_projectionLineWeight = 1;
            Color l_Existant_projectionLineColor = new Color(100, 100, 100);
            int l_Existant_cutLineWeight = 4;
            Color l_Existant_cutLineColor = new Color(100, 100, 100);
            Color l_Existant_SurfColor = new Color(255, 255, 255);

            // Deja demoli
            Color c_Demoli_Fill = new Color(243, 224, 88);
            Color l_Demoli_Fill = new Color(243, 224, 88);
            Color c_Demoli_Cut = new Color(243, 224, 88);
            Color l_Demoli_Cut = new Color(243, 224, 88);
            int l_Demoli_projectionLineWeight = 1;
            int l_Demoli_cutLineWeight = 4;
            int l_Demoli_surfaceTransp = -1;

            // En construction
            Color c_Nouveau_Fill = new Color(255, 160, 160);
            Color l_Nouveau_Fill = new Color(255, 20, 20);
            int l_Nouveau_projectionLineWeight = 3;
            Color c_Nouveau_Cut = new Color(255, 128, 128);
            Color l_Nouveau_Cut = new Color(255, 0, 0);
            int l_Nouveau_cutLineWeight = 5;

            // Fill patterns
            FilteredElementCollector allFillPatterns = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement));//.ToElements();
            ElementId filledPattern = allFillPatterns.FirstOrDefault().Id;

            //Line patterns
            FilteredElementCollector allLinePatterns = new FilteredElementCollector(doc).OfClass(typeof(LinePatternElement)); //.ToElements()

            #endregion
            #endregion

            #region Creating filters

            #region Deleting old filters
            ICollection<ElementId> allFilters = v.GetFilters();
            Element pfe; // ParameterFilterElement
            List<ElementId> filtersToBeDeletedIds = new List<ElementId>(); ;

            for (var i = 0; i < allFilters.Count(); i++)
            {
                pfe = doc.GetElement(allFilters.ElementAt(i));
                if (pfe != null && pfe.Name.Contains("Filtromatisation_TimeMachine"))
                {
                    filtersToBeDeletedIds.Add(pfe.Id);
                }
            }
            doc.Delete(filtersToBeDeletedIds);

            #endregion

            #region creating new filters

            ParameterFilterElement f0 = f_EnDemolition(doc, debutPhase, finPhase, c0, c1, d0, d1, "GLS_PHA_" + debutPhase.ToString() + "_" + finPhase.ToString() + "_EnDemolition _" + "_Filtromatisation_TimeMachine", catids);
            ParameterFilterElement f2 = f_EnConstruction(doc, debutPhase, finPhase, c0, c1, d0, d1, "GLS_PHA_" + debutPhase.ToString() + "_" + finPhase.ToString() + "_EnConstruction_" + "_Filtromatisation_TimeMachine", catids);
            ParameterFilterElement f3 = f_DejaConstruit(doc, debutPhase, finPhase, c0, c1, d0, d1, "GLS_PHA_" + debutPhase.ToString() + "_" + finPhase.ToString() + "_DejaConstruit_" + "_Filtromatisation_TimeMachine", catids);
            ParameterFilterElement f4 = f_DejaDemoli(doc, debutPhase, finPhase, c0, c1, d0, d1, "GLS_PHA_" + debutPhase.ToString() + "_" + finPhase.ToString() + "_DejaDemoli_" + "_Filtromatisation_TimeMachine", catids);
            ParameterFilterElement f5 = f_PasConstruit(doc, debutPhase, finPhase, c0, c1, d0, d1, "GLS_PHA_" + debutPhase.ToString() + "_" + finPhase.ToString() + "_PasEncoreConstruit_" + "_Filtromatisation_TimeMachine", catids);
            #endregion

            #region adding created filters to view
            v.AddFilter(f4.Id);
            v.AddFilter(f5.Id);
            v.AddFilter(f2.Id);
            v.AddFilter(f0.Id);
            v.AddFilter(f3.Id);

            overrideFilterGraphics(f0, v, filledPattern, c_Demoli_Fill, l_Demoli_Fill, l_Demoli_Cut, l_Demoli_projectionLineWeight, l_Demoli_cutLineWeight, filledPattern, c_Demoli_Fill, l_Demoli_surfaceTransp, filledPattern, l_Demoli_Fill, filledPattern, c_Demoli_Fill, false, false, false, false);
            overrideFilterGraphics(f2, v, filledPattern, c_Nouveau_Fill, l_Nouveau_Fill, l_Nouveau_Cut, l_Nouveau_projectionLineWeight, l_Nouveau_cutLineWeight, null, null, -1, null, null, filledPattern, c_Nouveau_Cut, true, true, true, true);
            overrideFilterGraphics(f3, v, filledPattern, c_Existant_Proj, l_Existant_projectionLineColor, l_Existant_cutLineColor, l_Existant_projectionLineWeight, l_Existant_cutLineWeight, filledPattern, l_Existant_SurfColor, -1, null, null, filledPattern, c_Existant_Cut, true, true, true, true);
            v.SetFilterVisibility(f4.Id, false);
            v.SetFilterVisibility(f5.Id, false);

            #endregion

            #endregion
        }

        public void removeFiltersFromView(Document doc, View v)
        {
            ICollection<ElementId> allFilters = v.GetFilters();
            Element pfe; // ParameterFilterElement
            List<ElementId> filtersToBeDeletedIds = new List<ElementId>(); ;

            for (var i = 0; i < allFilters.Count(); i++)
            {
                pfe = doc.GetElement(allFilters.ElementAt(i));
                if (pfe != null && pfe.Name.Contains("Filtromatisation_TimeMachine"))
                {
                    filtersToBeDeletedIds.Add(pfe.Id);
                }
            }
            doc.Delete(filtersToBeDeletedIds);
        }

        #endregion

        #endregion

        #region Time Machine
        public void timeMachine(Document doc, List<ElementId> catids)
        // Create new / update / remove TimeMachine filters
        {
            Application.EnableVisualStyles();
            Form1 timeMachine = new Form1();
            Application.Run(timeMachine); // Run UI TimeMachine form
            int debutPhase = timeMachine.minDateOut; // Information from UI TimeMachine form
            int finPhase = timeMachine.maxDateOut; // Information from UI TimeMachine form

            if (timeMachine.onEffaceLesFiltres) // Remove existing TimeMachine filters
            {
                removeFiltersFromView(doc, doc.ActiveView);
            }
            if (timeMachine.onGenereLesFiltres) // Create new TimeMachine filters
            {
                addFiltersToView(doc, doc.ActiveView, catids, debutPhase, finPhase);
            }
        }
        #endregion

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            #region API definitions
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            #endregion

            #region Filter categories
            // Filter categories for the TimeMachine generated filters
            List<ElementId> catids = new List<ElementId> {
            new ElementId(BuiltInCategory.OST_Walls),
            new ElementId(BuiltInCategory.OST_Floors),
            new ElementId(BuiltInCategory.OST_ColumnAnalytical),
            new ElementId(BuiltInCategory.OST_Columns),
            new ElementId(BuiltInCategory.OST_StructuralColumns),
            new ElementId(BuiltInCategory.OST_Stairs),
            new ElementId(BuiltInCategory.OST_StructuralFraming),
            new ElementId(BuiltInCategory.OST_StructConnections),
            new ElementId(BuiltInCategory.OST_GenericModel),
            new ElementId(BuiltInCategory.OST_StructuralFoundation),
            new ElementId(BuiltInCategory.OST_Conduit),
            new ElementId(BuiltInCategory.OST_PipeInsulations),
            new ElementId(BuiltInCategory.OST_PipeFitting),
            new ElementId(BuiltInCategory.OST_ConduitFitting),
            new ElementId(BuiltInCategory.OST_DuctFitting),
            new ElementId(BuiltInCategory.OST_Windows),
            new ElementId(BuiltInCategory.OST_PipeCurves),
            new ElementId(BuiltInCategory.OST_DuctCurves),
            new ElementId(BuiltInCategory.OST_DuctAccessory),
            new ElementId(BuiltInCategory.OST_PipeAccessory),
            new ElementId(BuiltInCategory.OST_Parts),
            new ElementId(BuiltInCategory.OST_FlexPipeCurves),
            new ElementId(BuiltInCategory.OST_Mass)
        };
            #endregion

            Transaction tran = new Transaction(doc);
            tran.Start("Time Machine");
            timeMachine(doc, catids);
            tran.Commit();

            return Result.Succeeded;
        }
    }
}

#region UI for time machine

namespace BH_TimeMachineForm
{

    public partial class Form1 : System.Windows.Forms.Form
    {
        List<int> allValidDates = new List<int>();
        int minDateInt = 200101;//Date par défaut: 1er janvier 2020
        int maxDateInt = 231230;//Date par défaut: 30 decembre 2023
        //BH_Date auxDate;
        DateTime minDate;
        DateTime maxDate;

        #region reading / writing log

        string logFolderPath = @"C:\AppData\BH_Plugins\";
        string fileName_timeMachine = "timeMachineLog_UserChoices.txt";

        public void VerifyDir(string path)
        {
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }

        public void readLog(string logFolderPath, string fileName_timeMachine)
        {
            VerifyDir(logFolderPath);
            if (!System.IO.File.Exists(logFolderPath + fileName_timeMachine))
            {
                return;
            }
            else
            {
                System.Collections.Generic.IEnumerable<string> log = System.IO.File.ReadLines(logFolderPath + fileName_timeMachine);

                minDateInt = int.Parse(log.ElementAt(0));
                maxDateInt = int.Parse(log.ElementAt(1));
                resetDates(new BH_Date(minDateInt).toDateTime(), new BH_Date(maxDateInt).toDateTime());
                trackBar1.Value = int.Parse(log.ElementAt(2));
                trackBar2.Value = int.Parse(log.ElementAt(3));
                textBox1.Text = log.ElementAt(4);

                int dateValue = allValidDates.ElementAt(trackBar1.Value);
                BH_Date myDate = new BH_Date(dateValue);
                label4.Text = dateValue.ToString();
                dateTimePicker1.Value = myDate.toDateTime();
                minDateOut = myDate.toInt();

                dateValue = allValidDates.ElementAt(trackBar2.Value);
                myDate = new BH_Date(dateValue);
                label5.Text = dateValue.ToString();
                dateTimePicker2.Value = myDate.toDateTime();
                maxDateOut = myDate.toInt();
                return;
            }

        }
        public void writeLog(string logFolderPath, string fileName_timeMachine)
        {
            string[] contentss = new string[]
            {
                minDateInt.ToString(),
                maxDateInt.ToString(),
                trackBar1.Value.ToString(),
                trackBar2.Value.ToString(),
                textBox1.Text //intervalle
            };
            VerifyDir(logFolderPath);
            bool fileExists = System.IO.File.Exists(logFolderPath + fileName_timeMachine);
            if (fileExists == false)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(logFolderPath + fileName_timeMachine, true);
                //file.WriteLine(imageFolderBrowserDialog.SelectedPath);
                file.Close();
                //return;
            }
            System.IO.File.WriteAllLines(logFolderPath + fileName_timeMachine, contentss);
            return; //System.IO.File.ReadLines(logFolderPath + fileName_timeMachine).First();
        }


        #endregion

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            BH_Date dateMin = new BH_Date(minDateInt);
            BH_Date dateMax = new BH_Date(maxDateInt);
            minDate = dateMin.toDateTime();
            maxDate = dateMax.toDateTime();

            readLog(logFolderPath, fileName_timeMachine);
            resetDates(minDate, maxDate);
        }

        private void resetDates(DateTime minDate, DateTime maxDate)
        {
            dateTimePicker3.Value = minDate;
            dateTimePicker4.Value = maxDate;
            int minDateInt = new BH_Date(minDate).toInt();
            int maxDateInt = new BH_Date(maxDate).toInt();
            BH_Date auxDate;
            allValidDates = new List<int>();
            for (int i = minDateInt; i <= maxDateInt; i++)
            {
                auxDate = new BH_Date(i);
                if (auxDate.isValid)
                {
                    allValidDates.Add(i);
                }
            }
            trackBar1.Minimum = 0;
            trackBar1.Maximum = allValidDates.Count() - 1; //1er octobre 2023
            trackBar1.TickFrequency = 1; //Intervalle de 1 mois

            trackBar2.Minimum = 0; //1er octobre 2020
            trackBar2.Maximum = allValidDates.Count() - 1; //1er octobre 2023
            trackBar2.TickFrequency = 1; //Intervalle de 1 mois

            dateTimePicker1.MinDate = minDate;
            dateTimePicker1.MaxDate = maxDate;

            dateTimePicker2.MinDate = minDate;
            dateTimePicker2.MaxDate = maxDate;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int dateValue = allValidDates.ElementAt(trackBar1.Value);
            BH_Date myDate = new BH_Date(dateValue);

            if (myDate.isValid)
            {
                label4.Text = dateValue.ToString();
                dateTimePicker1.Value = myDate.toDateTime();

                if (trackBar2.Value < trackBar1.Value)
                {
                    trackBar2.Value = trackBar1.Value;
                    dateTimePicker2.Value = myDate.toDateTime();
                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            int dateValue = allValidDates.ElementAt(trackBar2.Value);
            BH_Date myDate = new BH_Date(dateValue);

            if (myDate.isValid)
            {
                label5.Text = dateValue.ToString();
                dateTimePicker2.Value = myDate.toDateTime();
            }
            if (trackBar2.Value < trackBar1.Value)
            {
                trackBar1.Value = trackBar2.Value;
                dateTimePicker2.Value = myDate.toDateTime();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                trackBar1.Value += int.Parse(textBox1.Text);
                int dateValue = allValidDates.ElementAt(trackBar1.Value);
                BH_Date myDate = new BH_Date(dateValue);
                label4.Text = dateValue.ToString();
                dateTimePicker1.Value = myDate.toDateTime();
            }
            catch
            {
                try
                {

                    trackBar1.Value += 1;
                    int dateValue = allValidDates.ElementAt(trackBar1.Value);
                    BH_Date myDate = new BH_Date(dateValue);
                    label4.Text = dateValue.ToString();
                    dateTimePicker1.Value = myDate.toDateTime();
                }
                catch
                {

                }

            }
            try
            {
                trackBar2.Value += int.Parse(textBox1.Text);
                int dateValue2 = allValidDates.ElementAt(trackBar2.Value);
                BH_Date myDate2 = new BH_Date(dateValue2);
                label5.Text = dateValue2.ToString();
                dateTimePicker2.Value = myDate2.toDateTime();
            }
            catch
            {
                try
                {
                    trackBar2.Value += 1;
                    int dateValue2 = allValidDates.ElementAt(trackBar2.Value);
                    BH_Date myDate2 = new BH_Date(dateValue2);
                    label5.Text = dateValue2.ToString();
                    dateTimePicker2.Value = myDate2.toDateTime();
                }
                catch
                {

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            try
            {
                trackBar1.Value -= int.Parse(textBox1.Text);
                int dateValue = allValidDates.ElementAt(trackBar1.Value);
                BH_Date myDate = new BH_Date(dateValue);
                label4.Text = dateValue.ToString();
                dateTimePicker1.Value = myDate.toDateTime();
            }
            catch
            {
                try
                {

                    trackBar1.Value -= 1;
                    int dateValue = allValidDates.ElementAt(trackBar1.Value);
                    BH_Date myDate = new BH_Date(dateValue);
                    label4.Text = dateValue.ToString();
                    dateTimePicker1.Value = myDate.toDateTime();
                }
                catch
                {

                }

            }
            try
            {
                trackBar2.Value -= int.Parse(textBox1.Text);
                int dateValue2 = allValidDates.ElementAt(trackBar2.Value);
                BH_Date myDate2 = new BH_Date(dateValue2);
                label5.Text = dateValue2.ToString();
                dateTimePicker2.Value = myDate2.toDateTime();
            }
            catch
            {
                try
                {
                    trackBar2.Value -= 1;
                    int dateValue2 = allValidDates.ElementAt(trackBar2.Value);
                    BH_Date myDate2 = new BH_Date(dateValue2);
                    label5.Text = dateValue2.ToString();
                    dateTimePicker2.Value = myDate2.toDateTime();
                }
                catch
                {

                }
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            BH_Date minBHDate = new BH_Date(dateTimePicker3.Value);
            minDateInt = minBHDate.toInt();
            minDate = minBHDate.toDateTime();
            resetDates(dateTimePicker3.Value, maxDate);
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            BH_Date maxBHDate = new BH_Date(dateTimePicker4.Value);
            maxDateInt = maxBHDate.toInt();
            maxDate = maxBHDate.toDateTime();
            resetDates(minDate, dateTimePicker4.Value);

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            minDateOut = new BH_Date(dateTimePicker1.Value).toInt();
            maxDateOut = new BH_Date(dateTimePicker2.Value).toInt();
            onEffaceLesFiltres = true;
            onGenereLesFiltres = false;
            writeLog(logFolderPath, fileName_timeMachine);
            this.Close();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            minDateOut = new BH_Date(dateTimePicker1.Value).toInt();
            maxDateOut = new BH_Date(dateTimePicker2.Value).toInt();
            onEffaceLesFiltres = false;
            onGenereLesFiltres = true;
            writeLog(logFolderPath, fileName_timeMachine);
            this.Close();
        }
    }

    public class BH_Date
    {
        public bool isValid;
        public int day;
        public int month;
        public int year;

        public BH_Date(int dateNumber)
        {
            this.year = 2000 + (dateNumber / 10000);
            this.month = dateNumber / 100 % 100;
            this.day = dateNumber % 100;
            this.isValid = (month <= 12 && month != 0 && day <= DateTime.DaysInMonth(year, month) && day != 0);
        }

        public BH_Date(DateTime DateTime)
        {
            this.year = DateTime.Year;
            this.month = DateTime.Month;
            this.day = DateTime.Day;
            this.isValid = true;
        }


        public DateTime toDateTime()
        {
            return new DateTime(year, month, day);
        }

        public int toInt()
        {
            return ((year - 2000) * 10000) + month * 100 + day;
        }

    }

}

namespace BH_TimeMachineForm
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public int minDateOut;
        public int maxDateOut;
        public bool onEffaceLesFiltres;
        public bool onGenereLesFiltres;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.AllowDrop = true;
            this.trackBar1.Location = new System.Drawing.Point(136, 85);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(414, 69);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(136, 160);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(414, 69);
            this.trackBar2.TabIndex = 1;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "End:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(574, 85);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 26);
            this.dateTimePicker1.TabIndex = 4;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(574, 160);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 26);
            this.dateTimePicker2.TabIndex = 5;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(574, 268);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 45);
            this.button1.TabIndex = 6;
            this.button1.Text = "Generate filters";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(574, 328);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 45);
            this.button2.TabIndex = 7;
            this.button2.Text = "Delete filters";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(614, 422);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Version 16.11.2020";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label4.Location = new System.Drawing.Point(33, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Start date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label5.Location = new System.Drawing.Point(36, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = " End Date";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // dateTimePicker3
            // 
            this.dateTimePicker3.Location = new System.Drawing.Point(194, 369);
            this.dateTimePicker3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker3.Name = "dateTimePicker3";
            this.dateTimePicker3.Size = new System.Drawing.Size(298, 26);
            this.dateTimePicker3.TabIndex = 11;
            this.dateTimePicker3.ValueChanged += new System.EventHandler(this.dateTimePicker3_ValueChanged);
            // 
            // dateTimePicker4
            // 
            this.dateTimePicker4.Location = new System.Drawing.Point(194, 411);
            this.dateTimePicker4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker4.Name = "dateTimePicker4";
            this.dateTimePicker4.Size = new System.Drawing.Size(298, 26);
            this.dateTimePicker4.TabIndex = 12;
            this.dateTimePicker4.ValueChanged += new System.EventHandler(this.dateTimePicker4_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 369);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "Date Min (absolut):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 411);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(147, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "Date Max (absolut):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(87, 328);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 20);
            this.label8.TabIndex = 15;
            this.label8.Text = "Interval:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(189, 292);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 20);
            this.label9.TabIndex = 16;
            this.label9.Text = "Days:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(246, 292);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 20);
            this.label10.TabIndex = 17;
            this.label10.Text = "Months:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(303, 292);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 20);
            this.label11.TabIndex = 18;
            this.label11.Text = "Years:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(194, 323);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(46, 26);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "5";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(250, 323);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(46, 26);
            this.textBox2.TabIndex = 20;
            this.textBox2.Text = "0";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(308, 323);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(46, 26);
            this.textBox3.TabIndex = 21;
            this.textBox3.Text = "0";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(361, 207);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 46);
            this.button3.TabIndex = 22;
            this.button3.Text = "+ interval";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(241, 207);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 46);
            this.button4.TabIndex = 23;
            this.button4.Text = "- interval";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePicker4);
            this.Controls.Add(this.dateTimePicker3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(822, 505);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(822, 505);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "BaslerHofmann Time Machine";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DateTimePicker dateTimePicker3;
        private System.Windows.Forms.DateTimePicker dateTimePicker4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

#endregion

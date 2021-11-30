using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BH_form_ParameterMachine;
using View = Autodesk.Revit.DB.View;

namespace BH_App_ParameterMachine
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        private class BH_Parameter
        {
            #region Constructor
            //Parameter Info
            public ParameterValueProvider paramValueProvider;
            //Parameter Type
            public bool isString;
            public bool isInt;
            public bool isElementId;
            public bool isDouble;
            //Parameter has values in view
            public bool paramFound;
            //List of parameter values
            public List<string> listStrValues;
            public List<int> listIntValues;
            public List<double> listDblValues;
            public List<ElementId> listEIdValues;
            private int doubleValuesPrecision = 3;
            private ElementId parameterId;
            public List<ElementId> applicableCategories;

            #endregion
            public BH_Parameter(Document doc, string paramName)
            {
                this.isDouble = false;
                this.isString = false;
                this.isElementId = false;
                this.isInt = false;
                this.paramFound = false;

                this.listStrValues = new List<string>();
                this.listIntValues = new List<int>();
                this.listDblValues = new List<double>();
                this.listEIdValues = new List<ElementId>();
                findFilterParameterId(doc, paramName);
                fillListOfParamValues(doc, paramName);
                List<ElementId> applicableCats = GetCategoriesApplicableForParameter(doc, parameterId);
                this.applicableCategories = applicableCats;
            }
            private bool findFilterParameterId(Document doc, string paramName)
            // Search for filter parameters ids by name
            {
                Element myElem;
                ElementId paramId;
                FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType();
                foreach (Element e in allElementsInView)
                {
                    if (e.GetParameters(paramName).FirstOrDefault() != null)
                    {
                        myElem = e;
                        paramId = new ElementId(e.GetParameters(paramName).FirstOrDefault().Id.IntegerValue);
                        this.parameterId = paramId;
                        this.isString = new ParameterValueProvider(paramId).IsStringValueSupported(e);
                        this.isInt = new ParameterValueProvider(paramId).IsIntegerValueSupported(e);
                        this.isDouble = new ParameterValueProvider(paramId).IsDoubleValueSupported(e);
                        this.isElementId = new ParameterValueProvider(paramId).IsElementIdValueSupported(e);
                        this.paramValueProvider = new ParameterValueProvider(paramId);
                        this.paramFound = true;
                        return true;
                    }
                }
                return false;
            }
            private bool fillListOfParamValues(Document doc, string paramName)
            {
                if (paramFound)
                {
                    if (isString)
                    {
                        this.listStrValues= str_listOfParameterValues_(doc, doc.ActiveView, paramName);
                    }
                    else if (isInt)
                    {
                        this.listIntValues = int_listOfParameterValues(doc, doc.ActiveView, paramName);
                    }
                    else if (isDouble)
                    {
                        this.listDblValues = double_listOfParameterValues(doc, doc.ActiveView, paramName);
                    }
                    else if (isElementId)
                    {
                        this.listEIdValues = eId_listOfParameterValues(doc, doc.ActiveView, paramName);
                    }
                    else
                    {

                    }
                }
                return true;
            }
            private List<string> str_listOfParameterValues_(Document doc, View v, string paramName)
            {
                List<string> listOfParamVal = new List<string>();
                FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
                foreach (Element e in myFEC)
                {
                    try
                    {
                        Parameter param = e.GetParameters(paramName).FirstOrDefault();
                        string paramVal = "";
                        if (param != null && param.AsString().Trim() != "")
                        {
                            paramVal = param.AsString();
                        }
                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                    catch
                    {

                    }
                }
                return listOfParamVal;
            }
            private List<int> int_listOfParameterValues(Document doc, View v, string paramName)
            {
                List<int> listOfParamVal = new List<int>();
                FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
                foreach (Element e in myFEC)
                {
                    try
                    {
                        Parameter param = e.GetParameters(paramName).FirstOrDefault();
                        int paramVal = 0;
                        if (param.AsInteger() != 0)
                        {
                            paramVal = param.AsInteger();
                        }
                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                    catch
                    {

                    }
                }
                return listOfParamVal;
            }
            private List<double> double_listOfParameterValues(Document doc, View v, string paramName)
            {
                List<double> listOfParamVal = new List<double>();
                FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
                foreach (Element e in myFEC)
                {
                    try
                    {
                        Parameter param = e.GetParameters(paramName).FirstOrDefault();
                        double paramVal = 0;
                        if (param.AsDouble() != 0)
                        {
                            paramVal = Math.Round(param.AsDouble(),doubleValuesPrecision);
                        }
                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                    catch
                    {

                    }
                }
                return listOfParamVal;
            }
            private List<ElementId> eId_listOfParameterValues(Document doc, View v, string paramName)
            {
                List<ElementId> listOfParamVal = new List<ElementId>();
                List<string> elementIdValues = new List<string>();
                FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
                foreach (Element e in myFEC)
                {
                    try
                    {
                        Parameter param = e.GetParameters(paramName).FirstOrDefault();
                        ElementId paramVal = new ElementId(0);
                        string strElemIdVal = "";
                        if (int.Parse(param.AsElementId().ToString()) != -1)
                        {
                            paramVal = param.AsElementId();
                            strElemIdVal = paramVal.IntegerValue.ToString();
                        }
                        if (elementIdValues.Contains(strElemIdVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                    catch
                    {

                    }
                }
                return listOfParamVal;
            }
            
            #region new Function
            List<ElementId> GetCategoriesApplicableForParameter(Document doc, ElementId paramId)
            {
                // All categories available for filter 
                var allCategories = ParameterFilterUtilities.GetAllFilterableCategories();
                //allCategories.Remove(new ElementId(BuiltInCategory.OST_DuctTerminal));
                //allCategories.Remove(new ElementId(BuiltInCategory.OST_PlumbingFixtures));
                List<ElementId> retResult = new List<ElementId>();



                foreach (ElementId categoryId in allCategories)
                {
                    // get the list of parameters, compatible with the category.                 
                    var applicableParameters =
                        ParameterFilterUtilities.GetFilterableParametersInCommon(doc, new[] { categoryId });


                    // if the parameter we are interested in the collection 
                    // add it to the result 
                    if (applicableParameters.Contains(new ElementId(paramId.IntegerValue)))
                    {
                        retResult.Add(categoryId);
                    }
                }

                return retResult;
            }

            #endregion

        }

        #region collect project parameters
        public List<string> listOfParameters(Document doc)
        {
            List<string> projectParamNames = new List<string>();
            List<Definition> def_projectParams = new List<Definition>();
            FilteredElementCollector a = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement));

            foreach (var r in a)
            {
                SharedParameterElement shElem = r as SharedParameterElement;
                InternalDefinition def = shElem.GetDefinition();
                projectParamNames.Add(def.Name.Trim());
            }
            projectParamNames.Sort();
            projectParamNames = projectParamNames.Distinct().ToList();
            return projectParamNames;
        }
        #endregion


        #region filter management

        #region create filters for the views
        public ParameterFilterElement myStrFilter(Document doc, ParameterValueProvider myParam,
            string name, List<ElementId> catids, string paramVal)
        // Filter rules for elements under construction
        {
            //Filter Definition
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(
                new FilterStringRule(myParam, new FilterStringEquals(), paramVal, true), false));
            //lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericGreater(), (finPhase))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            //Creating the final filter (requires transaction)
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            //view.AddFilter(finalFilter.Id);
            return finalFilter;
        }
        public ParameterFilterElement myIntFilter(Document doc, ParameterValueProvider myParam,
            string name, List<ElementId> catids, int paramVal)
        // Filter rules for elements under construction
        {
            //Filter Definition
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(
                new FilterIntegerRule(myParam, new FilterNumericEquals(), paramVal), false));
            //lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericGreater(), (finPhase))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            //Creating the final filter (requires transaction)
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            //view.AddFilter(finalFilter.Id);
            return finalFilter;
        }
        public ParameterFilterElement myDoubleFilter(Document doc, ParameterValueProvider myParam,
            string name, List<ElementId> catids, double paramVal)
        // Filter rules for elements under construction
        {
            //Filter Definition
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(
                new FilterDoubleRule(myParam, new FilterNumericEquals(), paramVal, 0),false));
            //lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericGreater(), (finPhase))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            //Creating the final filter (requires transaction)
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            //view.AddFilter(finalFilter.Id);
            return finalFilter;
        }
        #endregion
        
        public ParameterFilterElement myElemIdFilter(Document doc, ParameterValueProvider myParam,
            string name, List<ElementId> catids, ElementId paramVal)
        // Filter rules for elements under construction
        {
            //Filter Definition
            List<ElementFilter> lev0Filter = new List<ElementFilter>();
            lev0Filter.Add(new ElementParameterFilter(
                new FilterElementIdRule(myParam, new FilterNumericEquals(), paramVal), false));
            //lev0Filter.Add(new ElementParameterFilter(new FilterIntegerRule(d0, new FilterNumericGreater(), (finPhase))));
            LogicalAndFilter fRule = new LogicalAndFilter(lev0Filter);
            //Creating the final filter (requires transaction)
            ParameterFilterElement finalFilter = ParameterFilterElement.Create(doc, name, catids);
            finalFilter.SetElementFilter(fRule);
            //view.AddFilter(finalFilter.Id);
            return finalFilter;
        }

        public ParameterFilterElement overrideFilterGraphics(ParameterFilterElement filter, View view,
        ElementId surfaceForegroundPatternId = null, Color projectionFillColor = null, Color projectionLineColor = null,
        Color cutLineColor = null, int projectionLineWeight = -1, int cutLineWeight = -1, ElementId surfaceBackgroundPatternId = null,
        Color surfaceBackgroundPatternColor = null, int surfaceTransparency = 0, ElementId cutBackgroundPatternId = null,
        Color cutBackgroundPatternColor = null, ElementId cutForegroundPatternId = null, Color cutForegroundPatternColor = null,
        bool cutBackgroundPatternVisible = true, bool cutForegroundPatternVisible = true,
        bool surfaceBackgroundPatternVisible = true, bool surfaceForegroundPatternVisible = true,
        ElementId cutLinePatternId = null, ElementId projectionLinePatternId = null)
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

            if (cutLinePatternId != null)
            {
                ogs.SetCutLinePatternId(cutLinePatternId);
            }
            if (projectionLinePatternId != null)
            {
                ogs.SetProjectionLinePatternId(projectionLinePatternId);
            }

            view.SetFilterOverrides(filter.Id, ogs);

            return filter;
        }


        #region Add / remove filters from view
        
        public List<string> str_listOfParameterValues(Document doc, View v, string paramName)
        {
            List<string> listOfParamVal = new List<string>();
            FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
            foreach (Element e in myFEC)
            {
                try
                {
                    Parameter param = e.GetParameters(paramName).FirstOrDefault();
                    string paramVal="";
                    if (param != null && param.AsString().Trim() != "")
                    {
                        paramVal=  param.AsString();
                    }
                    else if (param.AsInteger() != 0)
                    {
                        paramVal = param.AsInteger().ToString();
                    }
                    else if (param.AsDouble()!= 0)
                    {
                        paramVal = param.AsDouble().ToString();
                    }
                    else if (int.Parse(param.AsElementId().ToString())!= -1)
                    {
                        paramVal = param.AsElementId().ToString();
                    }
                    if (listOfParamVal.Contains(paramVal) == false)
                    {
                        listOfParamVal.Add(paramVal);
                    }
                }
                catch
                {

                }
            }
            return listOfParamVal;
        }
        public List<int> int_listOfParameterValues(Document doc, View v, string paramName)
        {
            List<int> listOfParamVal = new List<int>();
            FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
            foreach (Element e in myFEC)
            {
                try
                {
                    Parameter param = e.GetParameters(paramName).FirstOrDefault();
                    int paramVal = param.AsInteger();
                    if (param != null)
                    {

                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                }
                catch
                {

                }
            }
            return listOfParamVal;
        }
        public List<double> double_listOfParameterValues(Document doc, View v, string paramName)
        {
            List<double> listOfParamVal = new List<double>();
            FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
            foreach (Element e in myFEC)
            {
                try
                {
                    Parameter param = e.GetParameters(paramName).FirstOrDefault();
                    double paramVal = param.AsDouble();
                    if (param != null)
                    {

                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                }
                catch
                {

                }
            }
            return listOfParamVal;
        }
        public List<ElementId> elemId_listOfParameterValues(Document doc, View v, string paramName)
        {
            List<ElementId> listOfParamVal = new List<ElementId>();
            FilteredElementCollector myFEC = new FilteredElementCollector(doc, v.Id).WhereElementIsNotElementType();
            foreach (Element e in myFEC)
            {
                try
                {
                    Parameter param = e.GetParameters(paramName).FirstOrDefault();
                    ElementId paramVal = param.AsElementId();
                    if (param != null)
                    {

                        if (listOfParamVal.Contains(paramVal) == false)
                        {
                            listOfParamVal.Add(paramVal);
                        }
                    }
                }
                catch
                {

                }
            }
            return listOfParamVal;
        }
        public void addFiltersToView(Document doc, View v,  string parameterName, List<string> allParamValues)
        //Adds the parameter filters to a given view
        {
            #region Filters definitions and graphics
            BH_Parameter chosenParameter = new BH_Parameter(doc, parameterName);
            List<ElementId> catids = chosenParameter.applicableCategories;
            ElementId filledPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).FirstOrDefault().Id;  
            #endregion

            #region Creating filters
            #region Deleting old filters
            IList<Element> allFiltersinDoc = new FilteredElementCollector(doc).
                    OfClass(typeof(ParameterFilterElement)).ToElements();

                List<ElementId> filtersToBeDeletedIds = new List<ElementId>();
            for (var i = 0; i < allFiltersinDoc.Count(); i++)
            {
                Element pfe; // ParameterFilterElement
                pfe = doc.GetElement(allFiltersinDoc.ElementAt(i).Id);
                if (pfe != null && pfe.Name.StartsWith("zz_ParameterMachine"))
                {
                    filtersToBeDeletedIds.Add(pfe.Id);
                }
            }
            doc.Delete(filtersToBeDeletedIds);

            #endregion
            #region allColors
            List<Color> myColors = new List<Color>() {new Color(255, 235, 205),new Color(0, 0, 255),new Color(138, 43, 226),new Color(165, 42, 42),new Color(222, 184, 135),new Color(95, 158, 160),new Color(127, 255, 0),new Color(210, 105, 30),new Color(255, 127, 80),new Color(100, 149, 237),new Color(220, 20, 60),new Color(0, 255, 255),new Color(0, 0, 139),new Color(0, 139, 139),new Color(184, 134, 11),new Color(169, 169, 169),new Color(0, 100, 0),new Color(189, 183, 107),new Color(139, 0, 139),new Color(255, 140, 0),new Color(153, 50, 204),new Color(139, 0, 0),new Color(233, 150, 122),new Color(143, 188, 139),new Color(72, 61, 139),new Color(47, 79, 79),new Color(0, 206, 209),new Color(148, 0, 211),new Color(255, 20, 147),new Color(0, 191, 255),new Color(105, 105, 105),new Color(30, 144, 255),new Color(178, 34, 34),new Color(34, 139, 34),new Color(255, 0, 255),new Color(255, 215, 0),new Color(218, 165, 32),new Color(0, 128, 0),new Color(173, 255, 47),new Color(255, 105, 180),new Color(205, 92, 92),new Color(75, 0, 130),new Color(240, 230, 140),new Color(230, 230, 250),new Color(124, 252, 0),new Color(255, 250, 205),new Color(0, 255, 0),new Color(50, 205, 50),new Color(250, 240, 230),new Color(255, 0, 255),new Color(128, 0, 0),new Color(102, 205, 170),new Color(0, 0, 205),new Color(186, 85, 211),new Color(147, 112, 219),new Color(60, 179, 113),new Color(123, 104, 238),new Color(0, 250, 154),new Color(72, 209, 204),new Color(199, 21, 133),new Color(25, 25, 112),new Color(245, 255, 250),new Color(255, 228, 225),new Color(255, 228, 181),new Color(255, 222, 173),new Color(0, 0, 128),new Color(253, 245, 230),new Color(128, 128, 0),new Color(107, 142, 35),new Color(255, 165, 0),new Color(255, 69, 0),new Color(218, 112, 214),new Color(238, 232, 170),new Color(152, 251, 152),new Color(175, 238, 238),new Color(219, 112, 147),new Color(255, 239, 213),new Color(255, 218, 185),new Color(205, 133, 63),new Color(255, 192, 203),new Color(221, 160, 221),new Color(176, 224, 230),new Color(128, 0, 128),new Color(255, 0, 0),new Color(188, 143, 143),new Color(65, 105, 225),new Color(139, 69, 19),new Color(250, 128, 114),new Color(244, 164, 96),new Color(46, 139, 87),new Color(255, 245, 238),new Color(160, 82, 45),new Color(192, 192, 192),new Color(135, 206, 235),new Color(106, 90, 205),new Color(112, 128, 144),new Color(255, 250, 250),new Color(0, 255, 127),new Color(70, 130, 180),new Color(210, 180, 140),new Color(0, 128, 128),new Color(216, 191, 216),new Color(255, 99, 71),new Color(64, 224, 208),new Color(238, 130, 238),new Color(245, 222, 179),new Color(245, 245, 245),new Color(255, 255, 0),new Color(154, 205, 50)};
            if (false)
            {
                System.Drawing.Color auxColor = new System.Drawing.Color();
                foreach (System.Drawing.KnownColor knownColor in Enum.GetValues(typeof(System.Drawing.KnownColor)))
                {
                    auxColor = (System.Drawing.Color.FromKnownColor(knownColor));
                    myColors.Add(new Color(auxColor.R, auxColor.G, auxColor.B));
                }
            }

            //'#e6194b', '#3cb44b', '#ffe119', '#4363d8', '#f58231', '#911eb4', '#46f0f0', '#f032e6', '#bcf60c', '#fabebe', '#008080', '#e6beff', '#9a6324', '#fffac8', '#800000', '#aaffc3', '#808000', '#ffd8b1', '#000075', '#808080', '#ffffff', '#000000'
            #endregion
            #region creating new filters
            if (chosenParameter.paramFound)
            {
                int counter = 0;
                Color myVariableColor= new Color(0,0,0);
                ParameterFilterElement f0 = null;

                if (chosenParameter.isString)
                {
                    foreach (string paramVal in chosenParameter.listStrValues)
                    {
                        counter += 1;
                        try
                        {
                            myVariableColor = myColors[counter];
                        }
                        catch
                        {
                            throw new ArgumentException("Too many filters to be created: ( \nPlease use a view with fewer elements \n \n \n");
                        }
                        f0 = myStrFilter(doc, chosenParameter.paramValueProvider, "zz_ParameterMachine_" + parameterName + "_" + paramVal, catids, paramVal);

                        v.AddFilter(f0.Id);
                        overrideFilterGraphics(filter: f0,
                            view: v,
                            surfaceForegroundPatternId: filledPattern,
                            projectionFillColor: myVariableColor);
                    }
                }
                else if (chosenParameter.isInt)
                {
                    foreach (int intParamVal in chosenParameter.listIntValues)
                    {
                        counter += 1;
                        myVariableColor = myColors[counter];
                        f0 = myIntFilter(doc, chosenParameter.paramValueProvider, "zz_ParameterMachine_" + parameterName + "_" + intParamVal.ToString(), catids, intParamVal);

                        v.AddFilter(f0.Id);
                        overrideFilterGraphics(filter: f0,
                            view: v,
                            surfaceForegroundPatternId: filledPattern,
                            projectionFillColor: myVariableColor);
                    }
                }
                else if (chosenParameter.isDouble)
                {
                    foreach (double doubleParamVal in chosenParameter.listDblValues)
                    {
                        counter += 1;
                        myVariableColor = myColors[counter];
                        f0 = myDoubleFilter(doc, chosenParameter.paramValueProvider, "zz_ParameterMachine_" + parameterName + "_" + doubleParamVal.ToString(), catids, doubleParamVal);

                        v.AddFilter(f0.Id);
                        overrideFilterGraphics(filter: f0,
                            view: v,
                            surfaceForegroundPatternId: filledPattern,
                            projectionFillColor: myVariableColor);
                    }
                }
                else if (chosenParameter.isElementId)
                {
                    foreach (ElementId elemIdParamVal in chosenParameter.listEIdValues)
                    {
                        counter += 1;
                        myVariableColor = myColors[counter];
                        f0 = myElemIdFilter(doc, chosenParameter.paramValueProvider, "zz_ParameterMachine_" + parameterName + "_" + elemIdParamVal.IntegerValue.ToString(), catids, elemIdParamVal);

                        v.AddFilter(f0.Id);
                        overrideFilterGraphics(filter: f0,
                            view: v,
                            surfaceForegroundPatternId: filledPattern,
                            projectionFillColor: myVariableColor);
                    }
                }
                else { }
                
                ParameterFilterElement fSansDesignation = myStrFilter(doc, chosenParameter.paramValueProvider, "zz_ParameterMachine_" + parameterName + "_" + "SansValeurs", catids, "");
            v.AddFilter(fSansDesignation.Id);
            overrideFilterGraphics(filter: fSansDesignation,
                view: v,
                surfaceForegroundPatternId: filledPattern,
                projectionFillColor: new Color(192, 192, 192));
                //projectionLineColor: new Color(0, 0, 0),
                //cutForegroundPatternId: filledPattern,
                //cutForegroundPatternColor: new Color(192, 192, 192),
                //cutLineColor: new Color(0, 0, 0));//new Color(120, 120, 120));
            }

            #endregion
            #endregion
        }
        public void highlightChosenFilter(Document doc,View v, string filterName)
        {

            List<ElementId> allViewFilters = getViewFilters(doc);
            foreach (ElementId fId in allViewFilters)
            {
                if (doc.GetElement(fId).Name.EndsWith(filterName) == false)
                {
                    doc.Delete(fId);
                }
            }

        }
        #endregion


        #endregion

        public void parameterMachine(Document doc, List<string> projectParams)
        // Create new / update / remove TimeMachine filters
        {
            // Run UI
            Application.EnableVisualStyles();
            Form1 parameterMachine = new Form1();
            parameterMachine.fillListBox(projectParams);
            parameterMachine.setProjectParameters(projectParams);
            parameterMachine.fillListBox2(getViewFilterValues(doc));
            Application.Run(parameterMachine); // Run UI TimeMachine form
            // User results
            bool generateFilter = parameterMachine.generateFilter;
            if (generateFilter) //Highlight 1 filter
            {
                highlightChosenFilter(doc,doc.ActiveView,parameterMachine.filterToGenerate);
            }
            else //Generate all filters
            {
                string paramName = parameterMachine.chosenParameter; // Information from UI ParameterMachine
                List<string> allParamValues = str_listOfParameterValues(doc, doc.ActiveView, paramName);
                if (paramName != "")
                {
                    addFiltersToView(doc, doc.ActiveView,paramName, allParamValues);
                }
            }

        }

        public List<string> getViewFilterValues(Document doc)
        {
            List<string> viewParamValues = new List<string>();
            foreach (ElementId filterId in doc.ActiveView.GetFilters())
            {
                if (doc.GetElement(filterId).Name.StartsWith("zz_ParameterMachine"))
                {
                    viewParamValues.Add(doc.GetElement(filterId).Name.Remove(0,20));//.Split('_').LastOrDefault());
                }
            }
            return viewParamValues;
        }

        public List<ElementId> getViewFilters(Document doc)
        {
            List<ElementId> viewFilters = new List<ElementId>();
            foreach (ElementId filterId in doc.ActiveView.GetFilters())
            {
                if (doc.GetElement(filterId).Name.StartsWith("zz_ParameterMachine"))
                {
                    viewFilters.Add(filterId) ;
                }
            }
            return viewFilters;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            #region API definitions
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            #endregion
            #region Filter categories
            // Filter categories for the TimeMachine generated filters
            /*List<ElementId> catids = new List<ElementId> {
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
            new ElementId(BuiltInCategory.OST_SpecialityEquipment),
            new ElementId(BuiltInCategory.OST_Mass)
        };
            List<ElementId> catids_2 = new List<ElementId> {
            new ElementId(BuiltInCategory.OST_Parts)

        };*/
            #endregion

            #region Getting list of filters in view
            List<string> projectParams = listOfParameters(doc);

            Transaction tran = new Transaction(doc);
            tran.Start("parameter Machine");
            parameterMachine(doc, projectParams);
            tran.Commit();

            #endregion

            return Result.Succeeded;
        }
    }
}



#region UI for Parameter Machine

namespace BH_form_ParameterMachine
{
    partial class Form1
    {
        public string chosenParameter;
        public bool generateFilter;
        public string filterToGenerate;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.LightGray;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(26, 95);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(338, 251);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(338, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightGray;
            this.button1.Location = new System.Drawing.Point(178, 352);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 56);
            this.button1.TabIndex = 1;
            this.button1.Text = "Generate Filters";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox2
            // 
            this.listBox2.BackColor = System.Drawing.Color.LightGray;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(402, 95);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(338, 251);
            this.listBox2.TabIndex = 2;
            this.listBox2.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LightGray;
            this.button2.Location = new System.Drawing.Point(554, 352);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 56);
            this.button2.TabIndex = 3;
            this.button2.Text = "Highlight selected filter";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(28, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Step 1: Chose parameter to highlight its values";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label2.Location = new System.Drawing.Point(399, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(269, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Step 2: Highlight only a specific values of the parameter";
            this.label2.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(62)))), ((int)(((byte)(68)))));
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Gainsboro;
            this.button3.Location = new System.Drawing.Point(370, 95);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(22, 250);
            this.button3.TabIndex = 6;
            this.button3.Text = ">";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(62)))), ((int)(((byte)(68)))));
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Basler&Hofmann ParameterMachine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
    }
}


namespace BH_form_ParameterMachine
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        List<string> projectParameters = new List<string>();// { "List1", "List2", "List3", "List4" };

        string logFolderPath = @"C:\AppData\BH_Plugins\";
        string fileName_parameterMachine = "ParameterMachineLog_UserChoices.txt";

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

        public void readLog(string logFolderPath, string fileName_parameterMachine)
        {
            VerifyDir(logFolderPath);
            if (!System.IO.File.Exists(logFolderPath + fileName_parameterMachine))
            {
                return;
            }
            else
            {
                System.Collections.Generic.IEnumerable<string> log = System.IO.File.ReadLines(logFolderPath + fileName_parameterMachine);

                string userChoice = (log.ElementAt(0));

                #region set the list initially
                textBox1.Text = (userChoice);
                if (string.IsNullOrEmpty(userChoice) == false)
                {
                    listBox1.Items.Clear();
                    foreach (string item in projectParameters)
                    {
                        if (item.ToLower().Contains(userChoice.ToLower()))
                        {
                            listBox1.Items.Add(item);
                        }
                    }
                }
                else
                {
                    fillListBox(projectParameters);
                }

                return;
                #endregion
            }

        }
        public void writeLog(string logFolderPath, string fileName_parameterMachine)
        {
            string[] contentss = new string[]
            {
                textBox1.Text //intervalle
            };
            VerifyDir(logFolderPath);
            bool fileExists = System.IO.File.Exists(logFolderPath + fileName_parameterMachine);
            if (fileExists == false)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(logFolderPath + fileName_parameterMachine, true);
                //file.WriteLine(imageFolderBrowserDialog.SelectedPath);
                file.Close();
                //return;
            }
            System.IO.File.WriteAllLines(logFolderPath + fileName_parameterMachine, contentss);
            return; //System.IO.File.ReadLines(logFolderPath + fileName_timeMachine).First();
        }

        public void setProjectParameters(List<string> projParams)
        {
            projectParameters.Clear();
            projectParameters = projParams;
            readLog(logFolderPath, fileName_parameterMachine);

        }
        public void fillViewFilterValues(List<string> viewFilterValues)
        {
            listBox2.Items.Clear();
            foreach (string vfv in viewFilterValues)
            {
                listBox2.Items.Add(vfv);
            }
        }

        public Form1()
        {
            InitializeComponent();
            fillListBox(projectParameters);
            readLog(logFolderPath, fileName_parameterMachine);
            this.Width = 550;
            this.Height = 550;
        }

        public void fillListBox(List<string> projectParameters)
        {
            foreach (string parameterName in projectParameters)
            {
                listBox1.Items.Add(parameterName);
            }
        }
        public void fillListBox2(List<string> projectParameters)
        {
            foreach (string parameterName in projectParameters)
            {
                listBox2.Items.Add(parameterName);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            if (string.IsNullOrEmpty(textBox1.Text) == false)
            {
                foreach (string item in projectParameters)
                {
                    if (item.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox1.Items.Add(item);
                    }
                }
            }
            else
            {
                fillListBox(projectParameters);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex != -1))
            {
                chosenParameter = listBox1.SelectedItem.ToString();
                writeLog(logFolderPath, fileName_parameterMachine);
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((listBox2.SelectedIndex != -1))
            {
                generateFilter = true;
                filterToGenerate = listBox2.SelectedItem.ToString();
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Width = 1100;
            this.Height = 550;
            listBox2.Show();
            label2.Show();
            button2.Show();


        }
        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            listBox2.Hide();
            label2.Hide();
            button2.Hide();
            foreach (string paramValue in listBox2.Items)
            {
                if (paramValue.Contains(listBox1.SelectedItem.ToString()))
                {
                    this.Width = 1100;
                    this.Height = 550;
                    listBox2.Show();
                    label2.Show();
                    button2.Show();
                    break;
                }
            }
        }
    }
}



#endregion
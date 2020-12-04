/*
QR code creator for Revit
Created on 16.11.2020
By Basler&Hofmann SA (https://www.baslerhofmann.ch/)
@author: Mohamed Nadeem
*/

using System;
using System.Collections.Generic;
using System.Linq;

using QRCoder;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Github_QRCode
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public string qrCodeText(Document doc, Element elem)
        // Get QR Code text from Element location (shared coordinates)
        // QR Code text format: E2547858580N1122112220Z0426900
        // For E= 2547858.58, N = 1122112.22, Z=426.9
        //E##########N##########Z###### East(10)+North(10)+Elevation(7)
        {
            #region Variables definitions
            //Project base points
            var basePoints = new FilteredElementCollector(doc).OfClass(typeof(BasePoint)).ToElements();
            var projectPoint = basePoints.First(
                x => (x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ProjectBasePoint));
            //Base point main parameter values
            double ns = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_NORTHSOUTH_PARAM).AsDouble();
            double ew = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_EASTWEST_PARAM).AsDouble();
            double ele = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_ELEVATION_PARAM).AsDouble();
            double rot = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_ANGLETON_PARAM).AsDouble();
            ProjectPosition projPos = (doc).ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            XYZ iop = new XYZ(projPos.EastWest, projPos.NorthSouth, 0);
            XYZ pbp = new XYZ(ew, ns, ele);
            XYZ lcP= new XYZ(0,0,0);
            //Other definitions
            double ft2m = 0.3048; // for unit conversion
            string xCoord, yCoord, zCoord;
            string qrTextOut;
            double z0;
            Transform rotationTransform1;
            #endregion

            #region QR Code information
            //Coordinate conversion
            rotationTransform1 =
                Transform.CreateRotationAtPoint(XYZ.BasisZ, (rot * -1), iop);
            //Get element location
            if (null != (elem.Location as LocationPoint))
            {
                lcP = (elem.Location as LocationPoint).Point;
            } else if (null != (elem.Location as LocationCurve))
            {
                Curve lcC = (elem.Location as LocationCurve).Curve;
                lcP = (lcC.GetEndPoint(0)+(lcC.GetEndPoint(1))).Multiply(0.5); // curve midpoint
            }
            else
            {
                return "No valid location found for element number: "+elem.Id.IntegerValue;
            }
            lcP = new XYZ(lcP.X + iop.X, lcP.Y + iop.Y, lcP.Z + iop.Z);
            lcP = rotationTransform1.OfPoint(lcP);
            lcP = new XYZ(lcP.X, lcP.Y, lcP.Z + pbp.Z);
            lcP = new XYZ(lcP.X * ft2m, lcP.Y * ft2m, lcP.Z * ft2m);
            //Shared coordinates of point
            xCoord = Math.Round(lcP.X, 3).ToString();
            yCoord = Math.Round(lcP.Y, 3).ToString();
            zCoord = Math.Round(lcP.Z, 3).ToString();
            z0 = Math.Round(lcP.Z, 3);
            //QR Code text

            if (z0 < 10)// z coordinate should have exactly 7 digits ex z=5.203 becomes 0005203
            {
                zCoord = "000" + z0;
            }
            else if (z0 < 100)
            {
                zCoord = "00" + z0;
            }
            else if (z0 < 1000)
            {
                zCoord = "0" + z0;
            }
            qrTextOut = "E" + xCoord + "N" + yCoord + "Z" + zCoord;
            qrTextOut = qrTextOut.Replace(",", "");
            qrTextOut = qrTextOut.Replace(".", "");
            #endregion

            return qrTextOut;
        }

        public void createQRCode(string imageText, string imagePath)
        // Create a QR Code image and save it in a given folder
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData QR_data = generator.CreateQrCode(imageText, QRCodeGenerator.ECCLevel.H);
            QRCode QR_code = new QRCode(QR_data);
            System.Drawing.Bitmap qrCodeImage = QR_code.GetGraphic(2);
            qrCodeImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public string VerifyDir(string path)
        // Create a folder on desktop
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
            }
            return path;
        }
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //Main function
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string imagesFolderPath = VerifyDir(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + (@"\BH_QRcode_Images"));
            string myImagePath;

            IList<Element> pickedElements = uidoc.Selection.PickElementsByRectangle("Select elements for which you want to create QR Codes");

            foreach (Element elem in pickedElements)
            {
                myImagePath = imagesFolderPath + (@"\QR_") + elem.Id.ToString() + ".bmp";
                createQRCode(qrCodeText(doc, doc.GetElement(elem.Id)), myImagePath);
            }
            TaskDialog.Show("Baslerhofmann", pickedElements.Count + " QR Code images created.");
            return Result.Succeeded;
        }
    }
}

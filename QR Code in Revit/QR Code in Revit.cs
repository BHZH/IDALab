/*
QR code creator for Revit
Created on 13.11.2020
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
        public string encodeCoordinates(XYZ inCoordinates)
        {
            double factor = 1000;

            //Shared coordinates of point
            int xCoord = (int)Math.Round(inCoordinates.X * factor, 0);
            int yCoord = (int)Math.Round(inCoordinates.Y * factor, 0);
            int zCoord = (int)Math.Round(inCoordinates.Z * factor, 0);
            string E = xCoord.ToString().PadLeft(7, '0');
            string N = yCoord.ToString().PadLeft(7, '0');
            string Z = zCoord.ToString().PadLeft(7, '0');

            return "E" + E + "N" + N + "Z" + Z;
        }

        public string qrCodeText(Document doc, Element elem, XYZ internalOrigin, double internalRotation)
        // Get QR Code text from Element location (shared coordinates)
        // QR Code text format: E2547858580N1122112220Z0426900
        // For E= 2547858.58, N = 1122112.22, Z=426.9
        //E##########N##########Z###### East(10)+North(10)+Elevation(7)
        {
            #region Variables definitions
            XYZ locationPoint;
            //Other definitions
            double ft2m = 0.3048; // for unit conversion
            string qrTextOut;
            Transform rotationTransform1;
            #endregion

            #region QR Code information
            //Coordinate conversion
            rotationTransform1 =
                Transform.CreateRotationAtPoint(XYZ.BasisZ, (internalRotation * -1), internalOrigin);
            //Get element location
            if (null != (elem.Location as LocationPoint))
            {
                locationPoint = (elem.Location as LocationPoint).Point;
            } else if (null != (elem.Location as LocationCurve))
            {
                Curve lcC = (elem.Location as LocationCurve).Curve;
                locationPoint = (lcC.GetEndPoint(0)+(lcC.GetEndPoint(1))).Multiply(0.5); // curve midpoint
            }
            else
            {
                return "No valid location found for element number: "+elem.Id.IntegerValue;
            }
            locationPoint = locationPoint + internalOrigin;
            locationPoint = rotationTransform1.OfPoint(locationPoint);
            locationPoint.Multiply(ft2m);

            qrTextOut = encodeCoordinates(locationPoint);
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

            //Project base points
            var projectPoint = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectBasePoint).ToElements().FirstOrDefault();
            double internalRotation = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_ANGLETON_PARAM).AsDouble();
            ProjectPosition projPos = (doc).ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            XYZ internalOrigin = new XYZ(projPos.EastWest, projPos.NorthSouth, projPos.Elevation);

            string imagesFolderPath = VerifyDir(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + (@"\QRcode_Images"));
            string myImagePath;

            IList<Element> pickedElements = uidoc.Selection.PickElementsByRectangle("Select elements for which you want to create QR Codes");

            foreach (Element elem in pickedElements)
            {
                myImagePath = imagesFolderPath + (@"\QR_") + elem.Id.ToString() + ".bmp";
                createQRCode(qrCodeText(doc, doc.GetElement(elem.Id), internalOrigin, internalRotation), myImagePath);
            }
            TaskDialog.Show("Baslerhofmann", pickedElements.Count + " QR Code images created.");
            return Result.Succeeded;
        }
    }
}

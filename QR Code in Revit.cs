using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using QRCoder;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace Github_QRCode
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public string qrCodeText(UIDocument uidoc, Element elem)
        //Get QR Code text from Element location (shared coordinates)
        //QR Code text format: E##########N##########Z######
        // East(10)+North(10)+Elevation(6)
        {
            Document doc = uidoc.Document;
            Options opt = new Options();

            //ft to m
            double ft2m = 0.3048;

            //Project base points
            var basePoints = new FilteredElementCollector(uidoc.Document).OfClass(typeof(BasePoint)).ToElements();
            var projectPoint = basePoints.First(
                x => (x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ProjectBasePoint));
            //Base point main parameter values
            double ns = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_NORTHSOUTH_PARAM).AsDouble();
            double ew = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_EASTWEST_PARAM).AsDouble();
            double ele = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_ELEVATION_PARAM).AsDouble();
            double rot = projectPoint.get_Parameter(BuiltInParameter.BASEPOINT_ANGLETON_PARAM).AsDouble();
            ProjectPosition projPos = (uidoc.Document).ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            XYZ iop = new XYZ(projPos.EastWest, projPos.NorthSouth, 0);
            XYZ pbp = new XYZ(ew, ns, ele);
            XYZ lcP;

            //Coordinate conversion
            Transform rotationTransform1 =
                Transform.CreateRotationAtPoint(XYZ.BasisZ, (rot * -1), iop);
            lcP = (elem.Location as LocationPoint).Point;
            lcP = new XYZ(lcP.X + iop.X, lcP.Y + iop.Y, lcP.Z + iop.Z);
            lcP = rotationTransform1.OfPoint(lcP);
            lcP = new XYZ(lcP.X, lcP.Y, lcP.Z + pbp.Z);
            lcP = new XYZ(lcP.X * ft2m, lcP.Y * ft2m, lcP.Z * ft2m);
            //Shared coordinates of point
            string xCoord, yCoord, zCoord;
            xCoord = Math.Round(lcP.X, 3).ToString();
            yCoord = Math.Round(lcP.Y, 3).ToString();
            zCoord = Math.Round(lcP.Y, 3).ToString();
            double z0 = Math.Round(lcP.Z, 3);
            //QR Code text
            if (z0 < 10)
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
            string qrTextOut = "E" + xCoord + "N" + yCoord + "Z" + zCoord;
            qrTextOut = qrTextOut.Replace(",", "");
            qrTextOut = qrTextOut.Replace(".", "");
            return qrTextOut;
        }
        public void createQRCode(string imageText, string imagePath)
        // Create a QR Code image and save it in a folder
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData QR_data = generator.CreateQrCode(imageText, QRCodeGenerator.ECCLevel.H);
            QRCode QR_code = new QRCode(QR_data);
            System.Drawing.Bitmap qrCodeImage = QR_code.GetGraphic(2);
            qrCodeImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            FilteredElementCollector myFEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType();
            string imagesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + (@"\BH_QRcode_Images");
            string myImagePath;

            foreach (Element elem in myFEC)
            {
                myImagePath = imagesFolderPath + (@"\QR_") + elem.Id.ToString() + ".bmp";
                createQRCode(qrCodeText(uidoc, doc.GetElement(elem.Id)), myImagePath);
            }
            return Result.Succeeded;
        }
    }
}

// (C) Copyright 2025 by  
//jhm
using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
//using Autodesk.Civil.Runtime;
//using Autodesk.AutoCAD.Internal;

//todo: trim out any/all unnecessary using statements, not that it particularly matters

[assembly: CommandClass(typeof(jhm_utils.MyCommands))]

namespace jhm_utils
{
    public class MyCommands
    {
        [CommandMethod("xcont")]
        public void ExplodeContourLabel()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            using (Transaction ts = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                PromptEntityOptions opt = new PromptEntityOptions("\nSelect contour label(s)");
                //todo: allow this to select multiple objects, reject if it's not of the type? or maybe just leave as-is?
                opt.SetRejectMessage("\nObject must be a contour label.\n");
                opt.AddAllowedClass(typeof(SurfaceContourLabelGroup), false);
                ObjectId contID = ed.GetEntity(opt).ObjectId;

                SurfaceContourLabelGroup oldLbl = ts.GetObject(contID, OpenMode.ForWrite) as SurfaceContourLabelGroup;

                //set all the necessary reference parameters so we can make the new labels have the same properties
                ObjectId surf = oldLbl.FeatureId;
                Boolean isMajVis = oldLbl.IsMajorContourLabelVisible;
                Boolean isMinVis = oldLbl.IsMinorContourLabelVisible;
                Boolean isUsrVis = oldLbl.IsUserContourLabelVisible;
                Boolean isLabVis = oldLbl.IsLabelLineVisible;
                ObjectId majSty = oldLbl.MajorContourLabelStyleId;
                ObjectId minSty = oldLbl.MinorContourLabelStyleId;
                ObjectId usrSty = oldLbl.UserContourLabelStyleId;
                LabelMaskType maskType = oldLbl.Mask;
                Point2dCollection pts = oldLbl.LabelLinePoints;
                Int32 num = pts.Count;

                //iterate through the Point2dCollection from the original label and make a new label for each pair of sequential points
                //essentially, take a polyline and make a new line for each of the original segments. then delete the original poyline
                for (int i = 0; i < num - 1; i++)
                {
                    ObjectId iObj = SurfaceContourLabelGroup.Create(surf, new Point2dCollection { pts[i], pts[i + 1] }, majSty, minSty, usrSty);
                    SurfaceContourLabelGroup newLbl = iObj.GetObject(OpenMode.ForWrite) as SurfaceContourLabelGroup;
                    newLbl.IsMajorContourLabelVisible = isMajVis;
                    newLbl.IsMinorContourLabelVisible = isMinVis;
                    newLbl.IsUserContourLabelVisible = isUsrVis;
                    newLbl.IsLabelLineVisible = isLabVis;
                    newLbl.Mask = maskType;
                }
                oldLbl.Erase();
                ts.Commit();
            }
            ed.WriteMessage("\nContour Label Exploded. Probably.\n");
        }
    }
}
//imagine implementing error handling, lol.
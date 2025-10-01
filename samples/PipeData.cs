using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Linq.Expressions;

[assembly: CommandClass(typeof(PipeData.PipeExport))]

namespace PipeData {
	public class PipeExport : IExtensionApplication {
        void IExtensionApplication.Initialize()
        {
            // Add one time initialization here
            // One common scenario is to setup a callback function here that 
            // unmanaged code can call. 
            // To do this:
            // 1. Export a function from unmanaged code that takes a function
            //    pointer and stores the passed in value in a global variable.
            // 2. Call this exported function in this function passing delegate.
            // 3. When unmanaged code needs the services of this managed module
            //    you simply call acrxLoadApp() and by the time acrxLoadApp 
            //    returns  global function pointer is initialized to point to
            //    the C# delegate.
            // For more info see: 
            // http://msdn2.microsoft.com/en-US/library/5zwkzwf4(VS.80).aspx
            // http://msdn2.microsoft.com/en-us/library/44ey4b32(VS.80).aspx
            // http://msdn2.microsoft.com/en-US/library/7esfatk4.aspx
            // as well as some of the existing AutoCAD managed apps.

            // Initialize your plug-in application here
        }
        void IExtensionApplication.Terminate()
        {
            // Do plug-in application clean up here
        }

        [CommandMethod("ExportPipeData")]

		public void ExportPipeData()
		{
			Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
			CivilDocument doc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

			//check if pipe networks exist here
			if (doc.GetPipeNetworkIds() == null) {
				ed.WriteMessage("There are no pipe networks in this drawing.");
				return;
			}

			//iterate through each pipe network in drawing
			using (Transaction ts = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction()) {

				try {
					ObjectIdCollection oNetworkIds = doc.GetPipeNetworkIds();
					//int networkCount = oNetworkIds.Count;

					foreach (ObjectId oid_network in oNetworkIds) {
						Network oNetwork = ts.GetObject(oid_network, OpenMode.ForRead) as Network;
						ed.WriteMessage("Pipe Network: " + oNetwork.Name + "\n");

						ObjectIdCollection oPipeIds = oNetwork.GetPipeIds();

						if (oPipeIds.Count == 0) { //unsure .count == 0 versus == null..
							ed.WriteMessage(oNetwork.DisplayName + " has no pipes.");
							return;
						}
						else {

							foreach (ObjectId oid_pipe in oPipeIds) {
								Pipe oPipe = ts.GetObject(oid_pipe, OpenMode.ForRead) as Pipe;
								ed.WriteMessage(oPipe.Name + ", " + oPipe.Description + "\n");
								ed.WriteMessage(oPipe.Length2D.ToString("n0") + " LF, " + "@ SLOPE " + oPipe.Slope.ToString("n1") + "%" + "\n");
							}
						}

						ObjectIdCollection oStructureIds = oNetwork.GetStructureIds();

						if (oStructureIds.Count == 0) {
							ed.WriteMessage(oNetwork.DisplayName + " has no structures.");
							return;
						}
						else
						{
							foreach (ObjectId oid_structure in oStructureIds)
							{
								Structure oStructure = ts.GetObject(oid_structure, OpenMode.ForRead) as Structure;
								ed.WriteMessage(oStructure.Name + ", " + oStructure.Description + "\n");
								ed.WriteMessage("TOP ELEV = " + oStructure.RimElevation.ToString("n2") + "'\n");
                                ed.WriteMessage("Connected Pipes: \n");
                                foreach (string str_pipename in oStructure.GetConnectedPipeNames()) {
                                    ed.WriteMessage("     " + str_pipename + "\n");
                                }
							}
						}
						ed.WriteMessage("\n");
					}
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ex) {
					ed.WriteMessage("tell jhm: " + ex.Message);
					return;
				}
			} //don't need tr.Commit() because we're not writing anything to the drawing yet. Yet...
		}
	}
}
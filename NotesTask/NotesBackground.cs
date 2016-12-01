using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotesTask
{
    public sealed class NotesBackground : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Support for async task executions. 
            var deferral = taskInstance.GetDeferral();

            UpdateTile();
            deferral.Complete();
        }

        private void UpdateTile()
        {
            var tileDocument = new XmlDocument();
            var xml = @"<tile> " +
                      "<visual version=\"2\">" +
                      "<binding template=\"TileSquare150x150PeekImageAndText01\" fallback=\"TileSquarePeekImageAndText01\">" +
                      "<image id=\"1\" src=\"ms-appx:///Assets/Square150x150Logo.scale-240.png\"/>" +
                      "<text id=\"1\">There is a new Note</text>" +
                      "<text id=\"2\">Click here to access new note!</text>" +
                      " </binding>" +
                      " </visual>" +
                      "</tile>";

            tileDocument.LoadXml(xml);

            var tile = new TileNotification(tileDocument);

            var updater = TileUpdateManager
                .CreateTileUpdaterForApplication();

            updater.Update(tile);
        }

    }
}

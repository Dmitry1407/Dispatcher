using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Core.model.core.archive
{
    public class EventsArchive : Archive
    {
        public EventsArchive()
        {
            Type = ArchiveType.Events;
            dbPath = "D:\\Temp\\Events.db";

            connection = new SQLiteConnection();
            command = new SQLiteCommand();
        }

        protected override void CreateTable()
        {
            command.Connection = connection;
            command.CommandText = "CREATE TABLE IF NOT EXISTS Events (id INTEGER PRIMARY KEY AUTOINCREMENT, channelID INT, eventValue TEXT, timeStamp INTEGER)";
            command.ExecuteNonQuery(); // Query Execute
        }

        public override void Insert(Int32 channelID, String eventValue, Int64 timeStamp)
        {
            command.CommandText = "INSERT INTO Events ('channelID', 'eventValue', 'timeStamp') VALUES(@channelID, @eventValue, @timeStamp)";
            // Insert Parameters
            command.Parameters.AddWithValue("@channelID", channelID);
            command.Parameters.AddWithValue("@eventValue", eventValue);
            command.Parameters.AddWithValue("@timeStamp", timeStamp);
            command.ExecuteNonQuery();
        }

        public override List<ArchiveItem> Read(Int32 channelID)
        {
            List<ArchiveItem> items = new List<ArchiveItem>();

            lock (locker)
            {
                command.CommandText = "SELECT * FROM Events WHERE channelID = " + channelID;
                SQLiteDataReader sqlReader = command.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {   // Read Row
                        EventsArchiveItem item = ReadRow(sqlReader);
                        items.Add(item);
                    }
                }
                sqlReader.Close();
            }
            return items;
        }

        public override List<ArchiveItem> ReadAll()
        {
            List<ArchiveItem> items = new List<ArchiveItem>();

            lock (locker)
            {
                command.CommandText = "SELECT * FROM Events";
                SQLiteDataReader sqlReader = command.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {   // Read Row
                        EventsArchiveItem item = ReadRow(sqlReader);
                        items.Add(item);
                    }
                }
                sqlReader.Close();
            }
            return items;
        }

        private EventsArchiveItem ReadRow(SQLiteDataReader sqlReader)
        {
            EventsArchiveItem item = new EventsArchiveItem();

            item.ChannelID = (Int32)sqlReader["channelID"];
            item.EventValue = (String)sqlReader["eventValue"];
            item.TimeStamp = (Int64)sqlReader["timeStamp"];

            return item;
        }
    }
}

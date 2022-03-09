using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Core.model.core.archive
{
    public class ChannelsArchive : Archive
    {
        public ChannelsArchive()
        {
            Type = ArchiveType.Channels;
            dbPath = "D:\\Temp\\Channels.db";

            connection = new SQLiteConnection();
            command = new SQLiteCommand();
        }

        protected override void CreateTable()
        {
            command.Connection = connection;
            command.CommandText = "CREATE TABLE IF NOT EXISTS Channels (id INTEGER PRIMARY KEY AUTOINCREMENT, channelID INT, channelValue TEXT, timeStamp INTEGER)";
            command.ExecuteNonQuery(); // Query Execute
        }

        public override void Insert(Int32 channelID, String channelValue, Int64 timeStamp)
        {
            lock (locker)
            {
                command.CommandText = "INSERT INTO Channels ('channelID', 'channelValue', 'timeStamp') VALUES(@channelID, @channelValue, @timeStamp)";
                // Insert Parameters
                command.Parameters.AddWithValue("@channelID", channelID);
                command.Parameters.AddWithValue("@channelValue", channelValue);
                command.Parameters.AddWithValue("@timeStamp", timeStamp);
                command.ExecuteNonQuery();
            }
        }

        public override List<ArchiveItem> Read(Int32 channelID)
        {
            List<ArchiveItem> items = new List<ArchiveItem>();

            lock (locker)
            {
                command.CommandText = "SELECT * FROM Channels WHERE channelID = " + channelID;
                SQLiteDataReader sqlReader = command.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {   // Read Row
                        ChannelsArchiveItem item = ReadRow(sqlReader);
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
                command.CommandText = "SELECT * FROM Channels";
                SQLiteDataReader sqlReader = command.ExecuteReader();
                if (sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {   // Read Row
                        ChannelsArchiveItem item = ReadRow(sqlReader);
                        items.Add(item);
                    }
                }
                sqlReader.Close();
            }
            return items;
        }

        private ChannelsArchiveItem ReadRow(SQLiteDataReader sqlReader)
        {
            ChannelsArchiveItem item = new ChannelsArchiveItem();

            item.ChannelID = (Int32)sqlReader["channelID"];
            item.ChannelValue = (String)sqlReader["channelValue"];
            item.TimeStamp = (Int64)sqlReader["timeStamp"];

            return item;
        }
    }
}
